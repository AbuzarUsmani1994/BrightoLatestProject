using FOS.DataLayer;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class GetCompetitorActivitiesController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        // GET /api/GetCompetitorActivities?SOID=1&DateFrom=2026-01-01&DateTo=2026-12-31&ActivityTypeID=2
        [HttpGet]
        public IHttpActionResult Get(int? SOID = null, DateTime? DateFrom = null, DateTime? DateTo = null, int? ActivityTypeID = null)
        {
            try
            {
                var list = new List<object>();

                const string sql = @"
                    SELECT
                        ca.ID,
                        ca.ActivityDate,
                        at.Name        AS ActivityType,
                        cl.Name        AS CompetitorName,
                        ca.Remarks,
                        r.Name         AS Zone,
                        ci.Name        AS City,
                        so.Name        AS EmployeeName,
                        ca.PicturePath,
                        ca.VideoPath,
                        ca.VoicePath
                    FROM dbo.Tbl_CompetitorActivities ca
                    LEFT JOIN dbo.Tbl_CompetitorActivityTypes at ON at.ID = ca.ActivityTypeID
                    LEFT JOIN dbo.Tbl_CompititorList          cl ON cl.ID = ca.CompetitorID
                    LEFT JOIN dbo.Regions                     r  ON r.ID  = ca.ZoneID
                    LEFT JOIN dbo.Cities                      ci ON ci.ID = ca.CityID
                    LEFT JOIN dbo.SaleOfficers                so ON so.ID = ca.SOID
                    WHERE ca.IsDeleted = 0
                      AND (@SOID           IS NULL OR ca.SOID           = @SOID)
                      AND (@DateFrom       IS NULL OR ca.ActivityDate   >= @DateFrom)
                      AND (@DateTo         IS NULL OR ca.ActivityDate   <= @DateTo)
                      AND (@ActivityTypeID IS NULL OR ca.ActivityTypeID  = @ActivityTypeID)
                    ORDER BY ca.ActivityDate DESC, ca.ID DESC";

                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd  = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@SOID",           SqlDbType.Int)  { Value = (object)SOID           ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@DateFrom",       SqlDbType.Date) { Value = (object)DateFrom       ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@DateTo",         SqlDbType.Date) { Value = (object)DateTo         ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@ActivityTypeID", SqlDbType.Int)  { Value = (object)ActivityTypeID ?? DBNull.Value });

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new
                            {
                                ID             = Convert.ToInt32(reader["ID"]),
                                ActivityDate   = reader["ActivityDate"]   == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ActivityDate"]),
                                ActivityType   = reader["ActivityType"]   as string,
                                CompetitorName = reader["CompetitorName"] as string,
                                Remarks        = reader["Remarks"]        as string,
                                Zone           = reader["Zone"]           as string,
                                City           = reader["City"]           as string,
                                EmployeeName   = reader["EmployeeName"]   as string,
                                PicturePath    = reader["PicturePath"]    as string,
                                VideoPath      = reader["VideoPath"]      as string,
                                VoicePath      = reader["VoicePath"]      as string
                            });
                        }
                    }
                }

                return Ok(new { Data = list, Message = "Success", ResultType = "Success" });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetCompetitorActivities Failed");
                return Ok(new { Data = (object)null, Message = "Failed to load competitor activities", ResultType = "Exception" });
            }
        }
    }
}
