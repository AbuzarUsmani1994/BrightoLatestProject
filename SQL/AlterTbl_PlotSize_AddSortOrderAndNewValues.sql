-- Tbl_PlotSize currently has 8 rows (3/5/7/10 Marla, 1/2/4 Kanal, Others) with no
-- explicit display order - the dropdown just showed them in whatever order the
-- table scan returned them. Adding a SortOrder column (same convention as
-- Item.SortOrder elsewhere in this app) so the mobile/web dropdown can show the
-- full Marla 1-20 -> Kanal 1-5 -> Other sequence regardless of ID order.
--
-- Existing IDs are NOT changed or re-ordered - Tbl_HousingVisits.PlotSize already
-- stores these IDs on real visit records, so renumbering them would corrupt
-- historical data. Only SortOrder is set on existing rows; missing sizes are
-- inserted as new rows (new IDs), each with the SortOrder matching its position
-- in the target list. Safe to re-run: existing names are only updated, not
-- duplicated, and new rows are only inserted if missing.

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Tbl_PlotSize') AND name = 'SortOrder')
BEGIN
    ALTER TABLE dbo.Tbl_PlotSize ADD SortOrder INT NULL;
END
GO

-- Set SortOrder on the 8 existing rows to match their position in the target list.
UPDATE dbo.Tbl_PlotSize SET SortOrder = 3  WHERE Name = '3Marla';
UPDATE dbo.Tbl_PlotSize SET SortOrder = 5  WHERE Name = '5Marla';
UPDATE dbo.Tbl_PlotSize SET SortOrder = 7  WHERE Name = '7 Marla';
UPDATE dbo.Tbl_PlotSize SET SortOrder = 10 WHERE Name = '10 Marla';
UPDATE dbo.Tbl_PlotSize SET SortOrder = 21 WHERE Name = '1 Kanal';
UPDATE dbo.Tbl_PlotSize SET SortOrder = 22 WHERE Name = '2 Kanal';
UPDATE dbo.Tbl_PlotSize SET SortOrder = 24 WHERE Name = '4 Kanal';
UPDATE dbo.Tbl_PlotSize SET SortOrder = 26 WHERE Name = 'Others';
GO

-- Insert the missing sizes (new IDs), each tagged with its target SortOrder.
DECLARE @NewSizes TABLE (Name VARCHAR(50), SortOrder INT);
INSERT INTO @NewSizes (Name, SortOrder) VALUES
    ('1 Marla', 1),
    ('2 Marla', 2),
    ('4 Marla', 4),
    ('6 Marla', 6),
    ('8 Marla', 8),
    ('9 Marla', 9),
    ('11 Marla', 11),
    ('12 Marla', 12),
    ('13 Marla', 13),
    ('14 Marla', 14),
    ('15 Marla', 15),
    ('16 Marla', 16),
    ('17 Marla', 17),
    ('18 Marla', 18),
    ('19 Marla', 19),
    ('20 Marla', 20),
    ('3 Kanal', 23),
    ('5 Kanal', 25);

INSERT INTO dbo.Tbl_PlotSize (Name, IsActive, SortOrder)
SELECT ns.Name, 1, ns.SortOrder
FROM @NewSizes ns
WHERE NOT EXISTS (SELECT 1 FROM dbo.Tbl_PlotSize p WHERE p.Name = ns.Name);
GO

-- Verify: should list all 26 sizes in display order.
SELECT ID, Name, SortOrder, IsActive FROM dbo.Tbl_PlotSize ORDER BY SortOrder;
GO
