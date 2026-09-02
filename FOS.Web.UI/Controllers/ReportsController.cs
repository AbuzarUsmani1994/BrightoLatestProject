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


using FOS.Web.UI.DataSets;
using CrystalDecisions.CrystalReports.Engine;
using Shared.Diagnostics.Logging;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Data.Entity.Core.Objects;
using System.Net;
using FOS.Web.UI.Controllers.API;
using System.Data.Entity;
using System.Globalization;
using System.Security.Cryptography;



namespace FOS.Web.UI.Controllers
{
    public class ReportsController : Controller
    {
        FOSDataModel db = new FOSDataModel();

        private static string GetSafeString(IDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
            {
                return string.Empty;
            }
            var value = reader.GetValue(ordinal);
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("dd-MMM-yyyy hh:mm tt");
            }
            return value.ToString();
        }


    
    #region FOS Wise Date/Month Wise Intake Delivered Report-1A
    [HttpGet]
        public ActionResult FosDateWiseReport()
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();
            obj.FosNames = objPainter.getFosLov(0).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID.ToString()
            });
            var customer = objPainter.GetCityList(1);
            obj.PainterCityNames = customer;
            return View(obj);
        }
        #endregion

        #region FOS Wise Date/Month Wise Intake Report
        [HttpGet]
        public ActionResult FosDateMonthWiseIntakeReport()
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();
            obj.FosNames = objPainter.getFosLov(0).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID.ToString()
            });
            var customer = objPainter.GetCityList(1);
            obj.PainterCityNames = customer;
            return View(obj);
        }
        #endregion

        #region City Wise Date Wise Intake Delivered Report
        [HttpGet]
        public ActionResult CityDateWiseRetailerReport()
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();
            obj.FosNames = objPainter.getFosLov(0).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID.ToString()
            });
            var customer = objPainter.GetCityList(1);
            obj.PainterCityNames = customer;
            return View(obj);
        }
        #endregion

        #region City Wise Month Wise Intake Delivered Report
        [HttpGet]
        public ActionResult CityDateMonthWiseIntakeReport()
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();
            obj.FosNames = objPainter.getFosLov(0).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID.ToString()
            });
            var customer = objPainter.GetCityList(1);
            obj.PainterCityNames = customer;
            return View(obj);
        }
        #endregion

        #region Retail Shop Wise Date Wise Intake Delivered Report
        [HttpGet]
        public ActionResult RetailerShopDateWiseReport()
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();
            obj.FosNames = objPainter.getFosLov(0).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID.ToString()
            });
            var customer = objPainter.GetAllCitiesList();
            obj.PainterCityNames = customer;
            return View(obj);
        }
        #endregion

        #region Retail Shop Wise Month Wise Intake Delivered Report
        [HttpGet]
        public ActionResult RetailerShopDateMonthWiseIntakeReport()
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();
            obj.FosNames = objPainter.getFosLov(0).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID.ToString()
            });
            var customer = objPainter.GetAllCitiesList();
            obj.PainterCityNames = customer;
            return View(obj);
        }
        #endregion

        #region City Fos Wise Report
        [HttpGet]
        public ActionResult CityWiseFosReport()
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();
            obj.FosNames = objPainter.getFosLov(0).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID.ToString()
            });
            var customer = objPainter.GetAllCitiesList();
            obj.PainterCityNames = customer;
            return View(obj);
        }
        #endregion

        #region City Market Retailer Wise Report
        public ActionResult CityMktRtlrWiseReport()
        {
            ReportsInputData obj = new ReportsInputData();
            ManagePainter objPainter = new ManagePainter();
            obj.Territories = objPainter.GetTerritoryNamesList();
            obj.FosNames = objPainter.getFosLov(0).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID.ToString()
            });
            var customer = objPainter.GetAllCitiesList();
            obj.PainterCityNames = customer;
            return View(obj);
        }
        #endregion

        #region Helping Methods
        public JsonResult getData(string cities)
        {
            string[] mul = cities.Split(',');
            return null;
        }
        public ActionResult getCities(string TID)
        {
            int tid = 0;
            if (!string.IsNullOrEmpty(TID))
            {
                tid = Convert.ToInt32(TID);
            }
            ManagePainter objPainter = new ManagePainter();
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var customer = objPainter.GetCityList(tid).OrderBy(x => x.CityName);
                foreach (var p in customer)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input " + ((p.Selected == false) ? "" : "checked") + " id='painter" + p.ID + "' data-id='" + p.ID + "' class='pCheckBox' name='painter[]' onchange='painterSelected(this)' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.CityName + "</span><p style='font-size:9px;' id='cityName'>" + p.CityName + "</p></div>";
                }

            }

            return Json(new { Response = Reponse });
        }

        public ActionResult getShops(string CID)
        {
            int tid = 0;
            if (!string.IsNullOrEmpty(CID))
            {
                tid = Convert.ToInt32(CID);
            }
            ManagePainter objPainter = new ManagePainter();
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var customer = objPainter.GetShopsList(tid).OrderBy(x => x.CityName);
                foreach (var p in customer)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input " + ((p.Selected == false) ? "" : "checked") + " id='painter" + p.ID + "' data-id='" + p.ID + "' class='pCheckBox' name='painter[]' onchange='painterSelected(this)' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.CityName + "</span><p style='font-size:9px;' id='cityName'>" + p.CityName + "</p></div>";
                }

            }

            return Json(new { Response = Reponse });
        }

        public ActionResult getAreas(string CID)
        {
            int tid = 0;
            if (!string.IsNullOrEmpty(CID))
            {
                tid = Convert.ToInt32(CID);
            }
            ManagePainter objPainter = new ManagePainter();
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var customer = objPainter.GetAreaList(tid).OrderBy(x => x.CityName);
                foreach (var p in customer)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input " + ((p.Selected == false) ? "" : "checked") + " id='painter" + p.ID + "' data-id='" + p.ID + "' class='pCheckBox' name='painter[]' onchange='painterSelected(this)' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.CityName + "</span><p style='font-size:9px;' id='cityName'>" + p.CityName + "</p></div>";
                }

            }

            return Json(new { Response = Reponse });
        }

        public ActionResult getMarkets(string CID)
        {
            int tid = 0;
            if (!string.IsNullOrEmpty(CID))
            {
                tid = Convert.ToInt32(CID);
            }
            ManagePainter objPainter = new ManagePainter();
            string Reponse = "";
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                var customer = objPainter.GetMarketList(tid).OrderBy(x => x.CityName);
                foreach (var p in customer)
                {
                    Reponse += @"<div class='span3 scroll' style='margin:0px;margin-left: 0px;'><input " + ((p.Selected == false) ? "" : "checked") + " id='painter" + p.ID + "' data-id='" + p.ID + "' class='pCheckBox' name='painter[]' onchange='painterSelected(this)' type='checkbox' style='margin: -1px 0 0;'><span class='lbl' style='font-size: 11px;margin-left: 5px;color: #000000;'>" + p.CityName + "</span><p style='font-size:9px;' id='cityName'>" + p.CityName + "</p></div>";
                }

            }

            return Json(new { Response = Reponse });
        }

        public ActionResult getFosNames(string TID)
        {
            int tid = 0;
            if (!string.IsNullOrEmpty(TID))
            {
                tid = Convert.ToInt32(TID);
            }
            ManagePainter objPainter = new ManagePainter();
            var customers = objPainter.getFosLov(tid).OrderBy(x => x.Name);
            var customer = objPainter.getFosLov(tid).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.ID
            });
            return Json(customer);
        }

        public ActionResult getCitiess(string TID)
        {
            int tid = 0;
            if (!string.IsNullOrEmpty(TID))
            {
                tid = Convert.ToInt32(TID);
            }
            ManagePainter objPainter = new ManagePainter();
            var customer = objPainter.GetCityList(tid).OrderBy(x => x.CityName);


            return Json(customer);
        }
        #endregion

        public ActionResult FOSPlanning()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            regionalHeadData.Insert(0, new RegionalHeadData
            {
                ID = 0,
                Name = "All"
            });

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regionalHeadData.FirstOrDefault().ID, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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

        public ActionResult FOSPlanningReport(string StartingDate, string EndingDate, int TID, int fosid)
        {
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_FosPlanning_Result> result = objRetailers.FOSPlanningReport(start, end, TID, fosid);
                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol51 = new DataColumn("Teritory Head", typeof(System.String));
                dtNewTable.Columns.Add(dcol51);
                DataColumn dcol5 = new DataColumn("SaleOfficerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol2 = new DataColumn("DealerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("CityName", typeof(System.String));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("ShopName", typeof(System.String));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("JobDate", typeof(System.String));
                dtNewTable.Columns.Add(dcol6);

                foreach (var item in result)
                {

                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = item.Name;
                    dtrow[1] = item.SaleOfficerName;
                    dtrow[2] = item.DealerName;
                    dtrow[3] = item.CityName;
                    dtrow[4] = item.ShopName;
                    dtrow[5] = item.JobDate;

                    dtNewTable.Rows.Add(dtrow);

                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/FOSPlanning.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "FOSPlanning.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("FOSPlanning{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }

        }

        public ActionResult RetailerAvgSale()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            List<Shared.MainCategories> CityObj = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Regions = CityObj;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListBySOID(SaleOfficerObj.FirstOrDefault().ID);


            return View(objJob);
        }

        public void RetailerSaleAverage(int TID, int cityid, DateTime sdate, DateTime edate)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }


            try
            {


                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_RetailerAvgSale_Result> result = objRetailers.AvgSale(TID, 6, cityid, sdate, edate);

                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Shop Name\",\"Region Name\",\"City Name\",\"SaleOfficer Name\",\"Total Orders\",\"Distributor Name\",\"Total Value\",\"Average Value\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=RetailersAVGSale" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";





               
                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",


                        // retailer.Name,
                    retailer.shopname,
                        retailer.Region,
                        retailer.CityName,
                        retailer.SOName,
                        retailer.Orders,
                        retailer.rangeADealer,
                        retailer.value,
                        retailer.TotalAvgValue



                        ));
                    }

               


                Response.Write(sw.ToString());
                Response.End();



                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;

                hst.ReportName = "Retailer Average Sale";
                hst.ReportType = "AVG Sale";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

        }

        public ActionResult RetailerSummery()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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


            List<Shared.MainCategories> CityObj = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Regions = CityObj;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListByRegionIDD(regionalHeadData.FirstOrDefault().ID);


            return View(objJob);
        }

        public void RetailerSummaryRpt(int TID, int cityid, DateTime sdate, DateTime edate)
        {
            DateTime start = sdate;
            DateTime end = edate;
            DateTime final = end.AddDays(1);
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }


            try
            {


                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_Total_RetailerInformationSummery_Result> result = db.Sp_Total_RetailerInformationSummery(TID,0, cityid,start,final,6).ToList();

                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SrNO \",\"Region\",\"City\",\"Trade Count\",\"Housing Count\",\"Corporate Count\",\"All Purpose Count\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=RetailersSummaryRpt" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                int srNo = 1;

                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",

                        srNo,
                     retailer.Region,
                      retailer.City,
                    retailer.TradeParties,
                      retailer.HousingParties,

                   retailer.CorporateParties,
                        retailer.AllPurposeParties,

                    srNo++


                    ));
                }





                Response.Write(sw.ToString());
                Response.End();



                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;

                hst.ReportName = "RetailerInformation";
                hst.ReportType = "RetailerInfo";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

        }

        public ActionResult RetailerInfo()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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


            List<Shared.MainCategories> CityObj = db.Tbl_Segmenttype
                            .Select(
                                u => new Shared.MainCategories
                                {
                                    ID = u.ID,
                                    MainCategoryName = u.Name,
                                }).ToList();
                
            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Regions = CityObj;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListByRegionIDD(regionalHeadData.FirstOrDefault().ID);


            return View(objJob);
        }

        public void RetailerInformation(int TID, int RangeID, int cityid, DateTime sdate, DateTime edate)
        {
            DateTime start = sdate;
            DateTime end = edate;
            DateTime final = end.AddDays(1);
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }


            try
            {

                var segment = db.Tbl_Segmenttype.Where(x => x.ID == RangeID).Select(x => x.Name).FirstOrDefault();
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_Total_RetailerInformation1_5_Result> result = objRetailers.RetailerInfos(TID, 0, RangeID, cityid, start, final);

                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SrNO \",\"Shop ID\",\"Created On\",\"Customer Name\",\"Owner Name\",\"Sales Officer Name\",\"Compitators Name\",\"Region Name\",\"City Name\",\"Address\",\"Phone1\",\"Phone2\",\"Customer Type\",\"Segment\",\"Business Type\",\"Business Status\",\"Location\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", $"attachment;filename=Retailers_{segment}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                Response.ContentType = "application/octet-stream";


                //var RegionalHead = db.RegionalHeadRegions.Where(x => x.RegionID == TID).Select(x =>x.RegionHeadID).FirstOrDefault();
                //var name = db.RegionalHeads.Where(x => x.ID == RegionalHead).Select(x => x.Name).FirstOrDefault();

                int srNo = 1;

                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\"",

                            srNo,
                            retailer.ShopID,
                            retailer.CreatedOn,
                         retailer.shopname,
                          retailer.CustomerName,
                        retailer.SaleofficerName,
                          retailer.Compitators,
                        retailer.Region,
                        retailer.City,
                       // retailer.re,
                        retailer.address,
                        retailer.phone1,
                        retailer.phone2,
                        retailer.customerType,
                        retailer.Segment,
                        retailer.BusinessType,

                      retailer.BusinessStatus,
                        

                      retailer.location,

                        srNo++


                        ));
                    }
              

          


                Response.Write(sw.ToString());
                Response.End();



               


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

        }



        public ActionResult ItemSaleSummary()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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


            List<Shared.MainCategories> CityObj = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Regions = CityObj;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListByRegionIDD(regionalHeadData.FirstOrDefault().ID);


            return View(objJob);
        }

        public void ItemSummeryInfo(int TID, int RangeID, int cityid, string sdate, string edate, string ReportType)
        {
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(sdate) ? DateTime.Now.ToString() : sdate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(edate) ? DateTime.Now.ToString() : edate);
            DateTime final = end.AddDays(1);
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }


            try
            {
                if (ReportType == "RegionWise")
                {

                    ManageRetailer objRetailers = new ManageRetailer();
                    List<sp_ItemWisereportRegionWise_Result> result = db.sp_ItemWisereportRegionWise(start,final,TID,RangeID).ToList();

                    try
                    {


                        //ReportParameter[] prm = new ReportParameter[3];

                        //prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        //prm[1] = new ReportParameter("DateTo", edate);
                        //prm[2] = new ReportParameter("DateFrom", sdate);


                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\ItemRegionWise.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                       // ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=ItemsRegonWiseReport" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();

                    }

                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Report Not Working");

                    }

                }
                else
                {
                    ManageRetailer objRetailers = new ManageRetailer();
                    List<sp_ItemWisereportRegionandCityWise_Result> result = db.sp_ItemWisereportRegionandCityWise(start, final, TID, cityid, RangeID).ToList();

                    try
                    {


                        //ReportParameter[] prm = new ReportParameter[3];

                        //prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        //prm[1] = new ReportParameter("DateTo", edate);
                        //prm[2] = new ReportParameter("DateFrom", sdate);


                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\ItemCityWise.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        // ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=ItemsRegonWiseReport" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();

                    }

                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Report Not Working");

                    }
                }


           



                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;

                hst.ReportName = "RetailerInformation";
                hst.ReportType = "RetailerInfo";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

        }


        public ActionResult StockPosition()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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


            List<Shared.MainCategories> CityObj = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Regions = CityObj;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListBySOID(SaleOfficerObj.FirstOrDefault().ID);


            return View(objJob);
        }

        public void StockPositionReport(int TID, int RangeID, int cityid, string sdate, string edate)
        {


            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(sdate) ? DateTime.Now.ToString() : sdate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(edate) ? DateTime.Now.ToString() : edate);
                DateTime final = end.AddDays(1);

                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_StockPositionReport_Result> result = objRetailers.GetStockPosition(start,final,TID,cityid,RangeID);

                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Item Name\",\"Region Name\",\"City Name\",\"Range\",\"Quantity\",\"From Date\",\"To Date\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=StockPosition" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";


           


                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",


                    // retailer.Name,
                    retailer.ItemName,
                    retailer.RegionName,
                    retailer.CityName,
                    retailer.RangeName,
                  retailer.Quantity,
                    retailer.Start,
                    retailer.EndDate



                    ));
                }
                Response.Write(sw.ToString());
                Response.End();

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

        }


        public ActionResult StockInvoice()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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


            List<Shared.MainCategories> CityObj = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Regions = CityObj;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListBySOID(SaleOfficerObj.FirstOrDefault().ID);


            return View(objJob);
        }

        public void StockInvoiceReport(int TID, int RangeID, int cityid, string sdate, string edate)
        {


            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(sdate) ? DateTime.Now.ToString() : sdate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(edate) ? DateTime.Now.ToString() : edate);
                DateTime final = end.AddDays(1);

                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_StockInvoiceReport_Result> result = objRetailers.GetStockInvoice(start, final, TID, cityid, RangeID);

                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Item Name\",\"Region Name\",\"City Name\",\"Range\",\"Quantity\",\"From Date\",\"To Date\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=StockPosition" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";





                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",


                    // retailer.Name,
                    retailer.ItemName,
                    retailer.RegionName,
                    retailer.CityName,
                    retailer.RangeName,
                  retailer.Quantity,
                    retailer.Start,
                    retailer.EndDate



                    ));
                }
                Response.Write(sw.ToString());
                Response.End();

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

        }

        public ActionResult ShopVisitSummery()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangeType();
            var rangeid = ranges.FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            regionalHeadData.Insert(0, new RegionalHeadData
            {
                ID = 0,
                Name = "All"
            });

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regionalHeadData.FirstOrDefault().ID, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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

        public ActionResult ShopVisitSummeryy(string StartingDate, string EndingDate, int TID, int fosid, int dealerid, int cityid)
        {
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp__TotalShopVisitSummeryReport_Result> result = objRetailers.ShopVisitSummery(start, end, TID, fosid, dealerid, cityid);
                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol51 = new DataColumn("Teritory Head", typeof(System.String));
                dtNewTable.Columns.Add(dcol51);
                DataColumn dcol5 = new DataColumn("SaleOfficerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol3 = new DataColumn("DealerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol0 = new DataColumn("CItyName", typeof(System.String));
                dtNewTable.Columns.Add(dcol0);
                DataColumn dcol01 = new DataColumn("Zone", typeof(System.String));
                dtNewTable.Columns.Add(dcol01);
                DataColumn dcol2 = new DataColumn("TotalJobs", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("ShopVisited", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("ShopMissed", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("Order1kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol6);
                DataColumn dcol65 = new DataColumn("Order5kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol65);
                DataColumn dcol66 = new DataColumn("TotalOrders", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol66);
                DataColumn dcol67 = new DataColumn("Delievered1kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol67);
                DataColumn dcol68 = new DataColumn("Delievered5kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol68);
                DataColumn dcol69 = new DataColumn("TotalDelievered", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol69);


                foreach (var item in result)
                {

                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = item.Name;
                    dtrow[1] = item.SaleOfficerName;
                    dtrow[2] = item.DealerName;
                    dtrow[3] = (item.CityName);
                    dtrow[4] = (item.Zone);
                    dtrow[5] = item.TotalJobs;
                    dtrow[6] = item.Done;
                    dtrow[7] = item.Pending;
                    dtrow[8] = item.Order1KG;
                    dtrow[9] = item.Order5kg;
                    dtrow[10] = item.TotalOrder;
                    dtrow[11] = item.Delevired1Kg;
                    dtrow[12] = item.Delievered5kg;
                    dtrow[13] = item.TotalDelievered;
                    dtNewTable.Rows.Add(dtrow);

                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/ShopVisit_Summary.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "VisitSummary.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("VisitSummary{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }

        }


        public ActionResult DealerInfo()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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

            List<Shared.MainCategories> CityObj = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);

            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Regions = CityObj;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListBySOID(SaleOfficerObj.FirstOrDefault().ID);


            return View(objJob);
        }

        public void DealerInformation(int TID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }

            try
            {

                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_DealerInformationSummery1_1_Result> result = objRetailers.DealerInfo(TID, 0, 0,6);

                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SR NO\",\"Distributor ID\",\"Region\",\"City\",\"Distributor Name\",\"Owner Name\",\"Address\",\"Phone1\",\"Phone2\",\"ActiveInactive\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=Distributors" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                //   var retailers = ManageRetailer.GetRetailersForExportinExcel();
                int srNo = 1;
                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"",
                        srNo,
                    retailer.DistID,
                    // retailer.Name,
                    retailer.Region,
                    retailer.City,
                    retailer.ShopSchoolName,
                    retailer.PrincipleName,
                    retailer.Address,
                    retailer.Contact1,
                    retailer.Contact2,
                    retailer.Active,
                    
                   
                    srNo++

                    ));
                }
                Response.Write(sw.ToString());
                Response.End();

                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;
                hst.ReportName = "DealerInformation";
                hst.ReportType = "DealerInfo";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

        }








        public ActionResult ShopVisitSummeryOneLine()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Range = ranges;
            return View(objJob);
        }

        public void ShopVisitSummeryOneLiner(string StartingDate, string EndingDate, int TID, int fosid)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_DealersOrderDetail_Result> result = objRetailers.ShopVisitSummeryOneLine(start, final, TID, fosid,6);
                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"OrderID\",\"Zone\",\"RegionalHead Name\",\"Sales Officer Name\",\"Distributor Name\",\"Item Name\",\"Item Quantity(CTN)\",\"Item Price\",\"City Name\",\"Visited Date\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=DistributorOrders" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                //   var retailers = ManageRetailer.GetRetailersForExportinExcel();

                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"",
                    retailer.OrderID,
                    retailer.Zone,
                    // retailer.Name,
                    retailer.RegionalHeadName,
                    retailer.SaleOfficerName,
                    retailer.DistributorName,
                    retailer.ItemName,
                    retailer.Quantity,
                    retailer.Price,
                    retailer.CityName,
                    retailer.VisitDate

                    // retailer.CustomerType



                    ));
                }
                Response.Write(sw.ToString());
                Response.End();

                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;
                hst.ReportName = "Distributor Order Detail Report";
                hst.ReportType = "DealerOrder";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        public ActionResult Attandance()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.FirstOrDefault();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Range = ranges;
            return View(objJob);
        }

        public void AttandanceReport(string StartingDate, string EndingDate, int TID, int fosid, int RangeID)
        {


            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_AttandaceReport_Result> result = objRetailers.Attandance(start, final, TID, fosid, RangeID);
                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"RegionalHead Name\",\"Saleofficer Name\",\"Region Name\",\"City Name\",\"Type\",\"Date/Time\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=Attandance" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";



                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",

                    retailer.RegionalHead,
                    retailer.Name,
                    // retailer.Name,
                    retailer.Region,
                    retailer.City,
                    retailer.Type,
                    retailer.CreatedAt
              

                    // retailer.CustomerType



                    ));
                }
                Response.Write(sw.ToString());
                Response.End();


                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;
                hst.ReportName = "Attendance Report";
                hst.ReportType = "AttendanceReport";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        public ActionResult ManagersUpdate()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Range = ranges;
            return View(objJob);
        }


        public void ManagersLoginHistory(string StartingDate, string EndingDate, int TID, int RangeID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_ManagersUpdateSummary_Result> result = objRetailers.ManagersSummary(start, final, TID, RangeID);
                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"UserID\",\"UserName\",\"Password\",\"Fetch Date\",\"Report Type\",\"Report Name\",\"Phone No\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=ManagersDailySummary" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";



                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",

                    retailer.userid,
                    retailer.username,
                    // retailer.Name,
                    retailer.Password,
                    retailer.createdon,
                    retailer.rEPORTtYPE,
                    retailer.reportname,
                    retailer.PhoneNo

                    ));
                }
                Response.Write(sw.ToString());
                Response.End();
        
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        public ActionResult SaleOfficerDetail()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Range = ranges;
            return View(objJob);
        }


        public void SaleOfficerDetailRpt(int TID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {
                StringWriter sw = new StringWriter();
                sw.WriteLine("\"Sr No\",\"Name\",\"UserName\",\"Password\",\"Joining Date\",\"Regional Head\",\"Phone1\",\"Phone2\",\"Active\",\"Leave On\",\"SO Code\",\"Region\",\"Primary City\",\"Secondary City\",\"Segment\",\"Designation\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=SODetail_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                Response.ContentType = "application/octet-stream";

                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(
                    @"SELECT
                        so.Name,
                        so.UserName,
                        so.Password,
                        so.JoiningDate,
                        rh.Name          AS RegionalHeadName,
                        so.Phone1,
                        so.Phone2,
                        CASE WHEN so.IsActive = 1 THEN 'Yes' ELSE 'No' END AS Active,
                        so.LeaveOn,
                        so.ECode,
                        reg.Name         AS RegionName,
                        pc.Name          AS PrimaryCity,
                        sc.Name          AS SecondaryCity,
                        seg.Name         AS Segment,
                        des.Name         AS Designation
                      FROM dbo.SaleOfficers so
                      LEFT JOIN dbo.RegionalHeads rh  ON rh.ID  = so.RegionalHeadID
                      LEFT JOIN dbo.Regions reg        ON reg.ID = so.RegionID
                      LEFT JOIN dbo.Cities pc          ON pc.ID  = so.PrimaryCityID
                      LEFT JOIN dbo.Cities sc          ON sc.ID  = so.SecondaryCityID
                      LEFT JOIN dbo.Tbl_Segmenttype seg ON seg.ID = so.SegmentID
                      LEFT JOIN dbo.SODesignations des ON des.ID = so.DesignationID
                      WHERE so.IsDeleted = 0
                        AND (@TID = 0 OR so.RegionalHeadID = @TID)
                      ORDER BY rh.Name, so.Name", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@TID", SqlDbType.Int) { Value = TID });
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        int srNo = 1;
                        bool any = false;
                        while (reader.Read())
                        {
                            any = true;
                            string joiningDate = reader["JoiningDate"] == DBNull.Value ? "" :
                                Convert.ToDateTime(reader["JoiningDate"]).ToString("dd-MM-yyyy");
                            string leaveOn = reader["LeaveOn"] == DBNull.Value ? "" :
                                Convert.ToDateTime(reader["LeaveOn"]).ToString("dd-MM-yyyy");

                            sw.WriteLine(string.Format(
                                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\"",
                                srNo++,
                                reader["Name"],
                                reader["UserName"],
                                reader["Password"],
                                joiningDate,
                                reader["RegionalHeadName"],
                                reader["Phone1"],
                                reader["Phone2"],
                                reader["Active"],
                                leaveOn,
                                reader["ECode"],
                                reader["RegionName"],
                                reader["PrimaryCity"],
                                reader["SecondaryCity"],
                                reader["Segment"],
                                reader["Designation"]
                            ));
                        }
                        if (!any)
                            sw.WriteLine("\"No records found\"");
                    }
                }

                Response.Write(sw.ToString());

                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;
                hst.ReportName = "SO Detail Report";
                hst.ReportType = "SO Details";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();

                Response.End();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
            }
        }

        public ActionResult ShopsPerformance()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void ShopsPerformanceRpt(string StartingDate, string EndingDate, int TID, int fosid, int cityId, int areaId)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();

              



                var lists = db.sp_ShopProductivity(start, final, TID, fosid).ToList();

           


                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Region Name\",\"City Name\",\"Area Name\",\"SS Name\",\"ShopID\",\"Shop Name\",\"Contact No\",\"Distributor Name\",\"Total Visits\",\"Productive\",\"Followup\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=ShopsPerformanceReport" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";


                foreach (var retailer in lists)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",

                    retailer.Region,
                    retailer.CityName,
                    retailer.AreaName,
                    retailer.SaleOfficerName,
                    retailer.ShopID,
                    retailer.ShopName,
                    retailer.Phone1,
                    retailer.Dealer,
                    retailer.TotalVisits,
                    retailer.ProductiveOrders,
                    retailer.NonProductive

                   


                    ));
                }

                Response.Write(sw.ToString());
                Response.End();
            
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        public ActionResult SSSaleSummary()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }


        public void SSSaleSummaryRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();

                List<SaleOfficerCountFinal> list = new List<SaleOfficerCountFinal>();
                decimal? Val = 0;
                SaleOfficerCountFinal comlist;
                var result = new List<SaleOfficerCount>();
                    using (var context = new FOSDataModel())
                    {
                        var query = context.Database.SqlQuery<SaleOfficerCount>("EXEC GetSaleOfficerCountsFinal @datefrom, @dateto, @RHID, @SOID",
                            new SqlParameter("@datefrom", start),
                            new SqlParameter("@dateto", final),
                            new SqlParameter("@RHID", TID),
                            new SqlParameter("@SOID", fosid));

                        result = query.ToList();
                    }


                if (result != null)
                {
                    foreach (var items in result)
                    {
                        var soid = db.SaleOfficers.Where(x => x.Name == items.Name).Select(x => x.ID).FirstOrDefault();
                        comlist = new SaleOfficerCountFinal();

                        var tradecount = db.Retailers.Where(x => x.SaleOfficerID == soid && x.SegmentTypeID == 1 && x.Status==true).Select(x => x.ID).Count();
                        var Housingcount = db.Retailers.Where(x => x.SaleOfficerID == soid && x.SegmentTypeID == 2 && x.Status == true).Select(x => x.ID).Count();
                        var Corporatecount = db.Retailers.Where(x => x.SaleOfficerID == soid && x.SegmentTypeID == 3 && x.Status == true).Select(x => x.ID).Count();
                        var AllPurposecount = db.Retailers.Where(x => x.SaleOfficerID == soid && x.SegmentTypeID == 5 && x.Status == true).Select(x => x.ID).Count();

                        comlist.Name = items.Name;

                        comlist.Trade = tradecount;
                        comlist.Housing = Housingcount;

                        comlist.Corporate = Corporatecount;
                        comlist.AllPurposeCount = AllPurposecount;
                        comlist.TradeVisitsCount = items.TradeVisitsCount;
                        comlist.HousingVisitsCount = items.HousingVisitsCount;
                        comlist.CorporateVisitsCount = items.CorporateVisitsCount;
                        comlist.AllPurpose = items.AllPurposeVisitsCount;
                        comlist.TotalVisits = items.TradeVisitsCount + items.HousingVisitsCount + items.CorporateVisitsCount + items.AllPurposeVisitsCount;
                        list.Add(comlist);
                    
                    }



                }
                
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Saleofficer Name\",\"Trade \",\"Housing \",\"Corporate \",\"AllPurpose \",\"Trade Visits \",\"Housing Visits \",\"Corporate Visits \",\"All Purpose Visits \",\"Total Visits \"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=SSSaleSummaryReport" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";


                foreach (var retailer in list)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"",

                    retailer.Name,

                     retailer.Trade ,
                    retailer.Housing ,
                    retailer.Corporate ,
                     retailer.AllPurposeCount,

                    retailer.TradeVisitsCount ?? 0,
                    retailer.HousingVisitsCount ?? 0,
                    retailer.CorporateVisitsCount ?? 0,
                     retailer.AllPurpose ?? 0,
                       retailer.TotalVisits ?? 0




                    ));
                }

                Response.Write(sw.ToString());
                Response.End();
           
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }


        public ActionResult SSDailyPerformance()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void SSDailyPerformanceRpt(string StartingDate, string EndingDate, int TID, int fosid, int cityId, int areaId)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();

                List<KPIData> list = new List<KPIData>();
                decimal? Val = 0;
                KPIData comlist;



                var lists = db.sp_GetKPISummarySOWise(start, final, TID, fosid, cityId).ToList();

                if (lists != null)
                {
                    foreach (var items in lists)
                    {
                        DateTime start1 = Convert.ToDateTime(items.DateofOrders);
                        //DateTime end1 = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                        DateTime final1 = start1.AddDays(1);
                        comlist = new KPIData();

                        comlist.RHName = items.RegionName;
                        comlist.RegionName = items.Region;
                        comlist.CityName = items.CityName;
                        comlist.SoName = items.SaleOfficerName;
                        comlist.totalVisits = items.TotalVisits;

                        comlist.ProductiveShops = (int)items.ProductiveOrders;
                        comlist.NonProductive = (int)items.NonProductive;

                        comlist.StartDate = items.MarketStart;
                        comlist.EndDate = items.MarketClose;

                        comlist.DateOFOrder = items.DateofOrders;


                        



                        TimeSpan? diff = comlist.EndDate - comlist.StartDate;

                        comlist.ElapseTime = string.Format("{0:%h} hours, {0:%m} minutes, {0:%s} seconds", diff);

                        var total = db.sp_BrandAndItemWiseReport(start1, final1, 0, items.ID, 0, 0, 6).ToList();

                        foreach (var itemss in total)
                        {
                            Val += itemss.TotalQuantity;
                        }
                        comlist.totalSale = Val;
                        list.Add(comlist);
                        Val = 0;
                    }



                }


                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Region\",\"City\",\"SS Name\",\"Visit Date\",\"Total Vists\",\"Productive\",\"Followups\",\"Market Start\",\"Market Close\",\"Elapse Time\",\"Total Cartons\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=SSDailyPerformanceReport" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";


                foreach (var retailer in list)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",

                    retailer.RegionName,
                    retailer.CityName,
                    // retailer.Name,
                    retailer.SoName,
                    retailer.DateOFOrder,
                    retailer.totalVisits,
                    retailer.ProductiveShops,
                    retailer.NonProductive,

                    retailer.StartDate,
                    retailer.EndDate,
                    retailer.ElapseTime,

                    retailer.totalSale


                    ));
                }

                Response.Write(sw.ToString());
                Response.End();
                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;
                hst.ReportName = "Retailer Order Detail Report";
                hst.ReportType = "RetailerOrder";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }


        public ActionResult SOPayments()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void SOPaymentsRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();

               



                List<Sp_PaymentsInformation_Result> result  = db.Sp_PaymentsInformation(TID, fosid, start, final).ToList();



                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Region\",\"City\",\"SS Name\",\"Visit Date\",\"ShopName\",\"Address\",\"Phone\",\"Payment Mode\",\"Amount\",\"Remarks\",\"Image\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=SOPaymentsInfoReport" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                if (result != null)
                {

                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",

                        retailer.Region,
                        retailer.City,
                        // retailer.Name,
                        retailer.SOName,
                        retailer.CreatedAt,
                        retailer.shopname,
                        retailer.address,
                        retailer.phone1,

                        retailer.PaymentMode,
                        retailer.Amount,
                        retailer.Remarks,
                    retailer.ImageURL


                        ));
                    }


                }


              


             

                Response.Write(sw.ToString());
                Response.End();
               
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }


        #region Business Affiliate Report

        public ActionResult BusinessAffiliateReport()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
            });

            if (SaleOfficerObj == null)
            {
                return View();
            }

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Range = ranges;
            return View(objJob);
        }

        public void BusinessAffiliateReportRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);

                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Created Date\",\"Visit Date\",\"Regional Head\",\"SO Name\",\"Zone\",\"City\",\"Business Name\",\"Contact Person\",\"Contact Number\",\"Address\",\"Business Type\",\"Expertise\",\"Classification\",\"Nature of Client\",\"Competitors\",\"Affiliate Remarks\",\"Customer LatLong\",\"Purpose of Visit\",\"Target Agreement\",\"Visit Remarks\"");

                using (FOSDataModel dbContext = new FOSDataModel())
                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand("dbo.usp_GetBusinessAffiliateReport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DateFrom", start);
                    cmd.Parameters.AddWithValue("@DateTo", final);
                    cmd.Parameters.AddWithValue("@SOID", fosid);
                    cmd.Parameters.AddWithValue("@RegionalHeadID", TID);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\"",
                            EscapeCsvValue(GetSafeString(reader, "CreatedDate")),
                            EscapeCsvValue(GetSafeString(reader, "VisitDate")),
                            EscapeCsvValue(GetSafeString(reader, "RegionalHead")),
                            EscapeCsvValue(GetSafeString(reader, "SOName")),
                            EscapeCsvValue(GetSafeString(reader, "Zone")),
                            EscapeCsvValue(GetSafeString(reader, "City")),
                            EscapeCsvValue(GetSafeString(reader, "BusinessName")),
                            EscapeCsvValue(GetSafeString(reader, "ContactPerson")),
                            EscapeCsvValue(GetSafeString(reader, "ContactNumber")),
                            EscapeCsvValue(GetSafeString(reader, "Address")),
                            EscapeCsvValue(GetSafeString(reader, "BusinessType")),
                            EscapeCsvValue(GetSafeString(reader, "Expertise")),
                            EscapeCsvValue(GetSafeString(reader, "Classification")),
                            EscapeCsvValue(GetSafeString(reader, "NatureOfClient")),
                            EscapeCsvValue(GetSafeString(reader, "Competitors")),
                            EscapeCsvValue(GetSafeString(reader, "AffiliateRemarks")),
                            EscapeCsvValue(GetSafeString(reader, "CustomerLatLong")),
                            EscapeCsvValue(GetSafeString(reader, "PurposeOfVisit")),
                            EscapeCsvValue(GetSafeString(reader, "TargetAgreement")),
                            EscapeCsvValue(GetSafeString(reader, "VisitRemarks"))
                            ));
                        }
                    }
                }

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=BusinessAffiliateReport" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";
                Response.Write(sw.ToString());
                Response.End();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        #endregion


        #region Competitor Report

        public ActionResult CompetitorReport()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
            });

            if (SaleOfficerObj == null)
            {
                return View();
            }

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Range = ranges;
            return View(objJob);
        }

        public void CompetitorReportRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);

                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Submitted Date\",\"Regional Head\",\"SO Name\",\"Zone\",\"City\",\"Activity Type\",\"Competitor Name\",\"Remarks\",\"Video URL\"");

                using (FOSDataModel dbContext = new FOSDataModel())
                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand("dbo.usp_GetCompetitorReport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DateFrom", start);
                    cmd.Parameters.AddWithValue("@DateTo", final);
                    cmd.Parameters.AddWithValue("@SOID", fosid);
                    cmd.Parameters.AddWithValue("@RegionalHeadID", TID);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
                            EscapeCsvValue(GetSafeString(reader, "SubmittedDate")),
                            EscapeCsvValue(GetSafeString(reader, "RegionalHead")),
                            EscapeCsvValue(GetSafeString(reader, "SOName")),
                            EscapeCsvValue(GetSafeString(reader, "Zone")),
                            EscapeCsvValue(GetSafeString(reader, "City")),
                            EscapeCsvValue(GetSafeString(reader, "ActivityType")),
                            EscapeCsvValue(GetSafeString(reader, "CompetitorName")),
                            EscapeCsvValue(GetSafeString(reader, "Remarks")),
                            EscapeCsvValue(GetSafeString(reader, "VideoUrl"))
                            ));
                        }
                    }
                }

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=CompetitorReport" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";
                Response.Write(sw.ToString());
                Response.End();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        #endregion


        #region ClaimSummaryReport

        public ActionResult ClaimSummary()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void ClaimSummaryRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            try
            {
                // Log user activity
                var userID = Convert.ToInt32(Session["UserID"]);
                var ipAddress = Dns.GetHostAddresses(Dns.GetHostName())
                                  .FirstOrDefault()?.ToString() ?? "Unknown";

                // Parse dates with proper null handling
                DateTime start = DateTime.Parse(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = DateTime.Parse(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);

                // Get data from database
                var result = db.usp_GetClaimSummaryReportTraderWiseForBackEnd(TID, fosid, start, final, 0).ToList();

                // Set up CSV response
                Response.ClearContent();
                Response.AddHeader("content-disposition", $"attachment;filename=ClaimSummaryInfoReport_{DateTime.Now:yyyyMMddHHmmss}.csv");
                Response.ContentType = "text/csv";

                // Create CSV writer
                using (var sw = new StringWriter())
                {
                    // Write CSV header
                    sw.WriteLine("\"Sr#\",\"Head Name\",\"SO Name\",\"Region Name\",\"City Name\",\"Claim Date\",\"Submission Date\",\"Customer Name\",\"Trader Name\",\"Total Liters\",\"Incentive value\",\"Total Price\",\"Sales Tax\",\"MRP Value\",\"Discount % Value\"");

                    if (result != null)
                    {
                        int srNo = 1;
                        foreach (var retailer in result)
                        {
                            // Calculate discount percentage
                            string discountPercentage = CalculateDiscountPercentage(retailer.MRPValue, retailer.TotalPrice);

                            // Write CSV line using string interpolation for better readability
                            sw.WriteLine($"\"{srNo}\"," +
                                        $"\"{EscapeCsvValue(retailer.HeadName)}\"," +
                                        $"\"{EscapeCsvValue(retailer.SOName)}\"," +
                                        $"\"{EscapeCsvValue(retailer.RegionName)}\"," +
                                        $"\"{EscapeCsvValue(retailer.CityName)}\"," +
                                        $"\"{retailer.CreatedDate}\"," +
                                        $"\"{retailer.PunchingDate}\"," +
                                        $"\"{EscapeCsvValue(retailer.CustomerName)}\"," +
                                        $"\"{EscapeCsvValue(retailer.TraderName)}\"," +
                                        $"\"{retailer.TotalLiters}\"," +
                                        $"\"{retailer.IncentiveValue}\"," +
                                        $"\"{retailer.TotalPrice}\"," +
                                        $"\"{retailer.SalesTax}\"," +
                                        $"\"{retailer.MRPValue}\"," +
                                        $"\"{discountPercentage}\"");

                            srNo++;
                        }
                    }

                    Response.Write(sw.ToString());
                }

                Response.End();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Error generating Claim Summary Report");
                // Consider returning an error response to the client
                Response.StatusCode = 500;
                Response.Write("Error generating report. Please try again.");
                Response.End();
            }
        }

        private string CalculateDiscountPercentage(string mrpValue, string totalPrice)
        {
            if (string.IsNullOrEmpty(mrpValue) || mrpValue == "0.00")
                return "0.00%";

            try
            {
                decimal mrp = decimal.Parse(mrpValue.Replace(",", ""), CultureInfo.InvariantCulture);
                decimal price = string.IsNullOrEmpty(totalPrice)
                    ? 0
                    : decimal.Parse(totalPrice.Replace(",", ""), CultureInfo.InvariantCulture);

                if (mrp == 0)
                    return "0.00%";

                decimal discount = ((mrp - price) / mrp) * 100;
                discount = Math.Max(0, discount); // Ensure non-negative

                return discount.ToString("N2", CultureInfo.InvariantCulture) + "%";
            }
            catch
            {
                return "0.00%";
            }
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("\"", "\"\""); // Escape double quotes in CSV
        }
        #endregion


        #region ClaimSummarySOWiseReport

        public ActionResult ClaimSummarySOWise()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void ClaimSummarySOWiseRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            try
            {
                // Log user activity
                var userID = Convert.ToInt32(Session["UserID"]);
                var ipAddress = Dns.GetHostAddresses(Dns.GetHostName())
                                  .FirstOrDefault()?.ToString() ?? "Unknown";

                // Parse dates with proper null handling
                DateTime start = DateTime.Parse(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = DateTime.Parse(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);

                // Get data from database
                var result = db.usp_GetClaimSummaryReportSOWise(TID, fosid, start, final).ToList();

                // Set up CSV response
                Response.ClearContent();
                Response.AddHeader("content-disposition", $"attachment;filename=ClaimSummarySOWiseReport_{DateTime.Now:yyyyMMddHHmmss}.csv");
                Response.ContentType = "text/csv";

                // Create CSV writer
                using (var sw = new StringWriter())
                {
                    // Write CSV header
                    sw.WriteLine("\"Sr#\",\"Head Name\",\"SO Name\",\"Region Name\",\"Total Sale Value\",\"Total Liters\",\"Total Target Liters\",\"Target Balance\",\"Incentive value\",\"Total Price\",\"Sales Tax\",\"MRP Value\",\"Discount % Value\"");

                    if (result != null)
                    {
                        int srNo = 1;
                        foreach (var retailer in result)
                        {
                           


                            // Write CSV line using string interpolation for better readability
                            sw.WriteLine($"\"{srNo}\"," +
                                        $"\"{EscapeCsvValue(retailer.Head_Name)}\"," +
                                        $"\"{EscapeCsvValue(retailer.SO_Name)}\"," +
                                        $"\"{EscapeCsvValue(retailer.Region_Name)}\"," +
                                       $"\"{retailer.SaleValue}\"," +
                                        $"\"{retailer.Total_Liters}\"," +
                                        $"\"{retailer.Target_Liters}\"," +
                                        $"\"{retailer.Target_Balance}\"," +
                                        $"\"{retailer.IncentiveValue}\"," +
                                        $"\"{retailer.TotalPrice}\"," +
                                        $"\"{retailer.SalesTax}\"," +
                                        $"\"{retailer.MRPValue}\"," +
                                        
                                        $"\"{retailer.Disc__age}\"");

                            srNo++;
                        }
                    }

                    Response.Write(sw.ToString());
                }

                Response.End();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Error generating Claim Summary Report");
                // Consider returning an error response to the client
                Response.StatusCode = 500;
                Response.Write("Error generating report. Please try again.");
                Response.End();
            }
        }




        #endregion


        #region ChemicalSummarySOWiseReport

        public ActionResult ChemicalSummarySOWise()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void ChemicalSummarySOWiseRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            try
            {
                // Log user activity
                var userID = Convert.ToInt32(Session["UserID"]);
                var ipAddress = Dns.GetHostAddresses(Dns.GetHostName())
                                  .FirstOrDefault()?.ToString() ?? "Unknown";

                // Parse dates with proper null handling
                DateTime start = DateTime.Parse(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = DateTime.Parse(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);

                // Get data from database
                var result = db.usp_GetDSRForChemicalBackend(start, final, TID, fosid).ToList();

                // Set up CSV response
                Response.ClearContent();
                Response.AddHeader("content-disposition", $"attachment;filename=Sales Summary Report(Brigto Chemicals)_{DateTime.Now:yyyyMMddHHmmss}.csv");
                Response.ContentType = "text/csv";

                // Create CSV writer
                using (var sw = new StringWriter())
                {
                    // Write CSV header
                    sw.WriteLine("\"Sr#\",\"Order Number\",\"Order Date\",\"SO Name\",\"Customer Name\",\"Product Category\",\"Product Name\",\"UOM Value\",\"UOM Name\",\"Quantity\",\"Cost\",\"Sales Value\"");

                    if (result != null)
                    {
                        int srNo = 1;
                        foreach (var retailer in result)
                        {



                            // Write CSV line using string interpolation for better readability
                            sw.WriteLine($"\"{srNo}\"," +
                                        $"\"{retailer.OrderNumber}\"," +
                                        $"\"{retailer.OrderDate}\"," +
                                        $"\"{EscapeCsvValue(retailer.SaleOfficerName)}\"," +
                                        $"\"{EscapeCsvValue(retailer.CustomerName)}\"," +
                                       $"\"{retailer.ProductCategory}\"," +
                                        $"\"{retailer.ProductName}\"," +
                                        $"\"{retailer.UOMValue}\"," +
                                        $"\"{retailer.UomName}\"," +
                                        $"\"{retailer.Quantity}\"," +
                                        $"\"{retailer.Cost}\"," +
                                        $"\"{retailer.SaleValue}\"");

                            srNo++;
                        }
                    }

                    Response.Write(sw.ToString());
                }

                Response.End();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Error generating Claim Summary Report");
                // Consider returning an error response to the client
                Response.StatusCode = 500;
                Response.Write("Error generating report. Please try again.");
                Response.End();
            }
        }




        #endregion

        #region CoverageSummarySOWiseReport

        public ActionResult CoverageSummarySOWise()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void CoverageSummarySOWiseRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            try
            {
                // Log user activity
                var userID = Convert.ToInt32(Session["UserID"]);
                var ipAddress = Dns.GetHostAddresses(Dns.GetHostName())
                                  .FirstOrDefault()?.ToString() ?? "Unknown";

                // Parse dates with proper null handling
                DateTime start = DateTime.Parse(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = DateTime.Parse(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);

                // Get data from database
                var result = db.usp_GetCoverageReportForBackEnd(TID, fosid, start, final).ToList();

                // Set up CSV response
                Response.ClearContent();
                Response.AddHeader("content-disposition", $"attachment;filename=Coverage Summary SOWise Rpt_{DateTime.Now:yyyyMMddHHmmss}.csv");
                Response.ContentType = "text/csv";

                // Create CSV writer
                using (var sw = new StringWriter())
                {
                    // Write CSV header
                    sw.WriteLine("\"Sr#\",\"Date\",\"SO Name\",\"Customer Name\",\"Region\",\"Area In Sqft\",\"Area In Meters\",\"Category\",\"Product Name\",\"No Of Coats\",\"DRUM\",\"Gallon\",\"Quarter\",\"Drum Pkr\",\"Gallon Pkr\",\"Quarter Pkr\",\"Discount %\",\"MRP Value\",\"Discounted Value\"");

                    if (result != null)
                    {
                        int srNo = 1;
                        foreach (var retailer in result)
                        {



                            // Write CSV line using string interpolation for better readability
                            sw.WriteLine($"\"{srNo}\"," +
                                        $"\"{retailer.CreatedDate}\"," +
                                        $"\"{retailer.SOName}\"," +
                                        $"\"{EscapeCsvValue(retailer.CustomerName)}\"," +
                                        $"\"{EscapeCsvValue(retailer.RegionName)}\"," +
                                       $"\"{retailer.AreaInSqft}\"," +
                                        $"\"{retailer.AreaInMeters}\"," +
                                        $"\"{retailer.Category}\"," +
                                        $"\"{retailer.ProductName}\"," +
                                        $"\"{retailer.NoOfCoats}\"," +
                                        $"\"{retailer.Drum}\"," +
                                        $"\"{retailer.Gallon}\"," +
                                        $"\"{retailer.Quarter}\"," +
                                        $"\"{retailer.DrumPrice}\"," +
                                        $"\"{retailer.GallonPrice}\"," +
                                        $"\"{retailer.QtrPrice}\"," +
                                        $"\"{retailer.DiscountPercent}\"," +
                                        $"\"{retailer.MRPValue}\"," +
                                    
                                        $"\"{retailer.DiscountAmount}\"");

                            srNo++;
                        }
                    }

                    Response.Write(sw.ToString());
                }

                Response.End();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Error generating Claim Summary Report");
                // Consider returning an error response to the client
                Response.StatusCode = 500;
                Response.Write("Error generating report. Please try again.");
                Response.End();
            }
        }




        #endregion



        #region ClaimDetailReport

        public ActionResult ClaimDetail()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void ClaimDetailRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();





                List<usp_GetClaimSummaryReportForBackEnd_Result> result = db.usp_GetClaimSummaryReportForBackEnd(TID, fosid, start, final).ToList();



                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Sr#\",\"Head Name\",\"SO Name\",\"Region Name\",\"City Name\",\"Date Of Submission\",\"Claim Date\",\"Customer Name\",\"Trader Name\",\"Brand Name\",\"Product Name\",\"Drum\",\"Gallon\",\"Quarter\",\"Total Liters\",\"Incentive Category\",\"Incentive Pkr\",\"Incentive Value\",\"Sale Value\",\"Total Value\",\"Sales Tax Value\",\"MRP Value\",\"Discount Percentage Value\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=ClaimDetailInfoReport" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";
                int srNo = 1;
                if (result != null)
                {

                    foreach (var retailer in result)

                    {
                        decimal? discountPercentage = 0;
                        if (!string.IsNullOrEmpty(retailer.MRPValue) && retailer.MRPValue != "0.00")
                        {
                            try
                            {
                                // Remove thousands separators if present and parse
                                decimal mrpValue = decimal.Parse(retailer.MRPValue.Replace(",", ""), CultureInfo.InvariantCulture);
                                decimal saleValue = retailer.SaleValue ?? 0; // Handle null SaleValue

                                if (mrpValue != 0)
                                {
                                    discountPercentage = ((mrpValue - saleValue) / mrpValue) * 100;
                                }
                                else
                                {
                                    discountPercentage = 0;
                                }
                            }
                            catch (FormatException)
                            {
                                // Log error if needed
                                discountPercentage = 0;
                            }
                            catch (OverflowException)
                            {
                                // Log error if needed
                                discountPercentage = 0;
                            }
                        }

                        string discountPercentageFormatted = discountPercentage?.ToString("N2", CultureInfo.InvariantCulture) + "%";
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\"",
                         srNo,
                        retailer.HeadName,
                        retailer.SOName,
                        // retailer.Name,
                        retailer.RegionName,
                        retailer.CityName,
                        retailer.PunchingDate,
                        retailer.CreatedDate,
                        retailer.CustomerName,
                        retailer.TraderName,
                        retailer.BrandName,
                        retailer.ProductName,
                         retailer.Drum,
                        retailer.Gallon,
                        retailer.QuarterQty,
                        retailer.TotalLiters,
                         retailer.Incentivecategory,
                          retailer.IncentivePkr,
                        retailer.IncentiveValue,
                          retailer.SaleValue,
                        retailer.TotalPrice,
                        retailer.SalesTax,
                          retailer.MRPValue,
                        discountPercentageFormatted,
                        srNo++


                        ));
                    }


                }







                Response.Write(sw.ToString());
                Response.End();

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }


        #endregion


        #region ClaimSummaryVSKPISOWiseReport

        public ActionResult ClaimSummaryVSKPISOWise()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void ClaimSummaryVSKPISOWiseRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
        
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }

            FOSDataModel db = new FOSDataModel();

            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            DateTime final = end.AddDays(1);
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();


            try
            {

                List<usp_GetClaimVsKPIReportSOWise_Result> result = db.usp_GetClaimVsKPIReportSOWise(TID, fosid, start, final).ToList();


              


                ReportParameter[] prm = new ReportParameter[3];

                prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
             
                prm[1] = new ReportParameter("DateTo", EndingDate);
                prm[2] = new ReportParameter("DateFrom", StartingDate);

                ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\ClaimsVsKpiReport.rdlc");
                ReportViewer1.EnableExternalImages = true;
                ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
                ReportViewer1.SetParameters(prm);
                ReportViewer1.DataSources.Clear();
                ReportViewer1.DataSources.Add(dt1);
                ReportViewer1.Refresh();



                Warning[] warnings;
                string[] streamIds;
                string contentType;
                string encoding;
                string extension;

                //Export the RDLC Report to Byte Array.
                byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                //Download the RDLC Report in Word, Excel, PDF and Image formats.
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;filename=IncentiveClaimSOWISE" + DateTime.Now + ".Pdf");
                Response.BinaryWrite(bytes);
                Response.Flush();

                Response.End();

            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

          


        
        }




        #endregion


        #region ClaimApprovalReport

        public ActionResult ClaimApproval()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void ClaimApprovalRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();





                List<usp_GetClaimApprovalReportForBackEnd_Result> result = db.usp_GetClaimApprovalReportForBackEnd(TID, fosid, start, final).ToList();



                StringWriter sw = new StringWriter();

                sw.WriteLine("\"Sr#\",\"Head Name\",\"SO Name\",\"Region Name\",\"City Name\",\"Date Of Submission\",\"Claim Date\",\"Customer Name\",\"Trader Name\",\"Total Liters\",\"Sale Value\",\"HO Approval Status\",\"HO Approval Remarks\",\"Approval Date\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=ClaimApprovalReport" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";
                int srNo = 1;
                if (result != null)
                {

                    foreach (var retailer in result)

                    {
                        

                        //string discountPercentageFormatted = discountPercentage?.ToString("N2", CultureInfo.InvariantCulture) + "%";
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\"",
                         srNo,
                        retailer.HeadName,
                        retailer.SOName,
                        // retailer.Name,
                        retailer.RegionName,
                        retailer.CityName,
                        retailer.PunchingDate,
                        retailer.CreatedDate,
                        retailer.CustomerName,
                        retailer.TraderName,
                      
                        retailer.TotalLiters,
                        
                          retailer.SaleValue,
                        retailer.ClaimManagerLatestStatus,
                        retailer.ClaimManagerLatestRemarks,
                          retailer.ClaimManagerDate,
                      
                        srNo++


                        ));
                    }


                }







                Response.Write(sw.ToString());
                Response.End();

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }


        #endregion



        public ActionResult SOClaimsSummary()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void SOClaimsSummaryRpt(string StartingDate, string EndingDate, int TID, int fosid)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);

                List<usp_GetClaimBrandWiseReport_Result> result = db.usp_GetClaimBrandWiseReport(TID, fosid, start, final).ToList();


               
               


                ReportParameter[] prm = new ReportParameter[3];

                prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
               
                prm[1] = new ReportParameter("DateTo", EndingDate);
                prm[2] = new ReportParameter("DateFrom", StartingDate);

                ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\BrandClaims.rdlc");
                ReportViewer1.EnableExternalImages = true;
                ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
                ReportViewer1.SetParameters(prm);
                ReportViewer1.DataSources.Clear();
                ReportViewer1.DataSources.Add(dt1);
                ReportViewer1.Refresh();



                Warning[] warnings;
                string[] streamIds;
                string contentType;
                string encoding;
                string extension;

                //Export the RDLC Report to Byte Array.
                byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                //Download the RDLC Report in Word, Excel, PDF and Image formats.
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;filename=ClaimSummery" + DateTime.Now + ".Pdf");
                Response.BinaryWrite(bytes);
                Response.Flush();

                Response.End();

            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

        }




        public ActionResult RetailerOrdersDetail()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges= FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
           var  rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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

            List<Shared.MainCategories> CityObj = db.Tbl_Segmenttype
                          .Select(
                              u => new Shared.MainCategories
                              {
                                  ID = u.ID,
                                  MainCategoryName = u.Name,
                              }).ToList();

            //List<VisitPlanData> visitData = new List<VisitPlanData>();
            //visitData = FOS.Setup.ManageJobs.GetAllVisitList();

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            //objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            //objJob.VisitPlan = visitData;
            objJob.Regions = CityObj;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListBySOID(SaleOfficerObj.FirstOrDefault().ID);
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Retailers = ManageRetailer.GetRetailerBySOIDAndCity(SaleOfficerObj.Select(s => s.ID).FirstOrDefault(), objJob.Cities.FirstOrDefault().ID, objJob.Areas.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }
        public JsonResult GetRetailersBySOIDAndCity(int soId, int cityId, int areaId)
        {
            var result = FOS.Setup.ManageRetailer.GetRetailerBySOIDAndCity(soId, cityId, areaId);
            return Json(result);
        }
        public JsonResult GetAllAreaListByCityID(int ID)
        {
            var result = FOS.Setup.ManageArea.GetAllAreaListByCityID(ID);
            return Json(result);
        }
        public void RetailerOrders(string StartingDate, string EndingDate, int TID, int fosid, int segment)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }

            if (segment == 1)
            {

                try
                {



                    DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                    DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                    DateTime final = end.AddDays(1);
                    ManageRetailer objRetailers = new ManageRetailer();
                    List<Sp_OrderForPDFTradeVisitBackEnd_Result> result = db.Sp_OrderForPDFTradeVisitBackEnd(start, final, fosid,TID).ToList();
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"VisitID\",\"Customer Name\",\"City Name\",\"Head Name\",\"Created On\",\"Next Visit Date\",\"SaleOfficer Name\",\"Order Value\",\"Order Volume\",\"Visit Type\",\"Remarks\",\"VisitType Remarks\",\"Location\",\"OnlineOROffline\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=TradeVisits" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";


                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\"",

                        retailer.VisitID,
                        
                        retailer.CustomerName,
                        retailer.CityName,
                         retailer.RegionalHead,
                         retailer.CreatedAt,
                        
                          retailer.NextVisitDate.HasValue
        ? retailer.NextVisitDate.Value.ToString("yyyy-MM-dd")  // If DateOfAttendance has a value, format it
        : "",
                        retailer.Name,
                        retailer.OrderValue,
                        retailer.OrderVolume,

                       retailer.VisitType,
                        
                       retailer.Remarks,
                        retailer.VisitTypeRemarks,
                        retailer.Latitude +","+ retailer.longitude,
                        retailer.OnlineOrOffline




                        ));
                    }

                    Response.Write(sw.ToString());
                    Response.End();

                }
                catch (Exception exp)
                {
                    Log.Instance.Error(exp, "Report Not Working");
                    // return null;
                }

            }


            if (segment == 2)
            {

                try
                {



                    DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                    DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                    DateTime final = end.AddDays(1);
                    ManageRetailer objRetailers = new ManageRetailer();
                    List<Sp_OrderForPDFHousingVisitsBackend_Result> result = db.Sp_OrderForPDFHousingVisitsBackend(start, final, fosid,TID).ToList();
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"VisitID\",\"CustomerID\",\"Owner Name\",\"Customer Name\",\"Customer CreatedDate\",\"Compititors\",\"Business Status\",\"Regional Head Name\",\"Visit Date\",\"Next Visit Date\",\"SaleOfficer Name\",\"Address\",\"Mobile No\",\"Architect Name\",\"Architect Number\",\"Contractor Name\",\"Contractor Number\",\"Estimated Value\",\"Order Volume\",\"Off take From Others\",\"Scope\",\"Nature\",\"Construction Stage\",\"Plot Size\",\"Color Scheme\",\"Site Won\",\"Remarks\",\"Location\",\"Online OR Offline\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=HousingVisits" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";
                      

                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\",\"{26}\",\"{27}\",\"{28}\"",

                        retailer.VisitID,
                        retailer.CustomerID,
                        retailer.OwnerName,
                        retailer.CustomerName,
                        retailer.CreatedOn,
                        retailer.Compititors,
                          retailer.BusinessStatus,
                         retailer.RegionalHead,
                          retailer.CreatedAt,
                                  retailer.NextVisitDate.ToString("yyyy-MM-dd")  // If DateOfAttendance has a value, format it
        ,
                        retailer.SaleOfficerName,
                        retailer.Address,
                       retailer.Phone1,
                       
                        retailer.ArchitectName,

                              retailer.ArchitectNumber,
                        retailer.ContractorName,
                       retailer.ContractorNumber,
                        retailer.EstimatedValue,
                         retailer.OrderVolume,

                       retailer.OffTakeFromOthers,
                        retailer.Scope,

                          retailer.Nature,
                        retailer.ConstructionStage,
                       retailer.PlotSize,
                        retailer.ColorScheme,
                        
                       retailer.SiteWon,
                       retailer.Remarks,
                       retailer.Latitude + "," + retailer.Longitude,
                       retailer.OnlineOffline



                        ));
                    }

                    Response.Write(sw.ToString());
                    Response.End();

                }
                catch (Exception exp)
                {
                    Log.Instance.Error(exp, "Report Not Working");
                    // return null;
                }

            }


            if (segment == 3)
            {

                try
                {



                    DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                    DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                    DateTime final = end.AddDays(1);
                    ManageRetailer objRetailers = new ManageRetailer();
                    List<Sp_OrderForPDFCorporateVisitsBackEnd_Result> result = db.Sp_OrderForPDFCorporateVisitsBackEnd(start, final, fosid,TID).ToList();
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"VisitID\",\"Customer Name\",\"Head Name\",\"Visit Date\",\"Next Visit Date\",\"SaleOfficer Name\",\"ProjectTitle\",\"Industry\",\"OEM Name\",\"Project Estimated Value\",\"Order Volume\",\"Influencer Name\",\"Influencer Number\",\"Architect Name\",\"Architect Number\",\"Builder Name\",\"Builder Number\",\"Paint Contractor Name\",\"Paint Contractor Number\",\"Procurement Manager\",\"Procurement Manager No\",\"Accounts Manager\",\"Accounts Manager No\",\"Compitator Name\",\"Compitator No\",\"Remarks\",\"Location\",\"Online OR Offline\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=HousingVisits" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";


                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\",\"{26}\",\"{27}\"",

                        retailer.ID,
                        retailer.Customer,
                        retailer.RegionalHead,
                         retailer.CreatedAt,
                                        retailer.NextVisitDate.HasValue
        ? retailer.NextVisitDate.Value.ToString("yyyy-MM-dd")  // If DateOfAttendance has a value, format it
        : "",
                        retailer.SaleOfficer,
                        retailer.ProjectTitle,
                       retailer.Industry,
                       
                       retailer.OEMName,
                        retailer.ProjectEstimatedValue,
                         retailer.OrderVolume,
                              retailer.InfluencerName,
                        retailer.InfluencerNumber,
                       retailer.ArchitectName,
                        retailer.ArchitectNumber,
                       retailer.BuilderName,
                        retailer.BuilderNumber,

                          retailer.PaintContractorName,
                        retailer.PaintContractorNumber,
                       retailer.ProcurementManager,
                        retailer.ProcurementManagerNo,
                       retailer.AccountsManager,

                        retailer.AccountsManagerNo,
                       retailer.CompetitorName,
                        retailer.CompetitorNo,
                       retailer.Remarks,
                       retailer.Latitude + "," + retailer.Longitude,
                         retailer.OnlineOffline
                        ));
                    }

                    Response.Write(sw.ToString());
                    Response.End();

                }
                catch (Exception exp)
                {
                    Log.Instance.Error(exp, "Report Not Working");
                    // return null;
                }

            }


            if (segment == 5)
            {

                try
                {



                    DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                    DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                    DateTime final = end.AddDays(1);
                    ManageRetailer objRetailers = new ManageRetailer();
                    List<Sp_OrderForPDFAllPurposeVisitBackEnd_Result> result = db.Sp_OrderForPDFAllPurposeVisitBackEnd(start, final, fosid, TID).ToList();
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"VisitID\",\"Customer Name\",\"Head Name\",\"Visit Date\",\"Next Visit Date\",\"SaleOfficer Name\",\"Purpose Of Visit\",\"Other Remarks\",\"Remarks\",\"Location\",\"OnlineOROffline\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=AllPurposeVisits" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";


                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",

                        retailer.VisitID,
                        retailer.CustomerName,
                         retailer.RegionalHead,
                            retailer.Createdon,
                                       retailer.NextVisitDate.HasValue
        ? retailer.NextVisitDate.Value.ToString("yyyy-MM-dd")  // If DateOfAttendance has a value, format it
        : "",
                        retailer.Name,
                        retailer.PurposeOfVisit,
                        retailer.OtherRemarks,

                      
                     
                       retailer.Remarks,
                       
                        retailer.Latitude + "," + retailer.longitude,
                        retailer.OnlineOrOffline




                        ));
                    }

                    Response.Write(sw.ToString());
                    Response.End();

                }
                catch (Exception exp)
                {
                    Log.Instance.Error(exp, "Report Not Working");
                    // return null;
                }

            }


        }


        public ActionResult LowItemsDetail()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionData = new List<RegionalHeadData>();
            regionData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            if (userID == 1)
            {
                regionData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.RegionData = regionData;
            objJob.RegionalHead = regionalHeadData;
            //objJob.VisitPlan = visitData;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
            objJob.Cities = ManageCity.GetCityListByRegionIDD(regionData.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }


        public void LowOrdersSOWise(string StartingDate, string EndingDate, int TID, int fosid, string Type, int RegionID, int CityID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }

            if (Type == "SoWise")
            {
                try
                {

                    DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                    DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                    DateTime final = end.AddDays(1);
                    ManageRetailer objRetailers = new ManageRetailer();
                    List<Sp_LessItemsSoldRegionWise_Result> result = db.Sp_LessItemsSoldRegionWise(start, final, 6, TID, fosid).ToList();
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"RegionalHead Name\",\"Sales Officer Name\",\"Item Name\",\"Quantity in PCS\",\"Price\",\"Grand Total\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=LowSalesItemSOWise" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";


                  
                        foreach (var retailer in result)
                        {
                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",


                            // retailer.Name,
                            retailer.RegionalHeadName,
                            retailer.SaleofficerName,
                            
                             retailer.ItemName,
                            retailer.Orders,
                            retailer.Price,
                            retailer.Total


                            ));
                        }
                 

                    Response.Write(sw.ToString());
                    Response.End();
                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Low Item Sale Report SO Wise";
                    hst.ReportType = "Items Report";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
                catch (Exception exp)
                {
                    Log.Instance.Error(exp, "Report Not Working");
                    // return null;
                }
            }
            else
            {
                try
                {

                    DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                    DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                    DateTime final = end.AddDays(1);
                    ManageRetailer objRetailers = new ManageRetailer();
                    List<Sp_LessItemsSoldRegionandCityWise_Result> result = db.Sp_LessItemsSoldRegionandCityWise(start, final, 6, RegionID, CityID).ToList();
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"Region Name\",\"City Name\",\"Item Name\",\"Quantity in PCS\",\"Price\",\"Grand Total\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=LowSalesItemRegionWise" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";


                 
                        foreach (var retailer in result)
                        {
                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",


                            // retailer.Name,
                            retailer.RegionName,
                            retailer.CityName,
                     
                            retailer.ItemName,
                            retailer.Orders,
                            retailer.Price,
                            retailer.Total


                            ));
                        }
                 

                    Response.Write(sw.ToString());
                    Response.End();
                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Low Item Sale Report Region Wise";
                    hst.ReportType = "Items Report";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
                catch (Exception exp)
                {
                    Log.Instance.Error(exp, "Report Not Working");
                    // return null;
                }
            }
        }

        public ActionResult SOAttendance()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Range = ranges;
            return View(objJob);
        }

        public ActionResult StockTakingDetail()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Range = ranges;
            return View(objJob);
        }

        public void Stocktakingorderdetails(string StartingDate, string EndingDate, int TID, int fosid)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }

            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                //  DateTime Startfinal = start.AddDays(-1);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_StockTakingDetailReport_Result> result = objRetailers.Stocktaking(start, final, TID, fosid);
                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"StockID\",\"RegionalHead Name\",\"Sales Officer Name\",\"Distributor Name\",\"Item Name\",\"Quantity (IN CTN)\",\"City Name\",\"Stock Taking Date\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=StockTaking" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                //   var retailers = ManageRetailer.GetRetailersForExportinExcel();

                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",

                    retailer.StockID,

                    retailer.RegionalHeadName,
                    retailer.SaleOfficerName,
                    retailer.DistributorName,
                    retailer.ItemName,
                    retailer.Quantity,
                    retailer.CityName,
                    retailer.StockTakingTime



                    ));
                }
                Response.Write(sw.ToString());
                Response.End();
                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;
                hst.ReportName = "Stock taking Detail Report";
                hst.ReportType = "Stocktaking";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }










        public ActionResult ShopVisitDetail()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            regionalHeadData.Insert(0, new RegionalHeadData
            {
                ID = 0,
                Name = "All"
            });

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regionalHeadData.FirstOrDefault().ID, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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

        public ActionResult ShopVisitDetaill(string StartingDate, string EndingDate, int TID, int fosid, int dealerid, int cityid)
        {
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp__TotalShopVisitDetail_Result> Result = objRetailers.ShopVisitDetail(start, end, TID, fosid, dealerid, cityid);

                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol5 = new DataColumn("SaleOfficerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol3 = new DataColumn("DealerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol0 = new DataColumn("CItyName", typeof(System.String));
                dtNewTable.Columns.Add(dcol0);
                DataColumn dcol2 = new DataColumn("ShopName", typeof(System.String));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("JobDate", typeof(System.String));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("Order1kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("Order5kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol6);
                DataColumn dcol65 = new DataColumn("Delievered1kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol65);
                DataColumn dcol66 = new DataColumn("Delievered5kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol66);



                foreach (var item in Result)
                {

                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = item.SaleOfficerName;
                    dtrow[1] = item.DealerName;
                    dtrow[2] = (item.CityName);
                    dtrow[3] = item.ShopName;
                    dtrow[4] = item.JobDate;
                    dtrow[5] = item.Order1kg;
                    dtrow[6] = item.Order5kg;
                    dtrow[7] = item.Delevired1Kg;
                    dtrow[8] = item.Delievered5kg;

                    dtNewTable.Rows.Add(dtrow);

                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/ShopVisit_Detail.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "ShopVisit_Detail.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("ShopVisit_Detail{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }

        }




        public ActionResult MarketInfo()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            regionalHeadData.Insert(0, new RegionalHeadData
            {
                ID = 0,
                Name = "All"
            });

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regionalHeadData.FirstOrDefault().ID, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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

        public ActionResult PresentSOReport()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            //  var objRetailer = new RetailerData();
            //objRetailer.Regions = FOS.Setup.ManageCity.GetRegionList();

            List<RegionalHeadData> objRegion = new List<RegionalHeadData>();
            IEnumerable<RegionalHeadData> obj = new List<RegionalHeadData>();
            if (userID == 1)
            {
                objRegion.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;


          // regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            obj = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
                objRegion = obj.ToList();
          
            var objRetailer = new RetailerData();
            objRetailer.Regionss = objRegion;
            objRetailer.Range = ranges;
            return View(objRetailer);
        }


        public void TodayPresentSO(string StartingDate, string EndingDate, int TID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();
                List<spGetSalesOfficerWithLoginDate_Result> result = objRetailers.TodayPresentSalesOfficer(TID, start, final);
                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SrNo.\",\"Login Date\",\"Sales Officer Name\",\"Regional Head\",\"City\",\"Type\",\"Market Start\",\"Market Close\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=TodayPresentSO" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                //   var retailers = ManageRetailer.GetRetailersForExportinExcel();
                int srNo = 1;
                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                    srNo,
                     retailer.LoginDate,
                    retailer.SalesOfficer,
                    retailer.RegionalHead,
                    retailer.Name,
                     retailer.TypeofAttendance,
                     retailer.MarketStartLatlong,
                     retailer.MarketCloseLatlong,
                   
                    srNo++


                    ));
                }
                Response.Write(sw.ToString());
                Response.End();
                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;
                hst.ReportName = "Present SO Report";
                hst.ReportType = "PresentSO";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        public ActionResult PresentSOReportSync()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            //  var objRetailer = new RetailerData();
            //objRetailer.Regions = FOS.Setup.ManageCity.GetRegionList();

            List<RegionalHeadData> objRegion = new List<RegionalHeadData>();
            IEnumerable<RegionalHeadData> obj = new List<RegionalHeadData>();
            if (userID == 1)
            {
                objRegion.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;


            // regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            obj = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            objRegion = obj.ToList();

            var objRetailer = new RetailerData();
            objRetailer.Regionss = objRegion;
            objRetailer.Range = ranges;
            return View(objRetailer);
        }


        public void TodayPresentSOSync(string StartingDate, string EndingDate, int TID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();
                List<EmployeeAttendanceTimingData> result = objRetailers.TodayPresentSalesOfficerSync(TID, start, end);
                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SrNo.\",\"Date\",\"Regional Head\",\"Emp_Code\",\"Emp_Name\",\"Start_Time\",\"End_Time\",\"Last_Visit_Time\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=TodayPresentSOSync" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                //   var retailers = ManageRetailer.GetRetailersForExportinExcel();
                int srNo = 1;
                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                    srNo,
                    retailer.AttendanceDate,
                    retailer.RegionalHead,
                    retailer.Emp_Code,
                    retailer.Emp_Name,
                    retailer.Start_Time,
                    retailer.End_Time,
                    retailer.Last_Visit_Time
                    ));
                    srNo++;
                }
                Response.Write(sw.ToString());
                Response.End();
              


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }


        public ActionResult PresentSOReportSummary()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            //  var objRetailer = new RetailerData();
            //objRetailer.Regions = FOS.Setup.ManageCity.GetRegionList();

            List<RegionalHeadData> objRegion = new List<RegionalHeadData>();
            IEnumerable<RegionalHeadData> obj = new List<RegionalHeadData>();
            if (userID == 1)
            {
                objRegion.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;


            // regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            obj = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            objRegion = obj.ToList();

            var objRetailer = new RetailerData();
            objRetailer.Regionss = objRegion;
            objRetailer.Range = ranges;
            return View(objRetailer);
        }


        public void TodayPresentSOSummary(string StartingDate, string EndingDate, int TID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();
                List<spGetSalesOfficerAttendanceSummary_Result> result = db.spGetSalesOfficerAttendanceSummary(TID,start,final).ToList();
                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SrNo.\",\"Regional Head\",\"Date\",\"Total SOS\",\"Present SOS\",\"On Leave\",\"Absent SOS\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=TodayPresentSOSummary" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                //   var retailers = ManageRetailer.GetRetailersForExportinExcel();
                int srNo = 1;
                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                    srNo,
                    
                    retailer.RegionalHead,
                    retailer.DateOfAttendance.HasValue
        ? retailer.DateOfAttendance.Value.ToString("yyyy-MM-dd")  // If DateOfAttendance has a value, format it
        : "",
                    retailer.TotalSOCount,
                     retailer.PresentCount,
                     retailer.LeaveCount,
                     retailer.AbsentCount,
                   
                    srNo++


                    ));
                }
                Response.Write(sw.ToString());
                Response.End();
                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;
                hst.ReportName = "Present SO Report";
                hst.ReportType = "PresentSO";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        public ActionResult PresentSOReportDetail()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            //  var objRetailer = new RetailerData();
            //objRetailer.Regions = FOS.Setup.ManageCity.GetRegionList();

            List<RegionalHeadData> objRegion = new List<RegionalHeadData>();
            IEnumerable<RegionalHeadData> obj = new List<RegionalHeadData>();
            if (userID == 1)
            {
                objRegion.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;


            // regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            obj = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            objRegion = obj.ToList();

            var objRetailer = new RetailerData();
            objRetailer.Regionss = objRegion;
            objRetailer.Range = ranges;
            return View(objRetailer);
        }


        public void TodayPresentSODetail(string StartingDate, string EndingDate, int TID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();
                List<spGetSalesOfficerAttendanceDetail_Result> result = db.spGetSalesOfficerAttendanceDetail(TID, start, final).ToList();
                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SrNo.\",\"Date\",\"Regional Head\",\"SO Name\",\"Total SOS\",\"Present\",\"On Leave\",\"Absent\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=TodayPresentSODetail" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                //   var retailers = ManageRetailer.GetRetailersForExportinExcel();
                int srNo = 1;
                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                    srNo,
                    retailer.DateOfAttendance,
                    retailer.RegionalHead,
                    retailer.SalesOfficer,
                     
                    retailer.TotalSOCount,
                     retailer.Present,
                     retailer.OnLeave,
                     retailer.Absent,

                    srNo++


                    ));
                }
                Response.Write(sw.ToString());
                Response.End();
                ManagersLoginHst hst = new ManagersLoginHst();
                hst.UserID = userID;
                hst.IPAddress = remoteIpAddress;
                hst.ReportName = "Present SO Report";
                hst.ReportType = "PresentSO";
                hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.ManagersLoginHsts.Add(hst);
                db.SaveChanges();


            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        public ActionResult MarketInformation(string StartingDate, string EndingDate, int TID, int fosid, int dealerid)
        {
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                ManageRetailer objRetailers = new ManageRetailer();
                List<sp_MarketInformation_Result> Result = objRetailers.MarketInfo(start, end, TID, fosid, dealerid);

                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol5 = new DataColumn("SaleOfficerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol3 = new DataColumn("DealerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol0 = new DataColumn("ShopName", typeof(System.String));
                dtNewTable.Columns.Add(dcol0);
                DataColumn dcol06 = new DataColumn("JobDate", typeof(System.DateTime));
                dtNewTable.Columns.Add(dcol06);
                DataColumn dcol2 = new DataColumn("Available", typeof(System.String));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("Available1kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("Available5kg", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol7);
                DataColumn dcol6 = new DataColumn("PSO Material", typeof(System.String));
                dtNewTable.Columns.Add(dcol6);
                DataColumn dcol65 = new DataColumn("UBL Account Opened", typeof(System.String));
                dtNewTable.Columns.Add(dcol65);
                DataColumn dcol66 = new DataColumn("Broucher Avaiabe", typeof(System.String));
                dtNewTable.Columns.Add(dcol66);
                DataColumn dcol67 = new DataColumn("SMS Card Avaiable", typeof(System.String));
                dtNewTable.Columns.Add(dcol67);
                DataColumn dcol68 = new DataColumn("Shade Car Avaiable", typeof(System.String));
                dtNewTable.Columns.Add(dcol68);
                DataColumn dcol69 = new DataColumn("Display", typeof(System.String));
                dtNewTable.Columns.Add(dcol69);
                DataColumn dcol70 = new DataColumn("White 40 KG Avaiable", typeof(System.String));
                dtNewTable.Columns.Add(dcol70);
                DataColumn dcol71 = new DataColumn("Note", typeof(System.String));
                dtNewTable.Columns.Add(dcol71);
                foreach (var item in Result)
                {

                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = item.SaleOfficerName;
                    dtrow[1] = item.DealerName;
                    dtrow[2] = (item.ShopName);
                    dtrow[3] = item.JobDate;
                    dtrow[4] = item.Available;
                    dtrow[5] = item.Available1kg;
                    dtrow[6] = item.Available5kg;
                    dtrow[7] = item.PSOMaterial;
                    dtrow[8] = item.UBLAccountOpened;
                    dtrow[9] = item.BroucherAvaiabe;
                    dtrow[10] = item.SmsCardAvailable;
                    dtrow[11] = item.ShadeCardAvailable;
                    dtrow[12] = item.Display;
                    dtrow[13] = item.White40KgAvailable;
                    dtrow[14] = item.Note;

                    dtNewTable.Rows.Add(dtrow);

                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/MarketInformation.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "MarketInformation.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("MarketInformation{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }

        }


        public ActionResult CityRetailerWiseReport()
        {
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<CityData> cities = new List<CityData>();
            cities = FOS.Setup.ManageRegionalHead.GetCities();
            cities.Insert(0, new CityData
            {
                ID = 0,
                Name = "All"
            });

            List<RetailerData> RetailerObj = new List<RetailerData>();
            RetailerObj.Insert(0, new RetailerData
            {
                ID = 0,
                Name = "All"
            });
            var objJob = new JobsData();
            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.Retailers = RetailerObj;
            objJob.Cities = cities;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");

            return View(objJob);
        }

        public ActionResult CityRetailerWiseReportExtract(int cityid, int retailerid)
        {
            try
            {

                DateTime start = DateTime.Now;
                //DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp_CityMarketRetailerInfo_Result> Result = objRetailers.CityMarketRetailerInfo(start, cityid, retailerid);

                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol5 = new DataColumn("City", typeof(System.String));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol3 = new DataColumn("Area", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol0 = new DataColumn("ShopName", typeof(System.String));
                dtNewTable.Columns.Add(dcol0);
                DataColumn dcol06 = new DataColumn("Category", typeof(System.String));
                dtNewTable.Columns.Add(dcol06);
                DataColumn dcol2 = new DataColumn("Previous ", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("Current", typeof(System.Int32));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("Average", typeof(System.Decimal));
                dtNewTable.Columns.Add(dcol7);

                foreach (var item in Result)
                {

                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = item.City;
                    dtrow[1] = item.Area;
                    dtrow[2] = (item.ShopName);
                    dtrow[3] = item.Category;
                    dtrow[4] = item.Previous;
                    dtrow[5] = item.Current;
                    dtrow[6] = item.Average;


                    dtNewTable.Rows.Add(dtrow);

                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/CityRetailerWiseInfo.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "CityRetailerWiseInfo.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("CityRetailerWiseInfo{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }

        }


        public ActionResult ShopBrandWiseDisplay()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            regionalHeadData.Insert(0, new RegionalHeadData
            {
                ID = 0,
                Name = "All"
            });

            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regionalHeadData.FirstOrDefault().ID, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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


        public ActionResult ShopBrandWiseDisplayReport(string StartingDate, string EndingDate, int TID, int fosid, int dealerid, int cityid, int display)
        {
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                ManageRetailer objRetailers = new ManageRetailer();
                List<Sp__ShopBrandWiseDisplayData_Result> result = objRetailers.ShopBrandWiseDisplayReport(start, end, TID, fosid, dealerid, cityid, display);
                rptDataSet obj = new rptDataSet();
                DataRow dtrow;
                DataTable dtNewTable;
                dtNewTable = new DataTable();
                DataColumn dcol51 = new DataColumn("Terretory Head", typeof(System.String));
                dtNewTable.Columns.Add(dcol51);
                DataColumn dcol5 = new DataColumn("SaleOfficerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol5);
                DataColumn dcol3 = new DataColumn("DealerName", typeof(System.String));
                dtNewTable.Columns.Add(dcol3);
                DataColumn dcol0 = new DataColumn("ShopName", typeof(System.String));
                dtNewTable.Columns.Add(dcol0);
                DataColumn dcol01 = new DataColumn("City", typeof(System.String));
                dtNewTable.Columns.Add(dcol01);
                DataColumn dcol2 = new DataColumn("Date", typeof(System.DateTime));
                dtNewTable.Columns.Add(dcol2);
                DataColumn dcol4 = new DataColumn("Display", typeof(System.String));
                dtNewTable.Columns.Add(dcol4);
                DataColumn dcol7 = new DataColumn("Path", typeof(System.String));
                dtNewTable.Columns.Add(dcol7);





                foreach (var item in result)
                {

                    dtrow = dtNewTable.NewRow();
                    dtrow[0] = item.Name;
                    dtrow[1] = item.SaleOfficerName;
                    dtrow[2] = item.DealerName;
                    dtrow[3] = (item.ShopName);
                    dtrow[4] = (item.CityName);
                    dtrow[5] = item.DateComplete;
                    dtrow[6] = item.Display;
                    dtrow[7] = item.Path;

                    dtNewTable.Rows.Add(dtrow);

                }

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/ShopBrandWiseDisplay.rpt")));
                rd.SetDataSource(dtNewTable);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                string type = "xls";
                if (type == "pdf")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "ShopBrandWiseDisplay.pdf");
                }
                else
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/ms-excel", string.Format("ShopBrandWiseDisplay{0}.xls", DateTime.Now.ToShortDateString()));
                }

                return View();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                return null;
            }

        }

        public ActionResult DailyPerformanceKPI()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
           objJob.Range = ranges;
            objJob.PainterCityNames = FOS.Setup.ManagePainter.GetPainterCityNamesList();
            objJob.VisitPlanEach = new Shared.Common.SelectedWeekday("0000000");
           // objJob.Cities = ManageCity.GetCityListBySOID(SaleOfficerObj.FirstOrDefault().ID);
            return View(objJob);
        }

        public void DailyPerformanceKPIDetails(string StartingDate, string EndingDate, string Type , string Typess,int RHID, int SOID, string ReportType, int RHIDD)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();
            FOSDataModel data = new FOSDataModel();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            DateTime final = end.AddDays(1);
            //RegionWIse
            if (Type == "RegionWise")
            {
                List<sp_getRegionWiseOrdersAndFollowups1_4_Result> result = data.sp_getRegionWiseOrdersAndFollowups1_4(start, final,RHID).ToList();

                if (ReportType == "Excel")
                {
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"SrNo.\",\"RegionName\",\"Orders\",\"FollowUps\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=RegionWise" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";

                    //var retailers = ManageRetailer.GetRetailersForExportinExcel();
                    int srNo = 1;
                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                        srNo,

                        retailer.Name,
                        retailer.Orders,
                        retailer.Followups,
                        srNo++


                        ));
                    }
                    Response.Write(sw.ToString());
                    Response.End();

                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report";
                    hst.ReportType = "RegionWise";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();


                }
                else
                {
                    try
                    {
                        var regionalheadName = db.RegionalHeads.Where(x => x.ID == RHID).Select(x => x.Name).FirstOrDefault();

                        ReportParameter[] prm = new ReportParameter[4];
                       
                        prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[1] = new ReportParameter("DateTo", EndingDate);
                        prm[2] = new ReportParameter("DateFrom", StartingDate);
                        prm[3] = new ReportParameter("RegionalHeadName", regionalheadName);
                    
                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\RegionWise.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
                       
                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);
                     
                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=RegionWiseReport" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();

                    }
                   

                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Report Not Working");

                    }

                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report PDF";
                    hst.ReportType = "RegionWise";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }

            }

            //SOWise
            if (Type == "SoWise")
            {
                List<sp_getSoWiseOrdersAndFollowUps1_6_Result> result = data.sp_getSoWiseOrdersAndFollowUps1_6(start, final,RHID,6).ToList();
                if (ReportType == "Excel")
                {

                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"SrNo.\",\"RegionalHeadName\",\"SOName\",\"RetailerOrders\",\"Followups\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=SOWise" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";

                    //var retailers = ManageRetailer.GetRetailersForExportinExcel();
                    int srNo = 1;
                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                        srNo,
                        retailer.RegionName,
                        retailer.SaleOfficerName,
                        retailer.RetailerOrders,
                      
                        retailer.Followups,
                        srNo++


                        ));
                    }
                    Response.Write(sw.ToString());
                    Response.End();

                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report";
                    hst.ReportType = "SOWise";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        var regionalheadName = db.RegionalHeads.Where(x => x.ID == RHID).Select(x => x.Name).FirstOrDefault();

                        ReportParameter[] prm = new ReportParameter[4];

                        prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[1] = new ReportParameter("DateTo", EndingDate);
                        prm[2] = new ReportParameter("DateFrom", StartingDate);
                        prm[3] = new ReportParameter("RegionalHeadName", regionalheadName);

                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\SOWise.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=SOWiseReport" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();

                    }

                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Report Not Working");

                    }
                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report PDF";
                    hst.ReportType = "SOWise";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
            }

            //ItemWise
            if (Type == "ItemWise")
            {
                List<sp_getItemWiseOrdersCount1_1_Result> result = data.sp_getItemWiseOrdersCount1_1(start, final,RHID).ToList();

                if (ReportType == "Excel")
                {
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"SrNo.\",\"Range Name\",\"Item Name\",\"Quantity\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=ItemWise" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";

                    //var retailers = ManageRetailer.GetRetailersForExportinExcel();
                    int srNo = 1;
                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                        srNo,
                        retailer.Maincategdesc,
                 
                        retailer.ItemName,
                        retailer.Qty,
                        srNo++


                        ));
                    }
                    Response.Write(sw.ToString());
                    Response.End();

                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report";
                    hst.ReportType = "ItemWise";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        var regionalheadName = db.RegionalHeads.Where(x => x.ID == RHID).Select(x => x.Name).FirstOrDefault();

                        ReportParameter[] prm = new ReportParameter[4];

                        prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[1] = new ReportParameter("DateTo", EndingDate);
                        prm[2] = new ReportParameter("DateFrom", StartingDate);
                        prm[3] = new ReportParameter("RegionalHeadName", regionalheadName);

                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\ItemWise.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=ItemWiseReport" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();

                    }

                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Report Not Working");

                    }
                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report PDF";
                    hst.ReportType = "ItemWise";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
            }
            //DistributorWise
            if (Type == "DistributorWise")
            {
                List<sp_getDistributorWiseOrdersCount1_2_Result> result = data.sp_getDistributorWiseOrdersCount1_2(start, final,RHID).ToList();

                if (ReportType == "Excel")
                {
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"SrNo.\",\"Region\",\"ShopName\",\"Quantity\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=DistributorWise" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";

                    //var retailers = ManageRetailer.GetRetailersForExportinExcel();
                    int srNo = 1;
                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                        srNo,

                        retailer.Name,
                        retailer.ShopName,
                        retailer.Orders,
                        srNo++


                        ));
                    }
                    Response.Write(sw.ToString());
                    Response.End();

                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report";
                    hst.ReportType = "DistributorWise";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        var regionalheadName = db.RegionalHeads.Where(x => x.ID == RHID).Select(x => x.Name).FirstOrDefault();

                        ReportParameter[] prm = new ReportParameter[4];

                        prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[1] = new ReportParameter("DateTo", EndingDate);
                        prm[2] = new ReportParameter("DateFrom", StartingDate);
                        prm[3] = new ReportParameter("RegionalHeadName", regionalheadName);

                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\DistributorWise.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=DistributorWiseReport" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();

                    }

                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Report Not Working");

                    }
                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report PDF";
                    hst.ReportType = "DistributorWise";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
            }
            //NoSOPresent
            if (Type == "NoSoPresent")
            {
                List<sp_getNoSoPresentInJobOrder1_1_Result> result = data.sp_getNoSoPresentInJobOrder1_1(start, final,RHID).ToList();
                if (ReportType == "Excel")
                {

                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"SrNo.\",\"RegionalHeadName\",\"SoName\",\"Phone1\",\"Phone2\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=SoAbsent" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";

                    //var retailers = ManageRetailer.GetRetailersForExportinExcel();
                    int srNo = 1;
                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                        srNo,
                        retailer.RegionalHead,
                        retailer.Saleofficer,
                        retailer.Phone1,
                        retailer.Phone2,
                        srNo++


                        ));
                    }
                    Response.Write(sw.ToString());
                    Response.End();

                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report";
                    hst.ReportType = "NOSOPresent";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        var regionalheadName = db.RegionalHeads.Where(x => x.ID == RHID).Select(x => x.Name).FirstOrDefault();

                        ReportParameter[] prm = new ReportParameter[4];

                        prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[1] = new ReportParameter("DateTo", EndingDate);
                        prm[2] = new ReportParameter("DateFrom", StartingDate);
                        prm[3] = new ReportParameter("RegionalHeadName", regionalheadName);

                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\AbsentSOWise.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=AbsentSOWiseReport" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();

                    }

                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Report Not Working");

                    }
                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report PDF";
                    hst.ReportType = "NOSOPresent";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
            }


            if (Type == "OnlineVsOffline")
            {
                List<sp_getOnlineVsOffline1_1_Result> result = data.sp_getOnlineVsOffline1_1(start, final,RHID).ToList();

                if (ReportType == "Excel")
                {
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"SrNo.\",\"RegionalHeadName\",\"SoName\",\"Online\",\"Offline\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=OnlineVsOffline" + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";

                    //var retailers = ManageRetailer.GetRetailersForExportinExcel();
                    int srNo = 1;
                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                        srNo,
                        retailer.RegionalHaeadName,
                        retailer.SoName,
                        retailer.OnlineOrders,
                        retailer.OfflineOrders
                       ,
                        srNo++


                        ));
                    }
                    Response.Write(sw.ToString());
                    Response.End();
                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report";
                    hst.ReportType = "OnlineVSOffline";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
                else
                {
                    var regionalheadName = "";
                    try
                    {
                        if (RHID == 0)
                        {
                            regionalheadName = "All";
                        }
                        else
                        {

                             regionalheadName = db.RegionalHeads.Where(x => x.ID == RHID).Select(x => x.Name).FirstOrDefault();
                        }
                        

                        ReportParameter[] prm = new ReportParameter[4];

                        prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[1] = new ReportParameter("DateTo", EndingDate);
                        prm[2] = new ReportParameter("DateFrom", StartingDate);
                        prm[3] = new ReportParameter("RegionalHeadName", regionalheadName);

                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\OnlineOfflineWise.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=SOWiseReport" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();

                    }

                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Report Not Working");

                    }
                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Daily Performance KPI Report PDF";
                    hst.ReportType = "OnlineVSOffline";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
            }

            if (Type == "SOSummary")
            {
                if (Typess == "Daily")
                {
                    List<KPIData> list = new List<KPIData>();

                    KPIData comlist;

                    if(RHID==0 && SOID == 0)
                    {
                        var SaleOfficerIDs = db.SaleOfficers.Where(x =>  x.IsActive == true && x.IsDeleted == false).Select(x => x.ID).ToList();
                        foreach (var item in SaleOfficerIDs)
                        {
                            var totalVisitsToday = db.JobsDetails.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final && x.Status == true).ToList();

                            if (totalVisitsToday != null)
                            {
                                comlist = new KPIData();
                                comlist.SoName = db.SaleOfficers.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                                comlist.totalVisits = totalVisitsToday.Count();
                                comlist.CityName= totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final).Select(x => x.Retailer.City.Name).FirstOrDefault();
                                comlist.StartDate = totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final).Select(x => x.JobDate).FirstOrDefault();
                                comlist.EndDate = totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final).OrderByDescending(x => x.ID).Select(x => x.JobDate).FirstOrDefault();
                                comlist.ProductiveShops = totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final && x.VisitPurpose == "Ordering" && x.Status == true
                                ).Select(x => x.ID).Count();
                                comlist.totallines = db.Sp_TotalLinesForSummaryInDSR(item, start, final).FirstOrDefault();
                                var finallines = Convert.ToDecimal(comlist.totallines);
                                var finalProductiveShops = Convert.ToDecimal(comlist.ProductiveShops);

                                if (comlist.ProductiveShops != 0)
                                {

                                    var Linesperbill = finallines / finalProductiveShops;

                                    comlist.Linesperbill = Linesperbill.ToString("0.0");
                                }

                                TimeSpan? diff = comlist.EndDate - comlist.StartDate;

                                comlist.ElapseTime = string.Format("{0:%h} hours, {0:%m} minutes, {0:%s} seconds", diff);
                                var total = totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final && x.Status == true).Sum(i => i.OrderTotal);
                                comlist.totalSale = total;
                                list.Add(comlist);
                            }
                        }

                    }

                   else if (SOID == 0 && RHID != 0)
                    {
                        var SaleOfficerIDs = db.SaleOfficers.Where(x => x.RegionalHeadID == RHIDD && x.IsActive==true&& x.IsDeleted==false).Select(x => x.ID).ToList();
                        foreach (var item in SaleOfficerIDs)
                        {
                            var totalVisitsToday = db.JobsDetails.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final && x.Status == true).ToList();

                            if (totalVisitsToday != null)
                            {
                                comlist = new KPIData();
                                comlist.SoName = db.SaleOfficers.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                                comlist.totalVisits = totalVisitsToday.Count();
                                comlist.CityName = totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final).Select(x => x.Retailer.City.Name).FirstOrDefault();
                                comlist.StartDate = totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final).Select(x => x.JobDate).FirstOrDefault();
                                comlist.EndDate = totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final).OrderByDescending(x => x.ID).Select(x => x.JobDate).FirstOrDefault();
                                comlist.ProductiveShops = totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final && x.JobType == "Retailer Order" && x.VisitPurpose == "Ordering" && x.Status == true
                                ).Select(x => x.ID).Count();
                                comlist.totallines = db.Sp_TotalLinesForSummaryInDSR(item, start, final).FirstOrDefault();
                                var finallines = Convert.ToDecimal(comlist.totallines);
                                var finalProductiveShops = Convert.ToDecimal(comlist.ProductiveShops);

                                if (comlist.ProductiveShops != 0)
                                {

                                    var Linesperbill = finallines / finalProductiveShops;

                                    comlist.Linesperbill = Linesperbill.ToString("0.0");
                                }

                               TimeSpan? diff = comlist.EndDate - comlist.StartDate;

                                comlist.ElapseTime = string.Format("{0:%h} hours, {0:%m} minutes, {0:%s} seconds", diff);
                                var total = totalVisitsToday.Where(x => x.SalesOficerID == item && x.JobDate >= start && x.JobDate <= final && x.Status == true).Sum(i => i.OrderTotal);
                                comlist.totalSale = total;
                                list.Add(comlist);
                            }
                        }
                    }
                    else
                    {
                        var totalVisitsToday = db.JobsDetails.Where(x => x.SalesOficerID == SOID && x.JobDate >= start && x.JobDate <= final && x.Status == true).ToList();

                        comlist = new KPIData();
                        comlist.SoName = db.SaleOfficers.Where(x => x.ID == SOID).Select(x => x.Name).FirstOrDefault();
                        comlist.totalVisits = totalVisitsToday.Count();
                        comlist.CityName = totalVisitsToday.Where(x => x.SalesOficerID == SOID && x.JobDate >= start && x.JobDate <= final).Select(x => x.Retailer.City.Name).FirstOrDefault();
                        comlist.StartDate = totalVisitsToday.Where(x => x.SalesOficerID == SOID && x.JobDate >= start && x.JobDate <= final).Select(x => x.JobDate).FirstOrDefault();
                        comlist.EndDate = totalVisitsToday.Where(x => x.SalesOficerID == SOID && x.JobDate >= start && x.JobDate <= final).OrderByDescending(x => x.ID).Select(x => x.JobDate).FirstOrDefault();
                        comlist.ProductiveShops = totalVisitsToday.Where(x => x.SalesOficerID == SOID && x.JobDate >= start && x.JobDate <= final  && x.VisitPurpose == "Ordering" && x.Status == true
                        ).Select(x => x.ID).Count();
                        comlist.totallines = db.Sp_TotalLinesForSummaryInDSR(SOID, start, final).FirstOrDefault();
                        var finallines = Convert.ToDecimal(comlist.totallines);
                        var finalProductiveShops = Convert.ToDecimal(comlist.ProductiveShops);

                        if (comlist.ProductiveShops != 0)
                        {

                            var Linesperbill = finallines / finalProductiveShops;

                            comlist.Linesperbill = Linesperbill.ToString("0.0");
                        }

                        TimeSpan? diff = comlist.EndDate - comlist.StartDate;

                        comlist.ElapseTime = string.Format("{0:%h} hours, {0:%m} minutes, {0:%s} seconds", diff);
                        var total = totalVisitsToday.Where(x => x.SalesOficerID == SOID && x.JobDate >= start && x.JobDate <= final && x.Status==true).Sum(i => i.OrderTotal);
                        comlist.totalSale = total;
                        list.Add(comlist);

                    }

                    var filtered = list.Where(t => t.totalVisits > 0);

                    if (ReportType == "Excel")
                    {


                        // Example data
                        StringWriter sw = new StringWriter();

                        sw.WriteLine("\"SrNo.\",\"AreaName\",\"SOName\",\"Total Visits\",\"Productive Visits\",\"Total Lines\",\"LinesPerBill\",\"Market Start Time\",\"Market End Time\",\"Elapse Time\",\"TotalSale\"");

                        Response.ClearContent();
                        Response.AddHeader("content-disposition", "attachment;filename=Performance Report" + DateTime.Now + ".csv");
                        Response.ContentType = "application/octet-stream";

                        //var retailers = ManageRetailer.GetRetailersForExportinExcel();
                        int srNo = 1;
                        foreach (var retailer in filtered)
                        {
                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",
                            srNo,
                            retailer.CityName,
                            retailer.SoName,
                            retailer.totalVisits,
                            retailer.ProductiveShops,
                            retailer.totallines,
                             retailer.Linesperbill,
                              retailer.StartDate,
                               retailer.EndDate,
                                retailer.ElapseTime,
                                retailer.totalSale
                           ,
                            srNo++


                            ));
                        }
                        Response.Write(sw.ToString());
                        Response.End();
                        ManagersLoginHst hst = new ManagersLoginHst();
                        hst.UserID = userID;
                        hst.IPAddress = remoteIpAddress;
                        hst.ReportName = "Daily Performance KPI Report";
                        hst.ReportType = "SOSummary Daily";
                        hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                        db.ManagersLoginHsts.Add(hst);
                        db.SaveChanges();
                    }
                    else
                    {
                        try
                        {


                            var regionalheadName = db.RegionalHeads.Where(x => x.ID == RHIDD).Select(x => x.Name).FirstOrDefault();
                            if (regionalheadName == null)
                            {
                                regionalheadName = "ALL";
                            }

                            ReportParameter[] prm = new ReportParameter[4];

                            prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                            prm[1] = new ReportParameter("DateTo", EndingDate);
                            prm[2] = new ReportParameter("DateFrom", StartingDate);
                            prm[3] = new ReportParameter("RegionalHeadName", regionalheadName);

                            ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\DailyKPI.rdlc");
                            ReportViewer1.EnableExternalImages = true;
                            ReportDataSource dt1 = new ReportDataSource("DataSet1", filtered);

                            ReportViewer1.SetParameters(prm);
                            ReportViewer1.DataSources.Clear();
                            ReportViewer1.DataSources.Add(dt1);

                            ReportViewer1.Refresh();



                            Warning[] warnings;
                            string[] streamIds;
                            string contentType;
                            string encoding;
                            string extension;

                            //Export the RDLC Report to Byte Array.
                            byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                            //Download the RDLC Report in Word, Excel, PDF and Image formats.
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.ContentType = contentType;
                            Response.AddHeader("content-disposition", "attachment;filename=Performance Report Daily" + DateTime.Now + ".Pdf");
                            Response.BinaryWrite(bytes);
                            Response.Flush();

                            Response.End();

                        }

                        catch (Exception exp)
                        {
                            Log.Instance.Error(exp, "Report Not Working");

                        }
                        ManagersLoginHst hst = new ManagersLoginHst();
                        hst.UserID = userID;
                        hst.IPAddress = remoteIpAddress;
                        hst.ReportName = "Daily Performance KPI Report PDF  ";
                        hst.ReportType = "SOSummary Daily";
                        hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                        db.ManagersLoginHsts.Add(hst);
                        db.SaveChanges();
                    }
                }


                if (Typess == "Weekly")
                {
                    List<KPIData> list = new List<KPIData>();

                    KPIData comlist;



                    var lists = db.sp_GetKPISummarySOWise(start, final, RHIDD, SOID,6).ToList();

                    if (lists != null)
                    {
                        foreach (var items in lists)
                        {
                            comlist = new KPIData();

                            comlist.RHName = items.RegionName;
                            comlist.SoName = items.SaleOfficerName;
                            comlist.totalVisits = items.TotalVisits;

                            comlist.ProductiveShops = (int)items.ProductiveOrders;
                            comlist.NonProductive = (int)items.NonProductive;
                            //comlist.totallines = items.TotalLines;
                            //var finallines = Convert.ToDecimal(comlist.totallines);
                            //var finalProductiveShops = Convert.ToDecimal(comlist.ProductiveShops);

                            //if (comlist.ProductiveShops != 0)
                            //{

                            //    var Linesperbill = finallines / finalProductiveShops;

                            //    comlist.Linesperbill = Linesperbill.ToString("0.0");
                            //}



                            comlist.ProductivePer = (decimal)items.Productiveperage;

                            //TimeSpan t = final - start;
                            //double NrOfDays = t.TotalDays;


                            var days = db.sp_getSOWorkingDays(start,final,items.ID).Count();

                            

                            var val = (double) items.ProductiveOrders / days;
                            comlist.perDayorders = System.Math.Round(val, 2); 
                            //TimeSpan? diff = comlist.EndDate - comlist.StartDate;

                            //comlist.ElapseTime = string.Format("{0:%h} hours, {0:%m} minutes, {0:%s} seconds", diff);

                            list.Add(comlist);

                        }



                    }


                    if (ReportType == "Excel")
                    {

                        // Example data
                        StringWriter sw = new StringWriter();

                        sw.WriteLine("\"SrNo.\",\"HeadName\",\"SOName\",\"Total Visits\",\"Productive Visits\",\"Non-Productive Visits\",\"Total Lines\",\"LinesPerBill\",\"Productive Percentage\",\"Per Day Orders\"");

                        Response.ClearContent();
                        Response.AddHeader("content-disposition", "attachment;filename=Performance Report Weekly/Monthly" + DateTime.Now + ".csv");
                        Response.ContentType = "application/octet-stream";

                        //var retailers = ManageRetailer.GetRetailersForExportinExcel();
                        int srNo = 1;
                        foreach (var retailer in list)
                        {
                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"",
                            srNo,
                            retailer.RHName,
                            retailer.SoName,
                            retailer.totalVisits,
                            retailer.ProductiveShops,
                            retailer.NonProductive,
                            retailer.totallines,
                             retailer.Linesperbill,
                              retailer.ProductivePer,
                               retailer.perDayorders,


                            srNo++


                            ));
                        }
                        Response.Write(sw.ToString());
                        Response.End();
                        ManagersLoginHst hst = new ManagersLoginHst();
                        hst.UserID = userID;
                        hst.IPAddress = remoteIpAddress;
                        hst.ReportName = "Daily Performance KPI Report";
                        hst.ReportType = "SOSummary Weekly/Monthly";
                        hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                        db.ManagersLoginHsts.Add(hst);
                        db.SaveChanges();
                    }
                    else
                    {
                        try
                        {
                           

                            var regionalheadName = db.RegionalHeads.Where(x => x.ID == RHIDD).Select(x => x.Name).FirstOrDefault();

                            ReportParameter[] prm = new ReportParameter[4];

                            prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                            prm[1] = new ReportParameter("DateTo", EndingDate);
                            prm[2] = new ReportParameter("DateFrom", StartingDate);
                            prm[3] = new ReportParameter("RegionalHeadName", regionalheadName);

                            ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\Weeklymonthly.rdlc");
                            ReportViewer1.EnableExternalImages = true;
                            ReportDataSource dt1 = new ReportDataSource("DataSet1", list);

                            ReportViewer1.SetParameters(prm);
                            ReportViewer1.DataSources.Clear();
                            ReportViewer1.DataSources.Add(dt1);

                            ReportViewer1.Refresh();



                            Warning[] warnings;
                            string[] streamIds;
                            string contentType;
                            string encoding;
                            string extension;

                            //Export the RDLC Report to Byte Array.
                            byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                            //Download the RDLC Report in Word, Excel, PDF and Image formats.
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.ContentType = contentType;
                            Response.AddHeader("content-disposition", "attachment;filename=Performance Report Weekly/Monthly" + DateTime.Now + ".Pdf");
                            Response.BinaryWrite(bytes);
                            Response.Flush();

                            Response.End();

                        }

                        catch (Exception exp)
                        {
                            Log.Instance.Error(exp, "Report Not Working");

                        }

                        ManagersLoginHst hst = new ManagersLoginHst();
                        hst.UserID = userID;
                        hst.IPAddress = remoteIpAddress;
                        hst.ReportName = "Daily Performance KPI Report PDF";
                        hst.ReportType = "SOSummary Weekly/Monthly";
                        hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                        db.ManagersLoginHsts.Add(hst);
                        db.SaveChanges();
                    }
                }




            }
        }

        public ActionResult CallSummaryReport()
        {
            FOSDataModel db = new FOSDataModel();
            //var userID = Convert.ToInt32(Session["UserID"]);
            var objSaleOffice = new OrderSummaryReportData();
            //objSaleOffice.ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);

            //var rangeid = objSaleOffice.ranges.Select(r => r.ID).FirstOrDefault();
            //objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            //var regionid = objSaleOffice.RegionalHead.FirstOrDefault();
            //objSaleOffice.SaleOfficer = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regionid.ID, true);
            //var regionids = objSaleOffice.ranges.FirstOrDefault();
            //objSaleOffice.dealerdata = ManageDealer.GetSegmentType();
            return View(objSaleOffice);

        }

        public ActionResult VisitsCountReport()
        {
            FOSDataModel db = new FOSDataModel();
            //var userID = Convert.ToInt32(Session["UserID"]);
            var objSaleOffice = new OrderSummaryReportData();
            //objSaleOffice.ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);

            //var rangeid = objSaleOffice.ranges.Select(r => r.ID).FirstOrDefault();
            //objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            //var regionid = objSaleOffice.RegionalHead.FirstOrDefault();
            //objSaleOffice.SaleOfficer = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regionid.ID, true);
            //var regionids = objSaleOffice.ranges.FirstOrDefault();
            //objSaleOffice.dealerdata = ManageDealer.GetSegmentType();
            return View(objSaleOffice);

        }

        public ActionResult BrightoServicesReport()
        {
            FOSDataModel db = new FOSDataModel();
            //var userID = Convert.ToInt32(Session["UserID"]);
            var objSaleOffice = new OrderSummaryReportData();
            //objSaleOffice.ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);

            //var rangeid = objSaleOffice.ranges.Select(r => r.ID).FirstOrDefault();
            //objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            //var regionid = objSaleOffice.RegionalHead.FirstOrDefault();
            //objSaleOffice.SaleOfficer = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regionid.ID, true);
            //var regionids = objSaleOffice.ranges.FirstOrDefault();
            //objSaleOffice.dealerdata = ManageDealer.GetSegmentType();
            return View(objSaleOffice);

        }

        #region ClaimApprovalReport

        public ActionResult ClaimApprovalSummary()
        {
            FOSDataModel db = new FOSDataModel();
            var userID = Convert.ToInt32(Session["UserID"]);
            var objSaleOffice = new OrderSummaryReportData();
            objSaleOffice.ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);

            var rangeid = objSaleOffice.ranges.Select(r => r.ID).FirstOrDefault();
            objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            var regionid = objSaleOffice.RegionalHead.FirstOrDefault();
            objSaleOffice.SaleOfficer = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regionid.ID, true);
            var regionids = objSaleOffice.ranges.FirstOrDefault();
            objSaleOffice.dealerdata = ManageDealer.GetReportType();
            return View(objSaleOffice);

        }

        public void ApprovalSummary(string StartingDate, string EndingDate,int RHID, int? DisID, int TID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            var type = "Daily";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }

            FOSDataModel db = new FOSDataModel();

            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            DateTime final = end.AddDays(1);
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

            if (type == "Daily")

            {
                if (DisID == 1)
                {

                    try
                    {
                        

                        // Get data from database
                        var result = db.sp_GetSOClaimsApprovalStatus(RHID, TID, start, final).ToList();

                        // Set up CSV response
                        Response.ClearContent();
                        Response.AddHeader("content-disposition", $"attachment;filename=ClaimApprovalSummaryReport_{DateTime.Now:yyyyMMddHHmmss}.csv");
                        Response.ContentType = "text/csv";

                        // Create CSV writer
                        using (var sw = new StringWriter())
                        {
                            // Write CSV header
                            sw.WriteLine("\"Sr#\",\"Head Name\",\"SO Name\",\"Total Claims\",\"Approved Claims\",\"Not Approved Claims\"");

                            if (result != null)
                            {
                                int srNo = 1;
                                foreach (var retailer in result)
                                {
                                    // Calculate discount percentage
                                   // string discountPercentage = CalculateDiscountPercentage(retailer.MRPValue, retailer.TotalPrice);

                                    // Write CSV line using string interpolation for better readability
                                    sw.WriteLine($"\"{srNo}\"," +
                                                $"\"{EscapeCsvValue(retailer.HeadName)}\"," +
                                                $"\"{EscapeCsvValue(retailer.SOName)}\"," +
                                                $"\"{retailer.TotalClaims}\"," +
                                           
                                                $"\"{retailer.ApprovedClaims}\"," +
                                             
                                                $"\"{retailer.NotApprovedClaims}\"");

                                    srNo++;
                                }
                            }

                            Response.Write(sw.ToString());
                        }

                        Response.End();
                    }
                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Error generating Claim Summary Report");
                        // Consider returning an error response to the client
                        Response.StatusCode = 500;
                        Response.Write("Error generating report. Please try again.");
                        Response.End();
                    }





                }

                else if (DisID == 2)
                {

                    try
                    {


                        // Get data from database
                        var result = db.sp_GetSOClaimsDetailApproval(RHID, TID, start, final).ToList();

                        // Set up CSV response
                        Response.ClearContent();
                        Response.AddHeader("content-disposition", $"attachment;filename=ClaimApprovalDetailReport_{DateTime.Now:yyyyMMddHHmmss}.csv");
                        Response.ContentType = "text/csv";

                        // Create CSV writer
                        using (var sw = new StringWriter())
                        {
                            // Write CSV header
                            sw.WriteLine("\"Sr#\",\"ClaimID\",\"Head Name\",\"SO Name\",\"Customer Name\",\"Claim Date\",\"Submitted Date\",\"Approved Date\",\"Approved By\",\"Approved Status\",\"Remarks\"");

                            if (result != null)
                            {
                                int srNo = 1;
                                foreach (var retailer in result)
                                {
                                    // Calculate discount percentage
                                    // string discountPercentage = CalculateDiscountPercentage(retailer.MRPValue, retailer.TotalPrice);

                                    // Write CSV line using string interpolation for better readability
                                    sw.WriteLine($"\"{srNo}\"," +
                                                $"\"{retailer.ClaimID}\"," +
                                                $"\"{EscapeCsvValue(retailer.HeadName)}\"," +
                                                $"\"{EscapeCsvValue(retailer.SOName)}\"," +
                                                $"\"{EscapeCsvValue(retailer.ShopName)}\"," +
                                                $"\"{retailer.ClaimDate}\"," +
                                                  $"\"{retailer.SubmittedDate}\"," +
                                                    $"\"{retailer.ApprovedDate}\"," +
                                                $"\"{retailer.ApprovedBy}\"," +
                                                 $"\"{retailer.ApprovedStatus}\"," +
                                                $"\"{retailer.Remarks}\"");

                                    srNo++;
                                }
                            }

                            Response.Write(sw.ToString());
                        }

                        Response.End();
                    }
                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Error generating Claim Summary Report");
                        // Consider returning an error response to the client
                        Response.StatusCode = 500;
                        Response.Write("Error generating report. Please try again.");
                        Response.End();
                    }





                }




            }

        }

        #endregion

        public ActionResult OrderSummaryReport()
        {
            FOSDataModel db = new FOSDataModel();
            var userID = Convert.ToInt32(Session["UserID"]);
            var objSaleOffice = new OrderSummaryReportData();
            objSaleOffice.ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
      
            var rangeid = objSaleOffice.ranges.Select(r => r.ID).FirstOrDefault();
            objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            var regionid = objSaleOffice.RegionalHead.FirstOrDefault();
            objSaleOffice.SaleOfficer = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regionid.ID, true);
            var regionids = objSaleOffice.ranges.FirstOrDefault();
            objSaleOffice.dealerdata = ManageDealer.GetSegmentType();
            return View(objSaleOffice);

        }
        public ActionResult OrderSummaryReportForDistributor()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            FOSDataModel db = new FOSDataModel();
            var objSaleOffice = new OrderSummaryReportData();
            objSaleOffice.ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);

            var rangeid = objSaleOffice.ranges.Select(r => r.ID).FirstOrDefault();

            objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            var regionid = objSaleOffice.RegionalHead.FirstOrDefault();
            objSaleOffice.SaleOfficer = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regionid.ID, true);
            var regionids = objSaleOffice.ranges.FirstOrDefault();
            // objSaleOffice.CityData = ManageRegion.GetCityListForDSR(objSaleOffice.regionData.FirstOrDefault().RegionID);
            return View(objSaleOffice);

        }
        public JsonResult GetSaleOfficersRetailedtoReg(int? RegID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {


                if (RegID > 0)
                {





                    var result = ManageSaleOffice.GetAllSORelatedToRegionForDistributor(RegID).ToList();
                    return Json((result), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string a = "Select Region";
                    return Json((a), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public JsonResult GetSaleOfficersRetailedtoRegForDistributor(int? RegID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {


                if (RegID > 0)
                {





                    var result = ManageSaleOffice.GetAllSORelatedToRegionForDistributor(RegID).OrderBy(x=>x.Name).ToList();
                    return Json((result), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string a = "Select Region";
                    return Json((a), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public JsonResult GetCitiesRetailedtoRegForDistributor(int? RegID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {


                if (RegID > 0)
                {
                    var result = ManageSaleOffice.GetAllCitiesRelatedToRegionForDistributor(RegID).OrderBy(x => x.Name).ToList();
                    return Json((result), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string a = "Select Region";
                    return Json((a), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public JsonResult GetDistributorRetailedtoSO(int? SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);


                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime todate = dtFromToday.AddDays(1);
                DateTime fromdate = todate.AddDays(-30);

                if (SOID > 0)
                {
                    object[] param = { SOID };

                    var rangeid = db.SaleOfficers.Where(x => x.ID == SOID).Select(x => x.RangeID).FirstOrDefault();
                    if (rangeid == 6)
                    {


                        var result = dbContext.sp_GetDistributorListInDSR(SOID, fromdate, todate).ToList();
                        return Json((result), JsonRequestBehavior.AllowGet);
                    }
                  else if (rangeid == 7)
                    {
                        var result = dbContext.sp_GetDistributorListInDSRRangeB(SOID, fromdate, todate).ToList();
                        return Json((result), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var result = dbContext.sp_GetDistributorListInDSRRangeC(SOID, fromdate, todate).ToList();
                        return Json((result), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string a = "Select SaleOfficer";
                    return Json((a), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public JsonResult GetDistributorRetailedtoSOForDistributor(int? RegID,int? Cityid)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);


                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime todate = dtFromToday.AddDays(1);
                DateTime fromdate = todate.AddDays(-30);

                if (RegID > 0)
                {
                    object[] param = { RegID };




                    var result = ManageSaleOffice.GetDistributorRetailedtoSOForDistributor(RegID,Cityid).ToList();
                    return Json((result), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string a = "Select SaleOfficer";
                    return Json((a), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public void OrderSummary(string StartingDate, string EndingDate, int? DisID, int TID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            var type = "Daily";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }

            FOSDataModel db = new FOSDataModel();

            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            DateTime final = end.AddDays(1);
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();
        
            if (type == "Daily")

            {
                if (DisID == 1)
                {
                    

                       
                    List<Sp_OrderForPDFTradeVisit_Result> result = db.Sp_OrderForPDFTradeVisit(start, final, TID).ToList();
                       
                    if (result.Count > 0)
                        {


                            string SoName = "";
                            var SO = db.SaleOfficers.Where(u => u.ID == TID).FirstOrDefault();

                            SoName = SO.Name;


                            //var SOID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerID).Select(x => x.SORoleID).FirstOrDefault();


                            ReportParameter[] prm = new ReportParameter[10];
                            prm[0] = new ReportParameter("DistributorName", "Test");
                            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                            prm[2] = new ReportParameter("SOName", SoName);

                            prm[3] = new ReportParameter("DateTo", EndingDate);
                            prm[4] = new ReportParameter("DateFrom", StartingDate);

                            prm[5] = new ReportParameter("CityName", "Test");
                            prm[6] = new ReportParameter("TotalVisitsToday", "1");

                            prm[7] = new ReportParameter("ProductiveShops", "1");
                            prm[8] = new ReportParameter("TodayWorkingTime", "1");
                            prm[9] = new ReportParameter("FollowUps", "1");

                           
                            ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\TradeVisit.rdlc");
                            ReportViewer1.EnableExternalImages = true;
                            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
                           
                            ReportViewer1.SetParameters(prm);
                            ReportViewer1.DataSources.Clear();
                            ReportViewer1.DataSources.Add(dt1);
                           
                            ReportViewer1.Refresh();



                            Warning[] warnings;
                            string[] streamIds;
                            string contentType;
                            string encoding;
                            string extension;

                            //Export the RDLC Report to Byte Array.
                            byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                            //Download the RDLC Report in Word, Excel, PDF and Image formats.
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.ContentType = contentType;
                            Response.AddHeader("content-disposition", "attachment;filename=Trade" + DateTime.Now + ".Pdf");
                            Response.BinaryWrite(bytes);
                            Response.Flush();

                            Response.End();




                        }


                       

                  
                   
                }

               else if (DisID == 2)
                {



                    List<Sp_OrderForPDFHousingVisits_Result1> result = db.Sp_OrderForPDFHousingVisits(start, final, TID).ToList();

                    if (result.Count > 0)
                    {


                        string SoName = "";
                        var SO = db.SaleOfficers.Where(u => u.ID == TID).FirstOrDefault();

                        SoName = SO.Name;


                        //var SOID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerID).Select(x => x.SORoleID).FirstOrDefault();


                        ReportParameter[] prm = new ReportParameter[10];
                        prm[0] = new ReportParameter("DistributorName", "Test");
                        prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[2] = new ReportParameter("SOName", SoName);

                        prm[3] = new ReportParameter("DateTo", EndingDate);
                        prm[4] = new ReportParameter("DateFrom", StartingDate);

                        prm[5] = new ReportParameter("CityName", "Test");
                        prm[6] = new ReportParameter("TotalVisitsToday", "1");

                        prm[7] = new ReportParameter("ProductiveShops", "1");
                        prm[8] = new ReportParameter("TodayWorkingTime", "1");
                        prm[9] = new ReportParameter("FollowUps", "1");


                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\HousingVisit.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=Housing" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();




                    }




                }

               else if (DisID == 3)
                {



                    List<Sp_OrderForPDFCorporateVisits_Result> result = db.Sp_OrderForPDFCorporateVisits(start, final, TID).ToList();

                    if (result.Count > 0)
                    {


                        string SoName = "";
                        var SO = db.SaleOfficers.Where(u => u.ID == TID).FirstOrDefault();

                        SoName = SO.Name;


                        //var SOID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerID).Select(x => x.SORoleID).FirstOrDefault();


                        ReportParameter[] prm = new ReportParameter[10];
                        prm[0] = new ReportParameter("DistributorName", "Test");
                        prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[2] = new ReportParameter("SOName", SoName);

                        prm[3] = new ReportParameter("DateTo", EndingDate);
                        prm[4] = new ReportParameter("DateFrom", StartingDate);

                        prm[5] = new ReportParameter("CityName", "Test");
                        prm[6] = new ReportParameter("TotalVisitsToday", "1");

                        prm[7] = new ReportParameter("ProductiveShops", "1");
                        prm[8] = new ReportParameter("TodayWorkingTime", "1");
                        prm[9] = new ReportParameter("FollowUps", "1");


                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\CorporateVisit.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=Corporate" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();




                    }




                }

               else if (DisID == 5)
                {



                    List<Sp_OrderForPDFAllPurposeVisitBackEnd_Result> result = db.Sp_OrderForPDFAllPurposeVisitBackEnd(start, final, TID, 0).ToList();

                    if (result.Count > 0)
                    {


                        string SoName = "";
                        var SO = db.SaleOfficers.Where(u => u.ID == TID).FirstOrDefault();

                        SoName = SO.Name;


                        ReportParameter[] prm = new ReportParameter[10];
                        prm[0] = new ReportParameter("DistributorName", "Test");
                        prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[2] = new ReportParameter("SOName", SoName);

                        prm[3] = new ReportParameter("DateTo", EndingDate);
                        prm[4] = new ReportParameter("DateFrom", StartingDate);

                        prm[5] = new ReportParameter("CityName", "Test");
                        prm[6] = new ReportParameter("TotalVisitsToday", "1");

                        prm[7] = new ReportParameter("ProductiveShops", "1");
                        prm[8] = new ReportParameter("TodayWorkingTime", "1");
                        prm[9] = new ReportParameter("FollowUps", "1");


                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\AllPurposeVisit.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=AllPurpose" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();




                    }




                }



            }

        }


        public void CountReport(string StartingDate, string EndingDate)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            var type = "Daily";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }

            FOSDataModel db = new FOSDataModel();

            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            DateTime final = end.AddDays(1);
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();
            if (type == "Daily")
            {

                var result = db.Database.SqlQuery<VisitsInfo>("exec usp_CalculateVisitsByDateRange @StartDate, @EndDate",
                                   new SqlParameter("StartDate", start),
                                   new SqlParameter("EndDate", end)
                                  ).ToList();


                var result2 = db.Database.SqlQuery<RetailerCounts>("exec usp_CalculateRetailersCountByDateRange @StartDate, @EndDate",
                                  new SqlParameter("StartDate", start),
                                  new SqlParameter("EndDate", end)
                                 ).ToList();

                // var result = db.usp_CalculateVisitsByDateRange(start, final).ToList();

                if (result.Count > 0)
                {


                    string SoName = "";
                    var SO = db.SaleOfficers.Where(u => u.ID == 1).FirstOrDefault();

                    SoName = SO.Name;


                    //var SOID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerID).Select(x => x.SORoleID).FirstOrDefault();


                    ReportParameter[] prm = new ReportParameter[10];
                    prm[0] = new ReportParameter("DistributorName", "Test");
                    prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                    prm[2] = new ReportParameter("SOName", SoName);

                    prm[3] = new ReportParameter("DateTo", EndingDate);
                    prm[4] = new ReportParameter("DateFrom", StartingDate);

                    prm[5] = new ReportParameter("CityName", "Test");
                    prm[6] = new ReportParameter("TotalVisitsToday", "1");

                    prm[7] = new ReportParameter("ProductiveShops", "1");
                    prm[8] = new ReportParameter("TodayWorkingTime", "1");
                    prm[9] = new ReportParameter("FollowUps", "1");


                    ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\Count.rdlc");
                    ReportViewer1.EnableExternalImages = true;
                    ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
                    ReportDataSource dt2 = new ReportDataSource("DataSet2", result2);
                    ReportViewer1.SetParameters(prm);
                    ReportViewer1.DataSources.Clear();
                    ReportViewer1.DataSources.Add(dt1);
                    ReportViewer1.DataSources.Add(dt2);
                    ReportViewer1.Refresh();



                    Warning[] warnings;
                    string[] streamIds;
                    string contentType;
                    string encoding;
                    string extension;

                    //Export the RDLC Report to Byte Array.
                    byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                    //Download the RDLC Report in Word, Excel, PDF and Image formats.
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.ContentType = contentType;
                    Response.AddHeader("content-disposition", "attachment;filename=Counts" + DateTime.Now + ".Pdf");
                    Response.BinaryWrite(bytes);
                    Response.Flush();

                    Response.End();




                    //  }



                }






                //}

            }

        }


        public void CallSummaryReports(string StartingDate, string EndingDate)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            var type = "Daily";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();





                List<sp_GetCallData_Result> result = db.sp_GetCallData(start, final).ToList();


                StringWriter sw = new StringWriter();

                // Write header with your specified columns
                sw.WriteLine("\"Sr No\",\"Call Date\",\"CSR Name\",\"Customer Name\",\"Customer\",\"Customer no\",\"Customer CreatedDate\",\"Regional Head\",\"SO Name\",\"Customer Type\",\"Construction Stage\",\"Site Status\",\"Category\",\"Next Visit\",\"Nature of Call\",\"Call Rating\",\"Objections\",\"Remarks\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=CallDetailsReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                Response.ContentType = "application/octet-stream";

                if (result != null)
                {
                    int srNo = 1;
                    foreach (var call in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\"",
                            call.Sr_No,                                  // Sr No
                            call.Call_Date,   // Call Date (formatted as DD/MM/YYYY)
                            call.CSR_Name,                          // CSR Name
                            call.Customer_Name,                        // Customer Name
                            call.Customer,                            // Customer (ShopName)
                            call.Customer_no,                              // Customer no
                            call.Customer_CreatedDate?.ToString("dd/MM/yyyy HH:mm:ss"), // Customer CreatedDate
                            call.Regional_Head,                         // Regional Head
                            call.SO_Name,                               // SO Name
                            call.Customer_Type,                         // Customer Type
                            call.Construction_Stage,                    // Construction Stage
                            call.Site_Status,                            // Site Status
                            call.Category,                              // Category
                            call.Next_Visit,                            // Next Visit
                            call.Nature_of_Call,                          // Nature of Call
                            call.Call_Rating,                            // Call Rating
                            call.Objections,                            // Objections
                            call.Remarks                                // Remarks
                        ));
                    }
                }

                Response.Write(sw.ToString());
                Response.End();







                Response.Write(sw.ToString());
                Response.End();

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

        }

        public void BrightoServicesAttendanceReports(string StartingDate, string EndingDate)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            var type = "Daily";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);

                StringWriter sw = new StringWriter();
                sw.WriteLine("\"Sr No\",\"Date\",\"Time\",\"Day\",\"Region\",\"Employee Name\",\"Attendance Type\",\"Latitude\",\"Longitude\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=AttendanceReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                Response.ContentType = "application/octet-stream";

                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand("GetDailyAttendanceSummary", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.Date) { Value = start.Date });
                    cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.Date) { Value = final.Date });

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        bool any = false;
                        while (reader.Read())
                        {
                            any = true;

                            object dateVal = reader["Date"];
                            string dateStr = (dateVal == DBNull.Value)
                                ? ""
                                : Convert.ToDateTime(dateVal).ToString("dd-MM-yyyy");

                            sw.WriteLine(string.Format(
                                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
                                reader["Sr No"],
                                dateStr,
                                reader["Time"],
                                reader["Day"],
                                reader["Region"],
                                reader["Employee Name"],
                                reader["Attendance Type"],
                                reader["Latitude"] == DBNull.Value ? "" : reader["Latitude"],
                                reader["Longitude"] == DBNull.Value ? "" : reader["Longitude"]
                            ));
                        }
                        if (!any)
                        {
                            sw.WriteLine("\"No records found for the selected date range\"");
                        }
                    }
                }

                Response.Write(sw.ToString());
                Response.End();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Attendance Report Not Working");
                // Handle exception as needed
            }
        }

        public void OrderSummaryForDistributor(string StartingDate, string EndingDate, int? DisID, int TID)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }

            FOSDataModel db = new FOSDataModel();

            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
            DateTime final = end.AddDays(1);
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();


            try
            {

                List<Sp_OrderSummeryReportInExcelRangeWiseForDistributor1_1_Result> result = db.Sp_OrderSummeryReportInExcelRangeWiseForDistributor1_1(start, final,  6, TID).ToList();
                
               
                string SoName = "";
                List<SaleOfficer> SO = db.SaleOfficers.Where(u => u.ID == TID).ToList();
                foreach (var SOS in SO)
                {
                    SoName = SOS.Name;
                }
                string RangeName = "";
                List<MainCategory> range = db.MainCategories.Where(u => u.MainCategID == 6).ToList();
                foreach (var SOS in range)
                {
                    RangeName = SOS.MainCategDesc;
                }

              
                ReportParameter[] prm = new ReportParameter[4];
          
                prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                prm[1] = new ReportParameter("SOName", SoName);
                prm[2] = new ReportParameter("DateTo", EndingDate);
                prm[3] = new ReportParameter("DateFrom", StartingDate);
                
                ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\Report1.rdlc");
                ReportViewer1.EnableExternalImages = true;
                ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
                ReportViewer1.SetParameters(prm);
                ReportViewer1.DataSources.Clear();
                ReportViewer1.DataSources.Add(dt1);
                ReportViewer1.Refresh();



                Warning[] warnings;
                string[] streamIds;
                string contentType;
                string encoding;
                string extension;

                //Export the RDLC Report to Byte Array.
                byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                //Download the RDLC Report in Word, Excel, PDF and Image formats.
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;filename=D" + DateTime.Now + ".Pdf");
                Response.BinaryWrite(bytes);
                Response.Flush();

                Response.End();

            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

            ManagersLoginHst hst = new ManagersLoginHst();
            hst.UserID = userID;
            hst.IPAddress = remoteIpAddress;
            hst.ReportName = "Daily Sale Report For Distributor";
            hst.ReportType = "DealerSaleReport";
            hst.CreatedOn = DateTime.UtcNow.AddHours(5);
            db.ManagersLoginHsts.Add(hst);
            db.SaveChanges();


        }

        public ActionResult ShopsNotVisitedBySORpt()
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionData> RegionObj = ManageRegion.GetRegionDataList(userID);
            var objRegion = RegionObj.FirstOrDefault();
            List<CityData> cityObj = FOS.Setup.ManageRegion.GetCitiesList(objRegion.ID);
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            var objRegionalHead = regionalHeadData.FirstOrDefault();
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(objRegionalHead.ID, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
            });


            var objArea = new AreaData();
            objArea.Regions = RegionObj;
            objArea.SaleOfficersFroms = SaleOfficerObj;
            objArea.RegionalHeads = regionalHeadData;
            objArea.Cities = cityObj;
            objArea.Range = ranges;
            return View(objArea);
        }
        public void ShopsNotVisitedBySORptDetail(string StartingDate, string EndingDate,int RegionID ,int CityID,int RangeID ,int intSaleOfficerIDfrom)
        {


            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            FOSDataModel data = new FOSDataModel();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);

            List<sp_ShopsNotVisitedBySo1_3_Result> result = data.sp_ShopsNotVisitedBySo1_3(intSaleOfficerIDfrom, CityID, start, end,RangeID).ToList();

            StringWriter sw = new StringWriter();

            sw.WriteLine("\"SrNo.\",\"Shops\",\"Town\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=ShopsNotVisited" + DateTime.Now + ".csv");
            Response.ContentType = "application/octet-stream";

            //var retailers = ManageRetailer.GetRetailersForExportinExcel();
            int srNo = 1;
            foreach (var retailer in result)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\"",
                srNo,
                retailer.ShopName,
                retailer.CityName,

                srNo++


                ));
            }
            Response.Write(sw.ToString());
            Response.End();

            ManagersLoginHst hst = new ManagersLoginHst();
            hst.UserID = userID;
            hst.IPAddress = remoteIpAddress;
            hst.ReportName = "Shops not Visited by SO Report";
            hst.ReportType = "ShopsNotVisited";
            hst.CreatedOn = DateTime.UtcNow.AddHours(5);
            db.ManagersLoginHsts.Add(hst);
            db.SaveChanges();


        }
        public ActionResult ItemsNotSoldBySoRpt()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionData> RegionObj = ManageRegion.GetRegionDataList(userID);
            var objRegion = RegionObj.FirstOrDefault();
            List<Shared.MainCategories> Ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = Ranges.FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            var objhead = regionalHeadData.FirstOrDefault();
            List<SaleOfficer> Sos = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(objhead.ID, true);
            

            var objArea = new AreaData();
            objArea.Regions = RegionObj;
            objArea.SaleOfficersFroms = Sos;
            objArea.RegionalHeads = regionalHeadData;
            objArea.Range = Ranges;
            return View(objArea);
        }
        public void ItemsNotSoldBySoRptDetail(string StartingDate, string EndingDate,int RangeID ,int intSaleOfficerIDfrom)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }


            FOSDataModel data = new FOSDataModel();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);

            List<Sp_ItemsNotSoldBySo1_0_Result> result = data.Sp_ItemsNotSoldBySo1_0(intSaleOfficerIDfrom, RangeID,start, end).ToList();
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"SrNo.\",\"Range\",\"Brand\",\"ItemName\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=ItemsNotSold" + DateTime.Now + ".csv");
            Response.ContentType = "application/octet-stream";

            //var retailers = ManageRetailer.GetRetailersForExportinExcel();
            int srNo = 1;
            foreach (var retailer in result)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                srNo,
                retailer.MainCategdesc,
                retailer.SubCategDesc,
                retailer.ItemName,
                srNo++


                ));
            }
            Response.Write(sw.ToString());
            Response.End();

            ManagersLoginHst hst = new ManagersLoginHst();
            hst.UserID = userID;
            hst.IPAddress = remoteIpAddress;
            hst.ReportName = "Items Not Sold By So Report";
            hst.ReportType = "ItemsNotSoldBySoRpt";
            hst.CreatedOn = DateTime.UtcNow.AddHours(5);
            db.ManagersLoginHsts.Add(hst);
            db.SaveChanges();


        }
        public JsonResult GetSaleOfficersByRegionID(int RegionHeadID)
        {
            var result = FOS.Setup.ManageCity.GetSaleOfficersByRegionID(RegionHeadID);
            return Json(result);
        }

        public JsonResult getRegionalHeadsByRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.getRegionalHeadsByRegionID(RegionID);
            return Json(result);
        }


        public JsonResult getCitiesRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageRegion.GetCitiesList(RegionID);
            return Json(result);
        }
        #region Total Followup Report
        public ActionResult GetTotalFollowupReport()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            List<Shared.MainCategories> Ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = Ranges.FirstOrDefault();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
            });

            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Range = Ranges;
            return View(objJob);
        }
        public void GetAllFollowUpRetailer(string StartingDate, string EndingDate ,int HeadID, int fosid)
        {
          

            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }
            try
            {

                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);
                ManageRetailer objRetailers = new ManageRetailer();
                FOSDataModel db = new FOSDataModel();
                List<Sp_GetTotalFollowup1_3_Result> result = db.Sp_GetTotalFollowup1_3(start, final,HeadID,6 ,fosid).ToList();
                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SrNo.\",\"Shop Name\",\"Head Name\",\"SO Name\",\"City Name\",\"VisitPurpose\",\"Followup Reason\",\"Date\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=FollowUp" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                //   var retailers = ManageRetailer.GetRetailersForExportinExcel();
                int SrNo = 1;
                foreach (var retailer in result)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                    SrNo,
                    retailer.ShopName,
                    retailer.HeadName,
                    retailer.Saleofficer,
                    retailer.CityName,
                    retailer.FollowUp,
                    // retailer.Name,
                    retailer.FollowupReason,
                    retailer.JobDate,
                    SrNo++
                    // retailer.CustomerType



                    ));
                }
                Response.Write(sw.ToString());
                Response.End();

            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");
                // return null;
            }

            ManagersLoginHst hst = new ManagersLoginHst();
            hst.UserID = userID;
            hst.IPAddress = remoteIpAddress;
            hst.ReportName = "Follow Up Report";
            hst.ReportType = "Followup";
            hst.CreatedOn = DateTime.UtcNow.AddHours(5);
            db.ManagersLoginHsts.Add(hst);
            db.SaveChanges();


        }
        #endregion

        #region SovisitsFrequency
        public JsonResult GetSoRegionWise(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetSoRegionWise(RegionID);
            return Json(result);
        }
        public ActionResult SoVisitsFrequency()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionData> RegionObj = ManageRegion.GetRegionDataList(userID);
            var objRegion = RegionObj.FirstOrDefault();
            List<SaleOfficerData> Sos = FOS.Setup.ManageCity.GetSoRegionWise(objRegion.ID);


            var objArea = new AreaData();
            objArea.Regions = RegionObj;
            objArea.SaleOfficersFrom = Sos;

            return View(objArea);
        }
        public void SoVisitsFrequencyDetail(string StartingDate, string EndingDate, int intSaleOfficerIDfrom,int RegionID)
        {
            FOSDataModel data = new FOSDataModel();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);

            List<Sp_SoVisitsFrequency_Result> result = data.Sp_SoVisitsFrequency(RegionID,intSaleOfficerIDfrom, start, end).ToList();
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"SrNo.\",\"Saleofficer\",\"ShopName\",\"Date\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=SoVisitsFrequency" + DateTime.Now + ".csv");
            Response.ContentType = "application/octet-stream";

            //var retailers = ManageRetailer.GetRetailersForExportinExcel();
            int srNo = 1;
            foreach (var retailer in result)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                srNo,

                retailer.Name,
                retailer.ShopName,
                retailer.Date,
                srNo++


                ));
            }
            Response.Write(sw.ToString());
            Response.End();
        }
        #endregion SovisitsFrequency


        public ActionResult BrandWiseReport()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            List<Shared.MainCategories> CityObj = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);

            var objRegion = CityObj.FirstOrDefault();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }

            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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


            ////int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
           
            List<SubCategories> SubCategory = FOS.Setup.ManageCity.GetSubCatList(objRegion.ID);
            var objRegions = SubCategory.FirstOrDefault();
            List<Items> Items = FOS.Setup.ManageCity.GetItemsList(objRegion.ID, objRegions.SubCategoryID);
        



            var objJob = new JobsData();

            objJob.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType();
            objJob.SaleOfficer = SaleOfficerObj;
            objJob.Retailers = RetailerObj;
            objJob.RegionalHead = regionalHeadData;
            objJob.Regions = CityObj;
            objJob.SubCategory = SubCategory;
            objJob.itemList = Items;


            return View(objJob);
        }


        public void BrandWiseReportInExcel(int TID, int fosid, int Regionid, int cityid,int rangeid, string sdate, string edate, string ReportType)
        {

            var userID = Convert.ToInt32(Session["UserID"]);
            var remoteIpAddress = "";
            string hostName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ip in ipaddress)
            {
                remoteIpAddress = ip.ToString();
            }



            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(sdate) ? DateTime.Now.ToString() : sdate);
            DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(edate) ? DateTime.Now.ToString() : edate);
            DateTime final = end.AddDays(1);

            try
            {

                ManageRetailer objRetailers = new ManageRetailer();
                List<sp_BrandAndItemWiseReport_Result> result = objRetailers.BrandWisereports(start, final, Regionid,fosid,TID,cityid,rangeid);
               
                if (ReportType == "Excel")
                {
                    // Example data
                    StringWriter sw = new StringWriter();

                    sw.WriteLine("\"Regional Head Name\",\"Sales Officer Name\",\"Brand Name\",\"Item Name\",\"Quantity In PCS\",\"Quantity In CTN\"");

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=BrandWiseReport " + DateTime.Now + ".csv");
                    Response.ContentType = "application/octet-stream";

                    //var RegionalHead = db.RegionalHeadRegions.Where(x => x.RegionHeadID == Regionid).Select(x => x.RegionID).FirstOrDefault();



                    foreach (var retailer in result)
                    {
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",


                       // retailer.Name,
                       retailer.HeadName,
                        retailer.SaleOfficerName,
                        retailer.Brand,
                        retailer.ItemName,
                        retailer.TotalQuantity,
                        retailer.TotalQuantity




                        ));
                    }
                    Response.Write(sw.ToString());
                    Response.End();

                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Brand Wise Report Excel";
                    hst.ReportType = "BrandReport";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();


                }

                else
                {
                    try
                    {
                        

                        ReportParameter[] prm = new ReportParameter[3];

                        prm[0] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                        prm[1] = new ReportParameter("DateTo", edate);
                        prm[2] = new ReportParameter("DateFrom", sdate);
                        

                        ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\BrandWise.rdlc");
                        ReportViewer1.EnableExternalImages = true;
                        ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                        ReportViewer1.SetParameters(prm);
                        ReportViewer1.DataSources.Clear();
                        ReportViewer1.DataSources.Add(dt1);

                        ReportViewer1.Refresh();



                        Warning[] warnings;
                        string[] streamIds;
                        string contentType;
                        string encoding;
                        string extension;

                        //Export the RDLC Report to Byte Array.
                        byte[] bytes = ReportViewer1.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                        //Download the RDLC Report in Word, Excel, PDF and Image formats.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = contentType;
                        Response.AddHeader("content-disposition", "attachment;filename=BrandWiseReport" + DateTime.Now + ".Pdf");
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        Response.End();

                    }

                    catch (Exception exp)
                    {
                        Log.Instance.Error(exp, "Report Not Working");

                    }
                    ManagersLoginHst hst = new ManagersLoginHst();
                    hst.UserID = userID;
                    hst.IPAddress = remoteIpAddress;
                    hst.ReportName = "Brand Wise Report PDF";
                    hst.ReportType = "BrandReport";
                    hst.CreatedOn = DateTime.UtcNow.AddHours(5);
                    db.ManagersLoginHsts.Add(hst);
                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

        }

        public ActionResult ComplaintSummary()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            var ranges = FOS.Setup.ManageRegion.GetRangesRelatedToZSM(userID);
            var rangeid = ranges.Select(r => r.ID).FirstOrDefault();
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();

            if (userID == 1122)
            {
                regionalHeadData = ManageComplaint.GetRegionalHeads(userID);
            }
            else
            {
                regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            }
            //if (userID == 1)
            //{
            //    regionalHeadData.Insert(0, new RegionalHeadData
            //    {
            //        ID = 0,
            //        Name = "All"
            //    });
            //}
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }
            //List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0);
            List<SaleOfficer> SaleOfficerObj = FOS.Setup.ManageRegion.GetAllSOListRelatedtoregionalHeadID(regId, true);

            SaleOfficerObj.Insert(0, new SaleOfficer
            {
                ID = 0,
                Name = "All"
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
            objJob.Areas = ManageArea.GetAllAreaListByCityID(objJob.Cities.FirstOrDefault().ID);
            objJob.Range = ranges;
            return View(objJob);
        }

        public void ComplaintSummaryRpt(string StartingDate, string EndingDate, int TID, int fosid, string ReportType)
        {
            try
            {
                DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(StartingDate) ? DateTime.Now.ToString() : StartingDate);
                DateTime end = Convert.ToDateTime(string.IsNullOrEmpty(EndingDate) ? DateTime.Now.ToString() : EndingDate);
                DateTime final = end.AddDays(1);

                // Use Entity Framework to call stored procedure
                if (ReportType == "detail")
                {
                    List<usp_GetComplaintDetail_Result> result = new List<usp_GetComplaintDetail_Result>();

                    result = db.usp_GetComplaintDetail(start, final, fosid, TID).ToList();

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=ComplaintDetailReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                    Response.ContentType = "text/csv";

                    using (StringWriter sw = new StringWriter())
                    {
                        sw.WriteLine("\"ID\",\"Complaint Number\",\"Complaint Date\",\"SO Name\",\"Customer Name\",\"Phone No\",\"Dealer Name\",\"Product Description\",\"Product Nature\",\"Product Batch No\",\"Shaking Time\",\"Color Code\",\"Quantity\",\"Substrate Color\",\"Tinning Ratio\",\"No of Coasts Applied\",\"Time Duration Within Coats\",\"Thinning Solvant\",\"Applied Tool\",\"Remarks\",\"Type\",\"Attribute Name\",\"Type\",\"Attribute Name\",\"Application Cause\",\"Substrate Cause\",\"Quality of Product\",\"Varified\",\"Quality Review Remarks\",\"Updated At\"");

                        if (result.Count > 0)
                        {
                            foreach (var complaint in result)
                            {
                                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\",\"{23}\",\"{24}\",\"{25}\",\"{26}\",\"{27}\",\"{28}\",\"{29}\"",
                                    complaint.ComplaintID.ToString(),
                                    complaint.ComplaintNumber,
                                    complaint.ComplaintDate == null ? "" : complaint.ComplaintDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                    complaint.SONAME,
                                    complaint.CustomerName,
                                    complaint.Phone1,
                                    complaint.DealerName,
                                    complaint.ProductDescription,
                                    complaint.ProductNature,
                                    complaint.ProductbatchNo,
                                    complaint.ShakingTime,
                                    complaint.ColorCode,
                                    complaint.Quantity.ToString(),
                                    complaint.SubstrateColor,
                                    complaint.TinningRatio,
                                    complaint.NoOfCoatsApplied,
                                    complaint.TimeDurationWithinCoats,
                                    complaint.ThinningSolvent,
                                    complaint.AppliedTool,
                                    complaint.Remarks,
                                    complaint.SubstrateType,
                                    complaint.SubstrateAttributes,
                                    complaint.NatureType,
                                    complaint.NatureAttributes,
                                    complaint.ApplicationCaseName,
                                    complaint.SubstrateCaseName,
                                    complaint.QualityProductName,
                                    complaint.IsVerifiedName,
                                    complaint.QualityRemarks,
                                    complaint.QualityUpdateDate == null ? "" : complaint.QualityUpdateDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                                ));
                            }
                        }
                        else
                        {
                            sw.WriteLine("\"No complaints found for the selected criteria\"");
                        }

                        Response.Write(sw.ToString());
                    }

                    Response.End();
                }
                else
                {
                    List<sp_usp_GetComplaintSummary_Result> summary = new List<sp_usp_GetComplaintSummary_Result>();
                    summary = db.sp_usp_GetComplaintSummary(start, final, fosid, TID).ToList();

                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=ComplaintSummaryReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                    Response.ContentType = "text/csv";

                    using (StringWriter sw = new StringWriter())
                    {
                        sw.WriteLine("\"ID\",\"Complaint Number\",\"Complaint Date\",\"SO Name\",\"Customer Name\",\"Phone No\",\"Dealer Name\",\"Product Description\",\"Varified\",\"Quality Review Remarks\",\"Updated At\"");

                        if (summary.Count > 0)
                        {
                            foreach (var complaint in summary)
                            {
                                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",
                                    complaint.ComplaintID,
                                    complaint.ComplaintNumber,
                                    complaint.CreatedOn == null ? "" : complaint.CreatedOn.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                    complaint.SOName,
                                    complaint.CustomerName,
                                    complaint.Phone1,
                                    complaint.DealerName,
                                    complaint.ProductDescription,
                                    complaint.IsVerifiedName,
                                    complaint.Remarks,
                                    complaint.UpdatedDate == null ? "" : complaint.UpdatedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                                ));
                            }
                        }
                        else
                        {
                            sw.WriteLine("\"No complaints found for the selected criteria\"");
                        }

                        Response.Write(sw.ToString());
                    }

                    Response.End();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Complaint Summary Report Failed");
                Response.Write("Error generating report: " + exp.Message);
            }
        }

        #region Call Summary Detailed Report
        public ActionResult CallSummaryDetailedReport()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            List<RegionalHeadData> regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionaList(userID);
            if (userID == 1)
            {
                regionalHeadData.Insert(0, new RegionalHeadData
                {
                    ID = 0,
                    Name = "All"
                });
            }

            var objJob = new JobsData();
            objJob.RegionalHead = regionalHeadData;
            return View(objJob);
        }

        public void CallSummaryDetailedReportRpt(int TID, DateTime sdate, DateTime edate)
        {
            try
            {
                StringWriter sw = new StringWriter();
                sw.WriteLine("\"SrNo\",\"Caller Name\",\"Region\",\"Total SO\",\"Call Days\",\"Total Calls\",\"Calls per Day\",\"Owner\",\"Paint Contractor\",\"Const Contractor\",\"Ready to Paint\",\"Construction Stage Other\",\"Applying Brighto\",\"Competitor\",\"Other Site Status\",\"Call Attended\",\"Not Attended\",\"Wrong Data\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=CallSummaryReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                Response.ContentType = "application/octet-stream";

                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand("sp_GetCallSummaryReport_Simple", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = sdate.Date });
                    cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = edate.Date });
                    cmd.Parameters.Add(new SqlParameter("@RegionName", SqlDbType.Int) { Value = TID });

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        bool any = false;
                        while (reader.Read())
                        {
                            any = true;
                            sw.WriteLine(string.Format(
                                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\"",
                                reader["SrNo"],
                                reader["Caller Name"],
                                reader["Region"],
                                reader["Total SO"],
                                reader["Call Days"],
                                reader["Total Calls"],
                                reader["Calls per Day"],
                                reader["Owner"],
                                reader["Paint Contractor"],
                                reader["Const Contractor"],
                                reader["Ready to Paint"],
                                reader["ConstructionStageOther"],
                                reader["Applying Brighto"],
                                reader["Competitor"],
                                reader["OtherSiteStatus"],
                                reader["Call Attended"],
                                reader["Not Attended"],
                                reader["Wrong Data"]
                            ));
                        }
                        if (!any)
                        {
                            sw.WriteLine("\"No records found for the selected date range\"");
                        }
                    }
                }

                Response.Write(sw.ToString());
                Response.End();
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Call Summary Report Failed");
            }
        }
        #endregion

        #region SO Wise Target Allocation Report

        [HttpGet]
        public ActionResult SOWiseTargetAllocationReport()
        {
            var financialYears = db.Tbl_FinancialYear
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.ID)
                .Select(x => new FinancialYearListItem { ID = x.ID, Year = x.Year })
                .ToList();

            var userID = Convert.ToInt32(Session["UserID"]);
            var regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            var activeRHIds = new HashSet<int>(db.RegionalHeads.Where(x => x.IsActive && !x.IsDeleted).Select(x => x.ID));
            regionalHeadData = regionalHeadData.Where(r => r.ID != 0 && activeRHIds.Contains(r.ID)).ToList();

            var saleOfficers = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(0, false);
            saleOfficers.Insert(0, new SaleOfficer { ID = 0, Name = "All" });

            var model = new CityData
            {
                FinancialYears = financialYears,
                RegionalHeads = regionalHeadData,
                SaleOfficers = saleOfficers
            };
            return View(model);
        }

        public JsonResult SOWiseTargetAllocationDataHandler(DTParameters param, int SOID, int FinancialYearID, int RegionalHeadID)
        {
            try
            {
                var dtsource = ManageCity.GetKPIForGrid(SOID, FinancialYearID, RegionalHeadID);

                var columnSearch = new List<string>();
                foreach (var col in param.Columns)
                    columnSearch.Add(col.Search.Value);

                var data = ManageCity.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);

                foreach (var itm in data)
                {
                    if (itm.LastUpdate.HasValue)
                        itm.FormattedDate = Convert.ToDateTime(itm.LastUpdate).ToString("dd-MM-yyyy");
                }

                int count = ManageCity.Count(param.Search.Value, dtsource, columnSearch);
                var result = new DTResult<CityData>
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

        public ActionResult ExportSOWiseTargetAllocationReport(int SOID, int FinancialYearID, int RegionalHeadID)
        {
            var data = ManageCity.GetKPIForGrid(SOID, FinancialYearID, RegionalHeadID);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\"Sr No\",\"Financial Year\",\"Regional Head\",\"SO Name\",\"Platinum\",\"Premium\",\"Gold\",\"Total\"");
            int sr = 1;
            foreach (var row in data)
            {
                sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                    sr++,
                    row.FinancialYearName,
                    row.RegionalHeadName,
                    row.SOName,
                    row.CoatingTarget,
                    row.ACTDTarget,
                    row.ReadyMixTarget,
                    row.TotalTarget));
            }
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "application/vnd.ms-excel", "SOWiseTargetAllocationReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
        }

        #endregion SO Wise Target Allocation Report

    }
}