using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
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