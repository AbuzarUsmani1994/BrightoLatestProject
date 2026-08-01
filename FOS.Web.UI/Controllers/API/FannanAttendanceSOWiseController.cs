using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class FannanAttendanceSOWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID,string Date)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                //var SOType= db.SaleOfficers.Where(x=>x.ID==SOID).Select(x=>x.so)


                DateTime dtFromTodayUtc = Convert.ToDateTime(Date);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);

                if (SOID > 0)
                {
                  
                    object[] param = { SOID };


                  
                        var result = dbContext.Tbl_FannanSODirection.Where(x => x.SOID == SOID && x.CreatedOn >= dtFromToday && x.CreatedOn <= dtToToday ).Select(x => new
                        {
                            ID=x.ID,
                            SOID=x.SOID,
                            Lattitude=x.Lattitude,
                            Longitude=x.Longitude,
                            Location = x.Lattitude+","+x.Longitude,
                            CreatedDate=x.PushTime


                        }).ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                FannanAttendanceList = result

                            });
                        }
                  

                   

                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = {};
            return Ok(new
            {
                FannanAttendanceList = paramm
            });

        }


    }
}