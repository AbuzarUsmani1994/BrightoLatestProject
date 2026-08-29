using System;

namespace FOS.Shared
{
    public class EmployeeAttendanceTimingData
    {
        public DateTime? AttendanceDate { get; set; }
        public string Emp_Code { get; set; }
        public string Emp_Name { get; set; }
        public string Start_Time { get; set; }
        public string End_Time { get; set; }
        public string Last_Visit_Time { get; set; }
        public int? Working_Minutes { get; set; }
        public string Working_Hours { get; set; }
        public string RegionalHead { get; set; }
        public string City { get; set; }
        public string MarketStartLatlong { get; set; }
        public string MarketCloseLatlong { get; set; }
    }
}
