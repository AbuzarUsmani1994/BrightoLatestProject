using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;                       
using System.Threading.Tasks;
using FOS.Shared;
using FOS.DataLayer;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using System.Globalization;
using Shared.Diagnostics.Logging;


namespace FOS.Setup
{

  
    public class ManageJobs
    {
        #region SmSGrids
        public static List<SendSMSData> GetSMSDetailForGridForDistributor()
        {
            List<SendSMSData> doneJobData = new List<SendSMSData>();


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    doneJobData = (from jd in dbContext.Tbl_SMSLogs.Where(u=>u.Type=="Dealer")


                                       //where(jd => jd.job.IsDeleted == false)
                                   select new SendSMSData
                                   //u => new JobsDetailData
                                   {
                                      

                                       //First Column
                                       CityID=jd.CityID,
                                       CityName= dbContext.Cities.Where(x => x.ID == jd.CityID).Select(x => x.Name).FirstOrDefault(),
                                       RegionID =jd.RegionID,
                                       RegionName = dbContext.Regions.Where(x => x.ID == jd.RegionID).Select(x => x.Name).FirstOrDefault(),
                                       DealerID = jd.DealerID,
                                       DealerName = dbContext.Dealers.Where(x => x.ID == jd.DealerID).Select(x => x.ShopName).FirstOrDefault(),
                                       Phone1= jd.Phone1,
                                       msg = jd.Message,
                                       MSgDate = jd.MessageDate,
                                       MsgStatus = (jd.Status == true) ? "Message Sent" : "Message Failed",
                                 
                                   }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }

        public static List<SendSMSData> GetSMSDetailForGridForRetailers()
        {
            List<SendSMSData> doneJobData = new List<SendSMSData>();


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    doneJobData = (from jd in dbContext.Tbl_SMSLogs.Where(u => u.Type == "Retailer")


                                       //where(jd => jd.job.IsDeleted == false)
                                   select new SendSMSData
                                   //u => new JobsDetailData
                                   {


                                       //First Column
                                       CityID = jd.CityID,
                                       CityName = dbContext.Cities.Where(x => x.ID == jd.CityID).Select(x => x.Name).FirstOrDefault(),
                                       RegionID = jd.RegionID,
                                       RegionName = dbContext.Regions.Where(x => x.ID == jd.RegionID).Select(x => x.Name).FirstOrDefault(),
                                       ReatilerID = jd.RetailerID,
                                       ReatilerName = dbContext.Retailers.Where(x => x.ID == jd.RetailerID).Select(x => x.ShopName).FirstOrDefault(),
                                       Phone1 = jd.Phone1,
                                       msg = jd.Message,
                                       MSgDate = jd.MessageDate,
                                       MsgStatus = (jd.Status == true) ? "Message Sent" : "Message Failed",

                                   }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }

        public static List<SendSMSData> GetSMSDetailForGridForSaleOfficers()
        {
            List<SendSMSData> doneJobData = new List<SendSMSData>();


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    doneJobData = (from jd in dbContext.Tbl_SMSLogs.Where(u => u.Type == "SaleOfficer")


                                       //where(jd => jd.job.IsDeleted == false)
                                   select new SendSMSData
                                   //u => new JobsDetailData
                                   {


                                       //First Column
                                       CityID = jd.CityID,
                                       CityName = dbContext.Cities.Where(x => x.ID == jd.CityID).Select(x => x.Name).FirstOrDefault(),
                                       RegionID = jd.RegionID,
                                       RegionName = dbContext.Regions.Where(x => x.ID == jd.RegionID).Select(x => x.Name).FirstOrDefault(),
                                       SaleOfficerID = jd.SaleOfficerID,
                                       SaleOfficerName = dbContext.SaleOfficers.Where(x => x.ID == jd.SaleOfficerID).Select(x => x.Name).FirstOrDefault(),
                                       Phone1 = jd.Phone1,
                                       msg = jd.Message,
                                       MSgDate = jd.MessageDate,
                                       MsgStatus = (jd.Status == true) ? "Message Sent" : "Message Failed",

                                   }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }

        public static List<SendSMSData> GetResult1(string search, string sortOrder, int start, int length, List<SendSMSData> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count1(string search, List<SendSMSData> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<SendSMSData> FilterResult1(string search, List<SendSMSData> dtResult, List<string> columnFilters)
        {
            IQueryable<SendSMSData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(search.ToLower()) || p.DealerName != null && p.DealerName.ToLower().Contains(search.ToLower()))
                && (columnFilters[3] == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.DealerName != null && p.DealerName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.ReatilerName != null && p.ReatilerName.ToLower().Contains(columnFilters[5].ToLower())))
                
               ) );

            return results;
        }


        #endregion SMSGrids

        public static List<DealerData> GetAllSaleOfficerListRelatedToDealerForSMS(int Soid, bool selectOption = false)
        {
            List<DealerData> Retailer = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Retailer = dbContext.Retailers.Where(r => r.SaleOfficerID == Soid && r.IsDeleted == false)
                            .Select(u => new DealerData
                            {
                                SaleOfficerID = u.ID,
                                SaleOfficerName = u.Name,
                                RegionalHeadID = (int)u.SaleOfficerID
                            }).OrderBy(x => x.SaleOfficerName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            if (selectOption)
            {
                Retailer.Insert(0, new DealerData
                {
                    SaleOfficerID = 0,
                    SaleOfficerName = "Select"
                });
            }

            return Retailer;
        }


        #region Territorial Job


        // Get All Jobs List ...
        public static List<Job> GetAllJobsList()
        {
            List<Job> jobsData = new List<Job>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                jobsData = dbContext.Jobs.Where(j => j.IsDeleted == false).ToList();
            }
            return jobsData;
        }


        // Get All VisitPlan List ...
        public static List<VisitPlanData> GetAllVisitList()
        {
            List<VisitPlanData> visitsData = new List<VisitPlanData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                visitsData = dbContext.VisitPlans.Where(p => p.ID == 1).Select(
                                u => new VisitPlanData
                                {
                                    ID = u.ID,
                                    Type = u.Type

                                }).ToList();
            }
            return visitsData;
        }

        
        // Get SalesOfficer List Related To Delare ...
        public static List<DealerData> GetAllSaleOfficerSelectOption(int regionHeadID)
        {
            List<DealerData> saleOfficerData = new List<DealerData>();
            saleOfficerData.Add(new DealerData
            {
                SaleOfficerID = 0,
                SaleOfficerName = "Select Any",
                RegionalHeadID = regionHeadID
            });
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var list = dbContext.SaleOfficers.Where(r => r.RegionalHeadID ==  (regionHeadID > 0 ? regionHeadID : r.RegionalHeadID) && r.IsDeleted == false)
                            .Select(u => new DealerData
                            {
                                SaleOfficerID = u.ID,
                                SaleOfficerName = u.Name,
                                RegionalHeadID = (int)u.RegionalHeadID
                            }).OrderBy(x => x.SaleOfficerName).ToList();

                    foreach (var item in list)
                    {
                        saleOfficerData.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return saleOfficerData;
        }

        // Get SalesOfficer List Related To Delare ...
        public static List<DealerData> GetAllSaleOfficerListRelatedToDealer(int RegionHeadID, bool selectOption = false)
        {
            List<DealerData> saleOfficerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(r => r.RegionalHeadID == RegionHeadID && r.IsDeleted == false)
                            .Select(u => new DealerData
                                {
                                    SaleOfficerID = u.ID,
                                    SaleOfficerName = u.Name,
                                    RegionalHeadID = (int)u.RegionalHeadID
                                }).OrderBy(x => x.SaleOfficerName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            if (selectOption)
            {
                saleOfficerData.Insert(0, new DealerData
                {
                    SaleOfficerID = 0,
                    SaleOfficerName = "All"
                });
            }

            return saleOfficerData;
        }

        public static List<DealerData> GetAllSaleOfficerListRelatedToHead(int RegionHeadID, bool selectOption = false)
        {
            List<DealerData> saleOfficerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(r => r.RegionalHeadID == RegionHeadID && r.IsActive == true)
                            .Select(u => new DealerData
                            {
                                SaleOfficerID = u.ID,
                                SaleOfficerName = u.Name,
                                RegionalHeadID = (int)u.RegionalHeadID
                            }).OrderBy(x => x.SaleOfficerName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            if (selectOption)
            {
                saleOfficerData.Insert(0, new DealerData
                {
                    SaleOfficerID = 0,
                    SaleOfficerName = "All"
                });
            }

            return saleOfficerData;
        }
        public static List<DealerData> GetAllHEADsRelatedTorange(int RegionHeadID, int userID, bool selectOption = false)
        {
            List<DealerData> saleOfficerData = new List<DealerData>();
            DealerData comlist;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (userID != 1048)
                    {

                        saleOfficerData = dbContext.RegionalHeads.Where(r => r.RangeID == RegionHeadID && r.IsDeleted == false)
                                .Select(u => new DealerData
                                {
                                    SaleOfficerID = u.ID,
                                    SaleOfficerName = u.Name,

                                }).OrderBy(x => x.SaleOfficerName).ToList();
                    }
                    else
                    {
                        var ZSMids = dbContext.RSMPortalUsers.Where(x => x.UserID == userID && x.RangeID==RegionHeadID).Select(x => x.ZSMID).Distinct().ToList();

                        foreach (var item in ZSMids)
                        {

                            comlist = new DealerData();
                            comlist.SaleOfficerID = (int)item;
                            comlist.SaleOfficerName = dbContext.RegionalHeads.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                            saleOfficerData.Add(comlist);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            if (userID == 1 || userID==1048)
            {
                saleOfficerData.Insert(0, new DealerData
                {
                    SaleOfficerID = 0,
                    SaleOfficerName = "All"
                });

            }


            return saleOfficerData;
        }
        public static List<DealerData> GetAllDealersRelatedtorange(int RegionHeadID, int regionID)
        {
            List<DealerData> saleOfficerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    saleOfficerData = dbContext.Dealers.Where(r => r.RangeID == RegionHeadID && r.IsDeleted == false && r.RegionID==regionID)
                            .Select(u => new DealerData
                            {
                                SaleOfficerID = u.ID,
                                SaleOfficerName = u.ShopName,
                               
                            }).OrderBy(x => x.SaleOfficerName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }


            return saleOfficerData;
        }
        public static List<DealerData> GetAllSaleOfficerListRelated(int RegionHeadID, bool selectOption = false)
        {
            List<DealerData> saleOfficerData = new List<DealerData>();
            DealerData comlist;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {


                    var data = dbContext.Tbl_SOREGIONS.Where(r => r.RegionID == RegionHeadID).ToList();
                    foreach (var item in data)
                    {
                        comlist = new DealerData();
                        comlist.SaleOfficerID = item.SaleofficerID;
                        comlist.SaleOfficerName = dbContext.SaleOfficers.Where(x => x.ID == item.SaleofficerID).Select(x => x.Name).FirstOrDefault();

                        saleOfficerData.Add(comlist);
                    }

                    //saleOfficerData = dbContext.Tbl_SOREGIONS.Where(r => r.RegionID == RegionHeadID)
                    //        .Select(u => new DealerData
                    //        {
                    //            SaleOfficerID = u.ID,
                    //            SaleOfficerName = dbContext.SaleOfficers.Where(x=>x.ID==u.ID).Select(x=>x.Name).FirstOrDefault(),
                    //            RegionalHeadID = (int)u.RegionalHeadID
                    //        }).OrderBy(x => x.SaleOfficerName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            if (selectOption)
            {
                saleOfficerData.Insert(0, new DealerData
                {
                    SaleOfficerID = 0,
                    SaleOfficerName = "All"
                });
            }

            return saleOfficerData;
        }

        //Get Menu, Forms & Security ...
        public static List<RetailerData> GetRetailerWithDealer(int Check, int RegionalHeadID , int SalesOfficerID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (Check == 1)
                {
                    // Check == 1 For Retailer Data
                    retailerData = dbContext.Retailers.Where(r => r.SaleOfficerID == SalesOfficerID && r.IsDeleted == false).Select(r =>
                        new RetailerData
                        {
                            ID = r.ID,
                            Name = r.Name,
                            Address = r.Address
                        }).ToList();
                }
                    // Check == 2 For Dealer Data
                else if (Check == 2)
                {
                    retailerData = dbContext.Dealers.Where(r => r.RegionalHeadID == RegionalHeadID).Select(r =>
                        new RetailerData
                        {
                            DealerID = r.ID,
                            DealerName = r.Name,
                        }).ToList();
                }
            }
            return retailerData;
        }


        // Insert OR Update Jobs ...
        public static int AddUpdateJob(JobsData obj)
        {
            int boolFlag = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Job JobObj = new Job();


                        // IF For RegionalHead Is "Territorial".
                        // Visit Type Is "Shop". 
                        // Retailer Type Is "Regular" OR "Focused". 
                        #region VisitType "SHOP" & RetailerType "REGULAR OR FOCUSED"

                        if (obj.VisitType == "Shop" && obj.Type == 1 && (obj.RetailerType == "Focused" || obj.RetailerType == "Regular"))
                        {

                            if (obj.SelectedRetailers == null && obj.Type == 1)
                            {
                                boolFlag = 2;
                            }
                            else
                            {

                                if (obj.ID == 0)
                                {
                                    JobObj.ID = dbContext.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                    JobObj.JobTitle = obj.JobTitle;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = obj.RegionalHeadID;
                                    JobObj.RetailerType = obj.RetailerType;
                                    JobObj.RegionalHeadType = obj.Type;
                                    JobObj.VisitType = obj.VisitType;
                                    JobObj.Status = false;
                                    JobObj.JobType = "A";
                                    JobObj.VisitPlanType = 1;
                                    
                                    JobObj.StartingDate = DateTime.Now;
                                    JobObj.DateOfAssign = DateTime.Now;
                                    JobObj.CreatedDate = DateTime.Now;
                                    JobObj.LastUpdated = DateTime.Now;
                                    JobObj.IsActive = true;
                                    JobObj.IsDeleted = false;

                                    if (!String.IsNullOrEmpty(obj.SelectedRetailers))
                                    {
                                        //String[] strDealerId = obj.SelectedDealers.Split(',');
                                        String[] strRetailerId = obj.SelectedRetailers.Split(',');

                                            foreach (var retailerid in strRetailerId)
                                            {
                                            
                                                JobsDetail jobDetail = new JobsDetail();
                                                int intRetailerID = Convert.ToInt32(retailerid);
                                                jobDetail.JobID = JobObj.ID;
                                               // jobDetail.DealerID = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.DealerID).FirstOrDefault();
                                                JobObj.RetailerType = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.RetailerType).FirstOrDefault();
                                                jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                                jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                                jobDetail.RetailerID = intRetailerID;
                                                
                                                jobDetail.JobDate = obj.StartingDate;
                                                jobDetail.Status = false;
                                                dbContext.JobsDetails.Add(jobDetail);
                                            }
                                
                                    }

                                    dbContext.Jobs.Add(JobObj);

                                }
                                else
                                {
                                    JobObj = dbContext.Jobs.Where(u => u.ID == obj.ID).FirstOrDefault();
                                    JobObj.JobTitle = obj.JobTitle;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = obj.RegionalHeadID;
                                    JobObj.RegionalHeadType = obj.Type;
                                    JobObj.VisitType = obj.VisitType;
                                    JobObj.Status = false;
                                    JobObj.JobType = "A";
                                    JobObj.VisitPlanType = 1;
                                   
                                    JobObj.DateOfAssign = DateTime.Now;
                                  
                                    JobObj.StartingDate = Convert.ToDateTime(obj.StartingDate);
                                    JobObj.DateOfAssign = DateTime.Now;
                                    JobObj.CreatedDate = DateTime.Now;
                                    JobObj.LastUpdated = DateTime.Now;
                                    JobObj.IsActive = false;
                                    JobObj.IsDeleted = false;

                                    var selectedRetailers = JobObj.JobsDetails.ToList();

                                    foreach (var deleteRetailer in selectedRetailers)
                                    {
                                        dbContext.JobsDetails.Remove(deleteRetailer);
                                    }
                                    if (!String.IsNullOrEmpty(obj.SelectedRetailers) && !String.IsNullOrEmpty(obj.SelectedDealers))
                                    {
                                        String[] strDealerId = obj.SelectedDealers.Split(',');
                                        String[] strRetailerId = obj.SelectedRetailers.Split(',');
                                        
                                        foreach (var retailerid in strRetailerId)
                                        {
                                            JobsDetail jobDetail = new JobsDetail();
                                            int intRetailerID = Convert.ToInt32(retailerid);
                                            jobDetail.JobID = JobObj.ID;
                                            jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                            jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                          // jobDetail.DealerID = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.DealerID).FirstOrDefault();
                                            jobDetail.RetailerID = intRetailerID;
                                            jobDetail.JobDate = obj.StartingDate;
                                            jobDetail.Status = false;
                                            dbContext.JobsDetails.Add(jobDetail);
                                        }

                                    }

                                }
                                dbContext.SaveChanges();
                                boolFlag = 1;
                                scope.Complete();
                            }

                        }

                        #endregion 

                            
                        // IF For RegionalHead Is "B2B".
                        // Visit Type Is "Painter". 
                        // Retailer Type Is "Regular" OR "Focused".

                        #region VisitType "Painter"

                        else if (obj.VisitType == "Painter" && obj.Type == 1)
                        {

                            if (obj.SelectedPainters == null && obj.Type == 1)
                            {
                                boolFlag = 3;
                            }
                            else
                            {
                                if (obj.ID == 0)
                                {

                                    JobObj.ID = dbContext.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                    JobObj.JobTitle = obj.JobTitle;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = obj.RegionalHeadID;
                                    JobObj.RegionalHeadType = obj.Type;
                                    JobObj.VisitType = obj.VisitType;
                                    JobObj.RetailerType = "";
                                    JobObj.Status = false;
                                    JobObj.JobType = "A";
                                    JobObj.VisitPlanType = 1;
                                    
                                    JobObj.StartingDate = DateTime.Now;
                                    JobObj.DateOfAssign = DateTime.Now;
                                    JobObj.CreatedDate = DateTime.Now;
                                    JobObj.LastUpdated = DateTime.Now;
                                    JobObj.IsActive = true;
                                    JobObj.IsDeleted = false;

                                    if (!String.IsNullOrEmpty(obj.SelectedPainters))
                                    {
                                        String[] strPainterId = obj.SelectedPainters.Split(',');
                                        foreach (var painterid in strPainterId)
                                        {
                                            JobsDetail jobDetail = new JobsDetail();
                                            int intPainterID = Convert.ToInt32(painterid);
                                            jobDetail.JobID = JobObj.ID;
                                            jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                            jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                            jobDetail.PainterID = intPainterID;
                                            JobObj.RetailerType = "";
                                            jobDetail.JobDate = obj.StartingDate;
                                            jobDetail.Status = false;
                                            dbContext.JobsDetails.Add(jobDetail);
                                        }
                                    }
                                    dbContext.Jobs.Add(JobObj);
                                }
                                else
                                {
                                    JobObj = dbContext.Jobs.Where(u => u.ID == obj.ID).FirstOrDefault();
                                    JobObj.JobTitle = obj.JobTitle;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = obj.RegionalHeadID;
                                    JobObj.RegionalHeadType = obj.Type;
                                    JobObj.VisitType = obj.VisitType;
                                    JobObj.Status = false;
                                    JobObj.JobType = "A";
                                    JobObj.VisitPlanType = 1;

                                    JobObj.StartingDate = DateTime.Now;
                                    JobObj.LastProcessed = obj.StartingDate;
                                    JobObj.DateOfAssign = DateTime.Now;
                                    JobObj.CreatedDate = DateTime.Now;
                                    JobObj.LastUpdated = DateTime.Now;
                                    
                                    JobObj.IsActive = true;
                                    JobObj.IsDeleted = false;

                                    var selectedPainters = JobObj.JobsDetails.ToList();

                                    foreach (var deletePainter in selectedPainters)
                                    {
                                        dbContext.JobsDetails.Remove(deletePainter);
                                    }
                                    if (!String.IsNullOrEmpty(obj.SelectedPainters))
                                    {
                                        String[] strPainterId = obj.SelectedPainters.Split(',');
                                        foreach (var painterid in strPainterId)
                                        {
                                            JobsDetail jobDetail = new JobsDetail();
                                            int intPainterID = Convert.ToInt32(painterid);
                                            jobDetail.JobID = JobObj.ID;
                                            jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                            jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                            jobDetail.PainterID = intPainterID;
                                            JobObj.RetailerType = "";
                                            jobDetail.Status = false;
                                            jobDetail.JobDate = obj.StartingDate;
                                            dbContext.JobsDetails.Add(jobDetail);
                                        }
                                    }
                                }
                                dbContext.SaveChanges();
                                boolFlag = 1;
                                scope.Complete();
                            }

                        }

                        #endregion

                        else 
                        { 
                        }

                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Monthly Job Failed");
                boolFlag = 0;
            }
            return boolFlag;
        }

        public static int AddUpdateBeatPlan(BeatPlanData obj)
        {
            int boolFlag = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Job JobObj = new Job();


                        // IF For RegionalHead Is "Territorial".
                        // Visit Type Is "Shop". 
                        // Retailer Type Is "Regular" OR "Focused". 
                        #region VisitType "SHOP" & RetailerType "REGULAR OR FOCUSED"

                        if (obj.VisitType == "Shop" && obj.Type == 1 && (obj.RetailerType == "Both" || obj.RetailerType == "Focused" || obj.RetailerType == "Regular"))
                        {
                            if (obj.SelectedRetailers == null && obj.Type == 1)
                            {
                                boolFlag = 2;
                            }
                            else
                            {
                                var dbSOObj = dbContext.SaleOfficers.Where(u => u.ID == obj.SaleOfficerID).FirstOrDefault();
                                if (obj.ID == 0)
                                {
                                    JobObj.ID = dbContext.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                    JobObj.JobTitle = "";
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = dbSOObj.RegionalHeadID;
                                    JobObj.RetailerType = obj.RetailerType;
                                    JobObj.RegionalHeadType = obj.Type;
                                    JobObj.VisitType = obj.VisitType;
                                    JobObj.Status = false;
                                    JobObj.JobType = "A";

                                    JobObj.VisitPlanType = string.IsNullOrEmpty(obj.BeatPlanType) ? 4 : int.Parse(obj.BeatPlanType);

                                    JobObj.StartingDate = DateTime.Now;
                                    JobObj.DateOfAssign = DateTime.Now;
                                    JobObj.CreatedDate = DateTime.Now;
                                    JobObj.LastUpdated = DateTime.Now;
                                    JobObj.IsActive = true;
                                    JobObj.IsDeleted = false;

                                    if (!String.IsNullOrEmpty(obj.SelectedRetailers))
                                    {
                                        //String[] strDealerId = obj.SelectedDealers.Split(',');
                                        String[] strRetailerId = obj.SelectedRetailers.Split(',');

                                        foreach (var retailerid in strRetailerId)
                                        {
                                            if (obj.BeatPlanType.Equals("4"))
                                            {
                                                var selectiveDayDate = GetSpecificDaysDate(obj.StartingDate.Value, obj.SelectiveDays);

                                                int intRetailerID = Convert.ToInt32(retailerid);
                                                //int DealerID = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.DealerID).FirstOrDefault().Value;
                                                var RetailerType = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.RetailerType).FirstOrDefault();

                                                for (int i = 0; i < 4; i++)
                                                {
                                                    JobsDetail jobDetail = new JobsDetail();

                                                    jobDetail.JobID = JobObj.ID;
                                                    //jobDetail.DealerID = DealerID;
                                                    JobObj.RetailerType = RetailerType;
                                                    jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                                    jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                                    jobDetail.RetailerID = intRetailerID;

                                                    jobDetail.VisitPlanType = string.IsNullOrEmpty(obj.BeatPlanType) ? 4 : int.Parse(obj.BeatPlanType);

                                                    if (i == 0)
                                                    {
                                                        jobDetail.JobDate = selectiveDayDate;
                                                    }
                                                    else if (i == 1)
                                                    {
                                                        jobDetail.JobDate = selectiveDayDate.AddDays(7);
                                                    }
                                                    else if (i == 2)
                                                    {
                                                        jobDetail.JobDate = selectiveDayDate.AddDays(14);
                                                    }
                                                    else if (i == 3)
                                                    {
                                                        jobDetail.JobDate = selectiveDayDate.AddDays(21);
                                                    }

                                                    jobDetail.Status = false;
                                                    dbContext.JobsDetails.Add(jobDetail);
                                                }
                                            }
                                            else if (obj.BeatPlanType.Equals("5"))
                                            {
                                                var selectiveDayDate = GetSpecificDaysDate(obj.StartingDate.Value, obj.SelectiveDays);

                                                int intRetailerID = Convert.ToInt32(retailerid);
                                               // int DealerID = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.DealerID).FirstOrDefault().Value;
                                                var RetailerType = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.RetailerType).FirstOrDefault();

                                                for (int i = 0; i < 2; i++)
                                                {
                                                    JobsDetail jobDetail = new JobsDetail();

                                                    jobDetail.JobID = JobObj.ID;
                                                    //jobDetail.DealerID = DealerID;
                                                    JobObj.RetailerType = RetailerType;
                                                    jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                                    jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                                    jobDetail.RetailerID = intRetailerID;

                                                    jobDetail.VisitPlanType = string.IsNullOrEmpty(obj.BeatPlanType) ? 5 : int.Parse(obj.BeatPlanType);

                                                    if (i == 0)
                                                    {
                                                        jobDetail.JobDate = selectiveDayDate;
                                                    }
                                                    else
                                                    {
                                                        jobDetail.JobDate = selectiveDayDate.AddDays(14);
                                                    }

                                                    jobDetail.Status = false;
                                                    dbContext.JobsDetails.Add(jobDetail);
                                                }
                                            }
                                            else
                                            {
                                                JobsDetail jobDetail = new JobsDetail();
                                                int intRetailerID = Convert.ToInt32(retailerid);
                                                jobDetail.JobID = JobObj.ID;
                                               // jobDetail.DealerID = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.DealerID).FirstOrDefault();
                                                JobObj.RetailerType = dbContext.Retailers.Where(r => r.ID == intRetailerID).Select(r => r.RetailerType).FirstOrDefault();
                                                jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                                jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                                jobDetail.RetailerID = intRetailerID;

                                                jobDetail.VisitPlanType = string.IsNullOrEmpty(obj.BeatPlanType) ? 6 : int.Parse(obj.BeatPlanType);

                                                jobDetail.JobDate = obj.StartingDate;
                                                jobDetail.Status = false;
                                                dbContext.JobsDetails.Add(jobDetail);
                                            }
                                        }
                                    }
                                    dbContext.Jobs.Add(JobObj);
                                }

                                dbContext.SaveChanges();
                                boolFlag = 1;
                                scope.Complete();
                            }
                        }

                        #endregion

                        // IF For RegionalHead Is "B2B".
                        // Visit Type Is "Painter". 
                        // Retailer Type Is "Regular" OR "Focused".

                        #region VisitType "Painter"

                        else if (obj.VisitType == "Painter" && obj.Type == 1)
                        {

                            if (obj.SelectedPainters == null && obj.Type == 1)
                            {
                                boolFlag = 3;
                            }
                            else
                            {
                                if (obj.ID == 0)
                                {

                                    JobObj.ID = dbContext.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                    JobObj.JobTitle = obj.JobTitle;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = obj.RegionalHeadID;
                                    JobObj.RegionalHeadType = obj.Type;
                                    JobObj.VisitType = obj.VisitType;
                                    JobObj.RetailerType = "";
                                    JobObj.Status = false;
                                    JobObj.JobType = "A";
                                    JobObj.VisitPlanType = 1;

                                    JobObj.StartingDate = DateTime.Now;
                                    JobObj.DateOfAssign = DateTime.Now;
                                    JobObj.CreatedDate = DateTime.Now;
                                    JobObj.LastUpdated = DateTime.Now;
                                    JobObj.IsActive = true;
                                    JobObj.IsDeleted = false;

                                    if (!String.IsNullOrEmpty(obj.SelectedPainters))
                                    {
                                        String[] strPainterId = obj.SelectedPainters.Split(',');
                                        foreach (var painterid in strPainterId)
                                        {
                                            JobsDetail jobDetail = new JobsDetail();
                                            int intPainterID = Convert.ToInt32(painterid);
                                            jobDetail.JobID = JobObj.ID;
                                            jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                            jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                            jobDetail.PainterID = intPainterID;
                                            JobObj.RetailerType = "";
                                            jobDetail.JobDate = obj.StartingDate;
                                            jobDetail.Status = false;
                                            dbContext.JobsDetails.Add(jobDetail);
                                        }
                                    }
                                    dbContext.Jobs.Add(JobObj);
                                }
                                else
                                {
                                    JobObj = dbContext.Jobs.Where(u => u.ID == obj.ID).FirstOrDefault();
                                    JobObj.JobTitle = obj.JobTitle;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = obj.RegionalHeadID;
                                    JobObj.RegionalHeadType = obj.Type;
                                    JobObj.VisitType = obj.VisitType;
                                    JobObj.Status = false;
                                    JobObj.JobType = "A";
                                    JobObj.VisitPlanType = 1;

                                    JobObj.StartingDate = DateTime.Now;
                                    JobObj.LastProcessed = obj.StartingDate;
                                    JobObj.DateOfAssign = DateTime.Now;
                                    JobObj.CreatedDate = DateTime.Now;
                                    JobObj.LastUpdated = DateTime.Now;

                                    JobObj.IsActive = true;
                                    JobObj.IsDeleted = false;

                                    var selectedPainters = JobObj.JobsDetails.ToList();

                                    foreach (var deletePainter in selectedPainters)
                                    {
                                        dbContext.JobsDetails.Remove(deletePainter);
                                    }
                                    if (!String.IsNullOrEmpty(obj.SelectedPainters))
                                    {
                                        String[] strPainterId = obj.SelectedPainters.Split(',');
                                        foreach (var painterid in strPainterId)
                                        {
                                            JobsDetail jobDetail = new JobsDetail();
                                            int intPainterID = Convert.ToInt32(painterid);
                                            jobDetail.JobID = JobObj.ID;
                                            jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                                            jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                                            jobDetail.PainterID = intPainterID;
                                            JobObj.RetailerType = "";
                                            jobDetail.Status = false;
                                            jobDetail.JobDate = obj.StartingDate;
                                            dbContext.JobsDetails.Add(jobDetail);
                                        }
                                    }
                                }
                                dbContext.SaveChanges();
                                boolFlag = 1;
                                scope.Complete();
                            }

                        }

                        #endregion

                        else
                        {
                        }

                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Monthly Job Failed");
                boolFlag = 0;
            }
            return boolFlag;
        }
        
