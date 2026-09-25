-- Dealer Verification Form submissions from the mobile app - same shape as a
-- Claims submission (Tbl_SalesClaimMaster) minus the sale value/line items,
-- plus the verification Picture. Reviewed in the same "Claims Detail" manager
-- grid as Claims (Doc Type = Dealer Verification Form), where Details shows
-- just this Picture instead of the full claim/items/approvals view.
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_DealerVerification' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_DealerVerification
    (
        ID                          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SOID                        INT NULL,
        SegmentID                   INT NULL,
        CustomerID                  INT NULL,
        TradePartyID                INT NULL,
        TradeEmployeeID             INT NULL,
        DateSelected                DATETIME NULL,
        Picture                     VARCHAR(500) NULL,
        Status                      VARCHAR(50) NULL,
        ApprovedBy                  INT NULL,
        ClaimManagerLatestStatus    VARCHAR(50) NULL,
        ClaimManagerLatestRemarks   VARCHAR(500) NULL,
        ClaimManagerDate            DATETIME NULL,
        CreatedOn                   DATETIME NOT NULL CONSTRAINT DF_Tbl_DealerVerification_CreatedOn DEFAULT (GETDATE()),
        IsActive                    BIT NOT NULL CONSTRAINT DF_Tbl_DealerVerification_IsActive DEFAULT (1)
    );

    CREATE INDEX IX_Tbl_DealerVerification_SODate ON dbo.Tbl_DealerVerification (SOID, DateSelected) WHERE IsActive = 1;
    CREATE INDEX IX_Tbl_DealerVerification_Customer ON dbo.Tbl_DealerVerification (CustomerID) WHERE IsActive = 1;
END
GO
