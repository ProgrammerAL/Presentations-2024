using System;

namespace PulumiInfra.Config;
public record StaticSiteConfig(
    string StaticSitePath,
    string BlogPostUrl);

public class StaticSiteConfigDto : ConfigDtoBase<StaticSiteConfig>
{
    public string? StaticSitePath { get; set; }
    public string? BlogPostUrl { get; set; }

    public override StaticSiteConfig GenerateValidConfigObject()
    {
        if (!string.IsNullOrWhiteSpace(StaticSitePath)
            && !string.IsNullOrWhiteSpace(BlogPostUrl))
        {
            return new(StaticSitePath, BlogPostUrl);
        }

        throw new Exception($"{GetType().Name} has invalid config");
    }
}
