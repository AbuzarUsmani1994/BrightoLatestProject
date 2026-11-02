-- =============================================
-- Adds "Complaint Status" tracking to dbo.ComplaintSuggesionAdded.
--
-- Why: The Quality Review feature on Manage Complaints is moving from an
-- upsert (one row per ComplaintID) to insert-only history (many rows per
-- ComplaintID over time, shown as a list). As part of this, each history
-- row can now carry a Complaint Status:
--   1 = In Progress, 2 = On Hold, 3 = Resolved, 4 = Duplicate
--
-- ComplaintStatus (int)         - numeric code, same pattern as IsVerified
-- ComplaintStatusName (varchar) - display text mirror, same pattern as
--                                 the existing IsVerified/IsVerifiedName pair
--                                 on this same table.
--
-- Idempotent: guarded with COL_LENGTH checks so this script is safe to
-- re-run against a DB that already has the columns.
-- =============================================
-- Exec (sanity check after running, no proc call needed - this is schema only):
-- SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
-- FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ComplaintSuggesionAdded'
-- ORDER BY ORDINAL_POSITION

IF COL_LENGTH('dbo.ComplaintSuggesionAdded', 'ComplaintStatus') IS NULL
BEGIN
    ALTER TABLE dbo.ComplaintSuggesionAdded
    ADD ComplaintStatus INT NULL;
END
GO

IF COL_LENGTH('dbo.ComplaintSuggesionAdded', 'ComplaintStatusName') IS NULL
BEGIN
    ALTER TABLE dbo.ComplaintSuggesionAdded
    ADD ComplaintStatusName VARCHAR(100) NULL;
END
GO

-- Exec (sanity check after running, no proc call needed - this is schema only):
-- SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
-- FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ComplaintSuggesionAdded'
-- ORDER BY ORDINAL_POSITION
