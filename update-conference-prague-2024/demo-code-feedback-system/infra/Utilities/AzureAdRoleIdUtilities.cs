
using Pulumi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulumiInfra.Utilities;

public static class AzureAdRoleIdUtilities
{
    public const string AzureBuiltInReaderRoleId = "acdd72a7-3385-48ef-bd42-f606fba81ae7";

    public static string GenerateAzureReaderRoleId(string subscriptionId)
    {
        return AzureUtilities.GenerateRoleDefinitionId(subscriptionId, AzureBuiltInReaderRoleId);
    }
}
