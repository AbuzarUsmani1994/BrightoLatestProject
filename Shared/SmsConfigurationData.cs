using System;

namespace FOS.Shared
{
    public class SmsConfigurationData
    {
        public string Url { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string Mask { get; set; }
        public string Template1 { get; set; }
        public string Template2 { get; set; }
        public string Template3 { get; set; }
        public string Template4 { get; set; }
        public string Template5 { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
