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
    public class MyClaimsSOWiseController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        // DocType defaults to Claims (existing callers that don't send it get the
        // original, untouched behavior below). DocType = "DealerVerification" lists
        // Tbl_DealerVerification instead - it's not in the .edmx, so that branch is
        // raw ADO.NET rather than an EF query. Unlike Claims, it has no
        // TotalLiters/SaleValue (that table has neither), so those come back null.
        public IHttpActionResult Get(int SOID,string DateFrom, string DateTo, int SegmentID, string DocType = null)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                //var SOType= db.SaleOfficers.Where(x=>x.ID==SOID).Select(x=>x.so)


                DateTime dtFromTodayUtc = Convert.ToDateTime(DateFrom);

                DateTime dtFromToday = dtFromTodayUtc.Date;

                DateTime dttoTodayUtc = Convert.ToDateTime(DateTo);
                DateTime dtToToday = dttoTodayUtc.AddDays(1);

                if (SOID > 0)
                {
                    var Url = "http://116.58.33.11:81/";
                    object[] param = { SOID };

                    if (DocType == "DealerVerification")
                    {
                        var dvList = new List<object>();
                        using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                        using (var cmd = new SqlCommand(@"
                            SELECT dv.ID, r.ShopName AS Name, dv.Picture, dv.DateSelected, dv.CreatedOn,
                                   apso.Name AS ApprovedByName
                            FROM dbo.Tbl_DealerVerification dv
                            JOIN dbo.Retailers r ON dv.CustomerID = r.ID
                            LEFT JOIN dbo.SaleOfficers apso ON dv.ApprovedBy = apso.ID
                            WHERE dv.SOID = @SOID AND dv.DateSelected >= @DateFrom AND dv.DateSelected <= @DateTo
                              AND dv.SegmentID = @SegmentID AND dv.IsActive = 1 AND dv.Status = 'InProgress'
                            ORDER BY r.ShopName", conn))
                        {
                            cmd.Parameters.AddWithValue("@SOID", SOID);
                            cmd.Parameters.AddWithValue("@DateFrom", dtFromToday);
                            cmd.Parameters.AddWithValue("@DateTo", dtToToday);
                            cmd.Parameters.AddWithValue("@SegmentID", SegmentID);

                            conn.Open();
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    DateTime? dateSelected = reader["DateSelected"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateSelected"]);
                                    DateTime? createdOn = reader["CreatedOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedOn"]);
                                    string approvedByName = reader["ApprovedByName"] as string;

                                    dvList.Add(new
                                    {
                                        ID = Convert.ToInt32(reader["ID"]),
                                        Name = reader["Name"] as string,
                                        TotalLiters = (decimal?)null,
                                        pic1 = Url + (reader["Picture"] as string),
                                        SaleValue = (decimal?)null,
                                        Date = "Verification Date: " + dateSelected?.ToString("yyyy-MM-dd") + Environment.NewLine +
                                               "Submitted Date: " + createdOn?.ToString("yyyy-MM-dd") + Environment.NewLine +
                                               "Approved By: " + (approvedByName ?? "")
                                    });
                                }
                            }
                        }

                        if (dvList.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimSummery = dvList
                            });
                        }
                    }
                    else if (SegmentID == 1)
                    {
                        //var result = dbContext.Tbl_SalesClaimMaster.Where(x => x.SOID == SOID && x.CreatedOn >= dtFromToday && x.CreatedOn <= dtToToday && x.SegmentID==1 &&x.IsActive==true && x.Status== "InProgress").Select(x => new
                        //{
                        //    ID=x.ID,
                        //    Name=dbContext.Retailers.Where(y=>y.ID== x.CustomerID).Select(y=>y.ShopName).FirstOrDefault(),
                        //    TotalLiters=x.TotalLiters,
                        //    pic1= Url+x.Picture,
                        //    Date = x.CreatedOn.HasValue ? x.CreatedOn.Value.ToString("yyyy-MM-dd") : null




                        //}).ToList();


                        var result = dbContext.Tbl_SalesClaimMaster
                          .Where(x => x.SOID == SOID && x.DateSelected >= dtFromToday && x.DateSelected <= dtToToday && x.SegmentID == 1 && x.IsActive == true && x.Status == "InProgress")
                          .AsEnumerable() // Moves execution to memory
                          .Select(x => new
                          {
                              ID = x.ID,
                              Name = dbContext.Retailers
                                      .Where(y => y.ID == x.CustomerID)
                                      .Select(y => y.ShopName)
                                      .FirstOrDefault(),
                              TotalLiters = x.TotalLiters,
                              pic1 = Url + x.Picture,
                              SaleValue=x.SaleValue,
                              Date = "Claim Date: " + x.DateSelected?.ToString("yyyy-MM-dd") + Environment.NewLine +
                                      "Submitted Date: " + x.CreatedOn?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Approved By: " + string.Join(", ", dbContext.Tbl_ClaimsApproval
                                    .Where(ca => ca.ClaimID == x.ID && ca.IsActive==true)
                                    .Join(dbContext.SaleOfficers,
                                          ca => ca.ApprovedBy,
                                          so => so.ID,
                                          (ca, so) => so.Name)
                                    .ToList())
                          })
                           .OrderBy(x => x.Name)
                          .ToList();

                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimSummery = result

                            });
                        }
                    }

                    if (SegmentID == 2)
                    {
                     
                        //}).ToList();

                        var result = dbContext.Tbl_SalesClaimMaster
                            .Where(x => x.SOID == SOID && x.DateSelected >= dtFromToday && x.DateSelected <= dtToToday && x.SegmentID == 2 && x.IsActive == true && x.Status == "InProgress")
                            .AsEnumerable() // Moves execution to memory
                            .Select(x => new
                            {
                                ID = x.ID,
                                Name = dbContext.Retailers
                                        .Where(y => y.ID == x.CustomerID)
                                        .Select(y => y.ShopName)
                                        .FirstOrDefault(),
                                TotalLiters = x.TotalLiters,
                                pic1 = Url + x.Picture,
                                SaleValue = x.SaleValue,
                                Date = "Claim Date: " + x.DateSelected?.ToString("yyyy-MM-dd") + Environment.NewLine +
                                      "Submitted Date: " + x.CreatedOn?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Approved By: " + string.Join(", ", dbContext.Tbl_ClaimsApproval
                                    .Where(ca => ca.ClaimID == x.ID && ca.IsActive == true)
                                    .Join(dbContext.SaleOfficers,
                                          ca => ca.ApprovedBy,
                                          so => so.ID,
                                          (ca, so) => so.Name)
                                    .ToList())
                            })
                             .OrderBy(x => x.Name)
                            .ToList();




                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimSummery = result

                            });
                        }
                    }

                    if (SegmentID == 3)
                    {
                        //var result = dbContext.Tbl_SalesClaimMaster.Where(x => x.SOID == SOID && x.CreatedOn >= dtFromToday && x.CreatedOn <= dtToToday && x.SegmentID == 3 && x.IsActive == true && x.Status == "InProgress").Select(x => new
                        //{
                        //    ID = x.ID,
                        //    Name = dbContext.Retailers.Where(y => y.ID == x.CustomerID).Select(y => y.ShopName).FirstOrDefault(),
                        //    TotalLiters = x.TotalLiters,
                        //    pic1 = Url + x.Picture,
                        //    Date = x.CreatedOn.HasValue ? x.CreatedOn.Value.ToString("yyyy-MM-dd") : null



                        //}).ToList();

                        var result = dbContext.Tbl_SalesClaimMaster
                          .Where(x => x.SOID == SOID && x.DateSelected >= dtFromToday && x.DateSelected <= dtToToday && x.SegmentID == 3 && x.IsActive == true && x.Status == "InProgress")
                          .AsEnumerable() // Moves execution to memory
                          .Select(x => new
                          {
                              ID = x.ID,
                              Name = dbContext.Retailers
                                      .Where(y => y.ID == x.CustomerID)
                                      .Select(y => y.ShopName)
                                      .FirstOrDefault(),
                              TotalLiters = x.TotalLiters,
                              pic1 = Url + x.Picture,
                              SaleValue = x.SaleValue,
                              Date = "Claim Date: " + x.DateSelected?.ToString("yyyy-MM-dd") + Environment.NewLine +
                                      "Submitted Date: " + x.CreatedOn?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Approved By: " + string.Join(", ", dbContext.Tbl_ClaimsApproval
                                    .Where(ca => ca.ClaimID == x.ID && ca.IsActive == true)
                                    .Join(dbContext.SaleOfficers,
                                          ca => ca.ApprovedBy,
                                          so => so.ID,
                                          (ca, so) => so.Name)
                                    .ToList())
                          })
                           .OrderBy(x => x.Name)
                          .ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimSummery = result

                            });
                        }
                    }


                    if (SegmentID == 5)
                    {
                        //var result = dbContext.Tbl_SalesClaimMaster.Where(x => x.SOID == SOID && x.CreatedOn >= dtFromToday && x.CreatedOn <= dtToToday && x.SegmentID == 5 && x.IsActive == true && x.Status == "InProgress").Select(x => new
                        //{
                        //    ID = x.ID,
                        //    Name = dbContext.Retailers.Where(y => y.ID == x.CustomerID).Select(y => y.ShopName).FirstOrDefault(),
                        //    TotalLiters = x.TotalLiters,
                        //    pic1 = Url + x.Picture,
                        //    Date = x.CreatedOn.HasValue ? x.CreatedOn.Value.ToString("yyyy-MM-dd") : null



                        //}).ToList();

                        var result = dbContext.Tbl_SalesClaimMaster
                    .Where(x => x.SOID == SOID && x.DateSelected >= dtFromToday && x.DateSelected <= dtToToday && x.SegmentID == 5 && x.IsActive == true && x.Status == "InProgress")
                    .AsEnumerable()
                    .Select(x => new
                    {
                        ID = x.ID,
                        Name = dbContext.Retailers
                                .Where(y => y.ID == x.CustomerID)
                                .Select(y => y.ShopName)
                                .FirstOrDefault(),
                        TotalLiters = x.TotalLiters,
                        pic1 = Url + x.Picture,
                        SaleValue = x.SaleValue,
                        Date = "Claim Date: " + x.DateSelected?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Submitted Date: " + x.CreatedOn?.ToString("yyyy-MM-dd") + Environment.NewLine +
                               "Approved By: " + string.Join(", ", dbContext.Tbl_ClaimsApproval
                                    .Where(ca => ca.ClaimID == x.ID && ca.IsActive == true)
                                    .Join(dbContext.SaleOfficers,
                                          ca => ca.ApprovedBy,
                                          so => so.ID,
                                          (ca, so) => so.Name)
                                    .ToList())
                    })
                    .OrderBy(x => x.Name)
                    .ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ClaimSummery = result

                            });
                        }
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
                ClaimSummery = paramm
            });

        }


    }
}