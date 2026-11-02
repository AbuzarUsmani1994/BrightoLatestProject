-- =============================================
-- Fixes a duplicate-row bug in sp_GetComplaintDataForQuality that would be
-- exposed by the Quality Review history change (ComplaintSuggesionAdded
-- moving from upsert/one-row-per-complaint to insert-only history with many
-- rows per ComplaintID).
--
-- Previously this proc did:
--     left join ComplaintSuggesionAdded cds on tsc.ID = cds.ComplaintID
-- which only returned one row per complaint because today there's at most
-- one ComplaintSuggesionAdded row per ComplaintID (the app upserts). Once
-- ComplaintSuggesionAdded holds many history rows per ComplaintID, this
-- LEFT JOIN would fan out and the main Manage Complaints grid would start
-- showing duplicate rows per complaint (one per history row).
--
-- Fix: replace the LEFT JOIN with OUTER APPLY (TOP 1 ... ORDER BY
-- CreatedDate DESC, ID DESC) to pull only the single MOST RECENT
-- ComplaintSuggesionAdded row per ComplaintID. OUTER APPLY (not CROSS
-- APPLY) is used so complaints with zero suggestion rows still appear in
-- the grid, preserving prior behavior of the LEFT JOIN for that case.
--
-- Also adds cds.ComplaintStatusName to the SELECT list (aliased
-- ComplaintStatus) and to the GROUP BY, so the grid can optionally surface
-- the latest status later. Everything else (columns, filters, joins) is
-- unchanged from SQL\AlterSP_GetComplaintDataForQuality_AddNatureFilter.sql.
-- =============================================
-- Exec:
-- EXEC dbo.sp_GetComplaintDataForQuality @FromDate = '2025-12-01', @EndDate = '2025-12-31', @RegionalHeadID = 0, @ProductNatureID = 0

CREATE OR ALTER PROCEDURE [dbo].[sp_GetComplaintDataForQuality]
@FromDate DateTime,
@EndDate DateTime,
@RegionalHeadID int,
@ProductNatureID int = 0

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
and tsc.IsActive = 1

group by tsc.ID,s.Name,rc.ShopName,rd.ShopName,item.Product_Desc,
CAST(tsc.CreatedOn as date),tsc.ProductBatchNo,tsc.ShakingTime,tsc.ColorCode,tsc.Status,
CAST(cds.UpdatedDate as date),cds.IsVerifiedName, pn.Name, cds.ComplaintStatusName

END
GO

-- Exec:
-- EXEC dbo.sp_GetComplaintDataForQuality @FromDate = '2025-12-01', @EndDate = '2025-12-31', @RegionalHeadID = 0, @ProductNatureID = 0
