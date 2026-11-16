using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using FOS.DataLayer;
using FOS.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Diagnostics.Logging;

namespace FOS.Web.UI.Controllers
{
    public class AskAIController : Controller
    {
        private readonly FOSDataModel db = new FOSDataModel();

        // Users.RegionalheadRef - the same identity lookup every other head-scoped
        // feature in this app uses (Setup/ManageRegionalHead.cs, ManageRegion.cs,
        // ManageSaleOffice.cs, ManageCity.cs). NOT AdminPanelController's
        // GetRegionalHeadIDRelatedToUser()/Session["RegionalHeads"] - that reads the
        // Users<->RegionalHeads junction table, which nothing in this codebase ever
        // writes to, so it's always 0 for real accounts.
        private int CurrentRegionalHeadID()
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            return FOS.Setup.ManageRegionalHead.GetRegionalHeadRefForUser(userID);
        }

        public ActionResult Index()
        {
            if (CurrentRegionalHeadID() == 0)
            {
                return RedirectToAction("Login", "AdminPanel");
            }
            return View();
        }

        [HttpPost]
        public JsonResult Ask(string question)
        {
            int headID = CurrentRegionalHeadID();
            if (headID == 0)
            {
                return Json(new { narrative = "You're not set up as a Regional Head, so this screen isn't available to you." });
            }
            if (string.IsNullOrWhiteSpace(question))
            {
                return Json(new { narrative = "Ask a question about Visits, Trade Visits, Claims or Complaints." });
            }
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["ClaudeApiKey"]))
            {
                return Json(new { narrative = "AI isn't configured yet - the ClaudeApiKey setting is missing or empty in the live Web.config." });
            }

            DateTime start, end;
            GetDateRange(question, out start, out end);

            var domains = ClassifyDomains(question);

            var summary = new JObject();
            if (domains.Contains("Claims"))
            {
                summary["claims"] = ToJObject(GetClaimsSummary(headID, start, end));
            }
            if (domains.Contains("Complaints"))
            {
                summary["complaints"] = ToJObject(GetComplaintsSummary(headID, start, end));
            }
            if (domains.Contains("Visits") || domains.Contains("TradeVisits") || domains.Contains("Attendance"))
            {
                var team = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(headID, true);
                var soIds = team.Select(s => s.ID).ToList();
                var soNames = team.ToDictionary(s => s.ID, s => s.Name);

                if (domains.Contains("Visits"))
                {
                    summary["housingVisits"] = ToJObject(GetHousingVisitsSummary(soIds, soNames, start, end));
                }
                if (domains.Contains("TradeVisits"))
                {
                    summary["tradeVisits"] = ToJObject(GetTradeVisitsSummary(soIds, soNames, start, end));
                }
                if (domains.Contains("Attendance"))
                {
                    summary["attendance"] = ToJObject(GetAttendanceSummary(soIds, soNames, start, end));
                }
            }

            if (!summary.Properties().Any())
            {
                return Json(new
                {
                    numbers = summary,
                    narrative = "I couldn't match that to Visits, Trade Visits, Claims, Complaints or Attendance - try asking about one of those."
                });
            }

            string narrative = Narrate(question, summary, start, end);

            return Json(new { numbers = summary, narrative = narrative });
        }

        // Haiku only picks a label - the actual date math always happens here in C#, never the model.
        private void GetDateRange(string question, out DateTime start, out DateTime end)
        {
            string prompt = "Extract the date range this question is asking about. Reply with EXACTLY one of: " +
                "today, yesterday, this_week, last_week, this_month, last_month - or a specific date in YYYY-MM-DD " +
                "format if one is mentioned. If no date is mentioned, reply exactly: today. " +
                "Reply with ONLY that value, nothing else.\n\nQuestion: " + question;

            string reply = CallClaude(prompt, 15);
            ResolveDateRange(reply, out start, out end);
        }

        // start is inclusive, end is exclusive (same convention as every domain query below).
        private void ResolveDateRange(string label, out DateTime start, out DateTime end)
        {
            DateTime today = DateTime.Today;
            label = (label ?? "").Trim().ToLowerInvariant();

            int daysSinceMonday = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime thisWeekStart = today.AddDays(-daysSinceMonday);
            DateTime thisMonthStart = new DateTime(today.Year, today.Month, 1);

            switch (label)
            {
                case "yesterday":
                    start = today.AddDays(-1);
                    end = today;
                    return;
                case "this_week":
                    start = thisWeekStart;
                    end = today.AddDays(1);
                    return;
                case "last_week":
                    start = thisWeekStart.AddDays(-7);
                    end = thisWeekStart;
                    return;
                case "this_month":
                    start = thisMonthStart;
                    end = today.AddDays(1);
                    return;
                case "last_month":
                    start = thisMonthStart.AddMonths(-1);
                    end = thisMonthStart;
                    return;
                case "today":
                    start = today;
                    end = today.AddDays(1);
                    return;
                default:
                    DateTime specific;
                    if (DateTime.TryParseExact(label, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out specific))
                    {
                        start = specific;
                        end = specific.AddDays(1);
                        return;
                    }
                    start = today;
                    end = today.AddDays(1);
                    return;
            }
        }

        // Turns the generic DomainSummaryProcessor output into the JSON shape handed to Haiku and the UI.
        private JObject ToJObject(DomainSummary s)
        {
            var obj = new JObject { ["total"] = s.Total };

            if (s.ByGroup.Count > 0)
            {
                var byGroupObj = new JObject();
                foreach (var kv in s.ByGroup) byGroupObj[kv.Key] = kv.Value;
                obj["byGroup"] = byGroupObj;
            }

            if (s.Sums.Count > 0)
            {
                var sumsObj = new JObject();
                foreach (var kv in s.Sums) sumsObj[kv.Key] = kv.Value;
                obj["sums"] = sumsObj;
            }

            if (s.BySO.Count > 0)
            {
                var bySoObj = new JObject();
                foreach (var kv in s.BySO) bySoObj[kv.Key] = ToJObject(kv.Value);
                obj["bySO"] = bySoObj;
            }

            return obj;
        }

        private List<string> ClassifyDomains(string question)
        {
            string labelList = "Visits, TradeVisits, Claims, Complaints, Attendance";
            string prompt = "Classify the question below into one or more of these exact labels: " + labelList + ". " +
                "Visits = housing/market visits and site calls. TradeVisits = trade/retail visits that may result in an order. " +
                "Claims = dealer verification claims. Complaints = product complaints / quality review. " +
                "Attendance = whether sales officers have checked in / started their market day. " +
                "Reply with ONLY a comma-separated list of matching labels from that exact list, nothing else.\n\n" +
                "Question: " + question;

            string reply = CallClaude(prompt, 20);
            var labels = new[] { "Visits", "TradeVisits", "Claims", "Complaints", "Attendance" };
            return labels.Where(l => reply.IndexOf(l, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        private string Narrate(string question, JObject summary, DateTime start, DateTime end)
        {
            string rangeDescription = start.AddDays(1) == end
                ? start.ToString("dd MMM yyyy")
                : start.ToString("dd MMM yyyy") + " to " + end.AddDays(-1).ToString("dd MMM yyyy");

            string prompt = "You are summarising sales data for a Regional Head, covering " + rangeDescription + ". " +
                "Using ONLY the numbers in the JSON below, answer as a short, precise bulleted list - " +
                "one fact per line, each line starting with \"- \", no more than 5 bullets, no filler sentences. " +
                "Never state a number or a name that is not present in this JSON - if something isn't covered by the data, say so instead of guessing.\n\n" +
                "JSON data:\n" + summary.ToString(Formatting.None) +
                "\n\nQuestion: " + question;

            return CallClaude(prompt, 300);
        }

        private string CallClaude(string userMessage, int maxTokens)
        {
            string apiKey = ConfigurationManager.AppSettings["ClaudeApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return "(AI is not configured yet - ask your admin to add the ClaudeApiKey setting.)";
            }

            var body = new JObject
            {
                ["model"] = "claude-haiku-5-5",
                ["max_tokens"] = maxTokens,
                ["output_config"] = new JObject { ["effort"] = "low" },
                // Thinking is on by default on Haiku 5.5 and counts against max_tokens even
                // though it's hidden - with a small budget like this, it can consume the whole
                // thing and leave no room for the actual visible answer. Not needed for a plain
                // classify/narrate task anyway.
                ["thinking"] = new JObject { ["type"] = "disabled" },
                ["messages"] = new JArray
                {
                    new JObject { ["role"] = "user", ["content"] = userMessage }
                }
            };

            try
            {
                // Old .NET Framework on this box doesn't always default to TLS 1.2 for outbound
                // HTTPS, and api.anthropic.com requires it - without this, every call fails the
                // handshake silently and surfaces only as a generic AggregateException.
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                    client.DefaultRequestHeaders.Add("anthropic-version", "2026-10-21");
                    var content = new StringContent(body.ToString(Formatting.None), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync("https://api.anthropic.com/v1/messages", content).Result; // Blocking call, matches existing codebase convention
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        Log.Instance.Error("Claude API error: " + response.StatusCode + " " + responseBody);
                        return "(AI is temporarily unavailable.)";
                    }

                    var parsed = JObject.Parse(responseBody);
                    var contentArray = parsed["content"] as JArray;
                    var textBlock = contentArray?.FirstOrDefault(c => (string)c["type"] == "text");
                    return textBlock != null ? (string)textBlock["text"] : "(No response from AI.)";
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Claude API call failed: " + Log.GetDetailedError(ex));
                return "(AI is temporarily unavailable.)";
            }
        }

        private class ClaimRow
        {
            public string Status { get; set; }
            public string SOName { get; set; }
        }

        private DomainSummary GetClaimsSummary(int headID, DateTime start, DateTime end)
        {
            var rows = new List<ClaimRow>();

            using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT dv.ID AS ClaimID, dv.ClaimManagerLatestStatus AS ApprovedStatus, so.Name AS SOName
                FROM dbo.Tbl_DealerVerification dv
                LEFT JOIN dbo.SaleOfficers so ON dv.SOID = so.ID
                WHERE dv.IsActive = 1
                  AND dv.DateSelected >= @startdate AND dv.DateSelected < @enddate
                  AND so.RegionalHeadID = CASE WHEN @headID = 0 THEN so.RegionalHeadID ELSE @headID END", conn))
            {
                cmd.Parameters.AddWithValue("@headID", headID);
                cmd.Parameters.AddWithValue("@startdate", start);
                cmd.Parameters.AddWithValue("@enddate", end);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new ClaimRow
                        {
                            Status = reader["ApprovedStatus"] as string,
                            SOName = reader["SOName"] as string
                        });
                    }
                }
            }

            return DomainSummaryProcessor.SummarizeBySO(rows, r => r.SOName, r => r.Status);
        }

        private DomainSummary GetComplaintsSummary(int headID, DateTime start, DateTime end)
        {
            var rows = FOS.Setup.ManageComplaint.GetComplaintDataForGrid(
                start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"), headID, 0, 0);

            return DomainSummaryProcessor.SummarizeBySO(rows, r => r.SaleOfficerName, r => r.ComplaintStatusString);
        }

        private DomainSummary GetTradeVisitsSummary(List<int> soIds, Dictionary<int, string> soNames, DateTime start, DateTime end)
        {
            var visits = db.Tbl_TradeVisitsFinal
                .Where(v => v.IsActive == true && v.SOID != null && soIds.Contains(v.SOID.Value)
                    && v.CreatedAt >= start && v.CreatedAt < end)
                .ToList();

            return DomainSummaryProcessor.SummarizeBySO(
                visits,
                soNameSelector: v => v.SOID.HasValue && soNames.ContainsKey(v.SOID.Value) ? soNames[v.SOID.Value] : null,
                groupKeySelector: v => (v.OrderValue.HasValue && v.OrderValue.Value > 0) ? "WithOrder" : "NoOrder",
                sumSelectors: new Dictionary<string, Func<Tbl_TradeVisitsFinal, decimal?>>
                {
                    ["OrderValue"] = v => v.OrderValue
                });
        }

        private DomainSummary GetHousingVisitsSummary(List<int> soIds, Dictionary<int, string> soNames, DateTime start, DateTime end)
        {
            var visits = db.Tbl_HousingVisits
                .Where(v => v.IsActive == true && v.SOID != null && soIds.Contains(v.SOID.Value)
                    && v.CreatedAt >= start && v.CreatedAt < end)
                .ToList();

            var visitIds = visits.Select(v => v.ID).ToList();
            var loggedVisitIds = visitIds.Any()
                ? db.Tbl_SaveCall.Where(c => c.VisitID != null && visitIds.Contains(c.VisitID.Value))
                    .Select(c => c.VisitID.Value).ToList()
                : new List<int>();
            var loggedSet = new HashSet<int>(loggedVisitIds);

            var summary = DomainSummaryProcessor.SummarizeBySO(
                visits,
                soNameSelector: v => v.SOID.HasValue && soNames.ContainsKey(v.SOID.Value) ? soNames[v.SOID.Value] : null,
                groupKeySelector: v => loggedSet.Contains(v.ID) ? "CallLogged" : "PendingCall");

            summary.ByGroup["SiteWon"] = visits.Count(v => v.SiteWon == true);

            return summary;
        }

        private class AttendanceRow
        {
            public bool Present { get; set; }
            public string SOName { get; set; }
        }

        // "Present" matches SOPresentTodayController's own definition: a "Market start" row logged today.
        private DomainSummary GetAttendanceSummary(List<int> soIds, Dictionary<int, string> soNames, DateTime start, DateTime end)
        {
            var presentSoIds = db.SOAttendances
                .Where(a => a.SOID != null && soIds.Contains(a.SOID.Value) && a.Type == "Market start"
                    && a.CreatedAt >= start && a.CreatedAt < end)
                .Select(a => a.SOID.Value)
                .Distinct()
                .ToList();

            var presentSet = new HashSet<int>(presentSoIds);
            var perSo = soIds.Select(id => new AttendanceRow
            {
                Present = presentSet.Contains(id),
                SOName = soNames.ContainsKey(id) ? soNames[id] : null
            }).ToList();

            return DomainSummaryProcessor.SummarizeBySO(perSo, r => r.SOName, r => r.Present ? "Present" : "Absent");
        }
    }
}
