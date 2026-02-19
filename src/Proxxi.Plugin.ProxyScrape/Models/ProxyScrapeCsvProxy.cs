using CsvHelper.Configuration.Attributes;

namespace Proxxi.Plugin.ProxyScrape.Models;

public class ProxyScrapeCsvProxy
{
    [Name("alive")]
    public bool Alive { get; init; }

    [Name("ip")]
    public required string Ip { get; init; }

    [Name("port")]
    public required int Port { get; init; }

    [Name("protocol")]
    public required string Protocol { get; init; }

    [Name("ssl")]
    public bool Ssl { get; init; }
}