using System.ComponentModel;

using Proxxi.Plugin.Sdk.Attributes;

namespace Proxxi.Plugin.ProxyScrape.ProxySources;

[ProxySource("imus663.freeproxyscrape", "Free ProxyScrape")]
[Description("This proxy source uses the free ProxyScrape API (https://proxyscrape.com/).")]
public class FreeProxyScrapeProxySource : ProxyScrapeProxySource
{
    protected override string Url => "https://api.proxyscrape.com/v4/free-proxy-list/get?request=getproxies&format=csv";

    public FreeProxyScrapeProxySource() { }

    internal FreeProxyScrapeProxySource(HttpClient httpClient) : base(httpClient) { }

    public override Task InitializeAsync(IReadOnlyDictionary<string, string> parameters,
        CancellationToken ct = default) =>
        Task.CompletedTask;
}