using FOS.DataLayer;
//using FOS.DataLayerM;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class ManageRetailer
    {
        public static List<RetailerData> GetAllRetailersList()
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                retailerData = dbContext.Retailers.Select(r => new RetailerData
                {
                    ID = r.ID,
                    ShopName = r.ShopName
                }).ToList();
            }
            return retailerData;
        }


        public static List<RetailerData> GetAllRetailersRegionAndCity(int? RegionID,int? CityID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if(RegionID>0 && CityID>0)
                {
                    retailerData = dbContext.Retailers.Where(u => u.CityID == CityID && u.RegionID == RegionID && u.IsActive == true && u.IsDeleted == false).Select(r => new RetailerData
                    {
                        ID = r.ID,
                        ShopName = r.ShopName
                    }).ToList();
                    retailerData.Insert(0, new RetailerData
                    {
                        ID = 0,
                        ShopName = "All"
                    });
                }
               else if(RegionID>0 && CityID == 0)
                {
                    retailerData = dbContext.Retailers.Where(u => u.RegionID == RegionID && u.IsActive == true && u.IsDeleted == false).Select(r => new RetailerData
                    {
                        ID = r.ID,
                        ShopName = r.ShopName
                    }).ToList();
                    retailerData.Insert(0, new RetailerData
                    {
                        ID = 0,
                        ShopName = "All"
                    });
                }
              
            }
            return retailerData;
        }
        public List<Sp_SOVisitsToday1_1_Result> SOVisitsToday()
        {
            List<Sp_SOVisitsToday1_1_Result> RetailerObj = new List<Sp_SOVisitsToday1_1_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_SOVisitsToday1_1(dtFromToday,dtToToday).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_SOVisitsTodayRangeWise_Result> SOVisitsTodayForAdmin(int? ID, DateTime StartingDate, DateTime end)
        {
            List<Sp_SOVisitsTodayRangeWise_Result> RetailerObj = new List<Sp_SOVisitsTodayRangeWise_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_SOVisitsTodayRangeWise(StartingDate, end,ID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_SOVisitsTodayForManagers_Result> SOVisitsTodayForManagers(int? ID)
        {
            List<Sp_SOVisitsTodayForManagers_Result> RetailerObj = new List<Sp_SOVisitsTodayForManagers_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_SOVisitsTodayForManagers(dtFromToday, dtToToday,ID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }
        public List<Sp_SODistributorVisits_Result> DistributorVisitsToday()
        {
            List<Sp_SODistributorVisits_Result> RetailerObj = new List<Sp_SODistributorVisits_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_SODistributorVisits(dtFromToday, dtToToday).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_SODistributorVisitsRangeWise_Result> DistributorVisitsTodayForAdmin(int ? ID)
        {
            List<Sp_SODistributorVisitsRangeWise_Result> RetailerObj = new List<Sp_SODistributorVisitsRangeWise_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_SODistributorVisitsRangeWise(dtFromToday, dtToToday,ID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }
        public List<Sp_SODistributorVisitsForManagers_Result> DistributorVisitsTodayForManagers(int? ID)
        {
            List<Sp_SODistributorVisitsForManagers_Result> RetailerObj = new List<Sp_SODistributorVisitsForManagers_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_SODistributorVisitsForManagers(dtFromToday, dtToToday,ID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<spGetSalesOfficerWithLoginDate_Result> TodayPresentSalesOfficer(int TID, DateTime startDate, DateTime endDate)
        {
            List<spGetSalesOfficerWithLoginDate_Result> RetailerObj = new List<spGetSalesOfficerWithLoginDate_Result>();
            //   var RetailerObj;
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.spGetSalesOfficerWithLoginDate(TID, startDate, endDate).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<EmployeeAttendanceTimingData> TodayPresentSalesOfficerSync(int TID, DateTime startDate, DateTime endDate)
        {
            List<EmployeeAttendanceTimingData> RetailerObj = new List<EmployeeAttendanceTimingData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                using (var conn = new System.Data.SqlClient.SqlConnection(dbContext.Database.Connection.ConnectionString))
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.spGetEmployeeAttendanceTimings", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RID", TID);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RetailerObj.Add(new EmployeeAttendanceTimingData
                            {
                                AttendanceDate = reader["AttendanceDate"] as DateTime?,
                                Emp_Code = reader["Emp_Code"] as string,
                                Emp_Name = reader["Emp_Name"] as string,
                                Start_Time = reader["Start_Time"] as string,
                                End_Time = reader["End_Time"] as string,
                                Last_Visit_Time = reader["Last_Visit_Time"] as string,
                                Working_Minutes = reader["Working_Minutes"] as int?,
                                Working_Hours = reader["Working_Hours"] as string,
                                RegionalHead = reader["RegionalHead"] as string,
                                City = reader["City"] as string,
                                MarketStartLatlong = reader["MarketStartLatlong"] as string,
                                MarketCloseLatlong = reader["MarketCloseLatlong"] as string
                            });
                        }
                    }
                }
            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public static List<RetailerData> GetRetailerBySOID(int soId)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                retailerData = dbContext.Retailers.Where(p => p.SaleOfficerID == soId).Select(r => new RetailerData
                {
                    ID = r.ID,
                    Name = r.ShopName
                }).ToList();
            }
            return retailerData;
        }
        public static List<RetailerData> GetRetailerBySOIDAndCity(int soId, int cityId, int areaId)
        {
            List<RetailerData> retailerData = new List<RetailerData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                retailerData = dbContext.Retailers.Where(p => p.SaleOfficerID == soId && p.CityID == cityId && p.AreaID == areaId).Select(r => new RetailerData
                {
                    ID = r.ID,
                    Name = r.ShopName
                }).ToList();
                retailerData.Insert(0, new RetailerData
                {
                    ID = 0,
                    Name = "All"
                });
            }
            return retailerData;
        }
        public List<Sp_AbsentSOBarGraph_Result> TotalPresentSO()
        {
            List<Sp_AbsentSOBarGraph_Result> RetailerObj = new List<Sp_AbsentSOBarGraph_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_AbsentSOBarGraph(dtFromToday,dtToToday).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public static RetailerData GetRetailerByID(int ID)
        {
            RetailerData retailerData = new RetailerData();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                retailerData = dbContext.Retailers.Where(p => p.ID == ID).Select(u => new RetailerData
                {
                    ID = u.ID,
                    Name = u.Name,
                    ShopName = u.ShopName,
                    //Location = u.Location,
                    //LocationMargin = u.LocationMargin,
                    //LocationName = u.LocationName,
                    RetailerType = u.RetailerType,
                    //Lattitude = u.Latitude,
                    //Longitude = u.Longitude,
                    Address = u.Address == null ? "" : u.Address,
                    Phone1 = u.Phone1,

                    DealerName = u.Dealer.Name,
                    SaleOfficerName = u.SaleOfficer.Name,
                    CItyName = u.City.Name,
                    AreaName = u.Area.Name,
                    Phone2 = u.Phone2

                }).FirstOrDefault();
            }
            return retailerData;
        }


        //public static List<RetailerData> GetDealerRetailers(int DID)
        //{
        //    List<RetailerData> retailerData = new List<RetailerData>();
        //    using (FOSDataModel dbContext = new FOSDataModel())
        //    {
        //        retailerData = dbContext.Retailers.Where(r => r.DealerID == DID).Select(r => new RetailerData
        //        {
        //            ID = r.ID,
        //            ShopName = r.ShopName
        //        }).ToList();
        //    }
        //    return retailerData;
        //}

        //public static List<int> GetDealerRetailerCityIdsList(int DID)
        //{
        //    List<int> retailerCityData = new List<int>();
        //    using (FOSDataModel dbContext = new FOSDataModel())
        //    {
        //        retailerCityData = dbContext.Retailers.Where(r => r.DealerID == DID
        //        && r.IsActive && r.Status && !r.IsDeleted).Select(r => r.CityID ?? 0).ToList();
        //    }
        //    return retailerCityData;
        //}

        // Get All Retailers List ...
        public static List<Retailer> GetAllRetailerList()
        {
            List<Retailer> retailerData = new List<Retailer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                retailerData = dbContext.Retailers.ToList();
            }
            return retailerData;
        }

        // Get All Cities Related To Region ...
        public static List<CityData> GetCitiesRelatedToRegion(int RegionID)
        {
            List<CityData> cityData = new List<CityData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityData = dbContext.Cities
                        .Where(c => c.RegionID == RegionID)
                        .Select(u => new CityData
                        {
                            ID = u.ID,
                            Name = u.Name
                        }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return cityData;
        }

        // Get All Retailer Related To DealerID & SalesOfficerID ...
        public static List<RetailerData> GetAllRetailerSaleOfficerList(int RegionalHeadID, int SoID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    retailerData = dbContext.Retailers.Where(r => r.SaleOfficerID == SoID && r.SaleOfficer.RegionalHeadID == RegionalHeadID && r.Status == true && r.JobsDetails.Where(s => s.Retailer.ID == r.ID && s.Job.IsDeleted == false).Select(s => s.RetailerID).Count() == 0)
                            .AsEnumerable()

                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                   // DealerID = u.DealerID,
                                    DealerName = u.Dealer.Name,
                                    Address = u.Address,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationMargin = u.LocationMargin,
                                    //RetailerJobStatus = u.SoRetailersJobs.Where(s => s.Retailer.ID == s.RetailerID).Select(s => s.RetailerID).Count() == 0 ? true : false,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    RetailerType = u.RetailerType,
                                    LastUpdate = (DateTime)u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retailerData;
        }

        // Get All Retailers According To SalesOfficerID ...
        public static List<RetailerData> GetAllRetailerSaleOfficerList(int SoID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    retailerData = dbContext.Retailers.Where(r => r.SaleOfficerID == SoID && r.Status == true && r.JobsDetails.Where(s => s.Retailer.ID == s.RetailerID && s.Job.IsDeleted == false).Select(s => s.RetailerID).Count() == 0)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                   // DealerID = u.DealerID,
                                    DealerName = u.Dealer.Name,
                                    Address = u.Address,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationMargin = u.LocationMargin,
                                    //RetailerJobStatus = u.SoRetailersJobs.Where(s => s.Retailer.ID == s.RetailerID).Select(s => s.RetailerID).Count() == 0 ? true : false,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    RetailerType = u.RetailerType,
                                    LastUpdate = (DateTime)u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retailerData;
        }

        // Get One retailer Location ...
        public static RetailerData GetOneRetailerLocation(int RetailerID)
        {
            RetailerData retailerData = new RetailerData();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    retailerData = dbContext.Retailers.Where(u => u.ID == RetailerID)
                            .Select(
                                u => new RetailerData
                                {
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    Address = u.Address,
                                    RetailerType = u.RetailerType,
                                }).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return retailerData;
        }

        public static string GetRetailerLocationsCount(int RegionalHeadID, int DealerID, int SaleOfficerID, int RegionID, int CityID, int ZoneID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var tagCount = dbContext.Retailers.Where(u => u.IsDeleted == false
                    && u.Status == true
                    && u.Dealer.RegionalHeadID == (RegionalHeadID > 0 ? RegionalHeadID : u.Dealer.RegionalHeadID)
                    && u.Dealer.ID == (DealerID > 0 ? DealerID : u.Dealer.ID) 
                    && u.SaleOfficerID == (SaleOfficerID > 0 ? SaleOfficerID : u.SaleOfficerID)
                    && u.SaleOfficer.City.RegionID == (RegionID > 0 ? RegionID : u.SaleOfficer.City.RegionID)
                    && u.CityID == (CityID > 0 ? CityID : u.CityID)
                    && u.ZoneID == (ZoneID > 0 ? ZoneID : u.ZoneID)
                    && u.Location != null).Count();

                    var untagCount = dbContext.Retailers.Where(u => u.IsDeleted == false
                    && u.Status == true
                    && u.Dealer.RegionalHeadID == (RegionalHeadID > 0 ? RegionalHeadID : u.Dealer.RegionalHeadID)
                    && u.Dealer.ID == (DealerID > 0 ? DealerID : u.Dealer.ID)
                    && u.SaleOfficerID == (SaleOfficerID > 0 ? SaleOfficerID : u.SaleOfficerID)
                    && u.SaleOfficer.City.RegionID == (RegionID > 0 ? RegionID : u.SaleOfficer.City.RegionID)
                    && u.CityID == (CityID > 0 ? CityID : u.CityID)
                    && u.ZoneID == (ZoneID > 0 ? ZoneID : u.ZoneID)
                    && u.Location == null).Count();

                    return tagCount + "," + untagCount;
                    //return untagCount + "";

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<RetailerData> GetRetailerLocations(int RegionalHeadID, int DealerID, int SaleOfficerID, int RegionID, int CityID, int ZoneID, string untaggedOnly = null)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (untaggedOnly == null)
                    {
                        retailerData = dbContext.Retailers.Where(u => u.IsDeleted == false
                        && u.Status == true
                        && u.SaleOfficer.RegionalHeadID == (RegionalHeadID > 0 ? RegionalHeadID : u.SaleOfficer.RegionalHeadID)
                       // && u.Dealer.ID == (DealerID > 0 ? DealerID : u.Dealer.ID)
                        && u.SaleOfficerID == (SaleOfficerID > 0 ? SaleOfficerID : u.SaleOfficerID)
                        && u.SaleOfficer.City.RegionID == (RegionID > 0 ? RegionID : u.SaleOfficer.City.RegionID)
                        && u.CityID == (CityID > 0 ? CityID : u.CityID)
                        && u.RegionID == (RegionID > 0 ? RegionID : u.RegionID)
                        && u.Location != null
                        )
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                Name = u.Name,
                                ShopName = u.ShopName,
                                Location = u.Location,
                                LocationMargin = u.LocationMargin,
                                LocationName = u.LocationName,
                                RetailerType = u.RetailerType,
                                Lattitude = u.Latitude,
                                Longitude = u.Longitude,
                                Address = u.Address == null ? "" : u.Address,
                                Phone1 = u.Phone1,

                                //DealerName = u.Dealer.Name,
                                SaleOfficerName = u.SaleOfficer.Name,
                                CItyName = u.City.Name,
                                AreaName = u.Area.Name,
                                Phone2 = u.Phone2

                            }).ToList();
                    }
                    else
                    {
                        retailerData = dbContext.Retailers.Where(u => u.IsDeleted == false
                            && u.Status == true
                           && u.SaleOfficer.RegionalHeadID == (RegionalHeadID > 0 ? RegionalHeadID : u.SaleOfficer.RegionalHeadID)
                           //// && u.Dealer.ID == (DealerID > 0 ? DealerID : u.Dealer.ID)
                            && u.SaleOfficerID == (SaleOfficerID > 0 ? SaleOfficerID : u.SaleOfficerID)
                            && u.SaleOfficer.City.RegionID == (RegionID > 0 ? RegionID : u.SaleOfficer.City.RegionID)
                            && u.CityID == (CityID > 0 ? CityID : u.CityID)
                        && u.RegionID == (RegionID > 0 ? RegionID : u.RegionID)
                            && u.Location == null
                            )
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationMargin = u.LocationMargin,
                                    LocationName = u.LocationName,
                                    RetailerType = u.RetailerType,
                                    Lattitude = u.Latitude,
                                    Longitude = u.Longitude,
                                    Address = u.Address == null ? "" : u.Address,
                                    Phone1 = u.Phone1,

                                   // DealerName = u.Dealer.Name,
                                    SaleOfficerName = u.SaleOfficer.Name,
                                    CItyName = u.City.Name,
                                    AreaName = u.Area.Name,
                                    Phone2 = u.Phone2

                                }).ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return retailerData;
        }

        // Get All Locations ...
        public static List<RetailerData> GetRetailerLocations()
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    retailerData = dbContext.Retailers.Where(u => u.IsDeleted == false && u.Status == true && u.Location != null)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationMargin = u.LocationMargin,
                                    LocationName = u.LocationName,
                                    RetailerType = u.RetailerType,
                                    Address = u.Address == null ? "" : u.Address
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return retailerData;
        }

        // Get All Retailers Location Related To RegionalHeadID ...
        public static List<DealerData> GetAllDealersListRelatedToRegionalHead(int RegionHeadIDID)
        {
            List<DealerData> dealerData = new List<DealerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    dealerData = dbContext.Dealers.Where(r => r.RegionalHeadID == RegionHeadIDID)
                            .Select(
                                u => new DealerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    RegionalHeadID = (int)u.RegionalHeadID
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dealerData;
        }

        // Get All Retailers Location Related To DealerID ...
        public static List<RetailerData> GetRetailerLocationsByRegionalHeadID(int RegionalHeadID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    retailerData = dbContext.Retailers.Where(r => r.IsDeleted == false && r.Status == true && r.Dealer.RegionalHeadID == RegionalHeadID && r.Location != null)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationMargin = u.LocationMargin,
                                    LocationName = u.LocationName,
                                    Address = u.Address == null ? "" : u.Address,
                                    RetailerType = u.RetailerType,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return retailerData;
        }

        // Get All Retailers Location Related To DealerID ...
        public static List<RetailerData> GetRetailerLocationsByDealer(int DealerID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    retailerData = dbContext.Retailers.Where(r => r.IsDeleted == false && r.Status == true && r.DealerID == DealerID && r.Location != null)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationMargin = u.LocationMargin,
                                    LocationName = u.LocationName,
                                    Address = u.Address == null ? "" : u.Address,
                                    RetailerType = u.RetailerType,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return retailerData;
        }

        // Get All Retailers Location Related To SalesOfficer ...
        public static List<RetailerData> GetRetailerLocationsBySaleOfficer(int SaleOfficerID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    retailerData = dbContext.Retailers.Where(r => r.IsDeleted == false && r.Status == true && r.SaleOfficerID == SaleOfficerID && r.Location != null)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationMargin = u.LocationMargin,
                                    LocationName = u.LocationName,
                                    Address = u.Address == null ? "" : u.Address,
                                    RetailerType = u.RetailerType,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return retailerData;
        }

        // Get All Retailers Location Related To RegionID ...
        public static List<RetailerData> GetRetailerLocationsByRegion(int RegionID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    retailerData = dbContext.Retailers.Where(r => r.IsDeleted == false && r.Status == true && r.SaleOfficer.City.RegionID == RegionID && r.Location != null)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationMargin = u.LocationMargin,
                                    LocationName = u.LocationName,
                                    Address = u.Address == null ? "" : u.Address,
                                    RetailerType = u.RetailerType,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retailerData;
        }

        // Get All Retailers Location Related To CityID ...
        public static List<RetailerData> GetRetailerLocationsByCity(int CityID)
        {
            List<RetailerData> retailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    retailerData = dbContext.Retailers.Where(r => r.IsDeleted == false && r.Status == true && r.CityID == CityID && r.Location != null)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationMargin = u.LocationMargin,
                                    LocationName = u.LocationName,
                                    Address = u.Address == null ? "" : u.Address,
                                    RetailerType = u.RetailerType,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retailerData;
        }

        // Update DealerID & Retailer Status ...
        public static Boolean UpdateDealerID(int intRetailerID)
        {
            Boolean boolFlag = false;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Retailer retailerObj = new Retailer();

                        retailerObj = dbContext.Retailers.Where(u => u.ID == intRetailerID).FirstOrDefault();
                        retailerObj.LastUpdate = DateTime.Now;
                        retailerObj.Status = true;
                        dbContext.SaveChanges();

                        boolFlag = true;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Update Retailer Failed");
                boolFlag = false;
            }
            return boolFlag;
        }

        // Insert OR Update Retailer ...
        public static int AddUpdateRetailer(RetailerData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Retailer retailerObj = new Retailer();

                        if (obj.ID == 0)
                        {
                            retailerObj.ID = dbContext.Retailers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                            retailerObj.Name = obj.Name;
                            retailerObj.RetailerCode = obj.RetailerCode;
                            retailerObj.CNIC = obj.CNIC;
                            retailerObj.Email = obj.Email;
                            retailerObj.NoOfBranches = obj.NoOfBranches;
                            retailerObj.NoOfTeachers = obj.NoOfTeachers;
                            retailerObj.StudentStrength = obj.StudentStrength;
                           // retailerObj.CardNumber = obj.CardNumber;
                            retailerObj.SaleOfficerID = obj.SaleOfficerID;
                            retailerObj.DealerID = obj.DealerID;
                            retailerObj.RegionID = obj.RegionID;
                            retailerObj.CityID = obj.CityID;
                            retailerObj.AreaID = obj.AreaID;

                            retailerObj.ShopName = obj.ShopName;
                            retailerObj.Location = obj.Location;
                            retailerObj.Address = obj.Address;
                            retailerObj.RetailerType = obj.RetailerType;
                            retailerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                            retailerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                            retailerObj.LandLineNo = obj.ContactPersonCell;
                            //retailerObj.Market = obj.Market;
                            retailerObj.IsActive = true;
                            retailerObj.Status = true;
                            retailerObj.LastUpdate = DateTime.Now;
                            retailerObj.CreatedDate = DateTime.Now;
                            retailerObj.CreatedBy = obj.CreatedBy;
                            //retailerObj.Type = "WALLCOAT";
                            retailerObj.Source = (int)RetSourceEnum.Web;
                            dbContext.Retailers.Add(retailerObj);
                        }
                        else
                        {
                            retailerObj = dbContext.Retailers.Where(u => u.ID == obj.ID).FirstOrDefault();

                            retailerObj.Name = obj.Name;
                            retailerObj.RetailerCode = obj.RetailerCode;
                            retailerObj.CNIC = obj.CNIC;
                            retailerObj.Email = obj.Email;
                            retailerObj.NoOfBranches = obj.NoOfBranches;
                            retailerObj.NoOfTeachers = obj.NoOfTeachers;
                            retailerObj.StudentStrength = obj.StudentStrength;
                            // retailerObj.CardNumber = obj.CardNumber;
                            retailerObj.SaleOfficerID = obj.SaleOfficerID;
                            // retailerObj.DealerID = obj.DealerID;
                            retailerObj.CityID = obj.CityID;
                            retailerObj.AreaID = obj.AreaID;

                            retailerObj.ShopName = obj.ShopName;
                            retailerObj.Location = obj.Location;
                            retailerObj.Address = obj.Address;
                            retailerObj.RetailerType = obj.RetailerType;
                            retailerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                            retailerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                            retailerObj.LandLineNo = obj.ContactPersonCell;
                            //retailerObj.Market = obj.Market;
                            retailerObj.IsActive = true;
                            retailerObj.Status = true;
                            retailerObj.LastUpdate = DateTime.Now;
                            retailerObj.CreatedBy = obj.CreatedBy;
                            //retailerObj.Type = "WALLCOAT";
                            retailerObj.Source = (int)RetSourceEnum.Web;
                            retailerObj.UpdatedBy = obj.UpdatedBy;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Customer Failed");
                if (exp.InnerException.InnerException.Message.Contains("CNIC"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 3;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("AccountNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 4;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("CardNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 5;
                    return Res;
                }
                Res = 0;
            }
            return Res;
        }

        public static int AddUpdateFannanRetailer(RetailerData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_FannanCustomer retailerObj = new Tbl_FannanCustomer();

                      
                            retailerObj.ContactPerson = obj.Name;
                            retailerObj.SiteCode = obj.RetailerCode;
                            retailerObj.CNIC = obj.CNIC;
                            retailerObj.Email = obj.Email;
                            retailerObj.SiteName = obj.ShopName;
                         
                            
                            retailerObj.Address = obj.Address;
                           
                            retailerObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                            retailerObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                          
                            //retailerObj.Market = obj.Market;
                            retailerObj.IsActive = true;
                          
                            retailerObj.CreatedOn = DateTime.Now;
                            
                            dbContext.Tbl_FannanCustomer.Add(retailerObj);
                        
                       

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Customer Failed");
                if (exp.InnerException.InnerException.Message.Contains("CNIC"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 3;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("AccountNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 4;
                    return Res;
                }

                if (exp.InnerException.InnerException.Message.Contains("CardNo"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 5;
                    return Res;
                }
                Res = 0;
            }
            return Res;
        }


        public static int UndoRetailer(int RetailerID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Retailer obj = dbContext.Retailers.Where(u => u.TID == RetailerID).FirstOrDefault();
                    Retailer ret;
                    ret = dbContext.Retailers.Where(p => p.ID == obj.ID).FirstOrDefault();
                    ret.IsDeleted = false;
                    ret.Status = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Retailer Failed");
                Resp = 1;
            }
            return Resp;
        }
        // Delete Retailer ...
        public static int DeleteRetailer(int RetailerID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Retailer obj = dbContext.Retailers.Where(u => u.ID == RetailerID).FirstOrDefault();
                    dbContext.Retailers.Remove(obj);
                    //obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Retailer Failed");
                Resp = 1;
            }
            return Resp;
        }



        public static int DeleteOrder(int OrderID)
        {
            int Resp = 0;

            try
            {
                
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var obj = dbContext.JobsDetails.Where(u => u.JobID == OrderID).FirstOrDefault();
                    obj.Status = false;
                    //obj.IsDeleted = true;
                    var obj2 = dbContext.Jobs.Where(u => u.ID == OrderID).FirstOrDefault();
                    obj2.IsActive = false;
                    obj2.IsDeleted = true;

                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Retailer Failed");
                Resp = 1;
            }
            return Resp;
        }


        public static int DeleteKPI(int KPIID)
        {
            int Resp = 0;

            try
            {

                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_MasterKPIS obj = dbContext.Tbl_MasterKPIS.Where(u => u.ID == KPIID).FirstOrDefault();
                    obj.IsActive = false;
                  

                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Retailer Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static int DeleteDistributor(int RetailerID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Dealer obj = dbContext.Dealers.Where(u => u.ID == RetailerID).FirstOrDefault();
                    //dbContext.Retailers.Remove(obj);
                    obj.IsDeleted = true;
                    obj.IsActive = false;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Retailer Failed");
                Resp = 1;
            }
            return Resp;
        }
        public static int DeleteRetailerForApproval(int TID, int deletedBy)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailersForApproval obj = dbContext.RetailersForApprovals.Where(u => u.TID == TID).FirstOrDefault();
                    obj.IsDeleted = true;
                    obj.UpdatedBy = deletedBy;
                    obj.UpdatedOn = DateTime.Now;
                    obj.Source = (int)RetSourceEnum.Web;
                    obj.Action = (int)RetActionEnum.Deleted;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Retailer for approval Failed");
                Resp = 1;
            }
            return Resp;
        }
        public static int ApproveRetailerForDelete(int TID, RetActionEnum action, int approvedBy)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Retailer obj = dbContext.Retailers.Where(u => u.TID == TID).FirstOrDefault();
                    obj.Source = (int)RetSourceEnum.Web;
                    if (action == RetActionEnum.Update)
                    {
                        obj.Action = (int)RetActionEnum.UpdateApproved;
                    }
                    else
                    {
                        obj.Action = (int)RetActionEnum.AddApproved;
                    }

                    Retailer ret;

                    if (action == RetActionEnum.Update)
                    {
                        ret = dbContext.Retailers.Where(p => p.ID == obj.ID).FirstOrDefault();
                        ret.Status = false;
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "ApproveRetailerForApproval failed");
                Resp = 1;
            }
            return Resp;
        }
        public static int ApproveRetailerForApproval(int TID, RetActionEnum action, int approvedBy)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailersForApproval obj = dbContext.RetailersForApprovals.Where(u => u.TID == TID).FirstOrDefault();
                    obj.UpdatedBy = approvedBy;
                    obj.UpdatedOn = DateTime.Now;
                    obj.Source = (int)RetSourceEnum.Web;
                    if(action == RetActionEnum.Update)
                    {
                        obj.Action = (int)RetActionEnum.UpdateApproved;
                    }
                    else
                    {
                        obj.Action = (int)RetActionEnum.AddApproved;
                    }

                    Retailer ret;

                    if (action == RetActionEnum.Update)
                    {
                        ret = dbContext.Retailers.Where(p => p.ID == obj.ID).FirstOrDefault();
                        ret.Action = (int)RetActionEnum.UpdateApproved;
                        ret.UpdateSource = (int)RetSourceEnum.Mobile;
                        ret.UpdatedBy = obj.SaleOfficerID;
                    }
                    else
                    {
                        ret = new Retailer();
                        ret.ID = dbContext.Retailers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        
                        // set retailer id also to approval table
                        obj.ID = ret.ID;
                        //////////////

                        ret.CreatedBy = obj.SaleOfficerID;
                        ret.CreatedOn = DateTime.Now;
                        ret.LastUpdate = DateTime.Now;
                        ret.IsActive = true;
                        ret.Status = true;
                        ret.RetailerType = obj.RetailerType;
                        ret.SaleOfficerID = obj.SaleOfficerID ?? 0;
                        ret.Type = obj.Type;
                        ret.Action = (int)RetActionEnum.AddApproved;
                        ret.Source = (int)RetSourceEnum.Mobile;
                    }

                    if(ret != null)
                    {
                        ret.ShopName = obj.ShopName;
                        ret.Name = obj.Name;
                        ret.DealerID = obj.DealerID;
                        ret.CityID = obj.CityID;
                        ret.AreaID = obj.AreaID;
                        ret.ZoneID = obj.ZoneID;
                        ret.Phone1 = obj.Phone1;
                        ret.Phone2 = obj.Phone2;
                        ret.Address = obj.Address;

                        ret.AccountNo = obj.AccountNo;
                        ret.AccountNo2 = obj.AccountNo2;
                        ret.BankName = obj.BankName;
                        ret.BankName2 = obj.BankName2;

                        ret.LastUpdate = DateTime.Now;
                        ret.IsVerified = true;
                    }
                    if (action == RetActionEnum.Add)
                    {
                        dbContext.Retailers.Add(ret);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "ApproveRetailerForApproval failed");
                Resp = 1;
            }
            return Resp;
        }

        public static int ResetRetailerLatLongApproval(int TID, RetActionEnum action = RetActionEnum.ResetLoc)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailersForApproval obj = dbContext.RetailersForApprovals.Where(u => u.TID == TID).FirstOrDefault();
                    obj.UpdatedBy = 1;
                    obj.UpdatedOn = DateTime.Now;
                    obj.Source = (int)RetSourceEnum.Web;
                    if (action == RetActionEnum.ResetLoc)
                    {
                        obj.Action = (int)RetActionEnum.ResetLocApproved;
                    }
                    else
                    {
                        obj.Action = (int)RetActionEnum.UpdateLocApproved;
                    }

                    Retailer retailerObj = new Retailer();

                    retailerObj = dbContext.Retailers.Where(u => u.ID == obj.ID).FirstOrDefault();

                    retailerObj.Latitude = null;
                    retailerObj.Longitude = null;
                    retailerObj.Location = null;
                    retailerObj.LocationName = null;

                    retailerObj.Action = (int)RetActionEnum.ResetLocApproved;
                    retailerObj.UpdateSource = (int)RetSourceEnum.Mobile;
                    retailerObj.LastUpdate = DateTime.Now;


                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "ResetRetailerLatLongApproval failed");
                Resp = 1;
            }
            return Resp;
        }
        // Get All Pending Retailer For Grid ...
        public static List<RetailerPendingData> GetPendingRetailerForGrid(int RegionalHeadID)
        {
            List<RetailerPendingData> RetailerData = new List<RetailerPendingData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailerData = dbContext.Retailers.Where(u => u.Status == false && u.IsDeleted == false && u.SaleOfficer.RegionalHeadID == RegionalHeadID)
                            .ToList().Select(
                                u => new RetailerPendingData
                                {
                                    ID = u.ID,
                                    DealerID = u.DealerID,
                                    DealerName = u.Dealer.Name,
                                    SaleOfficerID = u.SaleOfficerID,
                                    RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location,
                                    LocationName = u.LocationName,
                                    CityID = (int)u.SaleOfficer.CityID,
                                    CityName = u.SaleOfficer.City.Name,
                                    //AreaID = (int)u.AreaID,
                                    //AreaName = FOS.Setup.ManageSaleOffice.GetSaleOfficerAreaName(u.SaleOfficerID),
                                    SaleOfficerName = u.SaleOfficer.Name,
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Pending Retailer List Failed");
                throw;
            }

            return RetailerData;
        }

        // Get All Pending Retailer For Grid ...
        public static List<RetailerPendingData> GetPendingRetailerForGrid()
        {
            List<RetailerPendingData> RetailerData = new List<RetailerPendingData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailerData = dbContext.Retailers.Where(u => u.Status == false && u.IsDeleted == false)
                            .ToList().Select(
                                u => new RetailerPendingData
                                {
                                    ID = u.ID,
                                    DealerID = u.DealerID,

                                    SaleOfficerID = u.SaleOfficerID,
                                    RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                                    DealerName = u.Dealer.Name,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    Location = u.Location == null ? "" : u.Location,
                                    LocationName = u.LocationName == null ? "" : u.LocationName,
                                    // CityID = (int)u.SaleOfficer.CityID,
                                    //CityName = dbContext.Cities.Where(c =>c.ID == u.CityID).Select(c => c.Name).FirstOrDefault(),
                                    //AreaID = (int)u.AreaID,
                                    // AreaName = FOS.Setup.ManageSaleOffice.GetSaleOfficerAreaName(u.SaleOfficerID),
                                    SaleOfficerName = u.SaleOfficer.Name,
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Pending Retailer List Failed");
                throw;
            }

            return RetailerData;
        }

        public static List<RetailerPendingData> GetPedingResult(string search, string sortOrder, int start, int length, List<RetailerPendingData> dtResult, List<string> columnFilters)
        {
            return FilterPendingResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int PendingCount(string search, List<RetailerPendingData> dtResult, List<string> columnFilters)
        {
            return FilterPendingResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<RetailerPendingData> FilterPendingResult(string search, List<RetailerPendingData> dtResult, List<string> columnFilters)
        {
            IQueryable<RetailerPendingData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.ShopName != null && p.ShopName.ToLower().Contains(search.ToLower()) || p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.ShopName != null && p.ShopName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(columnFilters[4].ToLower())))
                );

            return results;
        }

        // Get All Pending Retailer For APPROVAL Grid ...
        public static List<RetailerPendingData> GetPendingRetailerForApprovalGrid(RetActionEnum action, int regHeadId = 0)
        {
            List<RetailerPendingData> RetailerData = new List<RetailerPendingData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (RetActionEnum.Add == action || RetActionEnum.Update == action)
                    {
                        RetailerData = dbContext.RetailersForApprovals.Where(u => u.Action == (int)action && u.IsDeleted == false)
                            .Select(
                                u => new RetailerPendingData
                                {
                                    TID = u.TID,
                                    ID = u.ID,
                                    //DealerID = u.DealerID,
                                    //SaleOfficerID = u.SaleOfficerID ?? 0,
                                    //RegionalHeadID = 0,
                                    DealerName = dbContext.Dealers.Where(p => p.ID == u.DealerID).FirstOrDefault().Name,
                                    Name = u.Name,
                                    ShopName = u.ShopName,
                                    PersonName = u.Name,
                                    //Location = u.Location == null ? "" : u.Location,
                                    //LocationName = u.LocationName == null ? "" : u.LocationName,
                                    //CityID = u.CityID ?? 0,
                                    CityName = dbContext.Cities.Where(c => c.ID == u.CityID).Select(c => c.Name).FirstOrDefault(),
                                    ZoneName = dbContext.Zones.Where(c => c.ID == u.ZoneID).Select(c => c.Name).FirstOrDefault(),
                                    //ZoneID = u.ZoneID ?? 0,
                                    //AreaID = u.AreaID ?? 0,
                                    AreaName = dbContext.Areas.Where(c => c.ID == u.AreaID).Select(c => c.Name).FirstOrDefault(),
                                    SaleOfficerName = dbContext.SaleOfficers.Where(p => p.ID == u.SaleOfficerID).FirstOrDefault().Name,
                                    Phone1 = u.Phone1,
                                    Address = u.Address,
                                    BankName = u.BankName,
                                    AccountNo = u.AccountNo,
                                    BankName2 = u.BankName2,
                                    AccountNo2 = u.AccountNo2,
                                    CreatedOn = u.CreatedOn
                                }).ToList();

                        if (RetailerData.Count > 0)
                        {
                            foreach (var ret in RetailerData)
                            {
                                ret.CreatedOnStr = ret.CreatedOn.HasValue ? ret.CreatedOn.Value.ToString("dd-MMM-yyyy HH:mm") : "";
                            }

                            if (RetActionEnum.Update == action)
                            {
                                foreach (var ret in RetailerData)
                                {
                                    var dbRet = dbContext.Retailers.Where(x => x.ID == ret.ID).FirstOrDefault();
                                    if (dbRet != null)
                                    {
                                        if (!dbRet.ShopName.Equals(ret.ShopName))
                                        {
                                            ret.ShopName = "<label style='background-color:yellow'>" + ret.ShopName + "</label>";
                                        }
                                        if (!string.IsNullOrEmpty(dbRet.Name) 
                                            && !string.IsNullOrEmpty(ret.Name) 
                                            && !dbRet.Name.Equals(ret.Name))
                                        {
                                            ret.Name = "<label style='background-color:yellow'>" + ret.Name + "</label>";
                                        }
                                        if (string.IsNullOrEmpty(dbRet.Name)
                                            && !string.IsNullOrEmpty(ret.Name))
                                        {
                                            ret.Name = "<label style='background-color:yellow'>" + ret.Name + "</label>";
                                        }
                                        if (dbRet.DealerID.HasValue && ret.DealerID.HasValue &&
                                            dbRet.DealerID.Value != ret.DealerID.Value)
                                        {
                                            ret.DealerName = "<label style='background-color:yellow'>" + ret.DealerName + "</label>";
                                        }
                                        if (!dbRet.DealerID.HasValue && ret.DealerID.HasValue)
                                        {
                                            ret.DealerName = "<label style='background-color:yellow'>" + ret.DealerName + "</label>";
                                        }
                                        if (dbRet.CityID.HasValue &&
                                            dbRet.CityID.Value != ret.CityID)
                                        {
                                            ret.CityName = "<label style='background-color:yellow'>" + ret.CityName + "</label>";
                                        }
                                        if (!dbRet.CityID.HasValue)
                                        {
                                            ret.CityName = "<label style='background-color:yellow'>" + ret.CityName + "</label>";
                                        }
                                        if (dbRet.AreaID.HasValue &&
                                            dbRet.AreaID.Value != ret.AreaID)
                                        {
                                            ret.AreaName = "<label style='background-color:yellow'>" + ret.AreaName + "</label>";
                                        }
                                        if (!dbRet.AreaID.HasValue)
                                        {
                                            ret.AreaName = "<label style='background-color:yellow'>" + ret.AreaName + "</label>";
                                        }
                                        if (dbRet.ZoneID.HasValue &&
                                            dbRet.ZoneID.Value != ret.ZoneID)
                                        {
                                            ret.ZoneName = "<label style='background-color:yellow'>" + ret.ZoneName + "</label>";
                                        }
                                        if (!dbRet.ZoneID.HasValue)
                                        {
                                            ret.ZoneName = "<label style='background-color:yellow'>" + ret.ZoneName + "</label>";
                                        }
                                        if (!string.IsNullOrEmpty(dbRet.Phone1)
                                            && !string.IsNullOrEmpty(ret.Phone1)
                                            && !dbRet.Phone1.Equals(ret.Phone1))
                                        {
                                            ret.Phone1 = "<label style='background-color:yellow'>" + ret.Phone1 + "</label>";
                                        }
                                        if (string.IsNullOrEmpty(dbRet.Phone1)
                                            && !string.IsNullOrEmpty(ret.Phone1))
                                        {
                                            ret.Phone1 = "<label style='background-color:yellow'>" + ret.Phone1 + "</label>";
                                        }
                                        if (!string.IsNullOrEmpty(dbRet.Address)
                                            && !string.IsNullOrEmpty(ret.Address)
                                            && !dbRet.Address.Equals(ret.Address))
                                        {
                                            ret.Address = "<label style='background-color:yellow'>" + ret.Address + "</label>";
                                        }
                                        if (string.IsNullOrEmpty(dbRet.Address)
                                            && !string.IsNullOrEmpty(ret.Address))
                                        {
                                            ret.Address = "<label style='background-color:yellow'>" + ret.Address + "</label>";
                                        }
                                        if (!string.IsNullOrEmpty(dbRet.BankName)
                                            && !string.IsNullOrEmpty(ret.BankName)
                                            && !dbRet.BankName.Equals(ret.BankName))
                                        {
                                            ret.BankName = "<label style='background-color:yellow'>" + ret.BankName + "</label>";
                                        }
                                        if (string.IsNullOrEmpty(dbRet.BankName)
                                            && !string.IsNullOrEmpty(ret.BankName))
                                        {
                                            ret.BankName = "<label style='background-color:yellow'>" + ret.BankName + "</label>";
                                        }
                                        if (!string.IsNullOrEmpty(dbRet.BankName2)
                                            && !string.IsNullOrEmpty(ret.BankName2)
                                            && !dbRet.BankName2.Equals(ret.BankName2))
                                        {
                                            ret.BankName2 = "<label style='background-color:yellow'>" + ret.BankName2 + "</label>";
                                        }
                                        if (string.IsNullOrEmpty(dbRet.BankName2)
                                            && !string.IsNullOrEmpty(ret.BankName2))
                                        {
                                            ret.BankName2 = "<label style='background-color:yellow'>" + ret.BankName2 + "</label>";
                                        }
                                        if (!string.IsNullOrEmpty(dbRet.AccountNo)
                                            && !string.IsNullOrEmpty(ret.AccountNo)
                                            && !dbRet.AccountNo.Equals(ret.AccountNo))
                                        {
                                            ret.AccountNo = "<label style='background-color:yellow'>" + ret.AccountNo + "</label>";
                                        }
                                        if (string.IsNullOrEmpty(dbRet.AccountNo)
                                            && !string.IsNullOrEmpty(ret.AccountNo))
                                        {
                                            ret.AccountNo = "<label style='background-color:yellow'>" + ret.AccountNo + "</label>";
                                        }
                                        if (!string.IsNullOrEmpty(dbRet.AccountNo2)
                                            && !string.IsNullOrEmpty(ret.AccountNo2)
                                            && !dbRet.AccountNo2.Equals(ret.AccountNo2))
                                        {
                                            ret.AccountNo2 = "<label style='background-color:yellow'>" + ret.AccountNo2 + "</label>";
                                        }
                                        if (string.IsNullOrEmpty(dbRet.AccountNo2)
                                            && !string.IsNullOrEmpty(ret.AccountNo2))
                                        {
                                            ret.AccountNo2 = "<label style='background-color:yellow'>" + ret.AccountNo2 + "</label>";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var qry = dbContext.RetailersForApprovals.Where(u => u.Action == (int)action && u.IsDeleted == false)
                            .Select(
                                u => new RetailerPendingData
                                {
                                    ID = u.ID,
                                    TID = u.TID,
                                    Location = u.Location == null ? "" : u.Location,
                                    LocationName = u.LocationName == null ? "" : u.LocationName,
                                    CreatedOn = u.CreatedOn
                                });
                        RetailerData = qry.ToList();

                        foreach (var ret in RetailerData)
                        {
                            ret.CreatedOnStr = ret.CreatedOn.HasValue ? ret.CreatedOn.Value.ToString("dd-MMM-yyyy HH:mm") : "";
                            var retailer = dbContext.Retailers.Where(p => p.ID == ret.ID).FirstOrDefault();
                            if (retailer != null)
                            {
                                ret.DealerID = retailer.DealerID;
                                ret.SaleOfficerName = retailer.SaleOfficer.Name;
                                ret.SaleOfficerID = retailer.ID;
                                ret.RegionalHeadID = retailer.SaleOfficer.RegionalHeadID;
                                ret.DealerName = retailer.Dealer.Name;
                                ret.Name = retailer.Name;
                                ret.ShopName = retailer.ShopName;
                                ret.Location = retailer.Location;
                                ret.LocationName = retailer.LocationName;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "GetPendingRetailerForApprovalGrid Get Update approval List Failed");
                //throw;
            }

            return RetailerData;
        }

        public static List<RetailerPendingData> GetPendingRetailerForDelete(RetActionEnum action, int regHeadId = 0)
        {
            List<RetailerPendingData> RetailerData = new List<RetailerPendingData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (RetActionEnum.Add == action || RetActionEnum.Update == action)
                    {
                        RetailerData = dbContext.Retailers.Where(u => u.IsDeleted == true && u.Status == true)
                            .Select(
                                u => new RetailerPendingData
                                {
                                    TID = u.TID,
                                    ID = u.ID,
                                    DealerName = dbContext.Dealers.Where(p => p.ID == u.DealerID).FirstOrDefault().ShopName,
                                    ShopName = u.ShopName,
                                    CityName = dbContext.Cities.Where(c => c.ID == u.CityID).Select(c => c.Name).FirstOrDefault(),
                                    Address = u.Address,
                                    SaleOfficerName = dbContext.SaleOfficers.Where(s => s.ID == u.SaleOfficerID).Select(c => c.Name).FirstOrDefault(),
                                    
                                }).ToList();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "GetPendingRetailerForApprovalGrid Get Update approval List Failed");
                //throw;
            }

            return RetailerData;
        }
        public static List<RetailerPendingData> GetPedingApprovalResult(string search, string sortOrder, int start, int length, List<RetailerPendingData> dtResult, List<string> columnFilters)
        {
            return FilterPendingApprovalResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int PendingCountApproval(string search, List<RetailerPendingData> dtResult, List<string> columnFilters)
        {
            return FilterPendingApprovalResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<RetailerPendingData> FilterPendingApprovalResult(string search, List<RetailerPendingData> dtResult, List<string> columnFilters)
        {
            IQueryable<RetailerPendingData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.ShopName != null && p.ShopName.ToLower().Contains(search.ToLower()) || p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.ShopName != null && p.ShopName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(columnFilters[4].ToLower())))
                );

            return results; 
        }

        // Get All Retailer For Grid ...
        public static List<RetailerData> GetRetailerForGrid(int RegionID, int CityID)
        {
            List<RetailerData> RetailerData = new List<RetailerData>();

            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                   
                   
                    

                        RetailerData = dbContext.Retailers.OrderByDescending(r => r.ID).Where(u => u.Status == true && u.IsDeleted == false && u.RegionID == RegionID && u.CityID == CityID)
                         .Select(
                             u => new RetailerData
                             {
                                 ID = u.ID,

                                 RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                                 SaleOfficerID = u.SaleOfficerID,
                                 SaleOfficerName = u.SaleOfficer.Name,
                                 RetailerType = u.RetailerType,
                                 CityID = u.CityID,
                                 CItyName = u.City.Name,
                                 AreaID = (int)u.AreaID,
                                 AreaName = u.Area.Name,
                                 Address = u.Address == null ? "" : u.Address,
                                 Name = u.Name,
                                 RetailerCode = u.RetailerCode,
                                 CNIC = u.CNIC,
                                 ContactPerson = u.ContactPerson,
                                 ContactPersonCell = u.ContactCellNo,

                                 ShopName = u.ShopName,
                                 Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                 Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                 Location = u.Latitude + "," + u.Longitude,
                                 LocationName = u.LocationName,
                                 TypeOfShop = u.TypeOfShop,
                                 ShopCategory = u.ShopCategory,
                                 LocationMargin = u.LocationMargin,
                             }).ToList();
                    
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Retailer List Failed");
                throw;
            }

            return RetailerData;
        }


        public static List<RetailerData> GetDistributorForGrid()
        {
            List<RetailerData> RetailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailerData = dbContext.Dealers.OrderByDescending(r => r.ID).Where(u => u.IsActive == true && u.IsDeleted == false)
                            .ToList().Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,

                                    RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                                    SaleOfficerID = u.SaleOfficerID,
                                    SaleOfficerName = u.SaleOfficer.Name,
                                  //  RetailerType = u.RetailerType,
                                    CityID = u.CityID,
                                    CItyName = u.City.Name,
                                    AreaID = (int)u.AreaID,
                                 //   AreaName = u.Area.Name,
                                    Address = u.Address == null ? "" : u.Address,
                                    Name = u.Name,
                                   // RetailerCode = u.RetailerCode,
                                    CNIC = u.CNIC,
                                 //   ContactPerson = u.ContactPerson,
                                  //  ContactPersonCell = u.ContactCellNo,

                                    ShopName = u.ShopName,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    Location = u.Latitude + "," + u.Longitude,
                                    //LocationName = u.LocationName,
                                    //TypeOfShop = u.TypeOfShop,
                                    //ShopCategory = u.ShopCategory,
                                    //LocationMargin = u.LocationMargin,
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Retailer List Failed");
                throw;
            }

            return RetailerData;
        }


        // Get All Retailer For Grid ...
        public static List<RetailerData> GetRetailerForGrid(int RegionalHeadID)
        {
            List<RetailerData> RetailerData = new List<RetailerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailerData = dbContext.Retailers.Where(u => u.Status == true && u.IsDeleted == false && u.SaleOfficer.RegionalHeadID == RegionalHeadID)
                            .ToList().Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,

                                    RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                                    SaleOfficerID = u.SaleOfficerID,
                                    SaleOfficerName = u.SaleOfficer.Name,
                                    RetailerType = u.RetailerType,
                                    CityID = u.CityID,
                                    CItyName = u.City.Name,
                                    AreaID = (int)u.AreaID,
                                    AreaName = u.Area.Name,
                                    Address = u.Address == null ? "" : u.Address,
                                    Name = u.Name,
                                    RetailerCode = u.RetailerCode,
                                    CNIC = u.CNIC,
                                    ContactPerson = u.ContactPerson,
                                    ContactPersonCell = u.ContactCellNo,

                                    ShopName = u.ShopName,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    Location = u.Latitude + "," + u.Longitude,
                                    LocationName = u.LocationName,
                                    TypeOfShop = u.TypeOfShop,
                                    ShopCategory = u.ShopCategory,
                                    LocationMargin = u.LocationMargin,
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Retailer List Failed");
                throw;
            }

            return RetailerData;
        }

        //public static int GetDeletedRetailerCountApproval()
        //{
        //    try
        //    {
        //        using (FOSDataModel dbContext = new FOSDataModel())
        //        {
        //            return dbContext.Retailers
        //                .Where(x => x.Status == true && x.IsDeleted == true).Count();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        Log.Instance.Error(exp, "GetPendingRetailerCountApproval Failed");
        //        throw;
        //    }
        //}
        public static int GetPendingRetailerCountApproval(int regHdId = 0)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.RetailersForApprovals
                        .Where(u => (u.Action == (int)RetActionEnum.Add ||
                    u.Action == (int)RetActionEnum.Update ||
                    u.Action == (int)RetActionEnum.ResetLoc) && u.IsDeleted == false)
                            .ToList().Count();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "GetPendingRetailerCountApproval Failed");
                throw;
            }
        }
        // Get Pending Retailer Count ...
        public static int GetPendingRetailerCount()
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Retailers.Where(u => u.Status == false && u.IsDeleted == false)
                            .ToList().Count();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Pending Retailer List Failed");
                throw;
            }
        }

        // Get Pending Retailer Count ...
        public static int GetPendingRetailerCount(int RegionalHeadID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Retailers.Where(u => u.Status == false && u.IsDeleted == false && u.SaleOfficer.RegionalHeadID == RegionalHeadID)
                            .ToList().Count();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Pending Retailer List Failed");
                throw;
            }
        }

        public static List<RetailerData> GetResult(string search, string sortOrder, int start, int length, List<RetailerData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<RetailerData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<RetailerData> FilterResult(string search, List<RetailerData> dtResult, List<string> columnFilters)
        {
            IQueryable<RetailerData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.ShopName != null && p.ShopName.ToLower().Contains(search.ToLower())|| p.IsActiveYes != null && p.IsActiveYes.ToLower().Contains(search.ToLower()) || p.Phone1 != null && p.Phone1.ToLower().Contains(search.ToLower()) || p.RangeName != null && p.RangeName.ToLower().Contains(search.ToLower())|| p.CItyName != null && p.CItyName.ToLower().Contains(search.ToLower()) || p.RegionName != null && p.RegionName.ToLower().Contains(search.ToLower())))
               && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))
               && (columnFilters[3] == null || (p.ShopName != null && p.ShopName.ToLower().Contains(columnFilters[3].ToLower())))
               && (columnFilters[4] == null || (p.Phone1 != null && p.Phone1.ToLower().Contains(columnFilters[4].ToLower())))
               );

            return results;
        }

        //DET Edit One City
        public static RetailerData GetEditRetailer(int RetailerID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Retailers.Where(u => u.ID == RetailerID && u.IsActive == true && u.IsDeleted == false).Select(u => new RetailerData
                    {
                        ID = u.ID,
                       // DealerID = u.DealerID,
                        DealerName = u.Dealer.Name,
                        RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                        SaleOfficerID = u.SaleOfficerID,
                        SaleOfficerName = u.SaleOfficer.Name,
                        RetailerType = u.RetailerType,
                        CityID = u.CityID,
                        CItyName = u.City.Name,
                        AreaID = (int)u.AreaID == null ? dbContext.Areas.Where(a => a.CityID == u.CityID).Select(a => a.ID).FirstOrDefault() : (int)u.AreaID,
                        Address = u.Address == null ? "" : u.Address,
                        Name = u.Name,
                        RetailerCode = u.RetailerCode,
                        CNIC = u.CNIC,
                        AccountNo = u.AccountNo,
                        Email = u.Email,
                        AccountNo2 = u.AccountNo2,
                        BankName2 = u.BankName2,
                        CardNumber = u.CardNumber,
                        ShopName = u.ShopName,
                        Phone1 = u.Phone1 == null ? "" : u.Phone1,
                        Phone2 = u.Phone2 == null ? "" : u.Phone2,
                        LandLineNo = u.LandLineNo,
                        Market = u.Market,
                        Location = u.Location,
                        LocationName = u.LocationName,
                        LocationMargin = u.LocationMargin,
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }




        public static Boolean ResetRetailerLatLong(int intRetailerID)
        {
            Boolean boolFlag = false;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Retailer retailerObj = new Retailer();

                        retailerObj = dbContext.Retailers.Where(u => u.ID == intRetailerID).FirstOrDefault();
                        retailerObj.Location = null;
                        retailerObj.LocationName = null;
                        dbContext.SaveChanges();

                        boolFlag = true;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Reset Retailer LatLong failed");
                boolFlag = false;
            }
            return boolFlag;
        }

        public List<Sp_Total_RetailerInformation1_5_Result> RetailerInfos(int TID, int Fosid, int Range,int cityid, DateTime start, DateTime end)
        {
            List<Sp_Total_RetailerInformation1_5_Result> RetailerObj = new List<Sp_Total_RetailerInformation1_5_Result>();
            try
            {
                FOSDataModel dbContext = new FOSDataModel();


                RetailerObj = dbContext.Sp_Total_RetailerInformation1_5(TID, Fosid, cityid, start, end,Range).ToList();
            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<Sp_RetailerAvgSale_Result> AvgSale(int TID, int Range, int cityid, DateTime start, DateTime end)
        {
            List<Sp_RetailerAvgSale_Result> RetailerObj = new List<Sp_RetailerAvgSale_Result>();
            try
            {
                FOSDataModel dbContext = new FOSDataModel();


                RetailerObj = dbContext.Sp_RetailerAvgSale(TID, cityid, start, end, Range).ToList();
            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<Sp_StockPositionReport_Result> GetStockPosition(DateTime start, DateTime end, int TID,int Cityid, int RangeID)
        {
            List<Sp_StockPositionReport_Result> RetailerObj = new List<Sp_StockPositionReport_Result>();
            try
            {
                FOSDataModel dbContext = new FOSDataModel();


                RetailerObj = dbContext.Sp_StockPositionReport(start, end, TID, Cityid, RangeID).ToList();
            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<Sp_StockInvoiceReport_Result> GetStockInvoice(DateTime start, DateTime end, int TID, int Cityid, int RangeID)
        {
            List<Sp_StockInvoiceReport_Result> RetailerObj = new List<Sp_StockInvoiceReport_Result>();
            try
            {
                FOSDataModel dbContext = new FOSDataModel();


                RetailerObj = dbContext.Sp_StockInvoiceReport(start, end, TID, Cityid, RangeID).ToList();
            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<sp_BrandAndItemWiseReport_Result> BrandWisereports(DateTime start, DateTime end, int RHID,int Fosid, int TID, int cityid, int rangeId)
        {
            List<sp_BrandAndItemWiseReport_Result> RetailerObj = new List<sp_BrandAndItemWiseReport_Result>();
            try
            {
                FOSDataModel dbContext = new FOSDataModel();


                RetailerObj = dbContext.sp_BrandAndItemWiseReport(start, end,RHID,Fosid,TID,cityid,rangeId).ToList();
            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_DealerInformationSummery1_1_Result> DealerInfo(int TID, int Fosid, int cityid,int rangeid)
        {
            List<Sp_DealerInformationSummery1_1_Result> RetailerObj = new List<Sp_DealerInformationSummery1_1_Result>();
            try
            {
                FOSDataModel dbContext = new FOSDataModel();


                RetailerObj = dbContext.Sp_DealerInformationSummery1_1(TID, 0, 0, rangeid).ToList();
            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }



        public List<Sp__TotalShopVisitSummeryReport_Result> ShopVisitSummery(DateTime StartingDate, DateTime EndingDate, int TID, int fosid, int cityid, int dealerid)
        {
            List<Sp__TotalShopVisitSummeryReport_Result> RetailerObj = new List<Sp__TotalShopVisitSummeryReport_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp__TotalShopVisitSummeryReport(StartingDate, EndingDate, TID, fosid, dealerid, cityid).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_DealersOrderDetail_Result> ShopVisitSummeryOneLine(DateTime StartingDate, DateTime EndingDate, int TID, int fosid , int RangeID)
        {
            List<Sp_DealersOrderDetail_Result> RetailerObj = new List<Sp_DealersOrderDetail_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_DealersOrderDetail(StartingDate, EndingDate, TID, fosid,RangeID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }



        public List<Sp_RetailersOrderDetailFinal1_3_Result> RetailerVisitSummeryOneLine(DateTime StartingDate, DateTime EndingDate, int TID, int fosid, int cityId, int areaId, int retailerId)
        {
            List<Sp_RetailersOrderDetailFinal1_3_Result> RetailerObj = new List<Sp_RetailersOrderDetailFinal1_3_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_RetailersOrderDetailFinal1_3(StartingDate, EndingDate, TID, fosid,cityId).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<Sp_ManagersUpdateSummary_Result> ManagersSummary(DateTime StartingDate, DateTime EndingDate, int TID, int RangeID)
        {
            List<Sp_ManagersUpdateSummary_Result> RetailerObj = new List<Sp_ManagersUpdateSummary_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_ManagersUpdateSummary(StartingDate, EndingDate, TID, RangeID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_AttandaceReport_Result> Attandance(DateTime StartingDate, DateTime EndingDate, int TID, int fosid, int RangeID)
        {
            List<Sp_AttandaceReport_Result> RetailerObj = new List<Sp_AttandaceReport_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_AttandaceReport(StartingDate, EndingDate, TID, fosid, RangeID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_StockTakingDetailReport_Result> Stocktaking(DateTime StartingDate, DateTime EndingDate, int TID, int fosid)
        {
            List<Sp_StockTakingDetailReport_Result> RetailerObj = new List<Sp_StockTakingDetailReport_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_StockTakingDetailReport(StartingDate, EndingDate, TID, fosid).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }



        public List<Sp_FosPlanning_Result> FOSPlanningReport(DateTime StartingDate, DateTime EndingDate, int TID, int fosid)
        {
            List<Sp_FosPlanning_Result> RetailerObj = new List<Sp_FosPlanning_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_FosPlanning(StartingDate, EndingDate, TID, fosid).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp__TotalShopVisitDetail_Result> ShopVisitDetail(DateTime StartingDate, DateTime EndingDate, int TID, int fosid, int dealerid, int cityid)
        {
            List<Sp__TotalShopVisitDetail_Result> RetailerObj = new List<Sp__TotalShopVisitDetail_Result>();
            try
            {

                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp__TotalShopVisitDetail(StartingDate, EndingDate, TID, fosid, dealerid, cityid).ToList();

            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<sp_MarketInformation_Result> MarketInfo(DateTime StartingDate, DateTime EndingDate, int TID, int fosid, int dealerid)
        {
            List<sp_MarketInformation_Result> RetailerObj = new List<sp_MarketInformation_Result>();
            try
            {

                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.sp_MarketInformation(StartingDate, EndingDate, TID, fosid, dealerid).ToList();



            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp__ShopBrandWiseDisplayData_Result> ShopBrandWiseDisplayReport(DateTime StartingDate, DateTime EndingDate, int TID, int fosid, int cityid, int dealerid, int display)
        {
            List<Sp__ShopBrandWiseDisplayData_Result> RetailerObj = new List<Sp__ShopBrandWiseDisplayData_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp__ShopBrandWiseDisplayData(StartingDate, EndingDate, TID, fosid, dealerid, cityid,display).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<Sp_CityMarketRetailerInfo_Result> CityMarketRetailerInfo(DateTime StartingDate,  int cityid, int dealerid)
        {
            List<Sp_CityMarketRetailerInfo_Result> RetailerObj = new List<Sp_CityMarketRetailerInfo_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_CityMarketRetailerInfo(StartingDate, cityid, dealerid).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<Sp_Top10DistributorOrdersCityWiseGraphMonthly_Result> TopSales(DateTime StartingDate,DateTime ending)
        {
            List<Sp_Top10DistributorOrdersCityWiseGraphMonthly_Result> RetailerObj = new List<Sp_Top10DistributorOrdersCityWiseGraphMonthly_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_Top10DistributorOrdersCityWiseGraphMonthly(StartingDate,ending).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<Sp_Top10RetailersOrderCityWiseGraphMonthly_Result> TopSalesCityWise(DateTime StartingDate, DateTime ending)
        {
            List<Sp_Top10RetailersOrderCityWiseGraphMonthly_Result> RetailerObj = new List<Sp_Top10RetailersOrderCityWiseGraphMonthly_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_Top10RetailersOrderCityWiseGraphMonthly(StartingDate,ending).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_OrdersCurrentMonthQuantityWise_Result> TopZoneWise1()
        {
            List<Sp_OrdersCurrentMonthQuantityWise_Result> RetailerObj = new List<Sp_OrdersCurrentMonthQuantityWise_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = new DateTime(dtFromTodayUtc.Year, dtFromTodayUtc.Month, 1);
                DateTime dtToToday = dtFromToday.AddMonths(1).AddDays(-1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_OrdersCurrentMonthQuantityWise(dtFromToday, dtToToday).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }
        public List<Sp_OrdersCurrentMonthQuantityWise_Result> TopZoneWise2()
        {
            List<Sp_OrdersCurrentMonthQuantityWise_Result> RetailerObj = new List<Sp_OrdersCurrentMonthQuantityWise_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime fromdat = new DateTime(dtFromTodayUtc.Year, dtFromTodayUtc.Month, 1);
                DateTime dtFromToday = fromdat.AddMonths(-1);
                DateTime dtto = fromdat.AddDays(-1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_OrdersCurrentMonthQuantityWise(dtFromToday, dtto).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_OrdersCurrentMonthQuantityWiseManagersDashboard_Result> TopZoneWise12(int? ID,int? rangeID)
        {
            List<Sp_OrdersCurrentMonthQuantityWiseManagersDashboard_Result> RetailerObj = new List<Sp_OrdersCurrentMonthQuantityWiseManagersDashboard_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = new DateTime(dtFromTodayUtc.Year, dtFromTodayUtc.Month, 1);
                DateTime dtToToday = dtFromToday.AddMonths(1).AddDays(-1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_OrdersCurrentMonthQuantityWiseManagersDashboard(dtFromToday, dtToToday,ID, rangeID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }
        public List<Sp_OrdersCurrentMonthQuantityWiseManagersDashboard_Result> TopZoneWise23(int? ID, int? rangeID)
        {
            List<Sp_OrdersCurrentMonthQuantityWiseManagersDashboard_Result> RetailerObj = new List<Sp_OrdersCurrentMonthQuantityWiseManagersDashboard_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime fromdat = new DateTime(dtFromTodayUtc.Year, dtFromTodayUtc.Month, 1);
                DateTime dtFromToday = fromdat.AddMonths(-1);
                DateTime dtto = fromdat.AddDays(-1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_OrdersCurrentMonthQuantityWiseManagersDashboard(dtFromToday, dtto,ID, rangeID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_Top10OrderAmount_Result> ToAmountZoneWise1()
        {
            List<Sp_Top10OrderAmount_Result> RetailerObj = new List<Sp_Top10OrderAmount_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = new DateTime(dtFromTodayUtc.Year,dtFromTodayUtc.Month,1);
                DateTime dtToToday = dtFromToday.AddMonths(1).AddDays(-1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_Top10OrderAmount(dtFromToday,dtToToday).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }
        public List<Sp_Top10OrderAmount_Result> ToAmountZoneWise2()
        {
            List<Sp_Top10OrderAmount_Result> RetailerObj = new List<Sp_Top10OrderAmount_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime fromdat = new DateTime(dtFromTodayUtc.Year, dtFromTodayUtc.Month, 1);
                DateTime dtFromToday= fromdat.AddMonths(-1);
                DateTime dtto = fromdat.AddDays(-1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_Top10OrderAmount(dtFromToday, dtto).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_Top10OrderAmountManagersDashboard_Result> ToAmountZoneWise12(int? ID,int? rangeID)
        {
            List<Sp_Top10OrderAmountManagersDashboard_Result> RetailerObj = new List<Sp_Top10OrderAmountManagersDashboard_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = new DateTime(dtFromTodayUtc.Year, dtFromTodayUtc.Month, 1);
                DateTime dtToToday = dtFromToday.AddMonths(1).AddDays(-1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_Top10OrderAmountManagersDashboard(dtFromToday, dtToToday,ID, rangeID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }
        public List<Sp_Top10OrderAmountManagersDashboard_Result> ToAmountZoneWise23(int? ID,int? rangeID)
        {
            List<Sp_Top10OrderAmountManagersDashboard_Result> RetailerObj = new List<Sp_Top10OrderAmountManagersDashboard_Result>();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime fromdat = new DateTime(dtFromTodayUtc.Year, dtFromTodayUtc.Month, 1);
                DateTime dtFromToday = fromdat.AddMonths(-1);
                DateTime dtto = fromdat.AddDays(-1);


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_Top10OrderAmountManagersDashboard(dtFromToday, dtto,ID, rangeID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<Sp_TotalVisitsForAll_Result> Top30Items(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<Sp_TotalVisitsForAll_Result> RetailerObj = new List<Sp_TotalVisitsForAll_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_TotalVisitsForAll(StartingDate,end, RegionID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<usp_ApplicationCategorywisegraph_Result> AppcategoryWise(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<usp_ApplicationCategorywisegraph_Result> RetailerObj = new List<usp_ApplicationCategorywisegraph_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.usp_ApplicationCategorywisegraph(RegionID,StartingDate, end).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<usp_TopProductsByRegion_Result> Top3ProductWise(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<usp_TopProductsByRegion_Result> RetailerObj = new List<usp_TopProductsByRegion_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.usp_TopProductsByRegion(RegionID, StartingDate, end).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<Sp_AllRegionWiseData_Result> AllRegionsdata(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<Sp_AllRegionWiseData_Result> RetailerObj = new List<Sp_AllRegionWiseData_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_AllRegionWiseData(StartingDate, end, RegionID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<usp_IncentiveCategorywisegraph_Result> Incentivedata(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<usp_IncentiveCategorywisegraph_Result> RetailerObj = new List<usp_IncentiveCategorywisegraph_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.usp_IncentiveCategorywisegraph(RegionID,StartingDate, end).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<GetHousingVisitsByScopeOfWork_Result> AllScopedata(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<GetHousingVisitsByScopeOfWork_Result> RetailerObj = new List<GetHousingVisitsByScopeOfWork_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.GetHousingVisitsByScopeOfWork(StartingDate, end, RegionID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<usp_ProductTypeCategorywisegraph_Result> ProductTypeCategory(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<usp_ProductTypeCategorywisegraph_Result> RetailerObj = new List<usp_ProductTypeCategorywisegraph_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.usp_ProductTypeCategorywisegraph(RegionID,StartingDate, end).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<GetHousingVisitsByNatureOfWork_Result> AllNaturedata(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<GetHousingVisitsByNatureOfWork_Result> RetailerObj = new List<GetHousingVisitsByNatureOfWork_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.GetHousingVisitsByNatureOfWork(StartingDate, end, RegionID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<usp_RegionSaleswisegraph_Result> AllSalesdata(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<usp_RegionSaleswisegraph_Result> RetailerObj = new List<usp_RegionSaleswisegraph_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.usp_RegionSaleswisegraph(RegionID,StartingDate, end).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_GetHousingVisitsByConstructionStage_Result> AllConstructiondata(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<Sp_GetHousingVisitsByConstructionStage_Result> RetailerObj = new List<Sp_GetHousingVisitsByConstructionStage_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_GetHousingVisitsByConstructionStage(StartingDate, end, RegionID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        public List<usp_TopSalesOfficersGraph_Result> Top3SOSdata(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<usp_TopSalesOfficersGraph_Result> RetailerObj = new List<usp_TopSalesOfficersGraph_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.usp_TopSalesOfficersGraph(RegionID,StartingDate, end).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_Top30ItemsSoldMonthWise_Result> Top30Items1(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<Sp_Top30ItemsSoldMonthWise_Result> RetailerObj = new List<Sp_Top30ItemsSoldMonthWise_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_Top30ItemsSoldMonthWise(StartingDate, end, RegionID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_SegmentWiseOnlineOflline_Result> SegmentWiseOnlineOflline(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<Sp_SegmentWiseOnlineOflline_Result> RetailerObj = new List<Sp_SegmentWiseOnlineOflline_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_SegmentWiseOnlineOflline(StartingDate, end, RegionID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public List<Sp_SegmentWiseOnlineOflline_Result> SegmentWiseOnlineOflline1(DateTime StartingDate, DateTime end, int? RegionID)
        {
            List<Sp_SegmentWiseOnlineOflline_Result> RetailerObj = new List<Sp_SegmentWiseOnlineOflline_Result>();
            try
            {


                FOSDataModel dbContext = new FOSDataModel();

                RetailerObj = dbContext.Sp_SegmentWiseOnlineOflline(StartingDate, end, RegionID).ToList();


            }

            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }


        #region REPORTS WORK

        public static List<RetailerData> GetRetailersForExportinExcel()
        {
            List<RetailerData> RetailerObj = new List<RetailerData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RetailerObj = dbContext.Retailers.OrderBy(u => u.Name).Where(u => u.Status == true && u.IsDeleted == false)
                            .Select(
                                u => new RetailerData
                                {
                                    ID = u.ID,
                                   // DealerID = u.DealerID,
                                    DealerName = u.Dealer.Name,
                                    SaleOfficerID = u.SaleOfficerID,
                                    SaleOfficerName = u.SaleOfficer.Name,
                                    RetailerType = u.RetailerType,
                                    CityID = u.SaleOfficer.CityID,
                                    CItyName = u.City.Name,
                                    AreaID = u.Area == null ? 0 : u.AreaID.Value,
                                    AreaName = u.Area == null ? "" : u.Area.Name,
                                    Address = u.Address == null ? "" : u.Address,
                                    Name = u.Name,
                                    RetailerCode = u.RetailerCode,
                                    CNIC = u.CNIC,
                                    AccountNo = u.AccountNo,
                                    Email = u.Email,
                                    AccountNo2 = u.AccountNo2,
                                    BankName2 = u.BankName2,
                                    CardNumber = u.CardNumber,
                                    ShopName = u.ShopName,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    Location = u.Location,
                                    LocationName = u.LocationName,
                                    LocationMargin = u.LocationMargin,
                                    Type = u.Type,
                                    IsActive = (bool)u.IsActive,
                                    IsDeleted = (bool)u.IsDeleted,
                                    LastUpdate = (DateTime)u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "EXPORT Reatailer In Excel Not Work");
                throw;
            }

            return RetailerObj;
        }

        #region FOS wise Date wise Query

        public IEnumerable<RetailersReport> GetRetailersForExportinExcelReportC(DateTime Start, DateTime End, string TID, string FOSID, string[] data)
        {
            List<RetailersReport> RetailerObj = new List<RetailersReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk)
                {
                    data = new string[0];
                }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var cities = dbContext.Cities.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? cities.Contains(c.ID.ToString()) : data.Contains(c.ID.ToString()))
                            && (c.RegionID.ToString() == TID || TID == null) && (s.ID.ToString() == FOSID || FOSID == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SPreviousOrder1KG.Value == null ? 0 : j.SPreviousOrder1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate
                            }).Select(x => new RetailersReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                visiteddate = x.Key.visiteddate
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion FOS wise Date wise Query

        #region FOS wise Date wise Intake Query

        public IEnumerable<RetailersReport> FosWiseIntakeReport(DateTime Start, DateTime End, string TID, string FOSID, string[] data)
        {
            List<RetailersReport> RetailerObj = new List<RetailersReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk)
                {
                    data = new string[0];
                }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var cities = dbContext.Cities.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? cities.Contains(c.ID.ToString()) : data.Contains(c.ID.ToString()))
                            && (c.RegionID.ToString() == TID || TID == null) && (s.ID.ToString() == FOSID || FOSID == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SNewQuantity1KG.Value == null ? 0 : j.SNewQuantity1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate
                            }).Select(x => new RetailersReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                visiteddate = x.Key.visiteddate
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion FOS wise Date wise Intake Query

        #region FOS wise Month wise Query

        public IEnumerable<RetailersMonthlyReport> FOSGetRetailersForMonthWise(DateTime Start, DateTime End, string TID, string FOSID, string[] data)
        {
            List<RetailersMonthlyReport> RetailerObj = new List<RetailersMonthlyReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk) { data = new string[0]; }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var cities = dbContext.Cities.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories.OrderBy(x => x.VisitedDate)
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? cities.Contains(c.ID.ToString()) : data.Contains(c.ID.ToString()))
                            && (c.RegionID.ToString() == TID || TID == null) && (s.ID.ToString() == FOSID || FOSID == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SPreviousOrder1KG.Value == null ? 0 : j.SPreviousOrder1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate.Value.Month,
                                x.visiteddate.Value.Year
                            }).Select(x => new RetailersMonthlyReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                montht = x.Key.Month,
                                year = x.Key.Year,
                                months = x.Key.Month + "-" + x.Key.Year
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion FOS wise Month wise Query

        #region FOS wise Month wise Intake Query

        public IEnumerable<RetailersMonthlyReport> FOSMonthWiseIntake(DateTime Start, DateTime End, string TID, string FOSID, string[] data)
        {
            List<RetailersMonthlyReport> RetailerObj = new List<RetailersMonthlyReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk) { data = new string[0]; }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var cities = dbContext.Cities.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories.OrderBy(x => x.VisitedDate)
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? cities.Contains(c.ID.ToString()) : data.Contains(c.ID.ToString()))
                            && (c.RegionID.ToString() == TID || TID == null) && (s.ID.ToString() == FOSID || FOSID == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SNewQuantity1KG.Value == null ? 0 : j.SNewQuantity1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate.Value.Month,
                                x.visiteddate.Value.Year
                            }).Select(x => new RetailersMonthlyReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                montht = x.Key.Month,
                                year = x.Key.Year,
                                months = x.Key.Month + "-" + x.Key.Year
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion FOS wise Month wise Intake Query

        #region CITY wise Date wise Query

        public IEnumerable<RetailersReport> CityGetRetailersForExportinExcelReportC(DateTime Start, DateTime End, string TID, string Shoptypeid, string[] data)
        {
            List<RetailersReport> RetailerObj = new List<RetailersReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk)
                {
                    data = new string[0];
                }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var cities = dbContext.Cities.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? cities.Contains(c.ID.ToString()) : data.Contains(c.ID.ToString()))
                            && (c.RegionID.ToString() == TID || TID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SPreviousOrder1KG.Value == null ? 0 : j.SPreviousOrder1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate
                            }).Select(x => new RetailersReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                visiteddate = x.Key.visiteddate
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion CITY wise Date wise Query

        #region CITY wise Date wise Intake Query

        public IEnumerable<RetailersReport> CityDateWiseIntake(DateTime Start, DateTime End, string TID, string Shoptypeid, string[] data)
        {
            List<RetailersReport> RetailerObj = new List<RetailersReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk)
                {
                    data = new string[0];
                }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var cities = dbContext.Cities.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? cities.Contains(c.ID.ToString()) : data.Contains(c.ID.ToString()))
                            && (c.RegionID.ToString() == TID || TID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SNewQuantity1KG.Value == null ? 0 : j.SNewQuantity1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate
                            }).Select(x => new RetailersReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                visiteddate = x.Key.visiteddate
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion CITY wise Date wise Intake Query

        #region City wise Month wise Query

        public IEnumerable<RetailersMonthlyReport> CityGetRetailersForMonthWise(DateTime Start, DateTime End, string TID, string Shoptypeid, string[] data)
        {
            List<RetailersMonthlyReport> RetailerObj = new List<RetailersMonthlyReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk) { data = new string[0]; }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var cities = dbContext.Cities.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories.OrderBy(x => x.VisitedDate)
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? cities.Contains(c.ID.ToString()) : data.Contains(c.ID.ToString()))
                            && (c.RegionID.ToString() == TID || TID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SPreviousOrder1KG.Value == null ? 0 : j.SPreviousOrder1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate.Value.Month,
                                x.visiteddate.Value.Year
                            }).Select(x => new RetailersMonthlyReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                montht = x.Key.Month,
                                year = x.Key.Year,
                                months = x.Key.Month + "-" + x.Key.Year
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion City wise Month wise Query

        #region City wise Month wise Intake Query

        public IEnumerable<RetailersMonthlyReport> CityMonthWiseIntake(DateTime Start, DateTime End, string TID, string Shoptypeid, string[] data)
        {
            List<RetailersMonthlyReport> RetailerObj = new List<RetailersMonthlyReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk) { data = new string[0]; }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var cities = dbContext.Cities.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories.OrderBy(x => x.VisitedDate)
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? cities.Contains(c.ID.ToString()) : data.Contains(c.ID.ToString()))
                            && (c.RegionID.ToString() == TID || TID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SNewQuantity1KG.Value == null ? 0 : j.SNewQuantity1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate.Value.Month,
                                x.visiteddate.Value.Year
                            }).Select(x => new RetailersMonthlyReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                montht = x.Key.Month,
                                year = x.Key.Year,
                                months = x.Key.Month + "-" + x.Key.Year
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion City wise Month wise Intake Query

        #region SHOP wise Date wise Query

        public IEnumerable<RetailersReport> ShopGetRetailersForExportinExcelReportC(DateTime Start, DateTime End, string CID, string Shoptypeid, string[] data)
        {
            List<RetailersReport> RetailerObj = new List<RetailersReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk)
                {
                    data = new string[0];
                }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var shops = dbContext.Retailers.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? shops.Contains(r.ID.ToString()) : data.Contains(r.ID.ToString()))
                            && (c.ID.ToString() == CID || CID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SPreviousOrder1KG.Value == null ? 0 : j.SPreviousOrder1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate
                            }).Select(x => new RetailersReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                visiteddate = x.Key.visiteddate
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion SHOP wise Date wise Query

        #region SHOP wise Date wise Intake Query

        public IEnumerable<RetailersReport> RetailShopDateWiseIntake(DateTime Start, DateTime End, string CID, string Shoptypeid, string[] data)
        {
            List<RetailersReport> RetailerObj = new List<RetailersReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk)
                {
                    data = new string[0];
                }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var shops = dbContext.Retailers.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? shops.Contains(r.ID.ToString()) : data.Contains(r.ID.ToString()))
                            && (c.ID.ToString() == CID || CID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SNewQuantity1KG.Value == null ? 0 : j.SNewQuantity1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate
                            }).Select(x => new RetailersReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                visiteddate = x.Key.visiteddate
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion SHOP wise Date wise Intake Query

        #region Shop wise Month wise Query

        public IEnumerable<RetailersMonthlyReport> ShopGetRetailersForMonthWise(DateTime Start, DateTime End, string CID, string Shoptypeid, string[] data)
        {
            List<RetailersMonthlyReport> RetailerObj = new List<RetailersMonthlyReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk) { data = new string[0]; }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var shops = dbContext.Retailers.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories.OrderBy(x => x.VisitedDate)
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? shops.Contains(r.ID.ToString()) : data.Contains(r.ID.ToString()))
                            && (c.ID.ToString() == CID || CID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SPreviousOrder1KG.Value == null ? 0 : j.SPreviousOrder1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate.Value.Month,
                                x.visiteddate.Value.Year
                            }).Select(x => new RetailersMonthlyReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                montht = x.Key.Month,
                                year = x.Key.Year,
                                months = x.Key.Month + "-" + x.Key.Year
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion Shop wise Month wise Query

        #region Shop wise Month wise Intake Query

        public IEnumerable<RetailersMonthlyReport> RetailShopMonthWiseIntake(DateTime Start, DateTime End, string CID, string Shoptypeid, string[] data)
        {
            List<RetailersMonthlyReport> RetailerObj = new List<RetailersMonthlyReport>();
            try
            {
                bool chk = data == null ? true : false;
                if (chk) { data = new string[0]; }
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var shops = dbContext.Retailers.Select(x => x.ID.ToString()).ToList();

                    return (from j in dbContext.JobsHistories.OrderBy(x => x.VisitedDate)
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End && (chk ? shops.Contains(r.ID.ToString()) : data.Contains(r.ID.ToString()))
                            && (c.ID.ToString() == CID || CID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SNewQuantity1KG.Value == null ? 0 : j.SNewQuantity1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate.Value.Month,
                                x.visiteddate.Value.Year
                            }).Select(x => new RetailersMonthlyReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                montht = x.Key.Month,
                                year = x.Key.Year,
                                months = x.Key.Month + "-" + x.Key.Year
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion Shop wise Month wise Intake Query

        public IEnumerable<RetailersReport> GetRetailersForExportinExcelReport(DateTime Start, DateTime End)
        {
            List<RetailersReport> RetailerObj = new List<RetailersReport>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return (from j in dbContext.JobsHistories
                            join r in dbContext.Retailers on j.RetailerID equals r.ID
                            join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                            join c in dbContext.Cities on r.CityID equals c.ID
                            where j.VisitedDate >= Start && j.VisitedDate <= End
                            select new
                            {
                                RetailerID = r.ID,
                                retailername = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                saleofficername = s.Name == null ? "NA" : s.Name,
                                shopname = r.ShopName == null ? "NA" : r.ShopName,
                                cityname = c.Name == null ? "NA" : c.Name,
                                retailertype = r.RetailerType == null ? "NA" : r.RetailerType,
                                spreviousorder1kg = j.SPreviousOrder1KG.Value == null ? 0 : j.SPreviousOrder1KG,
                                visiteddate = j.VisitedDate == null ? DateTime.Now : j.VisitedDate
                            }).GroupBy(x => new
                            {
                                x.RetailerID,
                                x.retailername,
                                x.saleofficername,
                                x.shopname,
                                x.cityname,
                                x.retailertype,
                                x.visiteddate
                            }).Select(x => new RetailersReport
                            {
                                RetailerID = x.Key.RetailerID,
                                retailername = x.Key.retailername,
                                saleofficername = x.Key.saleofficername,
                                shopname = x.Key.shopname,
                                cityname = x.Key.cityname,
                                retailertype = x.Key.retailertype,
                                spreviousorder1kg = x.Sum(y => y.spreviousorder1kg),
                                visiteddate = x.Key.visiteddate
                            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #region City Wise FOS Report

        public IEnumerable<CityWiseFosReport> getCityWiseFOS(DateTime month, string TID, string[] dat)
        {
            List<CityWiseFosReport> RetailerObj = new List<CityWiseFosReport>();
            try
            {
                bool chk = dat == null ? true : false;
                if (chk) { dat = new string[0]; }
                DateTime d = month.AddMonths(-2);
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var cities = dbContext.Cities.Select(x => x.ID.ToString()).ToList();
                    var data = (from j in dbContext.JobsHistories
                                join r in dbContext.Retailers on j.RetailerID equals r.ID
                                join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                                join c in dbContext.Cities on r.CityID equals c.ID
                                where (chk ? cities.Contains(c.ID.ToString()) : dat.Contains(c.ID.ToString()))
                            && (c.RegionID.ToString() == TID || TID == null)
                                let cc = (from jj in dbContext.JobsHistories
                                          join rr in dbContext.Retailers on jj.RetailerID equals rr.ID
                                          join ct in dbContext.Cities on rr.CityID equals ct.ID
                                          where ct.ID == c.ID && jj.SaleOfficerID == j.SaleOfficerID
                                          && ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value.Month : 0) == month.Month) &&
                                               ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value.Year : 0) == month.Year)
                                               && (chk ? cities.Contains(ct.ID.ToString()) : dat.Contains(ct.ID.ToString()))
                                               && (ct.RegionID.ToString() == TID || TID == null)
                                          select jj)
                                             .Sum(x => x.SPreviousOrder1KG.HasValue ? x.SPreviousOrder1KG : 0)
                                let tillastSale = (from jj in dbContext.JobsHistories
                                                   join rr in dbContext.Retailers on jj.RetailerID equals rr.ID
                                                   join ct in dbContext.Cities on rr.CityID equals ct.ID
                                                   where ct.ID == c.ID && jj.SaleOfficerID == j.SaleOfficerID && jj.VisitedDate < month
                                                   && (chk ? cities.Contains(ct.ID.ToString()) : dat.Contains(ct.ID.ToString()))
                                                   && (ct.RegionID.ToString() == TID || TID == null)
                                                   select jj)
                                             .Sum(x => x.SPreviousOrder1KG.HasValue ? x.SPreviousOrder1KG : 0)
                                let avglast2mnth = (from jj in dbContext.JobsHistories
                                                    join rr in dbContext.Retailers on jj.RetailerID equals rr.ID
                                                    join ct in dbContext.Cities on rr.CityID equals ct.ID
                                                    where ct.ID == c.ID && jj.SaleOfficerID == j.SaleOfficerID
                                                    && ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value : DateTime.Now) < month) && ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value : DateTime.Now) >= d)
                                                    && (chk ? cities.Contains(ct.ID.ToString()) : dat.Contains(ct.ID.ToString()))
                                                    && (ct.RegionID.ToString() == TID || TID == null)
                                                    select jj)
                                             .Sum(x => x.SPreviousOrder1KG.HasValue ? x.SPreviousOrder1KG : 0) / 2
                                select new CityWiseFosReport
                                {
                                    saleofficername = s.Name,
                                    cityname = c.Name,
                                    crnt_mnth_sale = (cc.HasValue ? cc.Value : 0),
                                    till_last_mnth_sale = (tillastSale.HasValue ? tillastSale.Value : 0),
                                    avg_last_two_mnth_sale = (avglast2mnth.HasValue ? avglast2mnth.Value : 0),
                                }
                             ).Distinct().ToList();
                    return data;
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        #endregion City Wise FOS Report

        public IEnumerable<CityMktRtailrWiseReport> getCityMktRtlrWise(DateTime month, string CID, string Shoptypeid, string[] dat)
        {
            List<CityMktRtailrWiseReport> RetailerObj = new List<CityMktRtailrWiseReport>();
            try
            {
                DateTime d = month.AddMonths(-2);

                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    bool chk = dat == null ? true : false;
                    if (chk) { dat = new string[0]; }
                    var markets = dbContext.Areas.Select(x => x.ID.ToString()).ToList();
                    var data = (from j in dbContext.JobsHistories
                                join r in dbContext.Retailers on j.RetailerID equals r.ID
                                join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                                join c in dbContext.Cities on r.CityID equals c.ID
                                join a in dbContext.Areas on r.AreaID equals a.ID
                                where (chk ? markets.Contains(r.AreaID.ToString()) : dat.Contains(r.AreaID.ToString()))
                                && (c.ID.ToString() == CID || CID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                                let cc = (from jj in dbContext.JobsHistories
                                          join rr in dbContext.Retailers on jj.RetailerID equals rr.ID
                                          join ct in dbContext.Cities on rr.CityID equals ct.ID
                                          where ct.ID == c.ID && jj.RetailerID == j.RetailerID && jj.SaleOfficerID == j.SaleOfficerID
                                          && ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value.Month : 0) == month.Month) &&
                                               ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value.Year : 0) == month.Year) &&
                                               (chk ? markets.Contains(rr.AreaID.ToString()) : dat.Contains(rr.AreaID.ToString()))
                                               && (ct.ID.ToString() == CID || CID == null) && (rr.RetailerType == Shoptypeid || Shoptypeid == null)
                                          select jj)
                                             .Sum(x => x.SPreviousOrder1KG.HasValue ? x.SPreviousOrder1KG : 0)
                                let tillastSale = (from jj in dbContext.JobsHistories
                                                   join rr in dbContext.Retailers on jj.RetailerID equals rr.ID
                                                   join ct in dbContext.Cities on rr.CityID equals ct.ID
                                                   where ct.ID == c.ID && jj.RetailerID == j.RetailerID && jj.SaleOfficerID == j.SaleOfficerID && jj.VisitedDate < month &&
                                                   (chk ? markets.Contains(rr.AreaID.ToString()) : dat.Contains(rr.AreaID.ToString()))
                                                   && (ct.ID.ToString() == CID || CID == null) && (rr.RetailerType == Shoptypeid || Shoptypeid == null)
                                                   select jj)
                                             .Sum(x => x.SPreviousOrder1KG.HasValue ? x.SPreviousOrder1KG : 0)
                                let avglast2mnth = (from jj in dbContext.JobsHistories
                                                    join rr in dbContext.Retailers on jj.RetailerID equals rr.ID
                                                    join ct in dbContext.Cities on rr.CityID equals ct.ID
                                                    where ct.ID == c.ID && jj.RetailerID == j.RetailerID && jj.SaleOfficerID == j.SaleOfficerID
                                                    && ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value : DateTime.Now) < month) && ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value : DateTime.Now) >= d) &&
                                                    (chk ? markets.Contains(rr.AreaID.ToString()) : dat.Contains(rr.AreaID.ToString()))
                                                    && (ct.ID.ToString() == CID || CID == null) && (rr.RetailerType == Shoptypeid || Shoptypeid == null)
                                                    select jj)
                                             .Sum(x => x.SPreviousOrder1KG.HasValue ? x.SPreviousOrder1KG : 0) / 2
                                select new
                                {
                                    retailerid = j.RetailerID,
                                    rname = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                    shopname = r.ShopName,
                                    fos = s.Name,
                                    city = c.Name,
                                    rtype = r.RetailerType,
                                    mkt = a.Name,//r.Market,
                                    su = (cc.HasValue ? cc.Value : 0),
                                    tillastSal = (tillastSale.HasValue ? tillastSale.Value : 0),
                                    avglast2mnth = (avglast2mnth.HasValue ? avglast2mnth.Value : 0),
                                }
                             ).GroupBy(x => new
                             {
                                 x.retailerid,
                                 x.rname,
                                 x.shopname,
                                 x.fos,
                                 x.city,
                                 x.rtype,
                                 x.mkt,
                                 x.su,
                                 x.tillastSal,
                                 x.avglast2mnth
                             }).Select(y => new CityMktRtailrWiseReport
                             {
                                 cityname = y.Key.city,
                                 market = y.Key.mkt,
                                 retailer = y.Key.rname,
                                 category = y.Key.rtype,
                                 crnt_mnth_sale = y.Key.su,
                                 till_last_mnth_sale = y.Key.tillastSal,
                                 avg_last_two_mnth_sale = y.Key.avglast2mnth,
                             }).ToList();
                    return data;
                    /*var data = (from j in dbContext.JobsHistories
                                join r in dbContext.Retailers on j.RetailerID equals r.ID
                                join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                                join c in dbContext.Cities on r.CityID equals c.ID
                                where (chk ? markets.Contains(r.AreaID.ToString()) : dat.Contains(r.AreaID.ToString()))
                                && (c.ID.ToString() == CID || CID == null) && (r.RetailerType == Shoptypeid || Shoptypeid == null)
                                let cc = (from jj in dbContext.JobsHistories
                                          join rr in dbContext.Retailers on jj.RetailerID equals rr.ID
                                          join ct in dbContext.Cities on rr.CityID equals ct.ID
                                          where ct.ID == c.ID && jj.RetailerID == j.RetailerID && jj.SaleOfficerID == j.SaleOfficerID
                                          && ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value.Month : 0) == month.Month) &&
                                               ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value.Year : 0) == month.Year) &&
                                               (chk ? markets.Contains(rr.AreaID.ToString()) : dat.Contains(rr.AreaID.ToString()))
                                               && (ct.ID.ToString() == CID || CID == null) && (rr.RetailerType == Shoptypeid || Shoptypeid == null)
                                          select jj)
                                             .Sum(x => x.SPreviousOrder1KG.HasValue ? x.SPreviousOrder1KG : 0)
                                let tillastSale = (from jj in dbContext.JobsHistories
                                                   join rr in dbContext.Retailers on jj.RetailerID equals rr.ID
                                                   join ct in dbContext.Cities on rr.CityID equals ct.ID
                                                   where ct.ID == c.ID && jj.RetailerID == j.RetailerID && jj.SaleOfficerID == j.SaleOfficerID && jj.VisitedDate < month &&
                                                   (chk ? markets.Contains(rr.AreaID.ToString()) : dat.Contains(rr.AreaID.ToString()))
                                                   && (ct.ID.ToString() == CID || CID == null) && (rr.RetailerType == Shoptypeid || Shoptypeid == null)
                                                   select jj)
                                             .Sum(x => x.SPreviousOrder1KG.HasValue ? x.SPreviousOrder1KG : 0)
                                let avglast2mnth = (from jj in dbContext.JobsHistories
                                                    join rr in dbContext.Retailers on jj.RetailerID equals rr.ID
                                                    join ct in dbContext.Cities on rr.CityID equals ct.ID
                                                    where ct.ID == c.ID && jj.RetailerID == j.RetailerID && jj.SaleOfficerID == j.SaleOfficerID
                                                    && ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value : DateTime.Now) < month) && ((jj.VisitedDate.HasValue ? jj.VisitedDate.Value : DateTime.Now) >= d) &&
                                                    (chk ? markets.Contains(rr.AreaID.ToString()) : dat.Contains(rr.AreaID.ToString()))
                                                    && (ct.ID.ToString() == CID || CID == null) && (rr.RetailerType == Shoptypeid || Shoptypeid == null)
                                                    select jj)
                                             .Sum(x => x.SPreviousOrder1KG.HasValue ? x.SPreviousOrder1KG : 0) / 2
                                select new
                                {
                                    retailerid = j.RetailerID,
                                    rname = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                                    shopname = r.ShopName,
                                    fos = s.Name,
                                    city = c.Name,
                                    rtype = r.RetailerType,
                                    mkt = r.Market,
                                    su = (cc.HasValue ? cc.Value : 0),
                                    tillastSal = (tillastSale.HasValue ? tillastSale.Value : 0),
                                    avglast2mnth = (avglast2mnth.HasValue ? avglast2mnth.Value : 0),
                                }
                             ).GroupBy(x => new
                             {
                                 x.retailerid,
                                 x.rname,
                                 x.shopname,
                                 x.fos,
                                 x.city,
                                 x.rtype,
                                 x.mkt,
                                 x.su,
                                 x.tillastSal,
                                 x.avglast2mnth
                             }).Select(y => new CityMktRtailrWiseReport
                             {
                                 cityname = y.Key.city,
                                 market = y.Key.mkt,
                                 retailer = y.Key.rname,
                                 category = y.Key.rtype,
                                 crnt_mnth_sale = y.Key.su,
                                 till_last_mnth_sale = y.Key.tillastSal,
                                 avg_last_two_mnth_sale = y.Key.avglast2mnth,
                             }).ToList();
                    return data;*/
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return RetailerObj;
        }

        public IEnumerable<RetailerPoints> getRetailersPointSummary()
        {
            List<RetailerPoints> rptsumry = new List<RetailerPoints>();
            try
            {
                //FOSDataModel walcoat = new FOSDataModel();
                //MapleLeafDataModel mlp = new MapleLeafDataModel();

                //#region get data from MapleLeaf DB

                //var rtt = mlp.RetailerTransfers.Select(x => x).ToList();

                //#endregion get data from MapleLeaf DB

                //#region get data from WallCoat DB

                //var de = (from r in walcoat.Retailers
                //          join c in walcoat.Cities on r.CityID equals c.ID
                //          select new
                //          {
                //              rtid = r.ID,
                //              city = c.Name,
                //              rname = r.ShopName == null ? "NA" : r.ShopName,//r.Name == null ? "NA" : r.Name,//replace retailer name with shop name as per new requirement
                //              rtype = r.RetailerType,
                //              rcnic = r.CNIC
                //          }).ToList();

                //#endregion get data from WallCoat DB

                //#region Combine both DBs to get required Data

                //var rpointsummary = (from rt in rtt
                //                     join r in de on rt.RetailerId equals r.rtid
                //                     let tamoun = (from c in rtt.Where(x => x.RetailerId == r.rtid && x.Status != "NP")
                //                                   select c).Sum(y => y.NetAmount.HasValue ? y.NetAmount : 0)
                //                     select new
                //                     {
                //                         city = r.city,
                //                         rname = r.rname,
                //                         rtype = r.rtype,
                //                         rcnic = r.rcnic,
                //                         claimamount = rt.NetAmount.HasValue ? rt.NetAmount : 0,
                //                         tamount = tamoun.HasValue ? tamoun : 0,
                //                     }).GroupBy(x => new
                //                     {
                //                         x.city,
                //                         x.rname,
                //                         x.rtype,
                //                         x.rcnic,
                //                         x.tamount,
                //                     }).Select(x => new RetailerPoints
                //                     {
                //                         cityName = x.Key.city,
                //                         RetailerName = x.Key.rname,
                //                         RetailerType = x.Key.rtype,
                //                         CNIC = x.Key.rcnic,
                //                         Claim_Amount = x.Sum(y => y.claimamount),
                //                         Transferred_Amount = x.Key.tamount.HasValue ? x.Key.tamount : 0,
                //                         Balance_Amount = x.Sum(y => y.claimamount.HasValue ? y.claimamount : 0) - (x.Key.tamount.HasValue ? x.Key.tamount : 0),
                //                     }).OrderBy(x => x.cityName).ToList();
                //return rpointsummary;

                //#endregion Combine both DBs to get required Data
            }
            catch (Exception ex)
            {
            }
            return rptsumry;
        }

        public IEnumerable<PosAvailablility> PosAvailability(DateTime start, DateTime end)
        {
            List<PosAvailablility> PosObj = new List<PosAvailablility>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var data = (from j in dbContext.JobsHistories
                                join r in dbContext.Retailers on j.RetailerID equals r.ID
                                join s in dbContext.SaleOfficers on j.SaleOfficerID equals s.ID
                                join c in dbContext.Cities on r.CityID equals c.ID
                                select new PosAvailablility
                                {
                                    cityname = c.Name,
                                    market = r.Market,
                                    shopname = r.ShopName,
                                    category = r.RetailerType,
                                    fos = s.Name,
                                    pos_availability = (j.SPOSMaterialAvailable.Value == true ? 1 : 0),
                                }).ToList();
                    return data;
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }
            return PosObj;
        }

        public IEnumerable<CityWisePainters> getCityWisePainters(DateTime startdate)
        {
            List<CityWisePainters> cwpObj = new List<CityWisePainters>();
            try
            {
                //using (MapleLeafDataModel dbContext = new MapleLeafDataModel())
                //{
                //    var batch1kg = (from c in dbContext.Codes
                //                    join b in dbContext.Batches on c.BatchNo equals b.BatchNo
                //                    where b.BagSize == 1 && c.DateAdded<=startdate
                //                    select c.BatchNo.ToString()).Distinct().ToList();
                //    var batch5kg = (from c in dbContext.Codes
                //                    join b in dbContext.Batches on c.BatchNo equals b.BatchNo
                //                    where b.BagSize == 5 && c.DateAdded <= startdate
                //                    select c.BatchNo.ToString()).Distinct().ToList();

                //    var data = (from tr in dbContext.Transactions
                //                join p in dbContext.Painters on tr.PainterId equals p.PainterId into tmpmping
                //                where tr.TransactionType == "A" && tr.DateAdded <= startdate
                //                let points_redeemed_1kg = (from t in dbContext.Transactions
                //                                           where t.PainterId == tr.PainterId && t.TransactionType == "A" && t.DateAdded <= startdate && batch1kg.Contains(t.Details.Substring(7, 6).Trim())
                //                                           select t).Sum(x => x.Points)
                //                let points_redeemed_5kg = (from t in dbContext.Transactions
                //                                           where t.PainterId == tr.PainterId && t.TransactionType == "A" && t.DateAdded <= startdate && batch5kg.Contains(t.Details.Substring(7, 6).Trim())
                //                                           select t).Sum(x => x.Points)
                //                let points_transferred = (from t in dbContext.Transactions
                //                                          where t.PainterId == tr.PainterId && t.TransactionType == "T" && t.DateAdded <= startdate
                //                                          select t).Sum(x => x.Points)
                //                from p in tmpmping.DefaultIfEmpty()
                //                select new
                //                {
                //                    transactiontype = tr.TransactionType,
                //                    city = p.City == null ? "NA" : p.City,
                //                    magcard = p.MagneticCard == null ? "NA" : p.MagneticCard,
                //                    walletnumber = p.WalletNumber == null ? "NA" : p.WalletNumber,
                //                    cnic = p.CNIC == null ? "NA" : p.CNIC,
                //                    painterid = tr.PainterId,
                //                    pname = p.PainterName == null ? "NA" : p.PainterName,
                //                    prid = p.PainterId == null ? 0 : p.PainterId,
                //                    points_redeemed_1kg = points_redeemed_1kg == null ? 0 : points_redeemed_1kg,
                //                    points_redeemed_5kg = points_redeemed_5kg == null ? 0 : points_redeemed_5kg,
                //                    total_points_redeemed = tr.Points == null ? 0 : tr.Points,
                //                    points_transferred = points_transferred == null ? 0 : points_transferred,
                //                }).GroupBy(x => new
                //                {
                //                    x.transactiontype,
                //                    x.city,
                //                    x.magcard,
                //                    x.walletnumber,
                //                    x.cnic,
                //                    x.painterid,
                //                    x.pname,
                //                    x.points_redeemed_1kg,
                //                    x.points_redeemed_5kg,
                //                    x.points_transferred,
                //                }).Select(x => new CityWisePainters
                //                {
                //                    transactiontype = x.Key.transactiontype,
                //                    city = x.Key.city,
                //                    magcard = x.Key.magcard,
                //                    walletnumber = x.Key.walletnumber,
                //                    cnic = x.Key.cnic,
                //                    painterid = x.Key.painterid,
                //                    pname = x.Key.pname,
                //                    points_redeemed_1kg = x.Key.points_redeemed_1kg,
                //                    points_redeemed_5kg = x.Key.points_redeemed_5kg,
                //                    total_points_redeemed = x.Sum(y => y.total_points_redeemed),
                //                    points_transferred = x.Key.points_transferred,
                //                    balance_points = x.Sum(y => y.total_points_redeemed) - x.Key.points_transferred
                //                }).Distinct().ToList().Union((from tr in dbContext.Transactions
                //                                              join p in dbContext.Painters on tr.PainterId equals p.PainterId
                //                                              where tr.TransactionType == "T" && tr.DateAdded <= startdate && !dbContext.Transactions.Any(x => x.PainterId == tr.PainterId && x.TransactionType == "A")
                //                                              select new
                //                                              {
                //                                                  transactiontype = tr.TransactionType,
                //                                                  city = p.City == null ? "NA" : p.City,
                //                                                  magcard = p.MagneticCard == null ? "NA" : p.MagneticCard,
                //                                                  walletnumber = p.WalletNumber == null ? "NA" : p.WalletNumber,
                //                                                  cnic = p.CNIC == null ? "NA" : p.CNIC,
                //                                                  painterid = tr.PainterId,
                //                                                  pname = p.PainterName == null ? "NA" : p.PainterName,
                //                                                  prid = p.PainterId == null ? 0 : p.PainterId,
                //                                                  points_redeemed_1kg = 0,
                //                                                  points_redeemed_5kg = 0,
                //                                                  total_points_redeemed = 0,
                //                                                  points_transferred = tr.Points == null ? 0 : tr.Points,
                //                                              }).GroupBy(x => new
                //                                              {
                //                                                  x.transactiontype,
                //                                                  x.city,
                //                                                  x.magcard,
                //                                                  x.walletnumber,
                //                                                  x.cnic,
                //                                                  x.painterid,
                //                                                  x.pname,
                //                                                  x.points_redeemed_1kg,
                //                                                  x.points_redeemed_5kg,
                //                                                  x.total_points_redeemed,
                //                                              }).Select(x => new CityWisePainters
                //                                              {
                //                                                  transactiontype = x.Key.transactiontype,
                //                                                  city = x.Key.city,
                //                                                  magcard = x.Key.magcard,
                //                                                  walletnumber = x.Key.walletnumber,
                //                                                  cnic = x.Key.cnic,
                //                                                  painterid = x.Key.painterid,
                //                                                  pname = x.Key.pname,
                //                                                  points_redeemed_1kg = x.Key.points_redeemed_1kg,
                //                                                  points_redeemed_5kg = x.Key.points_redeemed_5kg,
                //                                                  total_points_redeemed = x.Key.total_points_redeemed,
                //                                                  points_transferred = x.Sum(y => y.points_transferred),
                //                                                  balance_points = x.Key.total_points_redeemed - x.Sum(y => y.points_transferred)
                //                                              }).Distinct().ToList()).Union((from tr in dbContext.Transactions
                //                                                                             join p in dbContext.Painters on tr.PainterId equals p.PainterId
                //                                                                             where tr.TransactionType == "M" && tr.DateAdded <= startdate
                //                                                                             select new
                //                                                                             {
                //                                                                                 transactiontype = tr.TransactionType,
                //                                                                                 city = p.City == null ? "NA" : p.City,
                //                                                                                 magcard = p.MagneticCard == null ? "NA" : p.MagneticCard,
                //                                                                                 walletnumber = p.WalletNumber == null ? "NA" : p.WalletNumber,
                //                                                                                 cnic = p.CNIC == null ? "NA" : p.CNIC,
                //                                                                                 painterid = tr.PainterId,
                //                                                                                 pname = p.PainterName == null ? "NA" : p.PainterName,
                //                                                                                 prid = p.PainterId == null ? 0 : p.PainterId,
                //                                                                                 points_redeemed_1kg = 0,
                //                                                                                 points_redeemed_5kg = 0,
                //                                                                                 total_points_redeemed = 0,
                //                                                                                 points_transferred = tr.Points == null ? 0 : tr.Points,
                //                                                                             }).GroupBy(x => new
                //                                                                             {
                //                                                                                 x.transactiontype,
                //                                                                                 x.city,
                //                                                                                 x.magcard,
                //                                                                                 x.walletnumber,
                //                                                                                 x.cnic,
                //                                                                                 x.painterid,
                //                                                                                 x.pname,
                //                                                                                 x.points_redeemed_1kg,
                //                                                                                 x.points_redeemed_5kg,
                //                                                                                 x.total_points_redeemed,
                //                                                                             }).Select(x => new CityWisePainters
                //                                                                             {
                //                                                                                 transactiontype = x.Key.transactiontype,
                //                                                                                 city = x.Key.city,
                //                                                                                 magcard = x.Key.magcard,
                //                                                                                 walletnumber = x.Key.walletnumber,
                //                                                                                 cnic = x.Key.cnic,
                //                                                                                 painterid = x.Key.painterid,
                //                                                                                 pname = x.Key.pname,
                //                                                                                 points_redeemed_1kg = x.Key.points_redeemed_1kg,
                //                                                                                 points_redeemed_5kg = x.Key.points_redeemed_5kg,
                //                                                                                 total_points_redeemed = x.Key.total_points_redeemed,
                //                                                                                 points_transferred = x.Sum(y => y.points_transferred),
                //                                                                                 balance_points = x.Key.total_points_redeemed - x.Sum(y => y.points_transferred)
                //                                                                             }).Distinct().ToList());

                //    return data;
                //}
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Data not Found");
                throw;
            }

            return cwpObj;
        }

        #endregion REPORTS WORK

        #region banks

        public static List<BankData> GetBanks()
        {
            var banks = new List<BankData>();
            banks.Add(new BankData
            {
                ID = 0,
                Name = "Select"
            });
          

            return banks;
        }

        #endregion
    }

    public enum RetSourceEnum
    {
        Script = 1,
        Mobile = 2,
        Web = 3
    }
    public enum RetActionEnum
    {
        Add = 10,
        AddApproved = 100,
        Update = 20,
        UpdateApproved = 200,
        ResetLoc = 30,
        ResetLocApproved = 300,
        UpdateLoc = 40,
        UpdateLocApproved = 400,
        Deleted = 50
    }
}