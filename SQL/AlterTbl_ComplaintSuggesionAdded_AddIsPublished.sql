-- =============================================
-- Adds IsPublished to dbo.ComplaintSuggesionAdded.
--
-- Why: Quality Review history entries are now reviewed by a quality
-- reviewer before they count as "official" - userID 1124 is the only
-- login allowed to mark an entry Published (see ComplaintController.
-- PublishComplaintSuggestion, gated server-side on that userID, and the
-- Publish button on Manage Complaints, hidden client-side for everyone
-- else). Unpublished entries still show in the Manage Complaints admin
-- grid (sp_GetComplaintDataForQuality is unchanged - the reviewer needs
-- to see everything to decide what to publish), but the SO Complaint
-- Summary/Detail reports and the SOComplaints.rdlc PDF report now only
-- surface the latest PUBLISHED entry per complaint - see
-- AlterSP_usp_GetComplaintDetail_AddIsPublishedFilter.sql,
-- AlterSP_sp_usp_GetComplaintSummary_AddIsPublishedFilter.sql and
-- AlterSP_Sp_GetComplaintVerifiedorNorbyQualtiy_AddIsPublishedFilter.sql.
--
-- Defaults existing rows to 0 (unpublished) - there is no prior
-- "published" concept to backfill from, so nothing already in this table
-- should silently start appearing in reports until a reviewer actually
-- publishes it.
--
-- Idempotent: guarded with a COL_LENGTH check so this script is safe to
-- re-run against a DB that already has the column.
-- =============================================
-- Exec (sanity check after running, no proc call needed - this is schema only):
-- SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
-- FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ComplaintSuggesionAdded' AND COLUMN_NAME = 'IsPublished'

IF COL_LENGTH('dbo.ComplaintSuggesionAdded', 'IsPublished') IS NULL
BEGIN
    ALTER TABLE dbo.ComplaintSuggesionAdded
    ADD IsPublished BIT NOT NULL CONSTRAINT DF_ComplaintSuggesionAdded_IsPublished DEFAULT (0);
END
GO
