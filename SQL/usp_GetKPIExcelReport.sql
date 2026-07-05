-- =============================================
-- Full KPI Excel Report SP — all 12 focus areas.
-- Tracked actuals:
--   Attendance And Coverage -> Tbl_SOAttendanceandPunctuality
--   Training Evaluation     -> Tbl_SOTraining
--   Total Sales Target      -> SUM(Tbl_SalesClaimMaster.TotalLiters)        per SO in quarter
--   Platinum Target         -> SUM where Tbl_Segmenttype.Name='Special Coating'
--   Premium Target          -> SUM where Tbl_Segmenttype.Name='ACTD'
--   All others              -> 1 (no tracking yet)
--
-- Verify exact FocusArea spellings before running:
--   SELECT Name FROM dbo.Tbl_FocusArea
-- Verify segment names:
--   SELECT ID, Name FROM dbo.Tbl_Segmenttype
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
      AND IsDeleted       = 0
      AND IsActive        = 1;

    IF @StartDate IS NULL
    BEGIN
        SELECT 0 AS Sr WHERE 1 = 0;
        RETURN;
    END

    SELECT
        ROW_NUMBER() OVER (ORDER BY rh.Name, so.Name)  AS Sr,
        rh.Name                                          AS HeadName,
        so.Name                                          AS SOName,

        -- ── 1. Total Sales Target -> actual = SUM of all claim liters ────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target'
                        THEN dk.TargetValue END), 0)    AS SalesTarget,
        ISNULL((
            SELECT SUM(c.TotalLiters)
            FROM   dbo.Tbl_SalesClaimMaster c
            WHERE  c.SOID          = so.ID
              AND  c.DateSelected >= @StartDate
              AND  c.DateSelected <= @EndDate
              AND  ISNULL(c.IsActive, 1) = 1
        ), 0)                                            AS SalesActual,

        -- ── 2. Platinum Target -> actual = Special Coating liters ─────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Platinum Target'
                        THEN dk.TargetValue END), 0)    AS PlatinumTarget,
        ISNULL((
            SELECT SUM(c.TotalLiters)
            FROM   dbo.Tbl_SalesClaimMaster c
            INNER JOIN dbo.Tbl_Segmenttype  st ON st.ID = c.SegmentID
            WHERE  c.SOID          = so.ID
              AND  c.DateSelected >= @StartDate
              AND  c.DateSelected <= @EndDate
              AND  ISNULL(c.IsActive, 1) = 1
              AND  st.Name = 'Special Coating'
        ), 0)                                            AS PlatinumActual,

        -- ── 3. Premium Target -> actual = ACTD liters ────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Premium Target'
                        THEN dk.TargetValue END), 0)    AS PremiumTarget,
        ISNULL((
            SELECT SUM(c.TotalLiters)
            FROM   dbo.Tbl_SalesClaimMaster c
            INNER JOIN dbo.Tbl_Segmenttype  st ON st.ID = c.SegmentID
            WHERE  c.SOID          = so.ID
              AND  c.DateSelected >= @StartDate
              AND  c.DateSelected <= @EndDate
              AND  ISNULL(c.IsActive, 1) = 1
              AND  st.Name = 'ACTD'
        ), 0)                                            AS PremiumActual,

        -- ── 4. Dealer Visits Target (actual = 1) ─────────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Dealer Visits Target'
                        THEN dk.TargetValue END), 0)    AS DealerVisitsTarget,
        1                                                AS DealerVisitsActual,

        -- ── 5. Site Visit Target (actual = 1) ────────────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target'
                        THEN dk.TargetValue END), 0)    AS SiteVisitsTarget,
        1                                                AS SiteVisitsActual,

        -- ── 6. Business Affiliate Visit Target (actual = 1) ──────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Business Affiliate Visit Target'
                        THEN dk.TargetValue END), 0)    AS ContractorVisitsTarget,
        1                                                AS ContractorVisitsActual,

        -- ── 7. Customer Satisfaction (actual = 1) ────────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Customer Satisfaction'
                        THEN dk.TargetValue END), 0)    AS CustSatisfactionTarget,
        1                                                AS CustSatisfactionActual,

        -- ── 8. Area Coverage (actual = 1) ────────────────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Area Coverage'
                        THEN dk.TargetValue END), 0)    AS AreaCoverageTarget,
        1                                                AS AreaCoverageActual,

        -- ── 9. Attendance And Coverage -> tracked ─────────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage'
                        THEN dk.TargetValue END), 0)    AS AttendanceTarget,
        ISNULL((
            SELECT SUM(a.AttendanceandPunctuality)
            FROM   dbo.Tbl_SOAttendanceandPunctuality a
            WHERE  a.SOID            = so.ID
              AND  a.FinancialYearID = @FinancialYearID
              AND  a.Quarter         = @Quarter
              AND  ISNULL(a.IsActive, 1) = 1
        ), 0)                                            AS AttendanceActual,

        -- ── 10. Product Knowledge (actual = 1) ───────────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge'
                        THEN dk.TargetValue END), 0)    AS ProdKnowTarget,
        1                                                AS ProdKnowActual,

        -- ── 11. Training Evaluation -> tracked ───────────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation'
                        THEN dk.TargetValue END), 0)    AS TrainingTarget,
        ISNULL((
            SELECT SUM(t.Training)
            FROM   dbo.Tbl_SOTraining t
            WHERE  t.SOID            = so.ID
              AND  t.FinancialYearID = @FinancialYearID
              AND  t.Quarter         = @Quarter
              AND  ISNULL(t.IsActive, 1) = 1
        ), 0)                                            AS TrainingActual,

        -- ── 12. Compititor Feedback (actual = 1) ─────────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback'
                        THEN dk.TargetValue END), 0)    AS CompFeedTarget,
        1                                                AS CompFeedActual,

        -- ── Total Target (sum of all 12 focus areas) ─────────────────────────
        ISNULL(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target'              THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Platinum Target'                 THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Premium Target'                  THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Dealer Visits Target'            THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target'               THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Business Affiliate Visit Target' THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Customer Satisfaction'           THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Area Coverage'                   THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage'         THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge'               THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation'             THEN dk.TargetValue END), 0)
      + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback'             THEN dk.TargetValue END), 0)
                                                         AS TotalTarget,

        -- ── Total Actual ──────────────────────────────────────────────────────
        -- Sales Total Liters
        ISNULL((
            SELECT SUM(c.TotalLiters)
            FROM   dbo.Tbl_SalesClaimMaster c
            WHERE  c.SOID          = so.ID
              AND  c.DateSelected >= @StartDate
              AND  c.DateSelected <= @EndDate
              AND  ISNULL(c.IsActive, 1) = 1
        ), 0)
        -- Platinum (Special Coating)
      + ISNULL((
            SELECT SUM(c.TotalLiters)
            FROM   dbo.Tbl_SalesClaimMaster c
            INNER JOIN dbo.Tbl_Segmenttype  st ON st.ID = c.SegmentID
            WHERE  c.SOID          = so.ID
              AND  c.DateSelected >= @StartDate
              AND  c.DateSelected <= @EndDate
              AND  ISNULL(c.IsActive, 1) = 1
              AND  st.Name = 'Special Coating'
        ), 0)
        -- Premium (ACTD)
      + ISNULL((
            SELECT SUM(c.TotalLiters)
            FROM   dbo.Tbl_SalesClaimMaster c
            INNER JOIN dbo.Tbl_Segmenttype  st ON st.ID = c.SegmentID
            WHERE  c.SOID          = so.ID
              AND  c.DateSelected >= @StartDate
              AND  c.DateSelected <= @EndDate
              AND  ISNULL(c.IsActive, 1) = 1
              AND  st.Name = 'ACTD'
        ), 0)
        + 1  -- Dealer Visits Target
        + 1  -- Site Visit Target
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
          ), 0)  -- Attendance And Coverage
        + 1  -- Product Knowledge
        + ISNULL((
              SELECT SUM(t.Training)
              FROM   dbo.Tbl_SOTraining t
              WHERE  t.SOID            = so.ID
                AND  t.FinancialYearID = @FinancialYearID
                AND  t.Quarter         = @Quarter
                AND  ISNULL(t.IsActive, 1) = 1
          ), 0)  -- Training Evaluation
        + 1  -- Compititor Feedback
                                                         AS TotalActual

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
