using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class HousingVisitsController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest rm)
        { // This controller is for retailers orders.
            JobsDetail jobDet = new JobsDetail();
            var JobObj = new Tbl_HousingVisits();
            var RemObj = new Tbl_HousingTradeCustomers();
            try
            {
                Retailer ret = db.Retailers.Where(r => r.ID == rm.CustomerID).FirstOrDefault();
                if (ret != null)
                {

                    if (rm.OnlineOffline == 1)
                    {
                        var result = db.Database.SqlQuery<SpReturn>("exec spValidateCheckInLatLong @latitude, @longitude, @shopId",
                                  new SqlParameter("latitude", rm.Latitude),
                                  new SqlParameter("longitude", rm.Longitude),
                                  new SqlParameter("shopId", rm.CustomerID)).ToList();

                        if (result.Count() > 0)
                        {
                            JobObj.OnlineOffline = rm.OnlineOffline;
                        }
                        else
                        {
                            return new Result<SuccessResponse>
                            {
                                Data = null,
                                Message = "You are not on the Location. Kindly Add Offline Visit",
                                ResultType = ResultType.Exception,
                                Exception = null,
                                ValidationErrors = null
                            };
                        }

                    }
                    else
                    {
                        JobObj.OnlineOffline = rm.OnlineOffline;
                    }

                    JobObj.CustomerID = rm.CustomerID;
                    JobObj.Remarks = rm.Remarks;
                    JobObj.ArchitectName = rm.ArchitectName;
                    JobObj.ArchitectNumber = rm.ArchitectNumber;
                    JobObj.PlotSize = rm.PlotSize;
                    JobObj.SiteWon = rm.SiteWon;
                    JobObj.OffTakeFromOthers = rm.OffTakeFromOthers;
                    JobObj.ColorScheme = rm.ColorScheme;
                    JobObj.ApproxCoveredArea = rm.ApproxCoveredArea;
                    JobObj.ScopeOfWorkID = rm.ScopeOfWorkID;
                    JobObj.NatureOfWorkID = rm.NatureOfWorkID;
                    JobObj.ContractorName = rm.ContractorName;
                    JobObj.Latitude = rm.Latitude;
                    JobObj.OrderVolume = rm.OrderVolume;
                    JobObj.Longitude = rm.Longitude;
                    JobObj.ContractorNumber = rm.ContractorNumber;
                    JobObj.ConstructionStageID = rm.ConstructionStageID;
                    JobObj.EstimatedValue = rm.EstimatedValue;
                    JobObj.NextVisitDate = rm.NextVisitDate;
                    JobObj.SiteWonMonth = rm.NextVisitDate;
                    JobObj.SOID = rm.SOID;
                    JobObj.IsActive = true;
                    JobObj.CreatedAt = DateTime.UtcNow.AddHours(5);
                    JobObj.AgainCall = rm.AgainCall;


                    db.Tbl_HousingVisits.Add(JobObj);
                    db.SaveChanges();

                    if (rm.HousingVisitsTradeList != null)
                    {



                        List<string> tokens = rm.HousingVisitsTradeList.Split(',').ToList();

                        foreach (var item in tokens)
                        {
                            db.Tbl_HousingTradeCustomers.Add(
                                new Tbl_HousingTradeCustomers
                                {
                                    HousingVisitID = JobObj.ID,
                                    TradeCustomerID = Convert.ToInt32(item),
                                    CreatedAt = DateTime.UtcNow.AddHours(5)


                                });
                            db.SaveChanges();
                        }

                    }
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Visit Submitted Successfully",
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
                    Message = "Housing Visit API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        public string ConvertIntoByte(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }


        public class SuccessResponse
        {

        }
        public class DailyActivityRequest
        {
           
           
            public int CustomerID { get; set; }
            public int ScopeOfWorkID { get; set; }
            public int NatureOfWorkID { get; set; }
            public int SOID { get; set; }
            public int OrderVolume { get; set; }
            public int ConstructionStageID { get; set; }
            public int PlotSize { get; set; }
            public int ApproxCoveredArea { get; set; }
            public string ArchitectName { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public string OffTakeFromOthers { get; set; }
            public bool ColorScheme { get; set; }
            public bool SiteWon { get; set; }
            public int OnlineOffline { get; set; }
            public string ArchitectNumber { get; set; }
            public string ContractorName { get; set; }
            public string ContractorNumber { get; set; }
            public decimal EstimatedValue { get; set; }
            public DateTime SiteWonMonth { get; set; }
            public DateTime NextVisitDate { get; set; }
            public string Remarks { get; set; }
            public string HousingVisitsTradeList { get; set; }
            public string AgainCall { get; set; }

           // public List<JobItemModel> HousingVisitsTradeList { get; set; }

        }

        public class JobItemModel
        {
            public int HousingVisitID { get; set; }
            public int CustomerID { get; set; }
           
        }
    }
}