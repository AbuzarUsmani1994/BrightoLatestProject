using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    // Edits an existing Business Affiliate registration (Tbl_BusinessAffiliates).
    // BusinessAffiliatesRegistrationController only ever creates new records - this
    // is the missing "save changes to an existing one" counterpart, paired with
    // GetBusinessAffiliateController for pre-filling the edit form.
    public class UpdateBusinessAffiliateController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(UpdateBusinessAffiliateRequest rm)
        {
            try
            {
                var retailerObj = db.Tbl_BusinessAffiliates.Where(x => x.ID == rm.ID).FirstOrDefault();
                if (retailerObj == null)
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Business Affiliate not found",
                        ResultType = ResultType.Failure,
                        Exception = null,
                        ValidationErrors = null
                    };
                }

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
                retailerObj.Remarks = rm.Remarks;

                db.SaveChanges();

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Business Affiliate Updated Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Update Business Affiliates API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Business Affiliate Update API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        public class SuccessResponse
        {
        }

        public class UpdateBusinessAffiliateRequest
        {
            public int ID { get; set; }
            public string BusinessName { get; set; }
            public string ContactPerson { get; set; }
            public int ContactNumber { get; set; }
            public int AffiliateBusinessTypeID { get; set; }
            public int AffiliateClassificationID { get; set; }
            public int AffiliateExpertiseID { get; set; }
            public int NatureOfClientID { get; set; }
            public int SalesOfficerID { get; set; }
            public int RegionID { get; set; }
            public int NoOfSites { get; set; }
            public int CityID { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public string Address { get; set; }
            public string AffiliatesCompititorIDS { get; set; }
            public string Remarks { get; set; }
        }
    }
}
