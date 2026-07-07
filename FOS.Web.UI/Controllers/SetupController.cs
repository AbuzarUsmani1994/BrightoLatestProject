using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;
using System.Drawing.Imaging;
using FoS.Web.UI.Report;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Web.Http.Results;

namespace FOS.Web.UI.Controllers
{
    public class SetupController : Controller
    {
        FOSDataModel dbContext = new FOSDataModel();
        #region SchemeInformation
        public JsonResult GetEditScheme(int SchemeID)
        {
            var Response = ManageScheme.GetEditScheme(SchemeID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public int DeleteScheme(int schemeID)
        {
            return FOS.Setup.ManageScheme.DeleteScheme(schemeID);
        }
        public JsonResult SchemeDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<SchemeData>();

                dtsource = ManageScheme.GetSchemesForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SchemeData> data = ManageScheme.GetSchemeResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageScheme.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<SchemeData> result = new DTResult<SchemeData>
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
        [CustomAuthorize]
        //View Work...
        public ActionResult SchemeInformation()
        {
            return View();
        }
        public JsonResult AddUpdateScheme(SchemeData tas)
        {

            try
            {
                //var serialize = JsonConvert.DeserializeObject<List<TblMasterScheme>>(cont);
                FOSDataModel dbContext = new FOSDataModel();
                ValidationResult results = new ValidationResult();
                //if (serialize != null)
                //{
                TblMasterScheme ms = new TblMasterScheme();
                if (tas.SchemeID == 0)
                {
                    //ms.RangeID = tas.RangeID;
                    ms.SchemeDateFrom = tas.SchemeDateFrom;
                    ms.SchemeDateTo = tas.SchemeDateTo;
                    ms.SchemeInfo = tas.SchemeInfo;
                    ms.isActive = true;
                    TblMasterScheme ms2 = dbContext.TblMasterSchemes.Where(x => x.isActive == true).FirstOrDefault();
                    if (ms2 != null)
                    {
                        ms2.isActive = false;
                    }
                    dbContext.TblMasterSchemes.Add(ms);
                    dbContext.SaveChanges();
                }
                else
                {
                    ms = dbContext.TblMasterSchemes.Where(u => u.MasterSchemeID == tas.SchemeID).FirstOrDefault();
                    ms.SchemeDateFrom = tas.SchemeDateFrom;
                    ms.SchemeDateTo = tas.SchemeDateTo;
                    ms.SchemeInfo = tas.SchemeInfo;
                    ms.isActive = true;
                    TblMasterScheme ms2 = dbContext.TblMasterSchemes.Where(x => x.isActive == true).FirstOrDefault();
                    if (ms2 != null)
                    {
                        ms2.isActive = false;
                    }
                    dbContext.SaveChanges();
                }
                //}
                return Json("0");
            }
            catch (Exception ex)
            {
                return Json("1");
            }


        }
        #endregion

        #region REGION

        [CustomAuthorize]
        //View Work...
        public ActionResult Region()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateRegion([Bind(Exclude = "TID")] RegionData newRegion)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRegion != null)
                {
                    if (newRegion.RegionID == 0)
                    {
                        RegionValidator validator = new RegionValidator();
                        results = validator.Validate(newRegion);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageRegion.AddUpdateRegion(newRegion);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult RegionDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<RegionData>();

                dtsource = ManageRegion.GetRegionForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RegionData> data = ManageRegion.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegion.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RegionData> result = new DTResult<RegionData>
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

        //Delete Region Method...
        public int DeleteRegion(int RegionID)
        {
            return FOS.Setup.ManageRegion.DeleteRegion(RegionID);
        }

        #endregion REGION

        #region CITY

        [CustomAuthorize]
        //View Work...
        public ActionResult City()
        {
            // Load Region Data For City Records ...
            var objCity = new CityData();
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            int THID = FOS.Web.UI.Controllers.AdminPanelController.GetTHIDRelatedToUser();
            if (THID > 0)
            {

                objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(THID);
            }
            else
            {
                objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(RHID);
            }



            return View(objCity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateCity([Bind(Exclude = "TID")] CityData newCity)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (ModelState.IsValid)
                {
                    if (newCity != null)
                    {
                        if (newCity.ID == 0)
                        {
                            CityValidator validator = new CityValidator();
                            results = validator.Validate(newCity);
                            boolFlag = results.IsValid;
                        }

                        if (boolFlag)
                        {
                            int Response = ManageCity.AddUpdateCity(newCity);
                            if (Response == 1)
                            {
                                return Content("1");
                            }
                            else if (Response == 2)
                            {
                                return Content("2");
                            }
                            else
                            {
                                return Content("0");
                            }
                        }
                        else
                        {
                            IList<ValidationFailure> failures = results.Errors;
                            StringBuilder sb = new StringBuilder();
                            sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                            foreach (ValidationFailure failer in results.Errors)
                            {
                                sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                                Response.StatusCode = 422;
                                return Json(new { errors = sb.ToString() });
                            }
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult CityDataHandler(DTParameters param, Int32 RegionID)
        {
            try
            {
                var dtsource = new List<CityData>();

                dtsource = ManageCity.GetCityForGrid(RegionID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<CityData> data = ManageCity.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageCity.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<CityData> result = new DTResult<CityData>
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

        //Delete Region Method...
        public int DeleteCity(int cityID)
        {
            return FOS.Setup.ManageCity.DeleteCity(cityID);
        }

        // Get One City For Edit
        public JsonResult GetEditCity(int CityID)
        {
            var Response = ManageCity.GetEditCity(CityID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        #endregion CITY

        #region AREA

        [CustomAuthorize]
        // View ...
        public ActionResult Area()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            List<RegionData> RegionObj = ManageRegion.GetRegionDataList(userID);
            var objRegion = RegionObj.FirstOrDefault();
            List<CityData> CityObj = FOS.Setup.ManageCity.GetCityListByRegionID(objRegion.ID);
            ViewData["CityObj"] = CityObj;

            var objArea = new AreaData();
            objArea.Regions = RegionObj;
            objArea.Cities = CityObj;

            return View(objArea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateArea([Bind(Exclude = "TID")] AreaData newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ID == 0)
                    {
                        AreaValidator validator = new AreaValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateArea(newData);
                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult AreaDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<AreaData>();

                dtsource = ManageArea.GetAreaForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<AreaData> data = ManageArea.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<AreaData> result = new DTResult<AreaData>
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

        public JsonResult GetCityListByRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionID(RegionID);
            return Json(result);
        }


        public JsonResult GetSOListByRegionID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetSOListByRegionID(RegionID);
            return Json(result);
        }


        //Delete Region...
        public int DeleteArea(int areaID)
        {
            return FOS.Setup.ManageArea.DeleteArea(areaID);
        }

        #endregion AREA


        #region KPIS

        [CustomAuthorize]
        //View Work...
        public ActionResult KPI()
        {
            var objCity = new CityData();
            int userID = Convert.ToInt32(Session["UserID"]);

            // Financial Years
            objCity.FinancialYears = new List<FinancialYearListItem>();
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand("SELECT ID, [Year] FROM dbo.Tbl_FinancialYear WHERE IsActive = 1 ORDER BY ID DESC", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        objCity.FinancialYears.Add(new FinancialYearListItem
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Year = reader["Year"].ToString()
                        });
                    }
                }
            }

            // Regional Heads (territory-scoped to current user) — active only
            var rhList = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            using (var rhConn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            {
                rhConn.Open();
                var activeIds = new HashSet<int>();
                using (var rhCmd = new SqlCommand("SELECT ID FROM dbo.RegionalHeads WHERE IsActive = 1 AND IsDeleted = 0", rhConn))
                using (var reader = rhCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        activeIds.Add(Convert.ToInt32(reader["ID"]));
                    }
                }
                rhList = rhList.Where(r => r.ID == 0 ? false : activeIds.Contains(r.ID)).ToList();
            }
            objCity.RegionalHeads = rhList;
            objCity.SaleOfficers = new List<SaleOfficer>();

            return View(objCity);
        }

        public JsonResult GetSOByRegionalHeadID(int RegionalHeadID)
        {
            var result = FOS.Setup.ManageSaleOffice.GetAllSaleOfficerListRelatedtoregionalHeadID(RegionalHeadID, false)
                .Select(x => new { ID = x.ID, Name = x.Name }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveKPISBulk(CityData newCity)
        {
            try
            {
                if (newCity == null || newCity.FinancialYearID == null || newCity.FinancialYearID == 0)
                    return Content("0");
                if (newCity.RegionalHeadID == null || newCity.RegionalHeadID == 0)
                    return Content("0");
                if (newCity.KpiRows == null || newCity.KpiRows.Count == 0)
                    return Content("0");

                int Response = ManageCity.AddKPIS(newCity);
                return Content(Response == 1 ? "1" : (Response == 2 ? "2" : "0"));
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateKPIS([Bind(Exclude = "TID")] CityData newCity)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
               
                    if (newCity != null)
                    {
                      

                        if (boolFlag)
                        {
                            int Response = ManageCity.AddKPIS(newCity);
                            if (Response == 1)
                            {
                                return Content("1");
                            }
                            else if (Response == 2)
                            {
                                return Content("2");
                            }
                            else
                            {
                                return Content("0");
                            }
                        }
                        else
                        {
                            IList<ValidationFailure> failures = results.Errors;
                            StringBuilder sb = new StringBuilder();
                            sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                            foreach (ValidationFailure failer in results.Errors)
                            {
                                sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                                Response.StatusCode = 422;
                                return Json(new { errors = sb.ToString() });
                            }
                        }
                    }
             

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult KPIDataHandler(DTParameters param, Int32 SOID, Int32 FinancialYearID, Int32 RegionalHeadID)
        {
            try
            {
                var dtsource = new List<CityData>();

                dtsource = ManageCity.GetKPIForGrid(SOID, FinancialYearID, RegionalHeadID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<CityData> data = ManageCity.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);

                foreach (var itm in data)
                {
                    if (itm.LastUpdate.HasValue)
                    {

                        itm.FormattedDate = Convert.ToDateTime(itm.LastUpdate).ToString("dd-MM-yyyy");
                        
                    }


                }

                int count = ManageCity.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<CityData> result = new DTResult<CityData>
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

        //Delete Region Method...
        //public int DeleteCity(int cityID)
        //{
        //    return FOS.Setup.ManageCity.DeleteCity(cityID);
        //}

        //// Get One City For Edit
        //public JsonResult GetEditCity(int CityID)
        //{
        //    var Response = ManageCity.GetEditCity(CityID);
        //    return Json(Response, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ExportKPIReport(int SOID, int FinancialYearID, int RegionalHeadID)
        {
            var data = ManageCity.GetKPIForGrid(SOID, FinancialYearID, RegionalHeadID);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\"Sr No\",\"Created Date\",\"Financial Year\",\"Head Name\",\"SO Name\",\"Platinum\",\"Premium\",\"Gold\",\"Total\"");
            int sr = 1;
            foreach (var row in data)
            {
                sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
                    sr++,
                    row.LastUpdate.HasValue ? row.LastUpdate.Value.ToString("dd-MMM-yyyy") : "",
                    row.FinancialYearName,
                    row.RegionalHeadName,
                    row.SOName,
                    row.CoatingTarget,
                    row.ACTDTarget,
                    row.ReadyMixTarget,
                    row.TotalTarget));
            }
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "application/vnd.ms-excel", "KPIReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
        }

        #endregion KPIS


        #region AttendanceAndPunc

        [CustomAuthorize]
        // View ...
        public ActionResult AttendanceAndPunctuality()
        {
            var objArea = BuildAreaSetupModel();
            return View(objArea);
        }

        private AreaData BuildAreaSetupModel()
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var regionalHeadData = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);

            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            {
                conn.Open();
                var activeIds = new HashSet<int>();
                using (var cmd = new SqlCommand("SELECT ID FROM dbo.RegionalHeads WHERE IsActive = 1 AND IsDeleted = 0", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        activeIds.Add(Convert.ToInt32(reader["ID"]));
                    }
                }
                regionalHeadData = regionalHeadData.Where(r => r.ID != 0 && activeIds.Contains(r.ID)).ToList();
            }

            var financialYears = new List<FinancialYearListItem>();
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand("SELECT ID, [Year] FROM dbo.Tbl_FinancialYear WHERE IsActive = 1 ORDER BY ID DESC", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        financialYears.Add(new FinancialYearListItem
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Year = reader["Year"].ToString()
                        });
                    }
                }
            }

            return new AreaData
            {
                RegionalHead = regionalHeadData,
                FinancialYears = financialYears,
                Salesofficerdata = new List<SaleOfficer>()
            };
        }

        [HttpPost]
        public ActionResult SaveAttendanceBulk(AreaData newData)
        {
            try
            {
                if (newData == null) return Content("0");
                if (newData.FinancialYearID == null || newData.FinancialYearID == 0) return Content("0");
                if (string.IsNullOrEmpty(newData.Quarter)) return Content("0");
                if (newData.RegionID == 0) return Content("0");
                if (newData.AttendanceRows == null || newData.AttendanceRows.Count == 0) return Content("0");

                int Response = ManageArea.AddUpdateAttendance(newData);
                return Content(Response == 1 ? "1" : (Response == 2 ? "2" : "0"));
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttendanceAndPunctuality([Bind(Exclude = "TID")] AreaData newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ID == 0)
                    {
                        AreaValidator validator = new AreaValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateAttendance(newData);
                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult AttendanceAndPunctualityDataHandler(DTParameters param, Int32 RegionalHeadID, Int32 FinancialYearID, string Quarter)
        {
            try
            {
                var dtsource = new List<AreaData>();

                dtsource = ManageArea.GetAttendanceForGrid(RegionalHeadID, FinancialYearID, Quarter);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<AreaData> data = ManageArea.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<AreaData> result = new DTResult<AreaData>
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

        public JsonResult GetAttendanceListByID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GetAttendanceListByRegionID(RegionID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteAttendanceRecord(int id)
        {
            try
            {
                var rec = dbContext.Tbl_SOAttendanceandPunctuality.Find(id);
                if (rec == null) return Json(new { success = false, message = "Record not found" });
                rec.IsActive = false;
                dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        public JsonResult GetAttendanceRecord(int id)
        {
            try
            {
                var rec = dbContext.Tbl_SOAttendanceandPunctuality.Find(id);
                if (rec == null) return Json(new { success = false });
                return Json(new { success = true, id = rec.ID, value = rec.AttendanceandPunctuality }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public JsonResult UpdateAttendanceRecord(int id, decimal value)
        {
            try
            {
                var rec = dbContext.Tbl_SOAttendanceandPunctuality.Find(id);
                if (rec == null) return Json(new { success = false, message = "Record not found" });
                rec.AttendanceandPunctuality = value;
                dbContext.Entry(rec).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost]
        public JsonResult DeleteTrainingRecord(int id)
        {
            try
            {
                var rec = dbContext.Tbl_SOTraining.Find(id);
                if (rec == null) return Json(new { success = false, message = "Record not found" });
                rec.IsActive = false;
                dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        public JsonResult GetTrainingRecord(int id)
        {
            try
            {
                var rec = dbContext.Tbl_SOTraining.Find(id);
                if (rec == null) return Json(new { success = false });
                return Json(new { success = true, id = rec.ID, value = rec.Training }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public JsonResult UpdateTrainingRecord(int id, decimal value)
        {
            try
            {
                var rec = dbContext.Tbl_SOTraining.Find(id);
                if (rec == null) return Json(new { success = false, message = "Record not found" });
                rec.Training = value;
                dbContext.Entry(rec).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost]
        public JsonResult DeleteKPIRecord(int id)
        {
            try
            {
                var rec = dbContext.Tbl_KPITargetsRegionWise.Find(id);
                if (rec == null) return Json(new { success = false, message = "Record not found" });
                rec.IsActive = false;
                dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        public JsonResult GetKPIRecord(int id)
        {
            try
            {
                var rec = dbContext.Tbl_KPITargetsRegionWise.Find(id);
                if (rec == null) return Json(new { success = false });
                return Json(new { success = true, id = rec.ID, platinum = rec.CoatingTarget, premium = rec.ACTDTarget, gold = rec.ReadyMixTarget, total = rec.TotalTarget }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public JsonResult UpdateKPIRecord(int id, int platinum, int premium, int gold, int total)
        {
            try
            {
                var rec = dbContext.Tbl_KPITargetsRegionWise.Find(id);
                if (rec == null) return Json(new { success = false, message = "Record not found" });
                rec.CoatingTarget  = platinum;
                rec.ACTDTarget     = premium;
                rec.ReadyMixTarget = gold;
                rec.TotalTarget    = total;
                dbContext.Entry(rec).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }


        //public JsonResult GetSOListByRegionID(int RegionID)
        //{
        //    var result = FOS.Setup.ManageCity.GetSOListByRegionID(RegionID);
        //    return Json(result);
        //}


        ////Delete Region...
        //public int DeleteArea(int areaID)
        //{
        //    return FOS.Setup.ManageArea.DeleteArea(areaID);
        //}

        public ActionResult ExportAttendanceReport(int RegionalHeadID, int FinancialYearID, string Quarter)
        {
            var data = ManageArea.GetAttendanceForGrid(RegionalHeadID, FinancialYearID, Quarter);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\"Sr No\",\"Created Date\",\"Financial Year\",\"Quarter\",\"Head Name\",\"SO Name\",\"Attendance Value\"");
            int sr = 1;
            foreach (var row in data)
            {
                sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                    sr++,
                    row.CreatedOn.HasValue ? row.CreatedOn.Value.ToString("dd-MMM-yyyy") : "",
                    row.FinancialYearName,
                    row.Quarter,
                    row.RegionName,
                    row.CityName,
                    row.Name));
            }
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "application/vnd.ms-excel", "AttendanceReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
        }

        public ActionResult ExportTrainingReport(int RegionalHeadID, int FinancialYearID, string Quarter)
        {
            var data = ManageArea.GetTrainingForGrid(RegionalHeadID, FinancialYearID, Quarter);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\"Sr No\",\"Created Date\",\"Financial Year\",\"Quarter\",\"Head Name\",\"SO Name\",\"Training Value\"");
            int sr = 1;
            foreach (var row in data)
            {
                sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                    sr++,
                    row.CreatedOn.HasValue ? row.CreatedOn.Value.ToString("dd-MMM-yyyy") : "",
                    row.FinancialYearName,
                    row.Quarter,
                    row.RegionName,
                    row.CityName,
                    row.Name));
            }
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "application/vnd.ms-excel", "TrainingReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
        }

        #endregion AttendanceAndPunc


        #region Training

        [CustomAuthorize]
        // View ...
        public ActionResult Training()
        {
            var objArea = BuildAreaSetupModel();
            return View(objArea);
        }

        [HttpPost]
        public ActionResult SaveTrainingBulk(AreaData newData)
        {
            try
            {
                if (newData == null) return Content("0");
                if (newData.FinancialYearID == null || newData.FinancialYearID == 0) return Content("0");
                if (string.IsNullOrEmpty(newData.Quarter)) return Content("0");
                if (newData.RegionID == 0) return Content("0");
                if (newData.TrainingRows == null || newData.TrainingRows.Count == 0) return Content("0");

                int Response = ManageArea.AddUpdateTraining(newData);
                return Content(Response == 1 ? "1" : (Response == 2 ? "2" : "0"));
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTraining([Bind(Exclude = "TID")] AreaData newData)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newData != null)
                {
                    if (newData.ID == 0)
                    {
                        AreaValidator validator = new AreaValidator();
                        results = validator.Validate(newData);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageArea.AddUpdateTraining(newData);
                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult AddTrainingDataHandler(DTParameters param, Int32 RegionalHeadID, Int32 FinancialYearID, string Quarter)
        {
            try
            {
                var dtsource = new List<AreaData>();

                dtsource = ManageArea.GetTrainingForGrid(RegionalHeadID, FinancialYearID, Quarter);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<AreaData> data = ManageArea.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<AreaData> result = new DTResult<AreaData>
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

        public JsonResult GetTrainingListByID(int RegionID)
        {
            var result = FOS.Setup.ManageCity.GettrainingListByRegionID(RegionID);
            return Json(result);
        }


        //public JsonResult GetSOListByRegionID(int RegionID)
        //{
        //    var result = FOS.Setup.ManageCity.GetSOListByRegionID(RegionID);
        //    return Json(result);
        //}


        ////Delete Region...
        //public int DeleteArea(int areaID)
        //{
        //    return FOS.Setup.ManageArea.DeleteArea(areaID);
        //}

        #endregion AttendanceAndPunc


        #region ProdKnowledgeCompFeed

        [CustomAuthorize]
        public ActionResult ProdKnowledge()
        {
            var objArea = BuildAreaSetupModel();
            return View(objArea);
        }

        [HttpPost]
        public ActionResult SaveProdKnowledgeBulk(AreaData newData)
        {
            try
            {
                if (newData == null
                    || newData.FinancialYearID == null || newData.FinancialYearID == 0
                    || string.IsNullOrEmpty(newData.Quarter)
                    || newData.RegionID == 0
                    || newData.ProdKnowledgeRows == null || newData.ProdKnowledgeRows.Count == 0)
                    return Content("0");

                var connStr = dbContext.Database.Connection.ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    var soIds = string.Join(",", newData.ProdKnowledgeRows.Select(r => r.SOID));
                    using (var cmd = new System.Data.SqlClient.SqlCommand(
                        "UPDATE dbo.Tbl_SOProdKnowledgeCompFeed SET IsActive = 0 " +
                        "WHERE FinancialYearID = @FY AND Quarter = @Q AND SOID IN (" + soIds + ")", conn))
                    {
                        cmd.Parameters.AddWithValue("@FY", newData.FinancialYearID);
                        cmd.Parameters.AddWithValue("@Q", newData.Quarter);
                        cmd.ExecuteNonQuery();
                    }

                    int nextId;
                    using (var cmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT ISNULL(MAX(ID), 0) + 1 FROM dbo.Tbl_SOProdKnowledgeCompFeed", conn))
                        nextId = (int)cmd.ExecuteScalar();

                    var now = DateTime.Now;
                    foreach (var r in newData.ProdKnowledgeRows)
                    {
                        using (var cmd = new System.Data.SqlClient.SqlCommand(@"
                            INSERT INTO dbo.Tbl_SOProdKnowledgeCompFeed
                                (ID, SOID, HeadID, ProductKnowledge, CompFeed, FinancialYearID, Quarter, IsActive, CreatedOn)
                            VALUES (@ID, @SOID, @HeadID, @PK, @CF, @FY, @Q, 1, @Now)", conn))
                        {
                            cmd.Parameters.AddWithValue("@ID", nextId++);
                            cmd.Parameters.AddWithValue("@SOID", r.SOID);
                            cmd.Parameters.AddWithValue("@HeadID", newData.RegionID);
                            cmd.Parameters.AddWithValue("@PK", (object)r.ProductKnowledge ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CF", (object)r.CompFeed ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@FY", newData.FinancialYearID);
                            cmd.Parameters.AddWithValue("@Q", newData.Quarter);
                            cmd.Parameters.AddWithValue("@Now", now);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return Content("1");
            }
            catch (Exception ex) { return Content("Exception: " + ex.Message); }
        }

        public JsonResult AddProdKnowledgeDataHandler(DTParameters param, int RegionalHeadID, int FinancialYearID, string Quarter)
        {
            try
            {
                var rows = new List<ProdKnowledgeGridRow>();
                var connStr = dbContext.Database.Connection.ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                using (var cmd = new System.Data.SqlClient.SqlCommand(@"
                    SELECT pk.ID,
                           ISNULL(rh.Name, '') AS HeadName,
                           ISNULL(so.Name, '') AS SOName,
                           ISNULL(fy.[Year], '') AS FinancialYearName,
                           ISNULL(pk.Quarter, '') AS Quarter,
                           ISNULL(pk.ProductKnowledge, 0) AS ProductKnowledge,
                           ISNULL(pk.CompFeed, 0) AS CompFeed,
                           pk.CreatedOn
                    FROM dbo.Tbl_SOProdKnowledgeCompFeed pk
                    LEFT JOIN dbo.SaleOfficers so ON so.ID = pk.SOID
                    LEFT JOIN dbo.RegionalHeads rh ON rh.ID = pk.HeadID
                    LEFT JOIN dbo.Tbl_FinancialYear fy ON fy.ID = pk.FinancialYearID
                    WHERE ISNULL(pk.IsActive, 1) = 1
                      AND (@HeadID = 0 OR pk.HeadID = @HeadID)
                      AND (@FY = 0 OR pk.FinancialYearID = @FY)
                      AND (@Q = '' OR pk.Quarter = @Q)
                    ORDER BY rh.Name, so.Name", conn))
                {
                    cmd.Parameters.AddWithValue("@HeadID", RegionalHeadID);
                    cmd.Parameters.AddWithValue("@FY", FinancialYearID);
                    cmd.Parameters.AddWithValue("@Q", Quarter ?? "");
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rows.Add(new ProdKnowledgeGridRow
                            {
                                ID               = Convert.ToInt32(reader["ID"]),
                                HeadName         = reader["HeadName"].ToString(),
                                SOName           = reader["SOName"].ToString(),
                                FinancialYearName= reader["FinancialYearName"].ToString(),
                                Quarter          = reader["Quarter"].ToString(),
                                ProductKnowledge = Convert.ToDecimal(reader["ProductKnowledge"]),
                                CompFeed         = Convert.ToDecimal(reader["CompFeed"]),
                                CreatedOn        = reader["CreatedOn"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToString("dd-MMM-yyyy")
                            });
                        }
                    }
                }

                int start  = param.Start;
                int length = param.Length > 0 ? param.Length : rows.Count;
                var paged  = rows.Skip(start).Take(length).ToList();
                return Json(new DTResult<ProdKnowledgeGridRow>
                {
                    draw            = param.Draw,
                    data            = paged,
                    recordsFiltered = rows.Count,
                    recordsTotal    = rows.Count
                });
            }
            catch (Exception ex) { return Json(new { error = ex.Message }); }
        }

        [HttpPost]
        public JsonResult DeleteProdKnowledgeRecord(int id)
        {
            try
            {
                var connStr = dbContext.Database.Connection.ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                using (var cmd = new System.Data.SqlClient.SqlCommand(
                    "UPDATE dbo.Tbl_SOProdKnowledgeCompFeed SET IsActive = 0 WHERE ID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        public JsonResult GetProdKnowledgeRecord(int id)
        {
            try
            {
                var connStr = dbContext.Database.Connection.ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                using (var cmd = new System.Data.SqlClient.SqlCommand(
                    "SELECT ID, ISNULL(ProductKnowledge,0) AS ProductKnowledge, ISNULL(CompFeed,0) AS CompFeed FROM dbo.Tbl_SOProdKnowledgeCompFeed WHERE ID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return Json(new { success = true, id = Convert.ToInt32(reader["ID"]), pk = reader["ProductKnowledge"], cf = reader["CompFeed"] }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public JsonResult UpdateProdKnowledgeRecord(int id, decimal pkValue, decimal cfValue)
        {
            try
            {
                var connStr = dbContext.Database.Connection.ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                using (var cmd = new System.Data.SqlClient.SqlCommand(
                    "UPDATE dbo.Tbl_SOProdKnowledgeCompFeed SET ProductKnowledge = @PK, CompFeed = @CF WHERE ID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@PK", pkValue);
                    cmd.Parameters.AddWithValue("@CF", cfValue);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        public class ProdKnowledgeGridRow
        {
            public int     ID                { get; set; }
            public string  HeadName          { get; set; }
            public string  SOName            { get; set; }
            public string  FinancialYearName { get; set; }
            public string  Quarter           { get; set; }
            public decimal ProductKnowledge  { get; set; }
            public decimal CompFeed          { get; set; }
            public string  CreatedOn         { get; set; }
        }

        public ActionResult ExportProdKnowledgeReport(int RegionalHeadID, int FinancialYearID, string Quarter)
        {
            var connStr = dbContext.Database.Connection.ConnectionString;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\"Sr No\",\"Financial Year\",\"Quarter\",\"Head Name\",\"SO Name\",\"Product Knowledge\",\"Competitor Activities\"");
            int sr = 1;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            using (var cmd = new System.Data.SqlClient.SqlCommand(@"
                SELECT ISNULL(fy.[Year],'') AS FY, ISNULL(pk.Quarter,'') AS Q,
                       ISNULL(rh.Name,'') AS Head, ISNULL(so.Name,'') AS SO,
                       ISNULL(pk.ProductKnowledge,0) AS PK, ISNULL(pk.CompFeed,0) AS CF
                FROM dbo.Tbl_SOProdKnowledgeCompFeed pk
                LEFT JOIN dbo.SaleOfficers so ON so.ID = pk.SOID
                LEFT JOIN dbo.RegionalHeads rh ON rh.ID = pk.HeadID
                LEFT JOIN dbo.Tbl_FinancialYear fy ON fy.ID = pk.FinancialYearID
                WHERE ISNULL(pk.IsActive,1)=1
                  AND (@HeadID=0 OR pk.HeadID=@HeadID)
                  AND (@FY=0 OR pk.FinancialYearID=@FY)
                  AND (@Q='' OR pk.Quarter=@Q)
                ORDER BY rh.Name, so.Name", conn))
            {
                cmd.Parameters.AddWithValue("@HeadID", RegionalHeadID);
                cmd.Parameters.AddWithValue("@FY", FinancialYearID);
                cmd.Parameters.AddWithValue("@Q", Quarter ?? "");
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                            sr++, reader["FY"], reader["Q"], reader["Head"], reader["SO"], reader["PK"], reader["CF"]));
                }
            }
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "ProdKnowledgeReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
        }

        #endregion ProdKnowledgeCompFeed



        //#region CITY

        //[CustomAuthorize]
        ////View Work...
        //public ActionResult Zone()
        //{
        //    // Load Region Data For City Records ...
        //    var objCity = new ZonesData();
        //    int RHID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
        //    objCity.Regions = FOS.Setup.ManageRegion.GetRegionList(RHID);
        //    objCity.Zones = FOS.Setup.ManageRegion.GetZones();
        //    return View(objCity);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddUpdateZone([Bind(Exclude = "TID")] ZonesData newCity)
        //{
        //    Boolean boolFlag = true;
        //    ValidationResult results = new ValidationResult();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (newCity != null)
        //            {
        //                if (newCity.ID == 0)
        //                {
        //                    zoneValidator validator = new zoneValidator();
        //                    results = validator.Validate(newCity);
        //                    boolFlag = results.IsValid;
        //                }

        //                if (boolFlag)
        //                {
        //                    int Response = ManageCity.AddUpdateZones(newCity);
        //                    if (Response == 1)
        //                    {
        //                        return Content("1");
        //                    }
        //                    else if (Response == 2)
        //                    {
        //                        return Content("2");
        //                    }
        //                    else
        //                    {
        //                        return Content("0");
        //                    }
        //                }
        //                else
        //                {
        //                    IList<ValidationFailure> failures = results.Errors;
        //                    StringBuilder sb = new StringBuilder();
        //                    sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
        //                    foreach (ValidationFailure failer in results.Errors)
        //                    {
        //                        sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
        //                        Response.StatusCode = 422;
        //                        return Json(new { errors = sb.ToString() });
        //                    }
        //                }
        //            }
        //        }

        //        return Content("0");
        //    }
        //    catch (Exception exp)
        //    {
        //        return Content("Exception : " + exp.Message);
        //    }
        //}

        ////Get All Region Method...
        //public JsonResult ZoneDataHandler(DTParameters param, Int32 RegionID)
        //{
        //    try
        //    {
        //        var dtsource = new List<ZonesData>();

        //        dtsource = ManageCity.GetZonesForGrid(RegionID);

        //        List<String> columnSearch = new List<string>();

        //        foreach (var col in param.Columns)
        //        {
        //            columnSearch.Add(col.Search.Value);
        //        }

        //        List<ZonesData> data = ManageCity.GetResult4(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
        //        int count = ManageCity.Count4(param.Search.Value, dtsource, columnSearch);
        //        DTResult<ZonesData> result = new DTResult<ZonesData>
        //        {
        //            draw = param.Draw,
        //            data = data,
        //            recordsFiltered = count,
        //            recordsTotal = count
        //        };
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}

        ////Delete Region Method...
        //public int DeleteZone(int cityID)
        //{
        //    return FOS.Setup.ManageCity.DeleteCity(cityID);
        //}

        //// Get One City For Edit
        //public JsonResult GetEditZone(int CityID)
        //{
        //    var Response = ManageCity.GetEditCity(CityID);
        //    return Json(Response, JsonRequestBehavior.AllowGet);
        //}

        //#endregion CITY


        #region ActivityPurpose

        [CustomAuthorize]
        //View Work...
        public ActionResult ActivityPurpose()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateActivityPurpose([Bind(Exclude = "TID")] PurposeOfActivityData newRegion)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRegion != null)
                {
                    if (newRegion.ID == 0)
                    {
                        ActivityPurposeValidator validator = new ActivityPurposeValidator();
                        results = validator.Validate(newRegion);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        int Response = ManageRegion.AddUpdateActivityPurpose(newRegion);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult ActivityPurposeDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<PurposeOfActivityData>();

                dtsource = ManageRegion.GetActivityPurposeForGrid();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<PurposeOfActivityData> data = ManageRegion.GetResult5(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegion.Count5(param.Search.Value, dtsource, columnSearch);
                DTResult<PurposeOfActivityData> result = new DTResult<PurposeOfActivityData>
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

        //Delete Region Method...
        public int DeleteActivityPurpose(int RegionID)
        {
            return FOS.Setup.ManageRegion.DeleteActivityPurpose(RegionID);
        }

        #endregion ActivityPurpose

        #region AccessGrid

        public JsonResult AccessDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<Tbl_AccessModel>();

                dtsource = ManageArea.GetAccessForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<Tbl_AccessModel> data = ManageArea.GetResult7(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count7(param.Search.Value, dtsource, columnSearch);
                DTResult<Tbl_AccessModel> result = new DTResult<Tbl_AccessModel>
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


        public void AccessDataHandler1( Int32 HeadID)
        {
            try
            {
                var dtsource = new List<Tbl_AccessModel>();

                dtsource = ManageArea.GetAccessForGrid(HeadID);

                List<String> columnSearch = new List<string>();


                // Example data
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"SrNO \",\"Head Name\",\"SaleOfficer Name\",\"Reported To\",\"Reported For\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=ReportingheirarchyRpt" + DateTime.Now + ".csv");
                Response.ContentType = "application/octet-stream";

                int srNo = 1;

                foreach (var retailer in dtsource)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",

                        srNo,
                     retailer.RegionName,
                      retailer.SaleOfficerName,
                    retailer.ReportedToName,
                      retailer.ReportedForName,

                    srNo++


                    ));
                }





                Response.Write(sw.ToString());
                Response.End();



         




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region SOREGIONSGrid

        public JsonResult SORegionsDataHandler(DTParameters param, Int32 CityID)
        {
            try
            {
                var dtsource = new List<Tbl_AccessModel>();

                dtsource = ManageArea.GetSORegionsForGrid(CityID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<Tbl_AccessModel> data = ManageArea.GetResult7(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageArea.Count7(param.Search.Value, dtsource, columnSearch);
                DTResult<Tbl_AccessModel> result = new DTResult<Tbl_AccessModel>
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


        #endregion SOREGIONSGrid


        #endregion ActivityPurpose


        [CustomAuthorize]
        public ActionResult SchemeInfo()
        {
            FOSDataModel db = new FOSDataModel();
            var objSaleOffice = new SchemeData();
            List<CategoryData> catData = new List<CategoryData>();
            catData = ManageCategory.GetCat();
            catData.Insert(0, new CategoryData
            {
                MainCategID = 0,
                MainCategDesc = "Select Range"
            });
            objSaleOffice.RangeData = catData;
            return View(objSaleOffice);

        }

        public JsonResult ItemDataHandler(DTParameters param, int? RangeID)
        {
            try
            {
                var dtsource = new List<Items>();

                dtsource = ManageItem.GetItemList(RangeID);


                return Json(dtsource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        //public JsonResult SubmitItem(string cont, SchemeData tas)
        //{

        //    try
        //    {
        //        var serialize = JsonConvert.DeserializeObject<List<Items>>(cont);
        //        FOSDataModel dbContext = new FOSDataModel();
        //        ValidationResult results = new ValidationResult();
        //        if (serialize != null)
        //        {
        //            var Schemeinfos = dbContext.TblMasterSchemes.Where(x => x.rangeID == tas.RangeID).ToList();
        //            foreach (var item in Schemeinfos)
        //            {
        //                item.isActive = false;
        //                item.isDeleted = true;
        //                dbContext.SaveChanges();
        //            }

        //            TblMasterScheme ms = new TblMasterScheme();
        //            TblDetailScheme ds = new TblDetailScheme();
        //            ms.rangeID = tas.RangeID;
        //            ms.SchemeDateFrom = tas.SchemeDateFrom;
        //            ms.SchemeDateTo = tas.SchemeDateTo;
        //            ms.SchemeInfo = tas.SchemeInfo;
        //            ms.isActive = true;
        //            ms.isDeleted = false;
        //            dbContext.TblMasterSchemes.Add(ms);
        //            dbContext.SaveChanges();
        //            ms.MasterSchemeID = dbContext.TblMasterSchemes.OrderByDescending(u => u.MasterSchemeID).Select(u => u.MasterSchemeID).FirstOrDefault();
        //            if (ms.MasterSchemeID > 0)
        //            {
        //                foreach (var items in serialize)
        //                {
        //                    ds.ItemID = items.ItemID;
        //                    ds.ItemName = items.ItemName;
        //                    ds.Packing = items.ItemPacking.ToString();
        //                    ds.TradePrice = items.ItemPrice.ToString();
        //                    ds.Scheme = items.Scheme;
        //                    if (items.SchemePrice == "")
        //                    {
        //                        ds.SchemePrice = 0;
        //                    }
        //                    else
        //                    {
        //                        ds.SchemePrice = Convert.ToInt32(items.SchemePrice);
        //                    }

        //                    ds.MasterID = ms.MasterSchemeID;
        //                    dbContext.TblDetailSchemes.Add(ds);
        //                    dbContext.SaveChanges();
        //                }
        //            }
        //        }
        //        return Json("1");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json("2");
        //    }


        //}


        //public JsonResult SubmitDealerDSRPrint(string cont, SchemeData tas)
        //{
        //    DateTime start = DateTime.Now;
        //    DateTime end= DateTime.Now;
        //    DateTime final= DateTime.Now;
        //    if (tas.SchemeDateFrom != null && tas.SchemeDateTo != null)
        //    {
        //         start = tas.SchemeDateFrom;
        //         end   = tas.SchemeDateTo;
        //         final = end.AddDays(1);
        //    }
        //    else
        //    {
        //        start = DateTime.UtcNow.AddHours(5);

        //        end = start.Date;
        //        final = end.AddDays(1);
        //    }



        //    FOSDataModel dbContext = new FOSDataModel();
        //    var rangeid = dbContext.Dealers.Where(x => x.ShopName == tas.DealerName && x.IsActive == true).FirstOrDefault();
        //    string FilePathReturn = "";
        //    string BillNo = "";
        //    var schemeData = new TblDetailScheme();
        //    var editdata = new DealerDispatchCalculation();
        //    var counter = dbContext.DealerDispatchCalculations.Where(x => x.DealerID==rangeid.ID).OrderByDescending(u => u.ID).Select(u => u.BillNo).FirstOrDefault();


        //    if (counter == null)
        //    {   var datein= DateTime.Now.Year.ToString();
        //        var ticketCount = 1;
        //        string s = ticketCount.ToString().PadLeft(3, '0');
        //        BillNo = rangeid.DealerCode+"-"+ datein  + "-" + s;
        //    }
        //    else
        //    {
        //        var datein = DateTime.Now.Year.ToString();
        //        var splittedcounter = counter.Split('-');
        //        var val = splittedcounter[2];
        //        int value = Convert.ToInt32(val) + 1;
        //        string s = value.ToString().PadLeft(3, '0');
        //        BillNo = rangeid.DealerCode + "-" + datein + "-" + s;
        //    }
        //    List<Items> Itemdata = new List<Items>();
        //    DealerDSRDispatch ms = new DealerDSRDispatch();
        //    DealerDispatchCalculation cal = new DealerDispatchCalculation();
        //    Items cty;
        //    var Orderid = 0;
        //    decimal GrossAmount = 0;
        //    decimal Scheme = 0;
        //    var masterID = dbContext.TblMasterSchemes.Where(x => x.rangeID == rangeid.RangeID && x.isActive == true).FirstOrDefault();
        //    try
        //    {
        //        var serialize = JsonConvert.DeserializeObject<List<Items>>(cont);

        //        var OrderIdData = serialize.Select(x => x.OrderID).FirstOrDefault();
        //        editdata = dbContext.DealerDispatchCalculations.Where(x => x.OrderID == OrderIdData).FirstOrDefault();
        //        if (editdata == null)
        //        {
        //            ValidationResult results = new ValidationResult();
        //            if (serialize != null)
        //            {

        //                foreach (var items in serialize)
        //                {
        //                    Orderid = items.OrderID;
        //                    cty = new Items();
        //                    ms.ItemID = items.ItemID;
        //                    ms.ItemName = items.ItemName;
        //                    ms.JobID = items.OrderID;
        //                    ms.OrderQuantity = items.OrderedQuan;
        //                    ms.SOID = tas.SOID;
        //                    ms.FromDate = start;
        //                    ms.ToDate = final;
        //                    if (items.Scheme == "")
        //                    {
        //                        ms.DispatchQuantity = items.OrderedQuan;
        //                    }
        //                    else
        //                    {
        //                        ms.DispatchQuantity = Convert.ToInt32(items.Scheme);
        //                    }

        //                    ms.Createddate = DateTime.UtcNow.AddHours(5);

        //                    cty.ItemID = items.ItemID;
        //                    var data = dbContext.Items.Where(x => x.ItemID == items.ItemID).FirstOrDefault();
        //                    if (masterID != null)
        //                    {
        //                        schemeData = dbContext.TblDetailSchemes.Where(x => x.MasterID == masterID.MasterSchemeID && x.ItemID == items.ItemID).FirstOrDefault();
        //                    }
        //                    cty.ItemName = items.ItemName;
        //                    cty.OrderID = items.OrderID;
        //                    cty.Packing = data.Packing;
        //                    if (schemeData.SchemePrice != null)
        //                    {
        //                        cty.Slab = "RS" +schemeData.SchemePrice  + "/" + data.Packing;

        //                    }
        //                    else
        //                    {
        //                        cty.Slab = "0";
        //                        cty.SchemeValue = 0;
        //                    }
        //                    cty.Rate = data.Price;


        //                    cty.OrderedQuan = items.OrderedQuan;
        //                    if (items.Scheme == "")
        //                    {
        //                        cty.DispatchQuantity = items.OrderedQuan;
        //                        cty.Value = data.Price * items.OrderedQuan;
        //                        cty.Amount = data.Price * items.OrderedQuan;
        //                        GrossAmount += data.Price * items.OrderedQuan;
        //                        if (schemeData.SchemePrice != 0)
        //                        {
        //                            double val = items.OrderedQuan / data.Packing;
        //                            int rounded = (int)Math.Round(val);
        //                            cty.SchemeValue = rounded * schemeData.SchemePrice;
        //                            Scheme += Convert.ToDecimal(cty.SchemeValue);
        //                        }
        //                        else
        //                        {
        //                            cty.Slab = "0";
        //                            cty.SchemeValue = 0;

        //                        }
        //                    }
        //                    else
        //                    {
        //                        cty.DispatchQuantity = Convert.ToInt32(items.Scheme);
        //                        cty.Value = data.Price * Convert.ToInt32(items.Scheme);
        //                        cty.Amount = data.Price * Convert.ToInt32(items.Scheme);
        //                        GrossAmount += data.Price * Convert.ToInt32(items.Scheme);
        //                        if (schemeData.SchemePrice != 0)
        //                        {
        //                            double val = Convert.ToInt32(items.Scheme) / data.Packing;
        //                            int rounded = (int)Math.Round(val);
        //                            cty.SchemeValue = rounded * schemeData.SchemePrice;
        //                            Scheme += Convert.ToDecimal(cty.SchemeValue);
        //                        }
        //                        else
        //                        {
        //                            cty.Slab = "0";
        //                            cty.SchemeValue = 0;
        //                        }
        //                    }

        //                    cty.Createddate = DateTime.UtcNow.AddHours(5);
        //                    Itemdata.Add(cty);
        //                    dbContext.DealerDSRDispatches.Add(ms);
        //                    dbContext.SaveChanges();


        //                }
        //            }
        //        }
        //        else
        //        {
        //            DealerDispatchCalculation obj = dbContext.DealerDispatchCalculations.Where(u => u.OrderID == OrderIdData).FirstOrDefault();
        //            dbContext.DealerDispatchCalculations.Remove(obj);
        //            var obj2 = dbContext.DealerDSRDispatches.Where(u => u.JobID == OrderIdData).ToList();
        //            foreach (var item in obj2)
        //            {
        //                dbContext.DealerDSRDispatches.Remove(item);
        //            }

        //            if (serialize != null)
        //            {

        //                foreach (var items in serialize)
        //                {
        //                    Orderid = items.OrderID;
        //                    cty = new Items();
        //                    ms.ItemID = items.ItemID;
        //                    ms.ItemName = items.ItemName;
        //                    ms.JobID = items.OrderID;
        //                    ms.OrderQuantity = items.OrderedQuan;
        //                    if (items.Scheme == "")
        //                    {
        //                        ms.DispatchQuantity = items.OrderedQuan;
        //                    }
        //                    else
        //                    {
        //                        ms.DispatchQuantity = Convert.ToInt32(items.Scheme);
        //                    }

        //                    ms.Createddate = DateTime.UtcNow.AddHours(5);
        //                    ms.SOID = tas.SOID;
        //                    cty.ItemID = items.ItemID;
        //                    var data = dbContext.Items.Where(x => x.ItemID == items.ItemID).FirstOrDefault();
        //                    if (masterID != null)
        //                    {
        //                        schemeData = dbContext.TblDetailSchemes.Where(x => x.MasterID == masterID.MasterSchemeID && x.ItemID == items.ItemID).FirstOrDefault();
        //                    }
        //                    cty.ItemName = items.ItemName;
        //                    cty.OrderID = items.OrderID;
        //                    cty.Packing = data.Packing;
        //                    if (schemeData.SchemePrice != null)
        //                    {
        //                        cty.Slab = schemeData.SchemePrice + "RS" + "/" + data.Packing;

        //                    }
        //                    else
        //                    {
        //                        cty.Slab = "0";
        //                        cty.SchemeValue = 0;
        //                    }
        //                    cty.Rate = data.Price;


        //                    cty.OrderedQuan = items.OrderedQuan;
        //                    if (items.Scheme == "")
        //                    {
        //                        cty.DispatchQuantity = items.OrderedQuan;
        //                        cty.Value = data.Price * items.OrderedQuan;
        //                        cty.Amount = data.Price * items.OrderedQuan;
        //                        GrossAmount += data.Price * items.OrderedQuan;
        //                        if (schemeData.SchemePrice != 0)
        //                        {
        //                            double val = items.OrderedQuan / data.Packing;
        //                            int rounded = (int)Math.Round(val);
        //                            cty.SchemeValue = rounded * schemeData.SchemePrice;
        //                            Scheme += Convert.ToDecimal(cty.SchemeValue);
        //                        }
        //                        else
        //                        {
        //                            cty.Slab = "0";
        //                            cty.SchemeValue = 0;

        //                        }
        //                    }
        //                    else
        //                    {
        //                        cty.DispatchQuantity = Convert.ToInt32(items.Scheme);
        //                        cty.Value = data.Price * Convert.ToInt32(items.Scheme);
        //                        cty.Amount = data.Price * Convert.ToInt32(items.Scheme);
        //                        GrossAmount += data.Price * Convert.ToInt32(items.Scheme);
        //                        if (schemeData.SchemePrice != 0)
        //                        {
        //                            double val = Convert.ToInt32(items.Scheme) / data.Packing;
        //                            int rounded = (int)Math.Round(val);
        //                            cty.SchemeValue = rounded * schemeData.SchemePrice;
        //                            Scheme += Convert.ToDecimal(cty.SchemeValue);
        //                        }
        //                        else
        //                        {
        //                            cty.Slab = "0";
        //                            cty.SchemeValue = 0;
        //                        }
        //                    }

        //                    cty.Createddate = DateTime.UtcNow.AddHours(5);
        //                    Itemdata.Add(cty);
        //                    dbContext.DealerDSRDispatches.Add(ms);
        //                    dbContext.SaveChanges();


        //                }
        //            }


        //        }
        //        cal.GrossAmount = GrossAmount;
        //        var display = GrossAmount * 3 / 100;
                
        //        cal.Display = Math.Round(display, 2);
        //        cal.OrderID = Orderid;
        //        var dis = GrossAmount * 2 / 100;
        //        var balamount1= GrossAmount - display;
        //        cal.Balanceamount1 = Math.Round(balamount1, 2);
        //        var balamo1 = cal.Balanceamount1;
        //        cal.Scheme = Scheme;
               
        //        cal.Balanceamount2 = balamo1 - dis;
        //        var balamo2 = cal.Balanceamount2;
               
        //        cal.WSDiscount = dis;
        //        cal.NetAmount = cal.Balanceamount2 - Scheme;
        //        cal.BillNo = BillNo;
        //        cal.DealerID = rangeid.ID;
        //        var Net = cal.Balanceamount2 - Scheme;
               
        //            dbContext.DealerDispatchCalculations.Add(cal);
        //            dbContext.SaveChanges();
            
        //        var changeStatus = dbContext.JobsDetails.Where(x => x.JobID == Orderid).FirstOrDefault();
        //        changeStatus.Dispatchstatus = "Invoiced";
        //        dbContext.SaveChanges();

        //        try
        //        {
        //            // Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new LocalReport();
        //            ReportViewer reportViewer = new ReportViewer();
        //            var DealerName = dbContext.JobsDetails.Where(x => x.JobID == Orderid).Select(x => x.RetailerID).FirstOrDefault();

        //            var ShopName = dbContext.Retailers.Where(x => x.ID == DealerName).FirstOrDefault();

        //            ReportParameter[] prm = new ReportParameter[12];


        //            prm[0] = new ReportParameter("DealerName", tas.DealerName + "/"+ rangeid.DealerCode );
        //            prm[1] = new ReportParameter("SOName", tas.SOName);
        //            prm[2] = new ReportParameter("ShopName", ShopName.ShopName);
        //            prm[3] = new ReportParameter("GrossAmount", GrossAmount.ToString());
        //            prm[4] = new ReportParameter("Display", display.ToString());
        //            prm[5] = new ReportParameter("Balance1", balamo1.ToString());
        //            prm[6] = new ReportParameter("Scheme", Scheme.ToString());
        //            prm[7] = new ReportParameter("Balance2", balamo2.ToString());
        //            prm[8] = new ReportParameter("Discount", dis.ToString());
        //            prm[9] = new ReportParameter("NetAmount", Net.ToString());
        //            prm[10] = new ReportParameter("Address", ShopName.Address);
        //            prm[11] = new ReportParameter("BillNo", BillNo);
        //            reportViewer.ProcessingMode = ProcessingMode.Local;
        //            reportViewer.LocalReport.ReportPath = Server.MapPath("~\\Views\\Reports\\DealerDSR.rdlc");
        //            reportViewer.LocalReport.EnableExternalImages = true;
        //            ReportDataSource dt1 = new ReportDataSource("DataSet1", Itemdata);

        //            reportViewer.LocalReport.SetParameters(prm);
        //            reportViewer.LocalReport.DataSources.Clear();
        //            reportViewer.LocalReport.DataSources.Add(dt1);
        //            reportViewer.LocalReport.Refresh();
        //            // PrintReport.PrintToPrinter(ReportViewer1);

        //            Warning[] warnings;
        //            string[] streamids;
        //            string mimeType, encoding, filenameExtension;

        //            byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

        //            //File  
        //            string FileName = "Test_" + DateTime.Now.Ticks.ToString() + ".pdf";
        //            string FilePath = HttpContext.Server.MapPath(@"~\TempFiles\") + FileName;

        //            //create and set PdfReader  
        //            PdfReader reader = new PdfReader(bytes);
        //            FileStream output = new FileStream(FilePath, FileMode.Create);

        //            string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

        //            //create and set PdfStamper  
        //            PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

        //            if (Agent.Contains("Firefox"))
        //                pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
        //            else
        //                pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

        //            pdfStamper.FormFlattening = false;
        //            pdfStamper.Close();
        //            reader.Close();

        //            //return file path  
        //            FilePathReturn = @"TempFiles/" + FileName;

        //        }

        //        catch (Exception exp)
        //        {
        //            Log.Instance.Error(exp, "Report Not Working");

        //        }



        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return Json(FilePathReturn);
        //}



        public JsonResult SubmitDealerDSRPrintForAll( SchemeData tas)
        {
            string FilePathReturn = "";
            FOSDataModel dbContext = new FOSDataModel();
            DateTime start = tas.SchemeDateFrom;
            DateTime end = tas.SchemeDateTo;
            DateTime final = end.AddDays(1);

          
                var Sodata = dbContext.Sp_GetAllDataForExportofSO(tas.SOID, start, final).ToList();

                var rangeid = dbContext.Dealers.Where(x => x.ShopName == tas.DealerName && x.IsActive == true).FirstOrDefault();
                
                string BillNo = "";
                var schemeData = new TblDetailScheme();
                var editdata = new DealerDispatchCalculation();

                List<Items> Itemdata = new List<Items>();
                DealerDSRDispatch ms = new DealerDSRDispatch();
                DealerDispatchCalculation cal = new DealerDispatchCalculation();
                //Items cty;
                //var Orderid = 0;
                //decimal GrossAmount = 0;
                //decimal Scheme = 0;
                var masterID = dbContext.TblMasterSchemes.Where(x => x.rangeID == rangeid.RangeID && x.isActive == true).FirstOrDefault();
                try
                {
                    // var serialize = JsonConvert.DeserializeObject<List<Items>>(cont);

                    // var OrderIdData = serialize.Select(x => x.OrderID).FirstOrDefault();
                    //editdata = dbContext.DealerDispatchCalculations.Where(x => x.OrderID == OrderIdData).FirstOrDefault();

                    ValidationResult results = new ValidationResult();


                    foreach (var items in Sodata)
                    {
                    var ID = dbContext.DealerDSRDispatches.Where(x => x.JobID == items.JobID && x.ItemID==items.ItemID).Select(x => x.JobID).ToList();
                    if (ID.Count == 0)
                    {
                        ms.ItemID = items.ItemID;
                        ms.ItemName = items.itemname;
                        ms.JobID = items.JobID;
                        ms.OrderQuantity = Convert.ToInt32(items.quantity);
                        ms.SOID = tas.SOID;

                        ms.DispatchQuantity = Convert.ToInt32(items.quantity);
                        ms.Createddate = DateTime.UtcNow.AddHours(5);

                        ms.FromDate = start;
                        ms.ToDate = final;

                        if (masterID != null)
                        {
                            schemeData = dbContext.TblDetailSchemes.Where(x => x.MasterID == masterID.MasterSchemeID && x.ItemID == items.ItemID).FirstOrDefault();
                        }



                        if (schemeData.SchemePrice != null)
                        {
                            ms.Slab = "RS" + schemeData.SchemePrice + "/" + items.packing;

                        }
                        else
                        {
                            ms.Slab = "0";
                            ms.Schemevalue = 0;
                        }




                        if (schemeData.SchemePrice != 0)
                        {
                            double val = Convert.ToDouble(items.quantity / items.packing);
                            int rounded = (int)Math.Round(val);
                            ms.Schemevalue = rounded * schemeData.SchemePrice;

                        }
                        else
                        {
                            ms.Slab = "0";
                            ms.Schemevalue = 0;

                        }





                        dbContext.DealerDSRDispatches.Add(ms);
                        dbContext.SaveChanges();

                    }
                    }

                    var CalData = dbContext.Sp_GetAllDataForExportofSOCalculations(tas.SOID, start, final).ToList();

                foreach (var cals in CalData)
                {
                    var Jobid = dbContext.DealerDispatchCalculations.Where(x => x.OrderID == cals.jobid).Select(x => x.OrderID).FirstOrDefault();
                    if (Jobid == null)
                    {

                        var counter = dbContext.DealerDispatchCalculations.Where(x => x.DealerID == rangeid.ID).OrderByDescending(u => u.ID).Select(u => u.BillNo).FirstOrDefault();


                        if (counter == null)
                        {
                            var datein = DateTime.Now.Year.ToString();
                            var ticketCount = 1;
                            string s = ticketCount.ToString().PadLeft(3, '0');
                            BillNo = rangeid.DealerCode + "-" + datein + "-" + s;
                        }
                        else
                        {
                            var datein = DateTime.Now.Year.ToString();
                            var splittedcounter = counter.Split('-');
                            var val = splittedcounter[2];
                            int value = Convert.ToInt32(val) + 1;
                            string s = value.ToString().PadLeft(3, '0');
                            BillNo = rangeid.DealerCode + "-" + datein + "-" + s;
                        }
                        cal.GrossAmount = cals.GrossAmount;
                        var display = cals.Display;

                        cal.Display = cals.Display;
                        cal.OrderID = cals.jobid;

                        cal.Balanceamount1 = cals.BalAmount1;



                        cal.Balanceamount2 = cals.balAmount2;
                        cal.WSDiscount = cals.WSDisplay;
                        if (cals.Scheme == null)
                        {
                            cal.Scheme = 0;
                            cal.NetAmount = cal.Balanceamount2 - 0;
                        }
                        else
                        {
                            cal.Scheme = cals.Scheme;
                            cal.NetAmount = cal.Balanceamount2 - cals.Scheme;
                        }

                        cal.BillNo = BillNo;
                        cal.DealerID = rangeid.ID;


                        dbContext.DealerDispatchCalculations.Add(cal);
                        dbContext.SaveChanges();
                        var changeStatus = dbContext.JobsDetails.Where(x => x.JobID == cals.jobid).FirstOrDefault();
                        changeStatus.Dispatchstatus = "Invoiced";
                        dbContext.SaveChanges();
                    }
                }
                }
                catch (Exception ex)
                {

                }
          


            try
            {

                var ItemData = dbContext.Sp_GetAllDataForExportofSOInReport(tas.SOID, start, final).ToList();
                //var ItemData2 = dbContext.Sp_GetAllDataForExportofSOCalculationsForReport(tas.SOID, start, final).ToList();
                // Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new LocalReport();
                ReportViewer reportViewer = new ReportViewer();
                //var DealerName = dbContext.JobsDetails.Where(x => x.JobID == Orderid).Select(x => x.RetailerID).FirstOrDefault();

                //var ShopName = dbContext.Retailers.Where(x => x.ID == DealerName).FirstOrDefault();

                //ReportParameter[] prm = new ReportParameter[12];


                //prm[0] = new ReportParameter("DealerName", tas.DealerName + "/" + rangeid.DealerCode);
                //prm[1] = new ReportParameter("SOName", tas.SOName);
                //prm[2] = new ReportParameter("ShopName", ShopName.ShopName);
                //prm[3] = new ReportParameter("GrossAmount", GrossAmount.ToString());
                //prm[4] = new ReportParameter("Display", display.ToString());
                //prm[5] = new ReportParameter("Balance1", balamo1.ToString());
                //prm[6] = new ReportParameter("Scheme", Scheme.ToString());
                //prm[7] = new ReportParameter("Balance2", balamo2.ToString());
                //prm[8] = new ReportParameter("Discount", dis.ToString());
                //prm[9] = new ReportParameter("NetAmount", Net.ToString());
                //prm[10] = new ReportParameter("Address", ShopName.Address);
                //prm[11] = new ReportParameter("BillNo", BillNo);
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.ReportPath = Server.MapPath("~\\Views\\Reports\\DealerDispatchAll.rdlc");
                reportViewer.LocalReport.EnableExternalImages = true;
                ReportDataSource dt1 = new ReportDataSource("DataSet1", ItemData);
                //ReportDataSource dt2 = new ReportDataSource("DataSet2", ItemData2);
                // reportViewer.LocalReport.SetParameters(prm);
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(dt1);
                //reportViewer.LocalReport.DataSources.Add(dt2);
                reportViewer.LocalReport.Refresh();
                // PrintReport.PrintToPrinter(ReportViewer1);

                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, filenameExtension;

                byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                //File  
                string FileName = "Test_" + DateTime.Now.Ticks.ToString() + ".pdf";
                string FilePath = HttpContext.Server.MapPath(@"~\TempFiles\") + FileName;

                //create and set PdfReader  
                PdfReader reader = new PdfReader(bytes);
                FileStream output = new FileStream(FilePath, FileMode.Create);

                string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

                //create and set PdfStamper  
                PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

                if (Agent.Contains("Firefox"))
                    pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
                else
                    pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

                pdfStamper.FormFlattening = false;
                pdfStamper.Close();
                reader.Close();

                //return file path  
                FilePathReturn = @"TempFiles/" + FileName;

            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Report Not Working");

            }

            return Json(FilePathReturn);

        }
            

            
     


        #region DelieveryBoys

        [CustomAuthorize]
        //View Work...
        public ActionResult DelieveryBoys()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateDelieveryBoys([Bind(Exclude = "TID")] RegionData newRegion)
        {
            
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRegion != null)
                {
                    if (newRegion.RegionID == 0)
                    {
                        RegionValidator validator = new RegionValidator();
                        results = validator.Validate(newRegion);
                        boolFlag = results.IsValid;
                    }

                    if (boolFlag)
                    {
                        var userID = Convert.ToInt32(Session["UserID"]);
                        var DealerID = dbContext.Users.Where(x => x.ID == userID).Select(x => x.DealerRefNo).FirstOrDefault();
                        int Response = ManageRegion.AddUpdateDelieveryBoys(newRegion,DealerID);

                        if (Response == 1)
                        {
                            return Content("1");
                        }
                        else if (Response == 2)
                        {
                            return Content("2");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        //Get All Region Method...
        public JsonResult DelieveryBoysDataHandler(DTParameters param)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var DealerID = dbContext.Users.Where(x => x.ID == userID).Select(x => x.DealerRefNo).FirstOrDefault();
            try
            {
                var dtsource = new List<RegionData>();

                dtsource = ManageRegion.GetDelieveryBoysForGrid(DealerID);

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<RegionData> data = ManageRegion.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageRegion.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<RegionData> result = new DTResult<RegionData>
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

       // Delete Region Method...
        public int DeleteDelieveryBoys(int RegionID)
        {
            return FOS.Setup.ManageRegion.DeleteBoys(RegionID);
        }

        #endregion DelieveryBoys

        #region FinancialYear

        [CustomAuthorize]
        public ActionResult FinancialYear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateFinancialYear(FinancialYearData model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Year))
                {
                    return Content("0");
                }

                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                {
                    conn.Open();

                    using (var checkCmd = new SqlCommand(
                        "SELECT COUNT(1) FROM dbo.Tbl_FinancialYear WHERE [Year] = @Year AND ID <> @ID", conn))
                    {
                        checkCmd.Parameters.Add(new SqlParameter("@Year", SqlDbType.NVarChar, 50) { Value = model.Year.Trim() });
                        checkCmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID });
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            return Content("2");
                        }
                    }

                    if (model.ID == 0)
                    {
                        using (var cmd = new SqlCommand(
                            "INSERT INTO dbo.Tbl_FinancialYear ([Year], CreatedOn, IsActive) VALUES (@Year, GETDATE(), 1)", conn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@Year", SqlDbType.NVarChar, 50) { Value = model.Year.Trim() });
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var cmd = new SqlCommand(
                            "UPDATE dbo.Tbl_FinancialYear SET [Year] = @Year WHERE ID = @ID", conn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@Year", SqlDbType.NVarChar, 50) { Value = model.Year.Trim() });
                            cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID });
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return Content("1");
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "AddUpdateFinancialYear Failed");
                return Content("Exception : " + exp.Message);
            }
        }

        public JsonResult FinancialYearDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<FinancialYearData>();

                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(
                    "SELECT ID, [Year], CreatedOn, IsActive FROM dbo.Tbl_FinancialYear WHERE IsActive = 1 ORDER BY ID DESC", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dtsource.Add(new FinancialYearData
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Year = reader["Year"].ToString(),
                                CreatedOn = reader["CreatedOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedOn"]),
                                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }

                string search = param.Search != null ? (param.Search.Value ?? string.Empty) : string.Empty;
                IEnumerable<FinancialYearData> filtered = dtsource;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    filtered = filtered.Where(x => (x.Year ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                int count = filtered.Count();
                List<FinancialYearData> data = filtered.Skip(param.Start).Take(param.Length).ToList();

                DTResult<FinancialYearData> result = new DTResult<FinancialYearData>
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

        public int DeleteFinancialYear(int ID)
        {
            try
            {
                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand("UPDATE dbo.Tbl_FinancialYear SET IsActive = 0 WHERE ID = @ID", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = ID });
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return 0;
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "DeleteFinancialYear Failed");
                return 1;
            }
        }

        public class FinancialYearData
        {
            public int ID { get; set; }
            public string Year { get; set; }
            public DateTime? CreatedOn { get; set; }
            public bool IsActive { get; set; }
        }

        #endregion FinancialYear

        #region Quarter

        [CustomAuthorize]
        public ActionResult QuarterSetup()
        {
            var model = new QuarterData();
            var fyList = new List<FinancialYearListItem>();
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand("SELECT ID, [Year] FROM dbo.Tbl_FinancialYear WHERE IsActive = 1 ORDER BY ID DESC", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fyList.Add(new FinancialYearListItem
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Year = reader["Year"].ToString()
                        });
                    }
                }
            }
            model.FinancialYears = fyList;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateQuarter(QuarterData model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Name) || model.FinancialYearID == 0)
                    return Content("0");
                if (model.StartDate == null || model.EndDate == null)
                    return Content("0");

                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (model.ID == 0)
                    {
                        using (var cmd = new SqlCommand(
                            "INSERT INTO dbo.Tbl_Quarters (Name, StartDate, EndDate, FinancialYearID, IsActive, IsDeleted, CreatedOn) VALUES (@Name, @StartDate, @EndDate, @FYID, 1, 0, GETDATE())", conn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = model.Name.Trim() });
                            cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = model.StartDate.Value });
                            cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = model.EndDate.Value });
                            cmd.Parameters.Add(new SqlParameter("@FYID", SqlDbType.Int) { Value = model.FinancialYearID });
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var cmd = new SqlCommand(
                            "UPDATE dbo.Tbl_Quarters SET Name=@Name, StartDate=@StartDate, EndDate=@EndDate, FinancialYearID=@FYID WHERE ID=@ID AND IsDeleted=0", conn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = model.Name.Trim() });
                            cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = model.StartDate.Value });
                            cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = model.EndDate.Value });
                            cmd.Parameters.Add(new SqlParameter("@FYID", SqlDbType.Int) { Value = model.FinancialYearID });
                            cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID });
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return Content("1");
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "AddUpdateQuarter Failed");
                return Content("Exception : " + exp.Message);
            }
        }

        [HttpPost]
        public JsonResult DeleteQuarterRecord(int id)
        {
            try
            {
                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand("UPDATE dbo.Tbl_Quarters SET IsDeleted=1, IsActive=0 WHERE ID=@ID", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = id });
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "DeleteQuarterRecord Failed");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult QuarterDataHandler(DTParameters param, int FinancialYearID)
        {
            try
            {
                var dtsource = new List<QuarterGridRow>();
                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(
                    @"SELECT q.ID, q.Name, q.StartDate, q.EndDate, q.FinancialYearID, q.CreatedOn,
                             fy.[Year] AS FinancialYearName
                      FROM dbo.Tbl_Quarters q
                      LEFT JOIN dbo.Tbl_FinancialYear fy ON fy.ID = q.FinancialYearID
                      WHERE q.IsDeleted = 0
                        AND (@FYID = 0 OR q.FinancialYearID = @FYID)
                      ORDER BY q.ID DESC", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@FYID", SqlDbType.Int) { Value = FinancialYearID });
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dtsource.Add(new QuarterGridRow
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                                StartDate = reader["StartDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDate"]),
                                EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EndDate"]),
                                FinancialYearID = Convert.ToInt32(reader["FinancialYearID"]),
                                FinancialYearName = reader["FinancialYearName"] as string,
                                CreatedOn = reader["CreatedOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedOn"])
                            });
                        }
                    }
                }

                string search = param.Search != null ? (param.Search.Value ?? string.Empty) : string.Empty;
                IEnumerable<QuarterGridRow> filtered = dtsource;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    filtered = filtered.Where(x =>
                        (x.Name ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                     || (x.FinancialYearName ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                int count = filtered.Count();
                var data = filtered.Skip(param.Start).Take(param.Length).ToList();
                return Json(new DTResult<QuarterGridRow>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult GetQuartersByFinancialYear(int financialYearID)
        {
            try
            {
                var list = new List<QuarterListItem>();
                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(
                    "SELECT ID, Name FROM dbo.Tbl_Quarters WHERE FinancialYearID=@FYID AND IsDeleted=0 AND IsActive=1 ORDER BY StartDate, Name", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@FYID", SqlDbType.Int) { Value = financialYearID });
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new QuarterListItem
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString()
                            });
                        }
                    }
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public class QuarterData
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int FinancialYearID { get; set; }
            public List<FinancialYearListItem> FinancialYears { get; set; }
        }

        public class QuarterGridRow
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int FinancialYearID { get; set; }
            public string FinancialYearName { get; set; }
            public DateTime? CreatedOn { get; set; }
        }

        public class QuarterListItem
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        #endregion Quarter

        #region AreaCoverage

        [CustomAuthorize]
        public ActionResult AreaCoverage()
        {
            var model = new AreaCoverageData();
            model.RegionalHeads = LoadActiveRegionalHeads();
            model.Regions = LoadAllRegions();
            return View(model);
        }

        [CustomAuthorize]
        public ActionResult AreaCoverageReport()
        {
            var model = new AreaCoverageData();
            model.RegionalHeads = LoadActiveRegionalHeads();
            return View(model);
        }

        public JsonResult GetAllRegionsList()
        {
            return Json(LoadAllRegions(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreasByRegionID(int RegionID)
        {
            var list = new List<DropDownItem>();
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand(
                @"SELECT a.ID, a.Name
                  FROM dbo.Areas a
                  WHERE a.RegionID = @RegionID AND a.IsDeleted = 0 AND a.IsActive = 1
                  ORDER BY a.Name", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@RegionID", SqlDbType.Int) { Value = RegionID });
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DropDownItem
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = reader["Name"] as string
                        });
                    }
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveAreaCoverage(AreaCoverageData model)
        {
            try
            {
                if (model == null) return Content("0");
                if (model.RegionalHeadID == null || model.RegionalHeadID == 0) return Content("0");
                if (model.SOID == null || model.SOID == 0) return Content("0");
                if (model.RegionID == null || model.RegionID == 0) return Content("0");
                if (model.AreaIDs == null || model.AreaIDs.Count == 0) return Content("0");

                int createdBy = 0;
                if (Session["UserID"] != null)
                    int.TryParse(Session["UserID"].ToString(), out createdBy);

                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                {
                    conn.Open();

                    var areaIds = model.AreaIDs.Distinct().ToList();

                    using (var tran = conn.BeginTransaction())
                    {
                        using (var deact = new SqlCommand(
                            "UPDATE dbo.Tbl_AreaCoverage SET IsActive = 0 WHERE SOID = @SOID AND RegionID = @RegionID AND IsActive = 1",
                            conn, tran))
                        {
                            deact.Parameters.Add(new SqlParameter("@SOID", SqlDbType.Int) { Value = model.SOID.Value });
                            deact.Parameters.Add(new SqlParameter("@RegionID", SqlDbType.Int) { Value = model.RegionID.Value });
                            deact.ExecuteNonQuery();
                        }

                        foreach (var areaId in areaIds)
                        {
                            using (var ins = new SqlCommand(
                                @"INSERT INTO dbo.Tbl_AreaCoverage (RegionalHeadID, SOID, RegionID, AreaID, CreatedOn, CreatedBy, IsActive)
                                  VALUES (@RHID, @SOID, @RegionID, @AreaID, GETDATE(), @CreatedBy, 1)", conn, tran))
                            {
                                ins.Parameters.Add(new SqlParameter("@RHID", SqlDbType.Int) { Value = model.RegionalHeadID.Value });
                                ins.Parameters.Add(new SqlParameter("@SOID", SqlDbType.Int) { Value = model.SOID.Value });
                                ins.Parameters.Add(new SqlParameter("@RegionID", SqlDbType.Int) { Value = model.RegionID.Value });
                                ins.Parameters.Add(new SqlParameter("@AreaID", SqlDbType.Int) { Value = areaId });
                                ins.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = createdBy });
                                ins.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                    }
                }
                return Content("1");
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "SaveAreaCoverage Failed");
                return Content("Exception : " + exp.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteAreaCoverage(int ID)
        {
            try
            {
                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand("UPDATE dbo.Tbl_AreaCoverage SET IsActive = 0 WHERE ID = @ID", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = ID });
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return Content("1");
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "DeleteAreaCoverage Failed");
                return Content("0");
            }
        }

        public JsonResult AreaCoverageDataHandler(DTParameters param, Int32 RegionalHeadID, Int32 SOID, Int32 RegionID)
        {
            try
            {
                var dtsource = new List<AreaCoverageGridRow>();
                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(
                    @"SELECT ac.ID, ac.RegionalHeadID, rh.Name AS RegionalHeadName,
                             ac.SOID, so.Name AS SOName,
                             ac.RegionID, r.Name AS RegionName,
                             ac.AreaID, a.Name AS AreaName,
                             ac.CreatedOn
                      FROM dbo.Tbl_AreaCoverage ac
                      LEFT JOIN dbo.RegionalHeads rh ON rh.ID = ac.RegionalHeadID
                      LEFT JOIN dbo.SaleOfficers so ON so.ID = ac.SOID
                      LEFT JOIN dbo.Regions r ON r.ID = ac.RegionID
                      LEFT JOIN dbo.Areas a ON a.ID = ac.AreaID
                      WHERE ac.IsActive = 1
                        AND (@RHID = 0 OR ac.RegionalHeadID = @RHID)
                        AND (@SOID = 0 OR ac.SOID = @SOID)
                        AND (@RegionID = 0 OR ac.RegionID = @RegionID)
                      ORDER BY ac.ID DESC", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@RHID", SqlDbType.Int) { Value = RegionalHeadID });
                    cmd.Parameters.Add(new SqlParameter("@SOID", SqlDbType.Int) { Value = SOID });
                    cmd.Parameters.Add(new SqlParameter("@RegionID", SqlDbType.Int) { Value = RegionID });
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dtsource.Add(new AreaCoverageGridRow
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                RegionalHeadName = reader["RegionalHeadName"] as string,
                                SOName = reader["SOName"] as string,
                                RegionName = reader["RegionName"] as string,
                                AreaName = reader["AreaName"] as string,
                                CreatedOn = reader["CreatedOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedOn"])
                            });
                        }
                    }
                }

                string search = param.Search != null ? (param.Search.Value ?? string.Empty) : string.Empty;
                IEnumerable<AreaCoverageGridRow> filtered = dtsource;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    filtered = filtered.Where(x =>
                        (x.RegionalHeadName ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                     || (x.SOName ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                     || (x.RegionName ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                     || (x.AreaName ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                int count = filtered.Count();
                var data = filtered.Skip(param.Start).Take(param.Length).ToList();
                return Json(new DTResult<AreaCoverageGridRow>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult AreaCoverageReportData(Int32 RegionalHeadID, Int32 SOID, Int32 RegionID)
        {
            // grouped by Region (Zone column) -> RH (Territory) -> SO with aggregated city list
            try
            {
                var rows = new List<AreaCoverageReportRow>();
                using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(
                    @"SELECT r.Name AS RegionName,
                             rh.Name AS TerritoryName,
                             so.ID AS SOID,
                             so.ECode AS ECode,
                             so.Name AS EmployeeName,
                             STUFF((
                                SELECT ', ' + a2.Name
                                FROM dbo.Tbl_AreaCoverage ac2
                                INNER JOIN dbo.Areas a2 ON a2.ID = ac2.AreaID
                                WHERE ac2.SOID = ac.SOID AND ac2.RegionID = ac.RegionID
                                  AND ac2.IsActive = 1
                                ORDER BY a2.Name
                                FOR XML PATH(''), TYPE
                             ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS AreaAllocation
                      FROM dbo.Tbl_AreaCoverage ac
                      LEFT JOIN dbo.Regions r ON r.ID = ac.RegionID
                      LEFT JOIN dbo.SaleOfficers so ON so.ID = ac.SOID
                      LEFT JOIN dbo.RegionalHeads rh ON rh.ID = ac.RegionalHeadID
                      WHERE ac.IsActive = 1
                        AND (@RHID = 0 OR ac.RegionalHeadID = @RHID)
                        AND (@SOID = 0 OR ac.SOID = @SOID)
                        AND (@RegionID = 0 OR ac.RegionID = @RegionID)
                      GROUP BY r.Name, rh.Name, so.ID, so.ECode, so.Name, ac.SOID, ac.RegionID
                      ORDER BY r.Name, rh.Name, so.Name", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@RHID", SqlDbType.Int) { Value = RegionalHeadID });
                    cmd.Parameters.Add(new SqlParameter("@SOID", SqlDbType.Int) { Value = SOID });
                    cmd.Parameters.Add(new SqlParameter("@RegionID", SqlDbType.Int) { Value = RegionID });
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        int sr = 0;
                        while (reader.Read())
                        {
                            rows.Add(new AreaCoverageReportRow
                            {
                                Sr = ++sr,
                                RegionName = reader["RegionName"] as string,
                                TerritoryName = reader["TerritoryName"] as string,
                                ECode = reader["ECode"] as string,
                                EmployeeName = reader["EmployeeName"] as string,
                                AreaAllocation = reader["AreaAllocation"] as string
                            });
                        }
                    }
                }
                return Json(rows, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<DropDownItem> LoadActiveRegionalHeads()
        {
            var list = new List<DropDownItem>();
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand("SELECT ID, Name FROM dbo.RegionalHeads WHERE IsActive = 1 AND IsDeleted = 0 ORDER BY Name", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DropDownItem
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = reader["Name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        private List<DropDownItem> LoadAllRegions()
        {
            var list = new List<DropDownItem>();
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand("SELECT ID, Name FROM dbo.Regions WHERE IsActive = 1 AND IsDeleted = 0 ORDER BY Name", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DropDownItem
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = reader["Name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public class DropDownItem
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public class AreaCoverageData
        {
            public int ID { get; set; }
            public int? RegionalHeadID { get; set; }
            public int? SOID { get; set; }
            public int? RegionID { get; set; }
            public List<int> AreaIDs { get; set; }
            public List<DropDownItem> RegionalHeads { get; set; }
            public List<DropDownItem> Regions { get; set; }
        }

        public class AreaCoverageGridRow
        {
            public int ID { get; set; }
            public string RegionalHeadName { get; set; }
            public string SOName { get; set; }
            public string RegionName { get; set; }
            public string AreaName { get; set; }
            public DateTime? CreatedOn { get; set; }
        }

        public class AreaCoverageReportRow
        {
            public int Sr { get; set; }
            public string RegionName { get; set; }
            public string TerritoryName { get; set; }
            public string ECode { get; set; }
            public string EmployeeName { get; set; }
            public string AreaAllocation { get; set; }
        }

        #endregion AreaCoverage


        #region KPIPerformanceReport

        [CustomAuthorize]
        public ActionResult KPIPerformanceReport()
        {
            var model = BuildKPIReportModel();
            return View(model);
        }

        public ActionResult ExportKPIPerformanceReport(int FinancialYearID, string Quarter, int RegionalHeadID, string ReportType = "value")
        {
            bool isPercent = (ReportType ?? "value").ToLower() == "percent";

            var sb = new System.Text.StringBuilder();
            sb.Append(@"<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel' xmlns='http://www.w3.org/TR/REC-html40'>
<head><meta charset='UTF-8'>
<!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>
<x:Name>KPI Report</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions>
</x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->
<style>
  td { border: 1px solid #999; font-size: 11pt; font-family: Calibri; vertical-align: middle; text-align: center; }
  .hdr1 { background-color: #1F3864; color: #FFFFFF; font-weight: bold; }
  .hdr2 { background-color: #2E75B6; color: #FFFFFF; font-weight: bold; }
  .left  { text-align: left; }
</style></head><body>
<table border='1' cellspacing='0' cellpadding='4'>
<tr>
  <td rowspan='2' class='hdr1'>Sr</td>
  <td rowspan='2' class='hdr1'>Head Name</td>
  <td rowspan='2' class='hdr1'>SO Name</td>
  <td class='hdr1'>Sales Target</td>
  <td class='hdr1'>Platinum</td>
  <td class='hdr1'>Premium</td>
  <td class='hdr1'>Dealer Visits</td>
  <td class='hdr1'>Site Visits</td>
  <td class='hdr1'>Contractor Visits</td>
  <td class='hdr1'>Customer Satisfaction</td>
  <td class='hdr1'>Area Coverage</td>
  <td class='hdr1'>Attendance</td>
  <td class='hdr1'>Product Knowledge</td>
  <td class='hdr1'>Training</td>
  <td class='hdr1'>Competitors Activities</td>
  <td class='hdr1'>Total</td>
</tr>
");
            string subHdr = isPercent ? "Target % / Actual %" : "Target / Actual";
            sb.AppendFormat(@"<tr>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
  <td class='hdr2'>{0}</td>
</tr>
", subHdr);

            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand(isPercent ? "dbo.usp_GetKPIPercentageReport" : "dbo.usp_GetKPIExcelReport", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
                cmd.Parameters.AddWithValue("@Quarter", Quarter ?? "");
                cmd.Parameters.AddWithValue("@RegionalHeadID", RegionalHeadID);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (isPercent)
                        {
                            sb.AppendFormat(@"<tr>
  <td>{0}</td>
  <td class='left'>{1}</td>
  <td class='left'>{2}</td>
  <td>{3:0.##}% / {4:0.##}%</td>
  <td>{5:0.##}% / {6:0.##}%</td>
  <td>{7:0.##}% / {8:0.##}%</td>
  <td>{9:0.##}% / {10:0.##}%</td>
  <td>{11:0.##}% / {12:0.##}%</td>
  <td>{13:0.##}% / {14:0.##}%</td>
  <td>{15:0.##}% / {16:0.##}%</td>
  <td>{17:0.##}% / {18:0.##}%</td>
  <td>{19:0.##}% / {20:0.##}%</td>
  <td>{21:0.##}% / {22:0.##}%</td>
  <td>{23:0.##}% / {24:0.##}%</td>
  <td>{25:0.##}% / {26:0.##}%</td>
  <td>{27:0.##}% / {28:0.##}%</td>
</tr>
",
                                Convert.ToInt32(reader["Sr"]),
                                reader["HeadName"], reader["SOName"],
                                Convert.ToDecimal(reader["SalesTargetPct"]),         Convert.ToDecimal(reader["SalesActualPct"]),
                                Convert.ToDecimal(reader["PlatinumTargetPct"]),      Convert.ToDecimal(reader["PlatinumActualPct"]),
                                Convert.ToDecimal(reader["PremiumTargetPct"]),       Convert.ToDecimal(reader["PremiumActualPct"]),
                                Convert.ToDecimal(reader["DealerVisitsTargetPct"]),  Convert.ToDecimal(reader["DealerVisitsActualPct"]),
                                Convert.ToDecimal(reader["SiteVisitsTargetPct"]),    Convert.ToDecimal(reader["SiteVisitsActualPct"]),
                                Convert.ToDecimal(reader["ContractorVisitsTargetPct"]), Convert.ToDecimal(reader["ContractorVisitsActualPct"]),
                                Convert.ToDecimal(reader["CustSatisfactionTargetPct"]), Convert.ToDecimal(reader["CustSatisfactionActualPct"]),
                                Convert.ToDecimal(reader["AreaCoverageTargetPct"]),  Convert.ToDecimal(reader["AreaCoverageActualPct"]),
                                Convert.ToDecimal(reader["AttendanceTargetPct"]),    Convert.ToDecimal(reader["AttendanceActualPct"]),
                                Convert.ToDecimal(reader["ProdKnowTargetPct"]),      Convert.ToDecimal(reader["ProdKnowActualPct"]),
                                Convert.ToDecimal(reader["TrainingTargetPct"]),      Convert.ToDecimal(reader["TrainingActualPct"]),
                                Convert.ToDecimal(reader["CompFeedTargetPct"]),      Convert.ToDecimal(reader["CompFeedActualPct"]),
                                Convert.ToDecimal(reader["TotalTargetPct"]),         Convert.ToDecimal(reader["TotalActualPct"]));
                        }
                        else
                        {
                            sb.AppendFormat(@"<tr>
  <td>{0}</td>
  <td class='left'>{1}</td>
  <td class='left'>{2}</td>
  <td>{3} / {4}</td>
  <td>{5} / {6}</td>
  <td>{7} / {8}</td>
  <td>{9} / {10}</td>
  <td>{11} / {12}</td>
  <td>{13} / {14}</td>
  <td>{15} / {16}</td>
  <td>{17} / {18}</td>
  <td>{19} / {20}</td>
  <td>{21} / {22}</td>
  <td>{23} / {24}</td>
  <td>{25} / {26}</td>
  <td>{27} / {28}</td>
</tr>
",
                                Convert.ToInt32(reader["Sr"]),
                                reader["HeadName"], reader["SOName"],
                                Convert.ToInt32(reader["SalesTarget"]),         Convert.ToInt32(reader["SalesActual"]),
                                Convert.ToInt32(reader["PlatinumTarget"]),      Convert.ToInt32(reader["PlatinumActual"]),
                                Convert.ToInt32(reader["PremiumTarget"]),       Convert.ToInt32(reader["PremiumActual"]),
                                Convert.ToInt32(reader["DealerVisitsTarget"]),  Convert.ToInt32(reader["DealerVisitsActual"]),
                                Convert.ToInt32(reader["SiteVisitsTarget"]),    Convert.ToInt32(reader["SiteVisitsActual"]),
                                Convert.ToInt32(reader["ContractorVisitsTarget"]), Convert.ToInt32(reader["ContractorVisitsActual"]),
                                Convert.ToInt32(reader["CustSatisfactionTarget"]), Convert.ToInt32(reader["CustSatisfactionActual"]),
                                Convert.ToInt32(reader["AreaCoverageTarget"]),  Convert.ToInt32(reader["AreaCoverageActual"]),
                                Convert.ToInt32(reader["AttendanceTarget"]),    Convert.ToInt32(reader["AttendanceActual"]),
                                Convert.ToInt32(reader["ProdKnowTarget"]),      Convert.ToInt32(reader["ProdKnowActual"]),
                                Convert.ToInt32(reader["TrainingTarget"]),      Convert.ToInt32(reader["TrainingActual"]),
                                Convert.ToInt32(reader["CompFeedTarget"]),      Convert.ToInt32(reader["CompFeedActual"]),
                                Convert.ToInt32(reader["TotalTarget"]),         Convert.ToInt32(reader["TotalActual"]));
                        }
                    }
                }
            }

            sb.Append("</table></body></html>");

            string suffix = isPercent ? "Percentage" : "Value";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "application/vnd.ms-excel", "KPIPerformanceReport_" + suffix + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
        }

        private KPIReportModel BuildKPIReportModel()
        {
            var financialYears = new List<FinancialYearListItem>();
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand("SELECT ID, [Year] FROM dbo.Tbl_FinancialYear WHERE IsActive = 1 ORDER BY ID DESC", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        financialYears.Add(new FinancialYearListItem { ID = Convert.ToInt32(reader["ID"]), Year = reader["Year"].ToString() });
                }
            }

            var userID = Convert.ToInt32(Session["UserID"]);
            var regionalHeads = FOS.Setup.ManageRegionalHead.GetTerritorialRegionalHeadList(userID);
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            {
                conn.Open();
                var activeIds = new HashSet<int>();
                using (var cmd = new SqlCommand("SELECT ID FROM dbo.RegionalHeads WHERE IsActive = 1 AND IsDeleted = 0", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) activeIds.Add(Convert.ToInt32(reader["ID"]));
                }
                regionalHeads = regionalHeads.Where(r => r.ID != 0 && activeIds.Contains(r.ID)).ToList();
            }

            return new KPIReportModel
            {
                FinancialYears = financialYears,
                RegionalHeads  = regionalHeads
            };
        }

        public class KPIReportModel
        {
            public List<FinancialYearListItem>    FinancialYears { get; set; }
            public List<RegionalHeadData>         RegionalHeads  { get; set; }
        }

        public class KPIReportRow
        {
            public int     Sr                     { get; set; }
            public string  HeadName               { get; set; }
            public string  SOName                 { get; set; }
            public int SalesTarget            { get; set; }
            public int SalesActual            { get; set; }
            public int PlatinumTarget         { get; set; }
            public int PlatinumActual         { get; set; }
            public int PremiumTarget          { get; set; }
            public int PremiumActual          { get; set; }
            public int DealerVisitsTarget     { get; set; }
            public int DealerVisitsActual     { get; set; }
            public int SiteVisitsTarget       { get; set; }
            public int SiteVisitsActual       { get; set; }
            public int ContractorVisitsTarget { get; set; }
            public int ContractorVisitsActual { get; set; }
            public int CustSatisfactionTarget { get; set; }
            public int CustSatisfactionActual { get; set; }
            public int AreaCoverageTarget     { get; set; }
            public int AreaCoverageActual     { get; set; }
            public int AttendanceTarget       { get; set; }
            public int AttendanceActual       { get; set; }
            public int ProdKnowTarget         { get; set; }
            public int ProdKnowActual         { get; set; }
            public int TrainingTarget         { get; set; }
            public int TrainingActual         { get; set; }
            public int CompFeedTarget         { get; set; }
            public int CompFeedActual         { get; set; }
            public int TotalTarget            { get; set; }
            public int TotalActual            { get; set; }
        }

        #endregion KPIPerformanceReport


    }
}
 

