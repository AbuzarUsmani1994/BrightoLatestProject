using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class FannanSODirectionController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            Tbl_FannanSODirection JobObj = new Tbl_FannanSODirection();
          
            try
            {

                JobObj.SOID = rm.SOID;
                JobObj.Lattitude = rm.Latitude;
                JobObj.Longitude = rm.Longitude;
                JobObj.PushTime = DateTimeOffset.FromUnixTimeMilliseconds(rm.PushTime).LocalDateTime;

                JobObj.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.Tbl_FannanSODirection.Add(JobObj);
                db.SaveChanges();
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "LatLong Saved Successfully",
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
                    Message = "Failed to Save LatLong",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

    


        public class SuccessResponse
        {

        }
        public class DailyActivityRequest
        {
       
            public int SOID { get; set; }

            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public long PushTime { get; set; }
       
        
        }

    }
}