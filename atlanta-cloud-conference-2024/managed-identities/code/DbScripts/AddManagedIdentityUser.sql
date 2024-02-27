
-- DECLARE @UserName nvarcahr(MAX);
-- SET @UserName = '3bd8baef-16f1-4a59-bf73-bc3743c1f868';

CREATE USER [functions-appe6d823bb] FROM EXTERNAL PROVIDER

ALTER ROLE db_datareader ADD MEMBER [functions-appe6d823bb];
ALTER ROLE db_datawriter ADD MEMBER [functions-appe6d823bb];