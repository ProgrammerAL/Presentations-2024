using Pulumi;
using Pulumi.AzureNative.Authorization;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulumiInfra.Config;
public record PersistenceConfig(
    string SqlAdminPassword);

public class PersistenceConfigDto : ConfigDtoBase<PersistenceConfig>
{
    public string? SqlAdminPassword { get; set; }

    public override PersistenceConfig GenerateValidConfigObject()
    {
        if (!string.IsNullOrWhiteSpace(SqlAdminPassword))
        {
            return new(SqlAdminPassword);
        }

        throw new Exception($"{GetType().Name} has invalid config");
    }
}
