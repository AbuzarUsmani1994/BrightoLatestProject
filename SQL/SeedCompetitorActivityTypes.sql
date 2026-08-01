-- Clear existing and re-seed Tbl_CompetitorActivityTypes with correct values (IDs 1-8)
DELETE FROM dbo.Tbl_CompetitorActivityTypes;

SET IDENTITY_INSERT dbo.Tbl_CompetitorActivityTypes ON;

INSERT INTO dbo.Tbl_CompetitorActivityTypes (ID, Name) VALUES
(1, 'Trade Promotions & Discounts'),
(2, 'Consumer Offers & Schemes'),
(3, 'In Shop Branding'),
(4, 'Out of Home Advertising'),
(5, 'Painter & Contractor Engagement'),
(6, 'Digital & Local Media'),
(7, 'New Product Launch'),
(8, 'Direct Exhibitions & Activations');

SET IDENTITY_INSERT dbo.Tbl_CompetitorActivityTypes OFF;

DBCC CHECKIDENT ('dbo.Tbl_CompetitorActivityTypes', RESEED, 8);
GO
