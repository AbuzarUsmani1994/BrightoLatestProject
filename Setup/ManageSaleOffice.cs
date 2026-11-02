using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class ManageSaleOffice
    {

        // Get SalesOfficer Area Names With Line Break ...
        public static string GetSaleOfficerAreaName(int intSaleOfficerID)
        {
            String strAreaName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == intSaleOfficerID).FirstOrDefault();
            strAreaName = string.Join("<br/>", objSaleOfficer.Areas.Select(p => p.Name));

            return strAreaName;
        }

        public static List<SaleOfficerData> GetSaleOfficerListRelatedtoregionalHeadID(int? RHID, bool selectOption = false)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if (RHID == 0 && !selectOption)
                {
                    saleOfficerData = dbContext.SaleOfficers
                           .Where(u => u.IsActive == true)
                           .Select(
                               u => new SaleOfficerData
                               {
                                   ID = u.ID,
                                   Name = u.Name,

                               }).ToList();
                    saleOfficerData.Insert(0, new SaleOfficerData
                    {
                        ID = 0,
                        Name = "All"
                    });
                }
                else
                {
                    var list = dbContext.Tbl_SOREGIONS.Where(x => x.RegionID == RHID).Select(x => x.SaleofficerID).ToList();

                    saleOfficerData = dbContext.SaleOfficers.Where(c => list.Contains(c.ID) && c.IsActive == true)
                           .Select(
                               u => new SaleOfficerData
                               {
                                   ID = u.ID,
                                   Name = u.Name,

                               }).ToList();


                    saleOfficerData.Insert(0, new SaleOfficerData
                    {
                        ID = 0,
                        Name = "All"
                    });
                }

            }
            return saleOfficerData;
        }
        // Get SalesOfficer Area ID With Comma Seperator ...


        public static List<JobsDetail> GetOrdersForRetailer(int? RHID, bool selectOption = false)
        {
            List<JobsDetail> saleOfficerData = new List<JobsDetail>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {


                saleOfficerData = dbContext.JobsDetails.Where(x => x.RetailerID == RHID && x.JobType=="Retailer Order").ToList();

                    
               

            }
            return saleOfficerData;
        }


        public static string GetSaleOfficerAreaID(int intSaleOfficerID)
        {
            String strAreaName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == intSaleOfficerID).FirstOrDefault();
            strAreaName = string.Join(",", objSaleOfficer.Areas.Select(p => p.ID));

            return strAreaName;
        }
        public static List<SaleOfficerData> GetAllSO()
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.SaleOfficers.Where(s => s.IsActive == true).ToList();

                foreach (var so in list)
                {
                    saleOfficerData.Add(new SaleOfficerData
                    {
                        ID = so.ID,
                        Name = so.Name
                    });
                }
            }
            return saleOfficerData;
        }


        public static List<SaleOfficerData> GetAllSOSRelatedtoHead(int ID,int userID)
        {
         

            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var headID = dbContext.Users.Where(x => x.ID == userID).Select(x => x.RegionalheadRef).FirstOrDefault();


                if (userID == 1)
                {
                    var activeSoIds = dbContext.SaleOfficers.Where(x => x.IsActive == true).Select(x => x.ID).ToList();
                    var list = dbContext.Tbl_SOREGIONS.Where(x => x.RegionID == ID && activeSoIds.Contains(x.SaleofficerID)).ToList();

                    foreach (var so in list)
                    {
                        saleOfficerData.Add(new SaleOfficerData
                        {
                            ID = so.SaleofficerID,
                            Name = dbContext.SaleOfficers.Where(x => x.ID == so.SaleofficerID).Select(x => x.Name).FirstOrDefault()
                        });
                    }
                }
                else
                {
                    var list = dbContext.SaleOfficers.Where(x => x.RegionalHeadID == headID && x.IsActive == true).ToList();

                    foreach (var so in list)
                    {
                        saleOfficerData.Add(new SaleOfficerData
                        {
                            ID = so.ID,
                            Name = dbContext.SaleOfficers.Where(x => x.ID == so.ID).Select(x => x.Name).FirstOrDefault()
                        });
                    }

                }
            }
            return saleOfficerData;
        }

        public static List<SaleOfficerData> GetAllSOS(int ID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var activeSoIds = dbContext.SaleOfficers.Where(x => x.IsActive == true).Select(x => x.ID).ToList();
                var list = dbContext.Tbl_SOREGIONS.Where(x => x.RegionID == ID && activeSoIds.Contains(x.SaleofficerID)).ToList();

                foreach (var so in list)
                {
                    saleOfficerData.Add(new SaleOfficerData
                    {
                        ID = so.SaleofficerID,
                        Name = dbContext.SaleOfficers.Where(x => x.ID == so.SaleofficerID).Select(x => x.Name).FirstOrDefault()
                    });
                }
            }
            return saleOfficerData;
        }
        // Get All Regions Related To SalesOfficer ...
        public static List<RegionData> GetRegionRelatedToSaleOfficer(int SaleOfficerID, int RegionalHeadID)
        {
            List<RegionData> strRegionID = new List<RegionData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID && s.RegionalHeadID == RegionalHeadID).FirstOrDefault();
                    strRegionID = objSaleOfficer.RegionalHead.RegionalHeadRegions
                        .Where(r => r.RegionHeadID == RegionalHeadID)
                        .Select(u => new RegionData
                        {
                            RegionID = u.RegionID,
                            Name = u.Region.Name
                        }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }


            return strRegionID;
        }

        public static List<RetailerData> GetDistributorRetailedtoSOForDistributor(int? Region, int? CityID)
        {
            List<RetailerData> saleOfficerData = new List<RetailerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.Dealers.Where(u => u.RegionID == Region && u.CityID== CityID &&  u.IsActive==true).OrderBy(x=>x.ShopName).ToList();

                foreach (var so in list)
                {

                    saleOfficerData.Add(new RetailerData
                    {
                        ID = so.ID,
                        ShopName = so.ShopName
                    });
                }
            }
            return saleOfficerData;
        }

        public static List<SaleOfficerData> GetAllSORelatedToRegion(int? Region)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.SaleOfficers.Where(u => u.RegionID == Region && u.IsActive == true).ToList();

                foreach (var so in list)
                {
                    saleOfficerData.Add(new SaleOfficerData
                    {
                        ID = so.ID,
                        Name = so.Name
                    });
                }
            }
            return saleOfficerData;
        }

        public static List<SaleOfficerData> GetAllSORelatedToRegionForDistributor(int? Region)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var activeSoIds = dbContext.SaleOfficers.Where(x => x.IsActive == true).Select(x => x.ID).ToList();
                var list = dbContext.Tbl_SOREGIONS.Where(u => u.RegionID == Region && activeSoIds.Contains(u.SaleofficerID)).ToList();

                foreach (var so in list)
                {
                    var saO = dbContext.SaleOfficers.Where(x => x.ID == so.SaleofficerID).FirstOrDefault();
                    saleOfficerData.Add(new SaleOfficerData
                    {
                        ID = so.SaleofficerID,
                        Name = saO.Name
                    });
                }
            }
            return saleOfficerData;
        }


        public static List<CityData> GetAllCitiesRelatedToRegionForDistributor(int? Region)
        {
            List<CityData> saleOfficerData = new List<CityData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.Cities.Where(u => u.RegionID == Region && u.IsActive==true).ToList();

                foreach (var so in list)
                {
                   // var saO = dbContext.SaleOfficers.Where(x => x.ID == so.SaleofficerID).FirstOrDefault();
                    saleOfficerData.Add(new CityData
                    {
                        ID = so.ID,
                        Name = so.Name
                    });
                }
            }
            return saleOfficerData;
        }
        public static List<SaleOfficerData> GetSaleOfficerByRegionalHeadID(int RegionalHeadID, bool orderBy, bool comboBox)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            if (comboBox)
            {
                saleOfficerData.Add(new SaleOfficerData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var list = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false && u.IsActive == true).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).OrderBy(p => p.Name).ToList();

                    foreach (var item in list)
                    {
                        saleOfficerData.Add(item);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return saleOfficerData;
        }

        // Get All SalesOfficer Related To CityID ...
        public static List<SaleOfficerData> GetSaleOfficerByRegionalHeadID(int RegionalHeadID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false && u.IsActive == true).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    RegionalHeadID = u.RegionalHeadID,
                                    Name = u.Name,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    CityName = u.City != null ? u.City.Name : "",
                                    CityID = u.CityID != null ? u.CityID : 0,
                                    AreaName = GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return saleOfficerData;
        }

        // Get All SalesOfficers List For Grid ...
        public static List<SaleOfficerData> GetSaleOfficerList(bool orderby = false)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (orderby)
                    {
                        saleOfficerData = dbContext.SaleOfficers.Where(u => u.IsDeleted == false && u.IsActive == true).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    RegionalHeadID = u.RegionalHeadID,
                                    Name = u.Name,
                                    UserName = u.UserName,
                                    Password = u.Password,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    CityName = u.City != null ? u.City.Name : "",
                                    CityID = u.CityID != null ? u.CityID : 0,
                                    AreaName = GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    LastUpdate = u.LastUpdate
                                }).OrderBy(x => x.Name).ToList();
                    }
                    else
                    {
                        saleOfficerData = dbContext.SaleOfficers.Where(u => u.IsDeleted == false && u.IsActive == true).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    RegionalHeadID = u.RegionalHeadID,
                                    Name = u.Name,
                                    UserName = u.UserName,
                                    Password = u.Password,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    CityName = u.City != null ? u.City.Name : "",
                                    CityID = u.CityID != null ? u.CityID : 0,
                                    AreaName = GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                    }

                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get SalesOfficer List Failed");
                throw;
            }

            return saleOfficerData;
        }

        // Get All SalesOfficers List For Grid ...
        public static List<SaleOfficerData> GetSaleOfficerListForGrid(int RegionalHeadType, int RegionalHeadID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.RegionalHead.Type == RegionalHeadType && u.IsDeleted == false).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    RegionalHeadID = u.RegionalHeadID,
                                    Name = u.Name,
                                    UserName = u.UserName,
                                    Password = u.Password,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    RegionID = u.RegionID,
                                    ECode=u.ECode,
                                    RangeName = dbContext.MainCategories.Where(x => x.MainCategID == u.RangeID).Select(z => z.MainCategDesc).FirstOrDefault(),
                                    RegionName = dbContext.Regions.Where(x => x.ID == u.RegionID).Select(z => z.Name).FirstOrDefault(),
                                    CityName = u.City != null ? u.City.Name : "",
                                    CityID = u.CityID != null ? u.CityID : 0,
                                    IsActiveYes=u.IsActive==true?"Yes":"No",
                                    AreaName = GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    DesignationName = dbContext.SODesignations.Where(x => x.ID == u.DesignationID).Select(z => z.Name).FirstOrDefault(),
                                    LastUpdate = u.LastUpdate,
                                    CreatedAT=u.CreatedDate,
                                    Leaveon=u.LeaveOn
                                 
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get SalesOfficer List Failed");
                throw;
            }

            return saleOfficerData;
        }


        public static List<SaleOfficerTargets> GetSOTargetsListForGrid( int RegionalHeadID)
        {
            List<SaleOfficerTargets> saleOfficerData = new List<SaleOfficerTargets>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.Tbl_Targets.Where(u => u.HeadID == RegionalHeadID ).ToList()
                            .Select(
                                u => new SaleOfficerTargets
                                {
                                    //ID = u.ID,
                                    //RegionalHeadID = u.HeadID,
                                    //Name = u.Name,
                                    //UserName = u.UserName,
                                    //Password = u.Password,
                                    //RegionalHeadName = u.RegionalHead.Name,
                                    //Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    //Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    //RegionID = u.RegionID,
                                    //ECode = u.ECode,
                                    //RangeName = dbContext.MainCategories.Where(x => x.MainCategID == u.RangeID).Select(z => z.MainCategDesc).FirstOrDefault(),
                                    //RegionName = dbContext.Regions.Where(x => x.ID == u.RegionID).Select(z => z.Name).FirstOrDefault(),
                                    //CityName = u.City != null ? u.City.Name : "",
                                    //CityID = u.CityID != null ? u.CityID : 0,
                                    //IsActiveYes = u.IsActive == true ? "Yes" : "No",
                                    //AreaName = GetSaleOfficerAreaName(u.ID),
                                    //AreaID = GetSaleOfficerAreaID(u.ID),
                                    //DesignationName = dbContext.SODesignations.Where(x => x.ID == u.DesignationID).Select(z => z.Name).FirstOrDefault(),
                                    //LastUpdate = u.LastUpdate,
                                    //CreatedAT = u.CreatedDate,
                                    //Leaveon = u.LeaveOn

                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get SalesOfficer List Failed");
                throw;
            }

            return saleOfficerData;
        }

        //Get All SalesOfficer List Method...
        public static List<SaleOfficer> GetAllSaleOfficerList()
        {
            List<SaleOfficer> saleOfficerData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                saleOfficerData = dbContext.SaleOfficers.Where(s => s.IsActive == true).ToList();
            }
            return saleOfficerData;
        }

        public static List<SaleOfficerData> GetSOByRegionalHeadId(int RHID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var list = dbContext.SaleOfficers.Where(s => s.RegionalHeadID == RHID && s.IsActive == true).ToList();

                foreach (var so in list)
                {
                    saleOfficerData.Add(new SaleOfficerData
                    {
                        ID = so.ID,
                        Name = so.Name
                    });
                }
            }
            return saleOfficerData;
        }
        //Get All SalesOfficer List Method...
        public static List<SaleOfficer> GetAllSaleOfficerListRelatedtoregionalHeadID(int RHID, bool selectOption = false)
        {
            List<SaleOfficer> saleOfficerData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if (RHID == 0 && !selectOption)
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(p=>p.IsActive==true).OrderBy(p => p.Name).ToList();
                }
                else
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(s => s.RegionalHeadID == RHID && s.IsActive==true).OrderBy(p => p.Name).ToList();
                }

            }
            return saleOfficerData;
        }

        public static List<Tbl_Segmenttype> GetAllSegments()
        {
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                return dbContext.Tbl_Segmenttype
                                .Where(s => s.IsActive == true)
                                .OrderBy(p => p.Name)
                                .ToList();
            }
        }

        public static List<SOType> GetAllRoles()
        {
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                return dbContext.SOTypes
                                .Where(s => s.Status == true)
                                .OrderBy(p => p.Name)
                                .ToList();
            }
        }

        public static List<Tbl_FocusArea> GetAllFocusAreas()
        {
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                return dbContext.Tbl_FocusArea
                                .Where(s => s.IsActive == true)
                                
                                .ToList();
            }
        }


        public static List<SaleOfficerData> GetAllSaleOfficerListRelatedtoDealerDSR(int RHID, int? RangeID)
        {
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);


            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime todate = dtFromToday.AddDays(1);
            DateTime fromdate = todate.AddDays(-30);
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if (RangeID == 6)
                {
                    saleOfficerData = (from jd in dbContext.JobsDetails
                                       join re in dbContext.Retailers on jd.RetailerID equals re.ID
                                       join so in dbContext.SaleOfficers on jd.SalesOficerID equals so.ID
                                       where re.DealerID == RHID && jd.JobDate >= fromdate && jd.JobDate <= todate && re.RangeID==6 && so.IsActive == true
                                    

                                       select new SaleOfficerData
                                       {
                                           ID = so.ID,
                                           Name = so.Name
                                       }).Distinct().ToList();


                }
                else if (RangeID == 7)
                {
                    saleOfficerData = (from jd in dbContext.JobsDetails
                                       join re in dbContext.Retailers on jd.RetailerID equals re.ID
                                       join so in dbContext.SaleOfficers on jd.SalesOficerID equals so.ID
                                       where re.RangeBDealer == RHID && jd.JobDate >= fromdate && jd.JobDate <= todate && re.RangeID == 7 && so.IsActive == true
                                       select new SaleOfficerData
                                       {
                                           ID = so.ID,
                                           Name = so.Name
                                       }).Distinct().ToList();
                }
                else
                {
                    saleOfficerData = (from jd in dbContext.JobsDetails
                                       join re in dbContext.Retailers on jd.RetailerID equals re.ID
                                       join so in dbContext.SaleOfficers on jd.SalesOficerID equals so.ID
                                       where re.RangeCDealer == RHID && jd.JobDate >= fromdate && jd.JobDate <= todate && re.RangeID == 10 && so.IsActive == true
                                       select new SaleOfficerData
                                       {
                                           ID = so.ID,
                                           Name = so.Name
                                       }).Distinct().ToList();
                }
            }

         
            return saleOfficerData;
        }


        public static Boolean AddUpdateSOTargets(SaleOfficerTargets obj, string StartingDate2)
        {
            Boolean boolFlag = false;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_Targets saleofficerObj = new Tbl_Targets();
                    DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate2) ? DateTime.Now.ToString() : StartingDate2);
                    if (obj.ID == 0)
                    {
                        var SOList = dbContext.SaleOfficers.Where(x => x.RegionalHeadID == obj.RegionalHeadID && x.IsActive == true).Select(x => x.ID).ToList();

                        foreach (var item in SOList)
                        {

                            saleofficerObj.HeadID = obj.RegionalHeadID;
                            saleofficerObj.SOID = item;
                            saleofficerObj.FinancialYear = start;
                            saleofficerObj.VisitsTargetsTrade = obj.VisitTargetTrade;
                            saleofficerObj.VisitsTargetsHousing = obj.VisitTargetHousing;
                            saleofficerObj.VisitsTargetsCorporate = obj.VisitTargetCorporate;

                            saleofficerObj.NewCustomerTrade = obj.NewCustomerTrade;
                            saleofficerObj.NewCustomerHousing = obj.NewCustomerHousing;
                            saleofficerObj.NewCustomerCorporate = obj.NewCustomerCorporate;


                            saleofficerObj.SaleTrade = obj.SaleTrade;
                            saleofficerObj.SaleHousing = obj.SaleHousing;
                            saleofficerObj.SaleCorporate = obj.SaleCorporate;


                            saleofficerObj.RecoveriesCorporate = obj.RecoveriesCorporate;
                            saleofficerObj.RecoveriesHousing = obj.RecoveriesHousing;
                            saleofficerObj.RecoveriesTrade = obj.RecoveriesTrade;

                            saleofficerObj.UpdatedON = DateTime.UtcNow.AddHours(5);
                            saleofficerObj.CreatedOn= DateTime.UtcNow.AddHours(5);


                            dbContext.Tbl_Targets.Add(saleofficerObj);
                            dbContext.SaveChanges();

                        }

                        

                       
                    }
                    else
                    {
                       
                        saleofficerObj = dbContext.Tbl_Targets.Where(u => u.SOID == obj.SOID).FirstOrDefault();
                        
                       
                        saleofficerObj.VisitsTargetsTrade = obj.VisitTargetTrade;
                        saleofficerObj.VisitsTargetsHousing = obj.VisitTargetHousing;
                        saleofficerObj.VisitsTargetsCorporate = obj.VisitTargetCorporate;

                        saleofficerObj.NewCustomerTrade = obj.NewCustomerTrade;
                        saleofficerObj.NewCustomerHousing = obj.NewCustomerHousing;
                        saleofficerObj.NewCustomerCorporate = obj.NewCustomerCorporate;


                        saleofficerObj.SaleTrade = obj.SaleTrade;
                        saleofficerObj.SaleHousing = obj.SaleHousing;
                        saleofficerObj.SaleCorporate = obj.SaleCorporate;


                        saleofficerObj.RecoveriesCorporate = obj.RecoveriesCorporate;
                        saleofficerObj.RecoveriesHousing = obj.RecoveriesHousing;
                        saleofficerObj.RecoveriesTrade = obj.RecoveriesTrade;

                        saleofficerObj.UpdatedON = DateTime.UtcNow.AddHours(5);
                     
                        dbContext.SaveChanges();
                    }

                    boolFlag = true;
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add SalesOfficer Failed");
                boolFlag = false;
            }
            return boolFlag;
        }



        //Insert OR Update SalesOfficers ...
        public static Boolean AddUpdateSaleOfficer(SaleOfficerData obj , string StartingDate2)
        {
            Boolean boolFlag = false;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SaleOfficer saleofficerObj = new SaleOfficer();

                    if (obj.ID == 0)
                    {
                        saleofficerObj.ID = dbContext.SaleOfficers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        saleofficerObj.Name = obj.Name;
                        saleofficerObj.UserName = obj.UserName;
                        saleofficerObj.Password = obj.Password;
                        saleofficerObj.RegionalHeadID = obj.RegionalHeadID;
                        saleofficerObj.CityID = 900;
                        saleofficerObj.RangeID = 6;
                        saleofficerObj.RegionID = obj.RegionID;
                        saleofficerObj.PrimaryCityID = obj.PrimaryCityID;
                        saleofficerObj.SecondaryCityID = obj.SecondaryCityID;
                        saleofficerObj.SegmentID = obj.SegmentTypeID;
                        saleofficerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                        saleofficerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                        saleofficerObj.IsActive = true;
                        saleofficerObj.IsDeleted = false;
                        saleofficerObj.SORoleID = obj.SOTypeID;
                        saleofficerObj.ECode = obj.ECode;
                        saleofficerObj.DesignationID = obj.DesignationID;
                        saleofficerObj.RegionID = obj.RegionID;
                        saleofficerObj.CreatedDate = DateTime.Now;
                        saleofficerObj.LastUpdate = DateTime.Now;
                        saleofficerObj.LeaveOn= null;
                        //Created By Work Pending...
                        saleofficerObj.CreatedBy = 1;

                        dbContext.SaleOfficers.Add(saleofficerObj);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate2) ? DateTime.Now.ToString() : StartingDate2);
                        saleofficerObj = dbContext.SaleOfficers.Where(u => u.ID == obj.ID).FirstOrDefault();
                        saleofficerObj.Name = obj.Name;
                        saleofficerObj.UserName = obj.UserName;
                        saleofficerObj.Password = obj.Password;
                        saleofficerObj.RangeID = 6;
                        saleofficerObj.RegionalHeadID = obj.RegionalHeadID;
                        saleofficerObj.CityID = obj.RegionID;
                        saleofficerObj.RegionID = obj.RegionID;
                        saleofficerObj.PrimaryCityID = obj.PrimaryCityID;
                        saleofficerObj.SecondaryCityID = obj.SecondaryCityID;
                        saleofficerObj.SegmentID = obj.SegmentTypeID;
                        saleofficerObj.SORoleID = obj.SOTypeID;
                        saleofficerObj.IsActive = obj.IsActive;
                        saleofficerObj.ECode = obj.ECode;
                        saleofficerObj.DesignationID = obj.DesignationID;
                        saleofficerObj.RegionID = obj.RegionID;
                        saleofficerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                        saleofficerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                        saleofficerObj.LastUpdate = DateTime.Now;
                        if (obj.IsActive == false)
                        {
                            saleofficerObj.LeaveOn = start;
                        }
                        dbContext.SaveChanges();
                    }

                    boolFlag = true;
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add SalesOfficer Failed");
                boolFlag = false;
            }
            return boolFlag;
        }



        //Insert OR Update SalesOfficers ...
        public static Boolean SaleOfficerTransfer(JobsData obj)
        {
            Boolean boolFlag = false;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var Sodata = dbContext.Retailers.Where(x => x.SaleOfficerID == obj.SaleOfficerID).Select(x => x.ID).ToList();

                    foreach (var item in Sodata)
                    {
                        Retailer saleofficerObj = new Retailer();
                        saleofficerObj = dbContext.Retailers.Where(u => u.ID == item).FirstOrDefault();

                        saleofficerObj.SaleOfficerID = (int)obj.SaleOfficerToID;
                        dbContext.SaveChanges();


                    }



                    boolFlag = true;
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add SalesOfficer Failed");
                boolFlag = false;
            }
            return boolFlag;
        }


        // Delete SalesOfficer ...
        public static int DeleteSaleOfficer(int SaleOfficerID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    SaleOfficer objSaleOfficer = dbContext.SaleOfficers.Where(u => u.ID == SaleOfficerID).FirstOrDefault();
                    var objSaleOfficerArea = objSaleOfficer.Areas.ToList();
                    foreach (var area in objSaleOfficerArea)
                    {
                        objSaleOfficer.Areas.Remove(area);
                    }
                    dbContext.SaleOfficers.Remove(objSaleOfficer);
                    //obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete SalesOfficer Failed");
                Resp = 1;
            }
            return Resp;
        }


        public static List<SaleOfficerData> GetResult(string search, string sortOrder, int start, int length, List<SaleOfficerData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<SaleOfficerData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<SaleOfficerData> FilterResult(string search, List<SaleOfficerData> dtResult, List<string> columnFilters)
        {
            IQueryable<SaleOfficerData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(search.ToLower()) || p.CityName != null && p.CityName.ToLower().Contains(search.ToLower()) || p.AreaName != null && p.AreaName.ToLower().Contains(search.ToLower()) || p.Phone1 != null && p.Phone1.ToLower().Contains(search.ToLower())
                || p.Phone2 != null && p.Phone2.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[3] == null || (p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(columnFilters[3].ToLower())))
                 && (columnFilters[4] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[3].ToLower())))
                  && (columnFilters[5] == null || (p.AreaName != null && p.AreaName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[6] == null || (p.Phone1 != null && p.Phone1.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[7] == null || (p.Phone2 != null && p.Phone2.ToLower().Contains(columnFilters[5].ToLower())))
                );

            return results;
        }



        public static List<SaleOfficerTargets> GetResult12(string search, string sortOrder, int start, int length, List<SaleOfficerTargets> dtResult, List<string> columnFilters)
        {
            return FilterResult12(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count12(string search, List<SaleOfficerTargets> dtResult, List<string> columnFilters)
        {
            return FilterResult12(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<SaleOfficerTargets> FilterResult12(string search, List<SaleOfficerTargets> dtResult, List<string> columnFilters)
        {
            IQueryable<SaleOfficerTargets> results = dtResult.AsQueryable();

            //results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(search.ToLower()) || p.CityName != null && p.CityName.ToLower().Contains(search.ToLower()) || p.AreaName != null && p.AreaName.ToLower().Contains(search.ToLower()) || p.Phone1 != null && p.Phone1.ToLower().Contains(search.ToLower())
            //    || p.Phone2 != null && p.Phone2.ToLower().Contains(search.ToLower())))
            //    && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
            //    && (columnFilters[3] == null || (p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(columnFilters[3].ToLower())))
            //     && (columnFilters[4] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[3].ToLower())))
            //      && (columnFilters[5] == null || (p.AreaName != null && p.AreaName.ToLower().Contains(columnFilters[3].ToLower())))
            //    && (columnFilters[6] == null || (p.Phone1 != null && p.Phone1.ToLower().Contains(columnFilters[4].ToLower())))
            //    && (columnFilters[7] == null || (p.Phone2 != null && p.Phone2.ToLower().Contains(columnFilters[5].ToLower())))
            //    );

            return results;
        }





        public static List<RegionalHeadData> GetRegionalHeadAccordingToType(int RegionalHeadType)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.RegionalHeads.Where(u => u.Type == RegionalHeadType && u.IsDeleted == false).ToList()
                            .Select(
                                u => new RegionalHeadData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }

        public static List<RegionalHeadData> GetRegionalHeadAccordingToTypeNew(int RegionalHeadType)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            try
            {
               
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (RegionalHeadType == 2)
                    {
                        regionalHeadData = dbContext.RegionalHeads.Where(u =>u.IsDeleted==false).ToList()
                         .Select(
                             u => new RegionalHeadData
                             {
                                 ID = u.ID,
                                 Name = u.Name,
                             }).ToList();
                    }
                    else
                    {
                        regionalHeadData = dbContext.RegionalHeads.Where(u => u.Type == RegionalHeadType && u.IsDeleted == false).ToList()
                                              .Select(
                                                  u => new RegionalHeadData
                                                  {
                                                      ID = u.ID,
                                                      Name = u.Name,
                                                  }).ToList();
                    }
              
                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }
        public static List<RegionalHeadData> GetRegionsofSO(int RegionalHeadType)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.RegionalHeadRegions.Where(u => u.RegionHeadID == RegionalHeadType && u.IsDeleted == false).ToList()
                            .Select(
                                u => new RegionalHeadData
                                {
                                    ID = u.RegionID,
                                    Name = dbContext.Regions.Where(x => x.ID == u.RegionID).Select(x => x.Name).FirstOrDefault(),
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }

        public static List<SaleOfficerData> GetSaleOfficerListByRegionalHeadID(int RegionalHeadID)
        {
            List<SaleOfficerData> saleOfficerData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false && u.IsActive == true).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).OrderBy(x => x.Name).ToList();


                }
            }
            catch (Exception)
            {
                throw;
            }

            return saleOfficerData;
        }

        public static List<SaleOfficer> GetAllSaleOfficerListRelatedtoregionalHeadIDd()
        {
            List<SaleOfficer> saleOfficerData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                saleOfficerData = dbContext.SaleOfficers.Where(p => p.IsActive == true).OrderBy(p => p.Name).ToList();



            }
            return saleOfficerData;
        }

        public static List<Sp_GetItemsRelatedToJobID_Result> GetItemsAcctoID(int JobID)
        {
            List<Sp_GetItemsRelatedToJobID_Result> regionalHeadData = new List<Sp_GetItemsRelatedToJobID_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Sp_GetItemsRelatedToJobID(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }

        public static List<usp_GetClaimSummaryByClaimId_Result> GetClaimsAcctoID(int JobID)
        {
            List<usp_GetClaimSummaryByClaimId_Result> regionalHeadData = new List<usp_GetClaimSummaryByClaimId_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.usp_GetClaimSummaryByClaimId(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }


        public static List<GetClaimApprovals_Result> GetClaimsApprovalsAcctoID(int JobID)
        {
            List<GetClaimApprovals_Result> regionalHeadData = new List<GetClaimApprovals_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.GetClaimApprovals(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }




        public static List<Sp_GetDistributorOrdersAccToJobIDFinal1_0_Result> GetDistriburorOrdersAcctoID(int JobID)
        {
            List<Sp_GetDistributorOrdersAccToJobIDFinal1_0_Result> regionalHeadData = new List<Sp_GetDistributorOrdersAccToJobIDFinal1_0_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Sp_GetDistributorOrdersAccToJobIDFinal1_0(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }



        public static List<Sp_GetRetailerOrdersAccToJobID_Result> GetRetailerOrdersAcctoID(int JobID)
        {
            List<Sp_GetRetailerOrdersAccToJobID_Result> regionalHeadData = new List<Sp_GetRetailerOrdersAccToJobID_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Sp_GetRetailerOrdersAccToJobID(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }


       
        public static List<Sp_GetItemsRelatedtoStock_Result> GetStockAcctoID(int JobID)
        {
            List<Sp_GetItemsRelatedtoStock_Result> regionalHeadData = new List<Sp_GetItemsRelatedtoStock_Result>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Sp_GetItemsRelatedtoStock(JobID).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }




    }
}