using System.ComponentModel;

using Proxxi.Plugin.Sdk.Attributes;

namespace Proxxi.Plugin.ProxyScrape.ProxySources;

[ProxySource("imus663.premproxyscrape", "Premium ProxyScrape")]
[Description("This proxy source uses the premium and dedicated ProxyScrape API (https://proxyscrape.com/).")]
[ParameterProxySource("apiKey", "Your premium API key from ProxyScrape.", true)]
public class PremProxyScrapeProxySource : ProxyScrapeProxySource
{
    private const string UrlForm =
        "https://api.proxyscrape.com/v2/account/datacenter_shared/proxy-list?auth={0}&request=getproxies&format=csv";

    private string? _apiKey;

    protected override string Url =>
        string.Format(UrlForm, _apiKey ?? throw new InvalidOperationException("Proxy source not initialized."));

    public PremProxyScrapeProxySource() { }

    internal PremProxyScrapeProxySource(HttpClient httpClient) : base(httpClient) { }

    public override Task InitializeAsync(IReadOnlyDictionary<string, string> parameters, CancellationToken ct = default)
    {
        if (!parameters.TryGetValue("apiKey", out var apiKey))
            throw new ArgumentException("Parameter 'apiKey' is a required parameter for this proxy source.");

        _apiKey = apiKey;

        return Task.CompletedTask;
    }
}