        public static DateTime GetSpecificDaysDate(DateTime startingDate, string day)
        {
            DateTime the_date = startingDate;
            if (day.Equals("Monday"))
            {
                int num_days = System.DayOfWeek.Monday - the_date.DayOfWeek;
                if (num_days < 0) num_days += 7;

                // Add the needed number of days.
                return the_date.AddDays(num_days);
            }
            else if (day.Equals("Tuesday"))
            {
                int num_days = System.DayOfWeek.Tuesday - the_date.DayOfWeek;
                if (num_days < 0) num_days += 7;

                // Add the needed number of days.
                return the_date.AddDays(num_days);
            }
            else if (day.Equals("Wednesday"))
            {
                int num_days = System.DayOfWeek.Wednesday - the_date.DayOfWeek;
                if (num_days < 0) num_days += 7;

                // Add the needed number of days.
                return the_date.AddDays(num_days);
            }
            else if (day.Equals("Thursday"))
            {
                int num_days = System.DayOfWeek.Thursday - the_date.DayOfWeek;
                if (num_days < 0) num_days += 7;

                // Add the needed number of days.
                return the_date.AddDays(num_days);
            }
            else if (day.Equals("Friday"))
            {
                // Find the next Friday.
                // Get the number of days between the_date's
                // day of the week and Friday.
                int num_days = System.DayOfWeek.Friday - the_date.DayOfWeek;
                if (num_days < 0) num_days += 7;

                // Add the needed number of days.
                DateTime friday = the_date.AddDays(num_days);

                return friday;
            }
            else if (day.Equals("Saturday"))
            {
                int num_days = System.DayOfWeek.Saturday - the_date.DayOfWeek;
                if (num_days < 0) num_days += 7;

                // Add the needed number of days.
                return the_date.AddDays(num_days);
            }
            else if (day.Equals("Sunday"))
            {
                int num_days = System.DayOfWeek.Sunday - the_date.DayOfWeek;
                if (num_days < 0) num_days += 7;

                // Add the needed number of days.
                return the_date.AddDays(num_days);
            }

            return startingDate;
        }
        // Delete Job ...
        public static int DeleteJob(int JobID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Job obj = dbContext.Jobs.Where(u => u.ID == JobID).FirstOrDefault();
                    obj.IsDeleted = true;
                    //dbContext.Jobs.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch(Exception exp)
            {
                Log.Instance.Error(exp, "Delete Job Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static string DeleteAllJobsBySOID(int SaleOfficerId)
        {
            string msg = "ok";

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dbContext.Database.ExecuteSqlCommand("delete from jobs where SaleOfficerID=" + SaleOfficerId);                    
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete All Job Failed by sale officer id");
                msg = exp.Message;
            }
            return msg;
        }
        
        // Get All Retailer List Related To The Job ...
        public static string GetJobRetailer(int intJobID)
        {
            String strRetailerShopName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            var objJob = dbContext.Jobs.Where(s => s.ID == intJobID && s.IsDeleted == false).FirstOrDefault();
            strRetailerShopName = string.Join(",", objJob.JobsDetails.Where(r => r.Job.IsDeleted == false).Select(r => r.RetailerID));

            return strRetailerShopName;
        }


        // Get All Jobs List For Grid Related To DealerID & SalesOfficerID ...
        public static List<JobsData> GetJobsForGrid(int RegionalHeadID, int SaleOfficerID)
        {
            List<JobsData> jobData = new List<JobsData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    jobData = dbContext.Jobs.Where(j => j.RegionalHeadID == RegionalHeadID && j.SaleOfficerID == SaleOfficerID && j.IsDeleted == false)
                            .ToList().Select(
                                u => new JobsData
                                {
                                    ID = u.ID,
                                    JobTitle = u.JobTitle,
                                    SaleOfficerID = u.SaleOfficerID,
                                    SaleOfficerName = u.SaleOfficer.Name,
                                    Type = (int) u.RegionalHead.Type,
                                    RegionalHeadID = (int)u.RegionalHeadID,
                                    VisitType = u.VisitType,
                                    RetailerType = u.RetailerType,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    CityID = u.CityID,
                                    SelectedAreas = u.Areas,
                                    DateOfAssign = u.DateOfAssign.ToString(),
                                    Status = u.Status,
                                    CreatedDate = u.CreatedDate,
                                    VisitPlanID = u.VisitPlan.ID,
                                    VisitPlanName = u.VisitPlan.Type,
                                    StartingDate = (DateTime)u.StartingDate,
                                    VisitPlanDays = u.VisitPlanDays,
                                    LastUpdated = u.LastUpdated,
                                    IsActive = u.IsActive,
                                    IsDeleted = u.IsDeleted,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return jobData;
        }


        // Get All Jobs Related To SalesOfficer ...
        public static List<JobsData> GetJobsAccordingToSaleOfficer(int SaleOfficerID)
        {
            List<JobsData> areaData = new List<JobsData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Jobs.Where(j => j.SaleOfficerID == SaleOfficerID && j.IsDeleted == false)
                            .ToList().Select(
                                u => new JobsData
                                {
                                    ID = u.ID,
                                    JobTitle = u.JobTitle,
                                    SaleOfficerID = u.SaleOfficerID,
                                    SaleOfficerName = u.SaleOfficer.Name,
                                  //  RetailerID = u.RetailerID,
                                 //   RetailerName = u.Retailer.Name,
                                    DateOfAssign = u.DateOfAssign.ToString(),
                                    Status = u.Status,
                                    CreatedDate = u.CreatedDate,
                                    LastUpdated = u.LastUpdated,
                                    IsActive = u.IsActive,
                                    IsDeleted = u.IsDeleted,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return areaData;
        }

        public static List<JobsData> GetResult(string search, string sortOrder, int start, int length, List<JobsData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<JobsData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<JobsData> FilterResult(string search, List<JobsData> dtResult, List<string> columnFilters)
        {
            IQueryable<JobsData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(search.ToLower()) || p.DealerName != null && p.DealerName.ToLower().Contains(search.ToLower()) || p.VisitPlanName != null && p.VisitPlanName.ToString().ToLower().Contains(search.ToLower()) || p.DateOfAssign.ToString() != null && p.DateOfAssign.ToString().ToLower().Contains(search.ToLower())))
                && (columnFilters[3] == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.DealerName != null && p.DealerName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.VisitPlanName != null && p.VisitPlanName.ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.DateOfAssign.ToString() != null && p.DateOfAssign.ToString().ToLower().Contains(columnFilters[6].ToLower())))
                );

            return results;
        }

        // Get Retailer Related To Job In Edit Mode ...
        public static List<RetailerData> GetEditRetailer(int JobID, int RegionalHeadID, int SoID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    var so = dbContext.SaleOfficers.Where(s => s.ID == SoID).FirstOrDefault();
                    var rt = dbContext.Jobs.Where(j => j.ID != JobID && j.IsDeleted == false).SelectMany(j => j.JobsDetails.Where(jd => jd.Job.IsDeleted == false).Select(jd => jd.RetailerID));

                    retailerData = dbContext.Retailers.Where(r => r.SaleOfficerID == SoID
                        && r.SaleOfficer.RegionalHeadID == RegionalHeadID
                        && !rt.Contains(r.ID)
                        && r.Status == true)
                        .Select(
                             u => new RetailerData
                             {
                                 ID = u.ID,
                                 Name = u.Name,
                                 //DealerID = u.DealerID,
                                 DealerName = u.Dealer.Name,
                                 LocationName = u.LocationName,
                                 RetailerJobStatus = u.JobsDetails.Where(s => s.JobID == JobID && s.Retailer.SaleOfficerID == SoID && s.Job.SaleOfficer.ID == SoID && s.Retailer.Status == true).Count() == 0 ? false : true
                             }).ToList();



                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Edit Retailer Failed");
                throw;
            }

            return retailerData;
        }

        #endregion


        #region B2B Job

        // Insert OR Update Jobs ...
        public static int AddUpdateB2BJob(JobsData obj)
        {
            int boolFlag = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Job JobObj = new Job();

                                if (obj.ID == 0)
                                {

                                    JobObj.ID = dbContext.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                    JobObj.JobTitle = obj.JobTitle;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = obj.RegionalHeadID;
                                    JobObj.RetailerType = "";
                                    JobObj.RegionalHeadType = obj.Type;
                                    JobObj.VisitType = "B2B";
                                    JobObj.JobType = "A";
                                    JobObj.CityID = obj.CityID;

                                    JobObj.Status = false;
                                    JobObj.VisitPlanType = (int)obj.VisitPlanID;

                                    if (obj.VisitPlanID == 1)
                                    {
                                        JobObj.StartingDate = DateTime.Now;
                                        JobObj.LastProcessed = obj.StartingDate;

                                        JobObj.DateOfAssign = DateTime.Now;
                                        JobObj.CreatedDate = DateTime.Now;
                                        JobObj.LastUpdated = DateTime.Now;
                                    }

                                    if (obj.VisitPlanID == 2)
                                    {
                                        if (obj.VisitPlanWeeklyDays != "0" && obj.VisitPlanEachDays != ",,,,,,,")
                                        {
                                            JobObj.StartingDate = DateTime.Now;
                                            for (int i = 0; i < 8; i++)
                                            {
                                                if (DateTime.Now.AddDays(i).DayOfWeek.ToString() == obj.VisitPlanWeeklyDays)
                                                {
                                                    JobObj.LastProcessed = DateTime.Now.AddDays(i);
                                                    break;
                                                }
                                            }
                                            JobObj.DateOfAssign = DateTime.Now;
                                            JobObj.CreatedDate = DateTime.Now;
                                            JobObj.LastUpdated = DateTime.Now;
                                        }
                                    }

                                    if (obj.VisitPlanID == 3)
                                    {
                                        // If VisitPlanWeeklyDays == 0 [TRUE] DN Daily Job
                                        if (obj.VisitPlanWeeklyDays == "0")
                                        {
                                            JobObj.VisitPlanDays = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday";
                                            JobObj.LastProcessed = DateTime.Now;
                                        }
                                        else
                                        {
                                            string dayselsected = obj.VisitPlanWeeklyDays;
                                            JobObj.VisitPlanDays = obj.VisitPlanWeeklyDays;
                                        }
                                        JobObj.StartingDate = DateTime.Now;
                                        JobObj.DateOfAssign = DateTime.Now;
                                        JobObj.CreatedDate = DateTime.Now;
                                        JobObj.LastUpdated = DateTime.Now;
                                    }

                                    JobObj.IsActive = true;
                                    JobObj.IsDeleted = false;

                                    

                                    if (!String.IsNullOrEmpty(obj.SelectedAreas))
                                    {
                                        JobObj.Areas = obj.SelectedAreas;
                                    }
                                    else 
                                    {
                                        JobObj.Areas = "";
                                    }

                                    dbContext.Jobs.Add(JobObj);

                                }
                                else
                                {
                                    JobObj = dbContext.Jobs.Where(u => u.ID == obj.ID).FirstOrDefault();
                                    JobObj.JobTitle = obj.JobTitle;
                                    JobObj.SaleOfficerID = (int)obj.SaleOfficerID;
                                    JobObj.RegionalHeadID = obj.RegionalHeadID;
                                    JobObj.RetailerType = "";
                                    JobObj.RegionalHeadType = obj.Type;
                                    JobObj.VisitType = "B2BVisit";
                                    JobObj.JobType = "A";
                                    JobObj.CityID = obj.CityID;

                                    JobObj.Status = false;
                                    JobObj.VisitPlanType = obj.VisitPlanHiddenID;

                                    if (obj.VisitPlanID == 1)
                                    {
                                        JobObj.StartingDate = DateTime.Now;
                                        JobObj.LastProcessed = obj.StartingDate;

                                        JobObj.DateOfAssign = DateTime.Now;
                                        JobObj.CreatedDate = DateTime.Now;
                                        JobObj.LastUpdated = DateTime.Now;
                                    }

                                    if (obj.VisitPlanID == 2)
                                    {
                                        if (obj.VisitPlanWeeklyDays != "0" && obj.VisitPlanEachDays != ",,,,,,,")
                                        {
                                            JobObj.StartingDate = DateTime.Now;
                                            for (int i = 0; i < 8; i++)
                                            {
                                                if (DateTime.Now.AddDays(i).DayOfWeek.ToString() == obj.VisitPlanWeeklyDays)
                                                {
                                                    JobObj.LastProcessed = DateTime.Now.AddDays(i);
                                                    break;
                                                }
                                            }
                                            JobObj.DateOfAssign = DateTime.Now;
                                            JobObj.CreatedDate = DateTime.Now;
                                            JobObj.LastUpdated = DateTime.Now;
                                        }
                                    }

                                    if (obj.VisitPlanID == 3)
                                    {
                                        // If VisitPlanWeeklyDays == 0 [TRUE] DN Daily Job
                                        if (obj.VisitPlanWeeklyDays == "0")
                                        {
                                            JobObj.VisitPlanDays = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday";
                                            JobObj.LastProcessed = DateTime.Now;
                                        }
                                        else
                                        {
                                            string dayselsected = obj.VisitPlanWeeklyDays;
                                            JobObj.VisitPlanDays = obj.VisitPlanWeeklyDays;
                                        }
                                        JobObj.StartingDate = DateTime.Now;
                                        JobObj.DateOfAssign = DateTime.Now;
                                        JobObj.CreatedDate = DateTime.Now;
                                        JobObj.LastUpdated = DateTime.Now;
                                    }

                                    JobObj.IsActive = false;
                                    JobObj.IsDeleted = false;

                                    if (!String.IsNullOrEmpty(obj.SelectedAreas))
                                    {
                                        JobObj.Areas = obj.SelectedAreas;
                                    }
                                    else
                                    {
                                        JobObj.Areas = "";
                                    }

                                }
                                dbContext.SaveChanges();
                                boolFlag = 1;
                                scope.Complete();
                            }

                        }

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add B2B Job Failed");
                boolFlag = 0;
            }
            return boolFlag;
        }

