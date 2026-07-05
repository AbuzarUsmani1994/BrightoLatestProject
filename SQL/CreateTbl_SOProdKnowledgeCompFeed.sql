IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE Name = 'Tbl_SOProdKnowledgeCompFeed' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_SOProdKnowledgeCompFeed (
        ID               INT            NOT NULL,
        SOID             INT            NULL,
        HeadID           INT            NULL,
        ProductKnowledge DECIMAL(18, 4) NULL,
        CompFeed         DECIMAL(18, 4) NULL,
        FinancialYearID  INT            NULL,
        Quarter          NVARCHAR(5)    NULL,
        IsActive         BIT            NULL,
        CreatedOn        DATETIME       NULL,
        CONSTRAINT PK_Tbl_SOProdKnowledgeCompFeed PRIMARY KEY (ID)
    );
END
GO
