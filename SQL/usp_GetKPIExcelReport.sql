-- =============================================
-- Full KPI Excel Report SP — all 12 focus areas.
-- Tracked actuals:
--   Attendance And Coverage -> Tbl_SOAttendanceandPunctuality
--   Training Evaluation     -> Tbl_SOTraining
--   Total Sales Target      -> SUM(Tbl_SalesClaimMaster.TotalLiters) per SO in quarter
--   Platinum Target         -> SUM liters from Tbl_ClaimDetail->Tbl_ProductDetail where Incentive_Category='Platinum'
--   Premium Target          -> SUM liters from Tbl_ClaimDetail->Tbl_ProductDetail where Incentive_Category='Premium'
--   Dealer Visits Target    -> COUNT(Tbl_TradeVisitsFinal) per SO in quarter
--   Product Knowledge       -> SUM(Tbl_SOProdKnowledgeCompFeed.ProductKnowledge) per SO in quarter
--   Compititor Feedback     -> SUM(Tbl_SOProdKnowledgeCompFeed.CompFeed) per SO in quarter
--   All others              -> 1 (no tracking yet)
-- All numeric outputs cast to INT (no decimal places).
-- =============================================
CREATE OR ALTER PROCEDURE dbo.usp_GetKPIExcelReport
    @FinancialYearID INT,
    @Quarter         NVARCHAR(100),
    @RegionalHeadID  INT = 0          -- 0 = all heads
