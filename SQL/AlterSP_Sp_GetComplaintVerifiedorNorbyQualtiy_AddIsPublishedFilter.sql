-- =============================================
-- Only the latest PUBLISHED ComplaintSuggesionAdded row per complaint should
-- reach the SOComplaints.rdlc PDF report (ComplaintController.
-- DownloadPDFCompalintReport) - see
-- SQL\AlterTbl_ComplaintSuggesionAdded_AddIsPublished.sql for why.
--
-- Change from AlterSP_Sp_GetComplaintVerifiedorNorbyQualtiy_LatestRowOnly.sql:
-- the csa CROSS APPLY now adds "AND IsPublished = 1" to its WHERE, so it
-- finds the latest PUBLISHED row instead of the latest row regardless of
-- publish state. CROSS APPLY (not OUTER APPLY) is unchanged - a complaint
-- whose history is entirely unpublished (or has none) still produces ZERO
-- rows here, same "no history -> no result" behavior as before, now
-- extended to "no PUBLISHED history -> no result". Everything else in the
-- proc is identical to that script.
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
      and IsPublished = 1
    order by CreatedDate desc, ID desc
) csa
where sc.ID = @ComplaintID
END
GO

-- Exec:
-- EXEC dbo.Sp_GetComplaintVerifiedorNorbyQualtiy @ComplaintID = 19
