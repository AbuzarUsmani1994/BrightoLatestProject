using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class OrderSummaryReportData
    {

        public int RetailerID { get; set; }
        public string ShopName { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int DistributorID { get; set; }
        public int RangeID { get; set; }
        public int RegionID { get; set; }
        public int CityID { get; set; }
        public int SaleOfficerID { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public List<CategoryData> range { get; set; }
        public List<MainCategories> ranges { get; set; }
        public List<SaleOfficerData> saleofficerdata { get; set; }
        public virtual List<SaleOfficer> SaleOfficer { get; set; }
        public List<RetailerData> dealerdata { get; set; }
        public List<CityData> CityData { get; set; }
        public virtual List<RegionalHeadData> RegionalHead { get; set; }
        public List<RegionData> regionData { get; set; }
    }


    public class SaleOfficerCount
    {
        public string Name { get; set; }

      
        public int? TradeVisitsCount { get; set; }
        public int? HousingVisitsCount { get; set; }
        public int? CorporateVisitsCount { get; set; }
        public int? AllPurposeVisitsCount { get; set; }
    }

    public class SaleOfficerCountFinal
    {
        public string Name { get; set; }

        public string SOCode { get; set; }
        public string Segment { get; set; }
        public string RegionalHead { get; set; }
        public string Region { get; set; }
        public string PrimaryCity { get; set; }
        public int Prospects { get; set; }
        public int Existing { get; set; }
        public int Active { get; set; }
        public int Completed { get; set; }
        public int Lost { get; set; }
        public int Unassigned { get; set; }
        public int TotalCustomers { get; set; }

        public int? Trade { get; set; }
        public int? Housing { get; set; }
        public int? Corporate { get; set; }
        public int? AllPurposeCount { get; set; }
        public int? TradeVisitsCount { get; set; }
        public int? HousingVisitsCount { get; set; }
        public int? CorporateVisitsCount { get; set; }
        public int? AllPurpose { get; set; }
        public int? TotalVisits { get; set; }
    }

    public class KPIData
    {

        public int SOID { get; set; }
        public string SoName { get; set; }

        public string Locations { get; set; }
        public string RHName { get; set; }
        public string RegionName { get; set; }
        //public string CityName { get; set; }


        public DateTime? DateOFOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? AttendanceStart { get; set; }
        public DateTime? AttendanceEnd { get; set; }


        public string Attstartstring { get; set; }
        public string Attendstring { get; set; }
        public DateTime? EndDate { get; set; }
        public string ElapseTime { get; set; }
        public string CityName { get; set; }
        public int? totalVisits { get; set; }
        public decimal? totalSale { get; set; }
        public string startstring { get; set; }
        public string endstring { get; set; }
        public int ProductiveShops { get; set; }

        public double? PerDayVisitShops { get; set; }
        public double? PerDayFollowups{ get; set; }

        public double? PerDaycartons { get; set; }

        public int PerDayOrders { get; set; }
        public int NonProductive { get; set; }
        public decimal ProductivePer { get; set; }
        public double perDayorders { get; set; }

        public double TotalWorkingDays { get; set; }

        public double AbsentDays { get; set; }
        public int? totallines { get; set; }
        public string Linesperbill { get; set; }
      
    }


    public class VisitDetailMapDto
    {

        public string CustomerName { get; set; }
        public decimal? Lattitude { get; set; }
        public decimal? Longitude { get; set; }
        public string VisitPurpose { get; set; }
        public DateTime? VisitDate { get; set; }

    }
    }
