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

        public ActionResult Index()
        {
            if (AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                return RedirectToAction("Login", "AdminPanel");
            }
            return View();
        }

        [HttpPost]
        public JsonResult Ask(string question)
        {
            int headID = AdminPanelController.GetRegionalHeadIDRelatedToUser();
            if (headID == 0)
            {
                return Json(new { narrative = "You're not set up as a Regional Head, so this screen isn't available to you." });
            }
            if (string.IsNullOrWhiteSpace(question))
            {
                return Json(new { narrative = "Ask a question about Visits, Trade Visits, Claims or Complaints." });
            }

            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            var domains = ClassifyDomains(question);

            var summary = new JObject();
            if (domains.Contains("Claims"))
            {
                summary["claims"] = ToJObject(GetClaimsSummary(headID, today, tomorrow));
            }
            if (domains.Contains("Complaints"))
            {
                summary["complaints"] = ToJObject(GetComplaintsSummary(headID, today, tomorrow));
            }
            if (domains.Contains("Visits") || domains.Contains("TradeVisits") || domains.Contains("Attendance"))
            {
                var team = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(headID, true);
                var soIds = team.Select(s => s.ID).ToList();
                var soNames = team.ToDictionary(s => s.ID, s => s.Name);

                if (domains.Contains("Visits"))
                {
                    summary["housingVisits"] = ToJObject(GetHousingVisitsSummary(soIds, soNames, today, tomorrow));
                }
                if (domains.Contains("TradeVisits"))
                {
                    summary["tradeVisits"] = ToJObject(GetTradeVisitsSummary(soIds, soNames, today, tomorrow));
                }
                if (domains.Contains("Attendance"))
                {
                    summary["attendance"] = ToJObject(GetAttendanceSummary(soIds, soNames, today, tomorrow));
                }
            }

            if (!summary.Properties().Any())
            {
                return Json(new
                {
                    numbers = summary,
                    narrative = "I couldn't match that to Visits, Trade Visits, Claims or Complaints - try asking about one of those."
                });
            }

            string narrative = Narrate(question, summary);

            return Json(new { numbers = summary, narrative = narrative });
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

        private string Narrate(string question, JObject summary)
        {
            string prompt = "You are summarising sales data for a Regional Head, for today only. " +
                "Using ONLY the numbers in the JSON below, answer the question in 2-4 short sentences. " +
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
                ["messages"] = new JArray
                {
                    new JObject { ["role"] = "user", ["content"] = userMessage }
                }
            };

            try
            {
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
                Log.Instance.Error("Claude API call failed: " + ex.Message);
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
