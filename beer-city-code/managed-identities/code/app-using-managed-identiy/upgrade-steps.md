

1. Enable the System Manged Identity for the Azure Function
1. On Server, go to "Microsoft Entra ID" section, set an admin
1. Grant the Azure Function user DB read and write permissions
1. Set the DB Sql Connection String to something like: Data Source=SQL_SERVER_NAME; Initial Catalog=DB_NAME; Authentication=Active Directory Managed Identity; Encrypt=True


