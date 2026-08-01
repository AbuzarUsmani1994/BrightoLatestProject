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
    public class ManageArea
    {

        // Get All Areas List ...
        public static List<AreaData> GetAreaList()
        {
            List<AreaData> area = new List<AreaData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    area = dbContext.Cities
                            .Select(
                                u => new AreaData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    RegionID = u.RegionID,
                                    //RegionName = u.City.Region.Name,
                                    //CityID = u.CityID,
                                    //CityName = u.City.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return area;
        }
        public static int DeleteNationalUserAccess(int ID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_Access obj = dbContext.Tbl_Access.Where(u => u.ID == ID).FirstOrDefault();
                    obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }
        public static int AddUpdateAccess(AreaData obj)
        {
            FOSDataModel dbContext = new FOSDataModel();
            int Res = 0;

            try
            {

              
                  
                        Tbl_Access AreaObj = new Tbl_Access();
                        if (obj.Type != 2)
                        {


                            if (obj.ID == 0)
                            {
                               // AreaObj.ID = dbContext.Tbl_Access.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                                AreaObj.SaleOfficerID = obj.SOID;
                                AreaObj.RegionID = obj.RegionalHeadID;
                                AreaObj.RepotedUP = obj.SOID1;
                                AreaObj.ReportedDown = obj.SOID2;
                                AreaObj.Status = true;
                                AreaObj.CreatedOn = DateTime.Now;
                                //  AreaObj.LastUpdate = DateTime.Now;
                                AreaObj.IsDeleted = false;

                                dbContext.Tbl_Access.Add(AreaObj);
                            }
                            else
                            {
                                AreaObj = dbContext.Tbl_Access.Where(u => u.ID == obj.ID).FirstOrDefault();
                                AreaObj.SaleOfficerID = obj.SOID;
                                //AreaObj.RegionID = obj.RegionID;
                                AreaObj.RepotedUP = obj.RepotedTo;
                                AreaObj.ReportedDown = obj.ReportedFor;
                                AreaObj.Status = true;
                                AreaObj.CreatedOn = DateTime.Now;

                                AreaObj.IsDeleted = false;
                            }
                        }
                        else
                        {
                  //  var HeadType = dbContext.SaleOfficers.Where(x => x.RegionalHeadID == obj.RegionalHeadID && x.SORoleID == 2).Select(x => x.ID).FirstOrDefault();


                    var obj1 = dbContext.Tbl_Access.Where(u => u.RepotedUP == obj.SOID).ToList();

                            foreach (var item in obj1)
                            {
                                dbContext.Tbl_Access.Remove(item);
                                dbContext.SaveChanges();
                            }


                  
                    

                              var sodata = dbContext.SaleOfficers.Where(x => x.IsActive==true && x.IsDeleted==false).ToList();
                                foreach (var item in sodata)
                                 {
                        Tbl_Access AreaObj1 = new Tbl_Access();
                        // AreaObj.ID = dbContext.Tbl_Access.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        AreaObj1.SaleOfficerID = item.ID;
                                    AreaObj1.RegionID = obj.RegionalHeadID;
                                    AreaObj1.RepotedUP = obj.SOID;
                                    AreaObj1.ReportedDown = item.ID;
                                    AreaObj1.Status = true;
                                    AreaObj1.CreatedOn = DateTime.Now;
                                    //  AreaObj.LastUpdate = DateTime.Now;
                                    AreaObj1.IsDeleted = false;

                                    dbContext.Tbl_Access.Add(AreaObj1);
                                    dbContext.SaveChanges();
                                }
                               
                           
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        
                  
              
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        // Get All Areas List By CityID ...
        public static List<AreaData> GetAreaListByCityID()
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var query = from saleoff in dbContext.SaleOfficers
                                select saleoff.Areas;

                    var qArea = dbContext.SaleOfficers.SelectMany(p => p.Areas);

                    var area = from areas in dbContext.Areas
                               where
                               !
                                   ((from saleoffarea in qArea
                                     select new
                                     {
                                         saleoffarea.ID
                                     }).Distinct()).Contains(new { ID = areas.ID })
                               orderby areas.Name
                               select new AreaData()
                               {
                                   ID = areas.ID,
                                   Name = areas.Name,
                               };

                    return area.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int DeleteAccess(int ID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_Access obj = dbContext.Tbl_Access.Where(u => u.ID == ID).FirstOrDefault();
                    dbContext.Tbl_Access.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static int DeleteSORegions(int ID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_SOREGIONS obj = dbContext.Tbl_SOREGIONS.Where(u => u.ID == ID).FirstOrDefault();
                    dbContext.Tbl_SOREGIONS.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }
        public static List<Area> GetAreaListByJOBCityID(int SOID, int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    List<Area> areaData = new List<Area>();
                    var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();
                    var SO = dbContext.SaleOfficers.Where(s => s.ID == SOID).FirstOrDefault();

                    var AreasIDs = dbContext.Jobs.Where(j => j.SaleOfficerID == SO.ID && j.CityID == CityID && j.IsDeleted == false).Select(j => j.Areas).ToList();

                    if (AreasIDs.Count() != 0)
                    {
                        var query = from val in FOS.Shared.Common.AppUtility.ListToCommaValues(AreasIDs).Split(',')
                                    select int.Parse(val);

                        areaData = dbContext.Areas.Where(a => a.CityID == CityID && !query.Contains(a.ID)).ToList();
                    }
                    else
                    {
                        areaData = dbContext.Areas.Where(a => a.CityID == CityID).ToList();
                    }
                    

                    return areaData.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get All Areas List By CityID ...
        public static List<AreaData> GetAreaListByCityID(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<AreaData> ChooseAreas = dbContext.Areas.AsEnumerable().Where(a => a.CityID == CityID).Select(a => new AreaData 
                    {
                        ID = a.ID,
                        Name = a.Name,
                        IsActive = a.IsActive
                    }).ToList();

                    ChooseAreas.Insert(0, new AreaData
                    {
                        ID = 0,
                        Name = "Select"
                    });

                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<AreaData> GetAllAreaListByCityID(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<AreaData> ChooseAreas = dbContext.Areas.AsEnumerable().Where(a => a.CityID == CityID).Select(a => new AreaData
                    {
                        ID = a.ID,
                        Name = a.Name,
                        IsActive = a.IsActive
                    }).ToList();

                    ChooseAreas.Insert(0, new AreaData
                    {
                        ID = 0,
                        Name = "All"
                    });

                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SubCategories> GetSubCatForAPI(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<SubCategories> ChooseAreas = dbContext.SubCategories.Where(a => a.MainCategID == CityID).Select(a => new SubCategories
                    {
                        ID = a.SubCategID,
                        SubName = a.SubCategDesc,
                        IsActive=a.IsActive
                       
                    }).ToList();

                

                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetCitiesForAPI(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Cities.Where(a => a.RegionID == CityID).Select(a => new CityData
                    {
                        ID = a.ID,
                        Name = a.Name,
                        IsActive = a.IsActive

                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<CityData> GetBusinessTypesForAPI(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.BusinessTypes.Where(a => a.SegmentID == CityID).Select(a => new CityData
                    {
                        ID = a.ID,
                        Name = a.Name,
                        IsActive = (bool)a.IsActive

                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<RegionData> GetRegionForAPI(int SOID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<RegionData> ChooseAreas = dbContext.Tbl_SOREGIONS.Where(a => a.SaleofficerID == SOID).Select(a => new RegionData
                    {
                        ID = dbContext.Regions.Where(u =>u.ID==a.RegionID).Select(u=>u.ID).FirstOrDefault(),
                        Name = dbContext.Regions.Where(u => u.ID == a.RegionID).Select(u => u.Name).FirstOrDefault(),
                        IsActive = dbContext.Regions.Where(u => u.ID == a.RegionID).Select(u => u.IsActive).FirstOrDefault(),

                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int AddUpdateSORegions(AreaData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_SOREGIONS AreaObj = new Tbl_SOREGIONS();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Tbl_SOREGIONS.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.SaleofficerID = obj.SOID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RegionalHeadID = dbContext.SaleOfficers.Where(x=>x.ID==obj.SOID).Select(x=>x.RegionalHeadID).FirstOrDefault();
                          
                            AreaObj.Status = true;
                          
                         
                          

                            dbContext.Tbl_SOREGIONS.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Tbl_SOREGIONS.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.SaleofficerID = obj.SOID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RegionalHeadID = dbContext.SaleOfficers.Where(x => x.ID == obj.SOID).Select(x => x.RegionalHeadID).FirstOrDefault();
                            AreaObj.Status = true;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static List<SubCategoryA> GetSubCatAForAPI(int MainCat, int SubCat)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {


                    List<SubCategoryA> Items = dbContext.SubCtegoryAs.Where(a => a.MainCategID == MainCat && a.SubCategID == SubCat).Select(a => new SubCategoryA
                    {
                        SubCategoryAID = a.SubCategIDA,
                        SubCategoryAName = a.SubCategADesc,
                        IsActive=a.IsActive

                    }).ToList();



                    return Items.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public static List<Items> GetItemsForAPI(int MainCat)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                  

                    List<Items> Items = dbContext.Items.Where(a => a.IsActive == true).Select(a => new Items
                    {
                        ItemId = a.ItemID,
                        ItemName = a.ItemName,
                        ItemPrice=a.Price,
                        ItemPacking=a.Packing,
                        IsActive = a.IsActive,
                        SortOrder=a.SortOrder
                       
                        

                    }).ToList();



                    return Items.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<AreaData> GetAreaListByCityIDForDealers(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<AreaData> ChooseAreas = dbContext.Areas.Where(a => a.CityID == CityID
                    && a.IsActive && !a.IsDeleted).Select(a => new AreaData
                    {
                        ID = a.ID,
                        Name = a.Name,
                        CityName = a.City.Name,
                        CityID = a.CityID
                    }).ToList();

                  

                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Get All Areas By CityID In Edit Mode ...
        public static List<Area> GetAreaListByCityIDJobEdit(int JobID, int SOID, int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    List<Area> areaData = new List<Area>();

                    var SO = dbContext.SaleOfficers.Where(s => s.ID == SOID).FirstOrDefault();

                    var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    var ChooseAreas = dbContext.Cities.Where(a => a.ID == CityID).SelectMany(a => a.Areas);

                    var AreasIDs = dbContext.Jobs.Where(j => j.ID == JobID && j.SaleOfficerID == SOID && j.CityID == CityID && j.IsDeleted == false).Select(j => j.Areas).FirstOrDefault();

                    if (AreasIDs != null)
                    {
                        var query = from val in AreasIDs.Split(',')
                                    select int.Parse(val);

                        areaData = dbContext.Areas.Where(a => a.CityID == CityID && query.Contains(a.ID)).ToList();
                    }
                    else
                    {
                        areaData = dbContext.Areas.Where(a => a.CityID == CityID).ToList();
                    }


                    return areaData.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<AreaData> GetAreasForAPI(int RegionID, int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {


                    List<AreaData> Items = dbContext.Areas.Where(a => a.RegionID == RegionID && a.CityID == CityID).Select(a => new AreaData
                    {
                        ID = a.ID,
                        Name = a.Name,
                        IsActive = a.IsActive

                    }).ToList();



                    return Items.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }





        public static List<CityData> GetRetailersForAPI(int RegionID, int CityID,int RangeID, int SOID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Retailers.Where(a => a.RegionID == RegionID && a.CityID==CityID && a.Status==true && a.RangeID == RangeID && a.SaleOfficerID==SOID).Select(a => new CityData
                    {
                        ID = a.ID,
                        ShopName = a.ID + "/" +" "+a.ShopName,
                        IsActive = a.IsActive,
                        ShopType=a.Shoptype,
                        address=a.Address,
                        OwnerName=a.Name,
                        Quota=a.Quota,
                        Phone=a.Phone1,
                        NewOrOld=a.NewOrOld,
                        Latitude=a.Latitude,
                        Longitude=a.Longitude,
                        AreaName=a.NewArea
                        
                        
                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetRetailersForAPIFinal(int RegionID, int CityID, int saleofficerid)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Retailers.Where(a => a.RegionID == RegionID && a.CityID == CityID && a.SaleOfficerID ==saleofficerid).Select(a => new CityData
                    {
                        ID = a.ID,
                        ShopName = a.ShopName,
                        IsActive = a.IsActive

                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }




        public static List<CityData> GetDistributorForAPI(int RegionID, int CityID,int RangeID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    List<CityData> ChooseAreas = dbContext.Dealers.Where(a => a.RegionID == RegionID /*&& a.CityID == CityID*/&& a.RangeID==RangeID).Select(a => new CityData
                    {
                        ID = a.ID,
                        ShopName = a.ShopName,
                        IsActive = a.IsActive

                    }).ToList();



                    return ChooseAreas.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static void DeleteRetailerAPI(int RetailerID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var CITY = dbContext.Cities.Where(s => s.ID == CityID).FirstOrDefault();

                    var retailers = dbContext.Retailers.Where(a => a.ID == RetailerID).FirstOrDefault();
                    retailers.IsDeleted = true;
                    dbContext.SaveChanges();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }










        //public static List<AreaData> GetRetailerForAPI(int ID)
        //{
        //    try
        //    {
        //        using (FOSDataModel dbContext = new FOSDataModel())
        //        {


        //            List<AreaData> Items = dbContext.Areas.Where(a => a.RegionID == RegionID && a.CityID == CityID).Select(a => new AreaData
        //            {
        //                ID = a.ID,
        //                Name = a.Name,
        //                IsActive = a.IsActive

        //            }).ToList();



        //            return Items.ToList();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}








        // Get All Areas By CityID In Edit Mode ...
        public static List<AreaData> GetAreaListByCityIDEdit(int intSaleOfficerID, int intCityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var query = from saleoff in dbContext.SaleOfficers
                                where saleoff.Areas.Any(s => s.CityID == intCityID)
                                select saleoff.Areas;

                    var qArea = dbContext.SaleOfficers.Where(p => p.CityID == intCityID).SelectMany(p => p.Areas);

                    var qAreaEdit = dbContext.SaleOfficers.Where(p => p.ID == intSaleOfficerID).SelectMany(p => p.Areas);

                    var area = (from areas in dbContext.Areas
                                where areas.CityID == intCityID &&
                                !
                                    ((from saleoffarea in qArea
                                      select new
                                      {
                                          saleoffarea.ID
                                      }).Distinct()).Contains(new { ID = areas.ID })
                                orderby areas.Name
                                select new AreaData()
                                {
                                    ID = areas.ID,
                                    Name = areas.Name,
                                }).Union
                                (
                                     from saleoffarea in qAreaEdit
                                     orderby saleoffarea.Name
                                     select new AreaData()
                                     {
                                         ID = saleoffarea.ID,
                                         Name = saleoffarea.Name,
                                     }
                                );

                    return area.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Insert OR Update Area ...
        public static int AddUpdateArea(AreaData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Area AreaObj = new Area();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Areas.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.Name = obj.Name;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.CityID = obj.CityID;
                            AreaObj.ShortCode = obj.ShortCode;
                            AreaObj.IsActive = true;
                            AreaObj.CreatedDate = DateTime.Now;
                            AreaObj.LastUpdate = DateTime.Now;
                            AreaObj.CreatedBy = 1;

                            dbContext.Areas.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Areas.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.Name = obj.Name;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.CityID = obj.CityID;
                            AreaObj.ShortCode = obj.ShortCode;
                            AreaObj.LastUpdate = DateTime.Now;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static int AddUpdateAttendance(AreaData obj)
        {
            int Res = 0;

            try
            {
                if (obj.AttendanceRows == null || obj.AttendanceRows.Count == 0)
                    return 0;

                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        var soIds = obj.AttendanceRows.Select(r => r.SOID).Distinct().ToList();

                        var existingRows = dbContext.Tbl_SOAttendanceandPunctuality
                            .Where(x => x.FinancialYearID == obj.FinancialYearID
                                     && x.Quarter == obj.Quarter
                                     && x.IsActive == true
                                     && x.SOID.HasValue
                                     && soIds.Contains(x.SOID.Value))
                            .ToList();
                        foreach (var row in existingRows)
                        {
                            row.IsActive = false;
                        }

                        var nextId = dbContext.Tbl_SOAttendanceandPunctuality
                            .OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        var nowDt = DateTime.Now;

                        foreach (var r in obj.AttendanceRows)
                        {
                            dbContext.Tbl_SOAttendanceandPunctuality.Add(new Tbl_SOAttendanceandPunctuality
                            {
                                ID = nextId++,
                                HeadID = obj.RegionID,
                                SOID = r.SOID,
                                AttendanceandPunctuality = r.Value,
                                FinancialYearID = obj.FinancialYearID,
                                Quarter = obj.Quarter,
                                IsActive = true,
                                CreatedOn = nowDt
                            });
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Attendance Failed");
                Res = 0;
                return Res;
            }
            return Res;
        }

        public static int AddUpdateTraining(AreaData obj)
        {
            int Res = 0;

            try
            {
                if (obj.TrainingRows == null || obj.TrainingRows.Count == 0)
                    return 0;

                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        var soIds = obj.TrainingRows.Select(r => r.SOID).Distinct().ToList();

                        var existingRows = dbContext.Tbl_SOTraining
                            .Where(x => x.FinancialYearID == obj.FinancialYearID
                                     && x.Quarter == obj.Quarter
                                     && x.IsActive == true
                                     && x.SOID.HasValue
                                     && soIds.Contains(x.SOID.Value))
                            .ToList();
                        foreach (var row in existingRows)
                        {
                            row.IsActive = false;
                        }

                        var nextId = dbContext.Tbl_SOTraining
                            .OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        var nowDt = DateTime.Now;

                        foreach (var r in obj.TrainingRows)
                        {
                            dbContext.Tbl_SOTraining.Add(new Tbl_SOTraining
                            {
                                ID = nextId++,
                                HeadID = obj.RegionID,
                                SOID = r.SOID,
                                Training = r.Value,
                                FinancialYearID = obj.FinancialYearID,
                                Quarter = obj.Quarter,
                                IsActive = true,
                                CreatedOn = nowDt
                            });
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Training Failed");
                Res = 0;
                return Res;
            }
            return Res;
        }

        // Delete Area...
        public static int DeleteArea(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Area obj = dbContext.Areas.Where(u => u.ID == AreaID).FirstOrDefault();
                    dbContext.Areas.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Area Failed");
                Resp = 1;
            }
            return Resp;
        }


        // Get All Areas List For Grid ...
        public static List<AreaData> GetAreaForGrid(int intCityID)
        {
            List<AreaData> areaData = new List<AreaData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Areas.Where(u => u.CityID == intCityID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new AreaData
                                {
                                    ID = u.ID,
                                    RegionID = u.RegionID,
                                    CityID = u.CityID,
                                    Name = u.Name,
                                    RegionName = "",
                                    CityName = u.City.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }

        public static List<AreaData> GetAttendanceForGrid(int intRegionalHeadID, int intFinancialYearID, string quarter)
        {
            List<AreaData> areaData = new List<AreaData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var q = dbContext.Tbl_SOAttendanceandPunctuality.Where(u => u.IsActive == true);
                    if (intRegionalHeadID > 0)
                        q = q.Where(u => u.HeadID == intRegionalHeadID);
                    if (intFinancialYearID > 0)
                        q = q.Where(u => u.FinancialYearID == intFinancialYearID);
                    if (!string.IsNullOrEmpty(quarter))
                        q = q.Where(u => u.Quarter == quarter);

                    var rows = q.ToList();
                    var fyIds = rows.Where(r => r.FinancialYearID.HasValue).Select(r => r.FinancialYearID.Value).Distinct().ToList();
                    var fyDict = dbContext.Database
                        .SqlQuery<FinancialYearListItem>("SELECT ID, [Year] FROM dbo.Tbl_FinancialYear WHERE ID IN (" + (fyIds.Count == 0 ? "0" : string.Join(",", fyIds)) + ")")
                        .ToDictionary(x => x.ID, x => x.Year);

                    areaData = rows.Select(u => new AreaData
                    {
                        ID = u.ID,
                        Name = u.AttendanceandPunctuality.ToString(),
                        RegionName = dbContext.RegionalHeads.Where(x => x.ID == u.HeadID).Select(x => x.Name).FirstOrDefault(),
                        CityName = dbContext.SaleOfficers.Where(x => x.ID == u.SOID).Select(x => x.Name).FirstOrDefault(),
                        FinancialYearID = u.FinancialYearID,
                        FinancialYearName = u.FinancialYearID.HasValue && fyDict.ContainsKey(u.FinancialYearID.Value) ? fyDict[u.FinancialYearID.Value] : "",
                        Quarter = u.Quarter,
                        CreatedOn = u.CreatedOn
                    }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Attendance Grid Failed");
                throw;
            }

            return areaData;
        }

        public static List<AreaData> GetTrainingForGrid(int intRegionalHeadID, int intFinancialYearID, string quarter)
        {
            List<AreaData> areaData = new List<AreaData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var q = dbContext.Tbl_SOTraining.Where(u => u.IsActive == true);
                    if (intRegionalHeadID > 0)
                        q = q.Where(u => u.HeadID == intRegionalHeadID);
                    if (intFinancialYearID > 0)
                        q = q.Where(u => u.FinancialYearID == intFinancialYearID);
                    if (!string.IsNullOrEmpty(quarter))
                        q = q.Where(u => u.Quarter == quarter);

                    var rows = q.ToList();
                    var fyIds = rows.Where(r => r.FinancialYearID.HasValue).Select(r => r.FinancialYearID.Value).Distinct().ToList();
                    var fyDict = dbContext.Database
                        .SqlQuery<FinancialYearListItem>("SELECT ID, [Year] FROM dbo.Tbl_FinancialYear WHERE ID IN (" + (fyIds.Count == 0 ? "0" : string.Join(",", fyIds)) + ")")
                        .ToDictionary(x => x.ID, x => x.Year);

                    areaData = rows.Select(u => new AreaData
                    {
                        ID = u.ID,
                        Name = u.Training.ToString(),
                        RegionName = dbContext.RegionalHeads.Where(x => x.ID == u.HeadID).Select(x => x.Name).FirstOrDefault(),
                        CityName = dbContext.SaleOfficers.Where(x => x.ID == u.SOID).Select(x => x.Name).FirstOrDefault(),
                        FinancialYearID = u.FinancialYearID,
                        FinancialYearName = u.FinancialYearID.HasValue && fyDict.ContainsKey(u.FinancialYearID.Value) ? fyDict[u.FinancialYearID.Value] : "",
                        Quarter = u.Quarter,
                        CreatedOn = u.CreatedOn
                    }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Training Grid Failed");
                throw;
            }

            return areaData;
        }


        public static List<Tbl_AccessModel> GetAccessForGrid(int intCityID)
        {
            List<Tbl_AccessModel> areaData = new List<Tbl_AccessModel>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Tbl_Access.Where(u => u.RegionID == intCityID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new Tbl_AccessModel
                                {
                                  ID = u.ID,
                                  RegionID = u.RegionID,
                                  RegionName= dbContext.RegionalHeads.Where(x=>x.ID==u.RegionID).Select(x=>x.Name).FirstOrDefault(),
                                  SaleOfficerID=u.SaleOfficerID,
                                  RepotedTo=u.RepotedUP,
                                  ReportedToName=dbContext.SaleOfficers.Where(x=>x.ID==u.RepotedUP).Select(x=>x.Name).FirstOrDefault(),
                                  SaleOfficerName=u.SaleOfficer.Name,
                                 ReportedFor = u.ReportedDown,
                                 ReportedForName = dbContext.SaleOfficers.Where(x => x.ID == u.ReportedDown).Select(x => x.Name).FirstOrDefault()

                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }

        public static List<SubCategories> GetSubCatForGrid(int intCityID)
        {
            List<SubCategories> areaData = new List<SubCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Tbl_ProductSubCategory.Where(u => u.fkProductCategoryID == intCityID && u.IsActive == true)
                            .ToList().Select(
                                u => new SubCategories
                                {
                                    ID = u.ID,
                                    SubName = u.Name,

                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }


        public static List<Tbl_AccessModel> GetSORegionsForGrid(int intCityID)
        {
            List<Tbl_AccessModel> areaData = new List<Tbl_AccessModel>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Tbl_SOREGIONS.Where(u => u.RegionID == intCityID && u.Status == true)
                            .ToList().Select(
                                u => new Tbl_AccessModel
                                {
                                    ID = u.ID,
                                    RegionID = u.RegionID,
                                    RegionName = dbContext.Regions.Where(x=>x.ID==u.RegionID).Select(x=>x.Name).FirstOrDefault(),
                                    SaleOfficerID = u.SaleofficerID,
                                    SaleOfficerName= dbContext.SaleOfficers.Where(x => x.ID == u.SaleofficerID).Select(x => x.Name).FirstOrDefault(),
                                    RHID=u.RegionalHeadID,
                                    RegionalHeadName = dbContext.RegionalHeads.Where(x => x.ID == u.RegionalHeadID).Select(x => x.Name).FirstOrDefault(),


                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }

        public static int AddUpdateSubCAt(SubCategories obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_ProductSubCategory AreaObj = new Tbl_ProductSubCategory();

                        if (obj.ID == 0)
                        {
                            //AreaObj.SubCategID = dbContext.SubCategories.OrderByDescending(u => u.SubCategID).Select(u => u.SubCategID).FirstOrDefault() + 1;
                            AreaObj.fkProductCategoryID = obj.MainCategoryID;
                            AreaObj.Name = obj.SubName;
                            AreaObj.IsActive = true ;
                          
                            AreaObj.CreatedOn = DateTime.Now;
                           

                            dbContext.Tbl_ProductSubCategory.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Tbl_ProductSubCategory.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.Name = obj.SubName;
                            AreaObj.IsActive = true;
                         
                            AreaObj.CreatedOn = DateTime.Now;
                           
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }
        public static int DeleteSubCat(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_ProductSubCategory obj = dbContext.Tbl_ProductSubCategory.Where(u => u.ID == AreaID).FirstOrDefault();
                    dbContext.Tbl_ProductSubCategory.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete SubCategory Failed");
                Resp = 1;
            }
            return Resp;
        }


        public static List<AreaData> GetResult(string search, string sortOrder, int start, int length, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<AreaData> FilterResult(string search, List<AreaData> dtResult, List<string> columnFilters)
        {
            IQueryable<AreaData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionName != null && p.RegionName.ToLower().Contains(search.ToLower()) || p.CityName != null && p.CityName.ToLower().Contains(search.ToLower()) || p.ShortCode != null && p.ShortCode.ToLower().Contains(search.ToLower())))
                && (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[6].ToLower())))
                );

            return results;
        }


        public static List<SubCategories> GetResult1(string search, string sortOrder, int start, int length, List<SubCategories> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count1(string search, List<SubCategories> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<SubCategories> FilterResult1(string search, List<SubCategories> dtResult, List<string> columnFilters)
        {
            IQueryable<SubCategories> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SubName != null && p.SubName.ToLower().Contains(search.ToLower()) )
       
                ));

            return results;
        }
        public static List<Items> GetResult2(string search, string sortOrder, int start, int length, List<Items> dtResult, List<string> columnFilters)
        {
            return FilterResult2(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count2(string search, List<Items> dtResult, List<string> columnFilters)
        {
            return FilterResult2(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<Items> FilterResult2(string search, List<Items> dtResult, List<string> columnFilters)
        {
            IQueryable<Items> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.ItemName != null && p.ItemName.ToLower().Contains(search.ToLower()) )
                
                ));

            return results;
        }


        public static List<SubCategoryA> GetResult3(string search, string sortOrder, int start, int length, List<SubCategoryA> dtResult, List<string> columnFilters)
        {
            return FilterResult3(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count3(string search, List<SubCategoryA> dtResult, List<string> columnFilters)
        {
            return FilterResult3(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<SubCategoryA> FilterResult3(string search, List<SubCategoryA> dtResult, List<string> columnFilters)
        {
            IQueryable<SubCategoryA> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SubCategoryAName != null && p.SubCategoryAName.ToLower().Contains(search.ToLower()))

                ));

            return results;
        }




        public static List<Items> GetItemsForGrid(int intCityID)
        {
            List<Items> areaData = new List<Items>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    areaData = dbContext.Items.Where(u => u.MainCategID == intCityID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new Items
                                {
                                    ItemId = u.ItemID,
                                    ItemName = u.ItemName,
                                    ItemPrice = u.Price,
                                    ItemCode=u.ItemCode,
                                    //SubCategoryAName=dbContext.SubCategories.Where(x=>x.SubCategID==u.SubCategID).Select(x=>x.SubCategDesc).FirstOrDefault(),
                                    ItemPacking=u.Packing,
                                    SortOrder=u.SortOrder,
                                    IsActiveYes = u.IsActive == true ? "Yes" : "No"

                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }



        public static int AddUpdateItems(Items obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Item AreaObj = new Item();

                        if (obj.ItemId == 0)
                        {
                            AreaObj.ItemID = dbContext.Items.OrderByDescending(u => u.ItemID).Select(u => u.ItemID).FirstOrDefault() + 1;
                            AreaObj.ItemName = obj.ItemName;
                            AreaObj.ItemDesc = null;
                            AreaObj.ItemCode = obj.ItemCode;
                            AreaObj.Price = (decimal)obj.ItemPrice;
                            AreaObj.Packing = obj.ItemPacking;
                            AreaObj.MainCategID = obj.MainCategoryID;
                            AreaObj.SubCategID = 1;
                            AreaObj.SubCategIDA = 1;
                            AreaObj.IsActive = true;
                            AreaObj.IsDeleted = false;
                            AreaObj.CreatedOn = DateTime.Now;
                            AreaObj.UpdatedOn = DateTime.Now;
                            AreaObj.SortOrder = obj.SortOrder;
                            AreaObj.CreatedBy = 1;
                            AreaObj.UpdatedBy = 1;

                            dbContext.Items.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Items.Where(u => u.ItemID == obj.ItemId).FirstOrDefault();
                            AreaObj.ItemName = obj.ItemName;
                            AreaObj.ItemDesc = null;
                            AreaObj.ItemCode = obj.ItemCode;
                            AreaObj.Price = (decimal)obj.ItemPrice;
                            AreaObj.Packing = obj.ItemPacking;
                            AreaObj.MainCategID = obj.MainCategoryID;
                            AreaObj.SubCategID = 1;
                            AreaObj.SubCategIDA = 1;
                            AreaObj.IsActive = obj.IsActive;
                            AreaObj.IsDeleted = false;
                            AreaObj.SortOrder = obj.SortOrder;
                            AreaObj.CreatedOn = DateTime.Now;
                            AreaObj.UpdatedOn = DateTime.Now;
                            AreaObj.CreatedBy = 1;
                            AreaObj.UpdatedBy = 1;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Item Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        public static int DeleteItem(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Item obj = dbContext.Items.Where(u => u.ItemID == AreaID).FirstOrDefault();
                    dbContext.Items.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Items Failed");
                Resp = 1;
            }
            return Resp;
        }




        public static int AddUpdateSubCatA(SubCategoryA obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_ProductDetail AreaObj = new Tbl_ProductDetail();

                        if (obj.SubCategoryAID == 0)
                        {
                            //AreaObj.SubCategIDA = dbContext.SubCtegoryAs.OrderByDescending(u => u.SubCategIDA).Select(u => u.SubCategIDA).FirstOrDefault() + 1;
                            //AreaObj.Name = obj.SubCategoryAName;
                          
                            //AreaObj.FkProductCategoryID = obj.MainCategoryID;
                            //AreaObj.fkProductSubCategoryID = obj.ID;
                            //AreaObj.IsActive = true;
                            //AreaObj.SalesIncentive = obj.LiterPrice;
                            //AreaObj.CreatedOn = DateTime.Now;
                            

                            dbContext.Tbl_ProductDetail.Add(AreaObj);
                        }
                        else
                        {
                            //AreaObj = dbContext.Tbl_ProductDetail.Where(u => u.ID == obj.SubCategoryAID).FirstOrDefault();
                            //AreaObj.Name = obj.SubCategoryAName;
                        
                            //AreaObj.FkProductCategoryID = obj.MainCategoryID;
                            //AreaObj.fkProductSubCategoryID = obj.ID;
                            //AreaObj.IsActive = true;
                            //AreaObj.SalesIncentive = obj.LiterPrice;
                            //AreaObj.CreatedOn = DateTime.Now;
                           
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Item Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        public static List<SubCategoryA> GetSubCatAForGrid(int intCityID, int SubCat)
        {
            List<SubCategoryA> areaData = new List<SubCategoryA>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //areaData = dbContext.Tbl_ProductDetail.Where(u => u.FkProductCategoryID == intCityID && u.fkProductSubCategoryID == SubCat && u.IsActive == true)
                    //        .ToList().Select(
                    //            u => new SubCategoryA
                    //            {
                    //                SubCategoryAID = u.ID,
                    //                SubCategoryAName = u.Name,
                    //                MainCatName= dbContext.Tbl_ProuctCategory.Where(y => y.ID == intCityID).Select(y=>y.Name).FirstOrDefault(),
                    //        SubCatName = dbContext.Tbl_ProductSubCategory.Where(x => x.ID == SubCat).Select(x => x.Name).FirstOrDefault(),
                    //        LiterPrice=u.SalesIncentive

                    //            }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load Area Grid Failed");
                throw;
            }

            return areaData;
        }

        public static int DeleteSubCatA(int AreaID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_ProductDetail obj = dbContext.Tbl_ProductDetail.Where(u => u.ID == AreaID).FirstOrDefault();
                    dbContext.Tbl_ProductDetail.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete Items Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static List<Tbl_AccessModel> GetResult7(string search, string sortOrder, int start, int length, List<Tbl_AccessModel> dtResult, List<string> columnFilters)
        {
            return FilterResult7(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count7(string search, List<Tbl_AccessModel> dtResult, List<string> columnFilters)
        {
            return FilterResult7(search, dtResult, columnFilters).Count();
        }


        private static IQueryable<Tbl_AccessModel> FilterResult7(string search, List<Tbl_AccessModel> dtResult, List<string> columnFilters)
        {
            IQueryable<Tbl_AccessModel> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.SaleOfficerName != null && p.SaleOfficerName.ToLower().Contains(search.ToLower()))
                //&& (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                //&& (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                //&& (columnFilters[5] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[5].ToLower())))
                //&& (columnFilters[6] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[6].ToLower())))
                ));

            return results;
        }

        public static int AddUpdateAccess(Tbl_AccessModel obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_Access AreaObj = new Tbl_Access();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Tbl_Access.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.SaleOfficerID = obj.SaleOfficerID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RepotedUP = obj.RepotedTo;
                            AreaObj.ReportedDown = obj.ReportedFor;
                            AreaObj.Status = true;
                            AreaObj.CreatedOn = DateTime.Now;
                          //  AreaObj.LastUpdate = DateTime.Now;
                            AreaObj.IsDeleted = false;

                            dbContext.Tbl_Access.Add(AreaObj);
                        }
                        else
                        {
                            AreaObj = dbContext.Tbl_Access.Where(u => u.ID == obj.ID).FirstOrDefault();
                            AreaObj.SaleOfficerID = obj.SaleOfficerID;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.RepotedUP = obj.RepotedTo;
                            AreaObj.ReportedDown = obj.ReportedFor;
                            AreaObj.Status = true;
                            AreaObj.CreatedOn = DateTime.Now;
                       
                            AreaObj.IsDeleted = false;
                        }
                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add Area Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code Area"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

    }
}