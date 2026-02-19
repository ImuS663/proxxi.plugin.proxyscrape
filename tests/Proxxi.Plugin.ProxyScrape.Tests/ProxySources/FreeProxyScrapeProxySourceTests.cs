using Proxxi.Plugin.ProxyScrape.ProxySources;
using Proxxi.Plugin.Sdk.Models;
using Proxxi.Plugin.Sdk.ProxySources;

using RichardSzalay.MockHttp;

namespace Proxxi.Plugin.ProxyScrape.Tests.ProxySources;

[TestFixture(TestOf = typeof(FreeProxyScrapeProxySource))]
public class FreeProxyScrapeProxySourceTests
{
    private const string Csv = """
                               alive,alive_since,anonymity,average_timeout,first_seen,ip_data_as,ip_data_asname,ip_data_city,ip_data_continent,ip_data_continentCode,ip_data_country,ip_data_countryCode,ip_data_district,ip_data_hosting,ip_data_isp,ip_data_lat,ip_data_lon,ip_data_mobile,ip_data_org,ip_data_proxy,ip_data_regionName,ip_data_status,ip_data_timezone,ip_data_zip,ip_data_last_update,last_seen,port,protocol,proxy,ssl,timeout,times_alive,times_dead,uptime,ip
                               true,1771424079.173,elite,421.06800392487,1751504745.7559,"AS5391 Hrvatski Telekom d.d.",T-HT,Zagreb,Europe,EU,Croatia,HR,,false,"Hrvatski Telekom d.d.",45.8293,15.9793,false,"Hrvatski Telekom d.d",false,"City of Zagreb",success,Europe/Zagreb,10000,1770868060,1771427240.8037,8000,http,195.29.229.211:8000,false,79.46252822876,54316,4499,92.350590835671,195.29.229.211
                               true,1771426832.3674,elite,2509.4412012996,1765106004.8046,"AS212552 BitCommand LLC",BitCommand,Paris,Europe,EU,France,FR,,false,"BitCommand LLC",48.8558,2.3494,true,"Deployish Limited",true,Île-de-France,success,Europe/Paris,75001,1771155033,1771427268.1039,443,http,202.133.88.173:443,true,182.74974822998,49633,33446,59.741932377616,202.133.88.173
                               true,1771427285.3046,elite,4497.6178023378,1711267646.9683,"AS17995 PT iForte Global Internet",SOLUSINET-AS-ID,"Jakarta Pusat",Asia,AS,Indonesia,ID,,false,"PT iForte Global Internet",-6.1788,106.8285,false,,true,Jakarta,success,Asia/Jakarta,10340,1771314399,1771427285.3046,1080,socks4,202.51.124.166:1080,true,900.77710151672,10701,539715,1.9441658672713,202.51.124.166
                               true,1771427263.9785,elite,6889.1146104067,1716262937.8119,"AS138915 Kaopu Cloud HK Limited",KAOPU-HK,Minkler,"North America",NA,"United States",US,,false,"Kaopu Cloud HK Limited",36.7783,-119.418,false,LightNode-US,true,California,success,America/Los_Angeles,,1771127441,1771427263.9785,4006,socks5,38.54.101.254:4006,true,791.42498970032,1963,188439,1.0309765653722,38.54.101.254
                               """;

    private HttpClient _httpClient;

    [SetUp]
    public void SetUp()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.When("https://api.proxyscrape.com/*")
            .Respond("application/csv", Csv);

        _httpClient = mockHttp.ToHttpClient();
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    [Test]
    public async Task FetchAsync_BatchMode_ReturnsExpectedProxies()
    {
        IBatchProxySource proxySource = new FreeProxyScrapeProxySource(_httpClient);

        await proxySource.InitializeAsync(new Dictionary<string, string>());

        var result = (await proxySource.FetchAsync()).ToArray();

        Assert.That(result, Has.Length.EqualTo(4));

        Assert.That(result, Contains.Item(new Proxy(Host: "195.29.229.211", Port: 8000, Protocols: Protocols.Http)));
        Assert.That(result, Contains.Item(new Proxy(Host: "202.133.88.173", Port: 443, Protocols: Protocols.Https)));
        Assert.That(result, Contains.Item(new Proxy(Host: "202.51.124.166", Port: 1080, Protocols: Protocols.Socks4)));
        Assert.That(result, Contains.Item(new Proxy(Host: "38.54.101.254", Port: 4006, Protocols: Protocols.Socks5)));
    }

    [Test]
    public async Task FetchAsync_StreamMode_ReturnsExpectedProxies()
    {
        IStreamProxySource proxySource = new FreeProxyScrapeProxySource(_httpClient);

        await proxySource.InitializeAsync(new Dictionary<string, string>());

        var result = await proxySource.FetchAsync().ToArrayAsync();

        Assert.That(result, Has.Length.EqualTo(4));

        Assert.That(result, Contains.Item(new Proxy(Host: "195.29.229.211", Port: 8000, Protocols: Protocols.Http)));
        Assert.That(result, Contains.Item(new Proxy(Host: "202.133.88.173", Port: 443, Protocols: Protocols.Https)));
        Assert.That(result, Contains.Item(new Proxy(Host: "202.51.124.166", Port: 1080, Protocols: Protocols.Socks4)));
        Assert.That(result, Contains.Item(new Proxy(Host: "38.54.101.254", Port: 4006, Protocols: Protocols.Socks5)));
    }
}