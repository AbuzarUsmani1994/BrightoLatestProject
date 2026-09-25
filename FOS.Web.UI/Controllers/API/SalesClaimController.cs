using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class SalesClaimController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.

            // DocType defaults to "Claims" (existing callers that don't send it get the
            // original, untouched Claims behavior below). Only "DealerVerification" takes
            // the separate branch, saving to Tbl_DealerVerification instead - no
            // SaleValue/items there, only the verification picture.
            if (rm.DocType == "DealerVerification")
            {
                return PostDealerVerification(rm);
            }

            Tbl_ClaimDetail jobDet = new Tbl_ClaimDetail();
            var JobObj = new Tbl_SalesClaimMaster();
            var RemObj = new TblReminder();
            var kpiReport = new KPIReport();

            try
            {
                //// Date validation logic
                //DateTime currentDate = DateTime.Now;
                //DateTime selectedDate = rm.DateSelected;

                //// Check if selected date is in previous month
                //if (selectedDate.Year < currentDate.Year ||
                //    (selectedDate.Year == currentDate.Year && selectedDate.Month < currentDate.Month))
                //{
                //    // For previous month data, only allow if current date is on or before 10th of current month
                //    if (currentDate.Day > 10)
                //    {
                //        return new Result<SuccessResponse>
                //        {
                //            Data = null,
                //            Message = "Previous month data can only be saved until the 10th of current month",
                //            ResultType = ResultType.Warning,
                //            Exception = null,
                //            ValidationErrors = null
                //        };
                //    }
                //}

                //else if (selectedDate > currentDate)
                //{
                //    return new Result<SuccessResponse>
                //    {
                //        Data = null,
                //        Message = "Future dates cannot be selected",
                //        ResultType = ResultType.Warning,
                //        Exception = null,
                //        ValidationErrors = null
                //    };
                //}
                //// Check if selected date is in current month
                //else if (selectedDate.Year == currentDate.Year && selectedDate.Month == currentDate.Month)
                //{
                //    // Current month data is always allowed
                //    // No additional validation needed
                //}
                //// Check if selected date is in future
              

                // Log incoming request for diagnostics
                Log.Instance.Info("SalesClaim POST => SOID: " + rm.SaleOfficerId
                    + ", CustomerID: " + rm.CustomerID
                    + ", SegmentID: " + rm.SegmentID
                    + ", ProductDetails count: " + (rm.ProductDetails != null ? rm.ProductDetails.Count.ToString() : "null"));

                if (rm.ProductDetails != null)
                {
                    foreach (var p in rm.ProductDetails)
                        Log.Instance.Info("SalesClaim Product => ProductID: " + p.ProductID + ", Gallon: " + p.Gallon + ", Drum: " + p.Drum + ", Quarter: " + p.Quarter);
                }

                // Proceed with saving the data
                JobObj.SOID = rm.SaleOfficerId;
                JobObj.SegmentID = rm.SegmentID;
                JobObj.CustomerID = rm.CustomerID;
                JobObj.TradePartyID = rm.TradePartyID;
                JobObj.TradeEmployeeID = rm.TradeEmployeeID;
                JobObj.TotalLiters = rm.TotalLiters;
                JobObj.SaleValue = rm.SalesValue;
                JobObj.CreatedOn = DateTime.Now;
                JobObj.IsActive = true;
                JobObj.Status = "InProgress";
                JobObj.DateSelected = rm.DateSelected;

                if (rm.pic1 == "" || rm.pic1 == null)
                {
                    JobObj.Picture = null;
                }
                else
                {
                    JobObj.Picture = ConvertIntoByte(rm.pic1, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "RetailerImages");
                }

                db.Tbl_SalesClaimMaster.Add(JobObj);
                db.SaveChanges();

                // Here StockItems is the array which is changed From JobItems Array due to Hassan 
                if (rm.ProductDetails != null && rm.ProductDetails.Count > 0)
                {
                    foreach (var item in rm.ProductDetails)
                    {
                        var productDetail = db.Tbl_ProductDetail.FirstOrDefault(p => p.ID == item.ProductID);
                                       

                        if (productDetail == null)
                            Log.Instance.Warn("SalesClaim: No product found for ProductID: " + item.ProductID);

                        db.Tbl_ClaimDetail.Add(
                            new Tbl_ClaimDetail
                            {
                                ClaimMasterID = JobObj.ID,
                                ProductID = item.ProductID,
                                Gallon = item.Gallon,
                                GallonRate = productDetail != null ? productDetail.Gallon_Price == null ? 0 : productDetail.Gallon_Price : 0,

                                Drum = item.Drum,
                                DrumRate = productDetail != null ? productDetail.Drum_Price == null ? 0 : productDetail.Drum_Price : 0,

                                Quarter = item.Quarter,
                                QtrRate = productDetail != null ? productDetail.Qtr_Price == null ? 0 : productDetail.Qtr_Price : 0,

                                SaleLineValue = item.SaleLineValue,
                                CreatedOn = DateTime.Now
                            });
                        db.SaveChanges();
                    }
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Claim Submit Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Order API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Order API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        // Tbl_DealerVerification isn't in the .edmx, so this is raw ADO.NET rather
        // than an EF insert, matching SubmitDealerVerificationController.
        private Result<SuccessResponse> PostDealerVerification(DailyActivityRequest rm)
        {
            try
            {
                string picturePath = null;
                if (!string.IsNullOrEmpty(rm.pic1))
                {
                    picturePath = ConvertIntoByte(rm.pic1, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "RetailerImages");
                }

                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    INSERT INTO dbo.Tbl_DealerVerification
                        (SOID, SegmentID, CustomerID, TradePartyID, TradeEmployeeID, DateSelected, Picture, Status, CreatedOn, IsActive)
                    VALUES
                        (@SOID, @SegmentID, @CustomerID, @TradePartyID, @TradeEmployeeID, @DateSelected, @Picture, @Status, GETDATE(), 1)", conn))
                {
                    cmd.Parameters.AddWithValue("@SOID", rm.SaleOfficerId);
                    cmd.Parameters.AddWithValue("@SegmentID", rm.SegmentID);
                    cmd.Parameters.AddWithValue("@CustomerID", rm.CustomerID);
                    cmd.Parameters.AddWithValue("@TradePartyID", rm.TradePartyID);
                    cmd.Parameters.AddWithValue("@TradeEmployeeID", rm.TradeEmployeeID);
                    cmd.Parameters.AddWithValue("@DateSelected", rm.DateSelected);
                    cmd.Parameters.AddWithValue("@Picture", (object)picturePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", "InProgress");

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Dealer Verification Submitted Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Dealer Verification API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Dealer Verification API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        public string ConvertIntoByte(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }


        public class SuccessResponse
        {

        }
        public class DailyActivityRequest
        {
            public DailyActivityRequest()
            {
                ProductDetails = new List<JobItemModel>();
            }
          
            public int CustomerID { get; set; }
            public int SaleOfficerId { get; set; }
            public int TradeEmployeeID { get; set; }
            public int TradePartyID { get; set; }
            public int SegmentID { get; set; }
            public DateTime DateSelected { get; set; }
            public decimal TotalLiters { get; set; }
            public decimal SalesValue { get; set; }
            public string pic1 { get; set; }
            // "Claims" (default/omitted) or "DealerVerification". Existing callers that
            // don't send this at all get the original Claims behavior, unchanged.
            public string DocType { get; set; }

            public List<JobItemModel> ProductDetails { get; set; }

        }

        public class JobItemModel
        {
            public int ProductID { get; set; }
          
            public decimal Drum { get; set; }
            public decimal Gallon { get; set; }

            public decimal Quarter { get; set; }
            public decimal SaleLineValue { get; set; }
        }
    }
}/*                                                                            */