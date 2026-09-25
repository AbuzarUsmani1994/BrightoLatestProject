-- Powers CompetitorActivitiesDSRReportController (mobile PDF report), mirroring
-- usp_GetBusinessAffiliatesDSR's shape (@DateFrom, @DateTo, @SOID only - no
-- @RegionalHeadID like the older usp_GetCompetitorReport CSV export has).
-- Joins match the already-working query in
-- FOS.Web.UI\Controllers\API\GetCompetitorActivitiesController.cs /
-- usp_GetCompetitorReport.sql (Tbl_CompetitorActivities -> Tbl_CompetitorActivityTypes
-- / Tbl_CompititorList / Regions (ZoneID) / Cities / SaleOfficers).
IF OBJECT_ID('dbo.usp_GetCompetitorActivitiesDSR', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetCompetitorActivitiesDSR;
GO

CREATE PROCEDURE dbo.usp_GetCompetitorActivitiesDSR
    @DateFrom DATETIME,
    @DateTo   DATETIME,
    @SOID     INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ca.ID                                     AS ActivityID,
        ca.ActivityDate                           AS ActivityDate,
        so.ID                                     AS SOID,
        so.Name                                   AS SaleOfficerName,
        r.ID                                       AS RegionID,
        r.Name                                     AS RegionName,
        c.ID                                       AS CityID,
        c.Name                                     AS CityName,
        at.Name                                    AS ActivityType,
        cl.Name                                    AS CompetitorName,
        ca.Remarks                                 AS Remarks,
        ca.VideoPath                               AS VideoUrl,
        ca.CreatedOn                               AS CreatedDate
    FROM dbo.Tbl_CompetitorActivities        ca
    LEFT JOIN dbo.Tbl_CompetitorActivityTypes at  ON at.ID  = ca.ActivityTypeID
    LEFT JOIN dbo.Tbl_CompititorList          cl  ON cl.ID  = ca.CompetitorID
    LEFT JOIN dbo.Regions                     r   ON r.ID   = ca.ZoneID
    LEFT JOIN dbo.Cities                      c   ON c.ID   = ca.CityID
    LEFT JOIN dbo.SaleOfficers                so  ON so.ID  = ca.SOID
    WHERE ca.IsDeleted = 0
      AND ca.ActivityDate >= @DateFrom
      AND ca.ActivityDate <  @DateTo
      AND (@SOID = 0 OR ca.SOID = @SOID)
    ORDER BY ca.ActivityDate, so.Name;
END
GO
