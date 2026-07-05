using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class AreaData
    {
        public List<RegionalHeadData> RegionalHeads { get; set; }
        public int RegionalHeadID { get; set; }
        public List<SaleOfficerData> SaleOfficersFrom { get; set; }
        public List<SaleOfficer> SaleOfficersFroms { get; set; }
        public List<SaleOfficerData> SaleOfficersTo { get; set; }
        public int intSaleOfficerIDfrom { get; set; }
        public int intSaleOfficerIDto { get; set; }
        public List<SaleOfficerData> IDc { get; set; }
        public List<SaleOfficerData> Namec { get; set; }
        public int TID { get; set; }
        public int ID { get; set; }

        [DisplayName("Area Name *")]
        [Required(ErrorMessage = "* Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int RegionID { get; set; }

        [DisplayName("Region Name *")]
        public string RegionName { get; set; }
        public int Type { get; set; }
   
        [Required(ErrorMessage = "* Required")]


        [DisplayName("City Name *")]
        public string CityName { get; set; }

        [DisplayName("City Name *")]
        public string ShortCode { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }

        public List<RegionData> Regions { get; set; }
        //public Nullable<int> RegionID { get; set; }
        public int CityID { get; set; }
        public int SOID { get; set; }
        public int SOID1 { get; set; }
        public int SOID2 { get; set; }
        public int RepotedTo { get; set; }
        public int ReportedFor { get; set; }
        public string SaleOfficerName { get; set; }
        public int RangeID { get; set; }
        public string ReportedToName { get; set; }
        public string ReportedForName { get; set; }
        public List<CityData> Cities { get; set; }
        public List<MainCategories> Range { get; set; }
        public List<SaleOfficerData> Salesofficer { get; set; }
        public IEnumerable<RegionalHeadTypeData> RegionalHeadTypeData { get; set; }
        public List<RegionalHeadData> RegionalHead { get; set; }
        public List<SaleOfficerData> Salesofficer1 { get; set; }
        public List<SaleOfficerData> Salesofficer2 { get; set; }
        public List<SaleOfficer> Salesofficerdata { get; set; }

        public int? FinancialYearID { get; set; }
        public string FinancialYearName { get; set; }
        public string Quarter { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<FinancialYearListItem> FinancialYears { get; set; }
        public List<TrainingRowData> TrainingRows { get; set; }
        public List<TrainingRowData> AttendanceRows { get; set; }
        public List<ProdKnowledgeRowData> ProdKnowledgeRows { get; set; }
    }

    public class TrainingRowData
    {
        public int SOID { get; set; }
        public decimal? Value { get; set; }
    }

    public class ProdKnowledgeRowData
    {
        public int SOID { get; set; }
        public decimal? ProductKnowledge { get; set; }
        public decimal? CompFeed { get; set; }
    }

    public class AreaGraphData
    {
        public string CityName { get; set; }
        public int AreaCount { get; set; }
    }

    public class Tbl_AccessModel
    {
        public int ID { get; set; }
        public int SaleOfficerID { get; set; }
        public int? RHID { get; set; }
        public string SaleOfficerName { get; set; }
        public string ReportedToName { get; set; }
        public string ReportedForName { get; set; }
        public string RegionName { get; set; }
        public string RegionalHeadName { get; set; }
        public int RepotedTo { get; set; }
        public int ReportedFor { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> RegionID { get; set; }

        public virtual Region Region { get; set; }
        public virtual SaleOfficer SaleOfficer { get; set; }
    }

}