        #endregion


        #region Job Detail


        // Get Retailer Related To Job In Edit Mode ...
        public static List<JobsDetailData> GetJobsDetailForGrid()
        {
            List<JobsDetailData> doneJobData = new List<JobsDetailData>();


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    doneJobData = (from jd in dbContext.JobsDetails
                                   join
                                    r in dbContext.Dealers on jd.DealerID equals r.ID

                                   where (jd.JobType == "Distributor Order")
                                   select new JobsDetailData
                                   //u => new JobsDetailData
                                   {
                                       ID = jd.JobID,

                                       //First Column
                                       SaleOfficerID = (int)jd.Job.SaleOfficerID,
                                       SaleOfficerName = jd.Job.SaleOfficer.Name,
                                       DealerID = (jd.Job.JobsDetails.Select(j => j.DealerID).FirstOrDefault() == null) ? 0 : jd.Job.JobsDetails.Select(j => j.DealerID).FirstOrDefault(),
                                       DealerName = (jd.Job.JobsDetails.Select(j => j.Dealer.Name).FirstOrDefault() == null) ? "-" : jd.Job.JobsDetails.Select(j => j.Dealer.Name).FirstOrDefault(),
                                       RetailerName = (jd.Retailer.Name == null) ? "-" : jd.Retailer.Name,
                                       ShopName = r.ShopName,
                                       DealerPhone=r.Phone1,
                                       RetailerAddress = jd.Retailer.Address,
                                       RegionID = r.RegionID,
                                       RegionName=r.Region.Name,
                                       VisitPlanName = jd.VisitPurpose,
                                       AssignDate = jd.JobDate,
                                       VisitedDate = jd.DateComplete,
                                       
                                       VisitDate = jd.DateComplete == null ? null : jd.DateComplete,
                                       // dateformat =Convert.ToDateTime(u.DateComplete).ToString("dd-MMM-yyyy"),
                                       //Second Column


                                       VisitType = jd.ActivityType,

                                       // Retailer
                                       RetailerType = jd.ActivityType,
                                       SAvailable = jd.SAvailable,
                                       SQuantity1KG = jd.SQuantity1KG,
                                       SQuantity5KG = jd.SQuantity5KG,
                                       SNewOrder = jd.SNewOrder,
                                       SNewQuantity1KG = jd.SNewQuantity1KG,
                                       SNewQuantity5KG = jd.SNewQuantity5KG,
                                       SPreviousOrder1KG = jd.SPreviousOrder1KG,
                                       SPreviousOrder5KG = jd.SPreviousOrder5KG,
                                       SPOSMaterialAvailable = jd.SPOSMaterialAvailable,
                                       SImage = jd.SImage,
                                       SNote = jd.SNote,
                                       ///END

                                       // Painter
                                       PUseWC = jd.PUseWC,
                                       PUseWC1KG = jd.PUseWC1KG,
                                       PUseWC5KG = jd.PUseWC5KG,
                                       PNewOrder = jd.PNewOrder,
                                       PNewOrder1KG = jd.PNewOrder1KG,
                                       PNewOrder5KG = jd.PNewOrder5KG,
                                       PNewLead = jd.PNewLead,
                                       PNewLeadMobNo = jd.PNewLeadMobNo,
                                       PRemarks = jd.PRemarks,
                                       ///END

                                       // B2B
                                       AreaID = jd.BAreaID,
                                       BAreaName = jd.Area.Name,
                                       BShop = (jd.BShop == null) ? "-" : jd.BShop,
                                       BOldHouse = (jd.BOldHouse == null) ? "-" : jd.BOldHouse,
                                       BNewHouse = (jd.BNewHouse == null) ? "-" : jd.BNewHouse,
                                       BParking = (jd.BParking == null) ? "-" : jd.BParking,
                                       BPlazaBasement = (jd.BPlazaBasement == null) ? "-" : jd.BPlazaBasement,
                                       BFactoryArea = (jd.BFactoryArea == null) ? "-" : jd.BFactoryArea,
                                       BMosque = (jd.BMosque == null) ? "-" : jd.BMosque,
                                       BOthers = (jd.BOthers == null) ? "-" : jd.BOthers,
                                       BLead = (jd.BLead == true) ? true : false,
                                       BSampleApplied = (jd.BSampleApplied == true) ? true : false,
                                       BRemarks = jd.BRemarks,
                                       ///END

                                       StatusChecker = jd.ActivityDetails,
                                   }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }


        // Get Retailer Related To Job In Edit Mode ...
        public static List<JobsDetailData> GetJobsDetailView(int JobDetailID)
        {
            List<JobsDetailData> doneJobData = new List<JobsDetailData>();


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    doneJobData = dbContext.JobsDetails.Where(j => j.ID == JobDetailID && j.Job.IsDeleted == false )
                                .Select(
                                    u => new JobsDetailData
                                    {
                                        ID = u.ID,

                                        //First Column
                                        SaleOfficerID = (int)u.Job.SaleOfficerID,
                                        SaleOfficerName = u.Job.SaleOfficer.Name,
                                        DealerID = (u.Job.JobsDetails.Select(j => j.DealerID).FirstOrDefault() == null) ? 0 : u.Job.JobsDetails.Select(j => j.DealerID).FirstOrDefault(),
                                        DealerName = (u.Job.JobsDetails.Select(j => j.Dealer.Name).FirstOrDefault() == null) ? "-" : u.Job.JobsDetails.Select(j => j.Dealer.Name).FirstOrDefault(),
                                        RetailerName = (u.Retailer.Name == null) ? "-" : u.Retailer.Name,
                                        RetailerAddress = u.Retailer.Address,
                                        PainterName = "",//dbContext.vPainters.Where(p => p.ID == u.PainterID).Select(p => p.Name).FirstOrDefault(),
                                        PainterAddress = "",//dbContext.vPainters.Where(p => p.ID == u.PainterID).Select(p => p.Address).FirstOrDefault(),
                                        VisitPlanName = u.Job.VisitPlan.Type,
                                        AssignDate = u.Job.DateOfAssign,
                                        VisitedDate =  u.DateComplete,
                                        VisitDate = u.DateComplete == null ? null : u.DateComplete,

                                        //Second Column
                                       

                                        VisitType = u.Job.VisitType,

                                        // Retailer
                                        RetailerType = u.Retailer.Type,
                                        SAvailable = u.SAvailable,
                                        SQuantity1KG = u.SQuantity1KG,
                                        SQuantity5KG = u.SQuantity5KG,
                                        SNewOrder = u.SNewOrder,
                                        SNewQuantity1KG = u.SNewQuantity1KG,
                                        SNewQuantity5KG = u.SNewQuantity5KG,
                                        SPreviousOrder1KG = u.SPreviousOrder1KG,
                                        SPreviousOrder5KG = u.SPreviousOrder5KG,
                                        SPOSMaterialAvailable = u.SPOSMaterialAvailable,
                                        SImage = u.SImage,
                                        SNote = u.SNote,
                                        ///END

                                        // Painter
                                        PUseWC = u.PUseWC,
                                        PUseWC1KG = u.PUseWC1KG,
                                        PUseWC5KG = u.PUseWC5KG,
                                        PNewOrder = u.PNewOrder,
                                        PNewOrder1KG = u.PNewOrder1KG,
                                        PNewOrder5KG = u.PNewOrder5KG,
                                        PNewLead = u.PNewLead,
                                        PNewLeadMobNo = u.PNewLeadMobNo,
                                        PRemarks = u.PRemarks,
                                        ///END

                                        // B2B
                                        AreaID = u.BAreaID,
                                        BAreaName = u.Area.Name,
                                        BShop = (u.BShop == null) ? "-" : u.BShop,
                                        BOldHouse = (u.BOldHouse == null) ? "-" : u.BOldHouse,
                                        BNewHouse = (u.BNewHouse == null) ? "-" : u.BNewHouse,
                                        BParking = (u.BParking == null) ? "-" : u.BParking,
                                        BPlazaBasement = (u.BPlazaBasement == null) ? "-" : u.BPlazaBasement,
                                        BFactoryArea = (u.BFactoryArea == null) ? "-" : u.BFactoryArea,
                                        BMosque = (u.BMosque == null) ? "-" : u.BMosque,
                                        BOthers = (u.BOthers == null) ? "-" : u.BOthers,
                                        BLead = (u.BLead == true) ? true : false,
                                        BSampleApplied = (u.BSampleApplied == true) ? true : false,
                                        BRemarks = u.BRemarks,
                                        ///END

                                        StatusChecker = (u.Status == true) ? "DONE" : "PENDING",
                                    }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }


        // Get Retailer Related To Job In Edit Mode ...
        public static List<JobsDetailData> GetJobsDetailForGrid(int RegionalHeadID)
        {
            List<JobsDetailData> doneJobData = new List<JobsDetailData>();


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    doneJobData = dbContext.JobsDetails
                                .Where(u => u.RegionalHeadID == RegionalHeadID && u.Job.IsDeleted == false)
                                .Select(
                                    u => new JobsDetailData
                                    {
                                        ID = u.ID,

                                        //First Column
                                        SaleOfficerID = (int)u.Job.SaleOfficerID,
                                        SaleOfficerName = u.Job.SaleOfficer.Name,
                                        DealerID = (int)u.Job.JobsDetails.Select(j => j.DealerID).FirstOrDefault(),
                                        DealerName = u.Job.JobsDetails.Select(j => j.Dealer.Name).FirstOrDefault(),
                                        RetailerName = (u.Retailer.Name == null) ? "-" : u.Retailer.Name,
                                        RetailerAddress = u.Retailer.Address,
                                        PainterName = "",//dbContext.vPainters.Where(p => p.ID == u.PainterID).Select(p => p.Name).FirstOrDefault(),
                                        PainterAddress = "",//dbContext.vPainters.Where(p => p.ID == u.PainterID).Select(p => p.Address).FirstOrDefault(),
                                        VisitPlanName = u.Job.VisitPlan.Type,
                                        AssignDate = u.Job.DateOfAssign,
                                        VisitedDate = u.DateComplete,
                                        VisitDate = u.DateComplete == null ? null : u.DateComplete,

                                        //Second Column
                                       
                                        StatusChecker = (u.Status == true) ? "DONE" : "PENDING",
                                    }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }


        public static List<JobsDetailData> GetResult1(string search, string sortOrder, int start, int length, List<JobsDetailData> dtResult, List<string> columnFilters/* string saleofficer, string StartingDate1,string StartingDate2*/)
        {
            return FilterResult1(search, dtResult, columnFilters/*saleofficer,StartingDate1,StartingDate2*/).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count1(string search, List<JobsDetailData> dtResult, List<string> columnFilters /*string saleofficer, string StartingDate1, string StartingDate2*/)
        {
            return FilterResult1(search, dtResult, columnFilters/*saleofficer, StartingDate1, StartingDate2*/).Count();
        }


        private static IQueryable<JobsDetailData> FilterResult1(string search, List<JobsDetailData> dtResult, List<string> columnFilters /*string saleofficer,string StartingDate1, string StartingDate2*/)
        {
            IQueryable<JobsDetailData> results = dtResult.AsQueryable();

           // DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate1) ? DateTime.Now.ToString() : StartingDate1);
            //DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate2) ? DateTime.Now.ToString() : StartingDate2);

            results = results.Where(p => (search == null|| (p.SaleOfficerName != "All" && p.SaleOfficerName.ToLower().Contains(search.ToLower()) 
              /*p.VisitedDate>=start && p.VisitedDate<=end)*/)
                && (columnFilters[2] == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.RetailerName != null && p.RetailerName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.VisitPlanName != null && p.VisitPlanName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.VisitedDate.ToString() != null && p.VisitedDate.ToString().ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.StatusChecker != null && p.StatusChecker.ToLower().Contains(columnFilters[6].ToLower())))
                ));

            return results;
        }


        #endregion


        public static List<JobsDetailData> GetResult11(string search, string sortOrder, int start, int length, List<JobsDetailData> dtResult, List<string> columnFilters/* string saleofficer, string StartingDate1,string StartingDate2*/)
        {
            return FilterResult11(search, dtResult, columnFilters/*saleofficer,StartingDate1,StartingDate2*/).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count11(string search, List<JobsDetailData> dtResult, List<string> columnFilters /*string saleofficer, string StartingDate1, string StartingDate2*/)
        {
            return FilterResult11(search, dtResult, columnFilters/*saleofficer, StartingDate1, StartingDate2*/).Count();
        }


        private static IQueryable<JobsDetailData> FilterResult11(string search, List<JobsDetailData> dtResult, List<string> columnFilters /*string saleofficer,string StartingDate1, string StartingDate2*/)
        {
            IQueryable<JobsDetailData> results = dtResult.AsQueryable();

            // DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate1) ? DateTime.Now.ToString() : StartingDate1);
            //DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate2) ? DateTime.Now.ToString() : StartingDate2);

            results = results.Where(p => (search == null || (p.DealerName != "All" && p.DealerName.ToLower().Contains(search.ToLower())
              /*p.VisitedDate>=start && p.VisitedDate<=end)*/)
                && (columnFilters[2] == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.RetailerName != null && p.RetailerName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.VisitPlanName != null && p.VisitPlanName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.VisitedDate.ToString() != null && p.VisitedDate.ToString().ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.StatusChecker != null && p.StatusChecker.ToLower().Contains(columnFilters[6].ToLower())))
                ));

            return results;
        }













        public static List<ShowJobData> GetJobsRelatedToSO(int SOID)
        {
            try
            { 
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.JobsDetails.Where(jd => jd.SalesOficerID == SOID).Select(jd => new ShowJobData
                        {
                            SalesOfficerID = (int)jd.SalesOficerID,
                            SalesOfficerName = jd.Job.SaleOfficer.Name,
                            DealerID = (int)jd.DealerID,
                            DealerName = jd.Dealer.Name,
                            RetailerID = (int)jd.RetailerID,
                            RetailerName = jd.Retailer.Name,
                            ShopName = (int)jd.RetailerID == null ? "" : jd.Retailer.ShopName,
                            PainterID = (int)jd.PainterID == null ? 0 : (int)jd.PainterID,
                            PainterName = (int)jd.PainterID == null ? "" : "",//dbContext.vPainters.Where(p => p.ID == jd.PainterID).Select(p => p.Name).FirstOrDefault(),

                        VisitPlanType = (int)jd.Job.VisitPlanType,
                            JobDate = (DateTime)jd.JobDate
                        }).ToList();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public static List<JobsDetailData> GetStockDetailForGrid()
        {
            List<JobsDetailData> doneJobData = new List<JobsDetailData>();


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    doneJobData = (from jd in dbContext.Tbl_MasterStock
                                   
                                
                                   //where(jd => jd.job.IsDeleted == false)
                                   select new JobsDetailData
                                   //u => new JobsDetailData
                                   {
                                       ID = jd.ID,

                                       //First Column
                                       SaleOfficerID = jd.SaleOfficerID,
                                       SaleOfficerName = jd.SaleOfficer.Name,
                                       DealerID = jd.DistributorID,
                                       DealerName = dbContext.Dealers.Where(x=>x.ID== jd.DistributorID).Select(x=>x.ShopName).FirstOrDefault(),
                                       DealerPhone= dbContext.Dealers.Where(x => x.ID == jd.DistributorID).Select(x => x.Phone1).FirstOrDefault(),
                                       RegionID= dbContext.Dealers.Where(x => x.ID == jd.DistributorID).Select(x => x.RegionID).FirstOrDefault(),
                                       RetailerAddress = jd.Retailer.Location,
                                       VisitPlanName = "Stock Taking",
                                       VisitedDate = jd.CreatedOn,
                             
                                       StatusChecker = "Done",
                                   }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }
        public static List<JobsDetailData> GetStockDetailForGridRelatedToHead( int HeadID)
        {
            List<JobsDetailData> doneJobData = new List<JobsDetailData>();


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    doneJobData = (from jd in dbContext.Tbl_MasterStock


                                       //where(jd => jd.job.IsDeleted == false)
                                   select new JobsDetailData
                                   //u => new JobsDetailData
                                   {
                                       ID = jd.ID,

                                       //First Column
                                       SaleOfficerID = jd.SaleOfficerID,
                                       SaleOfficerName = jd.SaleOfficer.Name,
                                       DealerID = jd.DistributorID,
                                       DealerName = dbContext.Dealers.Where(x => x.ID == jd.DistributorID).Select(x => x.ShopName).FirstOrDefault(),
                                       DealerPhone = dbContext.Dealers.Where(x => x.ID == jd.DistributorID).Select(x => x.Phone1).FirstOrDefault(),
                                       RegionID = dbContext.Dealers.Where(x => x.ID == jd.DistributorID).Select(x => x.RegionID).FirstOrDefault(),
                                       RetailerAddress = jd.Retailer.Location,
                                       VisitPlanName = "Stock Taking",
                                       VisitedDate = jd.CreatedOn,

                                       StatusChecker = "Done",
                                   }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }

























        public static List<ShowJobData> GetJobsMonthwiseOfRegHead(int regionalHeadId, string month, string plan)
        {
            DateTime fromDate = DateTime.Parse("01 " + month);
            var lastDay = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
            DateTime toDate = DateTime.Parse(lastDay + " " + month + " 23:59:59");
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (plan.Equals("beat"))
                    {
                        return dbContext.JobsDetails.Where(jd => jd.RegionalHeadID == (regionalHeadId > 0 ? regionalHeadId : jd.RegionalHeadID)
                            && jd.Job.IsDeleted == false && jd.Job.IsActive == true
                            && jd.Job.VisitPlanType >= 4 && jd.Job.VisitPlanType <= 6
                            && jd.JobDate >= fromDate && jd.JobDate <= toDate).Select(jd => new ShowJobData
                            {
                                SalesOfficerID = (int)jd.SalesOficerID,
                                SalesOfficerName = jd.Job.SaleOfficer.Name,
                                DealerID = (int)jd.DealerID,
                                DealerName = jd.Dealer.Name,
                                RetailerID = (int)jd.RetailerID,
                                RetailerName = jd.Retailer.Name,
                                ShopName = !jd.RetailerID.HasValue || jd.RetailerID == 0 ? "" : jd.Retailer.ShopName,
                                PainterID = !jd.PainterID.HasValue || jd.PainterID == 0 ? 0 : (int)jd.PainterID,
                                PainterName = !jd.PainterID.HasValue || jd.PainterID == 0 ? "" : "",//dbContext.vPainters.Where(p => p.ID == jd.PainterID).Select(p => p.Name).FirstOrDefault(),
                                Status = jd.Status ?? false,
                                VisitPlanType = (int)jd.Job.VisitPlanType,
                                JobDate = (DateTime)jd.JobDate,
                                DateComplete = jd.DateComplete,
                                JobDetailID = jd.ID,
                                JobID = jd.Job.ID
                            }).ToList();
                    }
                    else
                    {
                        var list = dbContext.JobsDetails.Where(jd => jd.RegionalHeadID == (regionalHeadId > 0 ? regionalHeadId : jd.RegionalHeadID)
                            && jd.Job.IsDeleted == false && jd.Job.IsActive == true
                            && jd.JobDate >= fromDate && jd.JobDate <= toDate).Select(jd => new ShowJobData
                            {
                                SalesOfficerID = (int)jd.SalesOficerID,
                                SalesOfficerName = jd.Job.SaleOfficer.Name,
                                DealerID = (int)jd.DealerID,
                                DealerName = jd.Dealer.Name,
                                RetailerID = (int)jd.RetailerID,
                                RetailerName = jd.Retailer.Name,
                                ShopName = !jd.RetailerID.HasValue || jd.RetailerID == 0 ? "" : jd.Retailer.ShopName,
                                PainterID = !jd.PainterID.HasValue || jd.PainterID == 0 ? 0 : (int)jd.PainterID,
                                PainterName = !jd.PainterID.HasValue || jd.PainterID == 0 ? "" : "",//dbContext.vPainters.Where(p => p.ID == jd.PainterID).Select(p => p.Name).FirstOrDefault(),
                                Status = jd.Status ?? false,
                                VisitPlanType = (int)jd.Job.VisitPlanType,
                                JobDate = (DateTime)jd.JobDate,
                                DateComplete = jd.DateComplete,
                                JobDetailID = jd.ID,
                                JobID = jd.Job.ID
                            }).ToList();

                        if (list != null && list.Count > 0)
                        {
                            var sortedList = list.OrderBy(x => x.JobDate);
                            List<ShowJobData> finalList = new List<ShowJobData>();
                            //DateTime dt;
                            HashSet<DateTime> dates = new HashSet<DateTime>();
                            foreach (var job in sortedList)
                            {
                                dates.Add(job.JobDate);
                            }
                            int visitedCount = 0;
                            int unvisitedCount = 0;
                            foreach (var dt in dates)
                            {
                                visitedCount = 0;
                                unvisitedCount = 0;
                                foreach (var j in sortedList)
                                {
                                    if(j.JobDate == dt)
                                    {
                                        if (j.Status)
                                        {
                                            visitedCount++;
                                        }
                                        else
                                        {
                                            unvisitedCount++;
                                        }
                                    }
                                }
                                finalList.Add(new ShowJobData
                                {
                                    JobDate = dt,
                                    Status = true,
                                    ShopName = "Visited = "+visitedCount,
                                    Count = visitedCount
                                });
                                finalList.Add(new ShowJobData
                                {
                                    JobDate = dt,
                                    Status = false,
                                    DateComplete = dt,
                                    ShopName = "Unvisited = " + unvisitedCount,
                                    Count = unvisitedCount
                                });
                            }

                            return finalList;
                        }

                        return list;
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "GetJobsMonthwiseOfRegHead List Failed");
                return new List<ShowJobData>();
            }
        }

        public static List<ShowJobData> GetJobsYearwise(int SOID, string year, string plan)
        {
            try
            {
                DateTime fromDate = DateTime.Parse("01 jan " + year);
                DateTime toDate = DateTime.Parse("31 dec " + year + " 23:59:59");

                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (plan.Equals("beat"))
                    {
                        return dbContext.JobsDetails.Where(jd => jd.SalesOficerID == (SOID > 0 ? SOID : jd.SalesOficerID)
                            && jd.Job.IsDeleted == false && jd.Job.IsActive == true
                            && jd.Job.VisitPlanType >= 4 && jd.Job.VisitPlanType <= 6
                            && jd.JobDate >= fromDate && jd.JobDate <= toDate).Select(jd => new ShowJobData
                            {
                                SalesOfficerID = (int)jd.SalesOficerID,
                                SalesOfficerName = jd.Job.SaleOfficer.Name,
                                DealerID = (int)jd.DealerID,
                                DealerName = jd.Dealer.Name,
                                RetailerID = (int)jd.RetailerID,
                                RetailerName = jd.Retailer.Name,
                                ShopName = !jd.RetailerID.HasValue || jd.RetailerID == 0 ? "" : jd.Retailer.ShopName,
                                PainterID = !jd.PainterID.HasValue || jd.PainterID == 0 ? 0 : (int)jd.PainterID,
                                PainterName = !jd.PainterID.HasValue || jd.PainterID == 0 ? "" : "",//dbContext.vPainters.Where(p => p.ID == jd.PainterID).Select(p => p.Name).FirstOrDefault(),
                                Status = jd.Status ?? false,
                                VisitPlanType = (int)jd.Job.VisitPlanType,
                                JobDate = (DateTime)jd.JobDate,
                                DateComplete = jd.DateComplete,
                                JobDetailID = jd.ID,
                                JobID = jd.Job.ID
                            }).ToList();
                    }
                    else
                    {
                        var list = dbContext.JobsDetails.Where(jd => jd.SalesOficerID == (SOID > 0 ? SOID : jd.SalesOficerID)
                            && jd.Job.IsDeleted == false && jd.Job.IsActive == true
                            && jd.JobDate >= fromDate && jd.JobDate <= toDate).Select(jd => new ShowJobData
                            {
                                SalesOfficerID = (int)jd.SalesOficerID,
                                SalesOfficerName = jd.Job.SaleOfficer.Name,
                                DealerID = (int)jd.DealerID,
                                DealerName = jd.Dealer.Name,
                                RetailerID = (int)jd.RetailerID,
                                RetailerName = jd.Retailer.Name,
                                ShopName = !jd.RetailerID.HasValue || jd.RetailerID == 0 ? "" : jd.Retailer.ShopName,
                                PainterID = !jd.PainterID.HasValue || jd.PainterID == 0 ? 0 : (int)jd.PainterID,
                                PainterName = !jd.PainterID.HasValue || jd.PainterID == 0 ? "" : "",//dbContext.vPainters.Where(p => p.ID == jd.PainterID).Select(p => p.Name).FirstOrDefault(),
                                Status = jd.Status ?? false,
                                VisitPlanType = (int)jd.Job.VisitPlanType,
                                JobDate = (DateTime)jd.JobDate,
                                DateComplete = jd.DateComplete,
                                JobDetailID = jd.ID,
                                JobID = jd.Job.ID
                            }).ToList();

                        if (list != null && list.Count > 0)
                        {
                            var sortedList = list.OrderBy(x => x.JobDate);
                            List<ShowJobData> finalList = new List<ShowJobData>();
                            //DateTime dt;
                            HashSet<DateTime> dates = new HashSet<DateTime>();
                            foreach (var job in sortedList)
                            {
                                dates.Add(job.JobDate);
                            }
                            int visitedCount = 0;
                            int unvisitedCount = 0;
                            foreach (var dt in dates)
                            {
                                visitedCount = 0;
                                unvisitedCount = 0;
                                foreach (var j in sortedList)
                                {
                                    if (j.JobDate == dt)
                                    {
                                        if (j.Status)
                                        {
                                            visitedCount++;
                                        }
                                        else
                                        {
                                            unvisitedCount++;
                                        }
                                    }
                                }
                                finalList.Add(new ShowJobData
                                {
                                    JobDate = dt,
                                    Status = true,
                                    ShopName = "Visited = " + visitedCount,
                                    Count = visitedCount
                                });
                                finalList.Add(new ShowJobData
                                {
                                    JobDate = dt,
                                    Status = false,
                                    ShopName = "Unvisited = " + unvisitedCount,
                                    Count = unvisitedCount
                                });
                            }
                            return finalList;
                        }
                        return list;
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "GetJobsYearwise List Failed");
                return new List<ShowJobData>();
            }
        }

        public static List<ShowJobData> GetJobsMonthwise(int SOID, string month, string plan)
        {
            DateTime fromDate = DateTime.Parse("01 " + month);
            var lastDay = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
           // var toDate = DateTime.Now;
            DateTime toDate = DateTime.Parse(lastDay + " " + month);

            return GetJobsMonthwise(SOID, month, plan, fromDate.ToString("dd-MMM-yyyy"), toDate.ToString("dd-MMM-yyyy"));
        }
        public static List<ShowJobData> GetJobsMonthwise(int SOID, string month, string plan, string startDate, string endDate)
        {
            try
            {
                DateTime fromDate = DateTime.Parse(startDate); //monthwise//DateTime.Parse("01 " + month);
                //monthwise//var lastDay = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
                DateTime toDate = DateTime.Parse(endDate); //monthwise//DateTime.Parse(lastDay + " " + month + " 23:59:59");

                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (plan.Equals("beat"))
                    {
                        return dbContext.JobsDetails.Where(jd => jd.SalesOficerID == (SOID > 0 ? SOID : jd.SalesOficerID)
                            && jd.Job.IsDeleted == false && jd.Job.IsActive == true
                            && jd.Job.VisitPlanType >= 4 && jd.Job.VisitPlanType <= 6
                            && jd.JobDate >= fromDate && jd.JobDate <= toDate).Select(jd => new ShowJobData
                            {
                                SalesOfficerID = (int)jd.SalesOficerID,
                                SalesOfficerName = jd.Job.SaleOfficer.Name,
                                DealerID = (int)jd.DealerID,
                                DealerName = jd.Dealer.Name,
                                RetailerID = (int)jd.RetailerID,
                                RetailerName = jd.Retailer.Name,
                                ShopName = !jd.RetailerID.HasValue || jd.RetailerID == 0 ? "" : jd.Retailer.ShopName,
                                PainterID = !jd.PainterID.HasValue || jd.PainterID == 0 ? 0 : (int)jd.PainterID,
                                PainterName = !jd.PainterID.HasValue || jd.PainterID == 0 ? "" : "",//dbContext.vPainters.Where(p => p.ID == jd.PainterID).Select(p => p.Name).FirstOrDefault(),
                                Status = jd.Status ?? false,
                                VisitPlanType = (int)jd.Job.VisitPlanType,
                                JobDate = (DateTime)jd.JobDate,
                                DateComplete = jd.DateComplete,
                                JobDetailID = jd.ID,
                                JobID = jd.Job.ID
                            }).ToList();
                    }
                    else
                    {
                        return dbContext.JobsDetails.Where(jd => jd.SalesOficerID == (SOID > 0 ? SOID : jd.SalesOficerID)
                            && jd.Job.IsDeleted == false && jd.Job.IsActive == true
                            && jd.JobDate >= fromDate && jd.JobDate <= toDate).Select(jd => new ShowJobData
                            {
                                SalesOfficerID = (int)jd.SalesOficerID,
                                SalesOfficerName = jd.Job.SaleOfficer.Name,
                               // DealerID = (int)jd.DealerID,
                               // DealerName = jd.Dealer.Name,
                                RetailerID = (int)jd.RetailerID,
                                RetailerName = jd.Retailer.Name,
                                ShopName = !jd.RetailerID.HasValue || jd.RetailerID == 0 ? "" : jd.Retailer.ShopName,
                               // PainterID = !jd.PainterID.HasValue || jd.PainterID == 0 ? 0 : (int)jd.PainterID,
                               // PainterName = !jd.PainterID.HasValue || jd.PainterID == 0 ? "" : "",//dbContext.vPainters.Where(p => p.ID == jd.PainterID).Select(p => p.Name).FirstOrDefault(),
                                Status = jd.Status ?? false,
                               // VisitPlanType = (int)jd.Job.VisitPlanType,
                                JobDate = (DateTime)jd.JobDate,
                               // DateComplete = jd.DateComplete,
                                JobDetailID = jd.ID,
                                JobID = jd.Job.ID
                            }).ToList();
                    }

                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "GetJobsMonthwise List Failed");
                return new List<ShowJobData>();
            }
        }

        public static bool? ReplicateJobs(int soId, int monthCount, string beatPlanOrMonthly, DateTime startDate, DateTime endDate)
        {
            DateTime fromDate = startDate;//DateTime.Parse("01 " + DateTime.Today.ToString("MMM-yyyy"));
            //var lastDay = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
            DateTime toDate = endDate.AddHours(23).AddMinutes(59);// startDate.AddDays(27).AddHours(23).AddMinutes(59);//DateTime.Parse(lastDay + " " + DateTime.Today.ToString("MMM-yyyy") + " 23:59:59");
            FOSDataModel dbContext = new FOSDataModel();
            List<JobsDetail> jobList = null;

            if (beatPlanOrMonthly.Equals("beat"))
            {
                var tempQry = dbContext.JobsDetails.Where(jd => jd.SalesOficerID == soId
                        && jd.Job.IsDeleted == false && jd.Job.IsActive == true
                        //&& jd.Job.Status == false && jd.Status == false
                        && jd.Job.VisitPlanType >= 4 && jd.Job.VisitPlanType <= 6
                        && jd.JobDate >= fromDate && jd.JobDate <= toDate);
                jobList = tempQry.ToList();
            }
            else if (beatPlanOrMonthly.Equals("monthly"))
            {
                var tempQry = dbContext.JobsDetails.Where(jd => jd.SalesOficerID == soId
                        && jd.Job.IsDeleted == false && jd.Job.IsActive == true
                        //&& jd.Job.Status == false && jd.Status == false
                        && jd.Job.VisitPlanType == 1
                        && jd.JobDate >= fromDate && jd.JobDate <= toDate);
                jobList = tempQry.ToList();
            }
            else
            {
                var tempQry = dbContext.JobsDetails.Where(jd => jd.SalesOficerID == soId
                        //&& jd.Job.Status == false && jd.Status == false
                        && jd.Job.IsDeleted == false && jd.Job.IsActive == true
                        && jd.JobDate >= fromDate && jd.JobDate <= toDate);
                jobList = tempQry.ToList();
            }

            if (jobList != null && jobList.Count > 0)
            {
                string qry = "delete from jobsdetail where JobDate > '"
                    //+ startDate.AddDays(28).ToString("dd-MMM-yyyy")
                    + endDate.ToString("dd-MMM-yyyy")
                    + "' and SalesOficerID=" + soId + " and status=0";

                if (beatPlanOrMonthly.Equals("beat"))
                {
                    qry = "delete from jobsdetail where JobDate > '"
                    //+ startDate.AddDays(28).ToString("dd-MMM-yyyy")
                    + endDate.ToString("dd-MMM-yyyy")
                    + "' and SalesOficerID=" + soId + " and status=0 and VisitPlanType >= 4 and VisitPlanType <= 6";
                }
                else if (beatPlanOrMonthly.Equals("monthly"))
                {
                    qry = "delete from jobsdetail where JobDate > '"
                    + endDate.ToString("dd-MMM-yyyy")
                    + "' and SalesOficerID=" + soId + " and status=0 and VisitPlanType = 1";
                }

                dbContext.Database.ExecuteSqlCommand(qry);
                
                //following query will delete those parents that hv no child
                dbContext.Database.ExecuteSqlCommand("delete Jobs FROM Jobs WHERE " +
                    " NOT EXISTS(SELECT 1 FROM FOS_WallCoat..JobsDetail WHERE FOS_WallCoat..Jobs.Id = FOS_WallCoat..JobsDetail.JobID)");

                // or use following query in case of above query not working
                /*
                Delete from FOS_WallCoat..jobs where id in 
                (Select ID from FOS_WallCoat..Jobs
                EXCEPT
                Select Jobid from FOS_WallCoat..JobsDetail)

                */

                int days = 0;
                for (int i = 0; i < monthCount; i++)
                {
                    days = days + 28;
                    foreach (var jobdet in jobList)
                    {
                        Job JobObj = new Job();
                        JobObj.ID = dbContext.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        JobObj.JobTitle = "";
                        JobObj.RegionalHeadType = jobdet.Job.RegionalHeadType;
                        JobObj.SaleOfficerID = jobdet.Job.SaleOfficerID;
                        JobObj.RegionalHeadID = jobdet.Job.RegionalHeadID;
                        JobObj.VisitType = jobdet.Job.VisitType;
                        JobObj.VisitPlanType = jobdet.Job.VisitPlanType;
                        JobObj.JobType = jobdet.Job.JobType;
                        JobObj.Status = false;

                        JobObj.RetailerType = jobdet.Job.RetailerType;

                        JobObj.DateOfAssign = DateTime.Now;
                        JobObj.CreatedDate = DateTime.Now;
                        JobObj.LastUpdated = DateTime.Now;
                        JobObj.StartingDate = DateTime.Now;

                        JobObj.IsActive = true;
                        JobObj.IsDeleted = false;

                        JobsDetail jobDetail = new JobsDetail();
                        jobDetail.JobID = JobObj.ID;
                        jobDetail.DealerID = jobdet.DealerID;

                        jobDetail.RegionalHeadID = JobObj.RegionalHeadID;
                        jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                        jobDetail.RetailerID = jobdet.RetailerID;

                        jobDetail.JobDate = jobdet.JobDate.Value.AddDays(days);
                        jobDetail.Status = false;

                        jobDetail.VisitPlanType = jobdet.VisitPlanType;

                        dbContext.JobsDetails.Add(jobDetail);
                        dbContext.Jobs.Add(JobObj);
                        dbContext.SaveChanges();
                    }
                }

                return true;
            }
            else
            {
                return null;
            }
        }

        public static List<JobsDetailData> GetDSRDealerForGrid(string From, string To, int ZoneID, int SOID)
        {

            List<JobsDetailData> doneJobData = new List<JobsDetailData>();
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (From == null && To == null)
                    {

                        doneJobData = dbContext.JobsDetails.Where(x => x.SalesOficerID == SOID && x.JobDate >= dtFromToday && x.JobDate <= dtToToday && x.Status == true && x.VisitPurpose == "Ordering" && x.JobType == "Retailer Order").Select(x => new JobsDetailData
                        {
                            ID = x.ID,
                            JobID = x.JobID,
                            //First Column
                            SaleOfficerID = (int)x.Job.SaleOfficerID,
                            SaleOfficerName = x.Job.SaleOfficer.Name,
                           
                            DealerName = dbContext.Dealers.Where(u=>u.ID==ZoneID).Select(u=>u.ShopName).FirstOrDefault(),
                            RetailerName = (x.Retailer.Name == null) ? "-" : x.Retailer.Name,
                            ShopName = (x.Retailer.ShopName == null) ? "-" : x.Retailer.ShopName,
                            DealerPhone = (x.Retailer.Phone1 == null) ? "-" : x.Retailer.Phone1,
                            RegionID = x.Retailer.RegionID,
                            RegionName = dbContext.Regions.Where(p => p.ID == x.Retailer.RegionID).Select(p => p.Name).FirstOrDefault(),
                            RetailerAddress = x.Retailer.Address,
                            CityName= x.Retailer.City.Name,
                            VisitPlanName = x.VisitPurpose,
                            AssignDate = x.JobDate,
                            VisitedDate = x.DateComplete,

                            VisitDate = x.DateComplete == null ? null : x.DateComplete,
                            VisitType = x.Job.VisitType,
                            RetailerType = x.ActivityType,
                            StatusChecker = x.Dispatchstatus,

                        }).ToList();
                    }
                    else
                    {
                        DateTime FromDate = Convert.ToDateTime(From);
                        DateTime ToDate = Convert.ToDateTime(To).AddDays(1);
                      

                        doneJobData = dbContext.JobsDetails.Where(x => x.SalesOficerID == SOID && x.JobDate >= FromDate && x.JobDate <= ToDate && x.Status == true && x.VisitPurpose == "Ordering" && x.JobType == "Retailer Order").Select(x => new JobsDetailData
                        {
                            ID = x.ID,
                            JobID = x.JobID,
                            //First Column
                            SaleOfficerID = (int)x.Job.SaleOfficerID,
                            SaleOfficerName = x.Job.SaleOfficer.Name,

                            DealerName = dbContext.Dealers.Where(u => u.ID == ZoneID).Select(u => u.ShopName).FirstOrDefault(),
                            RetailerName = (x.Retailer.Name == null) ? "-" : x.Retailer.Name,
                            ShopName = (x.Retailer.ShopName == null) ? "-" : x.Retailer.ShopName,
                            DealerPhone = (x.Retailer.Phone1 == null) ? "-" : x.Retailer.Phone1,
                            RegionID = x.Retailer.RegionID,
                            RegionName = dbContext.Regions.Where(p => p.ID == x.Retailer.RegionID).Select(p => p.Name).FirstOrDefault(),
                            RetailerAddress = x.Retailer.Address,
                            CityName = x.Retailer.City.Name,
                            VisitPlanName = x.VisitPurpose,
                            AssignDate = x.JobDate,
                            VisitedDate = x.DateComplete,

                            VisitDate = x.DateComplete == null ? null : x.DateComplete,
                            VisitType = x.Job.VisitType,
                            RetailerType = x.ActivityType,


                            StatusChecker = x.Dispatchstatus,

                        }).ToList();

                    }

                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }


        public static List<JobsDetailData> GetDispatchForvanForGrid(string From, string To, int ZoneID)
        {
            DateTime FromDate=DateTime.Now;
            DateTime ToDate = DateTime.Now;
            List<JobsDetailData> doneJobData = new List<JobsDetailData>();
            if (From != null && To != null)
            {
                 FromDate = Convert.ToDateTime(From);
                 ToDate = Convert.ToDateTime(To).AddDays(1);
            }
            else
            {
                FromDate = DateTime.UtcNow.AddHours(5);

                FromDate = FromDate.Date;
                ToDate = FromDate.AddDays(1);
            }
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                   

                        doneJobData = dbContext.Sp_GetDispatchForvanGriddata(ZoneID,FromDate, ToDate).Select(x => new JobsDetailData
                        {
                            
                            JobID = x.jobid,
                            //First Column
                      
                            Dboyname = x.DBoyName,

                        SaleOfficerName=x.SOName,
                            ShopName = x.Shopname,
                          
                            Dispatchdate = x.dispatchdate,

                            VisitDate = x.invoiceddate,
                      
                            StatusChecker = x.Dispatchstatus,

                        }).ToList();
               
                

                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }


        public static List<JobsDetailData> GetRetailerJobsDetailForGrid(string From, string To, int ZoneID, int SOID)
        {

            List<JobsDetailData> doneJobData = new List<JobsDetailData>();
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    
                        DateTime FromDate = Convert.ToDateTime(From);
                        DateTime ToDate = Convert.ToDateTime(To).AddDays(1);
                        doneJobData = (from jd in dbContext.Tbl_SalesClaimMaster
                                       join
                                       r in dbContext.Retailers on jd.CustomerID equals r.ID
                                       where ( jd.IsActive == true && jd.DateSelected >= FromDate && jd.DateSelected <= ToDate  && jd.SOID == SOID)
                                       select new JobsDetailData
                                       //u => new JobsDetailData
                                       {
                                           ID = jd.ID,
                                          JobID=jd.ID,
                                           //First Column
                                           SaleOfficerID = (int)jd.SOID,
                                           SaleOfficerName = dbContext.SaleOfficers.Where(p => p.ID == jd.SOID).Select(p => p.Name).FirstOrDefault(),
                                           //SegmentName = dbContext.Tbl_Segmenttype.Where(p => p.ID == jd.SegmentID).Select(p => p.Name).FirstOrDefault(),
                                           TradePartyName = dbContext.Retailers.Where(p => p.ID == jd.TradePartyID).Select(p => p.ShopName).FirstOrDefault(),
                                           Picture=jd.Picture,
                                           RetailerName = r.ShopName,
                                           ShopName = r.ShopName,
                                       LatestStatus=jd.ClaimManagerLatestStatus,
                                           RegionID = r.RegionID,
                                           RegionName = dbContext.Regions.Where(p => p.ID == r.RegionID).Select(p => p.Name).FirstOrDefault(),
                                           RetailerAddress = r.Address,
                                           TotalSale=jd.SaleValue,
                                           ClaimDate=jd.DateSelected,
                                           AssignDate = jd.CreatedOn,
                                           TotalLiters=jd.TotalLiters

                                           
                                       }).ToList();


                }

               

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }


        public static List<JobsDetailData> GetKPISDetailForGrid(string From, string To, int ZoneID, int SOID)
        {

            List<JobsDetailData> doneJobData = new List<JobsDetailData>();
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    DateTime FromDate = Convert.ToDateTime(From);
                    DateTime ToDate = Convert.ToDateTime(To).AddDays(1);
                    doneJobData = (from jd in dbContext.Tbl_MasterKPIS
                                   join
                                   r in dbContext.SaleOfficers on jd.SOID equals r.ID
                                   join
                                   head in dbContext.RegionalHeads on jd.HeadID equals head.ID
                                   join
                                   seg in dbContext.Tbl_Segmenttype on jd.BusinessSegmentID equals seg.ID
                                   join
                                   role in dbContext.SOTypes on jd.RoleID equals role.ID
                                   where ( jd.DateFrom >= FromDate && jd.DateTo <= ToDate
                                   && (SOID == 0 || jd.SOID == SOID)
                                   && (ZoneID == 0 || jd.HeadID == ZoneID)
                                   && jd.IsActive==true)
                                   select new JobsDetailData
                                   //u => new JobsDetailData
                                   {
                                       ID = jd.ID,
                                       JobID = jd.ID,
                                       //First Column
                                       SaleOfficerID = (int)jd.SOID,
                                       SaleOfficerName = dbContext.SaleOfficers.Where(p => p.ID == jd.SOID).Select(p => p.Name).FirstOrDefault(),
                                       SegmentName = dbContext.Tbl_Segmenttype.Where(p => p.ID == jd.BusinessSegmentID).Select(p => p.Name).FirstOrDefault(),
                                       RegionalHeadName = dbContext.RegionalHeads.Where(p => p.ID == jd.HeadID).Select(p => p.Name).FirstOrDefault(),
                                      
                                       RetailerName = dbContext.SOTypes.Where(p => p.ID == jd.RoleID).Select(p => p.Name).FirstOrDefault(),
                                       
                                      


                                   }).ToList();


                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }


        public static List<JobsDetailData> GetAgentCallingDetailForGrid(string From, string To, int ZoneID, int SOID)
        {
            List<JobsDetailData> doneJobData = new List<JobsDetailData>();
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);
            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    DateTime FromDate = Convert.ToDateTime(From);
                    DateTime ToDate = Convert.ToDateTime(To).AddDays(1);

                    if (SOID == 1)

                    {

                        doneJobData = (from r in dbContext.Retailers
                                       join s in dbContext.SaleOfficers on r.SaleOfficerID equals s.ID
                                       let allVisitIds = dbContext.Tbl_HousingVisits
                                           .Where(p => p.CustomerID == r.ID)
                                           .Select(p => p.ID)
                                           .ToList()
                                       let latestCall = dbContext.Tbl_SaveCall
                                           .Where(sc => allVisitIds.Contains((int)sc.VisitID))
                                           .OrderByDescending(sc => sc.CreatedOn)
                                           .FirstOrDefault()
                                       let latestVisit = dbContext.Tbl_HousingVisits
                                           .Where(p => p.CustomerID == r.ID)
                                           .OrderByDescending(p => p.ID)
                                           .FirstOrDefault()
                                       where (r.IsActive == true &&
                                              r.LastUpdate >= FromDate &&
                                              r.LastUpdate <= ToDate &&
                                              s.RegionalHeadID == ZoneID &&
                                              r.BusinessStatusID != 2

                                              )
                                       select new JobsDetailData
                                       {
                                           ID = r.ID,
                                           JobID = latestVisit != null ? latestVisit.ID : 0,
                                           SaleOfficerID = (int)r.SaleOfficerID,
                                           SaleOfficerName = s.Name,
                                           OwnerName = r.Name,
                                           OwnerMob = r.Phone1,
                                           RetailerName = r.ShopName,
                                           ShopName = r.ShopName,
                                           CustomerStatus = r.Status == true ? "Active" : "InActive",
                                           RegionID = r.RegionID,
                                           RegionName = dbContext.Regions
                                                       .Where(p => p.ID == r.RegionID)
                                                       .Select(p => p.Name)
                                                       .FirstOrDefault(),
                                           RetailerAddress = r.Address,
                                           ClaimDate = r.LastUpdate,
                                           AssignDate = latestVisit != null ? latestVisit.NextVisitDate : null,
                                           CallDate = latestCall != null ? latestCall.CreatedOn : null,
                                           CallerName = latestCall != null ? latestCall.CallerName : null,
                                           CallStatus = latestCall != null ? latestCall.NatureOfCall : null
                                       })
                                       .Where(x => x.JobID != 0)
                                       .ToList();

                    }
                    else if (SOID == 3)
                    {
                        doneJobData = (from r in dbContext.Retailers
                                       join s in dbContext.SaleOfficers on r.SaleOfficerID equals s.ID
                                       let allVisitIds = dbContext.Tbl_HousingVisits
                                           .Where(p => p.CustomerID == r.ID)
                                           .Select(p => p.ID)
                                           .ToList()
                                       let latestCall = dbContext.Tbl_SaveCall
                                           .Where(sc => allVisitIds.Contains((int)sc.VisitID))
                                           .OrderByDescending(sc => sc.CreatedOn)
                                           .FirstOrDefault()
                                       let latestVisit = dbContext.Tbl_HousingVisits
                                           .Where(p => p.CustomerID == r.ID)
                                           .OrderByDescending(p => p.ID)
                                           .FirstOrDefault()
                                       where (r.IsActive == true &&
                                              r.LastUpdate >= FromDate &&
                                              r.LastUpdate <= ToDate &&
                                              s.RegionalHeadID == ZoneID &&
                                              r.BusinessStatusID == 2

                                              )
                                       select new JobsDetailData
                                       {
                                           ID = r.ID,
                                           JobID = latestVisit != null ? latestVisit.ID : 0,
                                           SaleOfficerID = (int)r.SaleOfficerID,
                                           SaleOfficerName = s.Name,
                                           OwnerName = r.Name,
                                           OwnerMob = r.Phone1,
                                           RetailerName = r.ShopName,
                                           ShopName = r.ShopName,
                                           CustomerStatus = r.Status == true ? "Active" : "InActive",
                                           RegionID = r.RegionID,
                                           RegionName = dbContext.Regions
                                                       .Where(p => p.ID == r.RegionID)
                                                       .Select(p => p.Name)
                                                       .FirstOrDefault(),
                                           RetailerAddress = r.Address,
                                           ClaimDate = r.LastUpdate,
                                           AssignDate = latestVisit != null ? latestVisit.NextVisitDate : null,
                                           CallDate = latestCall != null ? latestCall.CreatedOn : null,
                                           CallerName = latestCall != null ? latestCall.CallerName : null,
                                           CallStatus = latestCall != null ? latestCall.NatureOfCall : null
                                       })
                                       .Where(x => x.JobID != 0)
                                       .ToList();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }



        public static List<JobsDetailData> GetResult12(string search, string sortOrder, int start, int length,
        List<JobsDetailData> dtResult, List<string> columnFilters)
        {
            IQueryable<JobsDetailData> query = FilterResult12(search, dtResult, columnFilters);

            // Apply sorting
            if (!string.IsNullOrEmpty(sortOrder))
            {
                switch (sortOrder)
                {
                    case "SaleOfficerName":
                        query = query.OrderBy(p => p.SaleOfficerName);
                        break;
                    case "SaleOfficerName_desc":
                        query = query.OrderByDescending(p => p.SaleOfficerName);
                        break;
                    case "OwnerName":
                        query = query.OrderBy(p => p.OwnerName);
                        break;
                    case "OwnerName_desc":
                        query = query.OrderByDescending(p => p.OwnerName);
                        break;
                    case "RetailerName":
                        query = query.OrderBy(p => p.RetailerName);
                        break;
                    case "RetailerName_desc":
                        query = query.OrderByDescending(p => p.RetailerName);
                        break;
                    case "CallStatus":
                        query = query.OrderBy(p => p.CallStatus);
                        break;
                    case "CallStatus_desc":
                        query = query.OrderByDescending(p => p.CallStatus);
                        break;
                    default:
                        query = query.OrderBy(p => p.ID); // Default sort
                        break;
                }
            }

            // Apply pagination
            return query.Skip(start).Take(length).ToList();
        }


        public static int Count12(string search, List<JobsDetailData> dtResult, List<string> columnFilters /*string saleofficer, string StartingDate1, string StartingDate2*/)
        {
            return FilterResult12(search, dtResult, columnFilters/*saleofficer, StartingDate1, StartingDate2*/).Count();
        }

        private static IQueryable<JobsDetailData> FilterResult12(string search, List<JobsDetailData> dtResult, List<string> columnFilters)
        {
            IQueryable<JobsDetailData> results = dtResult.AsQueryable();

            // Apply global search if provided
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                results = results.Where(p =>
                    (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(searchLower)) ||
                    (p.OwnerName != null && p.OwnerName.ToLower().Contains(searchLower)) ||
                    (p.OwnerMob != null && p.OwnerMob.ToLower().Contains(searchLower)) || // Added OwnerMob search
                    (p.RetailerName != null && p.RetailerName.ToLower().Contains(searchLower)) ||
                    (p.ShopName != null && p.ShopName.ToLower().Contains(searchLower)) ||
                    (p.CustomerStatus != null && p.CustomerStatus.ToLower().Contains(searchLower)) ||
                    (p.RegionName != null && p.RegionName.ToLower().Contains(searchLower)) ||
                    (p.CallerName != null && p.CallerName.ToLower().Contains(searchLower)) ||
                    (p.CallStatus != null && p.CallStatus.ToLower().Contains(searchLower)) ||
                    (p.VisitDateFormatted != null && p.VisitDateFormatted.ToLower().Contains(searchLower)) ||
                    (p.ClaimDateFormatted != null && p.ClaimDateFormatted.ToLower().Contains(searchLower)) ||
                    (p.CallDateFormatted != null && p.CallDateFormatted.ToLower().Contains(searchLower))
                );
            }

            // Apply column-specific filters
            // Assuming columnFilters index mapping:
            // Index 0: ID (if any)
            // Index 1: JobID (if any)  
            // Index 2: SaleOfficerName
            // Index 3: OwnerName/CustomerName
            // Index 4: RetailerName/ShopName
            // Index 5: ClaimDate/VisitDate
            // Index 6: CallStatus

            if (columnFilters.Count > 2 && !string.IsNullOrEmpty(columnFilters[2]))
            {
                string filterValue = columnFilters[2].ToLower();
                results = results.Where(p => p.SaleOfficerName != null &&
                                             p.SaleOfficerName.ToLower().Contains(filterValue));
            }

            if (columnFilters.Count > 3 && !string.IsNullOrEmpty(columnFilters[3]))
            {
                string filterValue = columnFilters[3].ToLower();
                results = results.Where(p => (p.OwnerName != null && p.OwnerName.ToLower().Contains(filterValue)) ||
                                             (p.RetailerName != null && p.RetailerName.ToLower().Contains(filterValue)));
            }

            if (columnFilters.Count > 4 && !string.IsNullOrEmpty(columnFilters[4]))
            {
                string filterValue = columnFilters[4].ToLower();
                results = results.Where(p => p.RetailerName != null && p.RetailerName.ToLower().Contains(filterValue));
            }

            if (columnFilters.Count > 5 && !string.IsNullOrEmpty(columnFilters[5]))
            {
                string filterValue = columnFilters[5].ToLower();
                results = results.Where(p => (p.ClaimDateFormatted != null && p.ClaimDateFormatted.ToLower().Contains(filterValue)) ||
                                             (p.VisitDateFormatted != null && p.VisitDateFormatted.ToLower().Contains(filterValue)) ||
                                             (p.CallDateFormatted != null && p.CallDateFormatted.ToLower().Contains(filterValue)));
            }

            if (columnFilters.Count > 6 && !string.IsNullOrEmpty(columnFilters[6]))
            {
                string filterValue = columnFilters[6].ToLower();
                results = results.Where(p => p.CallStatus != null && p.CallStatus.ToLower().Contains(filterValue));
            }
            // Add column-specific filter for OwnerMob if you have a column for it
            if (columnFilters.Count > 7 && !string.IsNullOrEmpty(columnFilters[7]))
            {
                string filterValue = columnFilters[7].ToLower();
                results = results.Where(p => p.OwnerMob != null && p.OwnerMob.ToLower().Contains(filterValue));
            }
                return results;
        }








        #region jobs resports

        // Get Retailer Related To Job In Edit Mode ...
        public static List<JobsDetailData> GetJobsToExportInExcel()
        {
             List<JobsDetailData> doneJobData = new List<JobsDetailData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    doneJobData = dbContext.JobsDetails.Where(j => j.Job.IsDeleted == false)
                                .Select(
                                    u => new JobsDetailData
                                    {
                                        ID = u.ID,

                                        //First Column
                                        SaleOfficerID = (int)u.Job.SaleOfficerID,
                                        SaleOfficerName = u.Job.SaleOfficer.Name,
                                        DealerID = (u.Job.JobsDetails.Select(j => j.DealerID).FirstOrDefault() == null) ? 0 : u.Job.JobsDetails.Select(j => j.DealerID).FirstOrDefault(),
                                        DealerName = (u.Job.JobsDetails.Select(j => j.Dealer.Name).FirstOrDefault() == null) ? "" : u.Job.JobsDetails.Select(j => j.Dealer.Name).FirstOrDefault(),
                                        RetailerName = (u.Retailer.Name == null) ? "" : u.Retailer.Name,
                                        ShopName = u.Retailer.ShopName,
                                        RetailerAddress = u.Retailer.Address,
                                        PainterName = "",//dbContext.vPainters.Where(p => p.ID == u.PainterID).Select(p => p.Name).FirstOrDefault(),
                                        PainterAddress = "",//dbContext.vPainters.Where(p => p.ID == u.PainterID).Select(p => p.Address).FirstOrDefault(),
                                        VisitPlanName = u.Job.VisitPlan.Type,
                                        AssignDate = u.Job.DateOfAssign,
                                        CompletedDate = u.DateComplete,
                                        //CompletedDateFormated = u.DateComplete != null ? u.DateComplete.Value.ToString("dd-MMM-yyyy") : "",
                                        VisitDate = u.JobDate,
                                        //VisitDateFormated = u.JobDate.Value.ToString("dd-MMM-yyyy"),
                                        RegionalHeadID = u.RegionalHead != null ? u.RegionalHeadID.Value : 0,
                                        RegionalHeadName = u.RegionalHead != null ? u.RegionalHead.Name : "",
                                        CityName = u.Retailer.City.Name,
                                        AreaName = u.Retailer.Area.Name,
                                        ShopAddress = u.Retailer.Address,
                                        LatLong = u.Retailer.Location,
                                        //Second Column


                                        VisitType = u.Job.VisitType,

                                        //Retailer
                                        RetailerType = u.Retailer.Type,
                                        SAvailable = u.SAvailable,
                                        SQuantity1KG = u.SQuantity1KG,
                                        SQuantity5KG = u.SQuantity5KG,
                                        SNewOrder = u.SNewOrder,
                                        SNewQuantity1KG = u.SNewQuantity1KG,
                                        SNewQuantity5KG = u.SNewQuantity5KG,
                                        SPreviousOrder1KG = u.SPreviousOrder1KG,
                                        SPreviousOrder5KG = u.SPreviousOrder5KG,
                                        SPOSMaterialAvailable = u.SPOSMaterialAvailable,
                                        SImage = u.SImage,
                                        SNote = u.SNote,
                                        ///END

                                        // Painter
                                        PUseWC = u.PUseWC,
                                        PUseWC1KG = u.PUseWC1KG,
                                        PUseWC5KG = u.PUseWC5KG,
                                        PNewOrder = u.PNewOrder,
                                        PNewOrder1KG = u.PNewOrder1KG,
                                        PNewOrder5KG = u.PNewOrder5KG,
                                        PNewLead = u.PNewLead,
                                        PNewLeadMobNo = u.PNewLeadMobNo,
                                        PRemarks = u.PRemarks,
                                        ///END

                                        // B2B
                                        AreaID = u.BAreaID,
                                        BAreaName = u.Area.Name,
                                        BShop = (u.BShop == null) ? "-" : u.BShop,
                                        BOldHouse = (u.BOldHouse == null) ? "-" : u.BOldHouse,
                                        BNewHouse = (u.BNewHouse == null) ? "-" : u.BNewHouse,
                                        BParking = (u.BParking == null) ? "-" : u.BParking,
                                        BPlazaBasement = (u.BPlazaBasement == null) ? "-" : u.BPlazaBasement,
                                        BFactoryArea = (u.BFactoryArea == null) ? "-" : u.BFactoryArea,
                                        BMosque = (u.BMosque == null) ? "-" : u.BMosque,
                                        BOthers = (u.BOthers == null) ? "-" : u.BOthers,
                                        BLead = (u.BLead == true) ? true : false,
                                        BSampleApplied = (u.BSampleApplied == true) ? true : false,
                                        BRemarks = u.BRemarks,
                                        ///END

                                        StatusChecker = (u.Status == true) ? "DONE" : "PENDING",
                                    }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }



        public static List<CityWisePainters> GetPaintersToExportInExcel()
        {
            List<CityWisePainters> doneJobData = new List<CityWisePainters>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {


                    //doneJobData = dbContext.vPainters.Where(x=>x.Registered==true)
                    //            .Select(
                    //                u => new CityWisePainters
                    //                {
                    //                    painterid = u.ID,
                    //                    pname=u.Name,
                    //                    Registered=u.Registered,
                    //                    cnic=u.CNIC,
                    //                    city=u.City,
                    //                    Market=u.Market,
                    //                    POS=u.POS,
                    //                    PhoneNumber=u.PhoneNumber
                                        
                                       
                                      
                    //                }).ToList();
                }



            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Jobs List Failed");
                throw;
            }

            return doneJobData;
        }



        #endregion
    }
}
