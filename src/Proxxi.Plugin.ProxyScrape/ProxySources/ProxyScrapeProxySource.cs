using System.Globalization;
using System.Runtime.CompilerServices;

using CsvHelper;
using CsvHelper.Configuration;

using Proxxi.Plugin.ProxyScrape.Models;
using Proxxi.Plugin.Sdk.Models;
using Proxxi.Plugin.Sdk.ProxySources;

namespace Proxxi.Plugin.ProxyScrape.ProxySources;

public abstract class ProxyScrapeProxySource : IBatchProxySource, IStreamProxySource
{
    protected abstract string Url { get; }

    private readonly HttpClient _httpClient;

    private static readonly CsvConfiguration Config = new(CultureInfo.InvariantCulture) { HasHeaderRecord = true };

    protected ProxyScrapeProxySource() : this(new HttpClient()) { }

    internal ProxyScrapeProxySource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public ValueTask DisposeAsync()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    public abstract Task InitializeAsync(IReadOnlyDictionary<string, string> parameters,
        CancellationToken ct = default);

    async IAsyncEnumerable<Proxy> IStreamProxySource.FetchAsync([EnumeratorCancellation] CancellationToken ct)
    {
        await using var stream = await _httpClient.GetStreamAsync(Url, ct);

        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, Config);

        await foreach (var proxy in csv.GetRecordsAsync<ProxyScrapeCsvProxy>(ct))
        {
            if (!proxy.Alive)
                continue;

            yield return new Proxy(Host: proxy.Ip, Port: proxy.Port, Protocols: GetProtocol(proxy));
        }
    }

    async Task<IEnumerable<Proxy>> IBatchProxySource.FetchAsync(CancellationToken ct)
    {
        var proxies = new List<Proxy>();

        var csvData = await _httpClient.GetStringAsync(Url, ct);

        using var reader = new StringReader(csvData);
        using var csv = new CsvReader(reader, Config);

        foreach (var proxy in csv.GetRecords<ProxyScrapeCsvProxy>())
        {
            if (ct.IsCancellationRequested)
                break;

            if (!proxy.Alive)
                continue;

            proxies.Add(new Proxy(
                Host: proxy.Ip,
                Port: proxy.Port,
                Protocols: GetProtocol(proxy)));
        }

        return proxies;
    }

    private static Protocols GetProtocol(ProxyScrapeCsvProxy proxy)
    {
        Protocols protocols = Protocols.None;

        switch (proxy.Protocol)
        {
            case "http" when !proxy.Ssl:
                protocols |= Protocols.Http;
                break;
            case "http" when proxy.Ssl:
                protocols |= Protocols.Https;
                break;
            case "socks4":
                protocols |= Protocols.Socks4;
                break;
            case "socks5":
                protocols |= Protocols.Socks5;
                break;
        }

        return protocols;
    }
}