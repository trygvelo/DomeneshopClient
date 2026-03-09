using System.Text.Json.Serialization;

namespace DomeneshopClient.Models;

public sealed class Domain
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("domain")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("expiry_date")]
    public string? ExpiryDate { get; init; }

    [JsonPropertyName("registered_date")]
    public string? RegisteredDate { get; init; }

    [JsonPropertyName("renew")]
    public bool Renew { get; init; }

    [JsonPropertyName("registrant")]
    public string Registrant { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("nameservers")]
    public IReadOnlyList<string> Nameservers { get; init; } = [];

    [JsonPropertyName("services")]
    public DomainServices Services { get; init; } = new();
}

public sealed class DomainServices
{
    [JsonPropertyName("registrar")]
    public bool Registrar { get; init; }

    [JsonPropertyName("dns")]
    public bool Dns { get; init; }

    [JsonPropertyName("email")]
    public bool Email { get; init; }

    /// <summary>Webhotel plan, e.g. "none", "webhotel", "webhotel_email". Null when not applicable.</summary>
    [JsonPropertyName("webhotel")]
    public string? Webhotel { get; init; }
}

