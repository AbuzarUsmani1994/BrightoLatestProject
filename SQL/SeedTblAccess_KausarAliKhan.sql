-- Grants Kausar Ali Khan (SaleOfficers.RepotedUP) access to view 5 Sales Officers,
-- by inserting rows into dbo.Tbl_Access.
--
-- The 5 numbers given (12479, 12457, 12275, 12344, 12761) are ECodes, not
-- SaleOfficerIDs - this script resolves the real SaleOfficerID for each via
-- dbo.SaleOfficers.ECode before inserting anything.
--
-- Column mapping follows the existing bulk-grant pattern in Setup/ManageArea.cs
-- (AddUpdateAccess / the "Type == 2" branch):
--   SaleOfficerID = the SO being granted access to (resolved from ECode below)
--   RepotedUP     = the SaleOfficerID of the person granted visibility (Kausar Ali Khan,
--                   resolved by name below)
--   ReportedDown  = same as SaleOfficerID (matches existing insert pattern)
--   RegionID      = Kausar Ali Khan's own RegionalHeadID (matches "RegionID = obj.RegionalHeadID"
--                   in the existing bulk-grant code) - flag if this assumption is wrong for
--                   this use case.
--
-- Fails loudly (RAISERROR, no insert) rather than guessing if any ECode doesn't resolve
-- to exactly one active Sale Officer, if the resolved name doesn't match what was given,
-- or if "Kausar Ali Khan" isn't a unique active name.

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

DECLARE @Targets TABLE (ECode VARCHAR(50), ExpectedName VARCHAR(200));
INSERT INTO @Targets (ECode, ExpectedName) VALUES
    ('12479', 'Haroon Badshah'),
    ('12457', 'Adnan Kiyani'),
    ('12275', 'Muhammad rasheed'),
    ('12344', 'Faisal Khan'),
    ('12761', 'Muhammad Waleed');

-- Resolve each ECode to its actual SaleOfficerID.
DECLARE @Resolved TABLE (SaleOfficerID INT, ECode VARCHAR(50), ExpectedName VARCHAR(200), ActualName VARCHAR(200));
INSERT INTO @Resolved (SaleOfficerID, ECode, ExpectedName, ActualName)
SELECT so.ID, t.ECode, t.ExpectedName, so.Name
FROM @Targets t
LEFT JOIN dbo.SaleOfficers so ON so.ECode = t.ECode AND so.IsActive = 1;

-- Abort loudly on anything that didn't resolve cleanly, instead of inserting a guess.
IF EXISTS (SELECT 1 FROM @Resolved WHERE SaleOfficerID IS NULL)
BEGIN
    SELECT * FROM @Resolved WHERE SaleOfficerID IS NULL;
    RAISERROR('One or more ECodes did not resolve to a unique active Sale Officer. See rows above. Aborting.', 16, 1);
    RETURN;
END

IF EXISTS (SELECT 1 FROM @Resolved WHERE ActualName <> ExpectedName)
BEGIN
    SELECT * FROM @Resolved WHERE ActualName <> ExpectedName;
    RAISERROR('One or more resolved names do not match the names given. See rows above. Aborting - confirm the right ECode/name pairs.', 16, 1);
    RETURN;
END

INSERT INTO dbo.Tbl_Access (SaleOfficerID, RepotedUP, ReportedDown, CreatedOn, Status, IsDeleted, RegionID)
SELECT r.SaleOfficerID, @GrantToSOID, r.SaleOfficerID, GETDATE(), 1, 0, @GrantToRegionID
FROM @Resolved r
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Tbl_Access a
    WHERE a.SaleOfficerID = r.SaleOfficerID
      AND a.RepotedUP = @GrantToSOID
      AND a.IsDeleted = 0
);

SELECT a.ID, a.SaleOfficerID, so.Name AS SaleOfficerName, so.ECode, a.RepotedUP, a.RegionID, a.Status, a.CreatedOn
FROM dbo.Tbl_Access a
JOIN dbo.SaleOfficers so ON so.ID = a.SaleOfficerID
WHERE a.RepotedUP = @GrantToSOID AND a.IsDeleted = 0
ORDER BY a.CreatedOn DESC;
GO
