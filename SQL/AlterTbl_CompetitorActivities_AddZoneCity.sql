-- Add ZoneID and CityID to Tbl_CompetitorActivities (run if table already exists)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Tbl_CompetitorActivities') AND name = 'ZoneID')
    ALTER TABLE dbo.Tbl_CompetitorActivities ADD ZoneID INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Tbl_CompetitorActivities') AND name = 'CityID')
    ALTER TABLE dbo.Tbl_CompetitorActivities ADD CityID INT NULL;
GO
