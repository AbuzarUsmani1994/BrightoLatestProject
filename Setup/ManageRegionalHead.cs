using FOS.DataLayer;
using FOS.Setup.Validation;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class ManageRegionalHead
    {


        // Get All Regional Head ...
        public static List<RegionalHeadData> GetRegionalHeadList()
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.RegionalHeads.Where(u => u.IsDeleted == false)
                            .ToList().Select(
                                u => new RegionalHeadData
                                {
                                    ID = u.ID,
                                    Type = (int)u.Type,
                                    TypeName = u.RegionalHeadsType.Type,
                                    Name = u.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionalHeadData;
        }

        public static List<CityData> GetCities()
        {
            List<CityData> regionalHeadData = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.Cities.Where(u => u.IsActive == true && u.IsDeleted == false)
                            .ToList().Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionalHeadData;
        }





        public static List<RegionalHeadData> GetTerritorialRegionalHeadList(int UserID)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            RegionalHeadData comlist;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (UserID == 1 || UserID== 1120|| UserID==1121 || UserID == 1122 || UserID==1123)
                    {
                        regionalHeadData = dbContext.RegionalHeads.Where(u => u.IsDeleted == false)
                                .ToList().Select(
                                    u => new RegionalHeadData
                                    {
                                        ID = u.ID,
                                        Type = (int)u.Type,
                                        TypeName = u.RegionalHeadsType.Type,
                                        Name = u.Name,
                                        Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                        Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                        LastUpdate = u.LastUpdate
                                    }).ToList();

                        regionalHeadData.Insert(0, new RegionalHeadData
                        {
                            ID = 0,
                            Name = "All"
                        });
                    }
                   else if (UserID == 1119)
                    {
                        regionalHeadData = dbContext.RegionalHeads.Where(u => u.IsDeleted == false)
                                .ToList().Select(
                                    u => new RegionalHeadData
                                    {
                                        ID = u.ID,
                                        Type = (int)u.Type,
                                        TypeName = u.RegionalHeadsType.Type,
                                        Name = u.Name,
                                        Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                        Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                        LastUpdate = u.LastUpdate
                                    }).ToList();

                        regionalHeadData.Insert(0, new RegionalHeadData
                        {
                            ID = 0,
                            Name = "All"
                        });
                    }

                    else
                    {
                        var regionalheadid = dbContext.Users.Where(x => x.ID == UserID).Select(x => x.RegionalheadRef).FirstOrDefault();
                        regionalHeadData = dbContext.RegionalHeads.Where(u => u.IsDeleted == false && u.ID==regionalheadid)
                                    .ToList().Select(
                                        u => new RegionalHeadData
                                        {
                                            ID = u.ID,
                                            Type = (int)u.Type,
                                            TypeName = u.RegionalHeadsType.Type,
                                            Name = u.Name,
                                            Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                            Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                            LastUpdate = u.LastUpdate
                                        }).ToList();

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionalHeadData;
        }

        public static List<RegionalHeadData> GetZonesListForDashboard()
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            RegionalHeadData comlist;


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {





                    var Regionids = dbContext.Regions.ToList();

                    foreach (var item in Regionids)
                    {

                        comlist = new RegionalHeadData();
                        comlist.ID = item.ID;
                        comlist.Name = dbContext.Regions.Where(x => x.ID == item.ID).Select(x => x.Name).FirstOrDefault();
                        regionalHeadData.Add(comlist);
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionalHeadData;
        }


        public static List<RegionalHeadData> GetZoneList(int userID)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            RegionalHeadData comlist;


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                


                 
                     
                        var Regionids = dbContext.RegionalHeadRegions.Where(x => x.RegionHeadID == userID).Select(x => x.RegionID).ToList();

                        foreach (var item in Regionids)
                        {

                            comlist = new RegionalHeadData();
                            comlist.ID = item;
                            comlist.Name = dbContext.Regions.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                            regionalHeadData.Add(comlist);
                        }
                 
                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionalHeadData;
        }

        public static List<RegionalHeadData> GetTerritorialRegionaList( int userID)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            RegionalHeadData comlist;


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (userID == 1)
                    {
                        regionalHeadData = dbContext.Regions.Where(u => u.IsDeleted == false)
                                .ToList().Select(
                                    u => new RegionalHeadData
                                    {
                                        ID = u.ID,

                                        Name = u.Name,

                                        LastUpdate = u.LastUpdate
                                    }).ToList();

                        regionalHeadData.Insert(0, new RegionalHeadData
                        {
                            ID = 0,
                            Name = "All"
                        });
                    }

                 
                    else if(userID == 1104 || userID == 1105 || userID== 1106 || userID== 1107 || userID== 1095 || userID==1035 || userID == 1041 || userID == 1040)
                    {
                        var regionalheadids = dbContext.RSMPortalUsers.Where(x => x.UserID == userID).Select(x => x.RegionID).Distinct().ToList();

                        foreach (var items in regionalheadids)
                        {

                            comlist = new RegionalHeadData();
                            comlist.ID = (int) items;
                            comlist.Name = dbContext.Regions.Where(x => x.ID == items).Select(x => x.Name).FirstOrDefault();
                            regionalHeadData.Add(comlist);



                        }

                      

                    }


                    else
                    {
                        var regionalheadid = dbContext.Users.Where(x => x.ID == userID).Select(x => x.RegionalheadRef).FirstOrDefault();
                        var Regionids = dbContext.RegionalHeadRegions.Where(x => x.RegionHeadID == regionalheadid).Select(x => x.RegionID).ToList();

                        // RegionalHeadRegions isn't reliably maintained for every head
                        // and can come back empty (seen on Call Summary Detailed Report's
                        // Region dropdown) - fall back to the regions this head's own
                        // active Sale Officers actually belong to, the same live data
                        // every "my Sale Officers" list already depends on (e.g.
                        // ManageRegion.GetAllSOListRelatedtoregionalHeadID).
                        if (Regionids == null || Regionids.Count == 0)
                        {
                            Regionids = dbContext.SaleOfficers
                                .Where(s => s.RegionalHeadID == regionalheadid && s.IsActive == true && s.RegionID.HasValue)
                                .Select(s => s.RegionID.Value)
                                .Distinct()
                                .ToList();
                        }

                        foreach (var item in Regionids)
                        {

                            comlist = new RegionalHeadData();
                            comlist.ID = item;
                            comlist.Name = dbContext.Regions.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                            regionalHeadData.Add(comlist);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionalHeadData;
        }

        public static List<RegionalHeadData> GetDealersListForUsers(int RangeId, int RegionID)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
           


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
               
                        regionalHeadData = dbContext.Dealers.Where(u => u.IsActive == true && u.RangeID==RangeId && u.RegionID==RegionID)
                                .ToList().Select(
                                    u => new RegionalHeadData
                                    {
                                        ID = u.ID,

                                        Name = u.ShopName,
                                        LastUpdate = u.LastUpdate
                                    }).ToList();
                

                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionalHeadData;
        }

        public static List<RegionalHeadData> GetB2BRegionalHeadList()
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.RegionalHeads.Where(u => u.Type == 2 && u.IsDeleted == false)
                            .ToList().Select(
                                u => new RegionalHeadData
                                {
                                    ID = u.ID,
                                    Type = (int)u.Type,
                                    TypeName = u.RegionalHeadsType.Type,
                                    Name = u.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionalHeadData;
        }


        // Insert Update Regional Head ...
        public static Boolean AddUpdateRegionalHead(RegionalHeadData obj)
        {
            Boolean boolFlag = false;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        RegionalHead regionalHeadObj = new RegionalHead();

                        if (obj.ID == 0)
                        {
                            regionalHeadObj.ID = dbContext.RegionalHeads.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            regionalHeadObj.Type = obj.Type;
                            regionalHeadObj.Name = obj.Name;
                            regionalHeadObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                            regionalHeadObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                            regionalHeadObj.IsActive = true;
                            regionalHeadObj.CreatedDate = DateTime.Now;
                            regionalHeadObj.LastUpdate = DateTime.Now;
                            regionalHeadObj.RangeID = obj.RangeID;
                            regionalHeadObj.CreatedBy = 1;

                            dbContext.RegionalHeads.Add(regionalHeadObj);

                            RegionalHeadRegion regionalHeadRegionObj;

                            String[] strRegionId = obj.RegionID.Split(',');

                            foreach (var regionid in strRegionId)
                            {
                                regionalHeadRegionObj = new RegionalHeadRegion();
                                regionalHeadRegionObj.RegionHeadID = regionalHeadObj.ID;
                                regionalHeadRegionObj.RegionID = Convert.ToInt32(regionid);
                                regionalHeadRegionObj.IsActive = true;
                                regionalHeadRegionObj.CreatedDate = DateTime.Now;
                                regionalHeadRegionObj.LastUpdate = DateTime.Now;
                                regionalHeadRegionObj.RHType = (int)obj.Type;

                                //Created By Work Pending...
                                regionalHeadRegionObj.CreatedBy = 1;
                                dbContext.RegionalHeadRegions.Add(regionalHeadRegionObj);
                            }

                            dbContext.SaveChanges();
                        }
                        else
                        {
                            regionalHeadObj = dbContext.RegionalHeads.Where(u => u.ID == obj.ID).FirstOrDefault();
                            regionalHeadObj.Type = obj.Type;
                            regionalHeadObj.Name = obj.Name;
                            regionalHeadObj.Phone1 = obj.Phone1 == "" ? null : obj.Phone1;
                            regionalHeadObj.Phone2 = obj.Phone2 == "" ? null : obj.Phone2;
                            regionalHeadObj.LastUpdate = DateTime.Now;
                            regionalHeadObj.IsActive = obj.IsActive;
                            regionalHeadObj.RangeID = obj.RangeID;
                            var objHeadRegion = dbContext.RegionalHeadRegions.Where(c => c.RegionHeadID == obj.ID);

                            foreach (var HeadRegion in objHeadRegion)
                            {
                                dbContext.RegionalHeadRegions.Remove(HeadRegion);
                            }

                            RegionalHeadRegion regionalHeadRegionObj;

                            String[] strRegionId = obj.RegionID.Split(',');

                            foreach (var regionid in strRegionId)
                            {
                                regionalHeadRegionObj = new RegionalHeadRegion();
                                regionalHeadRegionObj.RegionHeadID = regionalHeadObj.ID;
                                regionalHeadRegionObj.RegionID = Convert.ToInt32(regionid);
                                regionalHeadRegionObj.IsActive = true;
                                regionalHeadRegionObj.CreatedDate = DateTime.Now;
                                regionalHeadRegionObj.LastUpdate = DateTime.Now;
                                regionalHeadRegionObj.RHType = (int)obj.Type;

                                //Created By Work Pending...
                                regionalHeadRegionObj.CreatedBy = 1;
                                dbContext.RegionalHeadRegions.Add(regionalHeadRegionObj);
                            }

                            dbContext.SaveChanges();
                        }

                        boolFlag = true;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add RegionalHead Failed");
                boolFlag = false;
            }
            return boolFlag;
        }


        // Get All Regions Name Related To Regional Head Wit line Break ...
        public static string GetRegionalHeadRegions(int intRegionalHeadID)
        {
            String strRegionName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            strRegionName = string.Join("<br/>", dbContext.RegionalHeadRegions.Where(p => p.RegionHeadID == intRegionalHeadID)
                                 .Select(p => p.Region.Name));

            return strRegionName;
        }

        public static string GetItemsRelatedToGroup(int intRegionalHeadID)
        {
            String strRegionName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            strRegionName = string.Join("<br/>", dbContext.ItemGroupDetails.Where(p => p.GroupID == intRegionalHeadID)
                                 .Select(p => p.Item.ItemName));

            return strRegionName;
        }


        // Get All Regions ID Related To RegionalHeadID With Comma Seperator ...
        public static string GetRegionalHeadRegionsID(int intRegionalHeadID)
        {
            String strRegionName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            strRegionName = string.Join(",", dbContext.RegionalHeadRegions.Where(p => p.RegionHeadID == intRegionalHeadID)
                                 .Select(p => p.Region.ID));

            return strRegionName;
        }


        // Delete Regional Head ...
        public static int DeleteRegionalHead(int intRegionalHeadID)
        {
            int Resp = 0;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        var objHeadRegion = dbContext.RegionalHeadRegions.Where(c => c.RegionHeadID == intRegionalHeadID);

                        foreach (var HeadRegion in objHeadRegion)
                        {
                            dbContext.RegionalHeadRegions.Remove(HeadRegion);
                        }

                        RegionalHead obj = dbContext.RegionalHeads.Where(u => u.ID == intRegionalHeadID).FirstOrDefault();
                        dbContext.RegionalHeads.Remove(obj);
                        //obj.IsDeleted = true;
                        dbContext.SaveChanges();
                    }
                    scope.Complete();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete RegionalHead Failed");
                Resp = 1;
            }
            return Resp;
        }


        // Get All Regional Head For Grid ...
        public static List<RegionalHeadData> GetRegionalForGrid(int RegionalHeadType)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                   
                        regionalHeadData = dbContext.RegionalHeads.Where(u => u.Type == RegionalHeadType && u.IsDeleted == false)
                                .ToList().Select(
                                    u => new RegionalHeadData
                                    {
                                        ID = u.ID,
                                        Type = (int)u.Type,
                                        TypeName = u.RegionalHeadsType.Type,
                                        Name = u.Name,
                                        Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                        Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                        RegionName = GetRegionalHeadRegions(u.ID),
                                        RegionID = GetRegionalHeadRegionsID(u.ID),
                                        LastUpdate = u.LastUpdate,
                                        IsActiveYes = u.IsActive == true ? "Yes" : "No",
                                        RangeName =dbContext.MainCategories.Where(x=>x.MainCategID==u.RangeID).Select(x=>x.MainCategDesc).FirstOrDefault()
                                    }).ToList();
                    
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get RegionalHead List Failed");
                throw;
            }

            return regionalHeadData;
        }


        public static List<RegionalHeadData> GetResult(string search, string sortOrder, int start, int length, List<RegionalHeadData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<RegionalHeadData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<RegionalHeadData> FilterResult(string search, List<RegionalHeadData> dtResult, List<string> columnFilters)
        {
            IQueryable<RegionalHeadData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionName != null && p.RegionName.ToLower().Contains(search.ToLower()) || p.Phone1 != null && p.Phone1.ToLower().Contains(search.ToLower())
                || p.Phone2 != null && p.Phone2.ToLower().Contains(search.ToLower())))
                && (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.Phone1 != null && p.Phone1.ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.Phone2 != null && p.Phone2.ToLower().Contains(columnFilters[6].ToLower())))
                );

            return results;
        }



        public static List<RegionalHeadData> GetTerritorialHeadRegionalHeadList()
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.RegionalHeads.Where(u => u.Type == 1 && u.IsDeleted == false)
                            .ToList().Select(
                                u => new RegionalHeadData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            regionalHeadData.Insert(0, new RegionalHeadData
            {
                ID = 0,
                Name = "Select"
            });

            return regionalHeadData;
        }

        public static Boolean AddUpdateItemGroup(RegionalHeadData obj)
        {
            Boolean boolFlag = false;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        ItemGroupMaster regionalHeadObj = new ItemGroupMaster();
                       
                        if (obj.ID == 0)
                        {
                            regionalHeadObj.ID = dbContext.ItemGroupMasters.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                           
                            regionalHeadObj.Name = obj.Name;
                            regionalHeadObj.RangeID = obj.Type;
                            regionalHeadObj.isactive = true;
                            regionalHeadObj.Createdat = DateTime.UtcNow.AddHours(5);
                            dbContext.ItemGroupMasters.Add(regionalHeadObj);
                       
                            ItemGroupDetail regionalHeadRegionObj;

                            String[] strRegionId = obj.RegionID.Split(',');

                            foreach (var regionid in strRegionId)
                            {
                                regionalHeadRegionObj = new ItemGroupDetail();
                                regionalHeadRegionObj.GroupID = regionalHeadObj.ID;
                                regionalHeadRegionObj.ItemID = Convert.ToInt32(regionid);
                                regionalHeadRegionObj.CreatedAt = DateTime.UtcNow.AddHours(5);



                                dbContext.ItemGroupDetails.Add(regionalHeadRegionObj);
                            }

                            dbContext.SaveChanges();
                        }
                   

                        boolFlag = true;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add RegionalHead Failed");
                boolFlag = false;
            }
            return boolFlag;
        }

        public static List<RegionalHeadData> GetItemGroupForGrid(int RegionalHeadType)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    regionalHeadData = dbContext.ItemGroupMasters.Where(u => u.RangeID == RegionalHeadType && u.isactive == true)
                            .ToList().Select(
                                u => new RegionalHeadData
                                {
                                    ID = u.ID,
                                  TypeName=dbContext.MainCategories.Where(x=>x.MainCategID== RegionalHeadType).Select(x=>x.MainCategDesc).FirstOrDefault(),
                                    Name = u.Name,
                                  
                                    RegionName = GetItemsRelatedToGroup(u.ID),
                                    
                                    
                                }).ToList();

                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get RegionalHead List Failed");
                throw;
            }

            return regionalHeadData;
        }

        public static int DeleteItemgroup(int intRegionalHeadID)
        {
            int Resp = 0;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        var objHeadRegion = dbContext.ItemGroupDetails.Where(c => c.GroupID == intRegionalHeadID);

                        foreach (var HeadRegion in objHeadRegion)
                        {
                            dbContext.ItemGroupDetails.Remove(HeadRegion);
                        }

                        ItemGroupMaster obj = dbContext.ItemGroupMasters.Where(u => u.ID == intRegionalHeadID).FirstOrDefault();
                        dbContext.ItemGroupMasters.Remove(obj);
                        //obj.IsDeleted = true;
                        dbContext.SaveChanges();
                    }
                    scope.Complete();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete RegionalHead Failed");
                Resp = 1;
            }
            return Resp;
        }
    }
}