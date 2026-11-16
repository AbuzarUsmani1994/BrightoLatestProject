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
    public class BusinessAffiliatesRegistrationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(BusinessAffiliatesRegistrationRequest rm)
        {
            Tbl_SchoolException Excep = new Tbl_SchoolException();
            Tbl_BusinessAffiliates retailerObj = new Tbl_BusinessAffiliates();
            try
            {
                
                //var rangeid= db.SaleOfficers.Where(x=>x.ID==rm.SalesOfficerID).Select(x=>x.RangeID).FirstOrDefault();

               

                    retailerObj.BusinessName = rm.BusinessName;
                    retailerObj.SOID = rm.SalesOfficerID;
                    retailerObj.ContactPerson = rm.ContactPerson;
                    retailerObj.ContactNumber = rm.ContactNumber;
                    retailerObj.CityID = rm.CityID;
                    retailerObj.RegionID = rm.RegionID;
                retailerObj.Address = rm.Address;
                retailerObj.Latitude = rm.Latitude;
                    retailerObj.AffBusinessTypeID = rm.AffiliateBusinessTypeID;
                retailerObj.AffCompititors = rm.AffiliatesCompititorIDS;
                retailerObj.AffNatureOfClientID = rm.NatureOfClientID;
                    retailerObj.Longitude = rm.Longitude;
                    retailerObj.AffClassificationID = rm.AffiliateClassificationID;
                   
                    retailerObj.AffExpertiseID = rm.AffiliateExpertiseID;
                    retailerObj.NoOfSites = rm.NoOfSites;
                    
                    //if (rm.Picture1 == "" || rm.Picture1 == null)
                    //{
                    //    retailerObj.Picture1 = null;
                    //}
                    //else
                    //{
                    //    retailerObj.Picture1 = ConvertIntoByte(rm.Picture1, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "RetailerImages");
                    //}
                   
                
                    retailerObj.Remarks = rm.Remarks;
                    retailerObj.IsActive = true;
              
                    retailerObj.CreatedOn = DateTime.UtcNow.AddHours(5);
                   

                    db.Tbl_BusinessAffiliates.Add(retailerObj);




                    db.SaveChanges();

                    SendWelcomeMessage(retailerObj.ID, retailerObj.ContactNumber);


                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Business Affiliates Registration Successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
               


            }
            catch (Exception ex)
            {
               

                Log.Instance.Error(ex, "Add Business Affiliates API Failed");
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

        // Welcome SMS for a newly-registered Business Affiliate. Never lets a gateway
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
            string normalizedPhone = NormalizePakistaniMobile(rawContactNumber);

            try
            {
                if (string.IsNullOrEmpty(normalizedPhone))
                {
                    errorDetail = "No usable contact number on the Business Affiliate record";
                }
                else
                {
                    string url = "http://weblogin.premiumsms.pk/sendsms_url.html"
                        + "?Username=03214966886&Password=ms876&From=Test%20Acct"
                        + "&To=" + Uri.EscapeDataString(normalizedPhone)
                        + "&Message=" + Uri.EscapeDataString(WelcomeMessageText);

                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync(url).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Sent";
                        }
                        else
                        {
                            errorDetail = "Gateway StatusCode: " + (int)response.StatusCode + " " + response.ReasonPhrase;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorDetail = Log.GetDetailedError(ex);
                Log.Instance.Error(ex, "Business Affiliate welcome SMS failed");
            }

            LogWelcomeMessage(customerId, normalizedPhone, status, errorDetail);
        }

        private void LogWelcomeMessage(int customerId, string phoneNo, string status, string errorDetail)
        {
            try
            {
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
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
                Log.Instance.Error(ex, "Business Affiliate welcome SMS log insert failed");
            }
        }

        // Tbl_BusinessAffiliates.ContactNumber is free-text (varchar) - strip everything
        // but digits, then convert the local 03XXXXXXXXX form to the gateway's expected
        // 923XXXXXXXXX form (matches the format already used elsewhere against this gateway).
        private string NormalizePakistaniMobile(string rawContactNumber)
        {
            if (string.IsNullOrWhiteSpace(rawContactNumber)) return null;

            string digits = new string(rawContactNumber.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(digits)) return null;

            if (digits.StartsWith("0")) return "92" + digits.Substring(1);
            if (digits.StartsWith("92")) return digits;
            return "92" + digits;
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
        public class BusinessAffiliatesRegistrationRequest
        {
            public BusinessAffiliatesRegistrationRequest()
            {
              
            }
            public int RetailerID { get; set; }
            public string BusinessName { get; set; }
            public string ContactPerson { get; set; }
            public string ContactNumber { get; set; }
            public int AffiliateBusinessTypeID { get; set; }
            public int AffiliateClassificationID { get; set; }
            public int AffiliateExpertiseID { get; set; }
            public int NatureOfClientID { get; set; }
            public int SalesOfficerID { get; set; }
         
            public int RegionID { get; set; }
            public int NoOfSites { get; set; }
            public int CityID { get; set; }
            public int ZoneID { get; set; }
            public int AreaID { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
        
            public string Address { get; set; }

            public string AffiliatesCompititorIDS { get; set; }
      
            public string Picture1 { get; set; }
       
            public string Remarks { get; set; }
         

      



        }

     

    }
}