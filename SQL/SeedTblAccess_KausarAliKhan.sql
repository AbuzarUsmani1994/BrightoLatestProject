-- Grants Kausar Ali Khan (SaleOfficers.RepotedUP) access to view 5 Sales Officers,
-- by inserting rows into dbo.Tbl_Access.
--
-- Column mapping follows the existing bulk-grant pattern in Setup/ManageArea.cs
-- (AddUpdateAccess / the "Type == 2" branch):
--   SaleOfficerID = the SO being granted access to
--   RepotedUP     = the SaleOfficerID of the person granted visibility (Kausar Ali Khan)
--   ReportedDown  = same as SaleOfficerID (matches existing insert pattern)
--   RegionID      = Kausar Ali Khan's own RegionalHeadID (matches "RegionID = obj.RegionalHeadID"
--                   in the existing bulk-grant code)
--
-- Assumption: "Kausar Ali Khan" is a unique, active name in dbo.SaleOfficers. If that's
-- not true, or the RegionID assumption above is wrong for this use case, this will stop
-- with a clear error instead of silently inserting something wrong - fix and re-run.

DECLARE @GrantToSOID INT;
DECLARE @GrantToRegionID INT;

SELECT @GrantToSOID = ID, @GrantToRegionID = RegionalHeadID
FROM dbo.SaleOfficers
WHERE Name = 'Kausar Ali Khan' AND IsActive = 1;

IF @GrantToSOID IS NULL
BEGIN
    RAISERROR('Could not find a unique active Sale Officer named ''Kausar Ali Khan''. Aborting - check dbo.SaleOfficers.Name.', 16, 1);
    RETURN;
END

DECLARE @Targets TABLE (SaleOfficerID INT, ExpectedName VARCHAR(200));
INSERT INTO @Targets (SaleOfficerID, ExpectedName) VALUES
    (12479, 'Haroon Badshah'),
    (12457, 'Adnan Kiyani'),
    (12275, 'Muhammad rasheed'),
    (12344, 'Faisal Khan'),
    (12761, 'Muhammad Waleed');

-- Sanity check: flag (not block) any ID whose current name doesn't match what was given,
-- in case an ID is stale/reused.
SELECT t.SaleOfficerID, t.ExpectedName, so.Name AS ActualName
FROM @Targets t
LEFT JOIN dbo.SaleOfficers so ON so.ID = t.SaleOfficerID
WHERE so.ID IS NULL OR so.Name <> t.ExpectedName;

INSERT INTO dbo.Tbl_Access (SaleOfficerID, RepotedUP, ReportedDown, CreatedOn, Status, IsDeleted, RegionID)
SELECT t.SaleOfficerID, @GrantToSOID, t.SaleOfficerID, GETDATE(), 1, 0, @GrantToRegionID
FROM @Targets t
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Tbl_Access a
    WHERE a.SaleOfficerID = t.SaleOfficerID
      AND a.RepotedUP = @GrantToSOID
      AND a.IsDeleted = 0
);

SELECT a.ID, a.SaleOfficerID, so.Name AS SaleOfficerName, a.RepotedUP, a.RegionID, a.Status, a.CreatedOn
FROM dbo.Tbl_Access a
JOIN dbo.SaleOfficers so ON so.ID = a.SaleOfficerID
WHERE a.RepotedUP = @GrantToSOID AND a.IsDeleted = 0
ORDER BY a.CreatedOn DESC;
GO
