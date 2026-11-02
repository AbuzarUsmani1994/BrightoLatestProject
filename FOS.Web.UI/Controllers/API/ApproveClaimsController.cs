using FOS.DataLayer;
using FOS.Setup;
using FOS.Web.UI.Common;
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
    public class ApproveClaimsController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        // DocType defaults to Claims (existing callers that don't send it are
        // unaffected). DocType = "DealerVerification" approves the matching
        // Tbl_DealerVerification row instead - it's not in the .edmx, so that
        // branch is raw ADO.NET rather than an EF update.
        public IHttpActionResult Get(int ClaimID , int SOID, string DocType = null)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {

                if (ClaimID > 0)
                {
                    if (DocType == "DealerVerification")
                    {
                        using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                        using (var cmd = new SqlCommand("UPDATE dbo.Tbl_DealerVerification SET Status = 'Approved', ApprovedBy = @SOID WHERE ID = @ClaimID", conn))
                        {
                            cmd.Parameters.AddWithValue("@SOID", SOID);
                            cmd.Parameters.AddWithValue("@ClaimID", ClaimID);
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        var result = dbContext.Tbl_SalesClaimMaster.Where(x => x.ID == ClaimID).FirstOrDefault();
                        if (result != null)
                        {
                            result.Status = "Approved";
                            result.ApprovedBy = SOID;
                            dbContext.SaveChanges();
                        }
                    }

                    return Ok(new
                    {
                        Message = "Claim approved successfully.",
                        ResultType = ResultType.Success,

                    });





                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ApproveClaimsController GET API Failed");
            }
            object[] paramm = {};
            return Ok(new
            {
                ClaimSummery = paramm
            });

        }


    }
}