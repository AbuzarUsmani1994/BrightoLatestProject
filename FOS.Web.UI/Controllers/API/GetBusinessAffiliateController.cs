using FOS.DataLayer;
using Shared.Diagnostics.Logging;
using System;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    // Fetches one Business Affiliate's full details by ID, to pre-fill the mobile
    // app's Edit screen. Tbl_BusinessAffiliates is a normal EF entity (already used
    // via EF in BusinessAffiliatesRegistrationController's Add flow), so this reads
    // it the same way. Field names in the response match
    // BusinessAffiliatesRegistrationController's own request field names (not the
    // raw DB column names) so the app can reuse the same model for Add and Edit.
    public class GetBusinessAffiliateController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        [HttpGet]
        public IHttpActionResult Get(int ID)
        {
            try
            {
                var ba = db.Tbl_BusinessAffiliates.Where(x => x.ID == ID && x.IsActive == true).FirstOrDefault();
                if (ba == null)
                {
                    return Ok(new { Data = (object)null, Message = "Business Affiliate not found", ResultType = "Failure" });
                }

                var data = new
                {
                    ID = ba.ID,
                    BusinessName = ba.BusinessName,
                    ContactPerson = ba.ContactPerson,
                    ContactNumber = ba.ContactNumber,
                    AffiliateBusinessTypeID = ba.AffBusinessTypeID,
                    AffiliateClassificationID = ba.AffClassificationID,
                    AffiliateExpertiseID = ba.AffExpertiseID,
                    NatureOfClientID = ba.AffNatureOfClientID,
                    SalesOfficerID = ba.SOID,
                    RegionID = ba.RegionID,
                    NoOfSites = ba.NoOfSites,
                    CityID = ba.CityID,
                    Latitude = ba.Latitude,
                    Longitude = ba.Longitude,
                    Address = ba.Address,
                    AffiliatesCompititorIDS = ba.AffCompititors,
                    Remarks = ba.Remarks
                };

                return Ok(new { Data = data, Message = "Success", ResultType = "Success" });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetBusinessAffiliate Failed");
                return Ok(new { Data = (object)null, Message = "Failed to load Business Affiliate", ResultType = "Exception" });
            }
        }
    }
}
