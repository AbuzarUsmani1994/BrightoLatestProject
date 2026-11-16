using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using FOS.DataLayer;
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
                summary["claims"] = GetClaimsSummary(headID, today, tomorrow);
            }
            if (domains.Contains("Complaints"))
            {
                summary["complaints"] = GetComplaintsSummary(headID, today, tomorrow);
            }
            if (domains.Contains("Visits") || domains.Contains("TradeVisits"))
            {
                var soIds = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(headID, true)
                    .Select(s => s.ID).ToList();

                if (domains.Contains("Visits"))
                {
                    summary["housingVisits"] = GetHousingVisitsSummary(soIds, today, tomorrow);
                }
                if (domains.Contains("TradeVisits"))
                {
                    summary["tradeVisits"] = GetTradeVisitsSummary(soIds, today, tomorrow);
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

        private List<string> ClassifyDomains(string question)
        {
            string labelList = "Visits, TradeVisits, Claims, Complaints";
            string prompt = "Classify the question below into one or more of these exact labels: " + labelList + ". " +
                "Visits = housing/market visits and site calls. TradeVisits = trade/retail visits that may result in an order. " +
                "Claims = dealer verification claims. Complaints = product complaints / quality review. " +
                "Reply with ONLY a comma-separated list of matching labels from that exact list, nothing else.\n\n" +
                "Question: " + question;

            string reply = CallClaude(prompt, 20);
            var labels = new[] { "Visits", "TradeVisits", "Claims", "Complaints" };
            return labels.Where(l => reply.IndexOf(l, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        private string Narrate(string question, JObject summary)
        {
            string prompt = "You are summarising sales data for a Regional Head, for today only. " +
                "Using ONLY the numbers in the JSON below, answer the question in 2-4 short sentences. " +
                "Never state a number that is not present in this JSON - if something isn't covered by the data, say so instead of guessing.\n\n" +
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

        private JObject GetClaimsSummary(int headID, DateTime start, DateTime end)
        {
            int total = 0;
            var byStatus = new Dictionary<string, int>();

            using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT dv.ID AS ClaimID, dv.ClaimManagerLatestStatus AS ApprovedStatus
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
                        total++;
                        string status = reader["ApprovedStatus"] as string;
                        if (string.IsNullOrEmpty(status)) status = "Unknown";
                        byStatus[status] = byStatus.ContainsKey(status) ? byStatus[status] + 1 : 1;
                    }
                }
            }

            var statusObj = new JObject();
            foreach (var kv in byStatus)
            {
                statusObj[kv.Key] = kv.Value;
            }

            return new JObject
            {
                ["totalClaims"] = total,
                ["byStatus"] = statusObj
            };
        }

        private JObject GetComplaintsSummary(int headID, DateTime start, DateTime end)
        {
            var rows = FOS.Setup.ManageComplaint.GetComplaintDataForGrid(
                start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"), headID, 0, 0);

            var byStatus = rows
                .GroupBy(r => string.IsNullOrEmpty(r.ComplaintStatusString) ? "Unknown" : r.ComplaintStatusString)
                .ToDictionary(g => g.Key, g => g.Count());

            var statusObj = new JObject();
            foreach (var kv in byStatus)
            {
                statusObj[kv.Key] = kv.Value;
            }

            return new JObject
            {
                ["totalComplaints"] = rows.Count,
                ["byStatus"] = statusObj
            };
        }

        private JObject GetTradeVisitsSummary(List<int> soIds, DateTime start, DateTime end)
        {
            var visits = db.Tbl_TradeVisitsFinal
                .Where(v => v.IsActive == true && v.SOID != null && soIds.Contains(v.SOID.Value)
                    && v.CreatedAt >= start && v.CreatedAt < end)
                .ToList();

            int withOrder = visits.Count(v => v.OrderValue.HasValue && v.OrderValue.Value > 0);
            decimal totalOrderValue = visits.Where(v => v.OrderValue.HasValue).Sum(v => v.OrderValue.Value);

            return new JObject
            {
                ["totalTradeVisits"] = visits.Count,
                ["visitsWithOrder"] = withOrder,
                ["totalOrderValue"] = totalOrderValue
            };
        }

        private JObject GetHousingVisitsSummary(List<int> soIds, DateTime start, DateTime end)
        {
            var visits = db.Tbl_HousingVisits
                .Where(v => v.IsActive == true && v.SOID != null && soIds.Contains(v.SOID.Value)
                    && v.CreatedAt >= start && v.CreatedAt < end)
                .ToList();

            var visitIds = visits.Select(v => v.ID).ToList();
            int withCallLogged = visitIds.Any()
                ? db.Tbl_SaveCall.Count(c => c.VisitID != null && visitIds.Contains(c.VisitID.Value))
                : 0;
            int sitesWon = visits.Count(v => v.SiteWon == true);

            return new JObject
            {
                ["totalVisits"] = visits.Count,
                ["visitsWithCallLogged"] = withCallLogged,
                ["visitsPendingCall"] = visits.Count - withCallLogged,
                ["sitesWon"] = sitesWon
            };
        }
    }
}
