-- =====================================================================================
-- Proc:    dbo.usp_GetCompetitorReport
-- Purpose: Powers the new MIS "Competitor Report" Excel/CSV export in ReportsController.
--          Returns one row per Competitor Activity submission.
--
-- Change history:
--   2026-09-01  New proc created (sql-agent). No existing proc/table changed.
--               Join syntax adapted from the existing working query in
--               FOS.Web.UI\Controllers\API\GetCompetitorActivitiesController.cs
--               (Tbl_CompetitorActivities -> Tbl_CompetitorActivityTypes / Tbl_CompititorList
--               / Regions (ZoneID) / Cities / SaleOfficers), plus:
--                 - RegionalHeads join via SaleOfficers.RegionalHeadID, same pattern/table
--                   as usp_GetBusinessAffiliateReport.sql and ReportsController.SaleOfficerDetailRpt
--                   ("@TID = 0 OR so.RegionalHeadID = @TID").
--                 - "submitted Date" = ca.CreatedOn (row insert timestamp), while date-range
--                   filtering uses ca.ActivityDate, matching the existing controller's own
--                   filter field.
--               Local dev note: Tbl_CompetitorActivities / Tbl_CompetitorActivityTypes did
--               not exist yet on ABUZAR\Brighto - applied SQL\CreateTbl_CompetitorActivities.sql
--               first (idempotent, safe to re-run) before creating/testing this proc.
--
-- Example test call:
--   EXEC dbo.usp_GetCompetitorReport
--        @DateFrom = '2020-01-01', @DateTo = '2030-01-01',
--        @SOID = 0, @RegionalHeadID = 0;
-- =====================================================================================
CREATE OR ALTER PROCEDURE dbo.usp_GetCompetitorReport
    @DateFrom       DATETIME,
    @DateTo         DATETIME,
    @SOID           INT = 0,
    @RegionalHeadID INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ca.CreatedOn                                                       AS SubmittedDate,
        rh.Name                                                            AS RegionalHead,
        so.Name                                                            AS SOName,
        r.Name                                                             AS Zone,
        c.Name                                                             AS City,
        at.Name                                                            AS ActivityType,
        cl.Name                                                            AS CompetitorName,
        ca.Remarks                                                         AS Remarks,
        ca.VideoPath                                                       AS VideoUrl
    FROM dbo.Tbl_CompetitorActivities        ca
    LEFT JOIN dbo.Tbl_CompetitorActivityTypes at  ON at.ID  = ca.ActivityTypeID
    LEFT JOIN dbo.Tbl_CompititorList          cl  ON cl.ID  = ca.CompetitorID
    LEFT JOIN dbo.Regions                     r   ON r.ID   = ca.ZoneID
    LEFT JOIN dbo.Cities                      c   ON c.ID   = ca.CityID
    LEFT JOIN dbo.SaleOfficers                so  ON so.ID  = ca.SOID
    LEFT JOIN dbo.RegionalHeads               rh  ON rh.ID  = so.RegionalHeadID
    WHERE ca.IsDeleted = 0
      AND ca.ActivityDate >= @DateFrom
      AND ca.ActivityDate <  @DateTo
      AND (@SOID = 0 OR ca.SOID = @SOID)
      AND (@RegionalHeadID = 0 OR so.RegionalHeadID = @RegionalHeadID)
    ORDER BY ca.ActivityDate DESC, ca.ID DESC;
END
GO

-- Exec dbo.usp_GetCompetitorReport @DateFrom='2020-01-01', @DateTo='2030-01-01', @SOID=0, @RegionalHeadID=0;
