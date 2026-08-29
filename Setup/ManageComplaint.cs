using FOS.DataLayer;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class ManageComplaint
    {

        // Get SalesOfficer Area Names With Line Break ...
        public static string GetSaleOfficerAreaName(int intSaleOfficerID)
        {
            String strAreaName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == intSaleOfficerID).FirstOrDefault();
            strAreaName = string.Join("<br/>", objSaleOfficer.Areas.Select(p => p.Name));

            return strAreaName;
        }

        // Get SalesOfficer Area ID With Comma Seperator ...
        public static string GetSaleOfficerAreaID(int intSaleOfficerID)
        {
            String strAreaName = String.Empty;
            FOSDataModel dbContext = new FOSDataModel();
            var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == intSaleOfficerID).FirstOrDefault();
            strAreaName = string.Join(",", objSaleOfficer.Areas.Select(p => p.ID));

            return strAreaName;
        }

        // Get All Regions Related To SalesOfficer ...
        public static List<RegionData> GetRegionRelatedToSaleOfficer(int SaleOfficerID, int RegionalHeadID)
        {
            List<RegionData> strRegionID = new List<RegionData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    var objSaleOfficer = dbContext.SaleOfficers.Where(s => s.ID == SaleOfficerID && s.RegionalHeadID == RegionalHeadID).FirstOrDefault();
                    strRegionID = objSaleOfficer.RegionalHead.RegionalHeadRegions
                        .Where(r => r.RegionHeadID == RegionalHeadID)
                        .Select(u => new RegionData
                                {
                                    RegionID = u.RegionID,
                                    Name = u.Region.Name
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            

            return strRegionID;
        }


        // Get All SalesOfficer Related To CityID ...
        public static List<SaleOfficerData> GetSaleOfficerByRegionalHeadID(int RegionalHeadID)
        {
            List<SaleOfficerData> compData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    compData = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    RegionalHeadID = u.RegionalHeadID,
                                    Name = u.Name,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    CityName = u.City != null ? u.City.Name : "",
                                    CityID = u.CityID != null ? u.CityID : 0,
                                    AreaName = GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return compData;
        }

        // Get All Complaints List For Grid ...
        public static List<SaleOfficerData> GetSaleOfficerList()
        {
            List<SaleOfficerData> compData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    compData = dbContext.SaleOfficers.Where(u => u.IsDeleted == false).ToList()
                            .Select(
                                u => new SaleOfficerData
                                {
                                    ID = u.ID,
                                    RegionalHeadID = u.RegionalHeadID,
                                    Name = u.Name,
                                    UserName = u.UserName,
                                    Password = u.Password,
                                    RegionalHeadName = u.RegionalHead.Name,
                                    Phone1 = u.Phone1 == null ? "" : u.Phone1,
                                    Phone2 = u.Phone2 == null ? "" : u.Phone2,
                                    CityName = u.City != null ? u.City.Name : "",
                                    CityID = u.CityID != null ? u.CityID : 0,
                                    AreaName =  GetSaleOfficerAreaName(u.ID),
                                    AreaID = GetSaleOfficerAreaID(u.ID),
                                    LastUpdate = u.LastUpdate
                                }).ToList();
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get SalesOfficer List Failed");
                throw;
            }

            return compData;
        }

        // Get All Complaints List For Grid ...
        public static List<ComplaintData> GetComplaintListForGrid()
        {
            List<ComplaintData> compData = new List<ComplaintData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    compData = dbContext.Complaints.ToList()
                            .Select(
                                u => new ComplaintData
                                {
                                    ComplaintID = u.ComplaintID,
                                    RegionalHeadID = u.SaleOfficer.RegionalHeadID,
                                    RetailerId = u.RetailerId,
                                    SaleOfficerName = u.SaleOfficer.Name,
                                    SaleOfficerID = u.SaleOfficerId,
                                    Title = u.Title,
                                    Detail = u.Detail,
                                    RetailerName = u.Retailer.ShopName,
                                    Remarks = string.IsNullOrEmpty(u.Remarks) ? "" : u.Remarks,
                                    DueDateString = u.DueDate.ToString("dd-MMM-yyyy"),
                                    Priority = u.Priority.ToString(),
                                    PriorityName = u.Priority.ToString(),
                                    Status = u.Status.ToString(),
                                    StatusName = u.Status.ToString(),
                                    RemUpdatedOn = string.IsNullOrEmpty(u.Remarks) && !u.RemUpdatedOn.HasValue ? "" : "Added on " + u.RemUpdatedOn.Value.ToString("dd-MMM-yyyy HH:mm"),

                                }).ToList();

                    foreach (var item in compData)
                    {
                        if(int.Parse(item.PriorityName) == (int)PriorityEnum.Low)
                        {
                            item.PriorityName = PriorityEnum.Low.ToString();
                        }
                        else if (int.Parse(item.PriorityName) == (int)PriorityEnum.Medium)
                        {
                            item.PriorityName = PriorityEnum.Medium.ToString();
                        }
                        else if (int.Parse(item.PriorityName) == (int)PriorityEnum.High)
                        {
                            item.PriorityName = PriorityEnum.High.ToString();
                        }

                        if (int.Parse(item.StatusName) == (int)StatusEnum.Pending)
                        {
                            item.StatusName = StatusEnum.Pending.ToString();
                        }
                        else if (int.Parse(item.StatusName) == (int)StatusEnum.Completed)
                        {
                            item.StatusName = StatusEnum.Completed.ToString();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Get SalesOfficer List Failed");
                throw;
            }

            return compData;
        }

        //Get All SalesOfficer List Method...
        public static List<SaleOfficer> GetAllSaleOfficerList()
        {
            List<SaleOfficer> compData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                compData = dbContext.SaleOfficers.ToList();
            }
            return compData;
        }

        //Get All SalesOfficer List Method...
        public static List<SaleOfficer> GetAllSaleOfficerListRelatedtoregionalHeadID(int RHID)
        {
            List<SaleOfficer> compData = new List<SaleOfficer>();
            using (FOSDataModel dbContext = new FOSDataModel())
            {
                if (RHID == 0)
                {
                    compData = dbContext.SaleOfficers.ToList();
                }
                else
                {
                    compData = dbContext.SaleOfficers.Where(s => s.RegionalHeadID == RHID).ToList();
                }
                
            }
            return compData;
        }

        //Insert OR Update Complaints ...
        public static Boolean AddUpdateComplaints(ComplaintData obj)
        {
            Boolean boolFlag = false;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Complaint saleofficerObj = new Complaint();

                    if (obj.ComplaintID == 0)
                    {
                        //saleofficerObj.ComplaintID = dbContext.Complaints.OrderByDescending(u => u.ComplaintID).Select(u => u.ComplaintID).FirstOrDefault() + 1;
                        saleofficerObj.Title = obj.Title;
                        saleofficerObj.Detail = obj.Detail;
                        saleofficerObj.RetailerId = obj.RetailerId;
                        saleofficerObj.SaleOfficerId = obj.SaleOfficerID.Value;
                        //saleofficerObj.CityID = 1;
                        //saleofficerObj.IsActive = true;
                        //saleofficerObj.IsDeleted = false;
                        saleofficerObj.DueDate = DateTime.Parse(obj.DueDateString);
                        saleofficerObj.Priority = int.Parse(obj.Priority);
                        saleofficerObj.Status = int.Parse(obj.Status);

                        saleofficerObj.CreatedOn = DateTime.Now;
                        //saleofficerObj.UpdatedOn = DateTime.Now;
                        //Created By Work Pending...
                        saleofficerObj.CreatedBy = 1;

                        dbContext.Complaints.Add(saleofficerObj);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        saleofficerObj = dbContext.Complaints.Where(u => u.ComplaintID == obj.ComplaintID).FirstOrDefault();
                        if (saleofficerObj != null)
                        {
                            saleofficerObj.Title = obj.Title;
                            saleofficerObj.Detail = obj.Detail;
                            saleofficerObj.RetailerId = obj.RetailerId;
                            saleofficerObj.SaleOfficerId = obj.SaleOfficerID.Value;
                            saleofficerObj.DueDate = DateTime.Parse(obj.DueDateString);
                            saleofficerObj.Priority = int.Parse(obj.Priority);
                            saleofficerObj.Status = int.Parse(obj.Status);
                            saleofficerObj.UpdatedOn = DateTime.Now;
                            saleofficerObj.UpdatedBy = 1;
                        }
                        
                        dbContext.SaveChanges();
                    }

                    boolFlag = true;
                }
            }
            catch (Exception exp)
            {
                Log.Instance.Error(exp, "Add/update Complaint Failed");
                boolFlag = false;
            }
            return boolFlag;
        }

        // Delete DeleteComplaint ...
        public static int DeleteComplaint(int todoID)
        {
            int Resp = 0;

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    Complaint objSaleOfficer = dbContext.Complaints.Where(u => u.ComplaintID == todoID).FirstOrDefault();
                    dbContext.Complaints.Remove(objSaleOfficer);
                    //obj.IsDeleted = true;
                    dbContext.SaveChanges();
                }
            }
            catch(Exception exp)
            {
                Log.Instance.Error(exp, "Delete Complaint Failed");
                Resp = 1;
            }
            return Resp;
        }


        public static List<ComplaintData> GetResult(string search, string sortOrder, int start, int length, List<ComplaintData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ComplaintData> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private static IQueryable<ComplaintData> FilterResult(string search, List<ComplaintData> dtResult, List<string> columnFilters)
        {
            IQueryable<ComplaintData> results = dtResult.AsQueryable();

            //results = results.Where(p => (search == null || (p.Name != null && p.Name.ToLower().Contains(search.ToLower()) || p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(search.ToLower()) || p.CityName != null && p.CityName.ToLower().Contains(search.ToLower()) || p.AreaName != null && p.AreaName.ToLower().Contains(search.ToLower()) || p.Phone1 != null && p.Phone1.ToLower().Contains(search.ToLower())
            //    || p.Phone2 != null && p.Phone2.ToLower().Contains(search.ToLower())))
            //    && (columnFilters[2] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[3].ToLower())))
            //    && (columnFilters[3] == null || (p.RegionalHeadName != null && p.RegionalHeadName.ToLower().Contains(columnFilters[3].ToLower())))
            //     && (columnFilters[4] == null || (p.CityName != null && p.CityName.ToLower().Contains(columnFilters[3].ToLower())))
            //      && (columnFilters[5] == null || (p.AreaName != null && p.AreaName.ToLower().Contains(columnFilters[3].ToLower())))
            //    && (columnFilters[6] == null || (p.Phone1 != null && p.Phone1.ToLower().Contains(columnFilters[4].ToLower())))
            //    && (columnFilters[7] == null || (p.Phone2 != null && p.Phone2.ToLower().Contains(columnFilters[5].ToLower())))
            //    );

            return results;
        }

        public static List<RegionalHeadData> GetRegionalHeadAccordingToType(int RegionalHeadType)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.RegionalHeads.Where(u => u.Type == RegionalHeadType && u.IsDeleted == false).ToList()
                            .Select(
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
            return regionalHeadData;
        }



        public static List<SaleOfficerData> GetSaleOfficerListByRegionalHeadID(int RegionalHeadID)
        {
            List<SaleOfficerData> compData = new List<SaleOfficerData>();

            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    compData = dbContext.SaleOfficers.Where(u => u.RegionalHeadID == RegionalHeadID && u.IsDeleted == false).ToList()
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

            return compData;
        }

        public static List<RegionalHeadData> GetRegionalHeads(int UserID)
        {
            List<RegionalHeadData> regionalHeadData = new List<RegionalHeadData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.RegionalHeads.Where(u => u.IsActive == true && u.IsDeleted == false).ToList()
                            .Select(
                                u => new RegionalHeadData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).ToList();
                    if (UserID == 1119 || UserID == 0)
                    {
                        regionalHeadData.Insert(0, new RegionalHeadData
                        {
                            ID = 0,
                            Name = "All"
                        });
                    }
                    else
                    {
                        regionalHeadData.Insert(0, new RegionalHeadData
                        {
                            ID = 0,
                            Name = "Select"
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }

        public static List<ComplaintData> GetComplaintDataForGrid(string FromDate, string EndDate, int RegionheadID, int ProductNatureID)
        {
            List<ComplaintData> data = new List<ComplaintData>();
            DateTime start = Convert.ToDateTime(string.IsNullOrEmpty(FromDate) ? DateTime.Now.ToString() : FromDate);
            DateTime End = Convert.ToDateTime(string.IsNullOrEmpty(EndDate) ? DateTime.Now.ToString() : EndDate);
            using (FOSDataModel dbContext = new FOSDataModel())
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand("dbo.sp_GetComplaintDataForQuality", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", start);
                cmd.Parameters.AddWithValue("@EndDate", End);
                cmd.Parameters.AddWithValue("@RegionalHeadID", RegionheadID);
                cmd.Parameters.AddWithValue("@ProductNatureID", ProductNatureID);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var updatedComplaint = reader["UpdatedComplaint"] as DateTime?;
                        data.Add(new ComplaintData
                        {
                            ComplaintID = Convert.ToInt32(reader["ID"]),
                            SaleOfficerName = reader["SaleOfficerName"] as string,
                            CustomerName = reader["CustomerName"] as string,
                            DealerName = reader["DealerName"] as string,
                            ProductDesc = reader["ProductDescription"] as string,
                            ProductBatchNo = reader["ProductBatchNo"] as string,
                            StartingDate1 = ((DateTime)reader["CreatedOn"]).ToString("dd-MMM-yyyy"),
                            ShakingTime = reader["ShakingTime"] as string,
                            ColorCode = reader["ColorCode"] as string,
                            Status = reader["LaunchStatus"] as string,
                            ComplaintNumber = reader["ComplaintNumber"] as string ?? "",
                            StartingDate2 = updatedComplaint == null ? "" : updatedComplaint.Value.ToString("dd-MMM-yyyy"),
                            IsVerifiedString = updatedComplaint == null ? "" : reader["IsVarified"] as string
                        });
                    }
                }
            }

            return data;
        }

        public static List<ProductNatureData> GetProductNatureList()
        {
            List<ProductNatureData> productNatureData = new List<ProductNatureData>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    productNatureData = dbContext.Tbl_ProductNature.Where(u => u.IsActive == true).ToList()
                            .Select(
                                u => new ProductNatureData
                                {
                                    ID = u.ID,
                                    Name = u.Name,
                                }).ToList();
                    productNatureData.Insert(0, new ProductNatureData
                    {
                        ID = 0,
                        Name = "All"
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            return productNatureData;
        }

        public static List<ComplaintApplicationCause> GetApplicationCause()
        {
            List<ComplaintApplicationCause> regionalHeadData = new List<ComplaintApplicationCause>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.ComplaintApplicationCauses.Where(u => u.IsActive == true).ToList()
                            .Select(
                                u => new ComplaintApplicationCause
                                {
                                    ID = u.ID,
                                    AppName = u.AppName,
                                }).ToList();
                    //regionalHeadData.Insert(0, new ComplaintApplicationCause
                    //{
                    //    ID = 0,
                    //    AppName = "Select"
                    //});
                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }

        public static List<ComplaintSubstrateCause> GetSubstrateCause()
        {
            List<ComplaintSubstrateCause> regionalHeadData = new List<ComplaintSubstrateCause>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.ComplaintSubstrateCauses.Where(u => u.IsActive == true).ToList()
                            .Select(
                                u => new ComplaintSubstrateCause
                                {
                                    ID = u.ID,
                                    SubName = u.SubName,
                                }).ToList();
                    //regionalHeadData.Insert(0, new ComplaintSubstrateCause
                    //{
                    //    ID = 0,
                    //    SubName = "Select"
                    //});
                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }

        public static List<ComplaintProductQualityRelated> GetProductQualityCause()
        {
            List<ComplaintProductQualityRelated> regionalHeadData = new List<ComplaintProductQualityRelated>();
            try
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    regionalHeadData = dbContext.ComplaintProductQualityRelateds.Where(u => u.IsActive == true).ToList()
                            .Select(
                                u => new ComplaintProductQualityRelated
                                {
                                    ID = u.ID,
                                    QuaName = u.QuaName,
                                }).ToList();
                    //regionalHeadData.Insert(0, new ComplaintProductQualityRelated
                    //{
                    //    ID = 0,
                    //    QuaName = "Select"
                    //});
                }
            }
            catch (Exception)
            {
                throw;
            }
            return regionalHeadData;
        }
    }
}