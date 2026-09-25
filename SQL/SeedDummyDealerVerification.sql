-- Inserts one dummy Dealer Verification Form row so the "Dealer Verification Form"
-- Doc Type on Claims Detail can be tested end-to-end before the mobile app has a
-- real submission screen for it.
--
-- Requires SQL/CreateTbl_DealerVerification.sql to have already been run.
--
-- Picks 'Test MK SO' if it exists (the SO already used for other test data in this
-- app), otherwise falls back to any active Sale Officer. Picture is set to an
-- external placeholder image URL purely so the Details view has something to show -
-- real submissions will have an actual uploaded /Images/RetailerImages/... path.

DECLARE @SOID INT, @CustomerID INT, @TradePartyID INT;

SELECT TOP 1 @SOID = ID FROM dbo.SaleOfficers WHERE Name = 'Test MK SO' AND IsActive = 1;
IF @SOID IS NULL
    SELECT TOP 1 @SOID = ID FROM dbo.SaleOfficers WHERE IsActive = 1 ORDER BY ID;

IF @SOID IS NULL
BEGIN
    RAISERROR('No active Sale Officer found to attach dummy Dealer Verification data to.', 16, 1);
    RETURN;
END

SELECT TOP 1 @CustomerID = ID FROM dbo.Retailers WHERE SaleOfficerID = @SOID AND IsActive = 1 AND Status = 1 ORDER BY ID;
IF @CustomerID IS NULL
    SELECT TOP 1 @CustomerID = ID FROM dbo.Retailers WHERE IsActive = 1 AND Status = 1 ORDER BY ID;

IF @CustomerID IS NULL
BEGIN
    RAISERROR('No active Retailer found to use as the dummy Customer.', 16, 1);
    RETURN;
END

SELECT TOP 1 @TradePartyID = ID FROM dbo.Retailers WHERE IsActive = 1 AND Status = 1 AND ID <> @CustomerID ORDER BY ID;

INSERT INTO dbo.Tbl_DealerVerification
    (SOID, CustomerID, TradePartyID, DateSelected, Picture, Status, CreatedOn, IsActive)
VALUES
    (@SOID, @CustomerID, @TradePartyID, GETDATE(), 'https://placehold.co/600x400?text=Dealer+Verification', 'InProgress', GETDATE(), 1);

DECLARE @NewID INT = SCOPE_IDENTITY();

SELECT dv.ID, so.Name AS SaleOfficerName, r.ShopName AS CustomerShopName, dv.Picture, dv.DateSelected
FROM dbo.Tbl_DealerVerification dv
JOIN dbo.SaleOfficers so ON dv.SOID = so.ID
JOIN dbo.Retailers r ON dv.CustomerID = r.ID
WHERE dv.ID = @NewID;
GO
