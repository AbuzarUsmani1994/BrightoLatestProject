-- Tracks the welcome SMS sent to a newly-registered Business Affiliate, so we
-- can see who was messaged, when, and whether the send succeeded.
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_BusinessAffiliateWelcomeMessageLog' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_BusinessAffiliateWelcomeMessageLog
    (
        ID          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CustomerID  INT NOT NULL,
        PhoneNo     VARCHAR(20) NULL,
        Message     NVARCHAR(1000) NULL,
        Status      VARCHAR(20) NULL,
        ErrorDetail NVARCHAR(500) NULL,
        CreatedOn   DATETIME NOT NULL CONSTRAINT DF_Tbl_BusinessAffiliateWelcomeMessageLog_CreatedOn DEFAULT (GETDATE())
    );

    CREATE INDEX IX_Tbl_BusinessAffiliateWelcomeMessageLog_CustomerID ON dbo.Tbl_BusinessAffiliateWelcomeMessageLog (CustomerID);
END
GO
