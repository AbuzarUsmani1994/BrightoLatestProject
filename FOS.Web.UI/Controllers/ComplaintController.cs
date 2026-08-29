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

        public JsonResult ComplaintDataHandler(DTParameters param, int RegionalHeadID, int ProductNatureID)
        {
            try
            {
                var dtsource = new List<ComplaintData>();

                dtsource = ManageComplaint.GetComplaintDataForGrid(param.StartingDate1, param.StartingDate2, RegionalHeadID, ProductNatureID);


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
            ComplaintSuggesionAdded add = new ComplaintSuggesionAdded();
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
                add = db.ComplaintSuggesionAddeds.Where(x => x.ComplaintID == data.ComplaintID).FirstOrDefault();
                if (add == null)
                {
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
                    Dat.ImageUrl = Picture1;
                    Dat.VideoUrl = Video1;
                    Dat.CreatedDate = DateTime.Now;
                    Dat.UpdatedDate = DateTime.Now;
                    db.ComplaintSuggesionAddeds.Add(Dat);
                    db.SaveChanges();
                }
                else
                {
                    add.ComplaintID = data.ComplaintID;
                    add.ComplaintNo = data.ComplaintNumber;
                    add.ComplaintSubmitby = data.SaleOfficerName;
                    add.ComplaintSubmitDate = Convert.ToDateTime(data.StartingDate1);
                    add.ApplicationCauseId = ApplicationIds;
                    add.SubstrateCauseID = SubstrateIds;
                    add.ProductQualityRetailedID = QualityIds;
                    add.ApplicationCaseName = ApplicationName;
                    add.SubstrateCaseName = SubstrateName;
                    add.QualityProductName = QualityName;
                    add.Remarks = data.Remarks;
                    add.IsVerified = data.ComplaintYesNo;
                    if (data.ComplaintYesNo == 1)
                    {
                        add.IsVerifiedName = "Yes";
                    }
                    else if (data.ComplaintYesNo == 2)
                    {
                        add.IsVerifiedName = "No";
                    }
                    else
                    {
                        add.IsVerifiedName = "In Progress";
                    }
                    if (Picture1.Equals("") || Picture1 == null)
                    {
                        add.ImageUrl = add.ImageUrl;
                    }
                    else
                    {
                        add.ImageUrl = Picture1;
                    }
                    if (Video1.Equals("") || Video1 == null)
                    {
                        add.VideoUrl = add.VideoUrl;
                    }
                    else
                    {
                        add.VideoUrl = Video1;
                    }
                    add.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                }
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSuggestData(int ComplaintID)
        {
            ComplaintData complaint = new ComplaintData();
            using (FOSDataModel db = new FOSDataModel())
            {
                var data = db.ComplaintSuggesionAddeds.Where(x => x.ComplaintID == ComplaintID).FirstOrDefault();
                if (data != null)
                {
                    complaint.ComplaintID = (int)data.ComplaintID;
                    complaint.ComplaintNumber = data.ComplaintNo;
                    complaint.SaleOfficerName = data.ComplaintSubmitby;
                    complaint.StartingDate1 = data.ComplaintSubmitDate.Value.ToString("dd-MMM-yyyy");
                    complaint.AppID = data.ApplicationCauseId;
                    complaint.SubsID = data.SubstrateCauseID;
                    complaint.QuaID = data.ProductQualityRetailedID;
                    complaint.Remarks = data.Remarks;
                    complaint.ComplaintYesNo = (int)data.IsVerified;
                    complaint.ApplicationName = data.ApplicationCaseName;
                    complaint.SubstrateName = data.SubstrateCaseName;
                    complaint.QualityName = data.QualityProductName;
                    complaint.ImageUrl = data.ImageUrl;
                    complaint.VideoUrl = data.VideoUrl;
                    complaint.IsVerifiedString = data.IsVerifiedName;
                }
                else
                {
                    complaint = null;
                }

            }

            return Json(complaint, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteComplaintSuggestion(int ComplaintID)
        {
            using (FOSDataModel db = new FOSDataModel())
            {
                var add = db.ComplaintSuggesionAddeds.Where(x => x.ComplaintID == ComplaintID).FirstOrDefault();
                if (add != null)
                {
                    db.ComplaintSuggesionAddeds.Remove(add);
                    db.SaveChanges();
                }
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

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

    }
}