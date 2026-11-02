-- =============================================
-- Fixes a duplicate-row bug in Sp_GetComplaintVerifiedorNorbyQualtiy that is
-- exposed by the Quality Review history change (ComplaintSuggesionAdded
-- moved from upsert/one-row-per-complaint to insert-only history with many
-- rows per ComplaintID).
--
-- Previously this proc drove its result set FROM ComplaintSuggesionAdded
-- (inner-joined to Tbl_SaveComplaints) and filtered on
-- csa.ComplaintID = @ComplaintID with no TOP 1 / ORDER BY limiting:
--     from ComplaintSuggesionAdded csa
--     inner join Tbl_SaveComplaints sc on csa.ComplaintID = sc.ID
--     ...
--     where csa.ComplaintID = @ComplaintID
-- This returned one row per ComplaintSuggesionAdded match, which was fine
-- while there was at most one row per ComplaintID. Now that
-- ComplaintSuggesionAdded holds many history rows per ComplaintID, this
-- would fan out and the SOComplaints.rdlc PDF report
-- (ComplaintController.DownloadPDFCompalintReport) would print one
-- "Quality" line per history row instead of one.
--
-- Fix: flip the FROM so Tbl_SaveComplaints (filtered to @ComplaintID) is the
-- driving table, and pull ComplaintSuggesionAdded via CROSS APPLY (TOP 1 ...
-- ORDER BY CreatedDate DESC, ID DESC) to get only the single MOST RECENT
-- row. CROSS APPLY (not OUTER APPLY) is used here - deliberately different
-- from the OUTER APPLY used in sp_GetComplaintDataForQuality - because the
-- original query used an INNER JOIN against ComplaintSuggesionAdded, i.e. a
-- complaint with zero quality-review rows produced ZERO rows (no blank
-- "Quality" section in the report). CROSS APPLY preserves that exact
-- "0 history rows -> 0 result rows" behavior; OUTER APPLY would instead
-- introduce a new all-NULL row for complaints with no history, which is a
-- behavior change we are not asked to make.
--
-- All selected/aliased columns (csa.*) and the Retailers joins are otherwise
-- unchanged.
-- =============================================
-- Exec:
-- EXEC dbo.Sp_GetComplaintVerifiedorNorbyQualtiy @ComplaintID = 19

CREATE OR ALTER PROCEDURE [dbo].[Sp_GetComplaintVerifiedorNorbyQualtiy]
	@ComplaintID int
AS
BEGIN
	select csa.ComplaintSubmitby QualityLaunchBy,ret.ShopName QualityCustomerName,del.ShopName QualityDealerName
	,csa.ComplaintNo ComplaintsNo,FORMAT(csa.ComplaintSubmitDate,'dd-MMM-yyyy hh:mm') ComplaintLaunchAt
	,csa.ApplicationCaseName as ApplicationCauseName,csa.SubstrateCaseName SubstrateCauseName,csa.QualityProductName QualityProductCauseName
,csa.IsVerifiedName ComplaintVarified,Format(csa.UpdatedDate,'dd-MMM-yyyy hh:mm') ComplaintUpdatedAt,
csa.ComplaintSubmitby,csa.Remarks QualityRemarks, csa.ImageUrl QualityImageUrl, csa.VideoUrl QualityVideoUrl
from Tbl_SaveComplaints sc
inner join Retailers ret on sc.CustomerID = ret.ID
inner join Retailers del on sc.DealerID = del.ID
cross apply (
    select top 1 *
    from ComplaintSuggesionAdded
    where ComplaintID = sc.ID
    order by CreatedDate desc, ID desc
) csa
where sc.ID = @ComplaintID
END
GO

-- Exec:
-- EXEC dbo.Sp_GetComplaintVerifiedorNorbyQualtiy @ComplaintID = 19
