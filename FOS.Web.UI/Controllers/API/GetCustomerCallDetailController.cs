using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class GetCustomerCallDetailController : ApiController
    {
        FOSDataModel db = new FOSDataModel();
        public IHttpActionResult Get(int CustomerID)
        {
            try
            {
                var CheckCustomerID = db.Tbl_HousingVisits
                    .Where(x => x.ID == CustomerID)
                    .Select(x => x.CustomerID)
                    .FirstOrDefault();

                if(CheckCustomerID == null)
                {
                    return Ok(new
                    {
                        CallHistory = "No Customer Found"
                    });
                }

                // UI only shows the latest call, not a history - take just the most
                // recently created row instead of every call ever logged for this
                // customer. Still wrapped in a list (0 or 1 items) so the response
                // shape is unchanged for the app.
                var callHistory = (from j in db.Tbl_SaveCall
                                   join hv in db.Tbl_HousingVisits on j.VisitID equals hv.ID
                                   where hv.CustomerID == CustomerID
                                   orderby j.CreatedOn descending
                                   select new
                                   {
                                       CallID = j.ID,
                                       VisitID = j.VisitID,
                                       CallDate = j.CreatedOn,
                                       CallRemarks = j.CallerRemarks,
                                       Rating = j.CallRating,
                                       CallerName = j.CallerName,
                                       NextVisitDate = j.NextExpectedVisit,
                                       NatureOfCall = j.NatureOfCall,
                                       SiteStatus = j.SiteStatus
                                   }).Take(1).ToList();
                return Ok(new
                {
                    CallHistory = callHistory
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    CallHistory = ex.Message.ToString()
                });
            }

        }
    }
}
