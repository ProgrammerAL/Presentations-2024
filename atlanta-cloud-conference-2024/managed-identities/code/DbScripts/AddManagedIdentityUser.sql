
CREATE USER [] FROM EXTERNAL PROVIDER

ALTER ROLE db_datareader ADD MEMBER [];
ALTER ROLE db_datawriter ADD MEMBER [];


-- Example for Azure Function
-- CREATE USER [functions-appe6d823bb] FROM EXTERNAL PROVIDER

-- ALTER ROLE db_datareader ADD MEMBER [functions-appe6d823bb];
-- ALTER ROLE db_datawriter ADD MEMBER [functions-appe6d823bb];

