using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace FOS.Web.UI.Common
{
    // Zong Corporate Bulk SMS (CBS) "Send Quick SMS" REST API (CBS_API_Documentation_v2.pdf).
    // Shared by any feature that needs to send a single SMS - extracted from
    // RetailerOrdersController.SendCustomerSMS so the gateway call isn't duplicated.
    public static class CBSSmsSender
    {
        private const string SendQuickSmsUrl = "https://cbs.zong.com.pk/reachrestapi/home/SendQuickSMS";

        public class CBSResult
        {
            public bool Success { get; set; }
            public string StatusCode { get; set; }
            public string Description { get; set; }
            public string RawResponse { get; set; }
        }

        // Converts a locally-formatted PK mobile number (e.g. "0311xxxxxxx") to the
        // "92xxxxxxxxxx" format the CBS API expects as Destination.
        public static string NormalizePhoneForCBS(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;

            var digits = new string(phone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(digits)) return null;

            if (digits.StartsWith("0"))
            {
                digits = "92" + digits.Substring(1);
            }
            else if (!digits.StartsWith("92") && digits.Length == 10)
            {
                digits = "92" + digits;
            }
            return digits;
        }

        // uniCode: "0" for English (GSM_7), "1" for Urdu/Chinese - per the CBS docs,
        // sending Urdu text with UniCode=0 is exactly the kind of mismatch that gets
        // silently dropped by the carrier instead of delivered.
        public static CBSResult SendQuickSMS(string destination, string message, string uniCode)
        {
            var cbsLoginId = System.Configuration.ConfigurationManager.AppSettings["CBSLoginId"];
            var cbsLoginPassword = System.Configuration.ConfigurationManager.AppSettings["CBSLoginPassword"];
            var cbsMask = System.Configuration.ConfigurationManager.AppSettings["CBSMask"];

            if (string.IsNullOrEmpty(cbsLoginId) || string.IsNullOrEmpty(cbsLoginPassword) || string.IsNullOrEmpty(cbsMask))
            {
                throw new InvalidOperationException("CBSLoginId / CBSLoginPassword / CBSMask are not configured in Web.config <appSettings>.");
            }

            var payload = new
            {
                loginId = cbsLoginId,
                loginPassword = cbsLoginPassword,
                Destination = destination,
                Mask = cbsMask,
                Message = message,
                UniCode = uniCode,
                ShortCodePrefered = "N"
            };

            // This legacy .NET Framework app defaults to SSL3/TLS1.0, which cbs.zong.com.pk
            // (like most modern HTTPS endpoints) rejects - without this, PostAsync faults
            // with a connection error that surfaces as the unhelpful "One or more errors
            // occurred." AggregateException message.
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

            string rawResponse;
            using (var client = new HttpClient())
            {
                var content = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json");

                var response = client.PostAsync(SendQuickSmsUrl, content).Result;
                rawResponse = response.Content.ReadAsStringAsync().Result;
            }

            var parts = (rawResponse ?? string.Empty).Split('|');
            var statusCode = parts.Length > 0 ? parts[0].Trim() : string.Empty;
            var description = parts.Length > 1 ? parts[1] : rawResponse;

            return new CBSResult
            {
                Success = statusCode == "0",
                StatusCode = statusCode,
                Description = description,
                RawResponse = rawResponse
            };
        }
    }
}
