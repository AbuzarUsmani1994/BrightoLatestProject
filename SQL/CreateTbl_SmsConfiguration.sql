-- Backs the new "SMS Configuration" screen (Setup/SmsConfiguration). Moves the
-- SMS gateway URL/login/password (previously hardcoded in CBSSmsSender.cs /
-- Web.config <appSettings>) and the SMS template text (previously the
-- hardcoded WelcomeMessageText constant in RetailerRegistrationController.cs)
-- into the database so they can be edited from this screen instead of a
-- redeploy.
--
-- Single-row table (ID is always 1) - no IDENTITY needed, matching this
-- codebase's existing manual-ID convention elsewhere.
--
-- The application code (CBSSmsSender, RetailerRegistrationController's welcome
-- SMS) falls back to the existing Web.config settings / hardcoded template
-- text whenever the corresponding column here is NULL/empty, so applying this
-- script alone does not change any SMS behavior - nothing breaks before the
-- row is actually filled in via the new screen. I don't have access to your
-- live Web.config secrets (CBSLoginId/CBSLoginPassword/CBSMask) to seed the
-- real values myself, so after running this, open the new SMS Configuration
-- screen and enter your real URL/Login/Password/Templates once.

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tbl_SmsConfiguration' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tbl_SmsConfiguration
    (
        ID          INT             NOT NULL PRIMARY KEY,
        Url         NVARCHAR(300)   NULL,
        LoginId     NVARCHAR(200)   NULL,
        Password    NVARCHAR(200)   NULL,
        Mask        NVARCHAR(100)   NULL,
        Template1   NVARCHAR(1000)  NULL,
        Template2   NVARCHAR(1000)  NULL,
        Template3   NVARCHAR(1000)  NULL,
        Template4   NVARCHAR(1000)  NULL,
        Template5   NVARCHAR(1000)  NULL,
        UpdatedOn   DATETIME        NULL,
        UpdatedBy   INT             NULL
    );

    PRINT 'Created dbo.Tbl_SmsConfiguration.';
END
ELSE
BEGIN
    PRINT 'dbo.Tbl_SmsConfiguration already exists - skipped create.';
END

IF NOT EXISTS (SELECT 1 FROM dbo.Tbl_SmsConfiguration WHERE ID = 1)
BEGIN
    INSERT INTO dbo.Tbl_SmsConfiguration (ID, UpdatedOn)
    VALUES (1, GETDATE());

    PRINT 'Inserted the single placeholder row (ID = 1), fields left blank. Fill them in via the SMS Configuration screen - until then, the app keeps using Web.config/the hardcoded welcome template.';
END
ELSE
BEGIN
    PRINT 'Row ID = 1 already exists - left untouched.';
END
