using System;
using System.Collections.Generic;
using System.Configuration;
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
            if (domains.Contains("Complaints"))
            {
                summary["complaints"] = ToJObject(GetComplaintsSummary(headID, start, end));
            }
            if (domains.Contains("Visits") || domains.Contains("TradeVisits") || domains.Contains("Attendance") ||
                domains.Contains("Claims") || domains.Contains("Team"))
            {
                var team = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(headID, true);
                var soIds = team.Select(s => s.ID).ToList();
                var soNames = team.ToDictionary(s => s.ID, s => s.Name);

                if (domains.Contains("Claims"))
                {
                    summary["claims"] = ToJObject(GetClaimsSummary(soIds, soNames, start, end));
                }
                if (domains.Contains("Visits"))
                {
                    summary["housingVisits"] = ToJObject(GetHousingVisitsSummary(headID, start, end));
                }
                if (domains.Contains("TradeVisits"))
                {
                    summary["tradeVisits"] = ToJObject(GetTradeVisitsSummary(soIds, soNames, start, end));
                }
                if (domains.Contains("Attendance"))
                {
                    summary["attendance"] = ToJObject(GetAttendanceSummary(soIds, soNames, start, end));
                }
                if (domains.Contains("Team"))
                {
                    summary["team"] = GetTeamInfo(team);
                }
            }

            if (!summary.Properties().Any())
            {
                return Json(new
                {
                    numbers = summary,
                    narrative = "I couldn't match that to Visits, Trade Visits, Claims, Complaints, Attendance or Team - try asking about one of those."
                });
            }

            string narrative = Narrate(question, summary, start, end);

            // Temporary: CallClaude only logs on failure, so a successful call that still
            // produces a wrong-looking answer (e.g. missing SO-wise bullets) leaves no trace
            // to diagnose from. Log the exact data sent and what Haiku said back, to check
            // against real test questions instead of guessing why.
            Log.Instance.Info("Ask AI debug - Question: " + question + " | Domains: " + string.Join(",", domains)
                + " | JSON: " + summary.ToString(Formatting.None) + " | Narrative: " + narrative);

            return Json(new { numbers = summary, narrative = narrative });
        }

        // Haiku only picks a label - the actual date math always happens here in C#, never the model.
        private void GetDateRange(string question, out DateTime start, out DateTime end)
        {
            string prompt = "Extract the date range this question is asking about. Reply with EXACTLY one of: " +
                "today, yesterday, this_week, last_week, this_month, last_month - or a specific date in YYYY-MM-DD " +
                "format if a specific day is mentioned - or a specific month in YYYY-MM format if a month name is " +
                "mentioned without a specific day (e.g. \"September\", \"Sep claims\", \"August 2026\") - use the " +
                "most recent occurrence of that month that is not in the future if no year is given (today is " +
                DateTime.Today.ToString("yyyy-MM-dd") + "). If no date is mentioned, reply exactly: today. " +
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
                    DateTime month;
                    if (DateTime.TryParseExact(label, "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out month))
                    {
                        start = new DateTime(month.Year, month.Month, 1);
                        // Defensive: a bare month name with no year given (e.g. "September") should
                        // mean the most recent PAST occurrence, never a future one - if Haiku picked
                        // the current year for a month that hasn't happened yet this year, roll back
                        // one year rather than report on a month that doesn't have any data yet.
                        if (start > today)
                        {
                            start = start.AddYears(-1);
                        }
                        end = start.AddMonths(1);
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

            if (s.Timestamp.HasValue)
            {
                obj["timestamp"] = s.Timestamp.Value.ToString("dd MMM yyyy HH:mm");
            }

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
            string labelList = "Visits, TradeVisits, Claims, Complaints, Attendance, Team";
            string prompt = "Classify the question below into one or more of these exact labels: " + labelList + ". " +
                "Visits = housing/market visits and site calls. TradeVisits = trade/retail visits that may result in an order. " +
                "Claims = sales claims (claim value/sale value, litres, approval status). Complaints = product complaints / quality review. " +
                "Attendance = whether sales officers have checked in / started their market day. " +
                "Team = contact info/roster for sales officers (name, username, phone number). " +
                "Reply with ONLY a comma-separated list of matching labels from that exact list, nothing else.\n\n" +
                "Question: " + question;

            string reply = CallClaude(prompt, 20);
            var labels = new[] { "Visits", "TradeVisits", "Claims", "Complaints", "Attendance", "Team" };
            return labels.Where(l => reply.IndexOf(l, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        private string Narrate(string question, JObject summary, DateTime start, DateTime end)
        {
            string rangeDescription = start.AddDays(1) == end
                ? start.ToString("dd MMM yyyy")
                : start.ToString("dd MMM yyyy") + " to " + end.AddDays(-1).ToString("dd MMM yyyy");

            string prompt = "You are summarising sales data for a Regional Head, covering " + rangeDescription + ". " +
                "Using ONLY the numbers in the JSON below, answer as a short, precise bulleted list - " +
                "one fact per line, each line starting with \"- \", no filler sentences. " +
                "Start with the overall total/breakdown and overall sums (value, litres, etc. if present). Then, if the " +
                "JSON has a \"bySO\" object for the relevant domain, add one bullet per relevant Sale Officer showing their " +
                "name, their total, AND their own sums (value/litres/etc.) if that SO's entry has any - do this by default, " +
                "not only when the question says \"who\". If the question asks about a specific subset of SOs (e.g. who is " +
                "absent, who has pending items), only list the SOs matching that subset using the \"byGroup\" value inside " +
                "each SO's entry - do not list SOs outside that subset. Keep each SO bullet short - just the name and the " +
                "relevant numbers, no extra commentary. If an SO's entry has a \"timestamp\" value, include that exact " +
                "date/time in their bullet (e.g. their check-in time) - do this by default whenever it's present, not only " +
                "when the question asks for a time. " +
                "Never state a number, a name or a time that is not present in this JSON - if something isn't covered by the data, say so instead of guessing.\n\n" +
                "JSON data:\n" + summary.ToString(Formatting.None) +
                "\n\nQuestion: " + question;

            return CallClaude(prompt, 600);
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

        // Tbl_SalesClaimMaster, not Tbl_DealerVerification - the latter is a related but
        // separate sub-type (verification forms only) with no SaleValue/TotalLiters at all,
        // by design (see SQL/CreateTbl_DealerVerification.sql's own comment).
        private DomainSummary GetClaimsSummary(List<int> soIds, Dictionary<int, string> soNames, DateTime start, DateTime end)
        {
            var claims = db.Tbl_SalesClaimMaster
                .Where(c => c.IsActive == true && c.SOID != null && soIds.Contains(c.SOID.Value)
                    && c.DateSelected >= start && c.DateSelected < end)
                .ToList();

            return DomainSummaryProcessor.SummarizeBySO(
                claims,
                soNameSelector: c => c.SOID.HasValue && soNames.ContainsKey(c.SOID.Value) ? soNames[c.SOID.Value] : null,
                groupKeySelector: c => c.ClaimManagerLatestStatus,
                sumSelectors: new Dictionary<string, Func<Tbl_SalesClaimMaster, decimal?>>
                {
                    ["SaleValue"] = c => c.SaleValue,
                    ["TotalLiters"] = c => c.TotalLiters
                });
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

        // Wraps the same backend SP the real "Housing Visits" PDF/CSV report uses
        // (Sp_OrderForPDFHousingVisitsBackend, see ReportsController around line 5115),
        // instead of a thin ad-hoc query against Tbl_HousingVisits directly - that
        // ad-hoc version only had internal bookkeeping fields (call-logged vs not,
        // a raw SiteWon flag) with no real business meaning to a Regional Head.
        // This SP carries the same fields the business actually reports on: estimated
        // value, order volume, business status, scope/nature, etc.
        private DomainSummary GetHousingVisitsSummary(int headID, DateTime start, DateTime end)
        {
            var visits = db.Sp_OrderForPDFHousingVisitsBackend(start, end, 0, headID).ToList();

            return DomainSummaryProcessor.SummarizeBySO(
                visits,
                soNameSelector: v => v.SaleOfficerName,
                groupKeySelector: v => v.BusinessStatus,
                sumSelectors: new Dictionary<string, Func<Sp_OrderForPDFHousingVisitsBackend_Result, decimal?>>
                {
                    ["EstimatedValue"] = v => v.EstimatedValue,
                    ["OrderVolume"] = v => v.OrderVolume
                });
        }

        private class AttendanceRow
        {
            public bool Present { get; set; }
            public string SOName { get; set; }
            public DateTime? CheckInTime { get; set; }
        }

        // "Present" matches SOPresentTodayController's own definition: a "Market start" row logged today.
        private DomainSummary GetAttendanceSummary(List<int> soIds, Dictionary<int, string> soNames, DateTime start, DateTime end)
        {
            var checkIns = db.SOAttendances
                .Where(a => a.SOID != null && soIds.Contains(a.SOID.Value) && a.Type == "Market start"
                    && a.CreatedAt >= start && a.CreatedAt < end)
                .Select(a => new { SOID = a.SOID.Value, a.CreatedAt })
                .ToList();

            // First check-in of the range per SO, in case they logged "Market start" more than once.
            var firstCheckInBySO = checkIns
                .GroupBy(c => c.SOID)
                .ToDictionary(g => g.Key, g => g.Min(c => c.CreatedAt));

            var perSo = soIds.Select(id => new AttendanceRow
            {
                Present = firstCheckInBySO.ContainsKey(id),
                SOName = soNames.ContainsKey(id) ? soNames[id] : null,
                CheckInTime = firstCheckInBySO.ContainsKey(id) ? firstCheckInBySO[id] : null
            }).ToList();

            return DomainSummaryProcessor.SummarizeBySO(perSo, r => r.SOName, r => r.Present ? "Present" : "Absent",
                timestampSelector: r => r.CheckInTime);
        }

        // Plain roster, not a DomainSummary - there's nothing to count or sum here, just contact
        // info. Queries SaleOfficer directly rather than the existing SaleOfficerDetailRpt proc,
        // which also selects the plaintext Password column - deliberately never selected here.
        // No Email field exists anywhere on SaleOfficer in this system.
        private JArray GetTeamInfo(List<SaleOfficer> team)
        {
            var array = new JArray();
            foreach (var so in team)
            {
                array.Add(new JObject
                {
                    ["name"] = so.Name,
                    ["userName"] = so.UserName,
                    ["phone1"] = so.Phone1,
                    ["phone2"] = so.Phone2,
                    ["active"] = so.IsActive
                });
            }
            return array;
        }
    }
}
