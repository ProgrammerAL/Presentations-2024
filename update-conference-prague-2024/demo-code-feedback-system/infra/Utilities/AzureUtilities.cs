using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Pulumi.AzureAD.Outputs;
using Pulumi.AzureAD;
using Pulumi;
using System.Collections.Immutable;

namespace PulumiInfra.Utilities;

public static class AzureUtilities
{
    public static string ParseResourceGroupFromResourceId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return string.Empty;
        }

        string text = "/resourceGroups/";
        int num = id.IndexOf(text, StringComparison.OrdinalIgnoreCase);
        if (num == -1 && id.StartsWith("resourceGroups/", StringComparison.OrdinalIgnoreCase))
        {
            text = "resourceGroups/";
            num = 0;
        }

        if (num >= 0)
        {
            num += text.Length;
            int num2 = id.IndexOf('/', num);
            if (num2 == -1)
            {
                num2 = id.Length;
            }

            int length = num2 - num;
            return id.Substring(num, length);
        }

        return string.Empty;
    }

    public static ImmutableArray<GetUsersUserResult> LoadUsersFromAzureAd(IEnumerable<string> emails)
    {
        return GetUsers.InvokeAsync(new GetUsersArgs
        {
            UserPrincipalNames = emails.ToList()
        }).Result.Users;
    }

    public static string GenerateRoleDefinitionId(string subscriptionId, string roleId)
    {
        return "/subscriptions/" + subscriptionId + "/providers/Microsoft.Authorization/roleDefinitions/" + roleId;
    }
}