using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    // Submits a Dealer Verification Form from the mobile app - same fields as a
    // Claims submission (SalesClaimController) minus sale value/line items, plus
    // the verification Picture, into dbo.Tbl_DealerVerification.
    //
    // Tbl_DealerVerification isn't in the .edmx (avoiding touching that fragile
    // mapping for a brand new table), so this uses raw ADO.NET against
    // dbContext.Database.Connection.ConnectionString, matching the pattern already
    // used elsewhere in this app for tables outside the EF model.
    public class SubmitDealerVerificationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        {
            try
            {
                string picturePath = null;
                if (!string.IsNullOrEmpty(rm.Picture))
                {
                    picturePath = ConvertIntoByte(rm.Picture, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "RetailerImages");
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
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }

        public class SuccessResponse
        {
        }

        public class DailyActivityRequest
        {
            public int CustomerID { get; set; }
            public int SaleOfficerId { get; set; }
            public int TradeEmployeeID { get; set; }
            public int TradePartyID { get; set; }
            public int SegmentID { get; set; }
            public DateTime DateSelected { get; set; }
            public string Picture { get; set; }
        }
    }
}
