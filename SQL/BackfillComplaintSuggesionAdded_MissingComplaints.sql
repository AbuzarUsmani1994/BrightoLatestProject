-- One-time backfill: seed a ComplaintSuggesionAdded row for every existing
-- complaint that still has zero rows there.
--
-- Why this gap exists: ComplaintSuggesionAdded is written in exactly two
-- places - (1) SaveComplaintController.Post, which seeds an initial
-- "In Progress" row for every NEW complaint (already live), and (2)
-- ComplaintController.SaveManageComplaints, which adds a row each time a
-- quality reviewer submits a review. Complaints created BEFORE fix (1) was
-- deployed, and never reviewed by quality either, have no row at all -
-- hence "all the complaints are not added in ComplaintSuggesionAdded".
--
-- This only inserts for complaints missing a row (NOT EXISTS guard), so
-- it's safe to re-run and won't touch complaints that already have history
-- here (whether seeded automatically or genuinely reviewed by quality).
--
-- Same default values the live SaveComplaintController.Post insert already
-- uses: IsVerified=0/"In Progress", ComplaintStatus=1/"In Progress",
-- IsActive=1, IsPublished=0.

SET NOCOUNT ON;

INSERT INTO dbo.ComplaintSuggesionAdded
    (ComplaintID, ComplaintNo, ComplaintSubmitby, ComplaintSubmitDate,
     IsVerified, IsVerifiedName, ComplaintStatus, ComplaintStatusName,
     IsActive, IsPublished, CreatedDate, UpdatedDate)
SELECT
    tsc.ID,
    cu.ComplaintNumber,
    so.Name,
    tsc.CreatedOn,
    0, 'In Progress',
    1, 'In Progress',
    1, 0,
    GETDATE(), GETDATE()
FROM dbo.Tbl_SaveComplaints tsc
INNER JOIN dbo.SaleOfficers so ON tsc.SOID = so.ID
OUTER APPLY (
    -- Earliest Tbl_ComplaintUpdate row for this complaint, just to recover
    -- its ComplaintNumber (same number on every row for a given complaint).
    SELECT TOP 1 cu2.ComplaintNumber
    FROM dbo.Tbl_ComplaintUpdate cu2
    WHERE cu2.ComplaintID = tsc.ID
    ORDER BY cu2.LauncedTime ASC, cu2.ID ASC
) cu
WHERE tsc.IsActive = 1
  AND NOT EXISTS (
      SELECT 1 FROM dbo.ComplaintSuggesionAdded csa WHERE csa.ComplaintID = tsc.ID
  );

PRINT 'Backfilled ' + CAST(@@ROWCOUNT AS VARCHAR) + ' complaint(s) that had no ComplaintSuggesionAdded row.';
