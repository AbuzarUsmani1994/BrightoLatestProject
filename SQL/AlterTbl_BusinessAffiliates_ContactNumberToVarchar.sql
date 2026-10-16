-- Tbl_BusinessAffiliates.ContactNumber was INT, which cannot hold a phone number:
-- any number starting with 0 (every Pakistani mobile number) silently loses that
-- leading zero, and an 11-digit number read as an integer (e.g. 03324619571 ->
-- 3324619571) overflows Int32's ~2.147 billion max, which breaks JSON
-- deserialization of the whole request and crashes
-- UpdateBusinessAffiliateController/BusinessAffiliatesRegistrationController with
-- an unrelated-looking EF6 reflection error ("Non-static method requires a
-- target"), since the request model binds to null instead of a clean 400.
--
-- Existing INT values convert to their string form automatically (SQL Server
-- allows ALTER COLUMN int -> varchar in place) - no data is lost, though any
-- number that already lost its leading zero while stored as an int stays that way;
-- this only stops it happening to new saves.

ALTER TABLE dbo.Tbl_BusinessAffiliates ALTER COLUMN ContactNumber VARCHAR(20) NULL;
GO
