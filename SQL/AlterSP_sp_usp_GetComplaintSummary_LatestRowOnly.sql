-- =============================================
-- Fixes a duplicate-row bug in sp_usp_GetComplaintSummary that is exposed
-- by the Quality Review history change (ComplaintSuggesionAdded moved from
-- upsert/one-row-per-complaint to insert-only history with many rows per
-- ComplaintID).
--
-- Previously this proc did:
--     left join ComplaintSuggesionAdded csa on c.ID = csa.ComplaintID
-- which only returned one row per complaint while there was at most one
-- ComplaintSuggesionAdded row per ComplaintID. Once ComplaintSuggesionAdded
-- holds many history rows per ComplaintID, this LEFT JOIN would fan out and
-- the complaint summary export (ReportsController, ~line 9200) would start
-- showing duplicate rows per complaint (one per history row).
--
-- Fix: replace the LEFT JOIN with OUTER APPLY (TOP 1 ... ORDER BY
-- CreatedDate DESC, ID DESC) to pull only the single MOST RECENT
-- ComplaintSuggesionAdded row per ComplaintID, same pattern already used in
-- SQL\AlterSP_GetComplaintDataForQuality_LatestStatusOnly.sql. OUTER APPLY
-- (not CROSS APPLY) is used so complaints with zero suggestion rows still
-- appear in the summary, preserving the prior LEFT JOIN behavior for that
-- case. Alias name (csa) and every referenced csa.* column
-- (IsVerifiedName, Remarks, UpdatedDate) are unchanged, as is everything
-- else in the proc (other joins, OUTER APPLY, filters).
-- =============================================
-- Exec:
-- EXEC dbo.sp_usp_GetComplaintSummary @FromDate = '2026-05-01', @EndDate = '2026-05-31', @SOID = 0, @RegionHeadID = 0

CREATE OR ALTER PROCEDURE [dbo].[sp_usp_GetComplaintSummary]
	@FromDate DateTime,
	@EndDate DateTime,
	@SOID int,
	@RegionHeadID int
AS
BEGIN
	select cu.ComplaintNumber,c.ID ComplaintID,
c.CreatedOn,s.Name SOName,
r.ShopName CustomerName, r.Phone1,
d.ShopName DealerName,item.Product_Desc ProductDescription,
csa.IsVerifiedName,csa.Remarks,csa.UpdatedDate

From Tbl_SaveComplaints c
Outer Apply (
	select Top 1 * from Tbl_ComplaintUpdate cd
	where cd.ComplaintID = c.ID
	order by LauncedTime desc
) cu
inner join SaleOfficers s on c.SOID = s.ID
inner join Retailers r on c.CustomerID = r.ID
inner join Retailers d on c.DealerID = d.ID
inner join Tbl_ProductDetail item on c.ProductID = item.ID
outer apply (
	select top 1 *
	from ComplaintSuggesionAdded
	where ComplaintID = c.ID
	order by CreatedDate desc, ID desc
) csa

where c.CreatedOn >= @FromDate and c.CreatedOn <= @EndDate
and ((@SOID = 0) or (@SOID != 0 and c.SOID = @SOID)) and
((@RegionHeadID = 0) or (@RegionHeadID != 0 and  s.RegionalHeadID = @RegionHeadID))
END
GO

-- Exec:
-- EXEC dbo.sp_usp_GetComplaintSummary @FromDate = '2026-05-01', @EndDate = '2026-05-31', @SOID = 0, @RegionHeadID = 0
