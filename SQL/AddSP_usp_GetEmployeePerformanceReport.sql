-- =============================================
-- AddSP_usp_GetEmployeePerformanceReport.sql
--
-- What changed and why:
--   This SP was authored in a previous (unmerged) session and lived only in an
--   old git worktree — it was never copied into the main repo's SQL/ folder and
--   was never deployed to local dev (ABUZAR/Brighto), even though
--   FOS.Web.UI/Controllers/SetupController.cs (action EmployeePerformanceReportPrint,
--   already on main) calls it with @SOID, @RegionalHeadID, @FinancialYearID, @Quarter.
--   That made the controller action broken on main. This script brings the SP
--   into the main repo and applies it, using CREATE OR ALTER so it is idempotent.
--
--   Verification against the live ABUZAR/Brighto schema found all referenced
--   tables/columns exist (SaleOfficers, RegionalHeads, Regions, Cities incl.
--   PrimaryCityID/SecondaryCityID, Retailers, BusinessStatus, Tbl_ConstructionStage,
--   Tbl_HousingVisits, Tbl_TradeVisitsFinal, Tbl_AllPurposeVisits, Tbl_MasterKPIS,
--   Tbl_DetailKPI, Tbl_SalesClaimMaster, Tbl_ClaimDetail, Tbl_ProductDetail,
--   Tbl_SOAttendanceandPunctuality, Tbl_SOTraining) EXCEPT for one real bug found
--   and fixed here:
--
--     * dbo.Tbl_DetailKPI.FocusArea stores the value 'Business Affiliates Visit
--       Target' (plural "Affiliates" — matches the app's actual domain wording,
--       see BusinessAffiliatesVisitsController.cs etc.), but this SP (copied from
--       the same pattern as usp_GetKPIPercentageReport / usp_GetKPIExcelReport)
--       was comparing against the singular 'Business Affiliate Visit Target',
--       which never matches any row, so that KPI's Target always resolved to 0.
--       Fixed the literal to the plural form used by the real data. NOTE: the
--       sibling procs usp_GetKPIPercentageReport.sql and usp_GetKPIExcelReport.sql
--       have the exact same singular/plural mismatch and were NOT touched here
--       (out of scope for this task) — flagging for a follow-up fix.
--
--   No other column/table name mismatches were found. The hardcoded actual
--   values of 1 for ContractorVisitsActual / CustSatisfactionActual /
--   AreaCoverageActual are left as-is (matches the same "always full weight,
--   no real actual source" pattern used in usp_GetKPIPercentageReport for the
--   same three focus areas — not invented here).
--
--   Prerequisite objects (already committed to main, just not yet applied to
--   this local dev DB) were applied before this script so the SP can actually
--   run: dbo.Tbl_FinancialYear, dbo.Tbl_Quarters, dbo.Tbl_SOProdKnowledgeCompFeed,
--   and the FinancialYearID/Quarter columns on Tbl_SOAttendanceandPunctuality
--   and Tbl_SOTraining (see CreateTbl_FinancialYear.sql, CreateTbl_Quarters.sql,
--   CreateTbl_SOProdKnowledgeCompFeed.sql, AlterSOAttendanceAndTraining_AddFYQuarter.sql).
--
-- QA fix (revision 2):
--   When Tbl_Quarters has no matching row for the given @FinancialYearID/@Quarter,
--   the SP used to return a single-column 'ErrorMessage' result set. The consumer,
--   SetupController.EmployeePerformanceReportPrint, reads columns by name
--   (reader["SOID"], reader["EmployeeName"], etc.) with no fallback, so that shape
--   mismatch threw an unhandled IndexOutOfRangeException and showed a raw error
--   page instead of an empty report. Fixed by replacing the ErrorMessage SELECT
--   with a `SELECT TOP (0) ...` that has the EXACT same column names/order as the
--   final SELECT at the bottom of this proc, so the ADO.NET reader just gets a
--   valid, empty (0-row) result set and the C# loop naturally does nothing — no
--   controller changes required.
--
-- Data-integrity fix (revision 3):
--   The #CCRData section ("4. CCR Data") was built directly off Tbl_HousingVisits
--   joined to Retailers/Tbl_ConstructionStage. That is the EXACT SAME source table
--   already used for SiteVisitsActual in #KPIData, so CCR_TotalCalls always
--   duplicated KPI_SiteVisitsActual (confirmed: both = 192 for SOID 416, FY1 Q1).
--   In addition, CCR_Owner/CCR_PaintContractor/CCR_ConstContractor/CCR_Competitor
--   were checking Retailers.CustomerType (which never holds those values) and
--   CCR_ConstStageOther/CCR_ApplyingBrighto were checking Tbl_ConstructionStage.Name
--   via LIKE (which never matched either) — so those columns were always 0.
--
--   Fixed by sourcing #CCRData from dbo.Tbl_SaveCall (the real call-log table,
--   confirmed against the already-deployed, working dbo.sp_GetCallSummaryReport_Simple
--   used by the existing Call Summary Detailed Report feature) INNER JOINed to
--   dbo.Tbl_HousingVisits ON sc.VisitID = ho.ID to resolve ho.SOID, wrapped in a
--   LEFT JOIN from #SOs so SOs with zero calls in the period still return a row of
--   zeros instead of being dropped. The CASE/SUM value mapping (CustomerType /
--   ConstructionStage / SiteStatus / NatureOfCall literals) mirrors
--   sp_GetCallSummaryReport_Simple exactly, just grouped by SOID instead of
--   CallerName. Filtered on TRY_CONVERT(DATE, sc.CallDate) BETWEEN @StartDate AND
--   @EndDate and ISNULL(sc.IsActive,1)=1 (real data: 7373 rows IsActive=1, 1 row
--   NULL, 0 rows =0 — filter is a no-op today but correct/defensive going forward).
--
--   One extra guard vs. the reference proc: sp_GetCallSummaryReport_Simple uses all
--   INNER JOINs so a GROUP BY only ever sees rows for SOs that actually have calls.
--   Here we LEFT JOIN so zero-call SOs survive, which means the "OR ... IS NULL"
--   branch used for ConstStageOtherCount (mirroring [ConstructionStageOther] in the
--   reference proc) would incorrectly fire for the phantom LEFT JOIN row of a
--   zero-call SO. Added an explicit `sc.ID IS NOT NULL AND (...)` guard on that one
--   column so zero-call SOs correctly get ConstStageOtherCount = 0, not 1.
--
--   NOTE (flagging, not fixed here — out of scope): sp_GetCallSummaryReport_Simple's
--   Competitor check uses the literal 'Competitor', but the real Tbl_SaveCall.SiteStatus
--   data uses the misspelled 'Compititor'. That mismatch is mirrored as-is from the
--   reference proc per this task's instructions, so CCR_Competitor will read 0 even
--   though "Compititor" rows exist. Also left the Tbl_ConstructionStage join out of
--   the #CCRData section entirely (no longer needed); the #CustomerData section's
--   own Retailers/BusinessStatus joins are untouched (unrelated, out of scope).
--
-- Data-integrity fix (revision 4, this revision):
--   The #CustomerData column "SitesWon" was computed as a straight count of
--   Tbl_HousingVisits rows in the quarter for the SO — that is the exact same
--   source/semantics already used for KPI_SiteVisitsActual (#KPIData), so the
--   column was a pure duplicate and didn't represent anything called "Sites Won".
--   Confirmed with the user that this column is actually meant to represent
--   "All Unique Claim Customers" — the count of DISTINCT customers who had a
--   sales claim submitted by that SO within the period. Re-sourced it from
--   dbo.Tbl_SalesClaimMaster (COUNT(DISTINCT CustomerID)), filtered the same way
--   SalesActual already filters that table elsewhere in this proc
--   (SOID = SO, DateSelected BETWEEN @StartDate/@EndDate, ISNULL(IsActive,1)=1)
--   for consistency. The column alias "SitesWon" itself is intentionally left
--   unchanged in this script — renaming the DTO property / view label is a
--   follow-up backend/frontend task, out of scope here.
--
-- Sections:
--   1. Employee Profile  -> SaleOfficers + RegionalHeads + Regions + Cities
--   2. Customer Data     -> Retailers assigned to SO, BusinessStatus, Housing
--   3. KPI Data          -> Tbl_MasterKPIS / Tbl_DetailKPI (same source as KPIPerformanceReport)
--   4. CCR Data          -> Tbl_SaveCall joined to Tbl_HousingVisits SO-wise (same source as
--                            CallSummaryDetailedReport / sp_GetCallSummaryReport_Simple)
--
-- Parameters:
--   @SOID            INT           -- 0 = all SOs under the head
--   @RegionalHeadID  INT           -- 0 = all heads in the zone
--   @FinancialYearID INT
--   @Quarter         NVARCHAR(20)  -- e.g. 'Q1', 'Q2', 'Q3', 'Q4'
--
-- Date range is resolved automatically from Tbl_Quarters.
--
-- Exec dbo.usp_GetEmployeePerformanceReport @SOID = 0, @RegionalHeadID = 0, @FinancialYearID = 1, @Quarter = 'Q1'
-- =============================================
CREATE OR ALTER PROCEDURE dbo.usp_GetEmployeePerformanceReport
    @SOID            INT          = 0,
    @RegionalHeadID  INT          = 0,
    @FinancialYearID INT,
    @Quarter         NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- ── Resolve Quarter to date range (same pattern as usp_GetKPIExcelReport) ───
    DECLARE @StartDate DATE, @EndDate DATE;

    SELECT TOP 1
        @StartDate = StartDate,
        @EndDate   = EndDate
    FROM dbo.Tbl_Quarters
    WHERE FinancialYearID        = @FinancialYearID
      AND Name                   = @Quarter
      AND ISNULL(IsDeleted, 0)  = 0
      AND ISNULL(IsActive,  1)  = 1;

    IF @StartDate IS NULL
    BEGIN
        -- No matching quarter/date range found for the given FinancialYearID/Quarter.
        -- Return ZERO rows but with the EXACT same column names/order as the final
        -- SELECT below, so the ADO.NET reader in
        -- SetupController.EmployeePerformanceReportPrint (which reads columns by
        -- name, e.g. reader["SOID"], reader["EmployeeName"], ...) gets a valid,
        -- empty result set instead of throwing IndexOutOfRangeException against a
        -- 1-column ErrorMessage result.
        SELECT TOP (0)
            CAST(NULL AS INT)           AS SOID,
            CAST(NULL AS NVARCHAR(200)) AS EmployeeName,
            CAST(NULL AS NVARCHAR(100)) AS EmployeeCode,
            CAST(NULL AS INT)           AS RegionalHeadID,
            CAST(NULL AS NVARCHAR(200)) AS RegionalHead,
            CAST(NULL AS NVARCHAR(200)) AS Region,
            CAST(NULL AS NVARCHAR(200)) AS PrimaryTown,
            CAST(NULL AS NVARCHAR(200)) AS SecondaryTown,
            CAST(NULL AS INT)           AS FinancialYearID,
            CAST(NULL AS NVARCHAR(20))  AS Quarter,
            CAST(NULL AS DATE)          AS PeriodFrom,
            CAST(NULL AS DATE)          AS PeriodTo,
            CAST(NULL AS INT)           AS TotalCustomers,
            CAST(NULL AS INT)           AS ActiveCustomers,
            CAST(NULL AS INT)           AS LostCustomers,
            CAST(NULL AS INT)           AS CompletedCustomers,
            CAST(NULL AS INT)           AS ResidentialCustomers,
            CAST(NULL AS INT)           AS CommercialCustomers,
            CAST(NULL AS INT)           AS NewCustomers,
            CAST(NULL AS INT)           AS SitesWon,
            CAST(NULL AS INT)           AS OnlineVisits,
            CAST(NULL AS INT)           AS OfflineVisits,
            CAST(NULL AS INT)           AS KPI_SalesTarget,
            CAST(NULL AS INT)           AS KPI_SalesActual,
            CAST(NULL AS INT)           AS KPI_PlatinumTarget,
            CAST(NULL AS INT)           AS KPI_PlatinumActual,
            CAST(NULL AS INT)           AS KPI_PremiumTarget,
            CAST(NULL AS INT)           AS KPI_PremiumActual,
            CAST(NULL AS INT)           AS KPI_DealerVisitTarget,
            CAST(NULL AS INT)           AS KPI_DealerVisitActual,
            CAST(NULL AS INT)           AS KPI_SiteVisitsTarget,
            CAST(NULL AS INT)           AS KPI_SiteVisitsActual,
            CAST(NULL AS INT)           AS KPI_ContractorVisitsTarget,
            CAST(NULL AS INT)           AS KPI_ContractorVisitsActual,
            CAST(NULL AS INT)           AS KPI_CustSatisfactionTarget,
            CAST(NULL AS INT)           AS KPI_CustSatisfactionActual,
            CAST(NULL AS INT)           AS KPI_AreaCoverageTarget,
            CAST(NULL AS INT)           AS KPI_AreaCoverageActual,
            CAST(NULL AS INT)           AS KPI_AttendanceTarget,
            CAST(NULL AS INT)           AS KPI_AttendanceActual,
            CAST(NULL AS INT)           AS KPI_TrainingTarget,
            CAST(NULL AS INT)           AS KPI_TrainingActual,
            CAST(NULL AS INT)           AS KPI_ProdKnowTarget,
            CAST(NULL AS INT)           AS KPI_ProdKnowActual,
            CAST(NULL AS INT)           AS KPI_CompFeedTarget,
            CAST(NULL AS INT)           AS KPI_CompFeedActual,
            CAST(NULL AS INT)           AS CCR_TotalCalls,
            CAST(NULL AS INT)           AS CCR_CallAttended,
            CAST(NULL AS INT)           AS CCR_NotAttended,
            CAST(NULL AS INT)           AS CCR_WrongData,
            CAST(NULL AS INT)           AS CCR_Owner,
            CAST(NULL AS INT)           AS CCR_PaintContractor,
            CAST(NULL AS INT)           AS CCR_ConstContractor,
            CAST(NULL AS INT)           AS CCR_Competitor,
            CAST(NULL AS INT)           AS CCR_ReadyToPaint,
            CAST(NULL AS INT)           AS CCR_ConstStageOther,
            CAST(NULL AS INT)           AS CCR_ApplyingBrighto;
        RETURN;
    END

    -- ── 1. Candidate SOs ────────────────────────────────────────────────────────
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
        ISNULL(rh.ID, 0),
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
      AND  (@SOID           = 0 OR so.ID             = @SOID)
      AND  (@RegionalHeadID = 0 OR so.RegionalHeadID = @RegionalHeadID);

    -- ── 2. Customer / Retailer Counts (per SO) ──────────────────────────────────
    CREATE TABLE #CustomerData (
        SOID             INT,
        TotalCustomers   INT,
        ActiveCount      INT,
        LostCount        INT,
        CompletedCount   INT,
        ResidentialCount INT,
        CommercialCount  INT,
        NewCustomers     INT,
        SitesWon         INT,
        OnlineVisits     INT,
        OfflineVisits    INT
    );

    INSERT INTO #CustomerData
    SELECT
        s.SOID,
        COUNT(DISTINCT r.ID)                                                        AS TotalCustomers,
        SUM(CASE WHEN bs.Name LIKE '%Active%'
                  AND ISNULL(bs.IsActive,1) = 1 THEN 1 ELSE 0 END)                AS ActiveCount,
        SUM(CASE WHEN bs.Name LIKE '%Lost%'    THEN 1 ELSE 0 END)                  AS LostCount,
        SUM(CASE WHEN bs.Name LIKE '%Complet%' THEN 1 ELSE 0 END)                  AS CompletedCount,
        SUM(CASE WHEN r.CustomerType = 'Residential' THEN 1 ELSE 0 END)            AS ResidentialCount,
        SUM(CASE WHEN r.CustomerType = 'Commercial'  THEN 1 ELSE 0 END)            AS CommercialCount,
        -- New customers registered within the quarter
        SUM(CASE WHEN CAST(r.CreatedDate AS DATE) BETWEEN @StartDate
                                                       AND @EndDate THEN 1 ELSE 0 END) AS NewCustomers,
        -- All Unique Claim Customers = distinct customers with a sales claim in the
        -- period for this SO (column alias kept as "SitesWon" — see revision 4 note
        -- above; renaming the DTO/label is a follow-up backend/frontend task).
        ISNULL((SELECT COUNT(DISTINCT c.CustomerID)
                FROM   dbo.Tbl_SalesClaimMaster c
                WHERE  c.SOID = s.SOID
                  AND  c.DateSelected >= @StartDate
                  AND  c.DateSelected <= @EndDate
                  AND  ISNULL(c.IsActive, 1) = 1), 0)                              AS SitesWon,
        -- Online visits (AllPurpose) in the quarter
        ISNULL((SELECT COUNT(*)
                FROM   dbo.Tbl_AllPurposeVisits apv
                WHERE  apv.SOID = s.SOID
                  AND  CAST(apv.CreatedOn AS DATE) BETWEEN @StartDate AND @EndDate
                  AND  apv.OnlineOffline = 1
                  AND  ISNULL(apv.IsActive, 1) = 1), 0)                            AS OnlineVisits,
        -- Offline visits (AllPurpose) in the quarter
        ISNULL((SELECT COUNT(*)
                FROM   dbo.Tbl_AllPurposeVisits apv
                WHERE  apv.SOID = s.SOID
                  AND  CAST(apv.CreatedOn AS DATE) BETWEEN @StartDate AND @EndDate
                  AND  apv.OnlineOffline = 0
                  AND  ISNULL(apv.IsActive, 1) = 1), 0)                            AS OfflineVisits
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

        -- 6. Contractor (Business Affiliate) Visits Target / Actual
        -- NOTE: FocusArea fixed to plural 'Business Affiliates Visit Target' to
        -- match the actual value stored in Tbl_DetailKPI (was singular 'Business
        -- Affiliate Visit Target' in the original draft, which never matched).
        CAST(ISNULL(MAX(CASE WHEN dk.FocusArea = 'Business Affiliates Visit Target'
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

    FROM       #SOs            s
    INNER JOIN dbo.SaleOfficers so ON so.ID = s.SOID
    LEFT JOIN  dbo.Tbl_MasterKPIS mk ON mk.SOID    = so.ID
                                     AND mk.DateFrom = @StartDate
                                     AND mk.DateTo   = @EndDate
                                     AND ISNULL(mk.IsActive, 1) = 1
    LEFT JOIN  dbo.Tbl_DetailKPI  dk ON dk.KPIMasterID = mk.ID
    GROUP BY so.ID;

    -- ── 4. CCR Data — Tbl_SaveCall SO-wise (same source as CallSummaryDetailedReport /
    --      dbo.sp_GetCallSummaryReport_Simple; joined via Tbl_HousingVisits to resolve
    --      SOID, since Tbl_SaveCall has no direct SOID column) ─────────────────────────
    CREATE TABLE #CCRData (
        SOID                 INT,
        TotalCalls           INT,
        CallAttended         INT,
        NotAttended          INT,
        WrongData            INT,
        OwnerCount           INT,
        PaintContractorCount INT,
        ConstContractorCount INT,
        CompetitorCount      INT,
        ReadyToPaintCount    INT,
        ConstStageOtherCount INT,
        ApplyingBrightoCount INT
    );

    INSERT INTO #CCRData
    SELECT
        s.SOID,
        COUNT(sc.ID)                                                                        AS TotalCalls,
        SUM(CASE WHEN sc.NatureOfCall IN ('PartialFeedback', 'CompleteFeedback')
                 THEN 1 ELSE 0 END)                                                         AS CallAttended,
        SUM(CASE WHEN sc.NatureOfCall IN ('NotAnswering/Busy', 'NotResponding', 'Poweredoff')
                 THEN 1 ELSE 0 END)                                                         AS NotAttended,
        SUM(CASE WHEN sc.NatureOfCall = 'Wrong/incompleteData'
                 THEN 1 ELSE 0 END)                                                         AS WrongData,
        SUM(CASE WHEN sc.CustomerType = 'Owner'
                 THEN 1 ELSE 0 END)                                                         AS OwnerCount,
        SUM(CASE WHEN sc.CustomerType = 'PaintContractor'
                 THEN 1 ELSE 0 END)                                                         AS PaintContractorCount,
        SUM(CASE WHEN sc.CustomerType = 'ConstructionContractor'
                 THEN 1 ELSE 0 END)                                                         AS ConstContractorCount,
        SUM(CASE WHEN sc.SiteStatus = 'Competitor'
                 THEN 1 ELSE 0 END)                                                         AS CompetitorCount,
        SUM(CASE WHEN sc.ConstructionStage = 'ReadyToPaint'
                 THEN 1 ELSE 0 END)                                                         AS ReadyToPaintCount,
        -- Guard sc.ID IS NOT NULL so a zero-call SO (LEFT JOIN produces a NULL sc row)
        -- doesn't get incorrectly counted as 1 via the "OR ... IS NULL" branch below.
        SUM(CASE WHEN sc.ID IS NOT NULL
                  AND (sc.ConstructionStage != 'ReadyToPaint' OR sc.ConstructionStage IS NULL)
                 THEN 1 ELSE 0 END)                                                         AS ConstStageOtherCount,
        SUM(CASE WHEN sc.SiteStatus = 'ApplyingBrighto'
                 THEN 1 ELSE 0 END)                                                         AS ApplyingBrightoCount
    FROM       #SOs                    s
    LEFT JOIN (dbo.Tbl_SaveCall          sc
               INNER JOIN dbo.Tbl_HousingVisits ho ON sc.VisitID = ho.ID)
                                       ON ho.SOID = s.SOID
                                      AND TRY_CONVERT(DATE, sc.CallDate) BETWEEN @StartDate AND @EndDate
                                      AND ISNULL(sc.IsActive, 1) = 1
    GROUP BY s.SOID;

    -- ── Final SELECT ─────────────────────────────────────────────────────────────
    SELECT
        -- Employee Profile
        s.SOID,
        s.SOName                     AS EmployeeName,
        s.ECode                      AS EmployeeCode,
        s.RegionalHeadID,
        s.HeadName                   AS RegionalHead,
        s.RegionName                 AS Region,
        s.PrimaryTown,
        s.SecondaryTown,
        @FinancialYearID             AS FinancialYearID,
        @Quarter                     AS Quarter,
        @StartDate                   AS PeriodFrom,
        @EndDate                     AS PeriodTo,

        -- Customer Profile
        ISNULL(cd.TotalCustomers,   0) AS TotalCustomers,
        ISNULL(cd.ActiveCount,      0) AS ActiveCustomers,
        ISNULL(cd.LostCount,        0) AS LostCustomers,
        ISNULL(cd.CompletedCount,   0) AS CompletedCustomers,
        ISNULL(cd.ResidentialCount, 0) AS ResidentialCustomers,
        ISNULL(cd.CommercialCount,  0) AS CommercialCustomers,
        ISNULL(cd.NewCustomers,     0) AS NewCustomers,
        ISNULL(cd.SitesWon,         0) AS SitesWon,
        ISNULL(cd.OnlineVisits,     0) AS OnlineVisits,
        ISNULL(cd.OfflineVisits,    0) AS OfflineVisits,

        -- KPI Data
        ISNULL(k.SalesTarget,            0) AS KPI_SalesTarget,
        ISNULL(k.SalesActual,            0) AS KPI_SalesActual,
        ISNULL(k.PlatinumTarget,         0) AS KPI_PlatinumTarget,
        ISNULL(k.PlatinumActual,         0) AS KPI_PlatinumActual,
        ISNULL(k.PremiumTarget,          0) AS KPI_PremiumTarget,
        ISNULL(k.PremiumActual,          0) AS KPI_PremiumActual,
        ISNULL(k.DealerVisitsTarget,     0) AS KPI_DealerVisitTarget,
        ISNULL(k.DealerVisitsActual,     0) AS KPI_DealerVisitActual,
        ISNULL(k.SiteVisitsTarget,       0) AS KPI_SiteVisitsTarget,
        ISNULL(k.SiteVisitsActual,       0) AS KPI_SiteVisitsActual,
        ISNULL(k.ContractorVisitsTarget, 0) AS KPI_ContractorVisitsTarget,
        ISNULL(k.ContractorVisitsActual, 0) AS KPI_ContractorVisitsActual,
        ISNULL(k.CustSatisfactionTarget, 0) AS KPI_CustSatisfactionTarget,
        ISNULL(k.CustSatisfactionActual, 0) AS KPI_CustSatisfactionActual,
        ISNULL(k.AreaCoverageTarget,     0) AS KPI_AreaCoverageTarget,
        ISNULL(k.AreaCoverageActual,     0) AS KPI_AreaCoverageActual,
        ISNULL(k.AttendanceTarget,       0) AS KPI_AttendanceTarget,
        ISNULL(k.AttendanceActual,       0) AS KPI_AttendanceActual,
        ISNULL(k.TrainingTarget,         0) AS KPI_TrainingTarget,
        ISNULL(k.TrainingActual,         0) AS KPI_TrainingActual,
        ISNULL(k.ProdKnowTarget,         0) AS KPI_ProdKnowTarget,
        ISNULL(k.ProdKnowActual,         0) AS KPI_ProdKnowActual,
        ISNULL(k.CompFeedTarget,         0) AS KPI_CompFeedTarget,
        ISNULL(k.CompFeedActual,         0) AS KPI_CompFeedActual,

        -- CCR Data
        ISNULL(ccr.TotalCalls,            0) AS CCR_TotalCalls,
        ISNULL(ccr.CallAttended,          0) AS CCR_CallAttended,
        ISNULL(ccr.NotAttended,           0) AS CCR_NotAttended,
        ISNULL(ccr.WrongData,             0) AS CCR_WrongData,
        ISNULL(ccr.OwnerCount,            0) AS CCR_Owner,
        ISNULL(ccr.PaintContractorCount,  0) AS CCR_PaintContractor,
        ISNULL(ccr.ConstContractorCount,  0) AS CCR_ConstContractor,
        ISNULL(ccr.CompetitorCount,       0) AS CCR_Competitor,
        ISNULL(ccr.ReadyToPaintCount,     0) AS CCR_ReadyToPaint,
        ISNULL(ccr.ConstStageOtherCount,  0) AS CCR_ConstStageOther,
        ISNULL(ccr.ApplyingBrightoCount,  0) AS CCR_ApplyingBrighto

    FROM       #SOs          s
    LEFT JOIN  #CustomerData cd  ON cd.SOID  = s.SOID
    LEFT JOIN  #KPIData      k   ON k.SOID   = s.SOID
    LEFT JOIN  #CCRData      ccr ON ccr.SOID = s.SOID
    ORDER BY s.HeadName, s.SOName;

    DROP TABLE #SOs;
    DROP TABLE #CustomerData;
    DROP TABLE #KPIData;
    DROP TABLE #CCRData;

END
GO

-- ── Test ─────────────────────────────────────────────────────────────────────
-- All SOs, Q1:
-- EXEC dbo.usp_GetEmployeePerformanceReport
--     @SOID = 0, @RegionalHeadID = 0, @FinancialYearID = 1, @Quarter = 'Q1'
--
-- Single SO:
-- EXEC dbo.usp_GetEmployeePerformanceReport
--     @SOID = 5, @RegionalHeadID = 0, @FinancialYearID = 1, @Quarter = 'Q1'
--
-- SO known to have Tbl_SaveCall data (smoke test for the CCR fix):
-- EXEC dbo.usp_GetEmployeePerformanceReport
--     @SOID = 416, @RegionalHeadID = 0, @FinancialYearID = 1, @Quarter = 'Q1'
--
-- SO known to have Tbl_SalesClaimMaster data (smoke test for the SitesWon /
-- "All Unique Claim Customers" fix, revision 4):
-- EXEC dbo.usp_GetEmployeePerformanceReport
--     @SOID = 60, @RegionalHeadID = 0, @FinancialYearID = 1, @Quarter = 'Q1'
--
-- No matching quarter (must return 0 rows, ~57-column header, NOT an error/message row):
-- EXEC dbo.usp_GetEmployeePerformanceReport
--     @SOID = 0, @RegionalHeadID = 0, @FinancialYearID = 999, @Quarter = 'Q9'
