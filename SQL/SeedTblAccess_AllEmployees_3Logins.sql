-- Grants "All Access" (visibility over every active Sale Officer) in dbo.Tbl_Access
-- for 3 logins:
--   12688 - All Employees
--   12682 - All Employees
--   12507 - All Employees
--
-- The 3 numbers given are ECodes, not SaleOfficerIDs (same lesson as the earlier
-- Kausar Ali Khan access script) - each is resolved to its real SaleOfficerID via
-- dbo.SaleOfficers.ECode before inserting anything.
--
-- "All Employees" access is not a single Tbl_Access row - the Manage Access admin
-- screen (AdminPanel/Access.cshtml -> AddUpdateAccesss) only ever creates one row
-- per Sales Officer being granted. So "all employees" here means one row per every
-- other active Sales Officer in the system, each with:
--   SaleOfficerID = the employee being granted visibility (every active SO except
--                   the grantee themselves)
--   RepotedUP     = the grantee's own SaleOfficerID
--   ReportedDown  = same as SaleOfficerID (matches the existing single-row pattern)
--   RegionID      = that employee's own RegionalHeadID (per-row, since this spans
--                   every region, unlike a targeted same-region grant)
--
-- Fails loudly (RAISERROR, no insert) if any ECode doesn't resolve to exactly one
-- active Sale Officer. Inserts are idempotent (WHERE NOT EXISTS).

DECLARE @Grantees TABLE (ECode VARCHAR(50));
INSERT INTO @Grantees (ECode) VALUES ('12688'), ('12682'), ('12507');

DECLARE @Resolved TABLE (SaleOfficerID INT, ECode VARCHAR(50), Name VARCHAR(200));
INSERT INTO @Resolved (SaleOfficerID, ECode, Name)
SELECT so.ID, g.ECode, so.Name
FROM @Grantees g
LEFT JOIN dbo.SaleOfficers so ON so.ECode = g.ECode AND so.IsActive = 1;

IF EXISTS (SELECT 1 FROM @Resolved WHERE SaleOfficerID IS NULL)
BEGIN
    SELECT * FROM @Resolved WHERE SaleOfficerID IS NULL;
    RAISERROR('One or more ECodes did not resolve to a unique active Sale Officer. See rows above. Aborting.', 16, 1);
    RETURN;
END

INSERT INTO dbo.Tbl_Access (SaleOfficerID, RepotedUP, ReportedDown, CreatedOn, Status, IsDeleted, RegionID)
SELECT emp.ID, g.SaleOfficerID, emp.ID, GETDATE(), 1, 0, emp.RegionalHeadID
FROM @Resolved g
CROSS JOIN dbo.SaleOfficers emp
WHERE emp.IsActive = 1
  AND emp.ID <> g.SaleOfficerID
  AND NOT EXISTS (
      SELECT 1 FROM dbo.Tbl_Access a
      WHERE a.SaleOfficerID = emp.ID
        AND a.RepotedUP = g.SaleOfficerID
        AND a.IsDeleted = 0
  );

SELECT g.ECode, g.Name AS GrantedTo, COUNT(a.ID) AS TotalEmployeesVisible
FROM @Resolved g
JOIN dbo.Tbl_Access a ON a.RepotedUP = g.SaleOfficerID AND a.IsDeleted = 0
GROUP BY g.ECode, g.Name;
GO
