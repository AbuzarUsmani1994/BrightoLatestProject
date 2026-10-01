using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class ManageRegion
    {

        public static List<RegionalHeadTypeData> GetSORegionsToAdd()
        {
            List<RegionalHeadTypeData> TypeData = new List<RegionalHeadTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TypeData = dbContext.Regions.Where(u => u.IsActive == true)
                            .Select(
                                u => new RegionalHeadTypeData
                                {
                                    ID = u.ID,
                                    Type = u.Name,
                                }).ToList();

                    TypeData.Insert(0, new RegionalHeadTypeData
                    {
                        ID = 0,
                        Type = "All"
                    });

                }
            }
            catch (Exception)
            {
                throw;
            }

            return TypeData;
        }
        public static List<RegionData> GetRegionDataList(int userID)
        {
            List<RegionData> regionData = new List<RegionData>();
            RegionData comlist;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (userID == 1)
                    {
                        regionData = dbContext.Regions.Where(u => u.IsDeleted == false)
                                .ToList().Select(
                                    u => new RegionData
                                    {
                                        ID = u.ID,

                                        Name = u.Name,

                                        LastUpdate = u.LastUpdate
                                    }).ToList();
                    }
                    else if (userID == 1028 || userID == 1023)
                    {
                        var regionalheadids = dbContext.RSMPortalUsers.Where(x => x.UserID == userID).Select(x => x.RegionID).Distinct().ToList();

                        foreach (var items in regionalheadids)
                        {

                            comlist = new RegionData();
                            comlist.ID = (int)items;
                            comlist.Name = dbContext.Regions.Where(x => x.ID == items).Select(x => x.Name).FirstOrDefault();
                            regionData.Add(comlist);

                        }

                    }
                    else
                    {
                        var regionalheadid = dbContext.Users.Where(x => x.ID == userID).Select(x => x.RegionalheadRef).FirstOrDefault();
                        var Regionids = dbContext.RegionalHeadRegions.Where(x => x.RegionHeadID == regionalheadid).Select(x => x.RegionID).ToList();

                        foreach (var item in Regionids)
                        {

                            comlist = new RegionData();
                            comlist.ID = item;
                            comlist.Name = dbContext.Regions.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                            regionData.Add(comlist);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return regionData;
        }

        public static List<CityData> GetCityListForDSR( int ID)
        {
            List<CityData> regionData = new List<CityData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionData = dbContext.Cities.Where( u => u.IsActive == true && u.IsDeleted == false &&u.RegionID==ID )
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

            return regionData;
        }


        public static List<RegionData> GetHeadListForDSR()
        {
            List<RegionData> regionData = new List<RegionData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionData = dbContext.RegionalHeads.Where(u => u.IsActive == true && u.IsDeleted == false)
                            .ToList().Select(
                                u => new RegionData
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

            return regionData;
        }

        // Get All Region ...
        public static List<Region> GetRegionList(int RHID)
        {
            List<Region> region = new List<Region>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (RHID == 0)
                {
                    region = dbContext.Regions.OrderBy(x=>x.Name).ToList();
                  
                }
                else
                {
                    var Regions = dbContext.RegionalHeadRegions.Where(rhr => rhr.RegionHeadID == RHID).Select(rhr => rhr.RegionID).ToList();
                    region = dbContext.Regions.Where(r => Regions.Contains(r.ID)).ToList();
                }

                
            }
            return region;
        }

        public static List<CityData> GetCitiesList(int RHID)
        {
            List<CityData> region = new List<CityData>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

              
               
                    //var Regions = dbContext.RegionalHeadRegions.Where(rhr => rhr.RegionHeadID == RHID).Select(rhr => rhr.RegionID).ToList();
                    region = dbContext.Cities.Where(r => r.RegionID==RHID).Select
                            (
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).OrderBy(x => x.Name).ToList(); ;

                region.Insert(0, new CityData
                {
                    ID = 0,
                    Name = "All"
                });

            }
            return region;
        }

        public static List<MainCategory> GetMainCategory()
        {
            List<MainCategory> region = new List<MainCategory>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

             
               
                   region = dbContext.MainCategories.Where(rhr => rhr.IsActive ==true).Select
                            (
                                u => new MainCategory
                                {
                                    MainCategID = u.MainCategID,
                                    MainCategDesc = u.MainCategDesc,
                                }).OrderBy(x => x.MainCategDesc).ToList(); ;

                return region;


            }
            
        }
        public static List<MainCategories> GetRangesRelatedToZSM(int userID)
        {
            List<MainCategories> region = new List<MainCategories>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (userID == 1)
                {



                    region = dbContext.MainCategories.Where(rhr => rhr.IsActive == true).Select
                           (
                               u => new MainCategories
                               {
                                   ID = u.MainCategID,
                                   MainCategoryName = u.MainCategDesc,
                               }).OrderBy(x => x.MainCategoryName).ToList();

                }

                else if (userID == 1028 || userID == 1023 || userID == 1026 || userID == 1024 || userID == 1048 || userID == 1087 || userID == 1072 || userID == 1035 || userID == 1041|| userID==1040)
                {
                    List<MainCategories> regionalHeadData = new List<MainCategories>();
                    MainCategories comlist;
                    var regionalheadid = dbContext.RSMPortalUsers.Where(x => x.UserID == userID).Select(x => x.RangeID).Distinct().ToList();

                    foreach (var item in regionalheadid)
                    {
                       
                        comlist = new MainCategories();
                        comlist.ID = (int)item;
                        comlist.MainCategoryName = dbContext.MainCategories.Where(x => x.MainCategID == item).Select(x => x.MainCategDesc).FirstOrDefault();
                        region.Add(comlist);
                    }
                  
                }

                else
                {
                    var regionalheadid = dbContext.Users.Where(x => x.ID == userID).Select(x => x.RegionalheadRef).FirstOrDefault();
                    var rangeid = dbContext.RegionalHeads.Where(x => x.ID == regionalheadid).Select(x => x.RangeID).FirstOrDefault();

                    region = dbContext.MainCategories.Where(rhr => rhr.IsActive == true && rhr.MainCategID == rangeid).Select
                             (
                                 u => new MainCategories
                                 {
                                     ID = u.MainCategID,
                                     MainCategoryName = u.MainCategDesc,
                                 }).OrderBy(x => x.MainCategoryName).ToList();
                }
                return region;


            }

        }
        public static List<MainCategories> GetRangesRelatedToDealer(int userID)
        {
            List<MainCategories> region = new List<MainCategories>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {




                //var regionalheadid = dbContext.Users.Where(x => x.ID == userID).Select(x => x.DealerRefNo).FirstOrDefault();
                //var rangeid = dbContext.Dealers.Where(x => x.ID == regionalheadid).Select(x => x.RangeID).FirstOrDefault();

                //region = dbContext.MainCategories.Where(rhr => rhr.IsActive == true && rhr.MainCategID == rangeid).Select
                //         (
                //             u => new MainCategories
                //             {
                //                 ID = u.MainCategID,
                //                 MainCategoryName = u.MainCategDesc,
                //             }).OrderBy(x => x.MainCategoryName).ToList();


                region = dbContext.Regions.Where(rhr => rhr.IsActive == true).Select
                         (
                             u => new MainCategories
                             {
                                 ID = u.ID,
                                 MainCategoryName = u.Name,
                             }).OrderBy(x => x.MainCategoryName).ToList();


                return region;


            }

        }

        public static List<MainCategories> GetRangesForAdmin(int userID)
        {
            List<MainCategories> region = new List<MainCategories>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (userID == 1)
                {



                    region = dbContext.MainCategories.Where(rhr => rhr.IsActive == true).Select
                           (
                               u => new MainCategories
                               {
                                   ID = u.MainCategID,
                                   MainCategoryName = u.MainCategDesc,
                               }).OrderBy(x => x.MainCategoryName).ToList();

                }
                region.Insert(0, new MainCategories
                {
                    ID = 0,
                    MainCategoryName = "All"
                });


                return region;


            }

        }

        public static List<MainCategories> GetRanges()
        {
            List<MainCategories> region = new List<MainCategories>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {



                region = dbContext.MainCategories.Where(rhr => rhr.IsActive == true).Select
                         (
                             u => new MainCategories
                             {
                                 ID = u.MainCategID,
                                 MainCategoryName = u.MainCategDesc,
                             }).OrderBy(x => x.MainCategoryName).ToList(); ;

                return region;


            }

        }

        public static IEnumerable<RegionData> GetRegionForTH(int RegionalHeadType)
        {
            IEnumerable<RegionData> RegionList;


            using (FOSDataModel dbContext = new FOSDataModel())
            {

               
                    RegionList = from Regions in dbContext.Regions//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())
                                 where
                                 !
                                     ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RegionHeadID == RegionalHeadType)
                                       select new
                                       {
                                           RegionalHeadRegions.RegionID
                                       }).Distinct()).Contains(new { RegionID = Regions.ID })
                                 select new RegionData()
                                 {
                                     RegionID = Regions.ID,
                                     Name = Regions.Name,
                                     ShortCode = Regions.ShortCode,
                                     CreatedDate = Regions.CreatedDate,
                                     IsDeleted = Regions.IsDeleted,
                                     IsActive = Regions.IsActive,
                                     LastUpdate = Regions.LastUpdate
                                 };



                   
                

                return RegionList.ToList();
            }

        }


        public static IEnumerable<RegionData> GetRegionLists(int RegionalHeadType)
        {
            List<RegionData> regionalHeadData = new List<RegionData>();
            RegionData comlist;

            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if (RegionalHeadType == 1)
                {
                    regionalHeadData = dbContext.Regions.Where(u => u.IsDeleted == false)
                            .ToList().Select(
                                u => new RegionData
                                {
                                    ID = u.ID,

                                    Name = u.Name,

                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
                else
                {
                    var regionalheadid = dbContext.Users.Where(x => x.ID == RegionalHeadType).Select(x => x.RegionalheadRef).FirstOrDefault();
                    var Regionids = dbContext.RegionalHeadRegions.Where(x => x.RegionHeadID == regionalheadid).Select(x => x.RegionID).ToList();

                    foreach (var item in Regionids)
                    {

                        comlist = new RegionData();
                        comlist.ID = item;
                        comlist.Name = dbContext.Regions.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                        regionalHeadData.Add(comlist);
                    }
                }




                return regionalHeadData.ToList();
            }

        }
        public static List<RegionData> GetRegionList()
        {
            List<RegionData> city = new List<RegionData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Regions.Where(c => c.IsDeleted == false)
                            .Select
                            (
                                u => new RegionData
                                {
                                    RegionID = u.ID,
                                    Name = u.Name,
                                    //RegionID = u.RegionID,
                                    //RegionName = u.Region.Name,
                                    //ShortCode = u.ShortCode,
                                    //LastUpdate = u.LastUpdate
                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }

        public static List<Zone> GetZones()
        {
            List<Zone> zones = new List<Zone>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {



                zones = dbContext.Zones.ToList();

                return zones;


            }

        }

        public static List<SaleOfficer> GetAllSOListRelatedtoregionalHeadID(int RHID, bool selectOption = false)
        {
            List<SaleOfficer> saleOfficerData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if (RHID == 0 && !selectOption)
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(p => p.IsActive == true).OrderBy(p => p.Name).ToList();
                }
                else
                {
                    saleOfficerData = dbContext.SaleOfficers.Where(s => s.RegionalHeadID == RHID && s.IsActive == true).OrderBy(p => p.Name).ToList();
                }

            }
            return saleOfficerData;
        }

        public static IEnumerable<RegionData> GetRanges(int RegionalHeadType)
        {
            IEnumerable<RegionData> RegionList;


            using (FOSDataModel dbContext = new FOSDataModel())
            {


                RegionList = from Regions in dbContext.MainCategories//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())

                             select new RegionData()
                             {
                                 RegionID = Regions.MainCategID,
                                 Name = Regions.MainCategDesc,

                                 IsDeleted = Regions.IsDeleted,
                                 IsActive = Regions.IsActive


                             };

              

                return RegionList.ToList();
            }
        }

        // Get Regions For RegionalHead ...
        public static IEnumerable<RegionData> GetRegionForRegionalHead(int RegionalHeadType)
        {
            IEnumerable<RegionData> RegionList;

            
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (RegionalHeadType == 1)
                {
                    RegionList = from Regions in dbContext.Regions//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())
                                 where
                                 !
                                     ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == 1)
                                       select new
                                       {
                                           RegionalHeadRegions.RegionID
                                       }).Distinct()).Contains(new { RegionID = Regions.ID})
                                 select new RegionData()
                                 {
                                     RegionID = Regions.ID,
                                     Name = Regions.Name,
                                     ShortCode = Regions.ShortCode,
                                     CreatedDate = Regions.CreatedDate,
                                     IsDeleted = Regions.IsDeleted,
                                     IsActive = Regions.IsActive,
                                     LastUpdate = Regions.LastUpdate
                                 };
                }


                else if (RegionalHeadType == 3)
                {
                    RegionList = from Regions in dbContext.Regions//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())
                                 where
                                 !
                                     ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == 3)
                                       select new
                                       {
                                           RegionalHeadRegions.RegionID
                                       }).Distinct()).Contains(new { RegionID = Regions.ID })
                                 select new RegionData()
                                 {
                                     RegionID = Regions.ID,
                                     Name = Regions.Name,
                                     ShortCode = Regions.ShortCode,
                                     CreatedDate = Regions.CreatedDate,
                                     IsDeleted = Regions.IsDeleted,
                                     IsActive = Regions.IsActive,
                                     LastUpdate = Regions.LastUpdate
                                 };
                }
                else
                {
                    RegionList = from Regions in dbContext.Regions 
                                 where
                                 !
                                     ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == RegionalHeadType)
                                       select new
                                       {
                                           RegionalHeadRegions.RegionID
                                       }).Distinct()).Contains(new { RegionID = Regions.ID })
                                 select new RegionData()
                                 {
                                     RegionID = Regions.ID,
                                     Name = Regions.Name,
                                     ShortCode = Regions.ShortCode,
                                     CreatedDate = Regions.CreatedDate,
                                     IsDeleted = Regions.IsDeleted,
                                     IsActive = Regions.IsActive,
                                     LastUpdate = Regions.LastUpdate
                                 };
                }

                return RegionList.ToList();
            }
        }


        // Get Regions For RegionalHead In Edit Mode ...
        public static IEnumerable<RegionData> GetRegionForRegionalHeadEdit(int intRegionID , int RegionalHeadType)
        {
            IEnumerable<RegionData> RegionList;

            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (RegionalHeadType == 1)
                {
                    RegionList = (from Regions in dbContext.Regions
                                  where
                                  !
                                      ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == 1)
                                        select new
                                        {
                                            RegionalHeadRegions.RegionID
                                        }).Distinct()).Contains(new { RegionID = Regions.ID })
                                  select new RegionData()
                                  {
                                      RegionID = Regions.ID,
                                      Name = Regions.Name,
                                  }
                               ).Union
                               (
                                   from RegionalHeadRegions in dbContext.RegionalHeadRegions
                                   where
                                     RegionalHeadRegions.RegionHeadID == intRegionID
                                   select new RegionData()
                                   {
                                       RegionID = RegionalHeadRegions.Region.ID,
                                       Name = RegionalHeadRegions.Region.Name
                                   }
                               );
                }
                else
                {
                    RegionList = (from Regions in dbContext.Regions
                                  where
                                  !
                                      ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == 2)
                                        select new
                                        {
                                            RegionalHeadRegions.RegionID
                                        }).Distinct()).Contains(new { RegionID = Regions.ID })
                                  select new RegionData()
                                  {
                                      RegionID = Regions.ID,
                                      Name = Regions.Name,
                                  }
                               ).Union
                               (
                                   from RegionalHeadRegions in dbContext.RegionalHeadRegions
                                   where
                                     RegionalHeadRegions.RegionHeadID == intRegionID
                                   select new RegionData()
                                   {
                                       RegionID = RegionalHeadRegions.Region.ID,
                                       Name = RegionalHeadRegions.Region.Name
                                   }
                               );
                }

                return RegionList.ToList();
            }
        }



        public static IEnumerable<RegionData> GetItemsForRanges(int RegionalHeadType)
        {
            IEnumerable<RegionData> RegionList;


            using (FOSDataModel dbContext = new FOSDataModel())
            {

                if (RegionalHeadType == 6)
                {
                    RegionList = from Regions in dbContext.Items
                                 where Regions.MainCategID == 6 && Regions.IsActive==true
                                 
                                 select new RegionData()
                                 {
                                     RegionID = Regions.ItemID,
                                     Name = Regions.ItemName,

                                     IsDeleted = Regions.IsDeleted,
                                     IsActive = Regions.IsActive,

                                 };
                }


                else if (RegionalHeadType == 7)
                {
                    RegionList = from Regions in dbContext.Items//.Where(r=>r.RegionalHeadRegions.Select(a=>a.RegionalHeadtype == 1).FirstOrDefault())
                                 where Regions.MainCategID == 7 && Regions.IsActive == true

                                 select new RegionData()
                                 {
                                     RegionID = Regions.ItemID,
                                     Name = Regions.ItemName,

                                     IsDeleted = Regions.IsDeleted,
                                     IsActive = Regions.IsActive,

                                 };
                }
                else
                {
                    RegionList = from Regions in dbContext.Regions
                                 where
                                 !
                                     ((from RegionalHeadRegions in dbContext.RegionalHeadRegions.Where(r => r.RHType == 2)
                                       select new
                                       {
                                           RegionalHeadRegions.RegionID
                                       }).Distinct()).Contains(new { RegionID = Regions.ID })
                                 select new RegionData()
                                 {
                                     RegionID = Regions.ID,
                                     Name = Regions.Name,
                                     ShortCode = Regions.ShortCode,
                                     CreatedDate = Regions.CreatedDate,
                                     IsDeleted = Regions.IsDeleted,
                                     IsActive = Regions.IsActive,
                                     LastUpdate = Regions.LastUpdate
                                 };
                }

                return RegionList.ToList();
            }

        }

        // Insert OR Update Region ...
        public static int AddUpdateRegion(RegionData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Region RegionObj = new Region();

                        if (obj.RegionID == 0)
                        {
                            RegionObj.ID = dbContext.Regions.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            RegionObj.Name = obj.Name;
                            RegionObj.ShortCode = obj.ShortCode;
                            RegionObj.IsActive = true;
                            RegionObj.CreatedDate = DateTime.Now;
                            RegionObj.LastUpdate = DateTime.Now;
                            dbContext.Regions.Add(RegionObj);
                        }
                        else
                        {
                            RegionObj = dbContext.Regions.Where(u => u.ID == obj.RegionID).FirstOrDefault();
                            RegionObj.Name = obj.Name;
                            RegionObj.ShortCode = obj.ShortCode;
                            RegionObj.LastUpdate = DateTime.Now;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static int AddUpdateDelieveryBoys(RegionData obj,int? DealerID)
        {
          
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        DelieveryBoy RegionObj = new DelieveryBoy();

                        if (obj.RegionID == 0)
                        {
                            RegionObj.ID = dbContext.DelieveryBoys.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            RegionObj.Name = obj.Name;
                
                            RegionObj.IsActive = true;
                            RegionObj.IsDeleted = false;
                            RegionObj.CreatedOn = DateTime.UtcNow.AddHours(5);
                            RegionObj.DealerID = DealerID;
                            dbContext.DelieveryBoys.Add(RegionObj);
                        }
                        else
                        {
                            RegionObj = dbContext.DelieveryBoys.Where(u => u.ID == obj.RegionID).FirstOrDefault();
                            RegionObj.Name = obj.Name;
                            RegionObj.IsActive = true;
                            RegionObj.IsDeleted = false;
                            RegionObj.CreatedOn = DateTime.UtcNow.AddHours(5);
                            RegionObj.DealerID = DealerID;
                     
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        // Delete Region ...
        public static int DeleteRegion(int RegionID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Region obj = dbContext.Regions.Where(u => u.ID == RegionID).FirstOrDefault();
                    dbContext.Regions.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Region Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static int DeleteBoys(int RegionID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    DelieveryBoy obj = dbContext.DelieveryBoys.Where(u => u.ID == RegionID).FirstOrDefault();
                    obj.IsActive = false;
                    obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Region Failed");
                Resp = 1;
            }
            return Resp;
        }

        // Get All Regions For Grid ...
        public static List<RegionData> GetRegionForGrid()
        {
            List<RegionData> RegionData = new List<RegionData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RegionData = dbContext.Regions.Where(u => u.IsDeleted == false)
                            .Select(
                                u => new RegionData
                                {
                                    RegionID = u.ID,
                                    Name = u.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Region List Failed");
                throw;
            }

            return RegionData;
        }

        public static List<RegionData> GetDelieveryBoysForGrid(int? DealerID)
        {
            List<RegionData> RegionData = new List<RegionData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RegionData = dbContext.DelieveryBoys.Where(u => u.IsDeleted == false && u.DealerID==DealerID)
                            .ToList().Select(
                                u => new RegionData
                                {
                                    RegionID = u.ID,
                                    Name = u.Name,
                                    ActiveStatus=u.IsActive==true?"Yes":"No",
                                   
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Region List Failed");
                throw;
            }

            return RegionData;
        }

        public static List<RegionData> GetResult(string search, string sortOrder, int start, int length, List<RegionData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<RegionData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<RegionData> FilterResult(string search, List<RegionData> dtResult, List<string> columnFilters)
        {
            IQueryable<RegionData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.ShortCode != null && p.ShortCode.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[3].ToLower())))
                );

            return results;
        }




        public static List<RegionalHeadTypeData> GetRegionalHeadsType()
        {
            List<RegionalHeadTypeData> TypeData = new List<RegionalHeadTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TypeData = dbContext.RegionalHeadsTypes
                            .Select(
                                u => new RegionalHeadTypeData
                                {
                                    ID = u.ID,
                                    Type = u.Type,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return TypeData;
        }


        public static List<RegionalHeadTypeData> GetRangeType()
        {
            List<RegionalHeadTypeData> TypeData = new List<RegionalHeadTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TypeData = dbContext.MainCategories
                            .Select(
                                u => new RegionalHeadTypeData
                                {
                                    ID = u.MainCategID,
                                    Type = u.MainCategDesc,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return TypeData;
        }

        public static List<RegionalHeadTypeData> GetDesignations()
        {
            List<RegionalHeadTypeData> TypeData = new List<RegionalHeadTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TypeData = dbContext.SODesignations
                            .Select(
                                u => new RegionalHeadTypeData
                                {
                                    ID = u.ID,
                                    Type = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return TypeData;
        }


        public static List<RegionalHeadTypeData> GetSOTypes()
        {
            List<RegionalHeadTypeData> TypeData = new List<RegionalHeadTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TypeData = dbContext.SOTypes
                            .Select(
                                u => new RegionalHeadTypeData
                                {
                                    ID = u.ID,
                                    Type = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return TypeData;
        }

        public static List<RegionalHeadTypeData> GetSegmentTypes()
        {
            List<RegionalHeadTypeData> TypeData = new List<RegionalHeadTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TypeData = dbContext.Tbl_Segmenttype
                            .Select(
                                u => new RegionalHeadTypeData
                                {
                                    ID = u.ID,
                                    Type = u.Name,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return TypeData;
        }

        public static List<RegionalHeadTypeData> GetRegionalHeadsType(int Type)
        {
            List<RegionalHeadTypeData> TypeData = new List<RegionalHeadTypeData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    TypeData = dbContext.RegionalHeadsTypes.Where(r => r.ID == Type)
                            .Select(
                                u => new RegionalHeadTypeData
                                {
                                    ID = u.ID,
                                    Type = u.Type,
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return TypeData;
        }

        public static int AddUpdateActivityPurpose(PurposeOfActivityData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        ActivityPurpose RegionObj = new ActivityPurpose();

                        if (obj.ID == 0)
                        {
                            RegionObj.PurposeID = dbContext.ActivityPurposes.OrderByDescending(u => u.PurposeID).Select(u => u.PurposeID).FirstOrDefault() + 1;
                            RegionObj.PurposeName = obj.Name;
                            RegionObj.PurposeCode = obj.ShortCode;
                            RegionObj.Status =true;
                            RegionObj.IsDeleted = false;
                            RegionObj.CreatedOn = DateTime.Now;
                            RegionObj.CreatedBy = 1;
                            dbContext.ActivityPurposes.Add(RegionObj);
                        }
                        else
                        {
                            RegionObj = dbContext.ActivityPurposes.Where(u => u.PurposeID == obj.ID).FirstOrDefault();
                            RegionObj.PurposeName = obj.Name;
                            RegionObj.PurposeCode = obj.ShortCode;
                            RegionObj.Status = true;
                            RegionObj.IsDeleted = false;
                            RegionObj.CreatedOn = DateTime.Now;
                            RegionObj.CreatedBy = 1;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Region Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Region"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static List<PurposeOfActivityData> GetActivityPurposeForGrid()
        {
            List<PurposeOfActivityData> RegionData = new List<PurposeOfActivityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    RegionData = dbContext.ActivityPurposes.Where(u => u.IsDeleted == false)
                            .ToList().Select(
                                u => new PurposeOfActivityData
                                {
                                    ID = u.PurposeID,
                                    Name = u.PurposeName,
                                    ShortCode = u.PurposeCode,
                                  
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get Region List Failed");
                throw;
            }

            return RegionData;
        }


        public static List<PurposeOfActivityData> GetResult5(string search, string sortOrder, int start, int length, List<PurposeOfActivityData> dtResult, List<string> columnFilters)
        {
            return FilterResult5(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count5(string search, List<PurposeOfActivityData> dtResult, List<string> columnFilters)
        {
            return FilterResult5(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<PurposeOfActivityData> FilterResult5(string search, List<PurposeOfActivityData> dtResult, List<string> columnFilters)
        {
            IQueryable<PurposeOfActivityData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.ShortCode != null && p.ShortCode.ToLower().Contains(search.ToLower())))
                && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[3].ToLower())))
                );

            return results;
        }

        public static int DeleteActivityPurpose(int RegionID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    ActivityPurpose obj = dbContext.ActivityPurposes.Where(u => u.PurposeID == RegionID).FirstOrDefault();
                    dbContext.ActivityPurposes.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Region Failed");
                Resp = 1;
            }
            return Resp;
        }

    }
}