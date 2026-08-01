using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class CommonController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult GetCitiesList(int regionalHeadId) // now getting DEALERID 
        {
            try
            {
                //var dealerId = regionalHeadId; // now r getting cities on basis of DealerID
                //if (dealerId > 0)
                //{
                //    var retailerCityIds = ManageRetailer.GetDealerRetailerCityIdsList(regionalHeadId);
                //    var cities = ManageCity.GetCityListByDealerRetailerCityIds(retailerCityIds);
                //    if (cities != null && cities.Count > 0)
                //    {
                //        return Ok(new
                //        {
                //            Cities = cities.Where(s => s.IsActive).Select(d => new
                //            {
                //                d.ID,
                //                d.Name
                //            }).OrderBy(d => d.Name)
                //        });
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get Cities List API Failed");
            }

            return Ok(new
            {
                Cities = new { }
            });
        }

        public IHttpActionResult GetAreasList(int cityId)
        {
            try
            {
                if (cityId > 0)
                {
                    var areas = ManageArea.GetAreaListByCityID(cityId);
                    if (areas != null && areas.Count > 0)
                    {
                        return Ok(new
                        {
                            Areas = areas.Where(s => s.IsActive).Select(d => new
                            {
                                d.ID,
                                d.Name
                            }).OrderBy(d => d.Name)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get Areas List API Failed");
            }

            return Ok(new
            {
                Areas = new { }
            });
        }


        public IHttpActionResult GetZonesList(int cityId)
        {
            try
            {
                if (cityId > 0)
                {
                    var zones = ManageZone.GetZonesByCityID(cityId);
                    if (zones != null && zones.Count > 0)
                    {
                        return Ok(new
                        {
                            Zones = zones.Select(d => new
                            {
                                d.ID,
                                d.Name
                            }).OrderBy(d => d.Name)
                        });
                    }
                }
                else
                {
                    var zones = ManageZone.GetZoneList();
                    if (zones != null && zones.Count > 0)
                    {
                        return Ok(new
                        {
                            Zones = zones.Select(d => new
                            {
                                d.ID,
                                d.Name
                            }).OrderBy(d => d.Name)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get Zones by CITYID List API Failed");
            }

            return Ok(new
            {
                Zones = new { }
            });
        }


        [Route("api/FeesStruct")]
        [HttpGet]
        public List<FeeStructure> FeesStruct()
        {
            List<FeeStructure> cities = new List<FeeStructure>();
            FeeStructure fee;

            var dbFees = db.FeeStructures.Where(c => c.IsActive).ToList();

            foreach (var dbCty in dbFees)
            {
                fee = new FeeStructure();
                fee.FeeStructID = dbCty.FeeStructID;
                fee.FeeStructName = dbCty.FeeStructName;

                cities.Add(fee);
            }

            return cities;
        }


        [Route("api/GetCities")]
        [HttpGet]
        public List<Regions> GetCities(int? ID)
        {
            List<Regions> cities = new List<Regions>();
            //Regions cty;

            //var dbCities = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();


            var dbregions = (from i in db.RegionalHeadRegions
                             join re in db.Regions on i.RegionID equals re.ID
                             where (i.RegionHeadID == ID)
                             select new Regions
                             {
                                 ID = re.ID,
                                 Name = re.Name
                             }).ToList();

            
           // var dbRegions=db.Regions.Where(x=>dbCities.Contains())

            //foreach (var dbCty in dbCities)
            //{
            //    cty = new Regions();
            //    cty.ID = dbCty.ID;
            //    cty.Name = dbCty.Name;

            //    cities.Add(cty);
            //}

            return dbregions;
        }


        [Route("api/SyllabusOffered")]
        [HttpGet]
        public List<Customers> SyllabusOffered()
        {
            List<Customers> Syllabus = new List<Customers>();
            Customers cty;

            var dbCities = db.Retailers.Where(c => c.IsActive).ToList();

            foreach (var dbCty in dbCities)
            {
                cty = new Customers();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                Syllabus.Add(cty);
            }

            return Syllabus;
        }


        [Route("api/CustomersRrelatedToSoForCheckin/{Id}")]
        [HttpGet]
        public List<CustomersForCheckin> CustomersRrelatedToSoForCheckin(int Id)
        {
            List<CustomersForCheckin> CustomerValidate = new List<CustomersForCheckin>();
            CustomersForCheckin cty;

            var dbCities = db.Retailers.Where(c => c.SaleOfficerID == Id && c.IsActive == true).ToList();

            foreach (var dbCty in dbCities)
            {
                cty = new CustomersForCheckin();
                cty.ID = dbCty.ID;
                cty.ShopName = dbCty.ShopName;
                cty.ISActive = dbCty.IsActive;

                CustomerValidate.Add(cty);
            }

            return CustomerValidate;
        }


        [Route("api/CustomersRrelatedToSoForCheckin/{Id}")]
        [HttpGet]
        public List<CustomersForCheckin> CustomersRrelatedRegionIDForCheckin(int Id, int RangeID,int CityID)
        {
            List<CustomersForCheckin> CustomerValidate = new List<CustomersForCheckin>();
            CustomersForCheckin cty;


            var rangeid = db.SaleOfficers.Where(x => x.ID == RangeID).Select(x => x.RangeID).FirstOrDefault();

            var dbCities = db.Retailers.Where(c => c.RegionID == Id && c.CityID==CityID && c.IsActive == true && c.RangeID== rangeid).ToList();

            foreach (var dbCty in dbCities)
            {
                cty = new CustomersForCheckin();
                cty.ID = dbCty.ID;
                cty.ShopName = dbCty.ID + "/" + " " + dbCty.ShopName;
                cty.ISActive = dbCty.IsActive;

                CustomerValidate.Add(cty);
            }

            return CustomerValidate;
        }


        public List<CustomersForCheckin> DistributorRrelatedToSoForCheckin(int Id)
        {
            List<CustomersForCheckin> CustomerValidate = new List<CustomersForCheckin>();
            CustomersForCheckin cty;

            var dbCities = db.Dealers.Where(c => c.SaleOfficerID == Id && c.IsActive == true).ToList();

            foreach (var dbCty in dbCities)
            {
                cty = new CustomersForCheckin();
                cty.ID = dbCty.ID;
                cty.ShopName = dbCty.ShopName;
                cty.ISActive = dbCty.IsActive;

                CustomerValidate.Add(cty);
            }

            return CustomerValidate;
        }





        [Route("api/GetMainCategory")]
        [HttpGet]
        public List<MainCategories> MainCat(int RangeID)
        {
            List<MainCategories> MAinCat = new List<MainCategories>();
            MainCategories cty;
            if (RangeID != 0)
            {
                var dbMainCat = db.MainCategories.Where(c => c.IsActive == true).ToList();

                foreach (var dbCty in dbMainCat)
                {
                    cty = new MainCategories();
                    cty.MainCategID = dbCty.MainCategID;
                    cty.MainCategDesc = dbCty.MainCategDesc;

                    MAinCat.Add(cty);
                }
            }
            else
            {
                var dbMainCat = db.MainCategories.Where(c => c.IsActive == true).ToList();

                foreach (var dbCty in dbMainCat)
                {
                    cty = new MainCategories();
                    cty.MainCategID = dbCty.MainCategID;
                    cty.MainCategDesc = dbCty.MainCategDesc;

                    MAinCat.Add(cty);
                }
            }
            return MAinCat;
        }



        public List<RetailerType> RetailerType()
        {
            List<RetailerType> MAinCat = new List<RetailerType>();
            RetailerType cty;

            var dbMainCat = db.Tbl_RetailerClass.Where(c => c.Status == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new RetailerType();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.RetailerClass;

                MAinCat.Add(cty);
            }

            return MAinCat;
        }




        public List<RetailerType> RetailerType1()
        {
            List<RetailerType> MAinCat = new List<RetailerType>();
            RetailerType cty;

            var dbMainCat = db.Tbl_RetailerType.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new RetailerType();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.RetailerType;

                MAinCat.Add(cty);
            }

            return MAinCat;
        }


        public List<RetailerType> Soregions(int? ID)
        {
            List<RetailerType> MAinCat = new List<RetailerType>();
            RetailerType cty;

            var dbMainCat = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new RetailerType();

                var name = db.Regions.Where(x => x.ID == dbCty.RegionID).FirstOrDefault();
                cty.ID = name.ID;
                cty.Name = name.Name;

                MAinCat.Add(cty);
            }

            return MAinCat;
        }

        public List<RetailerType> SoCities(int? ID)
        {
            List<RetailerType> MAinCat = new List<RetailerType>();
            RetailerType cty;

            var dbMainCat = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).Select(c=>c.RegionID).FirstOrDefault();

            var cities = db.Cities.Where(x => x.RegionID == dbMainCat).Select(x => new RetailerType
            {
                ID=x.ID,
                Name=x.Name

            }).ToList();

            return cities;
        }

        public List<RetailerType> SoDistributor(int? ID)
        {
            List<RetailerType> MAinCat = new List<RetailerType>();
            RetailerType cty;

            var dbMainCat = db.RegionalHeadRegions.Where(c => c.RegionHeadID == ID).Select(c => c.RegionID).FirstOrDefault();

            var cities = db.Dealers.Where(x => x.RegionID == dbMainCat).Select(x => new RetailerType
            {
                ID = x.ID,
                Name = x.ShopName

            }).ToList();

            return cities;
        }
        public List<RetailerType> Reporttype()
        {
            List<RetailerType> MAinCat = new List<RetailerType>();
            RetailerType cty;

            var dbMainCat = db.ReportTypes.ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new RetailerType();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }

            return MAinCat;
        }

        public List<AllSaleOfficers> SalesOfficersNames(int saleofficerID)
        {  
            List<AllSaleOfficers> MAinCat = new List<AllSaleOfficers>();
            AllSaleOfficers cty;
            List<AllSaleOfficers> list;


            if (saleofficerID > 0)
            {
                //string SOName = "";
                var dbMainCat = db.Tbl_Access.Where(c => c.RepotedUP == saleofficerID && c.Status == true).Select(c => new
                {
                    id = c.ReportedDown
                });

                foreach (var dbCty in dbMainCat)
                {
                    cty = new AllSaleOfficers();
                    cty.ID = dbCty.id;
                    cty.Name = db.SaleOfficers.Where(x => x.ID == dbCty.id && x.IsActive==true).Select(x => x.Name).FirstOrDefault();

                    MAinCat.Add(cty);
                }
             
               
            }
            else
            {

            }


            return MAinCat.OrderBy(x=>x.Name).ToList();
        }


        public List<City> FollowUp()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_FollowupReasons.Where(c => c.Status == true).ToList();

                foreach (var dbCty in dbMainCat)
                {
                    cty = new City();
                    cty.ID = dbCty.ID;
                   cty.Name = dbCty.Name;

                    MAinCat.Add(cty);
                }


           


            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> ProductNature()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_ProductNature.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> ConditionOfSubstrate()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_ConditionOfSubstratee.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> ThinningSolvent()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_ThinningSolvent.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> AppliedTool()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_AppliedTool.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> NatureOfComplaint()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_NatureOfComplaint.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> BrightoUAEcategory()
        {
            return db.Tbl_ProductDetail
                .Where(c => c.IsActive == true)
                .Select(c => c.Product_Desc)
                .Distinct()
                .OrderBy(name => name)
                .AsEnumerable()  // Switch to LINQ to Objects
                .Select((name, index) => new City
                {
                    ID = index + 1,
                    Name = name
                })
                .ToList();
        }
        public List<City> ComplaintStatusList()
        {
            List<City> complaintStatuses = new List<City>();

            // Hardcoded complaint status values
            complaintStatuses.Add(new City { ID = 1, Name = "New Complaint" });

            complaintStatuses.Add(new City { ID = 2, Name = "Complaint In Progress" });
            complaintStatuses.Add(new City { ID = 3, Name = "Complaint Resolved" });
            complaintStatuses.Add(new City { ID = 4, Name = "Decesion Pending" });
            complaintStatuses.Add(new City { ID = 5, Name = "Commercial Decesion" });
            return complaintStatuses.OrderBy(x => x.ID).ToList();
        }

        public List<City> ProgressStatusList()
        {
            List<City> complaintStatuses = new List<City>();

            complaintStatuses.Add(new City { ID = 1, Name = "Complaint Noted" });
            complaintStatuses.Add(new City { ID = 2, Name = "Complaint Forwarded to R&D" });
            complaintStatuses.Add(new City { ID = 3, Name = "Complaint Justified" });
            complaintStatuses.Add(new City { ID = 4, Name = "Complaint Not Justified" });
            complaintStatuses.Add(new City { ID = 5, Name = "Material Arrangements in Progress" });
            complaintStatuses.Add(new City { ID = 6, Name = "Pending Due to Material/Shade Unavailable in stock" });
            complaintStatuses.Add(new City { ID = 7, Name = "Material available in warehouse and waiting for approval" });
            complaintStatuses.Add(new City { ID = 8, Name = "Material Replacement Completed" });
            complaintStatuses.Add(new City { ID = 9, Name = "Follow up in progress after Replacement" });
            complaintStatuses.Add(new City { ID = 10, Name = "Complaint Closed" });

            return complaintStatuses.OrderBy(x => x.Name).ToList();
        }

        public List<ProductDetails> ProductInfo()
        {
            List<ProductDetails> MAinCat = new List<ProductDetails>();
            ProductDetails cty;
            List<ProductDetails> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_ProductDetail.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new ProductDetails();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Product_Desc;
                cty.ProductDesc = dbCty.Product_Desc;
                cty.Qtrprice = dbCty.Qtr_Price;
                cty.QtrUom = dbCty.Qtr_UoM;
                cty.Gallonprice = dbCty.Gallon_Price;
                cty.GallonUom = dbCty.Gallon_UoM;
                cty.IncentivePkr = dbCty.Incentive_PKR;
                cty.Drumprice = dbCty.Drum_Price;
                cty.DrumUom = dbCty.Drum_UoM;
                cty.Sortorder = dbCty.Sortorder;


                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Sortorder).ToList();
        }
        public List<City> PurposeOfVisits()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_PurposeOfVisit.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> NoSale()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_NoSaleReason.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> RSMDoc()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_ChecklistBeforeVisit.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> ComBrands()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.TBL_CompititorBrandsForDropDown.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.ID).ToList();
        }


        public List<MMCItems> MMCItemsList()
        {
            List<MMCItems> MAinCat = new List<MMCItems>();
            MMCItems cty;
            List<MMCItems> list;



            //string SOName = "";
            var dbMainCat = db.Items.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new MMCItems();
                cty.ID = dbCty.ItemID;
                cty.Name = dbCty.ItemName;
                cty.Packing = dbCty.Packing;
                cty.Price = dbCty.Price;
                cty.SortOn = dbCty.SortOrder;
                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.SortOn).ToList();
        }

        public List<City> SegmenttypeList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_Segmenttype.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;
              
                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> BusinesstypeList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.BusinessTypes.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> PlotSizeList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_PlotSize.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> IndustryList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_Industry.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> ScopeOfWorkList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_ScopeOfWork.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> SiteStatusList()
        {
            List<City> MAinCat = new List<City>
    {
        new City { ID = 1, Name = "Active" },
        new City { ID = 2, Name = "Inactive" },
        new City { ID = 3, Name = "Closed" }
    };

            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> AppTypeList()
        {
            List<City> MAinCat = new List<City>
    {
        new City { ID = 1, Name = "TopCoat" },
        new City { ID = 2, Name = "Primer" },
        new City { ID = 3, Name = "Filler" }
    };

            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<ReportTypes> ReportsList()
        {
            List<ReportTypes> MAinCat = new List<ReportTypes>
    {
        new ReportTypes { ID = 1, Name = "Detail Report-Trader Wise",SortOrder=1 },
        new ReportTypes { ID = 2, Name = "Summary Report-Trader Wise",SortOrder=2 },
        new ReportTypes { ID = 3, Name = "Summary Report-Brand Wise",SortOrder=3 },
         new ReportTypes { ID = 4, Name = "Summary Report-Category Wise",SortOrder=4 },
        new ReportTypes { ID = 5, Name = "Detail Report-Value Wise",SortOrder=5 },
         new ReportTypes { ID = 6, Name = "Summary Report-value Wise" , SortOrder = 6},
    };

            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> SiteStatusVerificationList()
        {
            List<City> MAinCat = new List<City>
    {
        new City { ID = 1, Name = "Under Construction" },
        new City { ID = 2, Name = "Grey Structure" },
        new City { ID = 3, Name = "Ready To Paint" },
        new City { ID = 4, Name = "Site Won" }
    };

            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> AffiliatePurposeOfVisitList()
        {
            List<City> MAinCat = new List<City>
    {
        new City { ID = 1, Name = "PR Building" },
        new City { ID = 2, Name = "Site Specific Visit" },
        new City { ID = 3, Name = "Complaint Visit" },
        new City { ID = 4, Name = "Give Away Meeting" }
    };

            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> AffiliateTargetAgreementList()
        {
            List<City> MAinCat = new List<City>
    {
        new City { ID = 1, Name = "Liters" },
        new City { ID = 2, Name = "Value" },
        new City { ID = 3, Name = "Gallons" }
       
    };

            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> SOVisitsList()
        {
            List<City> MAinCat = new List<City>
    {
        new City { ID = 1, Name = "One" },
        new City { ID = 2, Name = "Two" },
        new City { ID = 3, Name = "Three" },
        new City { ID = 4, Name = "Four" }
    };

            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> PlottypeList()
        {
            List<City> MAinCat = new List<City>
    {
        new City { ID = 1, Name = "Residential" },
        new City { ID = 2, Name = "Commercial" }
       
    };

            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> SiteInfoList()
        {
            List<City> MAinCat = new List<City>
    {
       new City { ID = 1, Name = "Complete" },
        new City { ID = 2, Name = "Incomplete" },
        new City { ID = 3, Name = "Incorrect" },
        new City { ID = 4, Name = "NA" }

    };

            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> NatureOfWorkList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_NatureOfWork.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> ChemProductList()
        {
            List<City> MAinCat = new List<City>();
            City cty;

            // Get distinct ProductDesc values only
            var distinctProducts = db.Tbl_BrightoChemicalProducts
                                     .Where(c => c.IsActive == true)
                                     .Select(c => c.ProductDesc)
                                     .Distinct()
                                     .OrderBy(p => p)
                                     .ToList();

            int autoId = 1;
            foreach (var productDesc in distinctProducts)
            {
                cty = new City();
                cty.ID = autoId++;               // Auto-generated ID
                cty.Name = productDesc;          // Distinct product name

                MAinCat.Add(cty);
            }

            return MAinCat;
        }


        public List<ShadeCards> ShadeCardsList()
        {
            List<ShadeCards> MAinCat = new List<ShadeCards>();
            ShadeCards cty;
            List<ShadeCards> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_SharedCards.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new ShadeCards();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.URL;
                cty.Heading = dbCty.Heading;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<CompititorList> BrightoCompititorsList()
        {
            List<CompititorList> MAinCat = new List<CompititorList>();
            CompititorList cty;
            List<CompititorList> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_CompititorList.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new CompititorList();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;
                cty.SortOrder = dbCty.sortOrder;
                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.SortOrder).ToList();
        }

        public List<City> CompetitorActivityTypesList()
        {
            var list = new List<City>();
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(db.Database.Connection.ConnectionString))
                using (var cmd = new System.Data.SqlClient.SqlCommand(
                    "SELECT ID, Name FROM dbo.Tbl_CompetitorActivityTypes WHERE IsActive = 1 ORDER BY Name", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(new City { ID = Convert.ToInt32(reader["ID"]), Name = reader["Name"] as string });
                    }
                }
            }
            catch (Exception ex) {Log.Instance.Error(ex, "CompetitorActivityTypesList Failed"); }
            return list;
        }

        public List<City> ConstructionStageList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_ConstructionStage.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }


        public List<City> ChemBusinessLineList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var businessLines = db.Tbl_ChemCustomerInfo
                       .Where(c => c.IsActive == true && c.BusinessLine != null)
                       .Select(c => c.BusinessLine)
                       .Distinct()
                       .ToList();

            int idCounter = 1;
            foreach (var line in businessLines)
            {
                 cty = new City();
                cty.ID = idCounter++;  
                cty.Name = line;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> AffBusinessTypesList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_AffiliatesBusinessTypes.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> AffBusinessClassificationList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_AffiliatesClassification.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> AffExpertiseList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_AffiliatesExpertise.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> AffNatureOfClientList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_NatureOfClient.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<City> BusinessStatusList()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.BusinessStatus.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }

        public List<City> SOVisittypes()
        {
            List<City> MAinCat = new List<City>();
            City cty;
            List<City> list;



            //string SOName = "";
            var dbMainCat = db.Tbl_VisittypesOfSO.Where(c => c.IsActive == true).ToList();

            foreach (var dbCty in dbMainCat)
            {
                cty = new City();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }





            return MAinCat.OrderBy(x => x.Name).ToList();
        }
        public List<AllSaleOfficers> SalesOfficers(int? RegionalHeadID, int saleofficerID)
        {
            List<AllSaleOfficers> MAinCat = new List<AllSaleOfficers>();
            AllSaleOfficers cty;


            //if (RegionalHeadID == 5)
            //{
            //    if (saleofficerID == 23)
            //    {
            //        var dbMainCat = db.SaleOfficers.Where(c => c.IsActive == true).ToList();

            //        foreach (var dbCty in dbMainCat)
            //        {
            //            cty = new AllSaleOfficers();
            //            cty.ID = dbCty.ID;
            //            cty.Name = dbCty.Name;

            //            MAinCat.Add(cty);
            //        }
            //    }
            //    else
            //    {

            //    }

            //}

            if (RegionalHeadID == 4)
            {

                if (saleofficerID == 1)
                {
                    var dbMainCat2 = db.SaleOfficers.Where(c => c.RegionalHeadID == RegionalHeadID).ToList();

                    foreach (var dbCty in dbMainCat2)
                    {
                        cty = new AllSaleOfficers();
                        cty.ID = dbCty.ID;
                        cty.Name = dbCty.Name;

                        MAinCat.Add(cty);
                    }
                }
                else
                {
                    var dbMainCat2 = db.SaleOfficers.Where(c => c.ID == saleofficerID).ToList();

                    foreach (var dbCty in dbMainCat2)
                    {
                        cty = new AllSaleOfficers();
                        cty.ID = dbCty.ID;
                        cty.Name = dbCty.Name;

                        MAinCat.Add(cty);
                    }

                }
            }
            //else if (RegionalHeadID == 10)
            //{

            //    if (saleofficerID == 18)
            //    {
            //        var dbMainCat2 = db.SaleOfficers.Where(c => c.RegionalHeadID == RegionalHeadID).ToList();

            //        foreach (var dbCty in dbMainCat2)
            //        {
            //            cty = new AllSaleOfficers();
            //            cty.ID = dbCty.ID;
            //            cty.Name = dbCty.Name;

            //            MAinCat.Add(cty);
            //        }
            //    }
            //    else
            //    {
            //        var dbMainCat2 = db.SaleOfficers.Where(c => c.ID == saleofficerID).ToList();

            //        foreach (var dbCty in dbMainCat2)
            //        {
            //            cty = new AllSaleOfficers();
            //            cty.ID = dbCty.ID;
            //            cty.Name = dbCty.Name;

            //            MAinCat.Add(cty);
            //        }

            //    }
            //}
            //else
            //{
            //    var dbMainCat2 = db.SaleOfficers.Where(c => c.RegionalHeadID == RegionalHeadID).ToList();

            //    foreach (var dbCty in dbMainCat2)
            //    {
            //        cty = new AllSaleOfficers();
            //        cty.ID = dbCty.ID;
            //        cty.Name = dbCty.Name;

            //        MAinCat.Add(cty);
            //    }
            //}



            return MAinCat;
        }

        public List<AllSaleOfficers> AllSoForTakeoffForm(int? RegionalHeadID)
        {
            List<AllSaleOfficers> MAinCat = new List<AllSaleOfficers>();
            AllSaleOfficers cty;
            var MAinCatlist = db.SaleOfficers.Where(c => c.RegionalHeadID == RegionalHeadID).ToList();

            foreach (var dbCty in MAinCatlist)
            {
                cty = new AllSaleOfficers();
                cty.ID = dbCty.ID;
                cty.Name = dbCty.Name;

                MAinCat.Add(cty);
            }




            return MAinCat;
        }

    }
}