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
    public class ManageCity
    {
        public static List<SaleOfficerData> GetSoRegionWise(int intRegionID)
        {
            List<SaleOfficerData> so = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    so = dbContext.SaleOfficers.Where(c => c.RegionID == intRegionID && c.IsDeleted == false && c.IsActive == true)
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).ToList();
                    so.Insert(0, new SaleOfficerData
                    {
                        ID = 0,
                        Name = "--Select Sale Officer--"
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return so;
        }
        public static List<SaleOfficerData> GetSaleOfficersByRegionID(int intRegionHeadID)
        {
            List<SaleOfficerData> so = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    so = dbContext.SaleOfficers.Where(c => c.RegionalHeadID == intRegionHeadID && c.IsDeleted == false && c.IsActive == true)
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).ToList();
                    so.Insert(0, new SaleOfficerData
                    {
                        ID = 0,
                        Name = "--Select Sale Officer--"
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return so;
        }
        public static List<RegionalHeadData> getRegionalHeadsByRegionID(int intRegionID)
        {
            List<RegionalHeadData> so = new List<RegionalHeadData>();
            RegionalHeadData rhd;
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (intRegionID == 1)
                    {
                        List<RegionalHeadRegion> rhr = dbContext.RegionalHeadRegions.Where(x => x.RegionID == intRegionID && x.IsActive == true && x.IsDeleted == false).ToList();
                        foreach (var item in rhr)
                        {
                            rhd = new RegionalHeadData();
                            rhd.ID = item.RegionHeadID;
                            rhd.Name = dbContext.RegionalHeads.Where(x => x.ID == item.RegionHeadID).Select(x => x.Name).FirstOrDefault();
                            so.Add(rhd);

                        }

                    }
                    else
                    {
                        var headid = dbContext.Users.Where(x => x.ID == intRegionID).Select(x => x.RegionalheadRef).FirstOrDefault();
                        List<RegionalHead> rhr = dbContext.RegionalHeads.Where(x => x.ID == headid && x.IsActive == true && x.IsDeleted == false).ToList();
                        foreach (var item in rhr)
                        {
                            rhd = new RegionalHeadData();
                            rhd.ID = item.ID;
                            rhd.Name = dbContext.RegionalHeads.Where(x => x.ID == item.ID).Select(x => x.Name).FirstOrDefault();
                            so.Add(rhd);

                        }

                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return so;
        }
        // Get All Region Method ...
        public static List<CityData> GetCityList()
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Cities.Where(c => c.IsDeleted == false )
                            .Select
                            (
                                u => new CityData
                                {
                                    ID = u.ID,
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

        public static List<CityData> GetCitiesList(int ID)
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Cities.Where(c => c.IsDeleted == false && c.RegionID==ID)
                            .Select
                            (
                                u => new CityData
                                {
                                    ID = u.ID,
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


        public static List<RegionData> GetRegionsListRelatedToUser(int userID)
        {
            List<RegionData> city = new List<RegionData>();
            RegionData data;
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


        public static List<RegionData> GetDelieveryBoysList(int? UserID)
        {
            List<RegionData> city = new List<RegionData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.DelieveryBoys.Where(c => c.IsDeleted == false && c.DealerID==UserID)
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

        public static List<CityData> GetCityListCombo()
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Cities.Where(c => c.IsDeleted == false)
                            .Select
                            (
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name
                                }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            city.Insert(0, new CityData
            {
                ID = 0,
                Name = "Select"
            });
            return city;
        }

        // Get Cities List By RegionalHeadID ...
        public static List<CityData> GetCityListByRegionHeadID()
        {
            IEnumerable<CityData> cityList;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = from Cities in dbContext.Cities
                               where
                                   (from RegionalHeadRegions in dbContext.RegionalHeadRegions
                                    
                                    select new
                                    {
                                        RegionalHeadRegions.RegionID
                                    }).Contains(new { RegionID = Cities.RegionID })
                               select new CityData()
                               {
                                   ID = Cities.ID,
                                   Name = Cities.Name,
                               };

                    return cityList.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetCityListBySOID(int soId)
        {
            IEnumerable<CityData> qryCity;
            List<CityData> cityList = new List<CityData>();

            cityList.Add(new CityData
            {
                ID = 0,
                Name = "All"
            });
       

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    List<int> soRetailersCities = dbContext.Retailers.Where(p => p.SaleOfficerID == soId && p.Status).Select(pp => pp.CityID.Value).ToList<int>();
                    qryCity = dbContext.Cities.Where(p => soRetailersCities.Contains(p.ID)).Select(c => new CityData()
                    {
                        ID = c.ID,
                        Name = c.Name,
                    }).OrderBy(x => x.Name).ToList();

                    foreach (var cty in qryCity)
                    {
                        cityList.Add(cty);
                    }

                    return cityList;
                }
            }
            catch (Exception)
            {
                return new List<CityData>();
            }
        }
        public static List<CityData> GetCityListByRegionIDD(int soId)
        {
            IEnumerable<CityData> qryCity;
            List<CityData> cityList = new List<CityData>();

            cityList.Add(new CityData
            {
                ID = 0,
                Name = "All"
            });


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                  
                    qryCity = dbContext.Cities.Where(p => p.RegionID==soId).Select(c => new CityData()
                    {
                        ID = c.ID,
                        Name = c.Name,
                    }).OrderBy(x => x.Name).ToList();

                    foreach (var cty in qryCity)
                    {
                        cityList.Add(cty);
                    }

                    return cityList;
                }
            }
            catch (Exception)
            {
                return new List<CityData>();
            }
        }

        public static List<CityData> GetCitiesByRegionID(int soId)
        {
            IEnumerable<CityData> qryCity;
            List<CityData> cityList = new List<CityData>();

            cityList.Add(new CityData
            {
                ID = 0,
                Name = "All"
            });


            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    
                    qryCity = dbContext.Cities.Where(p => p.RegionID==soId).Select(c => new CityData()
                    {
                        ID = c.ID,
                        Name = c.Name,
                    }).OrderBy(x => x.Name).ToList();

                    foreach (var cty in qryCity)
                    {
                        cityList.Add(cty);
                    }

                    return cityList;
                }
            }
            catch (Exception)
            {
                return new List<CityData>();
            }
        }


        // Get Cities List By RegionalHeadID ...
        public static List<CityData> GetCityListByRegionHeadID(int RegionID, string selectText = "Select")
        {
            List<CityData> cityList;

            try
            {
               
                
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                    cityList = dbContext.Cities.Where(c => c.RegionID == RegionID && c.IsDeleted == false).OrderBy(c => c.Name)
                                .Select(
                                    u => new CityData
                                    {
                                        ID = u.ID,
                                        Name = u.Name,
                                     
                                    }).ToList();
                    }
                    var ctyList = cityList.OrderBy(x => x.Name).ToList();
                    ctyList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = selectText
                    });
                    return ctyList;
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetCityListByRegionID(int RegionID, string selectText = "Select")
        {
            IEnumerable<CityData> cityList;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = from Cities in dbContext.Cities
                               where Cities.RegionID== RegionID

                                  
                                   
                               select new CityData()
                               {
                                   ID = Cities.ID,
                                   Name = Cities.Name,
                                   IsActive = Cities.IsActive
                               };
                    var ctyList = cityList.OrderBy(x => x.Name).ToList();
                    ctyList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = selectText
                    });
                    return ctyList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetRetailersByRegionID(int RegionID,int CityID, string selectText = "Select")
        {
            IEnumerable<CityData> cityList;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = from Cities in dbContext.Retailers
                               where Cities.RegionID == RegionID && Cities.CityID==CityID



                               select new CityData()
                               {
                                   ID = Cities.ID,
                                   Name = Cities.ShopName,
                                   IsActive = Cities.IsActive
                               };
                    var ctyList = cityList.OrderBy(x => x.Name).ToList();
                    ctyList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = selectText
                    });
                    return ctyList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static List<CityData> GetCityByRegionID(int RegionID, string selectText = "All")
        {
            IEnumerable<CityData> cityList;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                    if (RegionID > 0)
                    {
                        cityList = from Cities in dbContext.Cities
                                   where Cities.RegionID == RegionID



                                   select new CityData()
                                   {
                                       ID = Cities.ID,
                                       Name = Cities.Name,
                                       IsActive = Cities.IsActive
                                   };
                        var ctyList = cityList.OrderBy(x => x.Name).ToList();
                        ctyList.Insert(0, new CityData
                        {
                            ID = 0,
                            Name = selectText
                        });
                        return ctyList;
                    }
                    else
                    {
                        cityList = from Cities in dbContext.Cities
                                  



                                   select new CityData()
                                   {
                                       ID = Cities.ID,
                                       Name = Cities.Name,
                                       IsActive = Cities.IsActive
                                   };
                        var ctyList = cityList.OrderBy(x => x.Name).ToList();
                        ctyList.Insert(0, new CityData
                        {
                            ID = 0,
                            Name = selectText
                        });
                        return ctyList;

                    }
                  
                   
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public static List<CityData> GetCityListByDealerId(int dealerId)
        {
            IEnumerable<CityData> cityList;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityList = from x in dbContext.Dealers
                               where x.ID == dealerId
                               select new CityData()
                               {
                                   ID = x.CityID ?? 0,
                                   Name = x.City.Name,
                                   IsActive = x.City.IsActive
                               };
                    var ctyList = cityList.ToList();
                    ctyList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = "Select"
                    });
                    return ctyList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<CityData> GetCityListByDealerRetailerCityIds(List<int> retailerCityIds)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var ctyList = dbContext.Cities.Where(x => retailerCityIds.Contains(x.ID) && x.IsActive && !x.IsDeleted)
                         .Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    IsActive = u.IsActive
                                }).ToList();

                    ctyList.Insert(0, new CityData
                    {
                        ID = 0,
                        Name = "Select"
                    });
                    return ctyList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get Cities List By RegionID ...
        public static List<CityData> GetCityListByRegionID(int intRegionID)
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Cities.Where(c => c.RegionID == intRegionID && c.IsDeleted == false).OrderBy(c => c.Name)
                            .Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                    RegionID = u.RegionID,
                                    RegionName = u.Region.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }



        public static List<CityData> GetAttendanceListByRegionID(int intRegionID)
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SaleOfficers.Where(c => c.RegionalHeadID == intRegionID && c.IsDeleted == false).OrderBy(c => c.Name)
                            .Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                  
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }

        public static List<CityData> GettrainingListByRegionID(int intRegionID)
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SaleOfficers.Where(c => c.RegionalHeadID == intRegionID && c.IsDeleted == false).OrderBy(c => c.Name)
                            .Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,

                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }

        public static List<SaleOfficerData> GetSOListByRegionID(int intRegionID)
        {
            List<SaleOfficerData> city = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {

                   // var regionalheadid = dbContext.RegionalHeadRegions.Where(x => x.RegionID == intRegionID).Select(x => x.RegionHeadID).ToList();

                    city = dbContext.SaleOfficers.Where(c => c.RegionalHeadID==intRegionID && c.IsActive==true)
                           .Select(
                               u => new SaleOfficerData
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

            return city;
        }



        public static List<SubCategories> GetSubCatList(int intRegionID)
        {
            List<SubCategories> city = new List<SubCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SubCategories.Where(c => c.MainCategID == intRegionID && c.IsDeleted == false).OrderBy(c => c.SubCategDesc)
                            .Select(
                                u => new SubCategories
                                {
                                    ID = u.SubCategID,
                                    SubName = u.SubCategDesc
                                
                                }).ToList();
                        }
            }
            catch (Exception)
            {
                throw;
            }
            city.Insert(0, new SubCategories
            {
                ID = 0,
                SubName = "All"
            });
            return city;
        }

        public static List<SubCategories> GetProdSubCatList(int intRegionID)
        {
            List<SubCategories> city = new List<SubCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Tbl_ProductSubCategory.Where(c => c.fkProductCategoryID == intRegionID && c.IsActive == true).OrderBy(c => c.Name)
                            .Select(
                                u => new SubCategories
                                {
                                    ID = u.ID,
                                    SubName = u.Name

                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            city.Insert(0, new SubCategories
            {
                ID = 0,
                SubName = "All"
            });
            return city;
        }



        public static List<Items> GetItemsList(int intRegionID,int subCatID)
        {
            List<Items> city = new List<Items>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Items.Where(c => c.MainCategID == intRegionID && c.SubCategID==subCatID && c.IsDeleted == false).OrderBy(c => c.ItemDesc)
                            .Select(
                                u => new Items
                                {
                                    ID = u.ItemID,
                                    ItemDesc = u.ItemName

                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            city.Insert(0, new Items
            {
                ID = 0,
                ItemDesc = "All"
            });
            return city;
        }

        public static List<SubCategoryA> GetSubCatAList(int intRegionID, int RegionID1)
        {
            List<SubCategoryA> city = new List<SubCategoryA>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SubCtegoryAs.Where(c=>c.MainCategID == intRegionID && c.SubCategID == RegionID1 && c.IsDeleted == false).OrderBy(c => c.SubCategADesc)
                            .Select(
                                u => new SubCategoryA
                                {
                                    SubCategoryAID = u.SubCategIDA,
                                    SubCategoryAName = u.SubCategADesc

                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }



        public static List<SubCategoryA> GetSubCatAList(int intRegionID)
        {
            List<SubCategoryA> city = new List<SubCategoryA>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.SubCtegoryAs.Where(c => c.SubCategID == intRegionID && c.IsDeleted == false).OrderBy(c => c.SubCategADesc)
                            .Select(
                                u => new SubCategoryA
                                {
                                    ID = u.SubCategIDA,
                                    SubCategoryAName = u.SubCategADesc

                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }




        // Get Cities List By RegionID ...
        public static List<CityData> LoadCitiesListRelatedToRegionalHead(int CityID)
        {
            List<CityData> city = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    //var SO = dbContext.SaleOfficers.Where(s => s.ID == SalesOfficerID && s.IsDeleted == false).Select(r => r.RegionalHeadID).FirstOrDefault();
                   // List<int> Regions = dbContext.RegionalHeadRegions.Where(r => r.RegionHeadID == SO).Select(s => s.RegionID).ToList();
                    city = dbContext.Cities.Where(c => c.RegionID==CityID).Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).ToList(); ;

                }
            }
            catch (Exception)
            {
                throw;
            }

            return city;
        }


        // Insert OR Update City ...
        public static int AddUpdateCity(CityData obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        City CityObj = new City();

                        if (obj.ID == 0)
                        {
                            CityObj.ID = dbContext.Cities.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            CityObj.Name = obj.Name;
                            CityObj.RegionID = obj.RegionID;
                            CityObj.ShortCode = obj.ShortCode;
                            CityObj.IsActive = true;
                            CityObj.CreatedDate = DateTime.Now;
                            CityObj.LastUpdate = DateTime.Now;
                            CityObj.CreatedBy = 1;

                            dbContext.Cities.Add(CityObj);
                        }

                        Area AreaObj = new Area();

                        if (obj.ID == 0)
                        {
                            AreaObj.ID = dbContext.Areas.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                            AreaObj.Name = obj.Name;
                            AreaObj.RegionID = obj.RegionID;
                            AreaObj.CityID = CityObj.ID;
                            AreaObj.IsActive = true;
                            AreaObj.CreatedDate = DateTime.Now;
                            AreaObj.LastUpdate = DateTime.Now;
                            AreaObj.CreatedBy = 1;

                            dbContext.Areas.Add(AreaObj);
                        }


                        else
                        {
                            CityObj = dbContext.Cities.Where(u => u.ID == obj.ID).FirstOrDefault();
                            CityObj.Name = obj.Name;
                            CityObj.RegionID = obj.RegionID;
                            CityObj.ShortCode = obj.ShortCode;
                            CityObj.LastUpdate = DateTime.Now;
                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add City Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code City"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        public static int AddKPIS(CityData obj)
        {
            int Res = 0;

            try
            {
                if (obj.KpiRows == null || obj.KpiRows.Count == 0)
                    return 0;

                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        try
                        {
                            var soIds = obj.KpiRows.Select(r => r.SOID).Distinct().ToList();

                            var existingRows = dbContext.Tbl_KPITargetsRegionWise
                                .Where(x => x.FinancialYearID == obj.FinancialYearID
                                         && x.IsActive == true
                                         && x.SOID.HasValue
                                         && soIds.Contains(x.SOID.Value))
                                .ToList();
                            foreach (var row in existingRows)
                            {
                                row.IsActive = false;
                            }

                            var nowPkt = DateTime.UtcNow.AddHours(5);
                            foreach (var r in obj.KpiRows)
                            {
                                dbContext.Tbl_KPITargetsRegionWise.Add(new Tbl_KPITargetsRegionWise
                                {
                                    SOID = r.SOID,
                                    RegionalHeadID = obj.RegionalHeadID,
                                    FinancialYearID = obj.FinancialYearID,
                                    // UI label -> DB column mapping:
                                    // Platinum -> CoatingTarget, Premium -> ACTDTarget, Gold -> ReadyMixTarget
                                    CoatingTarget = r.Platinum,
                                    ACTDTarget = r.Premium,
                                    ReadyMixTarget = r.Gold,
                                    TotalTarget = r.Total,
                                    IsActive = true,
                                    CreatedOn = nowPkt
                                });
                            }

                            dbContext.SaveChanges();
                            Res = 1;
                            scope.Complete();
                        }
                        catch (Exception)
                        {
                            Res = 0;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add City Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code City"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }

        // Delete City ...
        public static int DeleteCity(int CityID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    City obj = dbContext.Cities.Where(u => u.ID == CityID).FirstOrDefault();
                    dbContext.Cities.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete City Failed");
                Resp = 1;
            }
            return Resp;
        }

        //GET Edit One City
        public static CityData GetEditCity(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Cities.Where(i => i.ID == CityID && i.IsActive == true && i.IsDeleted == false).Select(i => new CityData
                    {
                        ID = i.ID,
                        Name = i.Name,
                        RegionID = i.RegionID,
                        RegionName = i.Region.Name,
                        ShortCode = i.ShortCode,
                        LastUpdate = i.LastUpdate,
                        IsDeleted = i.IsDeleted,
                        IsActive = i.IsActive,
                        CreatedBy = i.CreatedBy
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Get All Cities For Grid ...
        public static List<CityData> GetCityForGrid(int intRegionID)
        {
            List<CityData> cityData = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityData = dbContext.Cities.Where(u => u.RegionID == intRegionID && u.IsDeleted == false)
                            .ToList().Select(
                                u => new CityData
                                {
                                    ID = u.ID,
                                    RegionID = u.RegionID,
                                    Name = u.Name,
                                    RegionName = u.Region.Name,
                                    ShortCode = u.ShortCode,
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load City Grid Failed");
                throw;
            }

            return cityData;
        }

        public static List<CityData> GetKPIForGrid(int intSOID, int intFinancialYearID, int intRegionalHeadID)
        {
            List<CityData> cityData = new List<CityData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var q = dbContext.Tbl_KPITargetsRegionWise.Where(u => u.IsActive == true);

                    if (intSOID > 0)
                        q = q.Where(u => u.SOID == intSOID);
                    if (intFinancialYearID > 0)
                        q = q.Where(u => u.FinancialYearID == intFinancialYearID);
                    if (intRegionalHeadID > 0)
                        q = q.Where(u => u.RegionalHeadID == intRegionalHeadID);

                    var rows = q.ToList();

                    var soIds = rows.Where(r => r.SOID.HasValue).Select(r => r.SOID.Value).Distinct().ToList();
                    var rhIds = rows.Where(r => r.RegionalHeadID.HasValue).Select(r => r.RegionalHeadID.Value).Distinct().ToList();
                    var fyIds = rows.Where(r => r.FinancialYearID.HasValue).Select(r => r.FinancialYearID.Value).Distinct().ToList();

                    var soDict = dbContext.SaleOfficers.Where(x => soIds.Contains(x.ID))
                                    .ToDictionary(x => x.ID, x => x.Name);
                    var rhDict = dbContext.RegionalHeads.Where(x => rhIds.Contains(x.ID))
                                    .ToDictionary(x => x.ID, x => x.Name);
                    var fyDict = dbContext.Database
                                    .SqlQuery<FinancialYearListItem>("SELECT ID, [Year] FROM dbo.Tbl_FinancialYear WHERE ID IN (" + (fyIds.Count == 0 ? "0" : string.Join(",", fyIds)) + ")")
                                    .ToDictionary(x => x.ID, x => x.Year);

                    cityData = rows.Select(u => new CityData
                    {
                        ID = u.ID,
                        Name = "Name",
                        SOID = u.SOID,
                        SOName = u.SOID.HasValue && soDict.ContainsKey(u.SOID.Value) ? soDict[u.SOID.Value] : "",
                        RegionalHeadID = u.RegionalHeadID,
                        RegionalHeadName = u.RegionalHeadID.HasValue && rhDict.ContainsKey(u.RegionalHeadID.Value) ? rhDict[u.RegionalHeadID.Value] : "",
                        FinancialYearID = u.FinancialYearID,
                        FinancialYearName = u.FinancialYearID.HasValue && fyDict.ContainsKey(u.FinancialYearID.Value) ? fyDict[u.FinancialYearID.Value] : "",
                        TotalTarget = u.TotalTarget,
                        ACTDTarget = u.ACTDTarget,
                        CoatingTarget = u.CoatingTarget,
                        ReadyMixTarget = u.ReadyMixTarget,
                        LastUpdate = u.CreatedOn
                    }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load KPI Grid Failed");
                throw;
            }

            return cityData;
        }

        public static List<MainCategories> GetMainCategoryForGrid()
        {
            List<MainCategories> cityData = new List<MainCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityData = dbContext.MainCategories.Where( u=> u.IsDeleted == false)
                            .ToList().Select(
                                u => new MainCategories
                                {
                                    ID = u.MainCategID,
                                    MainCategoryName = u.MainCategDesc
                                   
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load MainCategory Grid Failed");
                throw;
            }

            return cityData;
        }

        public static List<MainCategories> GetProductCategoryForGrid()
        {
            List<MainCategories> cityData = new List<MainCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    cityData = dbContext.Tbl_ProuctCategory.Where(u => u.IsActive == true)
                            .ToList().Select(
                                u => new MainCategories
                                {
                                    ID = u.ID,
                                    MainCategoryName = u.Name

                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Load MainCategory Grid Failed");
                throw;
            }

            return cityData;
        }


        public static int AddUpdateMainCategory(MainCategories obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        MainCategory CityObj = new MainCategory();

                        if (obj.ID == 0)
                        {
                            CityObj.MainCategID = dbContext.MainCategories.OrderByDescending(u => u.MainCategID).Select(u => u.MainCategID).FirstOrDefault() + 1;
                            CityObj.MainCategDesc = obj.MainCategoryName;
                            CityObj.IsActive = true;
                            CityObj.IsDeleted = false;
                            CityObj.IsActive = true;
                            CityObj.CreatedOn = DateTime.Now;
                            CityObj.UpdatedOn = DateTime.Now;
                            CityObj.CreatedBy = 1;
                            CityObj.UpdatedBy = 1;

                            dbContext.MainCategories.Add(CityObj);
                        }
                        else
                        {
                            CityObj = dbContext.MainCategories.Where(u => u.MainCategID == obj.ID).FirstOrDefault();
                            CityObj.MainCategDesc = obj.MainCategoryName;
                            CityObj.IsActive = true;
                            CityObj.IsDeleted = false;
                            CityObj.IsActive = true;
                            CityObj.CreatedOn = DateTime.Now;
                            CityObj.UpdatedOn = DateTime.Now;
                            CityObj.CreatedBy = 1;
                            CityObj.UpdatedBy = 1;

                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add MainCategory Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code City"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        public static int AddUpdateProductCategory(MainCategories obj)
        {
            int Res = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (FOSDataModel dbContext = new FOSDataModel())
                    {
                        Tbl_ProuctCategory CityObj = new Tbl_ProuctCategory();

                        if (obj.ID == 0)
                        {
                            //CityObj.MainCategID = dbContext.MainCategories.OrderByDescending(u => u.MainCategID).Select(u => u.MainCategID).FirstOrDefault() + 1;
                            CityObj.Name = obj.MainCategoryName;
                            CityObj.IsActive = true;
                      
                            CityObj.CreatedOn = DateTime.Now;
                          

                            dbContext.Tbl_ProuctCategory.Add(CityObj);
                        }
                        else
                        {
                            CityObj = dbContext.Tbl_ProuctCategory.Where(u => u.ID == obj.ID).FirstOrDefault();
                            CityObj.Name = obj.MainCategoryName;
                            CityObj.IsActive = true;
                            
                            CityObj.CreatedOn = DateTime.Now;
                          

                        }

                        dbContext.SaveChanges();
                        Res = 1;
                        scope.Complete();
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add MainCategory Failed");
                Res = 0;
                if (exp.InnerException.InnerException.Message.Contains("Short Code City"))
                {
                    // Res = 2 Is For Unique Constraint Error...
                    Res = 2;
                    return Res;
                }
                return Res;
            }
            return Res;
        }


        public static MainCategories GetEditMAinCategory(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.MainCategories.Where(i => i.MainCategID == CityID && i.IsActive == true && i.IsDeleted == false).Select(i => new MainCategories
                    {
                        ID = i.MainCategID,
                        MainCategoryName = i.MainCategDesc,
                     
                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static MainCategories GetEditProductCategory(int CityID)
        {
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    return dbContext.Tbl_ProuctCategory.Where(i => i.ID == CityID && i.IsActive == true).Select(i => new MainCategories
                    {
                        ID = i.ID,
                        MainCategoryName = i.Name,

                    }).First();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static int DeleteMAinCategory(int CityID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    MainCategory obj = dbContext.MainCategories.Where(u => u.MainCategID == CityID).FirstOrDefault();
                    dbContext.MainCategories.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete MainCategory Failed");
                Resp = 1;
            }
            return Resp;
        }

        public static int DeleteProductCategory(int CityID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Tbl_ProuctCategory obj = dbContext.Tbl_ProuctCategory.Where(u => u.ID == CityID).FirstOrDefault();
                    dbContext.Tbl_ProuctCategory.Remove(obj);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Delete MainCategory Failed");
                Resp = 1;
            }
            return Resp;
        }



        public static List<CityData> GetResult(string search, string sortOrder, int start, int length, List<CityData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count(string search, List<CityData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }


        public static List<MainCategories> GetResult1(string search, string sortOrder, int start, int length, List<MainCategories> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }


        public static int Count1(string search, List<MainCategories> dtResult, List<string> columnFilters)
        {
            return FilterResult1(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<CityData> FilterResult(string search, List<CityData> dtResult, List<string> columnFilters)
        {
            IQueryable<CityData> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionName != null && p.RegionName.ToLower().Contains(search.ToLower()) || p.ShortCode != null && p.ShortCode.ToLower().Contains(search.ToLower())))
                && (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[5].ToLower())))
                );

            return results;
        }

        private static IQueryable<MainCategories> FilterResult1(string search, List<MainCategories> dtResult, List<string> columnFilters)
        {
            IQueryable<MainCategories> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.MainCategoryName != null && p.MainCategoryName.ToLower().Contains(search.ToLower()) )
                //&& (columnFilters[3] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
                //&& (columnFilters[4] == null || (p.RegionName != null && p.RegionName.ToLower().Contains(columnFilters[4].ToLower())))
                //&& (columnFilters[5] == null || (p.ShortCode != null && p.ShortCode.ToLower().Contains(columnFilters[5].ToLower())))
                ));

            return results;
        }


        public static List<MainCategories> GetMainCatList()
        {
            List<MainCategories> city = new List<MainCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.MainCategories.Where(c =>  c.IsDeleted == false).OrderBy(c => c.MainCategDesc)
                            .Select(
                                u => new MainCategories
                                {
                                    ID = u.MainCategID,
                                    MainCategoryName = u.MainCategDesc
                                   
                                }).ToList();

                    return city;
                }
            }
            catch (Exception)
            {
                throw;
            }

            
        }

        public static List<MainCategories> GetProductMainCatList()
        {
            List<MainCategories> city = new List<MainCategories>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    city = dbContext.Tbl_ProuctCategory.Where(c => c.IsActive == true).OrderBy(c => c.Name)
                            .Select(
                                u => new MainCategories
                                {
                                    ID = u.ID,
                                    MainCategoryName = u.Name

                                }).ToList();

                    return city;
                }
            }
            catch (Exception)
            {
                throw;
            }


        }







    }
}