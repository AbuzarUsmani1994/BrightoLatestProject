-- Competitor Activity Types lookup
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_CompetitorActivityTypes' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_CompetitorActivityTypes
    (
        ID       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name     NVARCHAR(200)     NOT NULL,
        IsActive BIT               NOT NULL CONSTRAINT DF_CompetitorActivityTypes_IsActive DEFAULT (1)
    );

    INSERT INTO dbo.Tbl_CompetitorActivityTypes (Name) VALUES
    ('Trade Promotions & Discounts'),
    ('Consumer Offers & Schemes'),
    ('In Shop Branding'),
    ('Out of Home Advertising'),
    ('Painter & Contractor Engagement'),
    ('Digital & Local Media'),
    ('New Product Launch'),
    ('Direct Exhibitions & Activations');
END
GO

-- Competitor Activities main table
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_CompetitorActivities' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_CompetitorActivities
    (
        ID               INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SOID             INT           NULL,
        ActivityDate     DATE          NULL,
        CompetitorID     INT           NULL,
        ActivityTypeID   INT           NULL,
        Remarks          NVARCHAR(MAX) NULL,
        PicturePath      NVARCHAR(500) NULL,
        VideoPath        NVARCHAR(500) NULL,
        VoicePath        NVARCHAR(500) NULL,
        CreatedOn        DATETIME      NOT NULL CONSTRAINT DF_CompetitorActivities_CreatedOn DEFAULT (GETDATE()),
        IsActive         BIT           NOT NULL CONSTRAINT DF_CompetitorActivities_IsActive DEFAULT (1),
        ZoneID           INT           NULL,
        CityID           INT           NULL,
        IsDeleted        BIT           NOT NULL CONSTRAINT DF_CompetitorActivities_IsDeleted DEFAULT (0)
    );

    CREATE INDEX IX_CompetitorActivities_SOID ON dbo.Tbl_CompetitorActivities (SOID) WHERE IsDeleted = 0;
    CREATE INDEX IX_CompetitorActivities_Date  ON dbo.Tbl_CompetitorActivities (ActivityDate) WHERE IsDeleted = 0;
END
GO
