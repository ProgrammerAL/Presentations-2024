# Setup Instructions

## Initial setup for demo using Connection Strings

1. In a terminal, open up the `~/code/app-original` folder and run `pwsh ./package-for-deployment.ps1` to create the packages that will be deployed by Pulumi in the next step
1. Run `pulumi up` in the `~/code/infra` folder (keep this window open to make it easy to get outputs for later step)
1. Add the SQL and Storage connection strings to the code from the Pulumi Outputs
1. In the Azure Portal, sign in to the SQL Database using the `MySqlAdminUser` account with the `aksjn&JHLB!!BNHJKasd` password
1. Run the `~/code/DbScripts/DbInit.sql` file in the newly created SQL Database
1. For local demo, update the `~/code/app-original/FeedbackFunctionsApp/local.settings.json` file with the pulumi outputs from running `pulumi up` above
   1. Update `StorageConfig__TableEndpoint` from the `StorageConnectionString` pulumi output
   1. Update `StorageConfig__SqlConnectionString` from the `SqlConnectionString` pulumi output
1. Run the `~/code/app-original/package-for-deployment.ps1` PowerShell script. This creates the files that will be published to Azure for the demo.
1. Open the `~/code/app-original/FeedbackApp.sln` solution file
1. Right-Click/Publish the `FeedbackFunctionsApp`

## For demo using Managed Identities

1. Make code change so Function can use `DefaultAzureCredential()` to talk to Azure Table storage
1. In Azure Portal, go to the table storage, add `Table Contributor` for yourself
1. Change Sql Connection String to look like it should for a managed identity connection
   1. `Server=tcp:sql-server8435a3fa.database.windows.net,1433;Initial Catalog=mydatabase;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;`
   1. Can copy/paste it from the /Settings/Connection Strings section in the Azure Portal for the Database
1. In Azure Portal, go to the Sql Server, then Settings, then Microsoft Entra ID
1. Set the admin to yourself, then click save
1. In Azure Portal, go to the Sql Database and sign in as yourself
1. Run the `~/code/AddManagedIdentity.sql` script, but replace the member name with your User Principal Name of the admin you just signed in with
1. Deploy the Azure Function, notice the app does not work anymore. Add the Managed Identities to the Function.
