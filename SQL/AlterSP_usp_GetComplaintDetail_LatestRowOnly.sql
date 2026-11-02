-- =============================================
-- Fixes a duplicate-row bug in usp_GetComplaintDetail that is exposed by
-- the Quality Review history change (ComplaintSuggesionAdded moved from
-- upsert/one-row-per-complaint to insert-only history with many rows per
-- ComplaintID).
--
-- Previously this proc did:
--     left join ComplaintSuggesionAdded sug on c.ID = sug.ComplaintID
-- which only returned one row per complaint while there was at most one
-- ComplaintSuggesionAdded row per ComplaintID. Once ComplaintSuggesionAdded
-- holds many history rows per ComplaintID, this LEFT JOIN would fan out and
-- the "Complaint Detail Report" CSV export
-- (ReportsController, ~line 9139) would start showing duplicate rows per
-- complaint (one per history row).
--
-- Fix: replace the LEFT JOIN with OUTER APPLY (TOP 1 ... ORDER BY
-- CreatedDate DESC, ID DESC) to pull only the single MOST RECENT
-- ComplaintSuggesionAdded row per ComplaintID, same pattern already used in
-- SQL\AlterSP_GetComplaintDataForQuality_LatestStatusOnly.sql. OUTER APPLY
-- (not CROSS APPLY) is used so complaints with zero suggestion rows still
-- appear in the export, preserving the prior LEFT JOIN behavior for that
-- case. Alias name (sug) and every referenced sug.* column
-- (ApplicationCaseName, SubstrateCaseName, QualityProductName,
-- IsVerifiedName, Remarks, UpdatedDate) are unchanged, as is everything
-- else in the proc (other joins, OUTER APPLYs, filters, GROUP BY, ORDER BY).
-- =============================================
-- Exec:
-- EXEC dbo.usp_GetComplaintDetail @DateFrom = '2026-05-01', @DateTo = '2026-05-31', @SOID = 0, @RegionalHeadID = 0

CREATE OR ALTER PROCEDURE [dbo].[usp_GetComplaintDetail]
@DateFrom DateTime,
@DateTo DateTime,
@SOID int,
@RegionalHeadID int
AS
BEGIN
SELECT
        C.ID AS ComplaintID,so.Name AS SONAME,ret.ShopName AS CustomerName,ret.Phone1,ret1.ShopName AS DealerName,
        PN.Name AS ProductNature,CU.ComplaintNumber,C.CreatedOn AS ComplaintDate,C.ProductbatchNo,Item.Product_Desc ProductDescription,
        C.ShakingTime,C.ColorCode,C.Quantity,C.SubstrateColor,C.TinningRatio,C.NoOfCoatsApplied,
        C.TimeDurationWithinCoats,ts.Name AS ThinningSolvent,ato.Name AS AppliedTool,C.Remarks,'Substrate' AS SubstrateType,
		'NaturalType' as NatureType,Substrate.SubstrateAttributes,Nature.NatureAttributes,
		sug.ApplicationCaseName,sug.SubstrateCaseName,sug.QualityProductName,sug.IsVerifiedName,sug.Remarks QualityRemarks,sug.UpdatedDate QualityUpdateDate

    FROM Tbl_SaveComplaints C
    OUTER APPLY (
        SELECT TOP 1 *
        FROM Tbl_ComplaintUpdate CU
        WHERE CU.ComplaintID = C.ID
        ORDER BY CU.launcedTime DESC
    ) CU
    LEFT JOIN SaleOfficers SO ON C.soid = SO.ID
    LEFT JOIN Retailers ret ON C.CustomerID = ret.ID
    LEFT JOIN Retailers ret1 ON C.DealerID = ret1.ID
    LEFT JOIN Tbl_ProductNature PN ON C.ProductnatureID = PN.ID
    LEFT JOIN Tbl_ProductDetail Item ON C.ProductID = item.ID
    LEFT JOIN Tbl_ThinningSolvent ts ON C.thinningSolventID = ts.ID
    LEFT JOIN Tbl_AppliedTool ATo ON C.AppliedToolID = ATo.ID
    OUTER APPLY (
    SELECT STRING_AGG(Name, ', ') AS SubstrateAttributes
    FROM (
        SELECT DISTINCT S.Name
        FROM Tbl_SaveConditionOfSubstrate S
        WHERE S.ComplaintMasterID = C.ID
          AND S.IsChecked = 1
    ) x
) Substrate
OUTER APPLY (
    SELECT STRING_AGG(Name, ', ') AS NatureAttributes
    FROM (
        SELECT DISTINCT NC.Name
        FROM Tbl_SaveNatureOfComplaint NC
        WHERE NC.ComplaintMasterID = C.ID
          AND NC.IsChecked = 1
    ) y
) Nature
OUTER APPLY (
    SELECT TOP 1 *
    FROM ComplaintSuggesionAdded
    WHERE ComplaintID = C.ID
    ORDER BY CreatedDate DESC, ID DESC
) sug

    WHERE
        C.CreatedOn BETWEEN @DateFrom AND @DateTo
        AND ((@SOID  = 0) or (@SOID != 0 and c.SOID = @SOID ))
		And ((@RegionalHeadID = 0) or (@RegionalHeadID != 0 and SO.RegionalHeadID = @RegionalHeadID))


GROUP BY
    C.ID, SO.Name, ret.ShopName, ret.Phone1, ret1.ShopName,
    PN.Name, CU.ComplaintNumber, C.CreatedOn, C.ProductbatchNo,
    item.Product_Desc, C.ShakingTime,
    C.ColorCode, C.Quantity, C.SubstrateColor, C.TinningRatio,
    C.NoOfCoatsApplied, C.TimeDurationWithinCoats, ts.Name,
    ato.Name, C.Remarks,Substrate.SubstrateAttributes,Nature.NatureAttributes,
	sug.ApplicationCaseName,sug.SubstrateCaseName,sug.QualityProductName,sug.IsVerifiedName,sug.UpdatedDate,sug.Remarks

	order by c.ID asc
END
GO

-- Exec:
-- EXEC dbo.usp_GetComplaintDetail @DateFrom = '2026-05-01', @DateTo = '2026-05-31', @SOID = 0, @RegionalHeadID = 0
