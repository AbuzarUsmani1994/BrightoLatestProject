using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FOS.Web.UI.Controllers
{
    public class ComplaintController : Controller
    {

        #region Complaints THINGS

        [CustomAuthorize]
        //View
        public ActionResult List()
        {
            // Load RegionalHead Data ...

            var objSaleOffice = new ComplaintData();
            objSaleOffice.RegionalHead = FOS.Setup.ManageRegionalHead.GetRegionalHeadList();
            objSaleOffice.RegionalHeadTypeData = FOS.Setup.ManageRegion.GetRegionalHeadsType(1);
            objSaleOffice.SaleOfficers = FOS.Setup.ManageSaleOffice.GetSOByRegionalHeadId(objSaleOffice.RegionalHead.FirstOrDefault() != null ? objSaleOffice.RegionalHead.FirstOrDefault().ID : 0);
            objSaleOffice.Retailers = FOS.Setup.ManageRetailer.GetRetailerBySOID(objSaleOffice.SaleOfficers.FirstOrDefault() != null ? objSaleOffice.SaleOfficers.FirstOrDefault().ID : 0);
            objSaleOffice.Cities = new List<CityData>();
            objSaleOffice.Areas = new List<Area>();
            return View(objSaleOffice);
        }

        //Insert Update Region Method...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUpdateComplaint([Bind(Exclude = "TID,RegionalHead")] ComplaintData newSaleOfficer)
        {
            Boolean boolFlag = true;
            Boolean PhoneNumberFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {

                if (newSaleOfficer != null)
                {

                    //if (newSaleOfficer.ComplaintID == 0)
                    //{
                    //    SaleOfficerValidator validator = new SaleOfficerValidator();
                    //    results = validator.Validate(newSaleOfficer);
                    //    boolFlag = results.IsValid;
                    //}

                    //if (newSaleOfficer.Phone1 != null)
                    //    {
                    //        if (FOS.Web.UI.Common.NumberCheck.CheckSalesOfficerNumber1Exist(newSaleOfficer.ID, newSaleOfficer.Phone1 == null ? "" : newSaleOfficer.Phone1) == 1)
                    //        {
                    //            return Content("2");
                    //        }
                    //    }

                    //if (newSaleOfficer.Phone2 != null)
                    //    {
                    //        if (FOS.Web.UI.Common.NumberCheck.CheckSalesOfficerNumber2Exist(newSaleOfficer.ID, newSaleOfficer.Phone2 == null ? "" : newSaleOfficer.Phone2) == 1)
                    //        {
                    //            return Content("2");
                    //        }
                    //    }

                    //if (newSaleOfficer.Phone1 != null && newSaleOfficer.Phone2 != null)
                    //    {
                    //        if (FOS.Web.UI.Common.NumberCheck.CheckSalesOfficerNumberExist(newSaleOfficer.ID, newSaleOfficer.Phone1 == null ? "" : newSaleOfficer.Phone1, newSaleOfficer.Phone2 == null ? "" : newSaleOfficer.Phone2) == 1)
                    //        {
                    //            PhoneNumberFlag = false;
                    //        }
                    //    }

                    if (PhoneNumberFlag)
                    {
                        if (boolFlag)
                        {
                            if (ManageComplaint.AddUpdateComplaints(newSaleOfficer))
                            {
                                return Content("1");
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
                    else
                    {
                        return Content("2");
                    }
                }

                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }

        public JsonResult GetCityListByRegionHeadID(int ID)
        {
            var result = FOS.Setup.ManageCity.GetCityListByRegionHeadID(ID);
            return Json(result);
        }

        public JsonResult GetRegionalHeadAccordingToType(int RegionalHeadType)
        {
            var result = FOS.Setup.ManageSaleOffice.GetRegionalHeadAccordingToType(RegionalHeadType);
            return Json(result);
        }

        public JsonResult GetAreaListByCityID(int ID)
        {
            var result = FOS.Setup.ManageArea.GetAreaListByCityID(ID);
            return Json(result);
        }

        public JsonResult GetAreaForSaleOfficerEdit(int ID, int CityID)
        {
            var result = FOS.Setup.ManageArea.GetAreaListByCityIDEdit(ID, CityID);
            return Json(result);
        }

        //Get All Region Method...
        public JsonResult DataHandler(DTParameters param , int RegionalHeadType , int RegionalHeadID)
        {
            try
            {
                var dtsource = new List<ComplaintData>();

                dtsource = ManageComplaint.GetComplaintListForGrid();
                

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<ComplaintData> data = ManageComplaint.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageComplaint.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<ComplaintData> result = new DTResult<ComplaintData>
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
        public int DeleteComplaint(int ComplaintID)
        {
            return ManageComplaint.DeleteComplaint(ComplaintID);
        }

        #endregion TODOS

        #region ManageComplaint

        public ActionResult ManageComplaints()
        {
            ComplaintData data = new ComplaintData();
            data.RegionalHead = ManageComplaint.GetRegionalHeads(0);
            data.ProductNatures = ManageComplaint.GetProductNatureList();
            data.ApplicationCauses = ManageComplaint.GetApplicationCause();
            data.SubstrateCauses = ManageComplaint.GetSubstrateCause();
            data.ComplaintProducts = ManageComplaint.GetProductQualityCause();
            return View(data);
        }

        public JsonResult ComplaintDataHandler(DTParameters param, int RegionalHeadID, int ProductNatureID, int ComplaintStatusFilter = 0)
        {
            try
            {
                var dtsource = new List<ComplaintData>();

                dtsource = ManageComplaint.GetComplaintDataForGrid(param.StartingDate1, param.StartingDate2, RegionalHeadID, ProductNatureID, ComplaintStatusFilter);


                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<ComplaintData> data = ManageComplaint.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = ManageComplaint.Count(param.Search.Value, dtsource, columnSearch);
                DTResult<ComplaintData> result = new DTResult<ComplaintData>
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

        public JsonResult SaveManageComplaints(ComplaintData data)
        {
            string ApplicationName = string.Empty;
            string SubstrateName = string.Empty;
            string QualityName = string.Empty;
            string Picture1 = String.Empty;
            string fileName = string.Empty;
            string path = string.Empty;
            string FullPath = string.Empty;
            string Video1 = String.Empty;
            string VideofileName = string.Empty;
            string Videopath = string.Empty;
            string VideoFullPath = string.Empty;
            int Res = 1;
            string ApplicationIds = Request.Form["AppID"];
            string SubstrateIds = Request.Form["SubsID"];
            string QualityIds = Request.Form["QuaID"];
            string[] ApplicationSplitsIds = ApplicationIds.Split(',');
            string[] SubstrateSplitsIds = SubstrateIds.Split(',');
            string[] QualitySplitsIds = QualityIds.Split(',');

            using (FOSDataModel db = new FOSDataModel())
            {
                for (int i = 0; i < ApplicationSplitsIds.Length; i++)
                {
                    int AppsID = Convert.ToInt32(ApplicationSplitsIds[i]);
                    if (AppsID == 20)
                    {
                        if (string.IsNullOrEmpty(ApplicationName))
                        {
                            ApplicationName = data.OtherApplication;
                        }
                        else
                        {
                            ApplicationName += "," + data.OtherApplication;
                        }
                    }
                    else
                    {
                        var AppData = db.ComplaintApplicationCauses.Where(x => x.ID == AppsID).FirstOrDefault();
                        if (AppData != null)
                        {
                            if (string.IsNullOrEmpty(ApplicationName))
                            {
                                ApplicationName = AppData.AppName;
                            }
                            else
                            {
                                ApplicationName += "," + AppData.AppName;
                            }
                        }
                    }
                }

                for (int i = 0; i < SubstrateSplitsIds.Length; i++)
                {
                    int SubssID = Convert.ToInt32(SubstrateSplitsIds[i]);
                    if (SubssID == 17)
                    {
                        if (string.IsNullOrEmpty(ApplicationName))
                        {
                            SubstrateName = data.OtherSubstrate;
                        }
                        else
                        {
                            SubstrateName += "," + data.OtherSubstrate;
                        }
                    }
                    else
                    {
                        var SubData = db.ComplaintSubstrateCauses.Where(x => x.ID == SubssID).FirstOrDefault();
                        if (SubData != null)
                        {
                            if (string.IsNullOrEmpty(SubstrateName))
                            {
                                SubstrateName = SubData.SubName;
                            }
                            else
                            {
                                SubstrateName += "," + SubData.SubName;
                            }
                        }
                    }
                }

                for (int i = 0; i < QualitySplitsIds.Length; i++)
                {
                    int QualID = Convert.ToInt32(QualitySplitsIds[i]);
                    if (QualID == 28)
                    {
                        if (string.IsNullOrEmpty(ApplicationName))
                        {
                            QualityName = data.OtherQuality;
                        }
                        else
                        {
                            QualityName += "," + data.OtherQuality;
                        }
                    }
                    else
                    {
                        var QuaData = db.ComplaintProductQualityRelateds.Where(x => x.ID == QualID).FirstOrDefault();
                        if (QuaData != null)
                        {
                            if (string.IsNullOrEmpty(QualityName))
                            {
                                QualityName = QuaData.QuaName;
                            }
                            else
                            {
                                QualityName += "," + QuaData.QuaName;
                            }
                        }
                    }
                }

                var Files = Request.Files["ImageUrl"];
                var VideoFiles = Request.Files["VideoUrl"];
                if (Files != null)
                {
                    string Extension = Path.GetExtension(Files.FileName);
                    if (Extension.ToLower().Equals(".jpeg") || Extension.ToLower().Equals(".jpg") || Extension.ToLower().Equals(".png") || Extension.ToLower().Equals(".gif"))
                    {
                        fileName = Path.GetFileName(Files.FileName);
                        path = Server.MapPath("~/Images/ComPicture");
                        FullPath = Path.Combine(path, fileName);
                        Files.SaveAs(FullPath);
                        Picture1 = "/Images/ComPicture/" + fileName;
                    }
                }
                if (VideoFiles != null)
                {
                    string Extension = Path.GetExtension(VideoFiles.FileName);
                    if (Extension.ToLower().Equals(".mp4") || Extension.ToLower().Equals(".mov") || Extension.ToLower().Equals(".mkv") || Extension.Equals(".avi"))
                    {
                        VideofileName = Path.GetFileName(VideoFiles.FileName);
                        Videopath = Server.MapPath("~/Images/ComVideo");
                        VideoFullPath = Path.Combine(Videopath, VideofileName);
                        VideoFiles.SaveAs(VideoFullPath);
                        Video1 = "/Images/ComVideo/" + VideofileName;
                    }
                }
                int? StatusCode = null;
                string StatusCodeString = Request.Form["ComplaintStatus"];
                if (!string.IsNullOrEmpty(StatusCodeString))
                {
                    int ParsedStatus;
                    if (int.TryParse(StatusCodeString, out ParsedStatus))
                    {
                        StatusCode = ParsedStatus;
                    }
                }

                var Dat = new ComplaintSuggesionAdded();
                Dat.ComplaintID = data.ComplaintID;
                Dat.ComplaintNo = data.ComplaintNumber;
                Dat.ComplaintSubmitby = data.SaleOfficerName;
                Dat.ComplaintSubmitDate = Convert.ToDateTime(data.StartingDate1);
                Dat.ApplicationCauseId = ApplicationIds;
                Dat.SubstrateCauseID = SubstrateIds;
                Dat.ProductQualityRetailedID = QualityIds;
                Dat.ApplicationCaseName = ApplicationName;
                Dat.SubstrateCaseName = SubstrateName;
                Dat.QualityProductName = QualityName;
                Dat.Remarks = data.Remarks;
                Dat.IsVerified = data.ComplaintYesNo;
                if (data.ComplaintYesNo == 1)
                {
                    Dat.IsVerifiedName = "Yes";
                }
                else if (data.ComplaintYesNo == 2)
                {
                    Dat.IsVerifiedName = "No";
                }
                else
                {
                    Dat.IsVerifiedName = "In Progress";
                }
                Dat.ComplaintStatus = StatusCode;
                Dat.ComplaintStatusName = StatusCode == 1 ? "In Progress" : StatusCode == 2 ? "On Hold" : StatusCode == 3 ? "Resolved" : StatusCode == 4 ? "Duplicate" : null;
                Dat.ImageUrl = Picture1;
                Dat.VideoUrl = Video1;
                Dat.CreatedDate = DateTime.Now;
                Dat.UpdatedDate = DateTime.Now;
                db.ComplaintSuggesionAddeds.Add(Dat);
                db.SaveChanges();
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSuggestData(int ComplaintID)
        {
            List<ComplaintData> list = new List<ComplaintData>();
            using (FOSDataModel db = new FOSDataModel())
            {
                var rows = db.ComplaintSuggesionAddeds.Where(x => x.ComplaintID == ComplaintID)
                    .OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.ID).ToList();
                foreach (var data in rows)
                {
                    list.Add(new ComplaintData
                    {
                        ID = data.ID,
                        ComplaintID = (int)data.ComplaintID,
                        ComplaintNumber = data.ComplaintNo,
                        SaleOfficerName = data.ComplaintSubmitby,
                        StartingDate1 = data.ComplaintSubmitDate.HasValue ? data.ComplaintSubmitDate.Value.ToString("dd-MMM-yyyy") : null,
                        AppID = data.ApplicationCauseId,
                        SubsID = data.SubstrateCauseID,
                        QuaID = data.ProductQualityRetailedID,
                        Remarks = data.Remarks,
                        ComplaintYesNo = data.IsVerified ?? 0,
                        ApplicationName = data.ApplicationCaseName,
                        SubstrateName = data.SubstrateCaseName,
                        QualityName = data.QualityProductName,
                        ImageUrl = data.ImageUrl,
                        VideoUrl = data.VideoUrl,
                        IsVerifiedString = data.IsVerifiedName,
                        ComplaintStatus = data.ComplaintStatus,
                        ComplaintStatusString = data.ComplaintStatusName,
                        IsPublished = data.IsPublished
                    });
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteComplaintSuggestion(int ID)
        {
            using (FOSDataModel db = new FOSDataModel())
            {
                var add = db.ComplaintSuggesionAddeds.Where(x => x.ID == ID).FirstOrDefault();
                if (add != null)
                {
                    db.ComplaintSuggesionAddeds.Remove(add);
                    db.SaveChanges();
                }
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        // Only userID 1124 may publish a Quality Review entry - the Publish button
        // is hidden client-side for everyone else, but this is the actual gate: the
        // SO Complaint Summary/Detail reports and the SOComplaints.rdlc PDF report
        // only surface the latest PUBLISHED entry per complaint (see
        // AlterSP_usp_GetComplaintDetail_AddIsPublishedFilter.sql and siblings), so
        // this is what actually controls what reaches those reports.
        [HttpPost]
        public JsonResult PublishComplaintSuggestion(int ID)
        {
            int userId = 0;
            int.TryParse(Convert.ToString(Session["UserID"]), out userId);
            if (userId != 1124)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }

            using (FOSDataModel db = new FOSDataModel())
            {
                var add = db.ComplaintSuggesionAddeds.Where(x => x.ID == ID).FirstOrDefault();
                if (add != null)
                {
                    add.IsPublished = true;
                    db.SaveChanges();
                }
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        #region Quality Visit Form (distinct from the Quality Review entries above)

        // Complaint Nature reuses Tbl_ProductNature - the same lookup already behind the
        // "Nature *" filter on this page (Model.ProductNatures) and the mobile app's
        // Submit Complaint "Nature" dropdown (Solvent Based / Water Based).
        // Tbl_ProductNature/Tbl_ConditionOfSubstratee/Tbl_ProuctCategory are already in
        // the .edmx, so these three are plain EF reads. Pack Size is hardcoded directly
        // in the view instead of DB-backed, per your instruction - no lookup table for it.
        public JsonResult GetQualityVisitLookups()
        {
            // The global HandleErrorAttribute filter (FilterConfig.cs) would otherwise
            // swallow any exception here into a generic "An error occurred" page for a
            // GET request hit directly in the browser, and into a silently-failed AJAX
            // call from the Visit popup - surfacing the real message as JSON instead.
            try
            {
                using (var db = new FOSDataModel())
                {
                    var complaintNatures = db.Tbl_ProductNature.Where(x => x.IsActive == true)
                        .OrderBy(x => x.Name).Select(x => new { ID = x.ID, Name = x.Name }).ToList();
                    var substrateNatures = db.Tbl_ConditionOfSubstratee.Where(x => x.IsActive == true)
                        .OrderBy(x => x.Name).Select(x => new { ID = x.ID, Name = x.Name }).ToList();
                    var productCategories = db.Tbl_ProuctCategory.Where(x => x.IsActive == true)
                        .OrderBy(x => x.Name).Select(x => new { ID = x.ID, Name = x.Name }).ToList();

                    return Json(new { complaintNatures, substrateNatures, productCategories }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveComplaintQualityVisit()
        {
            try
            {
                int complaintId;
                int.TryParse(Request.Form["ComplaintID"], out complaintId);
                if (complaintId == 0)
                {
                    return Json("0");
                }

                string picture1Path = SaveQualityVisitPicture(Request.Files["Picture1"]);
                string picture2Path = SaveQualityVisitPicture(Request.Files["Picture2"]);

                int userId = 0;
                int.TryParse(Convert.ToString(Session["UserID"]), out userId);

                int newId;
                using (var db = new FOSDataModel())
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    INSERT INTO dbo.Tbl_ComplaintQualityVisit
                        (ComplaintID, CustomerName, SiteContactPerson, ComplaintNatureID, SubstrateNatureID,
                         ProductCategoryID, PackSize, VisitTeamMembers, NoOfVisits,
                         VisitDates, ComplaintStatus, SalePersonName, ComplaintSummary,
                         SiteObservationInvestigation, CorrectiveAndPreventiveActions, Conclusion,
                         Picture1, Picture2, CreatedBy)
                    VALUES
                        (@ComplaintID, @CustomerName, @SiteContactPerson, @ComplaintNatureID, @SubstrateNatureID,
                         @ProductCategoryID, @PackSize, @VisitTeamMembers, @NoOfVisits,
                         @VisitDates, @ComplaintStatus, @SalePersonName, @ComplaintSummary,
                         @SiteObservationInvestigation, @CorrectiveAndPreventiveActions, @Conclusion,
                         @Picture1, @Picture2, @CreatedBy);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ComplaintID", SqlDbType.Int) { Value = complaintId });
                    cmd.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.VarChar, 200) { Value = (object)Request.Form["CustomerName"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@SiteContactPerson", SqlDbType.VarChar, 200) { Value = (object)Request.Form["SiteContactPerson"] ?? DBNull.Value });
                    cmd.Parameters.Add(ToNullableIntParam("@ComplaintNatureID", Request.Form["ComplaintNatureID"]));
                    cmd.Parameters.Add(ToNullableIntParam("@SubstrateNatureID", Request.Form["SubstrateNatureID"]));
                    cmd.Parameters.Add(ToNullableIntParam("@ProductCategoryID", Request.Form["ProductCategoryID"]));
                    cmd.Parameters.Add(new SqlParameter("@PackSize", SqlDbType.VarChar, 100) { Value = (object)Request.Form["PackSize"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@VisitTeamMembers", SqlDbType.VarChar, 500) { Value = (object)Request.Form["VisitTeamMembers"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@NoOfVisits", SqlDbType.VarChar, 50) { Value = (object)Request.Form["NoOfVisits"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@VisitDates", SqlDbType.VarChar, 500) { Value = (object)Request.Form["VisitDates"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@ComplaintStatus", SqlDbType.VarChar, 100) { Value = (object)Request.Form["ComplaintStatus"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@SalePersonName", SqlDbType.VarChar, 200) { Value = (object)Request.Form["SalePersonName"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@ComplaintSummary", SqlDbType.NVarChar, -1) { Value = (object)Request.Form["ComplaintSummary"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@SiteObservationInvestigation", SqlDbType.NVarChar, -1) { Value = (object)Request.Form["SiteObservationInvestigation"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@CorrectiveAndPreventiveActions", SqlDbType.NVarChar, -1) { Value = (object)Request.Form["CorrectiveAndPreventiveActions"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Conclusion", SqlDbType.NVarChar, -1) { Value = (object)Request.Form["Conclusion"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Picture1", SqlDbType.VarChar, 500) { Value = (object)picture1Path ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Picture2", SqlDbType.VarChar, 500) { Value = (object)picture2Path ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = userId });

                    conn.Open();
                    newId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                return Json(new { success = true, id = newId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Empty/unselected dropdowns come through Request.Form as "" rather than null -
        // treated as NULL here rather than failing the int parse.
        private static SqlParameter ToNullableIntParam(string name, string rawValue)
        {
            int parsed;
            if (!string.IsNullOrEmpty(rawValue) && int.TryParse(rawValue, out parsed))
            {
                return new SqlParameter(name, SqlDbType.Int) { Value = parsed };
            }
            return new SqlParameter(name, SqlDbType.Int) { Value = DBNull.Value };
        }

        // Same extension whitelist as the existing Quality Review Image upload in
        // SaveManageComplaints, saved to its own folder so these don't mix with the
        // existing ~/Images/ComPicture quality-review pictures.
        private string SaveQualityVisitPicture(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return null;
            }

            string extension = Path.GetExtension(file.FileName).ToLower();
            if (!(extension.Equals(".jpeg") || extension.Equals(".jpg") || extension.Equals(".png") || extension.Equals(".gif")))
            {
                return null;
            }

            string fileName = Guid.NewGuid().ToString("N") + extension;
            string folder = Server.MapPath("~/Images/ComQualityVisit");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            file.SaveAs(Path.Combine(folder, fileName));
            return "/Images/ComQualityVisit/" + fileName;
        }

        // Plain POCO bound to ComplaintQualityVisit.rdlc's DataSet1 - field names here
        // must match the Field Names in that file exactly.
        private class ComplaintQualityVisitReportRow
        {
            public string ComplaintSite { get; set; }
            public string ComplaintNature { get; set; }
            public string SubstrateNature { get; set; }
            public string ComplaintProductName { get; set; }
            public string VisitedTeam { get; set; }
            public string SiteContactPerson { get; set; }
            public string NumberOfVisits { get; set; }
            public string DateOfVisits { get; set; }
            public string ComplaintStatus { get; set; }
            public string ComplaintSummary { get; set; }
            public string SiteObservationInvestigation { get; set; }
            public string CorrectiveAndPreventiveActions { get; set; }
            public string Conclusion { get; set; }
            public string PictureUrl { get; set; }
        }

        // Auto-downloaded right after a successful Submit Visit (see qvSave's success
        // handler in ManageComplaints.cshtml). Same RDLC + LocalReport pattern as the
        // existing DownloadPDFCompalintReport action above - no new PDF dependency.
        // Complaint Nature/Substrate Nature/Complaint Product Name show whatever was
        // actually selected in the popup (Tbl_ProductNature / Tbl_ConditionOfSubstratee /
        // Tbl_ProuctCategory), not necessarily matching any particular reference sample.
        public void DownloadComplaintQualityVisitReport(int id)
        {
            using (var db = new FOSDataModel())
            {
                string customerName = null, siteContactPerson = null, visitTeamMembers = null,
                    noOfVisits = null, visitDates = null, complaintStatus = null,
                    complaintSummary = null, siteObservation = null, correctiveActions = null,
                    conclusion = null, picture1 = null;
                int? complaintNatureId = null, substrateNatureId = null, productCategoryId = null;

                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT CustomerName, SiteContactPerson, ComplaintNatureID, SubstrateNatureID,
                           ProductCategoryID, VisitTeamMembers, NoOfVisits, VisitDates,
                           ComplaintStatus, ComplaintSummary, SiteObservationInvestigation,
                           CorrectiveAndPreventiveActions, Conclusion, Picture1
                    FROM dbo.Tbl_ComplaintQualityVisit WHERE ID = @ID", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = id });
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customerName = reader["CustomerName"] as string;
                            siteContactPerson = reader["SiteContactPerson"] as string;
                            complaintNatureId = reader["ComplaintNatureID"] as int?;
                            substrateNatureId = reader["SubstrateNatureID"] as int?;
                            productCategoryId = reader["ProductCategoryID"] as int?;
                            visitTeamMembers = reader["VisitTeamMembers"] as string;
                            noOfVisits = reader["NoOfVisits"] as string;
                            visitDates = reader["VisitDates"] as string;
                            complaintStatus = reader["ComplaintStatus"] as string;
                            complaintSummary = reader["ComplaintSummary"] as string;
                            siteObservation = reader["SiteObservationInvestigation"] as string;
                            correctiveActions = reader["CorrectiveAndPreventiveActions"] as string;
                            conclusion = reader["Conclusion"] as string;
                            picture1 = reader["Picture1"] as string;
                        }
                    }
                }

                string complaintNatureName = complaintNatureId.HasValue
                    ? db.Tbl_ProductNature.Where(x => x.ID == complaintNatureId.Value).Select(x => x.Name).FirstOrDefault()
                    : null;
                string substrateNatureName = substrateNatureId.HasValue
                    ? db.Tbl_ConditionOfSubstratee.Where(x => x.ID == substrateNatureId.Value).Select(x => x.Name).FirstOrDefault()
                    : null;
                string productCategoryName = productCategoryId.HasValue
                    ? db.Tbl_ProuctCategory.Where(x => x.ID == productCategoryId.Value).Select(x => x.Name).FirstOrDefault()
                    : null;

                string baseUrl = "http://" + HttpContext.Request.Url.Authority + "/";
                string pictureUrl = string.IsNullOrEmpty(picture1) ? baseUrl + "Images/NoImage.jpg" : baseUrl + picture1.TrimStart('/');

                var rows = new List<ComplaintQualityVisitReportRow>
                {
                    new ComplaintQualityVisitReportRow
                    {
                        ComplaintSite = customerName,
                        ComplaintNature = complaintNatureName,
                        SubstrateNature = substrateNatureName,
                        ComplaintProductName = productCategoryName,
                        VisitedTeam = visitTeamMembers,
                        SiteContactPerson = siteContactPerson,
                        NumberOfVisits = noOfVisits,
                        DateOfVisits = visitDates,
                        ComplaintStatus = complaintStatus,
                        ComplaintSummary = complaintSummary,
                        SiteObservationInvestigation = siteObservation,
                        CorrectiveAndPreventiveActions = correctiveActions,
                        Conclusion = conclusion,
                        PictureUrl = pictureUrl
                    }
                };

                var reportViewer = new LocalReport();
                reportViewer.ReportPath = Server.MapPath("~\\Views\\Reports\\ComplaintQualityVisit.rdlc");
                reportViewer.EnableExternalImages = true;
                reportViewer.DataSources.Clear();
                reportViewer.DataSources.Add(new ReportDataSource("DataSet1", rows));
                reportViewer.Refresh();

                Warning[] warnings;
                string[] streamIds;
                string contentType, encoding, extension;
                byte[] bytes = reportViewer.Render("PDF", null, out contentType, out encoding, out extension, out streamIds, out warnings);

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;filename=ComplaintVisitReport_" + id + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
        }

        #endregion

        public void DownloadPDFCompalintReport(string DateTO, string FromTO, int ComplaintID)
        {
            FOSDataModel db = new FOSDataModel();
            string StickerPic = "";
            string ComplaintPic = "";
            Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

            List<UnifiedComplaintResult> unifiedResults = new List<UnifiedComplaintResult>();

            using (var context = new FOSDataModel())
            {
                var command = context.Database.Connection.CreateCommand();
                command.CommandText = "[usp_GetComplaintDetailsUnified1.1]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ComplaintID", ComplaintID));
                context.Database.Connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new UnifiedComplaintResult();
                        //{
                        result.ComplaintID = Convert.ToInt32(reader["ComplaintID"]);
                        result.SONAME = reader["SONAME"] as string;
                        result.CustomerName = reader["CustomerName"] as string;
                        result.Phone1 = reader["Phone1"] as string;
                        result.DealerName = reader["DealerName"] as string;
                        result.DealerPhoneNo = reader["DealerPhoneNo"] as string;
                        result.ProductNature = reader["ProductNature"] as string;
                        result.ComplaintNumber = reader["ComplaintNumber"] as string;
                        result.ComplaintDate = reader["ComplaintDate"] as DateTime? ?? DateTime.Now;
                        result.ProductbatchNo = reader["ProductbatchNo"] as string;
                        result.ProductDescription = reader["ProductDescription"] as string;
                        result.ShakingTime = reader["ShakingTime"] as string;
                        result.ColorCode = reader["ColorCode"] as string;
                        result.Quantity = Convert.ToInt32(reader["Quantity"]);
                        result.SubstrateColor = reader["SubstrateColor"] as string;
                        result.TinningRatio = reader["TinningRatio"] as string;
                        result.NoOfCoatsApplied = reader["NoOfCoatsApplied"] as string;
                        result.TimeDurationWithinCoats = reader["TimeDurationWithinCoats"] as string;
                        result.ThinningSolvent = reader["ThinningSolvent"] as string;
                        result.AppliedTool = reader["AppliedTool"] as string;
                        result.Remarks = reader["Remarks"] as string;
                        result.Type = reader["Type"] as string;
                        result.AttributeName = reader["AttributeName"] as string;
                        result.StickerPicture = reader["StickerPicture"] as string;
                        result.ComplaintPicture = reader["ComplaintPicture"] as string;
                        result.IsChecked = Convert.ToBoolean(reader["IsChecked"]);
                        //};
                        unifiedResults.Add(result);
                    }
                }
            }

            var pictureData = unifiedResults.Select(x => new { x.StickerPicture, x.ComplaintPicture }).FirstOrDefault();

            var CurrentPath = HttpContext.Request.Url.Authority + "/";
            var path = "http://" + CurrentPath;
            if (!string.IsNullOrEmpty(pictureData.StickerPicture))
            {
                StickerPic = path + pictureData.StickerPicture;
            }
            else
            {
                StickerPic = path + "/Images/NoImage.jpg";
            }
            if (!string.IsNullOrEmpty(pictureData.ComplaintPicture))
            {
                ComplaintPic = path + pictureData.ComplaintPicture;
            }
            else
            {
                ComplaintPic = path + "/Images/NoImage.jpg";
            }

            List<Sp_GetComplaintVerifiedorNorbyQualtiy_Result> result1 = db.Sp_GetComplaintVerifiedorNorbyQualtiy(ComplaintID).ToList();

            foreach (var item in result1)
            {
                if (string.IsNullOrEmpty(item.QualityImageUrl))
                {
                    item.QualityImageUrl = path + "/Images/NoImage.jpg";
                }
                else
                {
                    item.QualityImageUrl = path + item.QualityImageUrl;
                }
            }
            string SONAME = unifiedResults.Select(x => x.SONAME).FirstOrDefault();

            ReportParameter[] prm = new ReportParameter[7];

            prm[0] = new ReportParameter("DistributorName", "Test");
            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
            prm[2] = new ReportParameter("SOName", SONAME);
            prm[3] = new ReportParameter("DateTo", Convert.ToDateTime(DateTO).ToString("dd-MMM-yyyy"));
            prm[4] = new ReportParameter("DateFrom", Convert.ToDateTime(FromTO).ToString("dd-MMM-yyyy"));
            prm[5] = new ReportParameter("StickerPic", StickerPic);
            prm[6] = new ReportParameter("ComplaintPic", ComplaintPic);

            ReportViewer1.ReportPath = Server.MapPath("~\\Views\\Reports\\SOComplaints.rdlc");
            ReportViewer1.EnableExternalImages = true;
            ReportDataSource dt1 = new ReportDataSource("DataSet1", unifiedResults);
            ReportDataSource dt2 = new ReportDataSource("DataSet2", result1);

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
            Response.AddHeader("content-disposition", "attachment;filename=BrandWiseReport" + DateTime.Now + ".Pdf");
            Response.BinaryWrite(bytes);
            Response.Flush();

            Response.End();
        }

        #endregion

        #region Commercial Decision (visible only to userID 1124)

        // Same product-category source as the Quality Visit form's lookups, reused
        // here rather than duplicating the query - only ProductCategories is used by
        // this form's JS.
        [HttpPost]
        public JsonResult SaveComplaintCommercialDecision()
        {
            int userId = 0;
            int.TryParse(Convert.ToString(Session["UserID"]), out userId);
            if (userId != 1124)
            {
                return Json(new { success = false, error = "Not authorized." });
            }

            try
            {
                int complaintId;
                int.TryParse(Request.Form["ComplaintID"], out complaintId);
                if (complaintId == 0)
                {
                    return Json(new { success = false, error = "No complaint selected." });
                }

                string picture1Path = SaveQualityVisitPicture(Request.Files["Picture1"]);
                string picture2Path = SaveQualityVisitPicture(Request.Files["Picture2"]);

                using (var db = new FOSDataModel())
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    INSERT INTO dbo.Tbl_ComplaintCommercialDecision
                        (ComplaintID, ProductCategoryID, PackSize, Quantity, Remarks, Picture1, Picture2, CreatedBy)
                    VALUES
                        (@ComplaintID, @ProductCategoryID, @PackSize, @Quantity, @Remarks, @Picture1, @Picture2, @CreatedBy)", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ComplaintID", SqlDbType.Int) { Value = complaintId });
                    cmd.Parameters.Add(ToNullableIntParam("@ProductCategoryID", Request.Form["ProductCategoryID"]));
                    cmd.Parameters.Add(new SqlParameter("@PackSize", SqlDbType.VarChar, 100) { Value = (object)Request.Form["PackSize"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.VarChar, 50) { Value = (object)Request.Form["Quantity"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.NVarChar, 1000) { Value = (object)Request.Form["Remarks"] ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Picture1", SqlDbType.VarChar, 500) { Value = (object)picture1Path ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Picture2", SqlDbType.VarChar, 500) { Value = (object)picture2Path ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = userId });

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion

    }
}