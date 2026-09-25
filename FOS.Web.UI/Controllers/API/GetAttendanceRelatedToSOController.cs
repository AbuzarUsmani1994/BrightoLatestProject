using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class GetAttendanceRelatedToSOController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID, string Date)
        {
            try
            {
                DateTime dtFromTodayUtc = Convert.ToDateTime(Date);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);
                List<AllSaleOfficers> MAinCat = new List<AllSaleOfficers>();
                AllSaleOfficers cty;
                List<AllSaleOfficers> list;

                List<AllSaleOfficersForAttendance> MAinCat2 = new List<AllSaleOfficersForAttendance>();
                AllSaleOfficersForAttendance cty2;
                List<AllSaleOfficersForAttendance> list2;

               
                if (SOID > 0)
                {
                    var dbMainCat = db.Tbl_Access.Where(c => c.RepotedUP == SOID && c.Status == true).Select(c => new
                    {
                        id = c.ReportedDown
                    });

                    foreach (var dbCty in dbMainCat)
                    {
                        cty = new AllSaleOfficers();
                        cty.ID = dbCty.id;
                       //cty.Name = db.SaleOfficers.Where(x => x.ID == dbCty.id && x.IsActive == true).Select(x => x.Name).FirstOrDefault();

                        MAinCat.Add(cty);
                    }
                    foreach (var item in MAinCat)
                    {
                        var AttendanceData = db.SOAttendances.Where(x => x.SOID == item.ID && x.CreatedAt >= dtFromTodayUtc && x.CreatedAt <= dtToToday && x.Type== "Market start").FirstOrDefault();

                        var AttendanceDataclose = db.SOAttendances.Where(x => x.SOID == item.ID && x.CreatedAt >= dtFromTodayUtc && x.CreatedAt <= dtToToday && x.Type == "Market close").FirstOrDefault();


                        if (AttendanceData != null)
                        {
                            cty2 = new AllSaleOfficersForAttendance();
                            cty2.ID = AttendanceData.SOID;
                            cty2.Name = AttendanceData.SaleOfficer.Name;
                            cty2.City = AttendanceData.City.Name;
                            cty2.MarketStart = AttendanceData.CreatedAt;
                            if (AttendanceDataclose == null)
                            {
                                cty2.MarketClose = null;
                            }
                            else
                            {


                                cty2.MarketClose = AttendanceDataclose.CreatedAt;
                            }
                            MAinCat2.Add(cty2);
                        }
                            

                      
                        

                    }

                    if (MAinCat2 != null && MAinCat2.Count > 0)
                    {
                        return Ok(new
                        {
                            MyListForAttendance = MAinCat2.OrderBy(x => x.Name).ToList()

                        });
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "CitiesController GET API Failed");
            }
            object[] param = { };
            return Ok(new
            {
                MyListForAttendance = param
            });
        }


    }
}