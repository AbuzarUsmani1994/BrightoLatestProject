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
    public class GetApprovalsListController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ClaimID,int LoginSOID, string DocType = null)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<ApprovalsList> CustomerValidate = new List<ApprovalsList>();
                ApprovalsList cty;
                //List<RetailerData> MAinCat = new List<RetailerData>();

                // DocType defaults to Claims (existing callers unaffected).
                // Tbl_DealerVerification has no Tbl_ClaimsApproval-equivalent
                // multi-approver chain - just a single "latest reviewer" slot on the
                // row itself - so this returns at most one synthetic entry instead
                // of a growing history list.
                if (DocType == "DealerVerification" && ClaimID > 0)
                {
                    using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                    using (var cmd = new SqlCommand(@"
                        SELECT dv.ID, dv.ClaimManagerLatestRemarks, dv.ClaimManagerLatestStatus, dv.ClaimManagerDate, dv.ApprovedBy,
                               apso.Name AS ApprovedByName
                        FROM dbo.Tbl_DealerVerification dv
                        LEFT JOIN dbo.SaleOfficers apso ON dv.ApprovedBy = apso.ID
                        WHERE dv.ID = @ClaimID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ClaimID", ClaimID);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader["ClaimManagerLatestStatus"] != DBNull.Value)
                            {
                                var approvedBy = reader["ApprovedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ApprovedBy"]);
                                CustomerValidate.Add(new ApprovalsList
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Remarks = reader["ClaimManagerLatestRemarks"] as string,
                                    ApprovedStatus = reader["ClaimManagerLatestStatus"] as string,
                                    ApprovedAt = reader["ClaimManagerDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["ClaimManagerDate"]).ToString("yyyy-MM-dd HH:mm:ss"),
                                    SOName = reader["ApprovedByName"] as string,
                                    CanDelete = approvedBy == LoginSOID
                                });
                            }
                        }
                    }

                    return Ok(new { ApprovalList = CustomerValidate });
                }

                if (ClaimID > 0)
                {
                    object[] param = { ClaimID };

                  

                    var result = dbContext.Tbl_ClaimsApproval.Where(x=>x.ClaimID==ClaimID && x.IsActive==true ).OrderBy(x => x.ApprovedAt).ToList();

                    foreach (var dbCty in result)
                    {
                        cty = new ApprovalsList();
                        cty.ID = dbCty.ID;
                        cty.Remarks = dbCty.Remarks;
                        cty.ApprovedStatus = dbCty.ApprovedStatus;
                        cty.ApprovedAt = dbCty.ApprovedAt.HasValue? dbCty.ApprovedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
                        cty.SOName = dbContext.SaleOfficers.Where(x=>x.ID==dbCty.ApprovedBy).Select(x=>x.Name).FirstOrDefault();

                        cty.CanDelete = dbCty.ApprovedBy == LoginSOID ? true : false;

                        CustomerValidate.Add(cty);
                    }

                    if (CustomerValidate != null && CustomerValidate.Count > 0)
                    {
                        return Ok(new
                        {
                            ApprovalList = CustomerValidate

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                ApprovalList = paramm
            });

        }


    }
}