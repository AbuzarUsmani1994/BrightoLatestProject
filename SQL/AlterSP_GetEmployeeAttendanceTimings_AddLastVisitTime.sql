-- =============================================
-- Adds a "Last_Visit_Time" column to the attendance sync report so that
-- when a Sale Officer never presses "Market close" (End_Time = 'Not Recorded'),
-- reviewers can still see the last time the SO logged a visit in the field
-- (derived from Tbl_TradeVisitsFinal / Tbl_HousingVisits, the same tables
-- used by usp_GetKPIPercentageReport for Dealer/Site visit counts).
-- =============================================

-- Exec spGetEmployeeAttendanceTimings 0,'2026-03-06','2026-03-06'
CREATE OR ALTER PROCEDURE [dbo].[spGetEmployeeAttendanceTimings]
@RID INT = 0,
@startDate DATE,
@endDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    WITH StartTimes AS (
        SELECT
            so.ID AS SOID,
            so.ECode AS Emp_Code,
            so.Name AS Emp_Name,
            CAST(rh.CreatedAt AS DATE) AS AttendanceDate,
            MIN(CAST(rh.CreatedAt AS TIME)) AS Start_Time,
            MIN(rh.MarketStartLatlong) AS MarketStartLatlong,
            rhead.Name AS RegionalHead,
            city.Name AS City
        FROM SaleOfficers so WITH (NOLOCK)
        INNER JOIN [dbo].[SOAttendance] rh ON rh.SOID = so.ID
        LEFT JOIN Regionalheads rhead ON so.Regionalheadid = rhead.id
        LEFT JOIN cities city ON rh.cityid = city.id
        WHERE CAST(rh.CreatedAt AS DATE) >= @startDate
            AND CAST(rh.CreatedAt AS DATE) <= @endDate
            AND rh.type = 'Market start'
            AND so.RegionalHeadID = CASE WHEN @RID = 0 THEN so.RegionalHeadID ELSE @RID END
            AND so.IsActive = 1
            AND so.IsDeleted = 0
        GROUP BY so.ID, so.ECode, so.Name, CAST(rh.CreatedAt AS DATE), rhead.Name, city.Name
    ),
    EndTimes AS (
        SELECT
            so.ID AS SOID,
            CAST(rh.CreatedAt AS DATE) AS AttendanceDate,
            MAX(CAST(rh.CreatedAt AS TIME)) AS End_Time,
            MAX(rh.MarketCloseLatlong) AS MarketCloseLatlong
        FROM SaleOfficers so WITH (NOLOCK)
        INNER JOIN [dbo].[SOAttendance] rh ON rh.SOID = so.ID
        WHERE CAST(rh.CreatedAt AS DATE) >= @startDate
            AND CAST(rh.CreatedAt AS DATE) <= @endDate
            AND rh.type = 'Market close'
            AND so.RegionalHeadID = CASE WHEN @RID = 0 THEN so.RegionalHeadID ELSE @RID END
            AND so.IsActive = 1
            AND so.IsDeleted = 0
        GROUP BY so.ID, CAST(rh.CreatedAt AS DATE)
    ),
    Visits AS (
        SELECT SOID, CreatedAt FROM Tbl_TradeVisitsFinal
        WHERE CAST(CreatedAt AS DATE) >= @startDate
            AND CAST(CreatedAt AS DATE) <= @endDate
            AND ISNULL(IsActive, 1) = 1

        UNION ALL

        SELECT SOID, CreatedAt FROM Tbl_HousingVisits
        WHERE CAST(CreatedAt AS DATE) >= @startDate
            AND CAST(CreatedAt AS DATE) <= @endDate
            AND ISNULL(IsActive, 1) = 1
    ),
    LastVisits AS (
        SELECT
            v.SOID,
            CAST(v.CreatedAt AS DATE) AS AttendanceDate,
            MAX(CAST(v.CreatedAt AS TIME)) AS Last_Visit_Time
        FROM Visits v
        GROUP BY v.SOID, CAST(v.CreatedAt AS DATE)
    )
    SELECT
        s.AttendanceDate,
        s.Emp_Code,
        s.Emp_Name,
        CASE
            WHEN s.Start_Time IS NULL THEN 'Not Recorded'
            ELSE CONVERT(VARCHAR(8), s.Start_Time, 108)
        END AS Start_Time,
        CASE
            WHEN e.End_Time IS NULL THEN 'Not Recorded'
            ELSE CONVERT(VARCHAR(8), e.End_Time, 108)
        END AS End_Time,
        CASE
            WHEN lv.Last_Visit_Time IS NULL THEN 'Not Recorded'
            ELSE CONVERT(VARCHAR(8), lv.Last_Visit_Time, 108)
        END AS Last_Visit_Time,
        CASE
            WHEN s.Start_Time IS NOT NULL AND e.End_Time IS NOT NULL
            THEN DATEDIFF(MINUTE, s.Start_Time, e.End_Time)
            ELSE 0
        END AS Working_Minutes,
        CASE
            WHEN s.Start_Time IS NOT NULL AND e.End_Time IS NOT NULL
            THEN
                CONVERT(VARCHAR(10), DATEDIFF(MINUTE, s.Start_Time, e.End_Time)/60) + ':' +
                RIGHT('0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, s.Start_Time, e.End_Time)%60), 2)
            ELSE 'N/A'
        END AS Working_Hours,
        s.RegionalHead,
        s.City,
        s.MarketStartLatlong,
        e.MarketCloseLatlong
    FROM StartTimes s
    LEFT JOIN EndTimes e
        ON s.SOID = e.SOID
        AND s.AttendanceDate = e.AttendanceDate
    LEFT JOIN LastVisits lv
        ON s.SOID = lv.SOID
        AND s.AttendanceDate = lv.AttendanceDate

    UNION

    -- Include SOs that only have close punches but no start punches
    SELECT
        e.AttendanceDate,
        so.ECode AS Emp_Code,
        so.Name AS Emp_Name,
        'Not Recorded' AS Start_Time,
        CONVERT(VARCHAR(8), e.End_Time, 108) AS End_Time,
        CASE
            WHEN lv.Last_Visit_Time IS NULL THEN 'Not Recorded'
            ELSE CONVERT(VARCHAR(8), lv.Last_Visit_Time, 108)
        END AS Last_Visit_Time,
        0 AS Working_Minutes,
        'N/A' AS Working_Hours,
        rhead.Name AS RegionalHead,
        city.Name AS City,
        NULL AS MarketStartLatlong,
        e.MarketCloseLatlong
    FROM EndTimes e
    INNER JOIN SaleOfficers so ON e.SOID = so.ID
    LEFT JOIN Regionalheads rhead ON so.Regionalheadid = rhead.id
    LEFT JOIN cities city ON so.CityId = city.id
    LEFT JOIN LastVisits lv
        ON e.SOID = lv.SOID
        AND e.AttendanceDate = lv.AttendanceDate
    WHERE NOT EXISTS (
        SELECT 1 FROM StartTimes s
        WHERE s.SOID = e.SOID AND s.AttendanceDate = e.AttendanceDate
    )
    AND so.RegionalHeadID = CASE WHEN @RID = 0 THEN so.RegionalHeadID ELSE @RID END
    AND so.IsActive = 1
    AND so.IsDeleted = 0

    ORDER BY AttendanceDate DESC, RegionalHead, Emp_Name;
END;
GO

-- Test:
-- EXEC spGetEmployeeAttendanceTimings 0,'2026-03-06','2026-03-06'
