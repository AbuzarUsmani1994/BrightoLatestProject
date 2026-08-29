-- =============================================
-- Adds Product Nature filtering (Solvent Based / Water Based / All)
-- to the Manage Complaints grid.
-- @ProductNatureID = 0 means All.
-- =============================================
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
CAST(cds.UpdatedDate as date) UpdatedComplaint, cds.IsVerifiedName IsVarified, pn.Name ProductNature
from Tbl_SaveComplaints tsc
inner join SaleOfficers s on tsc.SOID = s.ID
inner join Retailers rc on tsc.CustomerID = rc.ID
inner join Retailers rd on tsc.DealerID = rd.ID
inner join Tbl_ProductDetail item on tsc.ProductID = item.ID
inner join Tbl_ProductNature pn on tsc.ProductNatureID = pn.ID
inner join Tbl_ComplaintUpdate cu on tsc.ID = cu.ComplaintID
left join ComplaintSuggesionAdded cds on tsc.ID = cds.ComplaintID
where tsc.CreatedOn >= @FromDate and tsc.CreatedOn <= @EndDate
and ((@RegionalHeadID = 0) or (@RegionalHeadID != 0 and s.RegionalHeadID = @RegionalHeadID))
and ((@ProductNatureID = 0) or (@ProductNatureID != 0 and tsc.ProductNatureID = @ProductNatureID))
and tsc.IsActive = 1

group by tsc.ID,s.Name,rc.ShopName,rd.ShopName,item.Product_Desc,
CAST(tsc.CreatedOn as date),tsc.ProductBatchNo,tsc.ShakingTime,tsc.ColorCode,tsc.Status,
CAST(cds.UpdatedDate as date),cds.IsVerifiedName, pn.Name

END
GO

-- ── Test ─────────────────────────────────────────────────────────────────────
-- EXEC dbo.sp_GetComplaintDataForQuality @FromDate = '2025-12-01', @EndDate = '2025-12-31', @RegionalHeadID = 0, @ProductNatureID = 0
