-- Adds "Competitor Activity Grid" to the left-nav Reports menu. That menu is
-- database-driven (dbo.Pages / dbo.RolePages), not static HTML - see
-- FOS.Web.UI/Common/CurrentUser.cs (GetUserPages) and
-- FOS.Web.UI/Views/AdminPanel/UserMenu.cshtml for how it's built.
--
-- dbo.Pages.PageID has NO IDENTITY (confirmed in FOS.Services/FOSDataModel.edmx -
-- unlike e.g. QrActivity.QrID, which is explicitly StoreGeneratedPattern="Identity").
-- This follows the same manual max(ID)+1 convention already used elsewhere in
-- this codebase (e.g. FOS.Web.UI/Controllers/API/RetailerRegistrationController.cs).
--
-- This script can't see your real Pages/RolePages data from here, so instead of
-- guessing at the parent menu page or which roles should get access, it looks
-- them up dynamically and PRINTs exactly what it found/did at every step.
-- Read the printed output after running this - do not assume it worked silently.

SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM dbo.Pages WHERE Controller = 'Reports' AND Action = 'CompetitorActivityGrid')
BEGIN
    PRINT 'A Pages row for Reports/CompetitorActivityGrid already exists - nothing to do.';
END
ELSE
BEGIN
    DECLARE @ReportsParentPageID INT;
    DECLARE @NewPageID INT;
    DECLARE @Path VARCHAR(100) = '/Reports/CompetitorActivityGrid';

    -- Prefer nesting right next to the existing "Competitor Report" page, since
    -- that's exactly the section this belongs in; fall back to the top-level
    -- Reports module page if Competitor Report isn't itself menu-registered.
    SELECT TOP 1 @ReportsParentPageID = ParentPageID
    FROM dbo.Pages
    WHERE Controller = 'Reports' AND Action = 'CompetitorReport';

    IF @ReportsParentPageID IS NULL
    BEGIN
        SELECT TOP 1 @ReportsParentPageID = PageID
        FROM dbo.Pages
        WHERE Controller = 'Reports' AND ParentPageID IS NULL;
    END

    IF @ReportsParentPageID IS NULL
    BEGIN
        PRINT 'Could not find a "Reports" parent page to nest under - no Reports/CompetitorReport row and no top-level Reports module page (ParentPageID IS NULL, Controller=''Reports''). Nothing inserted. Add this manually via the Pages table once you know the right ParentPageID, or tell me what the Reports module''s page looks like and I''ll adjust this script.';
    END
    ELSE
    BEGIN
        SELECT @NewPageID = ISNULL(MAX(PageID), 0) + 1 FROM dbo.Pages;

        INSERT INTO dbo.Pages (PageID, ParentPageID, Path, Type, Name, MenuInitials, Controller, Action, ShowMenu, RoleType, Icon)
        VALUES (@NewPageID, @ReportsParentPageID, @Path, 'Report', 'Competitor Activity Grid', @Path, 'Reports', 'CompetitorActivityGrid', 1, NULL, 'icon-film');

        PRINT 'Inserted Pages row ' + CAST(@NewPageID AS VARCHAR) + ' ("Competitor Activity Grid") under ParentPageID ' + CAST(@ReportsParentPageID AS VARCHAR) + '.';

        -- Grant it to whichever roles can already see the sibling "Competitor
        -- Report" page - i.e. the same audience that can already reach that
        -- report today. If Competitor Report itself has no RolePages entries
        -- (e.g. it was never added to this menu either), fall back to every
        -- role that has at least one page under the same Reports parent.
        DECLARE @SourcePageID INT;
        SELECT @SourcePageID = PageID FROM dbo.Pages WHERE Controller = 'Reports' AND Action = 'CompetitorReport';

        IF @SourcePageID IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.RolePages WHERE PageID = @SourcePageID)
        BEGIN
            INSERT INTO dbo.RolePages (RoleID, PageID, [Access], LastUpdate, [Read], [Write], [Update], [Delete], Status)
            SELECT RoleID, @NewPageID, [Access], GETDATE(), [Read], [Write], [Update], [Delete], Status
            FROM dbo.RolePages
            WHERE PageID = @SourcePageID;

            PRINT 'Copied RolePages access from Competitor Report (PageID ' + CAST(@SourcePageID AS VARCHAR) + ') to the new page for ' + CAST(@@ROWCOUNT AS VARCHAR) + ' role(s).';
        END
        ELSE
        BEGIN
            INSERT INTO dbo.RolePages (RoleID, PageID, [Access], LastUpdate, [Read], [Write], [Update], [Delete], Status)
            SELECT DISTINCT RoleID, @NewPageID, '1000', GETDATE(), 1, 0, 0, 0, 1
            FROM dbo.RolePages
            WHERE PageID IN (SELECT PageID FROM dbo.Pages WHERE ParentPageID = @ReportsParentPageID);

            PRINT 'Competitor Report has no RolePages entries to copy from, so instead granted read-only access on the new page to every role that already has a page under the same Reports parent (PageID ' + CAST(@ReportsParentPageID AS VARCHAR) + '), ' + CAST(@@ROWCOUNT AS VARCHAR) + ' role(s). Review dbo.RolePages for PageID ' + CAST(@NewPageID AS VARCHAR) + ' afterwards and adjust per-role if this is too broad or too narrow - Admin-role users see every page regardless via a separate PageID=0 wildcard, so they need no row here.';
        END
    END
END
