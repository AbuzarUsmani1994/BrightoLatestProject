-- ============================================================================
-- Alter: usp_GetClaimSummaryReportSOWise
-- What changed: Added @SubmissionType VARCHAR(10) = 'All' parameter to support
--   an "All / Late Submissions" filter on the Claim Summary SO Wise Report,
--   mirroring the filter already applied (in C#, row-level) on the sibling
--   Claim Detail / Claim Summary reports. Because this proc pre-aggregates
--   (SUM/MAX grouped by SO.ID), the filter must be applied inside the CTE's
--   WHERE clause, BEFORE aggregation, not after.
--
--   "Late" definition (matches ReportsController.IsLateSubmission logic):
--   A claim for month M (M.DateSelected) is late once it is punched
--   (M.CreatedOn) on or after the 11th of month M+1, i.e. strictly after the
--   10th of the month following the claim's month:
--     CAST(M.CreatedOn AS DATE) >
--       DATEADD(DAY, 9, DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(M.DateSelected), MONTH(M.DateSelected), 1)))
--
--   @SubmissionType = 'All'  (default) -> no additional filtering, 100%
--     backward compatible with existing callers that don't pass this param.
--   @SubmissionType = 'Late' -> only late claim-detail rows are included in
--     the aggregation.
--
-- Why: Requested to add a server-side "Late Submissions" filter to the Claim
--   Summary SO Wise report so late-punched claims can be isolated per SO,
--   consistent with the existing Claim Detail / Claim Summary reports.
--
-- Author: sql-agent
-- Date: 2026-09-03
--
-- Example test calls:
-- EXEC usp_GetClaimSummaryReportSOWise @headID = 0, @SaleOfficerId = 0, @startdate = '2026-04-01', @enddate = '2026-04-30';
-- EXEC usp_GetClaimSummaryReportSOWise @headID = 0, @SaleOfficerId = 0, @startdate = '2026-04-01', @enddate = '2026-04-30', @SubmissionType = 'All';
-- EXEC usp_GetClaimSummaryReportSOWise @headID = 0, @SaleOfficerId = 0, @startdate = '2026-04-01', @enddate = '2026-04-30', @SubmissionType = 'Late';
-- ============================================================================

CREATE OR ALTER PROCEDURE usp_GetClaimSummaryReportSOWise
    @headID INT = 0,
    @SaleOfficerId INT = 0,
    @startdate DATE,
    @enddate DATE,
    @SubmissionType VARCHAR(10) = 'All'
AS
BEGIN
    ;WITH SalesData AS (
        SELECT
            SO.ID AS SOID,
            MAX(head.Name) AS [Head Name],
            MAX(SO.Name) AS [SO Name],
            MAX(Reg.Name) AS [Region Name],
            SUM((ISNULL(D.Drum, 0) * ISNULL(prod.Drum_Price, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_Price, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_Price, 0)) * 1.18) AS MRPValue,
            SUM(DISTINCT M.SaleValue) AS SaleValue,
            SUM(ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0)) AS [Total Liters],
            MAX(kpi.TotalTarget) AS [Target Liters],
            SUM((ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0)) *
                ISNULL(prod.Incentive_PKR, 0)) AS IncentiveValue,
            SUM((ISNULL(D.Drum, 0) * ISNULL(prod.Drum_Price, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_Price, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_Price, 0))) AS TotalPrice
        FROM [dbo].[Tbl_SalesClaimMaster] M
        INNER JOIN [dbo].[Tbl_ClaimDetail] D ON M.ID = D.ClaimMasterID
        INNER JOIN [dbo].SaleOfficers SO ON M.SOID = SO.ID
        INNER JOIN [dbo].RegionalHeads head ON so.RegionalHeadID = head.ID
        INNER JOIN [dbo].Regions Reg ON SO.RegionID = Reg.ID
        INNER JOIN [dbo].[Tbl_ProductDetail] prod ON D.ProductID = prod.ID
        LEFT JOIN [dbo].Tbl_KPITargetsRegionWise kpi ON SO.RegionID = kpi.regionid
        WHERE
            M.DateSelected BETWEEN @startdate AND @enddate
            AND SO.RegionalHeadID = CASE WHEN @headID = 0 THEN SO.RegionalHeadID ELSE @headID END
            AND SO.ID = CASE WHEN @SaleOfficerId = 0 THEN SO.ID ELSE @SaleOfficerId END
            AND M.IsActive = 1
            AND ISNULL(kpi.isactive, 1) = 1
            AND (
                UPPER(@SubmissionType) <> 'LATE'
                OR CAST(M.CreatedOn AS DATE) > DATEADD(DAY, 9, DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(M.DateSelected), MONTH(M.DateSelected), 1)))
            )
        GROUP BY SO.ID
    )
    SELECT
        [Head Name], [SO Name], [Region Name],
        FORMAT(SaleValue, 'N2') AS SaleValue,
        FORMAT([Total Liters], 'N2') AS [Total Liters],
        ISNULL([Target Liters], 0) AS [Target Liters],
        CONVERT(DECIMAL(18,2), [Total Liters] - ISNULL([Target Liters], 0)) AS [Target Balance],
        FORMAT(IncentiveValue, 'N2') AS IncentiveValue,
        FORMAT(TotalPrice, 'N2') AS TotalPrice,
        FORMAT(TotalPrice * 0.18, 'N2') AS SalesTax,
        FORMAT(TotalPrice * 1.18, 'N2') AS MRPValue,
        CASE WHEN MRPValue = 0 THEN 0 ELSE CONVERT(DECIMAL(18,2), ((MRPValue - SaleValue) / MRPValue) * 100) END AS [Disc %age]
    FROM SalesData
    ORDER BY [Region Name]
END;

-- Example test calls:
-- EXEC usp_GetClaimSummaryReportSOWise @headID = 0, @SaleOfficerId = 0, @startdate = '2026-04-01', @enddate = '2026-04-30';
-- EXEC usp_GetClaimSummaryReportSOWise @headID = 0, @SaleOfficerId = 0, @startdate = '2026-04-01', @enddate = '2026-04-30', @SubmissionType = 'All';
-- EXEC usp_GetClaimSummaryReportSOWise @headID = 0, @SaleOfficerId = 0, @startdate = '2026-04-01', @enddate = '2026-04-30', @SubmissionType = 'Late';
