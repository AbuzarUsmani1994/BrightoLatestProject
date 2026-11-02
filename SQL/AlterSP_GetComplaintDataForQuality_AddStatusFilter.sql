-- =============================================
-- Adds a Complaint Status filter to the Manage Complaints grid.
-- @ComplaintStatus = 0 means All (default, same convention as the
-- existing @ProductNatureID = 0 "All" filter on this same proc).
-- 1 = In Progress, 2 = On Hold, 3 = Resolved, 4 = Duplicate - same codes
-- ComplaintSuggesionAdded.ComplaintStatus already uses.
--
-- Filters on cds.ComplaintStatus - the int column from the SAME latest-row
-- OUTER APPLY already in this proc (see
-- AlterSP_GetComplaintDataForQuality_LatestStatusOnly.sql), so this is the
-- complaint's current/latest status, not "ever had this status at some
-- point in its history".
--
-- Everything else (columns, joins, other filters) is unchanged from
-- AlterSP_GetComplaintDataForQuality_LatestStatusOnly.sql.
-- =============================================
-- Exec:
-- EXEC dbo.sp_GetComplaintDataForQuality @FromDate = '2025-12-01', @EndDate = '2025-12-31', @RegionalHeadID = 0, @ProductNatureID = 0, @ComplaintStatus = 0
-- EXEC dbo.sp_GetComplaintDataForQuality @FromDate = '2025-12-01', @EndDate = '2025-12-31', @RegionalHeadID = 0, @ProductNatureID = 0, @ComplaintStatus = 1

CREATE OR ALTER PROCEDURE [dbo].[sp_GetComplaintDataForQuality]
@FromDate DateTime,
@EndDate DateTime,
@RegionalHeadID int,
@ProductNatureID int = 0,
@ComplaintStatus int = 0

AS
BEGIN
select tsc.ID,s.Name SaleOfficerName,rc.ShopName CustomerName,rd.ShopName DealerName
,item.Product_Desc ProductDescription,CAST(tsc.CreatedOn as date) CreatedOn,tsc.ProductBatchNo,
tsc.ShakingTime,tsc.ColorCode,tsc.Status LaunchStatus,MAX(cu.ComplaintNumber) ComplaintNumber,
CAST(cds.UpdatedDate as date) UpdatedComplaint, cds.IsVerifiedName IsVarified, pn.Name ProductNature,
cds.ComplaintStatusName ComplaintStatus
from Tbl_SaveComplaints tsc
inner join SaleOfficers s on tsc.SOID = s.ID
inner join Retailers rc on tsc.CustomerID = rc.ID
inner join Retailers rd on tsc.DealerID = rd.ID
inner join Tbl_ProductDetail item on tsc.ProductID = item.ID
inner join Tbl_ProductNature pn on tsc.ProductNatureID = pn.ID
inner join Tbl_ComplaintUpdate cu on tsc.ID = cu.ComplaintID
outer apply (
    select top 1 *
    from ComplaintSuggesionAdded
    where ComplaintID = tsc.ID
    order by CreatedDate desc, ID desc
) cds
where tsc.CreatedOn >= @FromDate and tsc.CreatedOn <= @EndDate
and ((@RegionalHeadID = 0) or (@RegionalHeadID != 0 and s.RegionalHeadID = @RegionalHeadID))
and ((@ProductNatureID = 0) or (@ProductNatureID != 0 and tsc.ProductNatureID = @ProductNatureID))
and ((@ComplaintStatus = 0) or (@ComplaintStatus != 0 and cds.ComplaintStatus = @ComplaintStatus))
and tsc.IsActive = 1

group by tsc.ID,s.Name,rc.ShopName,rd.ShopName,item.Product_Desc,
CAST(tsc.CreatedOn as date),tsc.ProductBatchNo,tsc.ShakingTime,tsc.ColorCode,tsc.Status,
CAST(cds.UpdatedDate as date),cds.IsVerifiedName, pn.Name, cds.ComplaintStatusName

END
GO

-- Exec:
-- EXEC dbo.sp_GetComplaintDataForQuality @FromDate = '2025-12-01', @EndDate = '2025-12-31', @RegionalHeadID = 0, @ProductNatureID = 0, @ComplaintStatus = 0
-- EXEC dbo.sp_GetComplaintDataForQuality @FromDate = '2025-12-01', @EndDate = '2025-12-31', @RegionalHeadID = 0, @ProductNatureID = 0, @ComplaintStatus = 1
