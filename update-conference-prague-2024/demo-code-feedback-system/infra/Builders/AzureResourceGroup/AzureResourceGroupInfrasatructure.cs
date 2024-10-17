using Pulumi.AzureAD;
using Pulumi.AzureNative.Resources;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static PulumiInfra.Builders.AzureResourceGroup.AzureResourceGroupInfrasatructure;

namespace PulumiInfra.Builders.AzureResourceGroup;

public record AzureResourceGroupInfrasatructure(
    ResourceGroupInfrastructure ResourceGroupInfra,
    AdGroupInfrastructure AdGroupInfra)
{
    public record ResourceGroupInfrastructure(ResourceGroup ResourceGroup);
    public record AdGroupInfrastructure(Group Group, string GroupName, ImmutableArray<GroupMember> GroupMembers);
}
