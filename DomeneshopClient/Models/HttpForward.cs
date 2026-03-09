using System.Text.Json.Serialization;

namespace DomeneshopClient.Models;

public sealed class HttpForward
{
    /// <summary>The subdomain host to forward, e.g. "@" for root, "www" for www.</summary>
    [JsonPropertyName("host")]
    public string Host { get; init; } = string.Empty;

    /// <summary>Whether to wrap the destination URL in an iframe instead of issuing a redirect.</summary>
    [JsonPropertyName("frame")]
    public bool Frame { get; init; }

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}

