using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class RetailerRegistrationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(RetailerRegistrationRequest rm)
        {
            Tbl_SchoolException Excep = new Tbl_SchoolException();
            Retailer retailerObj = new Retailer();
            try
            {
                
                //var rangeid= db.SaleOfficers.Where(x=>x.ID==rm.SalesOfficerID).Select(x=>x.RangeID).FirstOrDefault();

                //ADD New Retailer 
                retailerObj.ID = db.Retailers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    retailerObj.Name = rm.OwnerName;
                    retailerObj.SaleOfficerID = rm.SalesOfficerID;

               

                    retailerObj.DealerID = 1;
                   
              
                    retailerObj.ShopName = rm.ShopName;
                    retailerObj.AreaID = rm.AreaID;
                    retailerObj.CityID = rm.CityID;
                    // Zone ID  is saving in Regions Table bcx in menu region changes to zone
                    retailerObj.ZoneID =1;
                    retailerObj.RegionID = rm.RegionID;
                    retailerObj.Location = rm.Latitude + "," + rm.Longitude;
                    retailerObj.Latitude = rm.Latitude;
                    retailerObj.NewArea = rm.AreaName;
                retailerObj.BusinessStatusRemarks = rm.BusinessStatusRemarks;
                retailerObj.CompititorIDS = rm.CompititorIDS;
                    retailerObj.Longitude = rm.Longitude;
                    retailerObj.LocationName = rm.LocationName;
                    retailerObj.LocationMargin = null;
                    retailerObj.Phone1 = rm.CellNo1;
                    retailerObj.Phone2 = rm.CellNo2;
                    retailerObj.Email = rm.Email;
                    retailerObj.RetailerClass = rm.RetailerClass;
                    retailerObj.RetailerChannel = 1;
                    retailerObj.RangeID = 6;
                    if (rm.Picture1 == "" || rm.Picture1 == null)
                    {
                        retailerObj.Picture1 = null;
                    }
                    else
                    {
                        retailerObj.Picture1 = ConvertIntoByte(rm.Picture1, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "RetailerImages");
                    }
                   
                
                    retailerObj.Remarks = rm.Remarks;
                    retailerObj.IsActive = true;
                    retailerObj.Status = true;
                    retailerObj.RetailerType = "Retailer";
                    retailerObj.IsDeleted = false;
                    retailerObj.Address = rm.Address;
                retailerObj.Shoptype = rm.ShopType;
                retailerObj.Quota = 1;
                retailerObj.NewOrOld = "Old";
                retailerObj.BusinessTypeID = rm.BusinessTypeID;
                retailerObj.BusinessStatusID = rm.BusinessStatusID;
                if (rm.SegmentTypeID == 0)
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Please Select The Segment Type",
                        ResultType = ResultType.Exception,
                       
                        ValidationErrors = null

                    };

                }
                else
                {
                    retailerObj.SegmentTypeID = rm.SegmentTypeID;

                }
               
                retailerObj.CustomerType = rm.CustomerType;
                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                    retailerObj.LastUpdate = localTime;
                    retailerObj.CreatedBy = rm.SalesOfficerID;

                    db.Retailers.Add(retailerObj);
                    //END

                    // Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = rm.Token;
                    tokenDetail.Action = "Add New Retailer";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();

                    SendWelcomeMessage(retailerObj.ID, retailerObj.Phone1);



                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Retailer Registration Successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
               


            }
            catch (Exception ex)
            {
                Tbl_SchoolException tokenDetail = new Tbl_SchoolException();
                tokenDetail.SaleOfficerID = rm.SalesOfficerID;
                tokenDetail.ExceptionMessage = ex.Message;
                if (ex.InnerException != null)

                    tokenDetail.InnerException = ex.InnerException.ToString();

                else

                    tokenDetail.InnerException = ("Inner Exception is Empty:");

                tokenDetail.Createdby = rm.SalesOfficerID;
                tokenDetail.CreatedOn = DateTime.Now;
                db.Tbl_SchoolException.Add(tokenDetail);
                db.SaveChanges();


                Log.Instance.Error(ex, "Add Retailer API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Retailer Registration API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex, 
                    ValidationErrors = null
              
            };

              
              


            }

         

        }

        // Welcome SMS for a newly-registered Retailer (customer). Never lets a gateway
        // failure affect the registration response - the customer record is already
        // saved by the time this runs, so any error here is only logged.
        private const string WelcomeMessageText =
            "برائیٹو پینٹس کی جانب سے خوش آمدید\n" +
            "برائیٹو پینٹس پاکستان کا پہلا ملٹی نیشنل پینٹ برانڈ ہے جو 30 سے زائد ممالک میں دستیاب ہے\n" +
            "مزید معلومات کے لیےرابطہ فرمائیں\n" +
            "08000-1973\n\n" +
            "https://brightopaints.com";

        private void SendWelcomeMessage(int customerId, string rawContactNumber)
        {
            string status = "Failed";
            string errorDetail = null;
            string normalizedPhone = FOS.Web.UI.Common.CBSSmsSender.NormalizePhoneForCBS(rawContactNumber);

            try
            {
                if (string.IsNullOrEmpty(normalizedPhone))
                {
                    errorDetail = "No usable contact number on the Retailer record";
                }
                else
                {
                    // UniCode="1": the welcome message is Urdu script, which the CBS docs
                    // say needs Unicode SMS encoding - sending it as UniCode="0" (GSM_7,
                    // English-only) is exactly the kind of mismatch a carrier can silently
                    // drop instead of delivering.
                    var result = FOS.Web.UI.Common.CBSSmsSender.SendQuickSMS(normalizedPhone, WelcomeMessageText, "1");
                    if (result.Success)
                    {
                        status = "Sent";
                    }
                    else
                    {
                        errorDetail = "CBS StatusCode: " + result.StatusCode + " " + result.Description;
                    }
                }
            }
            catch (Exception ex)
            {
                errorDetail = Log.GetDetailedError(ex);
                Log.Instance.Error(ex, "Retailer welcome SMS failed");
            }

            // Log the outcome independently of the history-table insert below, so the
            // real gateway result (sent vs. rejected, and why) is never lost even if
            // that insert itself fails for some other reason.
            Log.Instance.Info("Retailer welcome SMS outcome: CustomerID=" + customerId
                + ", Phone=" + normalizedPhone + ", Status=" + status
                + (errorDetail != null ? ", Detail=" + errorDetail : ""));

            LogWelcomeMessage(customerId, normalizedPhone, status, errorDetail);
        }

        private void LogWelcomeMessage(int customerId, string phoneNo, string status, string errorDetail)
        {
            try
            {
                // Use a fresh context rather than the shared `db` field: by this point
                // `db` has already had SaveChanges() called on it for the Retailer
                // insert, which opens its connection - and once a SqlConnection has
                // been opened, ADO.NET strips the password out of .ConnectionString on
                // read-back (Persist Security Info=false by default). Reusing that
                // stripped string here would make every log insert fail with "Login
                // failed for user '<name>'" even though the real credentials are fine.
                using (var freshDb = new FOSDataModel())
                using (var conn = new SqlConnection(freshDb.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(
                    @"INSERT INTO dbo.Tbl_BusinessAffiliateWelcomeMessageLog
                        (CustomerID, PhoneNo, Message, Status, ErrorDetail, CreatedOn)
                      VALUES
                        (@CustomerID, @PhoneNo, @Message, @Status, @ErrorDetail, GETDATE())", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerId });
                    cmd.Parameters.Add(new SqlParameter("@PhoneNo", SqlDbType.VarChar, 20) { Value = (object)phoneNo ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.NVarChar, 1000) { Value = WelcomeMessageText });
                    cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.VarChar, 20) { Value = (object)status ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@ErrorDetail", SqlDbType.NVarChar, 500) { Value = (object)errorDetail ?? DBNull.Value });
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Retailer welcome SMS log insert failed");
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
        public class RetailerRegistrationRequest
        {
            public RetailerRegistrationRequest()
            {
                CompititorInformation = new List<CompititorInfoModel>();
            }
            public int RetailerID { get; set; }
            public string ShopName { get; set; }
            public string ShopType { get; set; }
            public int Quota { get; set; }
            public int BusinessStatusID { get; set; }
            public int BusinessTypeID { get; set; }
            public int SegmentTypeID { get; set; }
            public string CustomerType { get; set; }
            public string OldorNew { get; set; }
            public string OwnerName { get; set; }
            public string CellNo1 { get; set; }
            public string CellNo2 { get; set; }
            public int SalesOfficerID { get; set; }
            public string Email { get; set; }
            public int RegionID { get; set; }
            public int CityID { get; set; }
            public int ZoneID { get; set; }
            public int AreaID { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public string LocationName { get; set; }
            public string Address { get; set; }

            public string CompititorIDS { get; set; }
            public string AreaName { get; set; }
            public string BusinessStatusRemarks { get; set; }
            public string Token { get; set; }
            public int RetailerClass { get; set; }
            public int RetailerChannel { get; set; }
            public int DistributorIDRangeA { get; set; }
            public int DistributorIDRangeB { get; set; }
            public int DistributorIDRangeC { get; set; }
            public string Picture1 { get; set; }
            //public string Picture2 { get; set; }
           // public string Customercode { get; set; }
            public string Remarks { get; set; }
            public bool IsVerified { get; set; }

            public List<CompititorInfoModel> CompititorInformation { get; set; }



        }

        public class CompititorInfoModel
        {
            public int SaleOfficerID { get; set; }
            public int RetailerID { get; set; }
            public int SylabusID { get; set; }
        }


    }
}