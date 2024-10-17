using Pulumi.AzureAD.Outputs;

using PulumiInfra.Utilities;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulumiInfra.Config;
public record ResourceUsersConfig(
    string GroupName,
    ImmutableArray<GetUsersUserResult> Users);

public class ResourceUsersConfigDto : ConfigDtoBase<ResourceUsersConfig>
{
    public string? GroupName { get; set; }
    public string?[]? Users { get; set; }

    public override ResourceUsersConfig GenerateValidConfigObject()
    {
        if (!string.IsNullOrWhiteSpace(GroupName)
            && Users != null && Users.All(x => !string.IsNullOrWhiteSpace(x)))
        {
            var userEmails = Users.ToImmutableArray();
            var users = AzureUtilities.LoadUsersFromAzureAd(userEmails);

            return new(GroupName, users);
        }

        throw new Exception($"{GetType().Name} has invalid config");
    }
}
