-- Grants ECode 12507 "Brighto Services Head Sale Officer" access: Tbl_Access
-- visibility over every active Sale Officer under RegionalHeadID = 22, which is
-- this app's marker for the "Brighto Services" branch (see RetailerController.
-- GetSaleOfficers(), which hardcodes RegionalHeadID == 22 for the Site Allocation
-- Sale Officer dropdown). This gives 12507 head-level visibility scoped to that one
-- branch, not the whole company - for that, see SeedTblAccess_AllEmployees_3Logins.sql.
--
-- 12507 is an ECode, not a SaleOfficerID (same lesson as every earlier access
-- script this session) - resolved via dbo.SaleOfficers.ECode before inserting
-- anything. Fails loudly if it doesn't resolve to exactly one active Sale Officer.
-- Idempotent (WHERE NOT EXISTS).

DECLARE @GrantToSOID INT;
SELECT @GrantToSOID = ID
FROM dbo.SaleOfficers
WHERE ECode = '12507' AND IsActive = 1;

IF @GrantToSOID IS NULL
BEGIN
    RAISERROR('ECode 12507 did not resolve to a unique active Sale Officer. Aborting - check dbo.SaleOfficers.ECode.', 16, 1);
    RETURN;
END

INSERT INTO dbo.Tbl_Access (SaleOfficerID, RepotedUP, ReportedDown, CreatedOn, Status, IsDeleted, RegionID)
SELECT emp.ID, @GrantToSOID, emp.ID, GETDATE(), 1, 0, emp.RegionalHeadID
FROM dbo.SaleOfficers emp
WHERE emp.IsActive = 1
  AND emp.RegionalHeadID = 22
  AND emp.ID <> @GrantToSOID
  AND NOT EXISTS (
      SELECT 1 FROM dbo.Tbl_Access a
      WHERE a.SaleOfficerID = emp.ID
        AND a.RepotedUP = @GrantToSOID
        AND a.IsDeleted = 0
  );

SELECT so.Name AS GrantedTo, so.ECode, COUNT(a.ID) AS BrightoServicesEmployeesVisible
FROM dbo.SaleOfficers so
JOIN dbo.Tbl_Access a ON a.RepotedUP = so.ID AND a.IsDeleted = 0
WHERE so.ID = @GrantToSOID
GROUP BY so.Name, so.ECode;
GO
