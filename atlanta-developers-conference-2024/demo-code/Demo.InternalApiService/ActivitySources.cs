using System.Diagnostics;

namespace Demo.InternalApiService;

public static class ActivitySources
{
    public const string ActivitySourcePrefix = "internal-api-source";
    public const string ActivitySourcesNameWildcard = $"{ActivitySourcePrefix}.*";
    public static readonly ActivitySource AppActivitySource = new($"{ActivitySourcePrefix}.root");
}
