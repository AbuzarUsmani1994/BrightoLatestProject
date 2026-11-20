-- Adds "SMS Configuration" to the left-nav Setup menu. That menu is
-- database-driven (dbo.Pages / dbo.RolePages) - see
-- FOS.Web.UI/Common/CurrentUser.cs (GetUserPages) and
-- FOS.Web.UI/Views/AdminPanel/UserMenu.cshtml.
--
-- Deliberately more conservative than the pattern used for other new menu
-- pages this app has added recently (e.g. AddCompetitorActivityGridMenuPage.sql,
-- which copies RolePages access from a sibling page): this screen holds the SMS
-- gateway login/password, so NO RolePages rows are inserted for it at all.
-- Per that same script's own comment, Admin-role users already see every page
-- regardless via a separate PageID=0 RolePages wildcard, so they need no row
-- here - everyone else simply won't see this link until you explicitly grant
-- it via dbo.RolePages, which is the point for a credentials screen.
--
-- dbo.Pages.PageID has NO IDENTITY (confirmed in FOS.Services/FOSDataModel.edmx).
-- This script can't see your real Pages data from here, so it looks up the
-- Setup parent page dynamically and PRINTs exactly what it found/did.

SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM dbo.Pages WHERE Controller = 'Setup' AND Action = 'SmsConfiguration')
BEGIN
    PRINT 'A Pages row for Setup/SmsConfiguration already exists - nothing to do.';
END
ELSE
BEGIN
    DECLARE @SetupParentPageID INT;
    DECLARE @NewPageID INT;
    DECLARE @Path VARCHAR(100) = '/Setup/SmsConfiguration';

    SELECT TOP 1 @SetupParentPageID = PageID
    FROM dbo.Pages
    WHERE Controller = 'Setup' AND ParentPageID IS NULL;

    IF @SetupParentPageID IS NULL
    BEGIN
        PRINT 'Could not find a top-level "Setup" module page (ParentPageID IS NULL, Controller=''Setup'') to nest under. Nothing inserted. Add this manually via the Pages table once you know the right ParentPageID, or tell me what the Setup module''s page looks like and I''ll adjust this script.';
    END
    ELSE
    BEGIN
        SELECT @NewPageID = ISNULL(MAX(PageID), 0) + 1 FROM dbo.Pages;

        INSERT INTO dbo.Pages (PageID, ParentPageID, Path, Type, Name, MenuInitials, Controller, Action, ShowMenu, RoleType, Icon)
        VALUES (@NewPageID, @SetupParentPageID, @Path, 'Setup', 'SMS Configuration', @Path, 'Setup', 'SmsConfiguration', 1, NULL, 'icon-envelope');

        PRINT 'Inserted Pages row ' + CAST(@NewPageID AS VARCHAR) + ' ("SMS Configuration") under ParentPageID ' + CAST(@SetupParentPageID AS VARCHAR) + '. No RolePages rows were added - only Admin-role users (via the PageID=0 wildcard) will see this link until you explicitly grant dbo.RolePages access to other roles.';
    END
END
