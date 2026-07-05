-- =============================================
-- KPI Percentage Report SP — all 12 focus areas.
-- Target  = TargetPercentage (weight) from Tbl_DetailKPI
-- Actual  = MIN((Actual / TargetValue) * TargetPercentage, TargetPercentage)
--           i.e. achievement cannot exceed the allocated weight.
-- All outputs are DECIMAL(18,2) — percentage values need decimals.
-- =============================================
CREATE OR ALTER PROCEDURE dbo.usp_GetKPIPercentageReport
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

    -- Helper: percentage achievement = MIN((actual/target)*weight, weight), safe division
    -- Inline as CASE expression per column.

    SELECT
        ROW_NUMBER() OVER (ORDER BY rh.Name, so.Name)  AS Sr,
        rh.Name                                          AS HeadName,
        so.Name                                          AS SOName,

        -- ── 1. Total Sales Target ────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS SalesTargetPct,
        CAST(ISNULL(
            (CASE
                WHEN ISNULL(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target' THEN dk.TargetValue END), 0) = 0 THEN 0
                ELSE ISNULL(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target' THEN dk.TargetPercentage END), 0)
                   * CAST(ISNULL((SELECT SUM(c.TotalLiters) FROM dbo.Tbl_SalesClaimMaster c
                                   WHERE c.SOID = so.ID AND c.DateSelected >= @StartDate AND c.DateSelected <= @EndDate
                                     AND ISNULL(c.IsActive,1)=1), 0) AS FLOAT)
                   / CAST(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target' THEN dk.TargetValue END) AS FLOAT)
             END)
        , 0) AS DECIMAL(18,2))                                                     AS SalesActualPct,

        -- ── 2. Platinum Target ───────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Platinum Target'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS PlatinumTargetPct,
        CAST(ISNULL(
            (CASE
                WHEN ISNULL(MAX(CASE WHEN dk.FocusArea = 'Platinum Target' THEN dk.TargetValue END), 0) = 0 THEN 0
                ELSE ISNULL(MAX(CASE WHEN dk.FocusArea = 'Platinum Target' THEN dk.TargetPercentage END), 0)
                   * CAST(ISNULL((
                         SELECT SUM(ISNULL(cd.Drum,0)*ISNULL(pd.Drum_UoM,0)+ISNULL(cd.Gallon,0)*ISNULL(pd.Gallon_UoM,0)+ISNULL(cd.Quarter,0)*ISNULL(pd.Qtr_UoM,0))
                         FROM dbo.Tbl_SalesClaimMaster cm
                         INNER JOIN dbo.Tbl_ClaimDetail cd ON cd.ClaimMasterID = cm.ID
                         INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID = cd.ProductID
                         WHERE cm.SOID=so.ID AND cm.DateSelected>=@StartDate AND cm.DateSelected<=@EndDate
                           AND ISNULL(cm.IsActive,1)=1 AND pd.Incentive_Category='Platinum'
                     ), 0) AS FLOAT)
                   / CAST(MAX(CASE WHEN dk.FocusArea = 'Platinum Target' THEN dk.TargetValue END) AS FLOAT)
             END)
        , 0) AS DECIMAL(18,2))                                                     AS PlatinumActualPct,

        -- ── 3. Premium Target ────────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Premium Target'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS PremiumTargetPct,
        CAST(ISNULL(
            (CASE
                WHEN ISNULL(MAX(CASE WHEN dk.FocusArea = 'Premium Target' THEN dk.TargetValue END), 0) = 0 THEN 0
                ELSE ISNULL(MAX(CASE WHEN dk.FocusArea = 'Premium Target' THEN dk.TargetPercentage END), 0)
                   * CAST(ISNULL((
                         SELECT SUM(ISNULL(cd.Drum,0)*ISNULL(pd.Drum_UoM,0)+ISNULL(cd.Gallon,0)*ISNULL(pd.Gallon_UoM,0)+ISNULL(cd.Quarter,0)*ISNULL(pd.Qtr_UoM,0))
                         FROM dbo.Tbl_SalesClaimMaster cm
                         INNER JOIN dbo.Tbl_ClaimDetail cd ON cd.ClaimMasterID = cm.ID
                         INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID = cd.ProductID
                         WHERE cm.SOID=so.ID AND cm.DateSelected>=@StartDate AND cm.DateSelected<=@EndDate
                           AND ISNULL(cm.IsActive,1)=1 AND pd.Incentive_Category='Premium'
                     ), 0) AS FLOAT)
                   / CAST(MAX(CASE WHEN dk.FocusArea = 'Premium Target' THEN dk.TargetValue END) AS FLOAT)
             END)
        , 0) AS DECIMAL(18,2))                                                     AS PremiumActualPct,

        -- ── 4. Dealer Visits Target ──────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Dealer visit Target'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS DealerVisitsTargetPct,
        CAST(ISNULL(
            (CASE
                WHEN ISNULL(MAX(CASE WHEN dk.FocusArea = 'Dealer visit Target' THEN dk.TargetValue END), 0) = 0 THEN 0
                ELSE ISNULL(MAX(CASE WHEN dk.FocusArea = 'Dealer visit Target' THEN dk.TargetPercentage END), 0)
                   * CAST(ISNULL((SELECT COUNT(*) FROM dbo.Tbl_TradeVisitsFinal tv
                                   WHERE tv.SOID=so.ID AND tv.CreatedAt>=@StartDate
                                     AND tv.CreatedAt<=DATEADD(DAY,1,CAST(@EndDate AS DATETIME))
                                     AND ISNULL(tv.IsActive,1)=1), 0) AS FLOAT)
                   / CAST(MAX(CASE WHEN dk.FocusArea = 'Dealer visit Target' THEN dk.TargetValue END) AS FLOAT)
             END)
        , 0) AS DECIMAL(18,2))                                                     AS DealerVisitsActualPct,

        -- ── 5. Site Visit Target ─────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS SiteVisitsTargetPct,
        CAST(ISNULL(
            (CASE
                WHEN ISNULL(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target' THEN dk.TargetValue END), 0) = 0 THEN 0
                ELSE ISNULL(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target' THEN dk.TargetPercentage END), 0)
                   * CAST(ISNULL((SELECT COUNT(*) FROM dbo.Tbl_HousingVisits hv
                                   WHERE hv.SOID=so.ID AND hv.CreatedAt>=@StartDate
                                     AND hv.CreatedAt<=DATEADD(DAY,1,CAST(@EndDate AS DATETIME))
                                     AND ISNULL(hv.IsActive,1)=1), 0) AS FLOAT)
                   / CAST(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target' THEN dk.TargetValue END) AS FLOAT)
             END)
        , 0) AS DECIMAL(18,2))                                                     AS SiteVisitsActualPct,

        -- ── 6. Business Affiliate Visit Target ───────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Business Affiliate Visit Target'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS ContractorVisitsTargetPct,
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Business Affiliate Visit Target'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS ContractorVisitsActualPct,

        -- ── 7. Customer Satisfaction ─────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Customer Satisfaction'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS CustSatisfactionTargetPct,
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Customer Satisfaction'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS CustSatisfactionActualPct,

        -- ── 8. Area Coverage ─────────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Area Coverage'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS AreaCoverageTargetPct,
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Area Coverage'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS AreaCoverageActualPct,

        -- ── 9. Attendance And Coverage ───────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS AttendanceTargetPct,
        CAST(ISNULL(
            (CASE
                WHEN ISNULL(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage' THEN dk.TargetValue END), 0) = 0 THEN 0
                ELSE ISNULL(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage' THEN dk.TargetPercentage END), 0)
                   * CAST(ISNULL((SELECT SUM(a.AttendanceandPunctuality) FROM dbo.Tbl_SOAttendanceandPunctuality a
                                   WHERE a.SOID=so.ID AND a.FinancialYearID=@FinancialYearID
                                     AND a.Quarter=@Quarter AND ISNULL(a.IsActive,1)=1), 0) AS FLOAT)
                   / CAST(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage' THEN dk.TargetValue END) AS FLOAT)
             END)
        , 0) AS DECIMAL(18,2))                                                     AS AttendanceActualPct,

        -- ── 10. Product Knowledge ────────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS ProdKnowTargetPct,
        CAST(ISNULL(
            (CASE
                WHEN ISNULL(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge' THEN dk.TargetValue END), 0) = 0 THEN 0
                ELSE ISNULL(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge' THEN dk.TargetPercentage END), 0)
                   * CAST(ISNULL((SELECT SUM(pk.ProductKnowledge) FROM dbo.Tbl_SOProdKnowledgeCompFeed pk
                                   WHERE pk.SOID=so.ID AND pk.FinancialYearID=@FinancialYearID
                                     AND pk.Quarter=@Quarter AND ISNULL(pk.IsActive,1)=1), 0) AS FLOAT)
                   / CAST(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge' THEN dk.TargetValue END) AS FLOAT)
             END)
        , 0) AS DECIMAL(18,2))                                                     AS ProdKnowActualPct,

        -- ── 11. Training Evaluation ──────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS TrainingTargetPct,
        CAST(ISNULL(
            (CASE
                WHEN ISNULL(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation' THEN dk.TargetValue END), 0) = 0 THEN 0
                ELSE ISNULL(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation' THEN dk.TargetPercentage END), 0)
                   * CAST(ISNULL((SELECT SUM(t.Training) FROM dbo.Tbl_SOTraining t
                                   WHERE t.SOID=so.ID AND t.FinancialYearID=@FinancialYearID
                                     AND t.Quarter=@Quarter AND ISNULL(t.IsActive,1)=1), 0) AS FLOAT)
                   / CAST(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation' THEN dk.TargetValue END) AS FLOAT)
             END)
        , 0) AS DECIMAL(18,2))                                                     AS TrainingActualPct,

        -- ── 12. Compititor Feedback ──────────────────────────────────────────
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback'
                              THEN dk.TargetPercentage END), 0) AS DECIMAL(18,2))  AS CompFeedTargetPct,
        CAST(ISNULL(
            (CASE
                WHEN ISNULL(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback' THEN dk.TargetValue END), 0) = 0 THEN 0
                ELSE ISNULL(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback' THEN dk.TargetPercentage END), 0)
                   * CAST(ISNULL((SELECT SUM(pk.CompFeed) FROM dbo.Tbl_SOProdKnowledgeCompFeed pk
                                   WHERE pk.SOID=so.ID AND pk.FinancialYearID=@FinancialYearID
                                     AND pk.Quarter=@Quarter AND ISNULL(pk.IsActive,1)=1), 0) AS FLOAT)
                   / CAST(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback' THEN dk.TargetValue END) AS FLOAT)
             END)
        , 0) AS DECIMAL(18,2))                                                     AS CompFeedActualPct,

        -- ── Total Target % (sum of all 12 weights) ───────────────────────────
        CAST(
            ISNULL(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target'              THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Platinum Target'                 THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Premium Target'                  THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Dealer visit Target'             THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target'               THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Business Affiliate Visit Target' THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Customer Satisfaction'           THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Area Coverage'                   THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage'         THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge'               THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation'             THEN dk.TargetPercentage END), 0)
          + ISNULL(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback'             THEN dk.TargetPercentage END), 0)
        AS DECIMAL(18,2))                                                          AS TotalTargetPct,

        -- ── Total Actual % (sum of all 12 achieved percentages) ──────────────
        -- Each term: MIN((actual/target)*weight, weight)  handled via CASE above;
        -- here we re-sum the same expressions for the grand total.
        CAST(
            -- Sales
            ISNULL((CASE WHEN ISNULL(MAX(CASE WHEN dk.FocusArea='Total Sales Target' THEN dk.TargetValue END),0)=0 THEN 0
                         ELSE ISNULL(MAX(CASE WHEN dk.FocusArea='Total Sales Target' THEN dk.TargetPercentage END),0)
                            * CAST(ISNULL((SELECT SUM(c.TotalLiters) FROM dbo.Tbl_SalesClaimMaster c WHERE c.SOID=so.ID AND c.DateSelected>=@StartDate AND c.DateSelected<=@EndDate AND ISNULL(c.IsActive,1)=1),0) AS FLOAT)
                            / CAST(MAX(CASE WHEN dk.FocusArea='Total Sales Target' THEN dk.TargetValue END) AS FLOAT)
                    END), 0)
            -- Platinum
          + ISNULL((CASE WHEN ISNULL(MAX(CASE WHEN dk.FocusArea='Platinum Target' THEN dk.TargetValue END),0)=0 THEN 0
                         ELSE ISNULL(MAX(CASE WHEN dk.FocusArea='Platinum Target' THEN dk.TargetPercentage END),0)
                            * CAST(ISNULL((SELECT SUM(ISNULL(cd.Drum,0)*ISNULL(pd.Drum_UoM,0)+ISNULL(cd.Gallon,0)*ISNULL(pd.Gallon_UoM,0)+ISNULL(cd.Quarter,0)*ISNULL(pd.Qtr_UoM,0)) FROM dbo.Tbl_SalesClaimMaster cm INNER JOIN dbo.Tbl_ClaimDetail cd ON cd.ClaimMasterID=cm.ID INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID=cd.ProductID WHERE cm.SOID=so.ID AND cm.DateSelected>=@StartDate AND cm.DateSelected<=@EndDate AND ISNULL(cm.IsActive,1)=1 AND pd.Incentive_Category='Platinum'),0) AS FLOAT)
                            / CAST(MAX(CASE WHEN dk.FocusArea='Platinum Target' THEN dk.TargetValue END) AS FLOAT)
                    END), 0)
            -- Premium
          + ISNULL((CASE WHEN ISNULL(MAX(CASE WHEN dk.FocusArea='Premium Target' THEN dk.TargetValue END),0)=0 THEN 0
                         ELSE ISNULL(MAX(CASE WHEN dk.FocusArea='Premium Target' THEN dk.TargetPercentage END),0)
                            * CAST(ISNULL((SELECT SUM(ISNULL(cd.Drum,0)*ISNULL(pd.Drum_UoM,0)+ISNULL(cd.Gallon,0)*ISNULL(pd.Gallon_UoM,0)+ISNULL(cd.Quarter,0)*ISNULL(pd.Qtr_UoM,0)) FROM dbo.Tbl_SalesClaimMaster cm INNER JOIN dbo.Tbl_ClaimDetail cd ON cd.ClaimMasterID=cm.ID INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID=cd.ProductID WHERE cm.SOID=so.ID AND cm.DateSelected>=@StartDate AND cm.DateSelected<=@EndDate AND ISNULL(cm.IsActive,1)=1 AND pd.Incentive_Category='Premium'),0) AS FLOAT)
                            / CAST(MAX(CASE WHEN dk.FocusArea='Premium Target' THEN dk.TargetValue END) AS FLOAT)
                    END), 0)
            -- Dealer Visits
          + ISNULL((CASE WHEN ISNULL(MAX(CASE WHEN dk.FocusArea='Dealer visit Target' THEN dk.TargetValue END),0)=0 THEN 0
                         ELSE ISNULL(MAX(CASE WHEN dk.FocusArea='Dealer visit Target' THEN dk.TargetPercentage END),0)
                            * CAST(ISNULL((SELECT COUNT(*) FROM dbo.Tbl_TradeVisitsFinal tv WHERE tv.SOID=so.ID AND tv.CreatedAt>=@StartDate AND tv.CreatedAt<=DATEADD(DAY,1,CAST(@EndDate AS DATETIME)) AND ISNULL(tv.IsActive,1)=1),0) AS FLOAT)
                            / CAST(MAX(CASE WHEN dk.FocusArea='Dealer visit Target' THEN dk.TargetValue END) AS FLOAT)
                    END), 0)
            -- Site Visits
          + ISNULL((CASE WHEN ISNULL(MAX(CASE WHEN dk.FocusArea='Site Visit Target' THEN dk.TargetValue END),0)=0 THEN 0
                         ELSE ISNULL(MAX(CASE WHEN dk.FocusArea='Site Visit Target' THEN dk.TargetPercentage END),0)
                            * CAST(ISNULL((SELECT COUNT(*) FROM dbo.Tbl_HousingVisits hv WHERE hv.SOID=so.ID AND hv.CreatedAt>=@StartDate AND hv.CreatedAt<=DATEADD(DAY,1,CAST(@EndDate AS DATETIME)) AND ISNULL(hv.IsActive,1)=1),0) AS FLOAT)
                            / CAST(MAX(CASE WHEN dk.FocusArea='Site Visit Target' THEN dk.TargetValue END) AS FLOAT)
                    END), 0)
            -- Business Affiliate (always full weight)
          + ISNULL(MAX(CASE WHEN dk.FocusArea='Business Affiliate Visit Target' THEN dk.TargetPercentage END), 0)
            -- Customer Satisfaction (always full weight)
          + ISNULL(MAX(CASE WHEN dk.FocusArea='Customer Satisfaction' THEN dk.TargetPercentage END), 0)
            -- Area Coverage (always full weight)
          + ISNULL(MAX(CASE WHEN dk.FocusArea='Area Coverage' THEN dk.TargetPercentage END), 0)
            -- Attendance
          + ISNULL((CASE WHEN ISNULL(MAX(CASE WHEN dk.FocusArea='Attendance And Coverage' THEN dk.TargetValue END),0)=0 THEN 0
                         ELSE ISNULL(MAX(CASE WHEN dk.FocusArea='Attendance And Coverage' THEN dk.TargetPercentage END),0)
                            * CAST(ISNULL((SELECT SUM(a.AttendanceandPunctuality) FROM dbo.Tbl_SOAttendanceandPunctuality a WHERE a.SOID=so.ID AND a.FinancialYearID=@FinancialYearID AND a.Quarter=@Quarter AND ISNULL(a.IsActive,1)=1),0) AS FLOAT)
                            / CAST(MAX(CASE WHEN dk.FocusArea='Attendance And Coverage' THEN dk.TargetValue END) AS FLOAT)
                    END), 0)
            -- Product Knowledge
          + ISNULL((CASE WHEN ISNULL(MAX(CASE WHEN dk.FocusArea='Product Knowledge' THEN dk.TargetValue END),0)=0 THEN 0
                         ELSE ISNULL(MAX(CASE WHEN dk.FocusArea='Product Knowledge' THEN dk.TargetPercentage END),0)
                            * CAST(ISNULL((SELECT SUM(pk.ProductKnowledge) FROM dbo.Tbl_SOProdKnowledgeCompFeed pk WHERE pk.SOID=so.ID AND pk.FinancialYearID=@FinancialYearID AND pk.Quarter=@Quarter AND ISNULL(pk.IsActive,1)=1),0) AS FLOAT)
                            / CAST(MAX(CASE WHEN dk.FocusArea='Product Knowledge' THEN dk.TargetValue END) AS FLOAT)
                    END), 0)
            -- Training
          + ISNULL((CASE WHEN ISNULL(MAX(CASE WHEN dk.FocusArea='Training Evaluation' THEN dk.TargetValue END),0)=0 THEN 0
                         ELSE ISNULL(MAX(CASE WHEN dk.FocusArea='Training Evaluation' THEN dk.TargetPercentage END),0)
                            * CAST(ISNULL((SELECT SUM(t.Training) FROM dbo.Tbl_SOTraining t WHERE t.SOID=so.ID AND t.FinancialYearID=@FinancialYearID AND t.Quarter=@Quarter AND ISNULL(t.IsActive,1)=1),0) AS FLOAT)
                            / CAST(MAX(CASE WHEN dk.FocusArea='Training Evaluation' THEN dk.TargetValue END) AS FLOAT)
                    END), 0)
            -- Competitor Feedback
          + ISNULL((CASE WHEN ISNULL(MAX(CASE WHEN dk.FocusArea='Compititor Feedback' THEN dk.TargetValue END),0)=0 THEN 0
                         ELSE ISNULL(MAX(CASE WHEN dk.FocusArea='Compititor Feedback' THEN dk.TargetPercentage END),0)
                            * CAST(ISNULL((SELECT SUM(pk.CompFeed) FROM dbo.Tbl_SOProdKnowledgeCompFeed pk WHERE pk.SOID=so.ID AND pk.FinancialYearID=@FinancialYearID AND pk.Quarter=@Quarter AND ISNULL(pk.IsActive,1)=1),0) AS FLOAT)
                            / CAST(MAX(CASE WHEN dk.FocusArea='Compititor Feedback' THEN dk.TargetValue END) AS FLOAT)
                    END), 0)
        AS DECIMAL(18,2))                                                          AS TotalActualPct

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
-- EXEC dbo.usp_GetKPIPercentageReport @FinancialYearID = 1, @Quarter = 'Q1', @RegionalHeadID = 0
