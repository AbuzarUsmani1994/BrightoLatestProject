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
    public class DeleteApprovalListController : ApiController
    {



        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DeleteApprovalRequest request)
        {
            // DocType defaults to Claims (existing callers unaffected). For
            // DealerVerification, ClaimApprovalID is the Tbl_DealerVerification row's
            // own ID (GetApprovalsListController's DV entry uses that same ID, since
            // there's no separate Tbl_ClaimsApproval-style row to soft-delete) -
            // "deleting" the single reviewer slot just clears it back to blank.
            if (request.DocType == "DealerVerification")
            {
                return DeleteDealerVerificationApproval(request);
            }

            try
            {
                var retailers = db.Tbl_ClaimsApproval.Where(a => a.ID == request.ClaimApprovalID).FirstOrDefault();
                retailers.IsActive = false;
                db.SaveChanges();

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Approval Deleted Successfully",
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
                    Message = "Delete Request Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        // Tbl_DealerVerification isn't in the .edmx, so raw ADO.NET rather than an
        // EF update, same as the other DealerVerification branches in this app.
        private Result<SuccessResponse> DeleteDealerVerificationApproval(DeleteApprovalRequest request)
        {
            try
            {
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(@"
                    UPDATE dbo.Tbl_DealerVerification
                    SET ApprovedBy = NULL, ClaimManagerLatestStatus = NULL,
                        ClaimManagerLatestRemarks = NULL, ClaimManagerDate = NULL
                    WHERE ID = @ClaimApprovalID", conn))
                {
                    cmd.Parameters.AddWithValue("@ClaimApprovalID", request.ClaimApprovalID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Approval Deleted Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Dealer Verification Delete Approval API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Delete Request Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }


    }
    public class DeleteApprovalRequest
    {
        public int ClaimApprovalID { get; set; }
        public string DocType { get; set; }
    }
}