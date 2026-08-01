using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class SaveCompetitorActivityController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(CompetitorActivityRequest rm)
        {
            try
            {
                if (rm == null)
                    return Fail("Request body is required");

                if (rm.SOID == null || rm.SOID == 0)
                    return Fail("SOID is required");

                if (rm.CompetitorID == null || rm.CompetitorID == 0)
                    return Fail("CompetitorID is required");

                if (rm.ActivityTypeID == null || rm.ActivityTypeID == 0)
                    return Fail("ActivityTypeID is required");

                string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string soLabel = rm.SOID.ToString();

                string picturePath = null;
                string videoPath   = null;
                string voicePath   = null;

                if (!string.IsNullOrEmpty(rm.Picture))
                    picturePath = SaveComplaintController.FileHandler.ConvertImage(
                        rm.Picture, soLabel, stamp, "CompetitorActivities");

                if (!string.IsNullOrEmpty(rm.Video))
                    videoPath = SaveComplaintController.FileHandler.ConvertVideo(
                        rm.Video, soLabel, stamp, "CompetitorActivities");

                if (!string.IsNullOrEmpty(rm.Voice))
                    voicePath = SaveComplaintController.VoiceNoteProcessor.ProcessAndSaveVoiceNote(
                        rm.Voice, soLabel, stamp, "CompetitorActivities");

                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new SqlCommand(
                    @"INSERT INTO dbo.Tbl_CompetitorActivities
                        (SOID, ActivityDate, CompetitorID, ActivityTypeID, Remarks, PicturePath, VideoPath, VoicePath, ZoneID, CityID, CreatedOn, IsActive, IsDeleted)
                      VALUES
                        (@SOID, @ActivityDate, @CompetitorID, @ActivityTypeID, @Remarks, @PicturePath, @VideoPath, @VoicePath, @ZoneID, @CityID, GETDATE(), 1, 0)", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@SOID",           SqlDbType.Int)          { Value = rm.SOID });
                    cmd.Parameters.Add(new SqlParameter("@ActivityDate",    SqlDbType.Date)         { Value = rm.ActivityDate.HasValue ? (object)rm.ActivityDate.Value.Date : DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@CompetitorID",    SqlDbType.Int)          { Value = rm.CompetitorID });
                    cmd.Parameters.Add(new SqlParameter("@ActivityTypeID",  SqlDbType.Int)          { Value = rm.ActivityTypeID });
                    cmd.Parameters.Add(new SqlParameter("@Remarks",         SqlDbType.NVarChar, -1) { Value = (object)rm.Remarks      ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@PicturePath",     SqlDbType.NVarChar, 500){ Value = (object)picturePath      ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@VideoPath",       SqlDbType.NVarChar, 500){ Value = (object)videoPath        ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@VoicePath",       SqlDbType.NVarChar, 500){ Value = (object)voicePath        ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@ZoneID",          SqlDbType.Int)          { Value = (object)rm.ZoneID ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@CityID",          SqlDbType.Int)          { Value = (object)rm.CityID ?? DBNull.Value });
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Competitor activity saved successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "SaveCompetitorActivity Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "An error occurred while saving competitor activity",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        private Result<SuccessResponse> Fail(string message)
        {
            return new Result<SuccessResponse>
            {
                Data = null,
                Message = message,
                ResultType = ResultType.Failure,
                Exception = null,
                ValidationErrors = null
            };
        }
    }

    public class CompetitorActivityRequest
    {
        public int?      SOID           { get; set; }
        public DateTime? ActivityDate   { get; set; }
        public int?      CompetitorID   { get; set; }
        public int?      ActivityTypeID { get; set; }
        public string    Remarks        { get; set; }
        public int?      ZoneID         { get; set; }
        public int?      CityID         { get; set; }
        public string    Picture        { get; set; }
        public string    Video          { get; set; }
        public string    Voice          { get; set; }
    }
}
