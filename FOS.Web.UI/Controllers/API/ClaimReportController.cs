using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Web.Http;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using System.Web;
using System.Net.Http.Headers;

namespace FOS.Web.UI.Controllers.API
{
    public class ClaimReportController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        [HttpPost]
        public Result<SuccessResponse> Post(ClaimSummery rm)
        {
            // Retailer retailerObj = new Retailer();
            try
            {
                Microsoft.Reporting.WebForms.LocalReport ReportViewer1 = new Microsoft.Reporting.WebForms.LocalReport();

                DateTime Todate = DateTime.Parse(rm.DateTo);
                // NOTE: DateSelected on the underlying claim procs is a date-only column
                // compared with an inclusive <=/BETWEEN, so do NOT add a day here - that
                // previously caused claim reports to leak in one extra day of data.
                DateTime newDate = Todate;
                DateTime FromDate = DateTime.Parse(rm.DateFrom);

                string DateTO = Todate.ToString("dd-MM-yyyy");
                string FromTO = FromDate.ToString("dd-MM-yyyy");


                 try
                    {
                    if (rm.ReportID == 5)
                    {

                        List<usp_GetClaimSummaryReportForApp_Result> result = db.usp_GetClaimSummaryReportForApp(rm.SaleOfficerID, rm.TraderID,FromDate, newDate).ToList();


                        if (result.Count > 0)
                        {


                            string SoName = "";
                            var SO = db.SaleOfficers.Where(u => u.ID == rm.SaleOfficerID).FirstOrDefault();

                            SoName = SO.Name;




                            ReportParameter[] prm = new ReportParameter[10];
                            prm[0] = new ReportParameter("DistributorName", "Test");
                            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                            prm[2] = new ReportParameter("SOName", SoName);

                            prm[3] = new ReportParameter("DateTo", DateTO);
                            prm[4] = new ReportParameter("DateFrom", FromTO);

                            prm[5] = new ReportParameter("CityName", "Test");
                            prm[6] = new ReportParameter("TotalVisitsToday", "1");

                            prm[7] = new ReportParameter("ProductiveShops", "1");
                            prm[8] = new ReportParameter("TodayWorkingTime", "1");
                            prm[9] = new ReportParameter("FollowUps", "1");

                            ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\ClaimsReport.rdlc");
                            ReportViewer1.EnableExternalImages = true;
                            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                            ReportViewer1.SetParameters(prm);
                            ReportViewer1.DataSources.Clear();
                            ReportViewer1.DataSources.Add(dt1);


                            ReportViewer1.Refresh();
                            Warning[] warnings;
                            string[] streamids;
                            string mimeType;
                            string encoding;
                            string extension;
                            byte[] bytes = ReportViewer1.Render("PDF", null, out mimeType,
                                    out encoding, out extension, out streamids, out warnings);
                            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                            // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                            using (MemoryStream memoryStream = new MemoryStream(bytes))
                            {

                                memoryStream.Seek(0, SeekOrigin.Begin);

                                memoryStream.Close();



                                SuccessResponse d = new SuccessResponse();
                                string fname = "Claim" + DateTime.Now.ToString("ddMMyyyyHHss");
                                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                                HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                                d.data = "http://116.58.33.11:81/" + "\\PDF\\" + fname + ".pdf";
                                return new Result<SuccessResponse>
                                {
                                    Data = d,
                                    Message = "Downloaded",
                                    ResultType = ResultType.Success,
                                    Exception = null,
                                    ValidationErrors = null
                                };



                            }

                        }
                        else
                        {
                            return new Result<SuccessResponse>
                            {
                                Data = null,
                                Message = "No data Present today for this Distributor",
                                ResultType = ResultType.Success,
                                Exception = null,

                            };

                        }
                    }
                        
                    else if (rm.ReportID == 1)
                    {

                        List<usp_GetClaimSummaryReportForApp_Result> result = db.usp_GetClaimSummaryReportForApp(rm.SaleOfficerID, rm.TraderID, FromDate, newDate).ToList();


                        if (result.Count > 0)
                        {


                            string SoName = "";
                            var SO = db.SaleOfficers.Where(u => u.ID == rm.SaleOfficerID).FirstOrDefault();

                            SoName = SO.Name;




                            ReportParameter[] prm = new ReportParameter[10];
                            prm[0] = new ReportParameter("DistributorName", "Test");
                            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                            prm[2] = new ReportParameter("SOName", SoName);

                            prm[3] = new ReportParameter("DateTo", DateTO);
                            prm[4] = new ReportParameter("DateFrom", FromTO);

                            prm[5] = new ReportParameter("CityName", "Test");
                            prm[6] = new ReportParameter("TotalVisitsToday", "1");

                            prm[7] = new ReportParameter("ProductiveShops", "1");
                            prm[8] = new ReportParameter("TodayWorkingTime", "1");
                            prm[9] = new ReportParameter("FollowUps", "1");

                            ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\ClaimsReportDetailTraderWise.rdlc");
                            ReportViewer1.EnableExternalImages = true;
                            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                            ReportViewer1.SetParameters(prm);
                            ReportViewer1.DataSources.Clear();
                            ReportViewer1.DataSources.Add(dt1);


                            ReportViewer1.Refresh();
                            Warning[] warnings;
                            string[] streamids;
                            string mimeType;
                            string encoding;
                            string extension;
                            byte[] bytes = ReportViewer1.Render("PDF", null, out mimeType,
                                    out encoding, out extension, out streamids, out warnings);
                            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                            // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                            using (MemoryStream memoryStream = new MemoryStream(bytes))
                            {

                                memoryStream.Seek(0, SeekOrigin.Begin);

                                memoryStream.Close();



                                SuccessResponse d = new SuccessResponse();
                                string fname = "Claim" + DateTime.Now.ToString("ddMMyyyyHHss");
                                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                                HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                                d.data = "http://116.58.33.11:81/" + "\\PDF\\" + fname + ".pdf";
                                return new Result<SuccessResponse>
                                {
                                    Data = d,
                                    Message = "Downloaded",
                                    ResultType = ResultType.Success,
                                    Exception = null,
                                    ValidationErrors = null
                                };



                            }

                        }
                        else
                        {
                            return new Result<SuccessResponse>
                            {
                                Data = null,
                                Message = "No data Present today for this Distributor",
                                ResultType = ResultType.Success,
                                Exception = null,

                            };

                        }

                    }


                    else if (rm.ReportID == 2)
                    {

                        List<usp_GetClaimSummaryReportTraderWiseForApp_Result> result = db.usp_GetClaimSummaryReportTraderWiseForApp(rm.SaleOfficerID, FromDate, newDate,rm.TraderID).ToList();


                        if (result.Count > 0)
                        {


                            string SoName = "";
                            var SO = db.SaleOfficers.Where(u => u.ID == rm.SaleOfficerID).FirstOrDefault();

                            SoName = SO.Name;




                            ReportParameter[] prm = new ReportParameter[10];
                            prm[0] = new ReportParameter("DistributorName", "Test");
                            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                            prm[2] = new ReportParameter("SOName", SoName);

                            prm[3] = new ReportParameter("DateTo", DateTO);
                            prm[4] = new ReportParameter("DateFrom", FromTO);

                            prm[5] = new ReportParameter("CityName", "Test");
                            prm[6] = new ReportParameter("TotalVisitsToday", "1");

                            prm[7] = new ReportParameter("ProductiveShops", "1");
                            prm[8] = new ReportParameter("TodayWorkingTime", "1");
                            prm[9] = new ReportParameter("FollowUps", "1");

                            ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\ClaimsSummaryTraderWise.rdlc");
                            ReportViewer1.EnableExternalImages = true;
                            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                            ReportViewer1.SetParameters(prm);
                            ReportViewer1.DataSources.Clear();
                            ReportViewer1.DataSources.Add(dt1);


                            ReportViewer1.Refresh();
                            Warning[] warnings;
                            string[] streamids;
                            string mimeType;
                            string encoding;
                            string extension;
                            byte[] bytes = ReportViewer1.Render("PDF", null, out mimeType,
                                    out encoding, out extension, out streamids, out warnings);
                            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                            // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                            using (MemoryStream memoryStream = new MemoryStream(bytes))
                            {

                                memoryStream.Seek(0, SeekOrigin.Begin);

                                memoryStream.Close();



                                SuccessResponse d = new SuccessResponse();
                                string fname = "Claim" + DateTime.Now.ToString("ddMMyyyyHHss");
                                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                                HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                                d.data = "http://116.58.33.11:81/" + "\\PDF\\" + fname + ".pdf";
                                return new Result<SuccessResponse>
                                {
                                    Data = d,
                                    Message = "Downloaded",
                                    ResultType = ResultType.Success,
                                    Exception = null,
                                    ValidationErrors = null
                                };



                            }

                        }
                        else
                        {
                            return new Result<SuccessResponse>
                            {
                                Data = null,
                                Message = "No data Present today for this Distributor",
                                ResultType = ResultType.Success,
                                Exception = null,

                            };

                        }

                    }

                    else if (rm.ReportID == 4)
                    {

                        List<usp_GetClaimSummaryReportCategoryWiseForApp_Result> result = db.usp_GetClaimSummaryReportCategoryWiseForApp(rm.SaleOfficerID, FromDate, newDate, rm.TraderID).ToList();


                        if (result.Count > 0)
                        {


                            string SoName = "";
                            var SO = db.SaleOfficers.Where(u => u.ID == rm.SaleOfficerID).FirstOrDefault();

                            SoName = SO.Name;




                            ReportParameter[] prm = new ReportParameter[10];
                            prm[0] = new ReportParameter("DistributorName", "Test");
                            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                            prm[2] = new ReportParameter("SOName", SoName);

                            prm[3] = new ReportParameter("DateTo", DateTO);
                            prm[4] = new ReportParameter("DateFrom", FromTO);

                            prm[5] = new ReportParameter("CityName", "Test");
                            prm[6] = new ReportParameter("TotalVisitsToday", "1");

                            prm[7] = new ReportParameter("ProductiveShops", "1");
                            prm[8] = new ReportParameter("TodayWorkingTime", "1");
                            prm[9] = new ReportParameter("FollowUps", "1");

                            ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\ClaimsSummaryCategoryWise.rdlc");
                            ReportViewer1.EnableExternalImages = true;
                            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);

                            ReportViewer1.SetParameters(prm);
                            ReportViewer1.DataSources.Clear();
                            ReportViewer1.DataSources.Add(dt1);


                            ReportViewer1.Refresh();
                            Warning[] warnings;
                            string[] streamids;
                            string mimeType;
                            string encoding;
                            string extension;
                            byte[] bytes = ReportViewer1.Render("PDF", null, out mimeType,
                                    out encoding, out extension, out streamids, out warnings);
                            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                            // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                            using (MemoryStream memoryStream = new MemoryStream(bytes))
                            {

                                memoryStream.Seek(0, SeekOrigin.Begin);

                                memoryStream.Close();



                                SuccessResponse d = new SuccessResponse();
                                string fname = "Claim" + DateTime.Now.ToString("ddMMyyyyHHss");
                                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                                HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                                d.data = "http://116.58.33.11:81/" + "\\PDF\\" + fname + ".pdf";
                                return new Result<SuccessResponse>
                                {
                                    Data = d,
                                    Message = "Downloaded",
                                    ResultType = ResultType.Success,
                                    Exception = null,
                                    ValidationErrors = null
                                };



                            }

                        }
                        else
                        {
                            return new Result<SuccessResponse>
                            {
                                Data = null,
                                Message = "No data Present today for this Distributor",
                                ResultType = ResultType.Success,
                                Exception = null,

                            };

                        }

                    }

                }
                    catch (Exception ex)
                    {
                        return new Result<SuccessResponse>
                        {
                            Data = null,
                            Message = ex.InnerException.Message,
                            ResultType = ResultType.Success,
                            Exception = null,

                        };
                    }

              
                //else if (rm.SegmentTypeID == 2)
                //{
                //    try
                //    {

                //        //List<Sp_OrderSummaryGrandTotal_Result> gt = db.Sp_OrderSummaryGrandTotal(FromDate, newDate, rm.DistributorID, rm.RangeID, rm.SaleOfficerID).ToList();



                //        // List<Sp_OrderSummeryReportNotSoldItem1_1_Result> NotItems = db.Sp_OrderSummeryReportNotSoldItem1_1(FromDate, newDate, rm.DistributorID, rm.RangeID, rm.SaleOfficerID).ToList();
                //          List<Sp_OrderForPDFHousingVisits_Result> result = db.Sp_OrderForPDFHousingVisits(FromDate, newDate, rm.SaleOfficerID).ToList();
                //        //  List<sp_BrandAndItemWiseReport_Result> result2 = db.sp_BrandAndItemWiseReport(FromDate, newDate, 0, rm.SaleOfficerID,0,0, rm.RangeID).ToList();
                //        //List<Sp_OurBrandForPDFinMMC_Result> result2 = db.Sp_OurBrandForPDFinMMC(FromDate, newDate, rm.DistributorID, rm.RangeID, rm.SaleOfficerID).ToList();
                //        // List<Sp_FollowUpVisitsDailyForMMC_Result> result3 = db.Sp_FollowUpVisitsDailyForMMC(FromDate, newDate, rm.DistributorID, rm.RangeID, rm.SaleOfficerID).ToList();
                //        if (result.Count > 0)
                //        {


                //            string SoName = "";
                //            var SO = db.SaleOfficers.Where(u => u.ID == rm.SaleOfficerID).FirstOrDefault();

                //            SoName = SO.Name;


                //            //var SOID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerID).Select(x => x.SORoleID).FirstOrDefault();


                //            ReportParameter[] prm = new ReportParameter[10];
                //            prm[0] = new ReportParameter("DistributorName", "Test");
                //            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                //            prm[2] = new ReportParameter("SOName", SoName);

                //            prm[3] = new ReportParameter("DateTo", DateTO);
                //            prm[4] = new ReportParameter("DateFrom", FromTO);

                //            prm[5] = new ReportParameter("CityName", "Test");
                //            prm[6] = new ReportParameter("TotalVisitsToday", "1");

                //            prm[7] = new ReportParameter("ProductiveShops", "1");
                //            prm[8] = new ReportParameter("TodayWorkingTime", "1");
                //            prm[9] = new ReportParameter("FollowUps", "1");


                //            ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\HousingVisit.rdlc");
                //            ReportViewer1.EnableExternalImages = true;
                //            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
                //           // ReportDataSource dt2 = new ReportDataSource("DataSet2", result2);
                //            //ReportDataSource dt3 = new ReportDataSource("DataSet3", result2);
                //            //ReportDataSource dt4 = new ReportDataSource("DataSet4", result3);
                //            ReportViewer1.SetParameters(prm);
                //            ReportViewer1.DataSources.Clear();
                //            ReportViewer1.DataSources.Add(dt1);
                //           // ReportViewer1.DataSources.Add(dt2);
                //            //ReportViewer1.DataSources.Add(dt3);
                //            //ReportViewer1.DataSources.Add(dt4);

                //            ReportViewer1.Refresh();
                //            Warning[] warnings;
                //            string[] streamids;
                //            string mimeType;
                //            string encoding;
                //            string extension;
                //            byte[] bytes = ReportViewer1.Render("PDF", null, out mimeType,
                //                    out encoding, out extension, out streamids, out warnings);
                //            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                //            // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                //            using (MemoryStream memoryStream = new MemoryStream(bytes))
                //            {

                //                memoryStream.Seek(0, SeekOrigin.Begin);

                //                memoryStream.Close();

                                
                                
                //                    SuccessResponse d = new SuccessResponse();
                //                    string fname = "D" + DateTime.Now.ToString("ddMMyyyyHHss");
                //                    System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                //                    HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                //                    d.data = "http://116.58.33.11:81/" + "\\PDF\\" + fname + ".pdf";
                //                    return new Result<SuccessResponse>
                //                    {
                //                        Data = d,
                //                        Message = "Downloaded",
                //                        ResultType = ResultType.Success,
                //                        Exception = null,
                //                        ValidationErrors = null
                //                    };

                                
                //            }

                //        }
                //        else
                //        {
                //            return new Result<SuccessResponse>
                //            {
                //                Data = null,
                //                Message = "No data Present today for this Distributor",
                //                ResultType = ResultType.Success,
                //                Exception = null,

                //            };

                //        }

                //    }
                //    catch (Exception ex)
                //    {
                //        return new Result<SuccessResponse>
                //        {
                //            Data = null,
                //            Message = ex.InnerException.Message,
                //            ResultType = ResultType.Success,
                //            Exception = null,

                //        };
                //    }
                //}


                //else if (rm.SegmentTypeID == 3)
                //{
                //    try
                //    {

                      
                //        List<Sp_OrderForPDFCorporateVisits_Result> result = db.Sp_OrderForPDFCorporateVisits(FromDate, newDate, rm.SaleOfficerID).ToList();
                        
                //        if (result.Count > 0)
                //        {


                //            string SoName = "";
                //            var SO = db.SaleOfficers.Where(u => u.ID == rm.SaleOfficerID).FirstOrDefault();

                //            SoName = SO.Name;


                //            //var SOID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerID).Select(x => x.SORoleID).FirstOrDefault();


                //            ReportParameter[] prm = new ReportParameter[10];
                //            prm[0] = new ReportParameter("DistributorName", "Test");
                //            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                //            prm[2] = new ReportParameter("SOName", SoName);

                //            prm[3] = new ReportParameter("DateTo", DateTO);
                //            prm[4] = new ReportParameter("DateFrom", FromTO);

                //            prm[5] = new ReportParameter("CityName", "Test");
                //            prm[6] = new ReportParameter("TotalVisitsToday", "1");

                //            prm[7] = new ReportParameter("ProductiveShops", "1");
                //            prm[8] = new ReportParameter("TodayWorkingTime", "1");
                //            prm[9] = new ReportParameter("FollowUps", "1");


                //            ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\CorporateVisit.rdlc");
                //            ReportViewer1.EnableExternalImages = true;
                //            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
                //            // ReportDataSource dt2 = new ReportDataSource("DataSet2", result2);
                //            //ReportDataSource dt3 = new ReportDataSource("DataSet3", result2);
                //            //ReportDataSource dt4 = new ReportDataSource("DataSet4", result3);
                //            ReportViewer1.SetParameters(prm);
                //            ReportViewer1.DataSources.Clear();
                //            ReportViewer1.DataSources.Add(dt1);
                //            // ReportViewer1.DataSources.Add(dt2);
                //            //ReportViewer1.DataSources.Add(dt3);
                //            //ReportViewer1.DataSources.Add(dt4);

                //            ReportViewer1.Refresh();
                //            Warning[] warnings;
                //            string[] streamids;
                //            string mimeType;
                //            string encoding;
                //            string extension;
                //            byte[] bytes = ReportViewer1.Render("PDF", null, out mimeType,
                //                    out encoding, out extension, out streamids, out warnings);
                //            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                //            // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                //            using (MemoryStream memoryStream = new MemoryStream(bytes))
                //            {

                //                memoryStream.Seek(0, SeekOrigin.Begin);

                //                memoryStream.Close();



                //                SuccessResponse d = new SuccessResponse();
                //                string fname = "D" + DateTime.Now.ToString("ddMMyyyyHHss");
                //                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                //                HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                //                d.data = "http://116.58.33.11:81/" + "\\PDF\\" + fname + ".pdf";
                //                return new Result<SuccessResponse>
                //                {
                //                    Data = d,
                //                    Message = "Downloaded",
                //                    ResultType = ResultType.Success,
                //                    Exception = null,
                //                    ValidationErrors = null
                //                };


                //            }

                //        }
                //        else
                //        {
                //            return new Result<SuccessResponse>
                //            {
                //                Data = null,
                //                Message = "No data Present today for this Distributor",
                //                ResultType = ResultType.Success,
                //                Exception = null,

                //            };

                //        }

                //    }
                //    catch (Exception ex)
                //    {
                //        return new Result<SuccessResponse>
                //        {
                //            Data = null,
                //            Message = ex.InnerException.Message,
                //            ResultType = ResultType.Success,
                //            Exception = null,

                //        };
                //    }
                //}

                //else if (rm.SegmentTypeID == 5)
                //{
                //    try
                //    {


                //        List<Sp_OrderForPDFAllPurposeVisits_Result> result = db.Sp_OrderForPDFAllPurposeVisits(FromDate, newDate, rm.SaleOfficerID).ToList();

                //        if (result.Count > 0)
                //        {


                //            string SoName = "";
                //            var SO = db.SaleOfficers.Where(u => u.ID == rm.SaleOfficerID).FirstOrDefault();

                //            SoName = SO.Name;


                //            //var SOID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerID).Select(x => x.SORoleID).FirstOrDefault();


                //            ReportParameter[] prm = new ReportParameter[10];
                //            prm[0] = new ReportParameter("DistributorName", "Test");
                //            prm[1] = new ReportParameter("Date", (System.DateTime.Now.ToString()));
                //            prm[2] = new ReportParameter("SOName", SoName);

                //            prm[3] = new ReportParameter("DateTo", DateTO);
                //            prm[4] = new ReportParameter("DateFrom", FromTO);

                //            prm[5] = new ReportParameter("CityName", "Test");
                //            prm[6] = new ReportParameter("TotalVisitsToday", "1");

                //            prm[7] = new ReportParameter("ProductiveShops", "1");
                //            prm[8] = new ReportParameter("TodayWorkingTime", "1");
                //            prm[9] = new ReportParameter("FollowUps", "1");


                //            ReportViewer1.ReportPath = HttpContext.Current.Server.MapPath("~\\Views\\Reports\\AllPurposeVisit.rdlc");
                //            ReportViewer1.EnableExternalImages = true;
                //            ReportDataSource dt1 = new ReportDataSource("DataSet1", result);
                //            // ReportDataSource dt2 = new ReportDataSource("DataSet2", result2);
                //            //ReportDataSource dt3 = new ReportDataSource("DataSet3", result2);
                //            //ReportDataSource dt4 = new ReportDataSource("DataSet4", result3);
                //            ReportViewer1.SetParameters(prm);
                //            ReportViewer1.DataSources.Clear();
                //            ReportViewer1.DataSources.Add(dt1);
                //            // ReportViewer1.DataSources.Add(dt2);
                //            //ReportViewer1.DataSources.Add(dt3);
                //            //ReportViewer1.DataSources.Add(dt4);

                //            ReportViewer1.Refresh();
                //            Warning[] warnings;
                //            string[] streamids;
                //            string mimeType;
                //            string encoding;
                //            string extension;
                //            byte[] bytes = ReportViewer1.Render("PDF", null, out mimeType,
                //                    out encoding, out extension, out streamids, out warnings);
                //            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                //            // HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                //            using (MemoryStream memoryStream = new MemoryStream(bytes))
                //            {

                //                memoryStream.Seek(0, SeekOrigin.Begin);

                //                memoryStream.Close();



                //                SuccessResponse d = new SuccessResponse();
                //                string fname = "D" + DateTime.Now.ToString("ddMMyyyyHHss");
                //                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath("~") + "/PDF/" + fname + ".pdf", bytes);
                //                HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.OK);
                //                d.data = "http://116.58.33.11:81/" + "\\PDF\\" + fname + ".pdf";
                //                return new Result<SuccessResponse>
                //                {
                //                    Data = d,
                //                    Message = "Downloaded",
                //                    ResultType = ResultType.Success,
                //                    Exception = null,
                //                    ValidationErrors = null
                //                };


                //            }

                //        }
                //        else
                //        {
                //            return new Result<SuccessResponse>
                //            {
                //                Data = null,
                //                Message = "No data Present today for this Distributor",
                //                ResultType = ResultType.Success,
                //                Exception = null,

                //            };

                //        }

                //    }
                //    catch (Exception ex)
                //    {
                //        return new Result<SuccessResponse>
                //        {
                //            Data = null,
                //            Message = ex.InnerException.Message,
                //            ResultType = ResultType.Success,
                //            Exception = null,

                //        };
                //    }
                //}










                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "There is an issue with your internet. Kindly Try again",
                    ResultType = ResultType.Success,
                    Exception = null,

                };





            }

            catch (Exception ex)
            {


                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Something Went Wrong",
                    ResultType = ResultType.Failure,
                    Exception = null,

                };

            }

        }


    }



}



public class SuccessResponse1
{
    public string data { get; set; }
}
public class ClaimSummery
{
    public int ReportID { get; set; }
    public int TraderID { get; set; }
    public int RetailerID { get; set; }
    public string ShopName { get; set; }
    public string DateFrom { get; set; }
    public string DateTo { get; set; }
    public int SegmentTypeID { get; set; }

    public int RangeID { get; set; }
    public int SaleOfficerID { get; set; }
    public string Email { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }

  


}


