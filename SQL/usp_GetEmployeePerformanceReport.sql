-- =============================================
-- Employee Performance with Customer & Visit Profiling Report
-- Sections:
--   1. Employee Profile  -> SaleOfficers + RegionalHeads + Regions + Cities
--   2. Customer Data     -> Retailers assigned to SO, BusinessStatus, Housing
--   3. KPI Data          -> Tbl_MasterKPIS / Tbl_DetailKPI (same source as KPIPerformanceReport)
--   4. CCR Data          -> sp_GetCallSummaryReport_Simple logic (SO-wise pivot)
--
-- Parameters:
--   @SOID           INT          -- 0 = all SOs under the head
--   @RegionalHeadID INT          -- 0 = all heads in the zone
--   @StartDate      DATE
--   @EndDate        DATE
--   @FinancialYearID INT
--   @Quarter        NVARCHAR(20)
-- =============================================
CREATE OR ALTER PROCEDURE dbo.usp_GetEmployeePerformanceReport
    @SOID            INT          = 0,
    @RegionalHeadID  INT          = 0,
    @StartDate       DATE,
    @EndDate         DATE,
    @FinancialYearID INT,
    @Quarter         NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- ── 1. Candidate SOs ────────────────────────────────────────────────────────
    -- Collect the SOs we need to report on into a temp table for reuse
    CREATE TABLE #SOs (
        SOID            INT,
        SOName          NVARCHAR(200),
        ECode           NVARCHAR(100),
        RegionalHeadID  INT,
        HeadName        NVARCHAR(200),
        RegionName      NVARCHAR(200),
        PrimaryTown     NVARCHAR(200),
        SecondaryTown   NVARCHAR(200)
    );

    INSERT INTO #SOs
    SELECT
        so.ID,
        so.Name,
        ISNULL(so.ECode, ''),
        rh.ID,
        ISNULL(rh.Name, ''),
        ISNULL(reg.Name, ''),
        ISNULL(c1.Name, ''),
        ISNULL(c2.Name, '')
    FROM       dbo.SaleOfficers  so
    LEFT JOIN  dbo.RegionalHeads rh  ON rh.ID  = so.RegionalHeadID
    LEFT JOIN  dbo.Regions       reg ON reg.ID  = so.RegionID
    LEFT JOIN  dbo.Cities        c1  ON c1.ID   = so.PrimaryCityID
    LEFT JOIN  dbo.Cities        c2  ON c2.ID   = so.SecondaryCityID
    WHERE  ISNULL(so.IsActive,  1) = 1
      AND  ISNULL(so.IsDeleted, 0) = 0
      AND  (@SOID           = 0 OR so.ID            = @SOID)
      AND  (@RegionalHeadID = 0 OR so.RegionalHeadID = @RegionalHeadID);

    -- ── 2. Customer / Retailer Counts (per SO) ──────────────────────────────────
    CREATE TABLE #CustomerData (
        SOID            INT,
        TotalCustomers  INT,
        ActiveCount     INT,
        LostCount       INT,
        CompletedCount  INT,
        ResidentialCount INT,
        CommercialCount  INT,
        NewCustomers    INT,
        SitesWon        INT,
        OnlineVisits    INT,
        OfflineVisits   INT
    );

    INSERT INTO #CustomerData
    SELECT
        s.SOID,
        -- Total assigned retailers
        COUNT(DISTINCT r.ID)                                               AS TotalCustomers,
        -- Active = BusinessStatus name like 'Active'
        SUM(CASE WHEN bs.Name LIKE '%Active%'
                  AND ISNULL(bs.IsActive, 1) = 1 THEN 1 ELSE 0 END)      AS ActiveCount,
        -- Lost
        SUM(CASE WHEN bs.Name LIKE '%Lost%'    THEN 1 ELSE 0 END)         AS LostCount,
        -- Completed
        SUM(CASE WHEN bs.Name LIKE '%Complet%' THEN 1 ELSE 0 END)         AS CompletedCount,
        -- Residential
        SUM(CASE WHEN r.CustomerType = 'Residential' THEN 1 ELSE 0 END)   AS ResidentialCount,
        -- Commercial
        SUM(CASE WHEN r.CustomerType = 'Commercial'  THEN 1 ELSE 0 END)   AS CommercialCount,
        -- New Customers registered in date range
        SUM(CASE WHEN CAST(r.CreatedDate AS DATE) BETWEEN @StartDate
                                                       AND @EndDate THEN 1 ELSE 0 END) AS NewCustomers,
        -- Sites Won = Housing visits within date range for this SO
        ISNULL((SELECT COUNT(*)
                FROM   dbo.Tbl_HousingVisits hv
                WHERE  hv.SOID = s.SOID
                  AND  CAST(hv.CreatedAt AS DATE) BETWEEN @StartDate AND @EndDate
                  AND  ISNULL(hv.IsActive, 1) = 1), 0)                    AS SitesWon,
        -- Online visits (AllPurpose) in date range
        ISNULL((SELECT COUNT(*)
                FROM   dbo.Tbl_AllPurposeVisits apv
                WHERE  apv.SOID = s.SOID
                  AND  CAST(apv.CreatedOn AS DATE) BETWEEN @StartDate AND @EndDate
                  AND  apv.OnlineOffline = 1
                  AND  ISNULL(apv.IsActive, 1) = 1), 0)                   AS OnlineVisits,
        -- Offline visits (AllPurpose) in date range
        ISNULL((SELECT COUNT(*)
                FROM   dbo.Tbl_AllPurposeVisits apv
                WHERE  apv.SOID = s.SOID
                  AND  CAST(apv.CreatedOn AS DATE) BETWEEN @StartDate AND @EndDate
                  AND  apv.OnlineOffline = 0
                  AND  ISNULL(apv.IsActive, 1) = 1), 0)                   AS OfflineVisits
    FROM       #SOs            s
    LEFT JOIN  dbo.Retailers   r  ON r.SaleOfficerID = s.SOID
                                  AND ISNULL(r.IsDeleted, 0) = 0
    LEFT JOIN  dbo.BusinessStatus bs ON bs.ID = r.BusinessStatusID
    GROUP BY s.SOID;

    -- ── 3. KPI Actuals (per SO, for the quarter) ────────────────────────────────
    CREATE TABLE #KPIData (
        SOID                    INT,
        SalesTarget             INT, SalesActual             INT,
        PlatinumTarget          INT, PlatinumActual          INT,
        PremiumTarget           INT, PremiumActual           INT,
        DealerVisitsTarget      INT, DealerVisitsActual      INT,
        SiteVisitsTarget        INT, SiteVisitsActual        INT,
        ContractorVisitsTarget  INT, ContractorVisitsActual  INT,
        CustSatisfactionTarget  INT, CustSatisfactionActual  INT,
        AreaCoverageTarget      INT, AreaCoverageActual      INT,
        AttendanceTarget        INT, AttendanceActual        INT,
        TrainingTarget          INT, TrainingActual          INT,
        ProdKnowTarget          INT, ProdKnowActual          INT,
        CompFeedTarget          INT, CompFeedActual          INT
    );

    INSERT INTO #KPIData
    SELECT
        so.ID                                                              AS SOID,

        -- 1. Sales Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Total Sales Target'
                              THEN dk.TargetValue END), 0) AS INT),
        CAST(ISNULL((SELECT SUM(c.TotalLiters)
                     FROM dbo.Tbl_SalesClaimMaster c
                     WHERE c.SOID = so.ID AND c.DateSelected >= @StartDate
                       AND c.DateSelected <= @EndDate AND ISNULL(c.IsActive,1)=1), 0) AS INT),

        -- 2. Platinum Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Platinum Target'
                              THEN dk.TargetValue END), 0) AS INT),
        CAST(ISNULL((SELECT SUM(ISNULL(cd.Drum,0)*ISNULL(pd.Drum_UoM,0)
                              + ISNULL(cd.Gallon,0)*ISNULL(pd.Gallon_UoM,0)
                              + ISNULL(cd.Quarter,0)*ISNULL(pd.Qtr_UoM,0))
                     FROM dbo.Tbl_SalesClaimMaster cm
                     INNER JOIN dbo.Tbl_ClaimDetail   cd ON cd.ClaimMasterID = cm.ID
                     INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID = cd.ProductID
                     WHERE cm.SOID = so.ID AND cm.DateSelected >= @StartDate
                       AND cm.DateSelected <= @EndDate AND ISNULL(cm.IsActive,1)=1
                       AND pd.Incentive_Category = 'Platinum'), 0) AS INT),

        -- 3. Premium Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Premium Target'
                              THEN dk.TargetValue END), 0) AS INT),
        CAST(ISNULL((SELECT SUM(ISNULL(cd.Drum,0)*ISNULL(pd.Drum_UoM,0)
                              + ISNULL(cd.Gallon,0)*ISNULL(pd.Gallon_UoM,0)
                              + ISNULL(cd.Quarter,0)*ISNULL(pd.Qtr_UoM,0))
                     FROM dbo.Tbl_SalesClaimMaster cm
                     INNER JOIN dbo.Tbl_ClaimDetail   cd ON cd.ClaimMasterID = cm.ID
                     INNER JOIN dbo.Tbl_ProductDetail pd ON pd.ID = cd.ProductID
                     WHERE cm.SOID = so.ID AND cm.DateSelected >= @StartDate
                       AND cm.DateSelected <= @EndDate AND ISNULL(cm.IsActive,1)=1
                       AND pd.Incentive_Category = 'Premium'), 0) AS INT),

        -- 4. Dealer Visits Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Dealer visit Target'
                              THEN dk.TargetValue END), 0) AS INT),
        ISNULL((SELECT COUNT(*)
                FROM dbo.Tbl_TradeVisitsFinal tv
                WHERE tv.SOID = so.ID AND tv.CreatedAt >= @StartDate
                  AND tv.CreatedAt <= DATEADD(DAY,1,CAST(@EndDate AS DATETIME))
                  AND ISNULL(tv.IsActive,1)=1), 0),

        -- 5. Site Visits Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Site Visit Target'
                              THEN dk.TargetValue END), 0) AS INT),
        ISNULL((SELECT COUNT(*)
                FROM dbo.Tbl_HousingVisits hv
                WHERE hv.SOID = so.ID AND hv.CreatedAt >= @StartDate
                  AND hv.CreatedAt <= DATEADD(DAY,1,CAST(@EndDate AS DATETIME))
                  AND ISNULL(hv.IsActive,1)=1), 0),

        -- 6. Contractor Visits Target / Actual (Business Affiliates)
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Business Affiliate Visit Target'
                              THEN dk.TargetValue END), 0) AS INT),
        1,

        -- 7. Customer Satisfaction Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Customer Satisfaction'
                              THEN dk.TargetValue END), 0) AS INT),
        1,

        -- 8. Area Coverage Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Area Coverage'
                              THEN dk.TargetValue END), 0) AS INT),
        1,

        -- 9. Attendance Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Attendance And Coverage'
                              THEN dk.TargetValue END), 0) AS INT),
        CAST(ISNULL((SELECT SUM(a.AttendanceandPunctuality)
                     FROM dbo.Tbl_SOAttendanceandPunctuality a
                     WHERE a.SOID = so.ID AND a.FinancialYearID = @FinancialYearID
                       AND a.Quarter = @Quarter AND ISNULL(a.IsActive,1)=1), 0) AS INT),

        -- 10. Training Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Training Evaluation'
                              THEN dk.TargetValue END), 0) AS INT),
        CAST(ISNULL((SELECT SUM(t.Training)
                     FROM dbo.Tbl_SOTraining t
                     WHERE t.SOID = so.ID AND t.FinancialYearID = @FinancialYearID
                       AND t.Quarter = @Quarter AND ISNULL(t.IsActive,1)=1), 0) AS INT),

        -- 11. Product Knowledge Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Product Knowledge'
                              THEN dk.TargetValue END), 0) AS INT),
        CAST(ISNULL((SELECT SUM(pk.ProductKnowledge)
                     FROM dbo.Tbl_SOProdKnowledgeCompFeed pk
                     WHERE pk.SOID = so.ID AND pk.FinancialYearID = @FinancialYearID
                       AND pk.Quarter = @Quarter AND ISNULL(pk.IsActive,1)=1), 0) AS INT),

        -- 12. Competitor Feedback Target / Actual
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Compititor Feedback'
                              THEN dk.TargetValue END), 0) AS INT),
        CAST(ISNULL((SELECT SUM(pk.CompFeed)
                     FROM dbo.Tbl_SOProdKnowledgeCompFeed pk
                     WHERE pk.SOID = so.ID AND pk.FinancialYearID = @FinancialYearID
                       AND pk.Quarter = @Quarter AND ISNULL(pk.IsActive,1)=1), 0) AS INT)

    FROM       #SOs           s
    INNER JOIN dbo.SaleOfficers so ON so.ID = s.SOID
    -- Join KPI master for this SO (may not exist if targets not set)
    LEFT JOIN  dbo.Tbl_MasterKPIS mk ON mk.SOID     = so.ID
                                     AND mk.DateFrom  = @StartDate
                                     AND mk.DateTo    = @EndDate
                                     AND ISNULL(mk.IsActive, 1) = 1
    LEFT JOIN  dbo.Tbl_DetailKPI  dk ON dk.KPIMasterID = mk.ID
    GROUP BY so.ID;

    -- ── 4. CCR Data (Call Summary SO-wise) ──────────────────────────────────────
    -- Calls are linked via Tbl_AllPurposeVisits.SOID or via the Jobs/HousingVisits
    -- The existing sp_GetCallSummaryReport_Simple groups by Caller (SO).
    -- We replicate the same logic here for the SO scope.
    CREATE TABLE #CCRData (
        SOID                    INT,
        TotalCalls              INT,
        CallAttended            INT,
        NotAttended             INT,
        WrongData               INT,
        OwnerCount              INT,
        PaintContractorCount    INT,
        ConstContractorCount    INT,
        CompetitorCount         INT,
        ReadyToPaintCount       INT,
        ConstStageOtherCount    INT,
        ApplyingBrightoCount    INT
    );

    -- CCR comes from housing visits classified by their construction stage / contact type.
    -- ConstructionStageID maps: check Tbl_ConstructionStage names.
    -- For Call data we use Tbl_HousingVisits in the date range, SO-wise.
    INSERT INTO #CCRData
    SELECT
        s.SOID,
        -- Total Calls = all housing visits in range
        COUNT(hv.ID)                                                       AS TotalCalls,
        -- Call Attended = visits where actual contact made (OnlineOffline = 1)
        SUM(CASE WHEN ISNULL(hv.OnlineOffline,0) = 1 THEN 1 ELSE 0 END)   AS CallAttended,
        -- Not Attended = offline / no contact
        SUM(CASE WHEN ISNULL(hv.OnlineOffline,0) = 0 THEN 1 ELSE 0 END)   AS NotAttended,
        -- Wrong Data = visits with null/zero customer
        SUM(CASE WHEN hv.CustomerID IS NULL THEN 1 ELSE 0 END)             AS WrongData,
        -- Owner = CustomerType 'Owner' on the linked Retailer
        SUM(CASE WHEN r.CustomerType = 'Owner'             THEN 1 ELSE 0 END) AS OwnerCount,
        -- Paint Contractor
        SUM(CASE WHEN r.CustomerType = 'Paint Contractor'  THEN 1 ELSE 0 END) AS PaintContractorCount,
        -- Const Contractor
        SUM(CASE WHEN r.CustomerType = 'Const Contractor'  THEN 1 ELSE 0 END) AS ConstContractorCount,
        -- Competitor
        SUM(CASE WHEN r.CustomerType = 'Competitor'        THEN 1 ELSE 0 END) AS CompetitorCount,
        -- Ready to Paint (ConstructionStage name match)
        SUM(CASE WHEN cs.Name LIKE '%Ready%Paint%'         THEN 1 ELSE 0 END) AS ReadyToPaintCount,
        -- Construction Stage Other
        SUM(CASE WHEN cs.Name LIKE '%Other%'               THEN 1 ELSE 0 END) AS ConstStageOtherCount,
        -- Applying Brighto
        SUM(CASE WHEN cs.Name LIKE '%Applying%Brighto%'
              OR cs.Name LIKE '%Applied%Brighto%'          THEN 1 ELSE 0 END) AS ApplyingBrightoCount
    FROM       #SOs            s
    LEFT JOIN  dbo.Tbl_HousingVisits hv ON hv.SOID = s.SOID
                                        AND CAST(hv.CreatedAt AS DATE) BETWEEN @StartDate AND @EndDate
                                        AND ISNULL(hv.IsActive, 1) = 1
    LEFT JOIN  dbo.Retailers   r   ON r.ID  = hv.CustomerID
    LEFT JOIN  dbo.Tbl_ConstructionStage cs ON cs.ID = hv.ConstructionStageID
    GROUP BY s.SOID;

    -- ── Final SELECT ─────────────────────────────────────────────────────────────
    SELECT
        -- Employee Profile
        s.SOID,
        s.SOName                    AS EmployeeName,
        s.ECode                     AS EmployeeCode,
        s.RegionalHeadID,
        s.HeadName                  AS RegionalHead,
        s.RegionName                AS Region,
        s.PrimaryTown,
        s.SecondaryTown,
        @StartDate                  AS PeriodFrom,
        @EndDate                    AS PeriodTo,

        -- Customer Profile
        ISNULL(cd.TotalCustomers,  0) AS TotalCustomers,
        ISNULL(cd.ActiveCount,     0) AS ActiveCustomers,
        ISNULL(cd.LostCount,       0) AS LostCustomers,
        ISNULL(cd.CompletedCount,  0) AS CompletedCustomers,
        ISNULL(cd.ResidentialCount,0) AS ResidentialCustomers,
        ISNULL(cd.CommercialCount, 0) AS CommercialCustomers,
        ISNULL(cd.NewCustomers,    0) AS NewCustomers,
        ISNULL(cd.SitesWon,        0) AS SitesWon,
        ISNULL(cd.OnlineVisits,    0) AS OnlineVisits,
        ISNULL(cd.OfflineVisits,   0) AS OfflineVisits,

        -- KPI Data
        ISNULL(k.SalesTarget,           0) AS KPI_SalesTarget,
        ISNULL(k.SalesActual,           0) AS KPI_SalesActual,
        ISNULL(k.PlatinumTarget,        0) AS KPI_PlatinumTarget,
        ISNULL(k.PlatinumActual,        0) AS KPI_PlatinumActual,
        ISNULL(k.PremiumTarget,         0) AS KPI_PremiumTarget,
        ISNULL(k.PremiumActual,         0) AS KPI_PremiumActual,
        ISNULL(k.DealerVisitsTarget,    0) AS KPI_DealerVisitTarget,
        ISNULL(k.DealerVisitsActual,    0) AS KPI_DealerVisitActual,
        ISNULL(k.SiteVisitsTarget,      0) AS KPI_SiteVisitsTarget,
        ISNULL(k.SiteVisitsActual,      0) AS KPI_SiteVisitsActual,
        ISNULL(k.ContractorVisitsTarget,0) AS KPI_ContractorVisitsTarget,
        ISNULL(k.ContractorVisitsActual,0) AS KPI_ContractorVisitsActual,
        ISNULL(k.CustSatisfactionTarget,0) AS KPI_CustSatisfactionTarget,
        ISNULL(k.CustSatisfactionActual,0) AS KPI_CustSatisfactionActual,
        ISNULL(k.AreaCoverageTarget,    0) AS KPI_AreaCoverageTarget,
        ISNULL(k.AreaCoverageActual,    0) AS KPI_AreaCoverageActual,
        ISNULL(k.AttendanceTarget,      0) AS KPI_AttendanceTarget,
        ISNULL(k.AttendanceActual,      0) AS KPI_AttendanceActual,
        ISNULL(k.TrainingTarget,        0) AS KPI_TrainingTarget,
        ISNULL(k.TrainingActual,        0) AS KPI_TrainingActual,
        ISNULL(k.ProdKnowTarget,        0) AS KPI_ProdKnowTarget,
        ISNULL(k.ProdKnowActual,        0) AS KPI_ProdKnowActual,
        ISNULL(k.CompFeedTarget,        0) AS KPI_CompFeedTarget,
        ISNULL(k.CompFeedActual,        0) AS KPI_CompFeedActual,

        -- CCR Data
        ISNULL(ccr.TotalCalls,           0) AS CCR_TotalCalls,
        ISNULL(ccr.CallAttended,         0) AS CCR_CallAttended,
        ISNULL(ccr.NotAttended,          0) AS CCR_NotAttended,
        ISNULL(ccr.WrongData,            0) AS CCR_WrongData,
        ISNULL(ccr.OwnerCount,           0) AS CCR_Owner,
        ISNULL(ccr.PaintContractorCount, 0) AS CCR_PaintContractor,
        ISNULL(ccr.ConstContractorCount, 0) AS CCR_ConstContractor,
        ISNULL(ccr.CompetitorCount,      0) AS CCR_Competitor,
        ISNULL(ccr.ReadyToPaintCount,    0) AS CCR_ReadyToPaint,
        ISNULL(ccr.ConstStageOtherCount, 0) AS CCR_ConstStageOther,
        ISNULL(ccr.ApplyingBrightoCount, 0) AS CCR_ApplyingBrighto

    FROM       #SOs          s
    LEFT JOIN  #CustomerData cd  ON cd.SOID  = s.SOID
    LEFT JOIN  #KPIData      k   ON k.SOID   = s.SOID
    LEFT JOIN  #CCRData      ccr ON ccr.SOID = s.SOID
    ORDER BY s.HeadName, s.SOName;

    -- Cleanup
    DROP TABLE #SOs;
    DROP TABLE #CustomerData;
    DROP TABLE #KPIData;
    DROP TABLE #CCRData;

END
GO

-- ── Test ─────────────────────────────────────────────────────────────────────
-- All SOs, Q1 of FY 1:
-- EXEC dbo.usp_GetEmployeePerformanceReport
--     @SOID = 0, @RegionalHeadID = 0,
--     @StartDate = '2025-07-01', @EndDate = '2025-09-30',
--     @FinancialYearID = 1, @Quarter = 'Q1'
--
-- Single SO:
-- EXEC dbo.usp_GetEmployeePerformanceReport
--     @SOID = 5, @RegionalHeadID = 0,
--     @StartDate = '2025-07-01', @EndDate = '2025-09-30',
--     @FinancialYearID = 1, @Quarter = 'Q1'
