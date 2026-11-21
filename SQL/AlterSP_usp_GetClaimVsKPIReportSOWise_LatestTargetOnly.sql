-- Fixes duplicate rows in the "Incentive Claim SO Wise" / Claims vs KPI report
-- (ReportsController.ClaimSummaryVSKPISOWiseRpt -> ClaimsVsKpiReport.rdlc).
--
-- Root cause: the proc LEFT JOINed dbo.Tbl_KPITargetsRegionWise purely on
-- SOID, with no filter at all, and GROUP BY included the kpi target columns.
-- Whenever an SO has more than one row in that table (confirmed live: SOID
-- 298 has 4 rows, with TWO of them both marked IsActive = 1 - one under an
-- old FinancialYearID, one under the current one), every matching kpi row
-- produced its own separate group/output row for that SO.
--
-- A simple IsActive = 1 filter (which was commented out in the original proc -
-- "-- and kpi.isactive=1") is NOT enough on its own, since this data already
-- has two IsActive = 1 rows for the same SOID.
--
-- Fix: replace the LEFT JOIN with OUTER APPLY + TOP 1 ... ORDER BY CreatedOn
-- DESC, so each SO always resolves to exactly one target row - the single
-- most recently created one - regardless of how many rows exist for that
-- SOID or how IsActive is set on any of them.

CREATE OR ALTER PROCEDURE [dbo].[usp_GetClaimVsKPIReportSOWise]
    @headID INT = 0,
    @SaleOfficerId INT = 0,
    @startdate DATE,
    @enddate DATE
AS
BEGIN
    SELECT
        MAX(head.Name) AS HeadName,
        MAX(SO.Name) AS SOName,
        MAX(Reg.Name) AS RegionName,
        IsNull(kpi.TotalTarget,0) AS TotalTargetLiters,
        IsNull(kpi.ReadyMixTarget,0) AS ReadyMixTarget,
        IsNull(kpi.CoatingTarget,0) AS CoatingTarget,
        IsNull(kpi.ACTDTarget,0) AS ACTDTarget,
        -- Total Liters Calculation
        FORMAT(  SUM(ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
            ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
            ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0)) ,'N2')AS TotalLiters,
        FORMAT(
            SUM(CASE WHEN prod.Brand_ID = 1 THEN
                (ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0))
                ELSE 0 END)
        ,'N2') AS ReadyMixAchieved,

        FORMAT(
            SUM(CASE WHEN prod.Brand_ID = 2 THEN
                (ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0))
                ELSE 0 END)
        ,'N2') AS ACTDAchieved,

        FORMAT(
            SUM(CASE WHEN prod.Brand_ID = 8 THEN
                (ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0))
                ELSE 0 END)
        ,'N2') AS SpecialCoatingAchieved,

        CONVERT(DECIMAL(18,2),
            SUM(ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
            ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
            ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0)) -
            ISNULL(kpi.TotalTarget, 0)
        ) AS TargetBalance,

        -- IncentiveValue Calculation
        FORMAT(
            SUM(
                (ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
                 ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
                 ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0)) *
                 ISNULL(prod.Incentive_PKR, 0)
            ), 'N2'
        ) AS TotalIncentiveValue    ,

        FORMAT(
            SUM(CASE WHEN prod.Brand_ID = 1 THEN
                (ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0))*
                ISNULL(prod.Incentive_PKR, 0)
                ELSE 0 END)
        ,'N2') AS IncentiveValueReadyMix ,

        FORMAT(
            SUM(CASE WHEN prod.Brand_ID = 2 THEN
                (ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0))*
                ISNULL(prod.Incentive_PKR, 0)
                ELSE 0 END)
        ,'N2') AS IncentiveValueACTD,
        FORMAT(
            SUM(CASE WHEN prod.Brand_ID = 8 THEN
                (ISNULL(D.Drum, 0) * ISNULL(prod.Drum_UOM, 0) +
                ISNULL(D.Gallon, 0) * ISNULL(prod.Gallon_UOM, 0) +
                ISNULL(D.Quarter, 0) * ISNULL(prod.Qtr_UOM, 0))*
                ISNULL(prod.Incentive_PKR, 0)
                ELSE 0 END)
        ,'N2') AS IncentiveValueSpecialCoating

    FROM [dbo].[Tbl_SalesClaimMaster] M
    INNER JOIN [dbo].[Tbl_ClaimDetail] D ON M.ID = D.ClaimMasterID
    INNER JOIN [dbo].SaleOfficers SO ON M.SOID = SO.ID
    INNER JOIN [dbo].RegionalHeads head ON so.RegionalHeadID = head.ID
    INNER JOIN [dbo].Regions Reg ON SO.RegionID = Reg.ID
    INNER JOIN [dbo].Retailers Ret ON M.tradePartyID = Ret.ID
    INNER JOIN [dbo].Cities City ON Ret.cityid = City.ID
    INNER JOIN [dbo].Retailers Ret1 ON M.CustomerID = Ret1.ID
    INNER JOIN [dbo].[Tbl_ProductDetail] prod ON D.ProductID = prod.ID
    OUTER APPLY (
        -- Exactly one target row per SO: whichever was created most recently,
        -- regardless of IsActive/FinancialYearID - this is what collapses the
        -- duplicate output rows.
        SELECT TOP 1 k.TotalTarget, k.ReadyMixTarget, k.CoatingTarget, k.ACTDTarget
        FROM [dbo].Tbl_KPITargetsRegionWise k
        WHERE k.SOID = SO.ID
        ORDER BY k.CreatedOn DESC, k.ID DESC
    ) kpi
    WHERE
        M.DateSelected >= @startdate AND M.DateSelected <= @enddate
        AND SO.RegionalHeadID = CASE WHEN @headID = 0 THEN SO.RegionalHeadID ELSE @headID END
        AND SO.ID = CASE WHEN @SaleOfficerId = 0 THEN SO.ID ELSE @SaleOfficerId END
        AND m.IsActive = 1

    GROUP BY
        SO.ID,
        kpi.TotalTarget,
        kpi.ReadyMixTarget,
        kpi.CoatingTarget,
        kpi.ACTDTarget,
        Reg.Name

    Order by Reg.Name
END;
