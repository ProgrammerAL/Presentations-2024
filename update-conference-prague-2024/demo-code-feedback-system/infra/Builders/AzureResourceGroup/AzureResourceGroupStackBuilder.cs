using Pulumi;

using System;
using System.Threading.Tasks;
using System.Linq;

using Pulumi.AzureNative.Resources;


using AzureNative = Pulumi.AzureNative;
using static PulumiInfra.Builders.AzureResourceGroup.AzureResourceGroupInfrasatructure;
using System.Collections.Generic;
using System.Collections.Immutable;
using PulumiInfra.Builders.AzureResourceGroup;
using PulumiInfra.Config;
using Pulumi.AzureAD;
using PulumiInfra.Utilities;

public record AzureResourceGroupStackBuilder(
    GlobalConfig GlobalConfig)
{
    public AzureResourceGroupInfrasatructure GenerateResources()
    {
        var resourceGroupInfra = GenerateResourceGroup();
        var adGroupsInfra = GenerateAdGroup(resourceGroupInfra);

        AssignRbacAccess(resourceGroupInfra, adGroupsInfra);

        return new AzureResourceGroupInfrasatructure(resourceGroupInfra, adGroupsInfra);
    }

    private ResourceGroupInfrastructure GenerateResourceGroup()
    {
        var resourceGroup = new ResourceGroup(GlobalConfig.ApiConfig.ResourceGroupName, new ResourceGroupArgs
        {
            Location = GlobalConfig.ApiConfig.Location,
            Tags = new InputMap<string> {
                { "environment", GlobalConfig.ServiceConfig.Environment },
                { "service-version", GlobalConfig.ServiceConfig.Version }
            }
        });

        return new ResourceGroupInfrastructure(resourceGroup);
    }

    private AdGroupInfrastructure GenerateAdGroup(ResourceGroupInfrastructure resourceGroupInfra)
    {
        var group = new Group(GlobalConfig.ResourceUsersConfig.GroupName, new GroupArgs
        {
            DisplayName = GlobalConfig.ResourceUsersConfig.GroupName,
            Description = resourceGroupInfra.ResourceGroup.Name.Apply(x => $"Users with access to resources in Resource Group {x}. Generally have access to 'read' the resources, but sometimes the ability to also 'write'."),
            SecurityEnabled = true,
            PreventDuplicateNames = true,
        });

        var groupMembers = new List<GroupMember>();
        foreach (var user in GlobalConfig.ResourceUsersConfig.Users)
        {
            var sanitizedUserName = user.DisplayName.Replace(" ", "-");
            var memberName = $"{GlobalConfig.ResourceUsersConfig.GroupName}-{sanitizedUserName}";

            var groupMember = new GroupMember(memberName, new()
            {
                GroupObjectId = group.ObjectId,
                MemberObjectId = user.ObjectId
            });

            groupMembers.Add(groupMember);
        }

        return new AdGroupInfrastructure(group, GlobalConfig.ResourceUsersConfig.GroupName, groupMembers.ToImmutableArray());
    }

    private void AssignRbacAccess(ResourceGroupInfrastructure resourceGroupInfra, AdGroupInfrastructure adGroupsInfra)
    {
        //Assign group to be able to read everything in the resource group
        _ = new AzureNative.Authorization.RoleAssignment("arcade-device-management-resource-group-reader-role-assignment", new AzureNative.Authorization.RoleAssignmentArgs
        {
            PrincipalId = adGroupsInfra.Group.Id,
            PrincipalType = AzureNative.Authorization.PrincipalType.Group,
            RoleDefinitionId = AzureAdRoleIdUtilities.GenerateAzureReaderRoleId(GlobalConfig.ApiConfig.ClientConfig.SubscriptionId),
            Scope = resourceGroupInfra.ResourceGroup.Id
        });
    }
}
