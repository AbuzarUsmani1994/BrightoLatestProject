-- Backs the new "Add Visit" popup on Manage Complaints (quality staff logging a
-- physical site visit against a specific complaint, distinct from the existing
-- ComplaintSuggesionAdded quality-review entries). Two new tables:
--
-- 1) dbo.Tbl_PackSize - brand-new lookup, no equivalent table exists anywhere
--    in this schema today. Seeded with Drum/Gallon/Quarter/Tin per your values.
--
-- 2) dbo.Tbl_ComplaintQualityVisit - one row per visit submission.
--    ComplaintNatureID/SubstrateNatureID/ProductCategoryID are stored as IDs
--    against existing lookup tables - ComplaintNatureID against
--    Tbl_ProductNature (the same "Nature" lookup already used elsewhere on
--    this page/mobile app, not a dedicated complaint-nature table) - plus
--    Tbl_ConditionOfSubstratee, Tbl_ProuctCategory. No Product Sub Category -
--    just Product Category, per your instruction. PackSize and
--    ComplaintStatus are stored as plain text (the dropdown's displayed
--    value, e.g. "Tin"/"In Progress"), not an ID/code - Tbl_PackSize is
--    still the source that populates the Pack Size dropdown, just not
--    referenced by ID on save. Every other field is free text. No FK
--    constraints, matching this schema's existing loose-FK convention.

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_PackSize' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_PackSize
    (
        ID         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name       VARCHAR(100) NOT NULL,
        IsActive   BIT NOT NULL CONSTRAINT DF_Tbl_PackSize_IsActive DEFAULT (1),
        CreatedOn  DATETIME NOT NULL CONSTRAINT DF_Tbl_PackSize_CreatedOn DEFAULT (GETDATE())
    );

    PRINT 'Created dbo.Tbl_PackSize.';
END
ELSE
BEGIN
    PRINT 'dbo.Tbl_PackSize already exists - skipped create.';
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Tbl_PackSize WHERE Name = 'Drum')
    INSERT INTO dbo.Tbl_PackSize (Name) VALUES ('Drum');
IF NOT EXISTS (SELECT 1 FROM dbo.Tbl_PackSize WHERE Name = 'Gallon')
    INSERT INTO dbo.Tbl_PackSize (Name) VALUES ('Gallon');
IF NOT EXISTS (SELECT 1 FROM dbo.Tbl_PackSize WHERE Name = 'Quarter')
    INSERT INTO dbo.Tbl_PackSize (Name) VALUES ('Quarter');
IF NOT EXISTS (SELECT 1 FROM dbo.Tbl_PackSize WHERE Name = 'Tin')
    INSERT INTO dbo.Tbl_PackSize (Name) VALUES ('Tin');
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_ComplaintQualityVisit' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_ComplaintQualityVisit
    (
        ID                              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ComplaintID                     INT NOT NULL,
        CustomerName                    VARCHAR(200) NULL,
        SiteContactPerson               VARCHAR(200) NULL,
        ComplaintNatureID                INT NULL,
        SubstrateNatureID               INT NULL,
        ProductCategoryID               INT NULL,
        PackSize                        VARCHAR(100) NULL,
        VisitTeamMembers                VARCHAR(500) NULL,
        NoOfVisits                      VARCHAR(50) NULL,
        VisitDates                      VARCHAR(500) NULL,
        ComplaintStatus                 VARCHAR(100) NULL,
        SalePersonName                  VARCHAR(200) NULL,
        ComplaintSummary                NVARCHAR(MAX) NULL,
        SiteObservationInvestigation    NVARCHAR(MAX) NULL,
        CorrectiveAndPreventiveActions  NVARCHAR(MAX) NULL,
        Conclusion                      NVARCHAR(MAX) NULL,
        Picture1                        VARCHAR(500) NULL,
        Picture2                        VARCHAR(500) NULL,
        CreatedBy                       INT NULL,
        CreatedOn                       DATETIME NOT NULL CONSTRAINT DF_Tbl_ComplaintQualityVisit_CreatedOn DEFAULT (GETDATE()),
        IsActive                        BIT NOT NULL CONSTRAINT DF_Tbl_ComplaintQualityVisit_IsActive DEFAULT (1)
    );

    CREATE INDEX IX_Tbl_ComplaintQualityVisit_ComplaintID ON dbo.Tbl_ComplaintQualityVisit (ComplaintID) WHERE IsActive = 1;

    PRINT 'Created dbo.Tbl_ComplaintQualityVisit.';
END
ELSE
BEGIN
    PRINT 'dbo.Tbl_ComplaintQualityVisit already exists - skipped create.';
END
GO
