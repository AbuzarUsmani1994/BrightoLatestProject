-- =============================================
-- Only the latest PUBLISHED ComplaintSuggesionAdded row per complaint should
-- reach the Complaint Summary Report (ReportsController, ~line 9200) - see
-- SQL\AlterTbl_ComplaintSuggesionAdded_AddIsPublished.sql for why.
--
-- Change from AlterSP_sp_usp_GetComplaintSummary_LatestRowOnly.sql: the csa
-- OUTER APPLY now adds "AND IsPublished = 1" to its WHERE, so it finds the
-- latest PUBLISHED row instead of the latest row regardless of publish
-- state. A complaint whose history is entirely unpublished (or has none)
-- still appears in the export, just with csa.* all NULL - OUTER APPLY (not
-- CROSS APPLY) is unchanged. Everything else in the proc is identical to
-- that script.
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
	  and IsPublished = 1
	order by CreatedDate desc, ID desc
) csa

where c.CreatedOn >= @FromDate and c.CreatedOn <= @EndDate
and ((@SOID = 0) or (@SOID != 0 and c.SOID = @SOID)) and
((@RegionHeadID = 0) or (@RegionHeadID != 0 and  s.RegionalHeadID = @RegionHeadID))
END
GO

-- Exec:
-- EXEC dbo.sp_usp_GetComplaintSummary @FromDate = '2026-05-01', @EndDate = '2026-05-31', @SOID = 0, @RegionHeadID = 0
