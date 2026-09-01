-- =====================================================================================
-- Proc:    dbo.usp_GetBusinessAffiliateReport
-- Purpose: Powers the new MIS "Business Affiliate Report" Excel/CSV export in
--          ReportsController. Returns one row per Business Affiliate visit, joined
--          with the affiliate master and its lookup tables.
--
-- Change history:
--   2026-09-01  New proc created (sql-agent). No existing proc/table changed.
--               Reuses the same join pattern as SQL\usp_GetBusinessAffiliatesDSR.sql
--               (Tbl_BusinessAffiliatesVisits joined to SaleOfficers / Tbl_BusinessAffiliates
--               / Regions / Cities via the VISIT's own RegionID/CityID/SOID, not the
--               affiliate master's), and adds:
--                 - RegionalHeads join via SaleOfficers.RegionalHeadID (confirmed pattern
--                   used in ReportsController.SaleOfficerDetailRpt: "@TID = 0 OR
--                   so.RegionalHeadID = @TID", table dbo.RegionalHeads, column Name).
--                 - Lookup joins for Business Type / Classification / Expertise /
--                   Nature of Client via Tbl_AffiliatesBusinessTypes, Tbl_AffiliatesClassification,
--                   Tbl_AffiliatesExpertise, Tbl_NatureOfClient.
--                 - Competitors: verified against live data that Tbl_BusinessAffiliates.
--                   AffCompititors already stores a comma-separated list of competitor
--                   NAMES as free text (e.g. "Akzonobel,Sparco,Master,Dealer Own Brand"),
--                   NOT numeric IDs referencing Tbl_CompititorList. So it is passed through
--                   as-is - no STRING_SPLIT/STRING_AGG lookup needed.
--                 - CustomerLatLong: Tbl_BusinessAffiliatesVisits has no lat/long columns,
--                   so this is built from Tbl_BusinessAffiliates.Latitude/Longitude (the
--                   affiliate/customer's registered location).
--
-- Example test call:
--   EXEC dbo.usp_GetBusinessAffiliateReport
--        @DateFrom = '2020-01-01', @DateTo = '2030-01-01',
--        @SOID = 0, @RegionalHeadID = 0;
-- =====================================================================================
CREATE OR ALTER PROCEDURE dbo.usp_GetBusinessAffiliateReport
    @DateFrom       DATETIME,
    @DateTo         DATETIME,
    @SOID           INT = 0,
    @RegionalHeadID INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ba.CreatedOn                                                       AS CreatedDate,
        v.VisitDate                                                        AS VisitDate,
        rh.Name                                                            AS RegionalHead,
        so.Name                                                            AS SOName,
        r.Name                                                             AS Zone,
        c.Name                                                             AS City,
        ba.BusinessName                                                    AS BusinessName,
        ba.ContactPerson                                                   AS ContactPerson,
        ba.ContactNumber                                                   AS ContactNumber,
        ba.Address                                                         AS Address,
        bt.Name                                                            AS BusinessType,
        ex.Name                                                            AS Expertise,
        cl.Name                                                            AS Classification,
        noc.Name                                                           AS NatureOfClient,
        ba.AffCompititors                                                  AS Competitors,
        ba.Remarks                                                         AS AffiliateRemarks,
        CASE
            WHEN ba.Latitude IS NULL OR ba.Longitude IS NULL THEN NULL
            ELSE CAST(ba.Latitude AS VARCHAR(50)) + ',' + CAST(ba.Longitude AS VARCHAR(50))
        END                                                                AS CustomerLatLong,
        v.PurposeOfVisit                                                   AS PurposeOfVisit,
        v.TargetAgreement                                                  AS TargetAgreement,
        v.Remarks                                                          AS VisitRemarks
    FROM Tbl_BusinessAffiliatesVisits v
    LEFT JOIN SaleOfficers                  so  ON so.ID  = v.SOID
    LEFT JOIN RegionalHeads                 rh  ON rh.ID  = so.RegionalHeadID
    LEFT JOIN Tbl_BusinessAffiliates        ba  ON ba.ID  = v.BusinessAffiliateID
    LEFT JOIN Regions                       r   ON r.ID   = v.RegionID
    LEFT JOIN Cities                        c   ON c.ID   = v.CityID
    LEFT JOIN Tbl_AffiliatesBusinessTypes   bt  ON bt.ID  = ba.AffBusinessTypeID
    LEFT JOIN Tbl_AffiliatesClassification  cl  ON cl.ID  = ba.AffClassificationID
    LEFT JOIN Tbl_AffiliatesExpertise       ex  ON ex.ID  = ba.AffExpertiseID
    LEFT JOIN Tbl_NatureOfClient            noc ON noc.ID = ba.AffNatureOfClientID
    WHERE v.VisitDate >= @DateFrom
      AND v.VisitDate <  @DateTo
      AND (@SOID = 0 OR v.SOID = @SOID)
      AND (@RegionalHeadID = 0 OR so.RegionalHeadID = @RegionalHeadID)
      AND ISNULL(v.IsActive, 1) = 1
    ORDER BY v.VisitDate, so.Name, ba.BusinessName;
END
GO

-- Exec dbo.usp_GetBusinessAffiliateReport @DateFrom='2020-01-01', @DateTo='2030-01-01', @SOID=0, @RegionalHeadID=0;
