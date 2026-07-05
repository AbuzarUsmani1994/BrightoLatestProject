-- Add Setup -> Product Knowledge & Competitor Activities page
IF NOT EXISTS (SELECT 1 FROM dbo.Pages WHERE Controller = 'Setup' AND [Action] = 'ProdKnowledge')
BEGIN
    INSERT INTO dbo.Pages (ParentPageID, [Path], [Type], [Name], MenuInitials, Controller, [Action], ShowMenu, RoleType, Icon)
    VALUES (3, '/Setup/ProdKnowledge', 'Razor', 'Product Knowledge & Competitor Activities', '/Setup', 'Setup', 'ProdKnowledge', 1, 2, NULL);
END
GO