AS
BEGIN
    SET NOCOUNT ON;

    -- Resolve quarter to date range
    DECLARE @StartDate DATE, @EndDate DATE;
    SELECT TOP 1
        @StartDate = StartDate,
        @EndDate   = EndDate
    FROM dbo.Tbl_Quarters
    WHERE FinancialYearID = @FinancialYearID
      AND Name            = @Quarter
      AND ISNULL(IsDeleted, 0) = 0
      AND ISNULL(IsActive,  1) = 1;

    IF @StartDate IS NULL
    BEGIN
        SELECT 0 AS Sr WHERE 1 = 0;
        RETURN;
    END

    SELECT
        ROW_NUMBER() OVER (ORDER BY rh.Name, so.Name)  AS Sr,
        rh.Name                                          AS HeadName,
        so.Name                                          AS SOName,

        -- ── 1. Total Sales Target ────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target'
                              THEN dk.TargetValue END), 0) AS INT)   AS SalesTarget,
        CAST(ISNULL((
            SELECT SUM(c.TotalLiters)
            FROM   dbo.Tbl_SalesClaimMaster c
            WHERE  c.SOID          = so.ID
              AND  c.DateSelected >= @StartDate
              AND  c.DateSelected <= @EndDate
              AND  ISNULL(c.IsActive, 1) = 1
        ), 0) AS INT)                                                AS SalesActual,

        -- ── 2. Platinum Target ───────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Platinum Target'
                              THEN dk.TargetValue END), 0) AS INT)   AS PlatinumTarget,
        CAST(ISNULL((
            SELECT SUM(
                ISNULL(cd.Drum,    0) * ISNULL(pd.Drum_UoM,   0) +
                ISNULL(cd.Gallon,  0) * ISNULL(pd.Gallon_UoM, 0) +
                ISNULL(cd.Quarter, 0) * ISNULL(pd.Qtr_UoM,    0)
            )
            FROM   dbo.Tbl_SalesClaimMaster cm
            INNER JOIN dbo.Tbl_ClaimDetail   cd ON cd.ClaimMasterID = cm.ID
            INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID = cd.ProductID
            WHERE  cm.SOID          = so.ID
              AND  cm.DateSelected >= @StartDate
              AND  cm.DateSelected <= @EndDate
              AND  ISNULL(cm.IsActive, 1) = 1
              AND  pd.Incentive_Category  = 'Platinum'
        ), 0) AS INT)                                                AS PlatinumActual,

        -- ── 3. Premium Target ────────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Premium Target'
                              THEN dk.TargetValue END), 0) AS INT)   AS PremiumTarget,
        CAST(ISNULL((
            SELECT SUM(
                ISNULL(cd.Drum,    0) * ISNULL(pd.Drum_UoM,   0) +
                ISNULL(cd.Gallon,  0) * ISNULL(pd.Gallon_UoM, 0) +
                ISNULL(cd.Quarter, 0) * ISNULL(pd.Qtr_UoM,    0)
            )
            FROM   dbo.Tbl_SalesClaimMaster cm
            INNER JOIN dbo.Tbl_ClaimDetail   cd ON cd.ClaimMasterID = cm.ID
            INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID = cd.ProductID
            WHERE  cm.SOID          = so.ID
              AND  cm.DateSelected >= @StartDate
              AND  cm.DateSelected <= @EndDate
              AND  ISNULL(cm.IsActive, 1) = 1
              AND  pd.Incentive_Category  = 'Premium'
        ), 0) AS INT)                                                AS PremiumActual,

        -- ── 4. Dealer Visits Target ──────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Dealer visit Target'
                              THEN dk.TargetValue END), 0) AS INT)   AS DealerVisitsTarget,
        ISNULL((
            SELECT COUNT(*)
            FROM   dbo.Tbl_TradeVisitsFinal tv
            WHERE  tv.SOID          = so.ID
              AND  tv.CreatedAt    >= @StartDate
              AND  tv.CreatedAt    <= DATEADD(DAY, 1, CAST(@EndDate AS DATETIME))
              AND  ISNULL(tv.IsActive, 1) = 1
        ), 0)                                                                   AS DealerVisitsActual,

        -- ── 5. Site Visit Target ─────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target'
                              THEN dk.TargetValue END), 0) AS INT)   AS SiteVisitsTarget,
        ISNULL((
            SELECT COUNT(*)
            FROM   dbo.Tbl_HousingVisits hv
            WHERE  hv.SOID          = so.ID
              AND  hv.CreatedAt    >= @StartDate
              AND  hv.CreatedAt    <= DATEADD(DAY, 1, CAST(@EndDate AS DATETIME))
              AND  ISNULL(hv.IsActive, 1) = 1
        ), 0)                                                                   AS SiteVisitsActual,

        -- ── 6. Business Affiliate Visit Target (actual = 1) ──────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Business Affiliate Visit Target'
                              THEN dk.TargetValue END), 0) AS INT)   AS ContractorVisitsTarget,
        1                                                                       AS ContractorVisitsActual,

        -- ── 7. Customer Satisfaction (actual = 1) ────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Customer Satisfaction'
                              THEN dk.TargetValue END), 0) AS INT)   AS CustSatisfactionTarget,
        1                                                                       AS CustSatisfactionActual,

        -- ── 8. Area Coverage (actual = 1) ────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Area Coverage'
                              THEN dk.TargetValue END), 0) AS INT)   AS AreaCoverageTarget,
        1                                                                       AS AreaCoverageActual,

        -- ── 9. Attendance And Coverage ───────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage'
                              THEN dk.TargetValue END), 0) AS INT)   AS AttendanceTarget,
        CAST(ISNULL((
            SELECT SUM(a.AttendanceandPunctuality)
            FROM   dbo.Tbl_SOAttendanceandPunctuality a
            WHERE  a.SOID            = so.ID
              AND  a.FinancialYearID = @FinancialYearID
              AND  a.Quarter         = @Quarter
              AND  ISNULL(a.IsActive, 1) = 1
        ), 0) AS INT)                                                AS AttendanceActual,

        -- ── 10. Product Knowledge ────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge'
                              THEN dk.TargetValue END), 0) AS INT)   AS ProdKnowTarget,
        ISNULL((
            SELECT SUM(pk.ProductKnowledge)
            FROM   dbo.Tbl_SOProdKnowledgeCompFeed pk
            WHERE  pk.SOID            = so.ID
              AND  pk.FinancialYearID = @FinancialYearID
              AND  pk.Quarter         = @Quarter
              AND  ISNULL(pk.IsActive, 1) = 1
        ), 0)                                                                   AS ProdKnowActual,

        -- ── 11. Training Evaluation ──────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation'
                              THEN dk.TargetValue END), 0) AS INT)   AS TrainingTarget,
        CAST(ISNULL((
            SELECT SUM(t.Training)
            FROM   dbo.Tbl_SOTraining t
            WHERE  t.SOID            = so.ID
              AND  t.FinancialYearID = @FinancialYearID
              AND  t.Quarter         = @Quarter
              AND  ISNULL(t.IsActive, 1) = 1
        ), 0) AS INT)                                                AS TrainingActual,

        -- ── 12. Compititor Feedback ──────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback'
                              THEN dk.TargetValue END), 0) AS INT)   AS CompFeedTarget,
        ISNULL((
            SELECT SUM(pk.CompFeed)
            FROM   dbo.Tbl_SOProdKnowledgeCompFeed pk
            WHERE  pk.SOID            = so.ID
              AND  pk.FinancialYearID = @FinancialYearID
              AND  pk.Quarter         = @Quarter
              AND  ISNULL(pk.IsActive, 1) = 1
        ), 0)                                                                   AS CompFeedActual,

        -- ── Total Target ─────────────────────────────────────────────────────
        CAST(
            ISNULL(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target'              THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Platinum Target'                 THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Premium Target'                  THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Dealer visit Target'             THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target'               THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Business Affiliate Visit Target' THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Customer Satisfaction'           THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Area Coverage'                   THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage'         THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge'               THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation'             THEN dk.TargetValue END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback'             THEN dk.TargetValue END), 0)
        AS INT)                                                       AS TotalTarget,

        -- ── Total Actual ──────────────────────────────────────────────────────
        CAST(
            -- Sales Total Liters
            ISNULL((
                SELECT SUM(c.TotalLiters)
                FROM   dbo.Tbl_SalesClaimMaster c
                WHERE  c.SOID          = so.ID
                  AND  c.DateSelected >= @StartDate
                  AND  c.DateSelected <= @EndDate
                  AND  ISNULL(c.IsActive, 1) = 1
            ), 0)
            -- Platinum
          + ISNULL((
                SELECT SUM(
                    ISNULL(cd.Drum,    0) * ISNULL(pd.Drum_UoM,   0) +
                    ISNULL(cd.Gallon,  0) * ISNULL(pd.Gallon_UoM, 0) +
                    ISNULL(cd.Quarter, 0) * ISNULL(pd.Qtr_UoM,    0)
                )
                FROM   dbo.Tbl_SalesClaimMaster cm
                INNER JOIN dbo.Tbl_ClaimDetail   cd ON cd.ClaimMasterID = cm.ID
                INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID = cd.ProductID
                WHERE  cm.SOID          = so.ID
                  AND  cm.DateSelected >= @StartDate
                  AND  cm.DateSelected <= @EndDate
                  AND  ISNULL(cm.IsActive, 1) = 1
                  AND  pd.Incentive_Category  = 'Platinum'
            ), 0)
            -- Premium
          + ISNULL((
                SELECT SUM(
                    ISNULL(cd.Drum,    0) * ISNULL(pd.Drum_UoM,   0) +
                    ISNULL(cd.Gallon,  0) * ISNULL(pd.Gallon_UoM, 0) +
                    ISNULL(cd.Quarter, 0) * ISNULL(pd.Qtr_UoM,    0)
                )
                FROM   dbo.Tbl_SalesClaimMaster cm
                INNER JOIN dbo.Tbl_ClaimDetail   cd ON cd.ClaimMasterID = cm.ID
                INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID = cd.ProductID
                WHERE  cm.SOID          = so.ID
                  AND  cm.DateSelected >= @StartDate
                  AND  cm.DateSelected <= @EndDate
                  AND  ISNULL(cm.IsActive, 1) = 1
                  AND  pd.Incentive_Category  = 'Premium'
            ), 0)
            -- Dealer Visits
          + ISNULL((
                SELECT COUNT(*)
                FROM   dbo.Tbl_TradeVisitsFinal tv
                WHERE  tv.SOID          = so.ID
                  AND  tv.CreatedAt    >= @StartDate
                  AND  tv.CreatedAt    <= DATEADD(DAY, 1, CAST(@EndDate AS DATETIME))
                  AND  ISNULL(tv.IsActive, 1) = 1
            ), 0)
          -- Site Visits
          + ISNULL((
                SELECT COUNT(*)
                FROM   dbo.Tbl_HousingVisits hv
                WHERE  hv.SOID          = so.ID
                  AND  hv.CreatedAt    >= @StartDate
                  AND  hv.CreatedAt    <= DATEADD(DAY, 1, CAST(@EndDate AS DATETIME))
                  AND  ISNULL(hv.IsActive, 1) = 1
            ), 0)
          + 1  -- Business Affiliate Visit Target
          + 1  -- Customer Satisfaction
          + 1  -- Area Coverage
          + ISNULL((
                SELECT SUM(a.AttendanceandPunctuality)
                FROM   dbo.Tbl_SOAttendanceandPunctuality a
                WHERE  a.SOID            = so.ID
                  AND  a.FinancialYearID = @FinancialYearID
                  AND  a.Quarter         = @Quarter
                  AND  ISNULL(a.IsActive, 1) = 1
            ), 0)
          -- Product Knowledge
          + ISNULL((
                SELECT SUM(pk.ProductKnowledge)
                FROM   dbo.Tbl_SOProdKnowledgeCompFeed pk
                WHERE  pk.SOID            = so.ID
                  AND  pk.FinancialYearID = @FinancialYearID
                  AND  pk.Quarter         = @Quarter
                  AND  ISNULL(pk.IsActive, 1) = 1
            ), 0)
          + ISNULL((
                SELECT SUM(t.Training)
                FROM   dbo.Tbl_SOTraining t
                WHERE  t.SOID            = so.ID
                  AND  t.FinancialYearID = @FinancialYearID
                  AND  t.Quarter         = @Quarter
                  AND  ISNULL(t.IsActive, 1) = 1
            ), 0)
          -- Compititor Feedback
          + ISNULL((
                SELECT SUM(pk.CompFeed)
                FROM   dbo.Tbl_SOProdKnowledgeCompFeed pk
                WHERE  pk.SOID            = so.ID
                  AND  pk.FinancialYearID = @FinancialYearID
                  AND  pk.Quarter         = @Quarter
                  AND  ISNULL(pk.IsActive, 1) = 1
            ), 0)
        AS INT)                                                       AS TotalActual

    FROM       dbo.Tbl_MasterKPIS  mk
    INNER JOIN dbo.SaleOfficers     so ON so.ID = mk.SOID
    INNER JOIN dbo.RegionalHeads    rh ON rh.ID = mk.HeadID
    INNER JOIN dbo.Tbl_DetailKPI    dk ON dk.KPIMasterID = mk.ID
    WHERE  mk.DateFrom             = @StartDate
      AND  mk.DateTo               = @EndDate
      AND  ISNULL(mk.IsActive, 1) = 1
      AND  (@RegionalHeadID       = 0 OR mk.HeadID = @RegionalHeadID)
    GROUP BY rh.Name, so.Name, so.ID
    ORDER BY rh.Name, so.Name;
END
GO

-- ── Test ─────────────────────────────────────────────────────────────────────
-- EXEC dbo.usp_GetKPIExcelReport @FinancialYearID = 1, @Quarter = 'Q1', @RegionalHeadID = 0
