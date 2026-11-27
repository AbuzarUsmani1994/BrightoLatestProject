-- Backs the new "Commercial Decision" popup on Manage Complaints - visible only to
-- userID 1124 (the same user already gated for the existing Quality Review Publish
-- button), opened via its own grid-row button, submitted via its "Publish" button.
--
-- ProductCategoryID is stored as an ID against the existing Tbl_ProuctCategory lookup
-- (same as the Quality Visit form's Product Category field). PackSize is stored as
-- plain text (the dropdown's displayed value, e.g. "Tin"), not DB-backed, matching
-- the Quality Visit form's Pack Size field. Quantity/Remarks are free text. No FK
-- constraints, matching this schema's existing loose-FK convention.

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_ComplaintCommercialDecision' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_ComplaintCommercialDecision
    (
        ID                INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ComplaintID       INT NOT NULL,
        ProductCategoryID INT NULL,
        PackSize          VARCHAR(100) NULL,
        Quantity          VARCHAR(50) NULL,
        Remarks           NVARCHAR(1000) NULL,
        Picture1          VARCHAR(500) NULL,
        Picture2          VARCHAR(500) NULL,
        CreatedBy         INT NULL,
        CreatedOn         DATETIME NOT NULL CONSTRAINT DF_Tbl_ComplaintCommercialDecision_CreatedOn DEFAULT (GETDATE()),
        IsActive          BIT NOT NULL CONSTRAINT DF_Tbl_ComplaintCommercialDecision_IsActive DEFAULT (1)
    );

    CREATE INDEX IX_Tbl_ComplaintCommercialDecision_ComplaintID ON dbo.Tbl_ComplaintCommercialDecision (ComplaintID) WHERE IsActive = 1;

    PRINT 'Created dbo.Tbl_ComplaintCommercialDecision.';
END
ELSE
BEGIN
    PRINT 'dbo.Tbl_ComplaintCommercialDecision already exists - skipped create.';
END
GO
