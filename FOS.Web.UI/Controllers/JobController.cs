using FOS.Setup;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FOS.DataLayer;
using FluentValidation.Results;
using FOS.Web.UI.Models;
using FOS.Web.UI.Common.CustomAttributes;
using Excel;
using System.IO;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace FOS.Web.UI.Controllers
{
    public class JobController : Controller
    {

        FOSDataModel db = new FOSDataModel();

        #region Monthly

        [CustomAuthorize]
        // View ...
        public ActionResult MonthlyJobWork()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);

            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(RHID);
            var objSalesOfficer = SaleOfficerObj.FirstOrDefault();

            List<RetailerData> RetailerObj = new List<RetailerData>();


            if (objSalesOfficer == null)
            {
                return View();
            }

            else
            {
                RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(objSalesOfficer.ID);
                ViewData["RetailerObj"] = RetailerObj;
            }


            List<VisitPlanData> visitData = new List<VisitPlanData>();
            visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();
            
            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.VisitPlan = visitData;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");

            return View(objJob);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateJob([Bind(Exclude = "TID")] JobsData newData)
        {
            Boolean boolFlag = true;
            int Res = 1;
            ValidationResult results = new ValidationResult();

           
            try
            {
                if (newData != null)
                {
                    
                    if (boolFlag)
                    {

                        Res = ManageJobs.AddUpdateJob(newData);
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 2)
                        {
                            return Content("2");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        return Content("3");
                    }

                }
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
            return Content("0");
        }


        //Get All Region Method...
        public JsonResult JobDataHandler(DTParameters param, int RegionalHeadID, int SaleOfficerID)
        {
            try
            {
                
                var dtsource = new List<JobsData>();

                int regionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                //if (regionalheadID == 0)
                //{
                    dtsource = ManageJobs.GetJobsForGrid(RegionalHeadID, SaleOfficerID);
                //}
                //else
                //{
                //    dtsource = ManageJobs.GetJobsForGrid(regionalheadID, SaleOfficerID);
                //}
                List<String> columnSearch = new List<String>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<JobsData> data = ManageJobs.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageJobs.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<JobsData> result = new DTResult<JobsData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        //Delete Job Method...
        public int DeleteJob(int JobID)
        {
            return ManageJobs.DeleteJob(JobID);
        }

        
        #endregion

        #region WEEKLY


        [CustomAuthorize]
        public ActionResult WeeklyJobWork()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            List<SaleOfficer> SaleOfficerObj;

            if (RHID == 0 || RHID == null)
            {
                SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regionalHeadData.Select(rh => rh.ID).FirstOrDefault());
            }
            else
            {
                SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(RHID);
            }

            List<RetailerData> RetailerObj = new List<RetailerData>();

            if (SaleOfficerObj == null)
            {
                return View();
            }

            else
            {
                RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(SaleOfficerObj.Select(s => s.ID).FirstOrDefault());
                ViewData["RetailerObj"] = RetailerObj;
            }

            List<VisitPlanData> visitData = new List<VisitPlanData>();
            visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.VisitPlan = visitData;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");

            return View(objJob);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateWeeklyJob([Bind(Exclude = "TID")] JobsData newData)
        {
            Boolean boolFlag = true;
            int Res = 1;
            ValidationResult results = new ValidationResult();


            try
            {
                if (newData != null)
                {

                    if (boolFlag)
                    {

                        Res = ManageWeeklyJob.AddUpdateWeekJob(newData);
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 2)
                        {
                            return Content("2");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        return Content("3");
                    }

                }
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
            return Content("0");
        }


        //Load Retailers...
        public ActionResult LoadWeeklyRetailers(int RegionalHeadID, int SaleOfficerID, string RetailerType, int CityID)
        {
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                List<RetailerData> DealerObject = new List<RetailerData>();
                List<RetailerData> RetailerObject = new List<RetailerData>();

                int RID = (int)dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).Select(s => s.RegionalHeadID).FirstOrDefault();

                DealerObject = dbContext.Dealers.Where(r => r.RegionalHeadID == RID).Select(r =>
                         new RetailerData
                         {
                             DealerID = r.ID,
                             DealerName = r.Name,
                         }).ToList();


                RetailerObject = dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID
                    && r.RetailerType == (RetailerType.Equals("Both") ? r.RetailerType : RetailerType)
                    && r.Status == true && !r.IsDeleted && r.IsActive
                    && r.CityID == (CityID == -1 ? r.CityID : CityID))
                    .Select(r =>
                        new RetailerData
                        {
                            ID = r.ID,
                            Name = r.Name,
                            SaleOfficerID = r.SaleOfficerID,
                            SaleOfficerName = r.SaleOfficer.Name,
                            //DealerID = r.DealerID,
                            DealerName = r.Name,
                            CItyName = r.City.Name,
                            AreaName = r.Area.Name,
                            RetailerType = r.RetailerType,
                            ShopName = r.ShopName,
                            Address = r.Address
                        }).OrderBy(r => r.Name).ToList();

                Reponse += "<div class=''>";
                foreach (var d in DealerObject)
                {
                    if (RetailerObject.Where(r => r.DealerID == d.DealerID).Count() > 0)
                    {

                        Reponse += "<div class='span12' style='margin-left:8px'><h4>" + d.DealerName + " <span style=''>(" + RetailerObject.Where(r => r.DealerID == d.DealerID).Count() + ")</span></h4></div>";
                        Reponse += "<div class='span12' style='margin-left:6px'>";
                        foreach (var r in RetailerObject)
                        {
                            if (r.DealerID == d.DealerID)
                            {
                                Reponse += "<a data-toggle='modal' data-target='#myModal' class='RetailerInfo fc-event ui-draggable ui-draggable-handle' style='z-index: auto;left: 0px;top: 0px;' data-id='" + r.ID + "' data-dealerid='" + r.DealerID + "' data-name='" + r.Name + "' data-salesofficerid='" + r.SaleOfficerID + "' data-cityname='" + r.CItyName + "' data-areaname='" + r.AreaName + "' data-retailertype='" + r.RetailerType + "' data-shopname='" + r.ShopName + "' data-address='" + r.Address + "'>" + r.ShopName + "</a>";
                            }
                        }
                        Reponse += "</div>";
                    }
                }
                Reponse += "<p style='display:none'><input type='checkbox' id='drop-remove' checked><label for='drop-remove'>Remove After Drop</label></p>";
                Reponse += "</div>";
            }

            return Json(new { Response = Reponse });
        }


        public ActionResult LoadWeeklyPainters(int SOID)
        {
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                List<PainterAssociationData> PainterObject = new List<PainterAssociationData>();

                var painters = dbContext.PaintersBySalesOfficer(SOID).Where(p => p.Selected != true);
                foreach (var p in painters)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input id='retailer" + p.PainterID + "' data-id='" + p.PainterID + "' class='' name='retailer[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.PainterName + "</span><p style='font-size:9px;'> " + p.City + "</p></div>";
                }

            }

            return Json(new { Response = Reponse });
        }
        public ActionResult LoadDealer(int RegionalHeadID, int SaleOfficerID)
        {
            //string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                List<RetailerData> DealerObject = new List<RetailerData>();
                List<RetailerData> RetailerObject = new List<RetailerData>();

                int RID = (int)dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).Select(s => s.RegionalHeadID).FirstOrDefault();

                DealerObject = dbContext.Dealers.Where(r => r.RegionalHeadID == RID).Select(r =>
                         new RetailerData
                         {
                             DealerID = r.ID,
                             DealerName = r.Name,
                         }).ToList();

                DealerObject.Insert(0, new RetailerData
                {
                    DealerID = 0,
                    DealerName = "All"
                });


                return Json(DealerObject);
            }


        }
        #endregion





        public ActionResult LoadRetailerRelatedToCities(int cityid)
        {
            //string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                
                List<RetailerData> RetailerObject = new List<RetailerData>();

                //int RID = (int)dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).Select(s => s.RegionalHeadID).FirstOrDefault();

                RetailerObject = dbContext.Retailers.Where(r => r.CityID == cityid).Select(r =>
                         new RetailerData
                         {
                             ID = r.ID,
                             ShopName = r.ShopName,
                         }).ToList();

                RetailerObject.Insert(0, new RetailerData
                {
                    ID = 0,
                    ShopName = "All"
                });


                return Json(RetailerObject);
            }


        }









        #region DAILY


        [CustomAuthorize]
        public ActionResult DailyJobWork()
        { var userID = Convert.ToInt32(Session["UserID"]);
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            regionalHeadData.Insert(0, new RegionalHeadData
            {
                ID = 0,
                Name = "Select"
            });

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regionalHeadData.FirstOrDefault().ID, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "Select"
            });

            List<RetailerData> RetailerObj = new List<RetailerData>();

            if (SaleOfficerObj == null)
            {
                return View();
            }

            else
            {
                RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(SaleOfficerObj.Select(s => s.ID).FirstOrDefault());
                ViewData["RetailerObj"] = RetailerObj;
            }

            

            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            //objJob.VisitPlan = visitData;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListBySOID(SaleOfficerObj.FirstOrDefault().ID);

            return View(objJob);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateDailyJob([Bind(Exclude = "TID")] JobsData newData)
        {
            Boolean boolFlag = true;
            int Res = 1;
            ValidationResult results = new ValidationResult();


            try
            {
                if (newData != null)
                {

                    if (boolFlag)
                    {

                        Res = ManageDailyJobs.AddUpdateDailyJob(newData);
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 2)
                        {
                            return Content("2");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        return Content("3");
                    }

                }
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
            return Content("0");
        }

        public ActionResult ReplicateJobs(int SOID, int monthCount, string beatPlanOrMonthly, DateTime startDate, DateTime endDate)
        {
            if (SOID > 0)
            {
                try
                {
                    //object[] param = { SOID };
                    //var result = db.Database.SqlQuery<SpReturnTest>("exec spReplicateJobsOfSO @soId",
                    //                        new SqlParameter("soId", param[0])).ToList();
                    bool? flag = ManageJobs.ReplicateJobs(SOID, monthCount, beatPlanOrMonthly, startDate, endDate);
                    if (flag.HasValue)
                    {
                        if (flag.Value)
                        {
                            return Json(new { Response = "Done" });
                        }
                        else
                        {
                            return Json(new { Response = "Failed" });
                        }
                    }
                    else
                    {
                        return Json(new { Response = "Nothing found" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Response = "Exception" + ex.Message });
                }
            }
            return Json(new { Response = "Failed" });

        }
        //Load Retailers...
        public ActionResult LoadDailyRetailers(int SaleOfficerID, string RetailerType)
        {
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                List<RetailerData> DealerObject = new List<RetailerData>();
                List<RetailerData> RetailerObject = new List<RetailerData>();

                int RID = (int)dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).Select(s => s.RegionalHeadID).FirstOrDefault();

                DealerObject = dbContext.Dealers.Where(r => r.RegionalHeadID == RID).Select(r =>
                         new RetailerData
                         {
                             DealerID = r.ID,
                             DealerName = r.Name,
                         }).ToList();


                RetailerObject = dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID && r.RetailerType == RetailerType && r.Status == true && r.JobsDetails.Where(s => s.Retailer.ID == s.RetailerID && s.Job.IsDeleted == false).Select(s => s.RetailerID).Count() == 0).Select(r =>
                        new RetailerData
                        {
                            ID = r.ID,
                            Name = r.Name,
                            SaleOfficerID = r.SaleOfficerID,
                            SaleOfficerName = r.SaleOfficer.Name,
                            //DealerID = r.DealerID,
                            DealerName = r.Name,
                            CItyName = r.City.Name,
                            AreaName = r.Area.Name,
                            RetailerType = r.RetailerType,
                            ShopName = r.ShopName,
                            Address = r.Address
                        }).OrderBy(r => r.Name).ToList();

                Reponse += "<div class=''>";
                foreach (var d in DealerObject)
                {
                    if (RetailerObject.Where(r => r.DealerID == d.DealerID).Count() > 0)
                    {

                        Reponse += "<div class='span12' style='margin-left:8px'><h4>" + d.DealerName + " <span style=''>(" + RetailerObject.Where(r => r.DealerID == d.DealerID).Count() + ")</span></h4></div>";
                        Reponse += "<div class='span12' style='margin-left:6px'>";
                        foreach (var r in RetailerObject)
                        {
                            if (r.DealerID == d.DealerID)
                            {
                                Reponse += "<a data-toggle='modal' data-target='#myModal' class='RetailerInfo fc-event ui-draggable ui-draggable-handle' style='z-index: auto;left: 0px;top: 0px;' data-id='" + r.ID + "' data-dealerid='" + r.DealerID + "' data-name='" + r.Name + "' data-salesofficerid='" + r.SaleOfficerID + "' data-cityname='" + r.CItyName + "' data-areaname='" + r.AreaName + "' data-retailertype='" + r.RetailerType + "' data-shopname='" + r.ShopName + "' data-address='" + r.Address + "'>" + r.ShopName + "</a>";
                            }
                        }
                        Reponse += "</div>";
                    }
                }
                Reponse += "<p style='display:none'><input type='checkbox' id='drop-remove' checked><label for='drop-remove'>Remove After Drop</label></p>";
                Reponse += "</div>";
            }

            return Json(new { Response = Reponse });
        }


        public ActionResult LoadDailyPainters(int SOID)
        {
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                List<PainterAssociationData> PainterObject = new List<PainterAssociationData>();

                var painters = dbContext.PaintersBySalesOfficer(SOID).Where(p => p.Selected != true);
                foreach (var p in painters)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input id='retailer" + p.PainterID + "' data-id='" + p.PainterID + "' class='' name='retailer[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.PainterName + "</span><p style='font-size:9px;'> " + p.City + "</p></div>";
                }

            }

            return Json(new { Response = Reponse });
        }

        #endregion

        #region COMMON

        [HttpPost]
        public JsonResult ResetLatLong(int retailerId)
        {
            var result = FOS.Setup.ManageRetailer.ResetRetailerLatLong(retailerId);
            return Json(new { message = result+"" });
        }

        public JsonResult GetRetailerBySalesOfficer(int SaleOfficerID)
        {
            var result = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(SaleOfficerID);
            return Json(result);
        }

        public JsonResult GetAllSaleOfficerListRelatedToDealer(int RegionalHeadID)
        {
            var result = FOS.Setup.ManageJobs.GetAllSaleOfficerListRelatedToDealer(RegionalHeadID);
            return Json(result);
        }
        public JsonResult GetAllSaleOfficerListRelatedToDealerSelectOption(int RegionalHeadID, bool selectOption)
        {
            var result = FOS.Setup.ManageJobs.GetAllSaleOfficerListRelatedToDealer(RegionalHeadID, selectOption);
            return Json(result);
        }
        public JsonResult GetAllSaleOfficerListRelatedToHeadrSelectOption(int RegionalHeadID, bool selectOption)
        {
            var result = FOS.Setup.ManageJobs.GetAllSaleOfficerListRelatedToHead(RegionalHeadID, selectOption);
            return Json(result);
        }
        public JsonResult GetHeadsRelatedtorange(int RegionalHeadID, bool selectOption)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var result = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            return Json(result);
        }

        public JsonResult GetDealersRelatedtorange(int RangeID, int RegionID)
        {
        
            var result = FOS.Setup.ManageRegionalHead.GetDealersListForUsers(RangeID, RegionID);
            return Json(result);
        }
        public JsonResult GetAllSaleOfficerListRelatedToSelectOption(int RegionalHeadID, bool selectOption)
        {
            var result = FOS.Setup.ManageJobs.GetAllSaleOfficerListRelated(RegionalHeadID, selectOption);
            return Json(result);
        }
        public JsonResult GetAllSaleOfficerSelectOption(int regionalHeadId)
        {
            var result = FOS.Setup.ManageJobs.GetAllSaleOfficerSelectOption(regionalHeadId);
            return Json(result);
        }

        public JsonResult GetZoneData(int HeadID)
        {
            var result = FOS.Setup.ManageRegionalHead.GetZoneList(HeadID);
            return Json(result);
        }
        public JsonResult GetAllSaleOfficerListByRegHeadId(int RegionalHeadID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetSaleOfficerByRegionalHeadID(RegionalHeadID, true, true);
            return Json(result);
        }
        public JsonResult GetSOCities(int SOID)
        {
            var result = FOS.Setup.ManageCity.GetCityListBySOID(SOID);
            return Json(result);
        }

        public JsonResult GetCities(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetCitiesByRegionID(RegionID);
            return Json(result);
        }
        public JsonResult GetAllRetailerBySalesOfficer(int RegionalHeadID, int SaleOfficerID)
        {

            List<RetailerData> result = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(RegionalHeadID, SaleOfficerID);
            return Json(result);
        }

        public JsonResult GetAllDealersrelatedtorange(int RegionalHeadID, int regionID)
        {
            var result = FOS.Setup.ManageJobs.GetAllDealersRelatedtorange(RegionalHeadID, regionID);
            return Json(result);
        }
        public ActionResult LoadRetailers(int RegionalHeadID, int SaleOfficerID, string RetailerType)
        {
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                List<RetailerData> DealerObject = new List<RetailerData>();
                List<RetailerData> RetailerObject = new List<RetailerData>();

                DealerObject = dbContext.Dealers.Where(r => r.RegionalHeadID == RegionalHeadID).Select(r =>
                         new RetailerData
                         {
                             DealerID = r.ID,
                             DealerName = r.Name,
                         }).ToList();


                RetailerObject = dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID && r.RetailerType == RetailerType && r.Status == true && r.JobsDetails.Where(s => s.Retailer.ID == s.RetailerID && s.Job.IsDeleted == false).Select(s => s.RetailerID).Count() == 0).Select(r =>
                        new RetailerData
                        {
                            ID = r.ID,
                            Name = r.ShopName,
                           // DealerID = r.DealerID,
                            DealerName = r.Name,
                            Address = r.Address
                        }).OrderBy(r => r.Name).ToList();


                foreach (var d in DealerObject)
                {

                    if (RetailerObject.Where(r => r.DealerID == d.DealerID).Count() > 0)
                    {
                        Reponse += @"<div class='span12' style='margin-left:0px'><h4 style='color: #bf0000;font-weight: 600;'>" + d.DealerName + "</h4></div>";

                        foreach (var r in RetailerObject)
                        {
                            if (r.DealerID == d.DealerID)
                            {
                                Reponse += @"<div class='span4' style='margin:0px;margin-left: 5px;'><input id='retailer" + r.ID + "' data-id='" + r.ID + "' data-dealerid='" + r.DealerID + "' class='' name='retailer[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #4a8bc2;font-weight: 700;'>" + r.Name + "</span><p style='font-size:9px;'> " + r.Address + "</p></div>";
                            }
                        }
                        Reponse += "<div class='span12' style='margin-left:0px'><hr></div>";
                    }
                }
            }

            return Json(new { Response = Reponse });
        }

        public JsonResult LoadRetailersEdit(int JobID, int RegionalHeadID, int SaleOfficerID, string RetailerType)
        {
            string Reponse = "";

            using (FOSDataModel dbContext = new FOSDataModel())
            {

                var so = dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).FirstOrDefault();
                var rt = dbContext.Jobs.Where(j => j.ID != JobID).SelectMany(j => j.JobsDetails.Where(jd => jd.Job.IsDeleted == false).Select(jd => jd.RetailerID));


                List<RetailerData> DealerObject = new List<RetailerData>();
                List<RetailerData> RetailerObject = new List<RetailerData>();


                DealerObject = dbContext.Dealers.Where(r => r.RegionalHeadID == RegionalHeadID).Select(r =>
                       new RetailerData
                       {
                           DealerID = r.ID,
                           DealerName = r.Name,
                       }).ToList();

                RetailerObject = dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID
                    && r.RetailerType == RetailerType && r.SaleOfficer.RegionalHeadID == RegionalHeadID
                    && !rt.Contains(r.ID)
                    && r.Status == true)
                    .Select(
                         u => new RetailerData
                         {
                             ID = u.ID,
                             Name = u.ShopName,
                             //DealerID = u.DealerID,
                             DealerName = u.Dealer.Name,
                             LocationName = u.LocationName,
                             Address = u.Address,
                             RetailerType = u.RetailerType,
                             RetailerJobStatus = u.JobsDetails.Where(s => s.JobID == JobID && s.Job.IsDeleted == false && s.Retailer.SaleOfficerID == SaleOfficerID && s.Job.SaleOfficer.ID == SaleOfficerID && s.Retailer.Status == true).Count() == 0 ? false : true
                         }).OrderBy(r => r.Name).ToList();


                foreach (var d in DealerObject)
                {
                    if (RetailerObject.Where(r => r.DealerID == d.DealerID).Count() > 0)
                    {
                        Reponse += @"<div class='span12' style='margin-left:0px'><h4 style='color: #bf0000;font-weight: 600;'>" + d.DealerName + "</h4></div>";

                        foreach (var r in RetailerObject)
                        {
                            if (r.DealerID == d.DealerID)
                            {
                                Reponse += @"<div class='span4' style='margin:0px;margin-left: 5px;'><input id='retailer" + r.ID + "' data-id='" + r.ID + "' data-dealerid='" + r.DealerID + "' class='' name='retailer[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #4a8bc2;font-weight: 700;'> " + r.Name + "</span><p style='font-size:9px;'> " + r.Address + "</p></div>";
                            }
                        }

                        Reponse += "<div class='span12' style='margin-left:0px'><hr></div>";
                    }
                }

            }

            List<RetailerData> retailerData = new List<RetailerData>();
            retailerData = FOS.Setup.ManageJobs.GetEditRetailer(JobID, RegionalHeadID, SaleOfficerID);

            return Json(new { Response = Reponse, EditRetailer = retailerData });

        }

        public JsonResult GetEditRetailer(int JobID, int RegionalHeadID, int SoID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            retailerData = FOS.Setup.ManageJobs.GetEditRetailer(JobID, RegionalHeadID, SoID);

            return Json(retailerData);
        }

        public JsonResult GetRegionalHeadAccordingToType(int RegionalHeadType)
        {
            var result = FOS.Setup.ManageSaleOffice.GetRegionalHeadAccordingToType(RegionalHeadType);
            return Json(result);
        }

        public ActionResult LoadPainters(int SOID)
        {
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                List<PainterAssociationData> PainterObject = new List<PainterAssociationData>();

                var painters = dbContext.PaintersBySalesOfficer(SOID).Where(p => p.Selected != true);
                foreach (var p in painters)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input id='retailer" + p.PainterID + "' data-id='" + p.PainterID + "' class='' name='retailer[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.PainterName + "</span><p style='font-size:9px;'> " + p.City + "</p></div>";
                }

            }

            return Json(new { Response = Reponse });
        }

        public JsonResult LoadPaintersEdit(int JobID, int SOID)
        {

            string Reponse = "";
            List<int> painterIds;
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                painterIds = dbContext.PaintersBySalesOfficerEDIT(SOID, JobID).Where(p => p.Selected == true).Select(p => p.PainterID).ToList();

                var painters = dbContext.PaintersBySalesOfficerEDIT(SOID, JobID);

                foreach (var p in painters)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input " + ((p.Selected ?? false) ? "checked" : "") + " id='retailer" + p.PainterID + "' data-id='" + p.PainterID + "' class='pCheckBox' name='retailer[]' onchange='painterSelected(this)' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.PainterName + "</span><p style='font-size:9px;' id='cityName'>" + p.City + "</p></div>";
                }

            }

            return Json(new { Response = Reponse, PainterIDs = painterIds });

        }




        #endregion

        #region B2B Job

        [CustomAuthorize]
        // View ...
        public ActionResult B2BJob()
        {
           
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetB2BRegionalHeadList();

            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(RHID);
            
            List<VisitPlanData> visitData = new List<VisitPlanData>();
            visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType(2);
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Cities = FOS.Setup.ManageCity.GetCityListByRegionHeadID(RHID);
            objJob.RegionalHead = regionalHeadData;
            objJob.VisitPlan = visitData;
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");

            return View(objJob);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateB2BJob([Bind(Exclude = "TID")] JobsData newData)
        {
            Boolean boolFlag = true;
            int Res = 1;
            ValidationResult results = new ValidationResult();

            try
            {
                if (newData != null)
                {

                    if (boolFlag)
                    {

                        Res = ManageJobs.AddUpdateB2BJob(newData);
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 2)
                        {
                            return Content("2");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        return Content("3");
                    }

                }
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
            return Content("0");
        }

        public JsonResult GetCitiesRelatedToRegionalHead(int RegionalHeadID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionHeadID(RegionalHeadID);
            return Json(result);
        }

        public JsonResult GetAreasRelatedToCityID(int SOID, int CityID)
        {
            string Reponse = "";
            
            if (CityID == 0 || SOID == 0)
            {
                return Json(new { Response = Reponse });
            }

            var areas = FOS.Setup.ManageArea.GetAreaListByJOBCityID(SOID,CityID);

            foreach (var a in areas)
            {
                Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input id='area" + a.ID + "' data-id='" + a.ID + "' class='' name='area[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + a.Name + "</span><p style='font-size:9px;'> " + a.ShortCode + "</p></div>";
            }

            return Json(new { Response = Reponse });
        }

        public JsonResult GetAreasRelatedToCityIDEdit(int JobID, int SOID, int CityID)
        {

            string Reponse = "";
       
            var areas = FOS.Setup.ManageArea.GetAreaListByCityIDJobEdit(JobID, SOID, CityID);


            var SO = db.SaleOfficers.Where(s => s.ID == SOID).FirstOrDefault();

            var CITY = db.Cities.Where(s => s.ID == CityID).FirstOrDefault();

            var AreasIDs = db.Jobs.Where(j => j.ID == JobID && j.SaleOfficerID == SOID && j.CityID == CityID && j.IsDeleted == false).Select(j => j.Areas).FirstOrDefault();

            var query = from val in AreasIDs.Split(',')
                        select int.Parse(val);

            var CityAreas = db.Areas.Where(a => a.CityID == CityID && !query.Contains(a.ID)).ToList();

            foreach (var a in areas)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input checked id='area" + a.ID + "' data-id='" + a.ID + "' class='' name='area[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + a.Name + "</span><p style='font-size:9px;'> " + a.ShortCode + "</p></div>";
                }

            foreach (var a in CityAreas)
                {
                        Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input id='area" + a.ID + "' data-id='" + a.ID + "' class='' name='area[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + a.Name + "</span><p style='font-size:9px;'> " + a.ShortCode + "</p></div>";
                }
            


                return Json(new { Response = Reponse});
        }


        #endregion

        #region Show Job


        // View ...
        public ActionResult ShowJob()
        {
            var regHeadList = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            ViewData["RegionalHead"] = regHeadList;
            ViewData["SaleOfficer"] = FOS.Setup.ManageSaleOffice.GetSaleOfficerByRegionalHeadID(regHeadList.FirstOrDefault().ID, true, true);


            return View();
        }

        public ActionResult ShowPlans()
        {

            ViewData["SaleOfficer"] = FOS.Setup.ManageSaleOffice.GetSaleOfficerList(true);


            return View();
        }

        public ActionResult ShowFullYearCal()
        {
            return View();
        }

        public JsonResult GetJobs(int SOID)
        {
            var Response = ManageJobs.GetJobsRelatedToSO(SOID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobsYearwise(int SOID, string year, string plan)
        {
            var Response = ManageJobs.GetJobsYearwise(SOID, year, plan);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetJobsMonthwise(int SOID, string month, string plan)
        {
            var Response = ManageJobs.GetJobsMonthwise(SOID, month, plan);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobsMonthwiseOfRegHead(int regionalHeadId, string month, string plan)
        {
            var Response = ManageJobs.GetJobsMonthwiseOfRegHead(regionalHeadId, month, plan);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Done Job

        [CustomAuthorize]
        public ActionResult DoneJob()
        {

         

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerList();

            //SaleOfficerObj.Insert(0, new SaleOfficer
            //{
            //    ID = 0,
            //    Name = "All"
            //});

            var objJob = new JobsData();
          
            objJob.SaleOfficer = SaleOfficerObj;


            return View (objJob);
         }


        // Get One City For Edit
        public JsonResult GetJobsDetailView(int JobDetaiID)
        {
            var Response = ManageJobs.GetJobsDetailView(JobDetaiID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        //Get All Region Method...
        public JsonResult JobDetailDataHandler(DTParameters param)
        {
            try
            { 
                var dtsource = new List<JobsDetailData>();

                //int regionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                //if (regionalheadID == 0)
                //{
                    dtsource = ManageJobs.GetJobsDetailForGrid();
                //}
                //else
                //{
                    //dtsource = ManageJobs.GetJobsDetailForGrid(regionalheadID);
                //}

                List<String> columnSearch = new List<String>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<JobsDetailData> data = ManageJobs.GetResult1(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch/*param.SaleOfficer,param.StartingDate1,param.StartingDate2*/);

                foreach (var itm in data)
                {
                    if (itm.AssignDate.HasValue)
                    {
                        itm.VisitDateFormatted = Convert.ToDateTime(itm.AssignDate).ToString("dd-MM-yyyy HH:mm");
                    }

                }

               // List<JobsDetailData> data = ManageJobs.GetResult1(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch/*param.SaleOfficer,param.StartingDate1,param.StartingDate2*/);
                int count = ManageJobs.Count1(param.Search.Value, dtsource, columnSearch /*param.SaleOfficer, param.StartingDate1, param.StartingDate2*/);
                DTResult<JobsDetailData> result = new DTResult<JobsDetailData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetItemsRelatedToJobID(int JobID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetItemsAcctoID(JobID);
            return Json(result);
        }

        public JsonResult GetClaimsData(int JobID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetClaimsAcctoID(JobID);
            return Json(result);
        }
        public JsonResult GetClaimsApprovalData(int JobID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetClaimsApprovalsAcctoID(JobID);
            return Json(result);
        }


        [HttpPost]
        public JsonResult GetDistributorOrdersToJobID(int JobID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetDistriburorOrdersAcctoID(JobID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetRetailerOrdersToJobID(int JobID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetRetailerOrdersAcctoID(JobID);
            return Json(result);
        }

        public class JobItemDto
        {
            public int itemid { get; set; }
            public string itemvalue { get; set; }

        }


        [HttpPost]
        public JsonResult SaveDispatchRelatedToJobID(string cont, DateTime date, int JobID)
        {

            var serialize = JsonConvert.DeserializeObject<List<JobItemDto>>(cont);


            JobItem retailerObj = new JobItem();
            JobsDetail retailerObj1 = new JobsDetail();
            tbl_StockDetail tblstock = new tbl_StockDetail();
            Tbl_MasterStock tblmaster = new Tbl_MasterStock();
            //  var variable = db.JobItems.Where(u => u.JobItemID == id).Select(u => u.JobItemID);

            foreach (var item in serialize)
            {
                int id = item.itemid;

                var ji = db.JobItems.Where(u => u.JobItemID == id).SingleOrDefault();
                var dealers = db.JobsDetails.Where(u => u.JobID == JobID).SingleOrDefault();


                var itms = db.tbl_StockDetail.Where(u => u.DealerID == dealers.DealerID && u.itemID == ji.ItemID).SingleOrDefault();
                if (itms != null)
                {
                    itms.Quantity += Convert.ToDecimal(item.itemvalue);
                    db.SaveChanges();
                }
                else
                {
                    int MasterID = db.Tbl_MasterStock.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault() + 1;
                    tblmaster.DistributorID = Convert.ToInt32(dealers.DealerID);
                    tblmaster.SaleOfficerID = Convert.ToInt32(dealers.SalesOficerID);
                    tblmaster.RegionalHeadID = Convert.ToInt32(dealers.RegionalHeadID);
                    tblmaster.CreatedOn = DateTime.Now;
                    db.Tbl_MasterStock.Add(tblmaster);
                    db.SaveChanges();
                    tblstock.Quantity = Convert.ToDecimal(item.itemvalue);
                    tblstock.itemID = ji.ItemID;
                    tblstock.DealerID = dealers.DealerID;
                    tblstock.StockMasterID = MasterID;
                    tblstock.CreatedOn = DateTime.Now;
                    tblstock.IsDeleted = false;
                    tblstock.IsActive = true;
                    db.tbl_StockDetail.Add(tblstock);
                    db.SaveChanges();
                }

                ji.DispatchQuantity = item.itemvalue;
                ji.DispatchDate = date;
                //  ji.Status = "Dispatched";
                ji.JobItemID = item.itemid;

                db.SaveChanges();

            }

            var data = db.JobsDetails.Where(u => u.JobID == JobID).SingleOrDefault();
            data.ActivityType = "Dispatched";
            db.SaveChanges();
            int res = 1;
            return Json(res);
        }



        #endregion

        [HttpPost]
        public JsonResult UpdateClaimStatus(int claimId, string status, string remarks, string docType = null)
        {
            try
            {
                // Claims (Tbl_SalesClaimMaster) and Dealer Verification Form (Tbl_DealerVerification)
                // have separate ID sequences, so which table to update must come from docType,
                // not be assumed - a Dealer Verification row's ID could collide with an
                // unrelated claim's ID. docType defaults to Claims for existing callers that
                // don't send it.
                if (docType == "DealerVerification")
                {
                    using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                    using (var cmd = new SqlCommand(@"
                        UPDATE dbo.Tbl_DealerVerification
                        SET ClaimManagerLatestStatus = @Status, ClaimManagerLatestRemarks = @Remarks, ClaimManagerDate = @ClaimManagerDate
                        WHERE ID = @ID", conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", (object)status ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Remarks", (object)remarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ClaimManagerDate", DateTime.UtcNow.AddHours(5));
                        cmd.Parameters.AddWithValue("@ID", claimId);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    var data = db.Tbl_SalesClaimMaster.Where(u => u.ID == claimId).SingleOrDefault();
                    data.ClaimManagerLatestStatus = status;
                    data.ClaimManagerLatestRemarks = remarks;
                    data.ClaimManagerDate = DateTime.UtcNow.AddHours(5);

                    db.SaveChanges();
                }

                // Also add to approval history if needed

                return Json(new { success = true, message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #region Job Uploader

        public ActionResult Upload()
        {
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerList();
            var objSalesOfficer = SaleOfficerObj.FirstOrDefault();

            List<RetailerData> RetailerObj = new List<RetailerData>();


            if (objSalesOfficer == null)
            {
                return View();
            }

            else
            {
                //RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(objSalesOfficer.ID);
                //ViewData["RetailerObj"] = RetailerObj;
            }

            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();

            List<VisitPlanData> visitData = new List<VisitPlanData>();
            visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.VisitPlan = visitData;
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");

            return View(objJob);
            //return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase files, JobsData data)
        {
            if (files != null)
            {
                var msg = "";
                var fileName = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + DateTime.Now.Hour + "" + DateTime.Now.Minute + "" + DateTime.Now.Second
                    + "" + Path.GetFileName(files.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/filez/"), fileName);
                files.SaveAs(path);

                var excelFilePath = Server.MapPath(@"~\Content\filez\" + fileName);
                
                DateTime jobDate = DateTime.Now.Date;
                int res = 0;
                data.JobTitle = "";
                // following are required for wallcoat things
                data.VisitType = "Shop";
                data.Type = 1;
                data.RetailerType = "Focused";

                foreach (var worksheet in Workbook.Worksheets(excelFilePath))
                {
                    int i = 1;
                    foreach (var row in worksheet.Rows.Skip(1))
                    {
                        i++;
                        if (row.Cells[0] != null && row.Cells[0].Value.Trim().Length > 0)
                        {
                            try
                            {
                                if (row.Cells[0].Text.Trim().Contains(","))
                                {
                                    data.RetailerID = Convert.ToInt32(row.Cells[0].Text.Trim().Replace(",", ""));
                                }
                                else
                                {
                                    data.RetailerID = Convert.ToInt32(row.Cells[0].Text.Trim());
                                }

                                data.SelectedRetailers = row.Cells[0].Value.Trim();
                            }
                            catch (Exception ex)
                            {
                                msg = msg + "At row# " + i + " controller level exception for retailer ids occured and it is :: " + ex.Message + " :: " + (ex.InnerException != null ? ex.InnerException.Message : "");
                                continue;
                            }

                            if (row.Cells[2] != null && row.Cells[2].Value.Trim().Length > 0)
                            {
                                try
                                {
                                    data.StartingDate = DateTime.FromOADate(Convert.ToDouble(row.Cells[2].Value.Trim()));
                                    //= jobDate.ToString("dd-MMM-yyyy");
                                    if(data.StartingDate.Value < Convert.ToDateTime("01-Dec-2017"))
                                    {
                                        msg = msg + " At v1:row# " + i + " Date or its format is incorrect.";
                                    }
                                    else
                                    {
                                        res = ManageJobs.AddUpdateJob(data);
                                        if (res == 2)
                                        {
                                            msg = msg + " At v1:row# " + i + " shop fixed id not found, so this line is not executed.";
                                        }
                                        else if (res == 0)
                                        {
                                            msg = msg + "At v1:row# " + i + " some exception occurred please see log, so this line is not executed.";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    msg = msg + "At v1:row# " + i + " controller level exception occured and it is :: "+ ex.Message + " :: " + (ex.InnerException != null ? ex.InnerException.Message : "");
                                }
                            }
                            if (row.Cells[3] != null && row.Cells[3].Value.Trim().Length > 0)
                            {
                                try
                                {
                                    data.StartingDate = DateTime.FromOADate(Convert.ToDouble(row.Cells[3].Value.Trim()));
                                    //= jobDate.ToString("dd-MMM-yyyy");
                                    if (data.StartingDate.Value < Convert.ToDateTime("01-Dec-2017"))
                                    {
                                        msg = msg + " At v2:row# " + i + " Date or its format is incorrect.";
                                    }
                                    else
                                    {
                                        res = ManageJobs.AddUpdateJob(data);
                                        if (res == 2)
                                        {
                                            msg = msg + " At v2:row# " + i + " shop fixed id not found, so this line is not executed.";
                                        }
                                        else if (res == 0)
                                        {
                                            msg = msg + "At v2:row# " + i + " some exception occurred please see log, so this line is not executed.";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    msg = msg + "At v2:row# " + i + " controller level exception occured and it is :: " + ex.Message + " :: " + (ex.InnerException != null ? ex.InnerException.Message : "");
                                }
                            }

                            if (row.Cells[4] != null && row.Cells[4].Value.Trim().Length > 0)
                            {
                                try
                                {
                                    data.StartingDate = DateTime.FromOADate(Convert.ToDouble(row.Cells[4].Value.Trim()));
                                    //= jobDate.ToString("dd-MMM-yyyy");
                                    if (data.StartingDate.Value < Convert.ToDateTime("01-Dec-2017"))
                                    {
                                        msg = msg + " At v3:row# " + i + " Date or its format is incorrect.";
                                    }
                                    else
                                    {
                                        res = ManageJobs.AddUpdateJob(data);
                                        if (res == 2)
                                        {
                                            msg = msg + " At v3:row# " + i + " shop fixed id not found, so this line is not executed.";
                                        }
                                        else if (res == 0)
                                        {
                                            msg = msg + "At v3:row# " + i + " some exception occurred please see log, so this line is not executed.";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    msg = msg + "At v3:row# " + i + " controller level exception occured and it is :: " + ex.Message + " :: " + (ex.InnerException != null ? ex.InnerException.Message : "");
                                }
                            }

                            if (row.Cells[5] != null && row.Cells[5].Value.Trim().Length > 0)
                            {
                                try
                                {
                                    data.StartingDate = DateTime.FromOADate(Convert.ToDouble(row.Cells[5].Value.Trim()));
                                    //= jobDate.ToString("dd-MMM-yyyy");
                                    if (data.StartingDate.Value < Convert.ToDateTime("01-Dec-2017"))
                                    {
                                        msg = msg + " At v4:row# " + i + " Date or its format is incorrect.";
                                    }
                                    else
                                    {
                                        res = ManageJobs.AddUpdateJob(data);
                                        if (res == 2)
                                        {
                                            msg = msg + " At v4:row# " + i + " shop fixed id not found, so this line is not executed.";
                                        }
                                        else if (res == 0)
                                        {
                                            msg = msg + "At v4:row# " + i + " some exception occurred please see log, so this line is not executed.";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    msg = msg + "At v4:row# " + i + " controller level exception occured and it is :: " + ex.Message + " :: " + (ex.InnerException != null ? ex.InnerException.Message : "");
                                }
                            }
                        }
                    }
                }
                if (msg.Length == 0)
                {
                    msg = "Jobs data uploaded successfully.";
                }

                TempData["UploadMessage"] = msg;
                return Upload();
            }
            else
            {
                TempData["UploadMessage"] = "Please provide the excel file.";
                return Upload();
            }
        }

        public JsonResult DeleteAllJobsBySOID(int SaleOfficerID)
        {
            var result = FOS.Setup.ManageJobs.DeleteAllJobsBySOID(SaleOfficerID);
            return Json(new { message = result + "" });
        }

        #endregion

        #region Job Reports
        
        public void ExportToExcel()
        {

            // Example data
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"SO Name\",\"RegionalHead\",\"Customer Name\",\"Person Name\",\"Job Date\",\"City Name\",\"Area Name\",\"Address\",\"LatLong\",\"Status\"");
                //,\"Available\"");//,\"Bank Name\",\"Phone1\",\"Phone2\",\"Retailer Type\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=JobsDetail" + DateTime.Now + ".csv");
            Response.ContentType = "application/octet-stream";

            var jobs = ManageJobs.GetJobsToExportInExcel();

            foreach (var jb in jobs)
            {
                jb.CompletedDateFormatted = jb.CompletedDate != null ? jb.CompletedDate.Value.ToString("dd-MMM-yyyy") : "";
                jb.VisitDateFormatted = jb.VisitDate.Value.ToString("dd-MMM-yyyy");
                jb.SAvailableFormatted = jb.SAvailable.HasValue && jb.SAvailable.Value ? "Yes" : "No";
                jb.SNewOrderFormatted = jb.SNewOrder.HasValue && jb.SNewOrder.Value ? "Yes" : "No";
                jb.SPOSMaterialAvailableFormatted = jb.SPOSMaterialAvailable.HasValue && jb.SPOSMaterialAvailable.Value ? "Yes" : "No";
                jb.SImageFormatted = jb.SImageFormatted != null ? "Yes" : "No";
            }

            foreach (var job in jobs)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"",

                job.SaleOfficerName,
                job.RegionalHeadName,
                job.ShopName,
                job.RetailerName,
            
                job.VisitDateFormatted,
                job.CityName,
                job.AreaName,
                job.ShopAddress,
                job.LatLong,
                job.StatusChecker
                

                ));
            }
            Response.Write(sw.ToString());
            Response.End();


        }

        public void ExportToExcelPaintersList()
        {

            // Example data
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"ID\",\"Name\",\"Registered\",\"CNIC\",\"City\",\"Market\",\"POS\",\"PhoneNumber\"");
            //,\"Available\"");//,\"Bank Name\",\"Phone1\",\"Phone2\",\"Retailer Type\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=PaintersDetail" + DateTime.Now + ".csv");
            Response.ContentType = "application/octet-stream";

            List<CityWisePainters> jobs = ManageJobs.GetPaintersToExportInExcel();


            foreach (var job in jobs)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",

                job.painterid,
                job.pname,
                job.Registered,
                job.cnic,
                job.city,
                job.Market,
                job.POS,
                job.PhoneNumber

                ));
            }
            Response.Write(sw.ToString());
            Response.End();


        }









        public void ExportToExcelConsolidate(int soId, string month, string startDate, string endDate)
        {
            var monthYr = month + " 2018";

            // Example data
            StringWriter sw = new StringWriter();

            //monthwise //sw.WriteLine("\"Month\",\"SO Name\",\"Total Jobs\",\"Visited Jobs\",\"Unvisited Jobs\",\"Visited Jobs %\",\"Unvisited Jobs %\"");
            sw.WriteLine("\"Date Range\",\"SO Name\",\"Total Jobs\",\"Visited Jobs\",\"Unvisited Jobs\",\"Visited Jobs %\",\"Unvisited Jobs %\"");
            

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=ConsolidateView" + soId + DateTime.Now + ".csv");
            Response.ContentType = "application/octet-stream";

            var jobs = ManageJobs.GetJobsMonthwise(soId, monthYr, "all", startDate, endDate);

            int totalAllJobs = 0;
            int totalAllVsJobs = 0;
            int totalAllUnvsJobs = 0;

            int totalVisited = 0;
            int totalUnvisited = 0;
            string soName = "";

            HashSet<int> uniqSoIds = new HashSet<int>();
            foreach (var mainJob in jobs)
            {
                uniqSoIds.Add(mainJob.SalesOfficerID);
            }
            int count = uniqSoIds.Count;
            int i = 0;
            foreach (var so in uniqSoIds)
            {
                totalVisited = 0;
                totalUnvisited = 0;
                foreach (var job in jobs)
                {
                    if(so == job.SalesOfficerID)
                    {
                        if (job.Status)
                        {
                            totalVisited++;
                        }
                        else
                        {
                            totalUnvisited++;
                        }
                        soName = job.SalesOfficerName;
                    }
                }
                decimal vsPer = ((decimal)((decimal)totalVisited / (decimal)(totalVisited + totalUnvisited)) * 100);
                decimal uvsPer = ((decimal)((decimal)totalUnvisited / (decimal)(totalVisited + totalUnvisited)) * 100);

                totalAllJobs = totalAllJobs + (totalVisited + totalUnvisited);
                totalAllVsJobs = totalAllVsJobs + totalVisited;
                totalAllUnvsJobs = totalAllUnvsJobs + totalUnvisited;

                string visitedPercent = vsPer.ToString("0.00") + " %";
                string unvisitedPercent = uvsPer.ToString("0.00") + " %";

                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                //month, soName, (totalVisited + totalUnvisited), totalVisited, totalUnvisited, visitedPercent, unvisitedPercent));
                startDate + " - " + endDate, soName, (totalVisited + totalUnvisited), totalVisited, totalUnvisited, visitedPercent, unvisitedPercent));
                if (i == count - 1)
                {
                    vsPer = ((decimal)((decimal)totalAllVsJobs / (decimal)(totalAllJobs)) * 100);
                    uvsPer = ((decimal)((decimal)totalAllUnvsJobs / (decimal)(totalAllJobs)) * 100);

                    visitedPercent = vsPer.ToString("0.00") + " %";
                    unvisitedPercent = uvsPer.ToString("0.00") + " %";

                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                    "", "Total:", totalAllJobs, totalAllVsJobs, totalAllUnvsJobs, visitedPercent, unvisitedPercent));
                }
                i++;
            }

           //if(sw.)

            Response.Write(sw.ToString());
            Response.End();


        }
        public ActionResult Consolidate()
        {
            //ViewData["RegionalHead"] = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            //ViewData["Dealers"] = FOS.Setup.ManageDealer.GetDealerList();
            ViewData["SaleOfficer"] = FOS.Setup.ManageSaleOffice.GetSaleOfficerList(true);
            //ViewData["City"] = FOS.Setup.ManageCity.GetCityList();
            //ViewData["Zone"] = FOS.Setup.ManageZone.GetZoneList();

            //var objDealer = ManageDealer.PlannedDistributors();
            //return View(objDealer);
            return View();
        }

        #endregion

        #region BeatPlan

        [CustomAuthorize]
        // View ...
        public ActionResult BeatPlan()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);

            regionalHeadData.Insert(0, new RegionalHeadData
            {
                ID = 0,
                Name = "Select"
            });
            //int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regionalHeadData.FirstOrDefault().ID, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "Select"
            });

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            var objSalesOfficer = SaleOfficerObj.FirstOrDefault();

            List<RetailerData> RetailerObj = new List<RetailerData>();


            if (objSalesOfficer == null)
            {
                return View();
            }
            else
            {
                RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(objSalesOfficer.ID);
                ViewData["RetailerObj"] = RetailerObj;
            }

            
            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new BeatPlanData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            //objJob.VisitPlan = visitData;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListBySOID(SaleOfficerObj.FirstOrDefault().ID);
            return View(objJob);
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateJob([Bind(Exclude = "TID")] JobsData newData)
        {
            Boolean boolFlag = true;
            int Res = 1;
            ValidationResult results = new ValidationResult();


            try
            {
                if (newData != null)
                {

                    if (boolFlag)
                    {

                        Res = ManageJobs.AddUpdateJob(newData);
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 2)
                        {
                            return Content("2");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        return Content("3");
                    }

                }
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
            return Content("0");
        }


        //Get All Region Method...
        public JsonResult JobDataHandler(DTParameters param, int RegionalHeadID, int SaleOfficerID)
        {
            try
            {

                var dtsource = new List<JobsData>();

                int regionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                //if (regionalheadID == 0)
                //{
                dtsource = ManageJobs.GetJobsForGrid(RegionalHeadID, SaleOfficerID);
                //}
                //else
                //{
                //    dtsource = ManageJobs.GetJobsForGrid(regionalheadID, SaleOfficerID);
                //}
                List<String> columnSearch = new List<String>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<JobsData> data = ManageJobs.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageJobs.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<JobsData> result = new DTResult<JobsData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        //Delete Job Method...
        public int DeleteJob(int JobID)
        {
            return ManageJobs.DeleteJob(JobID);
        }

    */

        public ActionResult LoadRetailersBeatPlan(int RegionalHeadID, int SaleOfficerID, string RetailerType, int CityID)
        {
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                List<RetailerData> DealerObject = new List<RetailerData>();
                List<RetailerData> RetailerObject = new List<RetailerData>();
                var saleOfficer = dbContext.SaleOfficers.Where(p => p.ID == SaleOfficerID).FirstOrDefault();
                if(saleOfficer != null)
                {
                    DealerObject = dbContext.Dealers.Where(r => r.RegionalHeadID == saleOfficer.RegionalHeadID
                    && r.IsActive && !r.IsDeleted).Select(r =>
                         new RetailerData
                         {
                             DealerID = r.ID,
                             DealerName = r.Name,
                         }).ToList();


                    RetailerObject = dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID
                    && r.RetailerType == (RetailerType.Equals("Both") ? r.RetailerType : RetailerType)
                    && r.Status == true && !r.IsDeleted && r.IsActive
                    && r.CityID == (CityID == -1 ? r.CityID : CityID))
                    .Select(r =>
                            new RetailerData
                            {
                                ID = r.ID,
                                Name = r.ShopName,
                               // DealerID = r.DealerID,
                                DealerName = r.Name,
                                Address = r.Address
                            }).OrderBy(r => r.Name).ToList();


                    foreach (var d in DealerObject)
                    {

                        if (RetailerObject.Where(r => r.DealerID == d.DealerID).Count() > 0)
                        {
                            Reponse += @"<div class='span12' style='margin-left:0px'><h4 style='color: #bf0000;font-weight: 600;'>" + d.DealerName + "</h4></div>";

                            foreach (var r in RetailerObject)
                            {
                                if (r.DealerID == d.DealerID)
                                {
                                    Reponse += @"<div class='span4' style='margin:0px;margin-left: 5px;'><input id='retailer" + r.ID + "' data-id='" + r.ID + "' data-dealerid='" + r.DealerID + "' class='' name='retailer[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #4a8bc2;font-weight: 700;'>" + r.Name + "</span><p style='font-size:9px;'> " + r.Address + "</p></div>";
                                }
                            }
                            Reponse += "<div class='span12' style='margin-left:0px'><hr></div>";
                        }
                    }
                }
            }

            return Json(new { Response = Reponse });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateBeatPlan([Bind(Exclude = "TID")] BeatPlanData newData)
        {
            Boolean boolFlag = true;
            int Res = 1;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {

                    if (boolFlag)
                    {

                        Res = ManageJobs.AddUpdateBeatPlan(newData);
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 2)
                        {
                            return Content("2");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        return Content("3");
                    }

                }
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
            return Content("0");
        }


        #endregion

        #region SEND SMS CODE SINGLE PAGE


        // Ye function Bhai JobController ma 

        public ActionResult LoadRetailerRelatedtoForSMS(int SaleOfficerID)
        {
            //string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
           
                List<RetailerData> RetailerObject = new List<RetailerData>();

                //int RID = (int)dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).Select(s => s.RegionalHeadID).FirstOrDefault();
                RetailerObject = dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID)
                    .Select(r =>
                        new RetailerData
                        {
                            ID = r.ID,
                            Name = r.Name,
                            ShopName = r.ShopName,

                        }).ToList();




                return Json(RetailerObject);
            }

        }













        // ye bhi job controller ma

        public JsonResult GetAllSaleOfficerListRelatedToDealerSelectOptionForSMS(int SaleOfficerID, bool selectOption)
        {
            var result = FOS.Setup.ManageJobs.GetAllSaleOfficerListRelatedToDealerForSMS(SaleOfficerID, selectOption);
            return Json(result);


        }

        public ActionResult SendSMS()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            regionalHeadData.Insert(0, new RegionalHeadData
            {
                ID = 0,
                Name = "Select"
            });

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadIDd();

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "Select"
            });

            List<RetailerData> RetailerObj = new List<RetailerData>();

            if (SaleOfficerObj == null)
            {
                return View();
            }

            else
            {
                //RetailerObj = FOS.Setup.ManageRetailer.GetAllRetailerSaleOfficerList(SaleOfficerObj.Select(s => s.ID).FirstOrDefault());
                RetailerObj.Insert(0, new RetailerData
                {
                    ID = 0,
                    Name = "All"
                });

               // ViewData["RetailerObj"] = RetailerObj;
            }



            List<VisitPlanData> visitData = new List<VisitPlanData>();
            visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            // objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            //objJob.VisitPlan = visitData;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListBySOID(SaleOfficerObj.FirstOrDefault().ID);

            return View(objJob);
        }

        [HttpPost]
        public ActionResult SendSMS(int retailerId)
        {
            if (retailerId == 0)
            {
                return Content("3");
            }
            else
            {

                var retailer = db.Retailers.Where(p => p.ID == retailerId).FirstOrDefault();
                string phoneNo = "";
                string phone2 = "";
                int soid = retailer.SaleOfficerID;
                if (retailer != null)
                {
                    if (!string.IsNullOrEmpty(retailer.Phone1))
                    {
                        phoneNo = retailer.Phone1.Trim();
                        var smsPin = "" + GenerateRandomNo();
                        var result = CallSmsApi(phoneNo, smsPin);
                        if (result == "1")
                        {
                            SMSLog sms = new SMSLog
                            {
                                PhoneNo = phoneNo,
                                RetailerID = retailerId,
                                SaleOfficerID = soid,
                                CreatedOn = DateTime.Now,
                                SmsPin = smsPin,
                                //ErrorDetail = error,
                                //Status = error.Equals("") ? 1 : 0
                            };

                            db.SMSLogs.Add(sms);
                            db.SaveChanges();
                            return Content("1");

                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(retailer.Phone2))
                        {
                            phone2 = retailer.Phone2.Trim();
                            var smsPin = "" + GenerateRandomNo();
                            var result = CallSmsApi(phone2, smsPin);
                            if (result == "1")
                            {
                                SMSLog sms = new SMSLog
                                {
                                    PhoneNo = phoneNo,
                                    RetailerID = retailerId,
                                    SaleOfficerID = soid,
                                    CreatedOn = DateTime.Now,
                                    SmsPin = smsPin,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };

                                db.SMSLogs.Add(sms);
                                db.SaveChanges();
                                return Content("1");
                            }
                            else
                            {
                                return Content("0");
                            }
                        }
                    }
                }
                return Content("0");
            }
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        public string CallSmsApi(string phoneNo , string msgText)
        {
            string phone = "923315140289";
            string URL = "http://weblogin.premiumsms.pk/sendsms_url.html";

            string urlParameters = "?Username=03214966886&Password=ms876&From=Test%20Acct&To="
                + phone + "&Message="
                + msgText;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                return "1";
            }
            else
            {
                return "Response StatusCode: " + (int)response.StatusCode + " :: ReasonPhrase :: " + response.ReasonPhrase;
            }
        }

        #endregion



        #region StockTaking

        [CustomAuthorize]
        public ActionResult StockTaking()
        {



            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerList();

            //SaleOfficerObj.Insert(0, new SaleOfficer
            //{
            //    ID = 0,
            //    Name = "All"
            //});

            var objJob = new JobsData();

            objJob.SaleOfficer = SaleOfficerObj;


            return View(objJob);
        }


        // Get One City For Edit
        public JsonResult GetStockDetailView(int JobDetaiID)
        {
            var Response = ManageJobs.GetJobsDetailView(JobDetaiID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        //Get All Region Method...
        public JsonResult StockDetailDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<JobsDetailData>();

                //int regionalheadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();

                //if (regionalheadID == 0)
                //{
                dtsource = ManageJobs.GetStockDetailForGrid();
                //}
                //else
                //{
                //dtsource = ManageJobs.GetJobsDetailForGrid(regionalheadID);
                //}

                List<String> columnSearch = new List<String>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<JobsDetailData> data = ManageJobs.GetResult11(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch/*param.SaleOfficer,param.StartingDate1,param.StartingDate2*/);
                foreach (var itm in data)
                {
                    if (itm.VisitedDate.HasValue)
                    {
                        itm.VisitDateFormatted = Convert.ToDateTime(itm.VisitedDate).ToString("dd-MM-yyyy HH:mm");
                        itm.RegionName = db.Regions.Where(x => x.ID == itm.RegionID).Select(x => x.Name).FirstOrDefault();
                    }

                }

                int count = ManageJobs.Count11(param.Search.Value, dtsource, columnSearch /*param.SaleOfficer, param.StartingDate1, param.StartingDate2*/);
                DTResult<JobsDetailData> result = new DTResult<JobsDetailData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetStockRelatedToJobID(int JobID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetStockAcctoID(JobID);
            return Json(result);
        }


        #endregion

        #region SendSMS
        public ActionResult MsgDistributor()
        {
            
            List<RegionData> regionData = new List<RegionData>();
            regionData = FOS.Setup.ManageRegion.GetRegionList();
            regionData.Insert(0, new RegionData
            {
                RegionID = 0,
                Name = "Select Region"
            });
            List<CityData> citydata = new List<CityData>();
            citydata = FOS.Setup.ManageCity.GetCityList();
            citydata.Insert(0, new CityData
            {
                ID = 0,
                Name = "All"
            });

            List<DealerData> dealerdata = new List<DealerData>();
            dealerdata = FOS.Setup.ManageDealer.GetAllDealersListShops();
            dealerdata.Insert(0, new DealerData
            {
                ID = 0,
                 Name= "All"
            });
            SendSMSData data = new SendSMSData();
            data.regiondata = regionData;
            data.dealerdata = dealerdata;
            data.citiesdata = citydata;

          
            return View(data);
        }
        public ActionResult MsgRetailers()
        {
            List<RegionData> regionData = new List<RegionData>();
            regionData = FOS.Setup.ManageRegion.GetRegionList();
            regionData.Insert(0, new RegionData
            {
                RegionID = 0,
                Name = "Select Region"
            });
            List<CityData> citydata = new List<CityData>();
            citydata = FOS.Setup.ManageCity.GetCityList();
            citydata.Insert(0, new CityData
            {
                ID = 0,
                Name = "All"
            });
            List<RetailerData> retailerdata = new List<RetailerData>();
            retailerdata = FOS.Setup.ManageRetailer.GetAllRetailersList();
            retailerdata.Insert(0, new RetailerData
            {
                ID = 0,
                ShopName = "All"
            });
            SendSMSData data = new SendSMSData();
            data.regiondata = regionData;
            data.retailerdata = retailerdata;
            data.citiesdata = citydata;
            return View(data);
        }
        public ActionResult MsgSaleOfficer()
        {
            List<RegionData> regionData = new List<RegionData>();
            regionData = FOS.Setup.ManageRegion.GetRegionList();
            regionData.Insert(0, new RegionData
            {
                RegionID = 0,
                Name = "Select Region"
            });
            List<SaleOfficerData> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSO();

            SaleOfficerObj.Insert(0, new SaleOfficerData
            {
                ID = 0,
                Name = "All"
            });
            SendSMSData data = new SendSMSData();
            data.regiondata = regionData;
            data.SaleOfficersdata = SaleOfficerObj;
            return View(data);
        }

        public JsonResult GetSaleOfficerbyRegionID(int? RegionID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetSaleOfficerListRelatedtoregionalHeadID(RegionID);
            return Json(result);
        }
        public JsonResult GetCitiesByRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetCityByRegionID(RegionID);
            return Json(result);
        }
        public JsonResult GetRetailersByRegionIDAndCityID(int RegionID,int CityID)
        {
            var result = FOS.Setup.ManageRetailer.GetAllRetailersRegionAndCity(RegionID,CityID);
            return Json(result);
        }
        public JsonResult GetDealersByRegionIDAndCityID(int RegionID, int CityID)
        {
            var result = FOS.Setup.ManageDealer.GetAllDealersByRegionAndCity(RegionID, CityID);
            return Json(result);
        }
        public ActionResult SendSMSDistributor(SendSMSData dt)
        {
            try
            {
                string phoneNo = "";
                string phone2 = "";
                FOSDataModel db = new FOSDataModel();
                if (dt.CityID == 0 && dt.RegionID > 0)
                {
                    List<Dealer> dealerdata = new List<Dealer>();
                    dealerdata = db.Dealers.Where(u => u.RegionID == dt.RegionID).ToList();
                    foreach (var data in dealerdata)
                    {
                        if (!string.IsNullOrEmpty(data.Phone1) && data.Phone1.Length == 12)
                        {
                            phoneNo = data.Phone1.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };

                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }
                        }
                        else if (!string.IsNullOrEmpty(data.Phone2) && data.Phone2.Length == 12)
                        {
                            phoneNo = data.Phone2.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }

                        }
                        else
                        {
                            Tbl_SMSLogs lg = new Tbl_SMSLogs
                            {
                                CityID = data.CityID,
                                RegionID = dt.RegionID,
                                DealerID = data.ID,
                                Type = "Dealer",
                                Message = dt.msg,
                                MessageDate = DateTime.UtcNow.AddHours(5),
                                Phone1 = data.Phone1,
                                Language = "English",
                                Status = false,
                                //ErrorDetail = error,
                                //Status = error.Equals("") ? 1 : 0
                            };
                            db.Tbl_SMSLogs.Add(lg);
                            db.SaveChanges();
                        }
                    }

                }
                else if (dt.CityID > 0 && dt.RegionID > 0 && dt.DealerID == 0)
                {
                    List<Dealer> dealerdata = new List<Dealer>();
                    dealerdata = db.Dealers.Where(u => u.RegionID == dt.RegionID && u.CityID == dt.CityID).ToList();
                    foreach (var data in dealerdata)
                    {
                        if (!string.IsNullOrEmpty(data.Phone1) && data.Phone1.Length == 12)
                        {
                            phoneNo = data.Phone1.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };

                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }
                        }
                        else if (!string.IsNullOrEmpty(data.Phone2) && data.Phone2.Length == 12)
                        {
                            phoneNo = data.Phone2.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }

                        }
                        else
                        {
                            Tbl_SMSLogs lg = new Tbl_SMSLogs
                            {
                                CityID = data.CityID,
                                RegionID = data.RegionID,
                                DealerID = data.ID,
                                Type = "Dealer",
                                Message = dt.msg,
                                MessageDate = DateTime.UtcNow.AddHours(5),
                                Phone1 = data.Phone1,
                                Language = "English",
                                Status = false,
                                //ErrorDetail = error,
                                //Status = error.Equals("") ? 1 : 0
                            };
                            db.Tbl_SMSLogs.Add(lg);
                            db.SaveChanges();
                        }
                    }

                }
                else if (dt.CityID > 0 && dt.RegionID > 0 && dt.DealerID > 0)
                {
                    List<Dealer> dealerdata = new List<Dealer>();
                    dealerdata = db.Dealers.Where(u => u.RegionID == dt.RegionID && u.CityID == dt.CityID && u.ID == dt.DealerID).ToList();
                    foreach (var data in dealerdata)
                    {
                        if (!string.IsNullOrEmpty(data.Phone1) && data.Phone1.Length == 12)
                        {
                            phoneNo = data.Phone1.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };

                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }
                        }
                        else if (!string.IsNullOrEmpty(data.Phone2) && data.Phone2.Length == 12)
                        {
                            phoneNo = data.Phone2.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    DealerID = data.ID,
                                    Type = "Dealer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }

                        }
                        else
                        {
                            Tbl_SMSLogs lg = new Tbl_SMSLogs
                            {
                                CityID = data.CityID,
                                RegionID = data.RegionID,
                                DealerID = data.ID,
                                Type = "Dealer",
                                Message = dt.msg,
                                MessageDate = DateTime.UtcNow.AddHours(5),
                                Phone1 = data.Phone1,
                                Language = "English",
                                Status = false,
                                //ErrorDetail = error,
                                //Status = error.Equals("") ? 1 : 0
                            };
                            db.Tbl_SMSLogs.Add(lg);
                            db.SaveChanges();
                        }
                    }

                }
                return Content("1");
            }
            catch(Exception ex)
            {
                return Content("0");
            }
          

        }
        public ActionResult SendSMSRetailer(SendSMSData dt)
        {
            try
            {
                string phoneNo = "";
                string phone2 = "";
                FOSDataModel db = new FOSDataModel();
                if (dt.CityID == 0 && dt.RegionID > 0)
                {
                    List<Retailer> dealerdata = new List<Retailer>();
                    dealerdata = db.Retailers.Where(u => u.RegionID == dt.RegionID).ToList();
                    foreach (var data in dealerdata)
                    {
                        if (!string.IsNullOrEmpty(data.Phone1) && data.Phone1.Length == 12)
                        {
                            phoneNo = data.Phone1.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };

                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }
                        }
                        else if (!string.IsNullOrEmpty(data.Phone2) && data.Phone2.Length == 12)
                        {
                            phoneNo = data.Phone2.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }

                        }
                        else
                        {
                            Tbl_SMSLogs lg = new Tbl_SMSLogs
                            {
                                CityID = data.CityID,
                                RegionID = data.RegionID,
                                RetailerID = data.ID,
                                Type = "Retailer",
                                Message = dt.msg,
                                MessageDate = DateTime.UtcNow.AddHours(5),
                                Phone1 = data.Phone1,
                                Language = "English",
                                Status = false,
                                //ErrorDetail = error,
                                //Status = error.Equals("") ? 1 : 0
                            };
                            db.Tbl_SMSLogs.Add(lg);
                            db.SaveChanges();
                        }
                    }

                }
                else if (dt.CityID > 0 && dt.RegionID > 0 && dt.ReatilerID == 0)
                {
                    List<Retailer> dealerdata = new List<Retailer>();
                    dealerdata = db.Retailers.Where(u => u.RegionID == dt.RegionID && u.CityID == dt.CityID).ToList();
                    foreach (var data in dealerdata)
                    {
                        if (!string.IsNullOrEmpty(data.Phone1) && data.Phone1.Length == 12)
                        {
                            phoneNo = data.Phone1.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };

                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }
                        }
                        else if (!string.IsNullOrEmpty(data.Phone2) && data.Phone2.Length == 12)
                        {
                            phoneNo = data.Phone2.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }

                        }
                        else
                        {
                            Tbl_SMSLogs lg = new Tbl_SMSLogs
                            {
                                CityID = data.CityID,
                                RegionID = data.RegionID,
                                RetailerID = data.ID,
                                Type = "Retailer",
                                Message = dt.msg,
                                MessageDate = DateTime.UtcNow.AddHours(5),
                                Phone1 = data.Phone1,
                                Language = "English",
                                Status = false,
                                //ErrorDetail = error,
                                //Status = error.Equals("") ? 1 : 0
                            };
                            db.Tbl_SMSLogs.Add(lg);
                            db.SaveChanges();
                        }
                    }

                }
                else if (dt.CityID > 0 && dt.RegionID > 0 && dt.ReatilerID > 0)
                {
                    List<Retailer> dealerdata = new List<Retailer>();
                    dealerdata = db.Retailers.Where(u => u.RegionID == dt.RegionID && u.CityID == dt.CityID && u.ID == dt.ReatilerID).ToList();
                    foreach (var data in dealerdata)
                    {
                        if (!string.IsNullOrEmpty(data.Phone1) && data.Phone1.Length == 12)
                        {
                            phoneNo = data.Phone1.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };

                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }
                        }
                        else if (!string.IsNullOrEmpty(data.Phone2) && data.Phone2.Length == 12)
                        {
                            phoneNo = data.Phone2.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    RetailerID = data.ID,
                                    Type = "Retailer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }

                        }
                        else
                        {
                            Tbl_SMSLogs lg = new Tbl_SMSLogs
                            {
                                CityID = data.CityID,
                                RegionID = data.RegionID,
                                RetailerID = data.ID,
                                Type = "Retailer",
                                Message = dt.msg,
                                MessageDate = DateTime.UtcNow.AddHours(5),
                                Phone1 = data.Phone1,
                                Language = "English",
                                Status = false,
                                //ErrorDetail = error,
                                //Status = error.Equals("") ? 1 : 0
                            };
                            db.Tbl_SMSLogs.Add(lg);
                            db.SaveChanges();
                        }
                    }

                }
                return Content("1");
            }
            catch (Exception ex)
            {
                return Content("0");
            }
           
        }

        public ActionResult SendSMSSaleOfficer(SendSMSData dt)
        {

            try
            {
                string phoneNo = "";
                string phone2 = "";
                FOSDataModel db = new FOSDataModel();
                if (dt.RegionID > 0 && dt.SaleOfficerID == 0)
                {
                    List<SaleOfficerData> dealerdata = new List<SaleOfficerData>();
                    List<Tbl_SOREGIONS> sodata = new List<Tbl_SOREGIONS>();
                    sodata = db.Tbl_SOREGIONS.Where(u => u.RegionID == dt.RegionID).ToList();
                    dealerdata = db.Tbl_SOREGIONS.Where(u => u.RegionID == dt.RegionID).Select(r => new SaleOfficerData
                    {
                        ID = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.ID).FirstOrDefault(),
                        Name = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.Name).FirstOrDefault(),
                        Phone1 = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.Phone1).FirstOrDefault(),
                        Phone2 = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.Phone2).FirstOrDefault(),
                        CityID = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.CityID).FirstOrDefault(),
                        RegionID = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.RegionID).FirstOrDefault(),
                    }).ToList();
                    foreach (var data in dealerdata)
                    {
                        if (!string.IsNullOrEmpty(data.Phone1) && data.Phone1.Length == 12)
                        {
                            phoneNo = data.Phone1.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                               
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {

                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    SaleOfficerID = data.ID,
                                    Type = "SaleOfficer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };

                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    SaleOfficerID = data.ID,
                                    Type = "SaleOfficer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }
                        }
                        else if (!string.IsNullOrEmpty(data.Phone2) && data.Phone2.Length == 12)
                        {
                            phoneNo = data.Phone2.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    SaleOfficerID = data.ID,
                                    Type = "SaleOfficer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    SaleOfficerID = data.ID,
                                    Type = "SaleOfficer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }

                        }
                        else
                        {
                            Tbl_SMSLogs lg = new Tbl_SMSLogs
                            {
                                CityID = data.CityID,
                                RegionID = data.RegionID,
                                SaleOfficerID = data.ID,
                                Type = "SaleOfficer",
                                Message = dt.msg,
                                MessageDate = DateTime.UtcNow.AddHours(5),
                                Phone1 = data.Phone1,
                                Language = "English",
                                Status = false,
                                //ErrorDetail = error,
                                //Status = error.Equals("") ? 1 : 0
                            };
                            db.Tbl_SMSLogs.Add(lg);
                            db.SaveChanges();
                        }
                    }

                }

                else if (dt.RegionID > 0 && dt.SaleOfficerID > 0)
                {
                    List<SaleOfficer> dealerdata = new List<SaleOfficer>();
                    dealerdata = db.Tbl_SOREGIONS.Where(u => u.RegionID == dt.RegionID && u.SaleofficerID == dt.SaleOfficerID).Select(r => new SaleOfficer
                    {
                        ID = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.ID).FirstOrDefault(),
                        Name = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.Name).FirstOrDefault(),
                        Phone1 = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.Phone1).FirstOrDefault(),
                        Phone2 = db.SaleOfficers.Where(u => u.ID == r.SaleofficerID).Select(u => u.Phone2).FirstOrDefault()
                    }).ToList();
                    foreach (var data in dealerdata)
                    {
                        if (!string.IsNullOrEmpty(data.Phone1) && data.Phone1.Length == 12)
                        {
                            phoneNo = data.Phone1.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    SaleOfficerID = data.ID,
                                    Type = "SaleOfficer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };

                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    SaleOfficerID = data.ID,
                                    Type = "SaleOfficer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone1,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }
                        }
                        else if (!string.IsNullOrEmpty(data.Phone2) && data.Phone2.Length == 12)
                        {
                            phoneNo = data.Phone2.Trim();
                            var smsPin = dt.msg;
                            var result = CallSmsApi(phoneNo, smsPin);
                            if (result == "1")
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    SaleOfficerID = data.ID,
                                    Type = "SaleOfficer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = true,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();

                            }
                            else
                            {
                                Tbl_SMSLogs lg = new Tbl_SMSLogs
                                {
                                    CityID = data.CityID,
                                    RegionID = data.RegionID,
                                    SaleOfficerID = data.ID,
                                    Type = "SaleOfficer",
                                    Message = dt.msg,
                                    MessageDate = DateTime.UtcNow.AddHours(5),
                                    Phone1 = data.Phone2,
                                    Language = "English",
                                    Status = false,
                                    //ErrorDetail = error,
                                    //Status = error.Equals("") ? 1 : 0
                                };
                                db.Tbl_SMSLogs.Add(lg);
                                db.SaveChanges();


                            }

                        }
                        else
                        {
                            Tbl_SMSLogs lg = new Tbl_SMSLogs
                            {
                                CityID = data.CityID,
                                RegionID = data.RegionID,
                                SaleOfficerID = data.ID,
                                Type = "SaleOfficer",
                                Message = dt.msg,
                                MessageDate = DateTime.UtcNow.AddHours(5),
                                Phone1 = data.Phone1,
                                Language = "English",
                                Status = false,
                                //ErrorDetail = error,
                                //Status = error.Equals("") ? 1 : 0
                            };
                            db.Tbl_SMSLogs.Add(lg);
                            db.SaveChanges();
                        }
                    }


                }
                return Content("1");
        }
            catch (Exception ex)
            {
                Tbl_SMSLogs lg = new Tbl_SMSLogs
                {
                    CityID = 0,
                    RegionID = 0,
                    SaleOfficerID = 0,
                    Type = "SaleOfficer",
                    Message = dt.msg,
                    MessageDate = DateTime.UtcNow.AddHours(5),
                    Phone1 = "0",
                    Language =ex.InnerException.ToString(),
                    Status = false,
                    //ErrorDetail = error,
                    //Status = error.Equals("") ? 1 : 0
                };
                db.Tbl_SMSLogs.Add(lg);
                db.SaveChanges();
                return Content("0");
    }
   
        }

        public JsonResult DistributorSMSGrid(DTParameters param)
        {
            try
            {
                var dtsource = new List<SendSMSData>();

                    dtsource = ManageJobs.GetSMSDetailForGridForDistributor();
              
                List<String> columnSearch = new List<String>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SendSMSData> data = ManageJobs.GetResult1(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch/*param.SaleOfficer,param.StartingDate1,param.StartingDate2*/);


                int count = ManageJobs.Count1(param.Search.Value, dtsource, columnSearch /*param.SaleOfficer, param.StartingDate1, param.StartingDate2*/);
                DTResult<SendSMSData> result = new DTResult<SendSMSData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public JsonResult SaleOfficerSMSGrid(DTParameters param)
        {
            try
            {
                var dtsource = new List<SendSMSData>();

                dtsource = ManageJobs.GetSMSDetailForGridForSaleOfficers();

                List<String> columnSearch = new List<String>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SendSMSData> data = ManageJobs.GetResult1(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch/*param.SaleOfficer,param.StartingDate1,param.StartingDate2*/);


                int count = ManageJobs.Count1(param.Search.Value, dtsource, columnSearch /*param.SaleOfficer, param.StartingDate1, param.StartingDate2*/);
                DTResult<SendSMSData> result = new DTResult<SendSMSData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public JsonResult RetailerSMSGrid(DTParameters param)
        {
            try
            {
                var dtsource = new List<SendSMSData>();

                dtsource = ManageJobs.GetSMSDetailForGridForRetailers();

                List<String> columnSearch = new List<String>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SendSMSData> data = ManageJobs.GetResult1(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch/*param.SaleOfficer,param.StartingDate1,param.StartingDate2*/);


                int count = ManageJobs.Count1(param.Search.Value, dtsource, columnSearch /*param.SaleOfficer, param.StartingDate1, param.StartingDate2*/);
                DTResult<SendSMSData> result = new DTResult<SendSMSData>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        #endregion SendSMS


    }

    //public ActionResult LoadWeeklyRetailers(int RegionalHeadID, int SaleOfficerID, string RetailerType, int CityID)
    //{
    //    string Reponse = "";
    //    using (FOSDataModel dbContext = new FOSDataModel())
    //    {
    //        List<RetailerData> DealerObject = new List<RetailerData>();
    //        List<RetailerData> RetailerObject = new List<RetailerData>();

    //        int RID = (int)dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).Select(s => s.RegionalHeadID).FirstOrDefault();

    //        DealerObject = dbContext.Dealers.Where(r => r.RegionalHeadID == RID).Select(r =>
    //                 new RetailerData
    //                 {
    //                     DealerID = r.ID,
    //                     DealerName = r.Name,
    //                 }).ToList();


    //        RetailerObject = dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID
    //            && r.RetailerType == (RetailerType.Equals("Both") ? r.RetailerType : RetailerType)
    //            && r.Status == true && !r.IsDeleted && r.IsActive
    //            && r.CityID == (CityID == -1 ? r.CityID : CityID))
    //            .Select(r =>
    //                new RetailerData
    //                {
    //                    ID = r.ID,
    //                    Name = r.Name,
    //                    SaleOfficerID = r.SaleOfficerID,
    //                    SaleOfficerName = r.SaleOfficer.Name,
    //                    DealerID = r.DealerID,
    //                    DealerName = r.Name,
    //                    CItyName = r.City.Name,
    //                    AreaName = r.Area.Name,
    //                    RetailerType = r.RetailerType,
    //                    ShopName = r.ShopName,
    //                    Address = r.Address
    //                }).OrderBy(r => r.Name).ToList();

    //        Reponse += "<div class=''>";
    //        foreach (var d in DealerObject)
    //        {
    //            if (RetailerObject.Where(r => r.DealerID == d.DealerID).Count() > 0)
    //            {

    //                Reponse += "<div class='span12' style='margin-left:8px'><h4>" + d.DealerName + " <span style=''>(" + RetailerObject.Where(r => r.DealerID == d.DealerID).Count() + ")</span></h4></div>";
    //                Reponse += "<div class='span12' style='margin-left:6px'>";
    //                foreach (var r in RetailerObject)
    //                {
    //                    if (r.DealerID == d.DealerID)
    //                    {
    //                        Reponse += "<a data-toggle='modal' data-target='#myModal' class='RetailerInfo fc-event ui-draggable ui-draggable-handle' style='z-index: auto;left: 0px;top: 0px;' data-id='" + r.ID + "' data-dealerid='" + r.DealerID + "' data-name='" + r.Name + "' data-salesofficerid='" + r.SaleOfficerID + "' data-cityname='" + r.CItyName + "' data-areaname='" + r.AreaName + "' data-retailertype='" + r.RetailerType + "' data-shopname='" + r.ShopName + "' data-address='" + r.Address + "'>" + r.ShopName + "</a>";
    //                    }
    //                }
    //                Reponse += "</div>";
    //            }
    //        }
    //        Reponse += "<p style='display:none'><input type='checkbox' id='drop-remove' checked><label for='drop-remove'>Remove After Drop</label></p>";
    //        Reponse += "</div>";
    //    }

    //    return Json(new { Response = Reponse });
    //}

    //public ActionResult LoadWeeklyRetailersForSMS(int RegionalHeadID, int SaleOfficerID)
    //{
    //    //string Reponse = "";
    //    using (FOSDataModel dbContext = new FOSDataModel())
    //    {
    //        List<RetailerData> DealerObject = new List<RetailerData>();
    //        List<RetailerData> RetailerObject = new List<RetailerData>();

    //        int RID = (int)dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).Select(s => s.RegionalHeadID).FirstOrDefault();

    //        DealerObject = dbContext.Dealers.Where(r => r.RegionalHeadID == RID).Select(r =>
    //                 new RetailerData
    //                 {
    //                     DealerID = r.ID,
    //                     DealerName = r.Name,
    //                 }).ToList();


    //        RetailerObject = dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID

    //           )
    //            .Select(r =>
    //                new RetailerData
    //                {
    //                    ID = r.ID,
    //                    Name = r.Name,
    //                    SaleOfficerID = r.SaleOfficerID,
    //                    SaleOfficerName = r.SaleOfficer.Name,
    //                    DealerID = r.DealerID,
    //                    DealerName = r.Name,
    //                    CItyName = r.City.Name,
    //                    AreaName = r.Area.Name,
    //                    RetailerType = r.RetailerType,
    //                    ShopName = r.ShopName,
    //                    Address = r.Address
    //                }).OrderBy(r => r.Name).ToList();



    //        return Json(RetailerObject);
    //    }


    //}




    //public ActionResult LoadRetailer(int CityId)
    //{
    //    //string Reponse = "";
    //    using (FOSDataModel dbContext = new FOSDataModel())
    //    {
    //        List<RetailerData> DealerObject = new List<RetailerData>();
    //        List<RetailerData> RetailerObject = new List<RetailerData>();



    //        RetailerObject = dbContext.Retailers.Where(r => r.CityID == CityId)

    //            .Select(r =>
    //                new RetailerData
    //                {
    //                    ID = r.ID,
    //                    Name = r.Name,
    //                    SaleOfficerID = r.SaleOfficerID,
    //                    SaleOfficerName = r.SaleOfficer.Name,
    //                    DealerID = r.DealerID,
    //                    DealerName = r.Name,
    //                    CItyName = r.City.Name,
    //                    AreaName = r.Area.Name,
    //                    RetailerType = r.RetailerType,
    //                    ShopName = r.ShopName,
    //                    Address = r.Address
    //                }).OrderBy(r => r.Name).ToList();

    //        RetailerObject.Insert(0, new RetailerData
    //        {
    //            ID = 0,
    //            ShopName = "All"
    //        });


    //        return Json(RetailerObject);
    //    }


    //}



    //public List<RetailerData> LoadWeeklyRetailersForSMS(int RegionalHeadID, int SaleOfficerID, int CityID)
    //{
    //    //string Reponse = "";
    //    using (FOSDataModel dbContext = new FOSDataModel())
    //    {
    //        List<RetailerData> DealerObject = new List<RetailerData>();
    //        List<RetailerData> RetailerObject = new List<RetailerData>();

    //        int RID = (int)dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID).Select(s => s.RegionalHeadID).FirstOrDefault();

    //        //DealerObject = dbContext.Dealers.Where(r => r.RegionalHeadID == RID).Select(r =>
    //        //         new RetailerData
    //        //         {
    //        //             DealerID = r.ID,
    //        //             DealerName = r.Name,
    //        //         }).ToList();


    //        RetailerObject = dbContext.Retailers.Where(r => r.SaleOfficerID == SaleOfficerID
    //            // && r.RetailerType == (RetailerType.Equals("Both") ? r.RetailerType : RetailerType)
    //            && r.Status == true && !r.IsDeleted && r.IsActive
    //            && r.CityID == (CityID == -1 ? r.CityID : CityID))
    //            .Select(r =>
    //                new RetailerData
    //                {
    //                    ID = r.ID,
    //                    Name = r.Name,
    //                    SaleOfficerID = r.SaleOfficerID,
    //                    SaleOfficerName = r.SaleOfficer.Name,
    //                    DealerID = r.DealerID,
    //                    DealerName = r.Name,
    //                    CItyName = r.City.Name,
    //                    AreaName = r.Area.Name,
    //                    RetailerType = r.RetailerType,
    //                    ShopName = r.ShopName,
    //                    Address = r.Address
    //                }).OrderBy(r => r.Name).ToList();

    //        //    Reponse += "<div class=''>";
    //        //    foreach (var d in DealerObject)
    //        //    {
    //        //        if (RetailerObject.Where(r => r.DealerID == d.DealerID).Count() > 0)
    //        //        {

    //        //            Reponse += "<div class='span12' style='margin-left:8px'><h4>" + d.DealerName + " <span style=''>(" + RetailerObject.Where(r => r.DealerID == d.DealerID).Count() + ")</span></h4></div>";
    //        //            Reponse += "<div class='span12' style='margin-left:6px'>";
    //        //            foreach (var r in RetailerObject)
    //        //            {
    //        //                if (r.DealerID == d.DealerID)
    //        //                {
    //        //                    Reponse += "<a data-toggle='modal' data-target='#myModal' class='RetailerInfo fc-event ui-draggable ui-draggable-handle' style='z-index: auto;left: 0px;top: 0px;' data-id='" + r.ID + "' data-dealerid='" + r.DealerID + "' data-name='" + r.Name + "' data-salesofficerid='" + r.SaleOfficerID + "' data-cityname='" + r.CItyName + "' data-areaname='" + r.AreaName + "' data-retailertype='" + r.RetailerType + "' data-shopname='" + r.ShopName + "' data-address='" + r.Address + "'>" + r.ShopName + "</a>";
    //        //                }
    //        //            }
    //        //            Reponse += "</div>";
    //        //        }
    //        //    }
    //        //    Reponse += "<p style='display:none'><input type='checkbox' id='drop-remove' checked><label for='drop-remove'>Remove After Drop</label></p>";
    //        //    Reponse += "</div>";
    //        //}

    //        ViewBag.data = RetailerObject;
    //        return ViewBag.data;
    //    }

    //}


    //public ActionResult LoadWeeklyPainters(int SOID)
    //{
    //    string Reponse = "";
    //    using (FOSDataModel dbContext = new FOSDataModel())
    //    {
    //        List<PainterAssociationData> PainterObject = new List<PainterAssociationData>();

    //        var painters = dbContext.PaintersBySalesOfficer(SOID).Where(p => p.Selected != true);
    //        foreach (var p in painters)
    //        {
    //            Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input id='retailer" + p.PainterID + "' data-id='" + p.PainterID + "' class='' name='retailer[]' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.PainterName + "</span><p style='font-size:9px;'> " + p.City + "</p></div>";
    //        }

    //    }

    //    return Json(new { Response = Reponse });
    //}







    public class SpReturnTest
    {
        public int id { get; set; }
    }

    public class DispatchVAlues
    {
        public int JobID { get; set; }

    }

    public class Test
    {
        public string[] Names;
    }


}