using System.Text.Json;
using System.Text.Json.Serialization;

namespace DomeneshopClient.Models;

/// <summary>
/// Base DNS record. Use a concrete subtype when creating records:
/// <see cref="ADnsRecord"/>, <see cref="AAAADnsRecord"/>, <see cref="CnameDnsRecord"/>,
/// <see cref="AnameDnsRecord"/>, <see cref="NsDnsRecord"/>, <see cref="TxtDnsRecord"/>,
/// <see cref="MxDnsRecord"/>, <see cref="SrvDnsRecord"/>, <see cref="CaaDnsRecord"/>,
/// <see cref="TlsaDnsRecord"/>, <see cref="DsDnsRecord"/>, <see cref="SshfpDnsRecord"/>.
/// </summary>
public abstract class DnsRecord
{
    /// <summary>Record ID. Assigned by the API; omit when creating a new record.</summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; init; }

    /// <summary>The subdomain host part, e.g. "@", "www", "*".</summary>
    [JsonPropertyName("host")]
    public string Host { get; init; } = string.Empty;

    [JsonPropertyName("ttl")]
    public int Ttl { get; init; } = 3600;

    [JsonPropertyName("type")]
    public abstract string Type { get; }
}

/// <summary>Base for records whose only type-specific field is <c>data</c>.</summary>
public abstract class DataDnsRecord : DnsRecord
{
    [JsonPropertyName("data")]
    public string Data { get; init; } = string.Empty;
}

/// <summary>A record — IPv4 address.</summary>
public sealed class ADnsRecord : DataDnsRecord
{
    public override string Type => "A";
}

/// <summary>AAAA record — IPv6 address.</summary>
public sealed class AAAADnsRecord : DataDnsRecord
{
    public override string Type => "AAAA";
}

/// <summary>CNAME record — canonical hostname alias.</summary>
public sealed class CnameDnsRecord : DataDnsRecord
{
    public override string Type => "CNAME";
}

/// <summary>ANAME (ALIAS) record — root-compatible hostname alias.</summary>
public sealed class AnameDnsRecord : DataDnsRecord
{
    public override string Type => "ANAME";
}

/// <summary>NS record — authoritative name server.</summary>
public sealed class NsDnsRecord : DataDnsRecord
{
    public override string Type => "NS";
}

/// <summary>TXT record — arbitrary text.</summary>
public sealed class TxtDnsRecord : DataDnsRecord
{
    public override string Type => "TXT";
}

/// <summary>MX record — mail exchange.</summary>
public sealed class MxDnsRecord : DataDnsRecord
{
    public override string Type => "MX";

    [JsonPropertyName("priority")]
    public int Priority { get; init; }
}

/// <summary>SRV record — service locator.</summary>
public sealed class SrvDnsRecord : DataDnsRecord
{
    public override string Type => "SRV";

    [JsonPropertyName("priority")]
    public int Priority { get; init; }

    [JsonPropertyName("weight")]
    public int Weight { get; init; }

    [JsonPropertyName("port")]
    public int Port { get; init; }
}

/// <summary>CAA record — certification authority authorization.</summary>
public sealed class CaaDnsRecord : DnsRecord
{
    public override string Type => "CAA";

    /// <summary>0 or 128 (critical).</summary>
    [JsonPropertyName("flags")]
    public int Flags { get; init; }

    /// <summary>"issue", "issuewild", or "iodef".</summary>
    [JsonPropertyName("tag")]
    public string Tag { get; init; } = string.Empty;

    [JsonPropertyName("data")]
    public string Data { get; init; } = string.Empty;
}

/// <summary>TLSA record — TLS authentication.</summary>
public sealed class TlsaDnsRecord : DnsRecord
{
    public override string Type => "TLSA";

    /// <summary>Certificate usage field (0–3).</summary>
    [JsonPropertyName("usage")]
    public int Usage { get; init; }

    /// <summary>Selector field (0–1).</summary>
    [JsonPropertyName("selector")]
    public int Selector { get; init; }

    /// <summary>Matching type field (0–2).</summary>
    [JsonPropertyName("dtype")]
    public int Dtype { get; init; }

    /// <summary>Certificate association data (hex).</summary>
    [JsonPropertyName("data")]
    public string Data { get; init; } = string.Empty;
}

/// <summary>DS record — delegation signer for DNSSEC.</summary>
public sealed class DsDnsRecord : DnsRecord
{
    public override string Type => "DS";

    [JsonPropertyName("key_tag")]
    public int KeyTag { get; init; }

    /// <summary>Algorithm number.</summary>
    [JsonPropertyName("alg")]
    public int Alg { get; init; }

    [JsonPropertyName("digest_type")]
    public int DigestType { get; init; }

    /// <summary>Digest value (hex).</summary>
    [JsonPropertyName("digest")]
    public string Digest { get; init; } = string.Empty;
}

/// <summary>SSHFP record — SSH public key fingerprint.</summary>
public sealed class SshfpDnsRecord : DnsRecord
{
    public override string Type => "SSHFP";

    /// <summary>1 = RSA, 2 = DSA, 3 = ECDSA, 4 = Ed25519.</summary>
    [JsonPropertyName("algorithm")]
    public int Algorithm { get; init; }

    /// <summary>1 = SHA-1, 2 = SHA-256.</summary>
    [JsonPropertyName("hash_type")]
    public int HashType { get; init; }

    /// <summary>Fingerprint (hex).</summary>
    [JsonPropertyName("data")]
    public string Data { get; init; } = string.Empty;
}

/// <summary>
/// Handles polymorphic JSON for <see cref="DnsRecord"/>.
/// CanConvert is restricted to the abstract base type so that the default STJ serializer
/// is used for concrete subtypes — avoiding infinite recursion in both Read and Write paths.
/// </summary>
internal sealed class DnsRecordConverter : JsonConverter<DnsRecord>
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(DnsRecord);

    public override DnsRecord? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        var rawText = root.GetRawText();

        if (!root.TryGetProperty("type", out var typeElement))
            throw new JsonException("DNS record is missing the 'type' property.");

        return typeElement.GetString() switch
        {
            "A"     => JsonSerializer.Deserialize<ADnsRecord>(rawText, options),
            "AAAA"  => JsonSerializer.Deserialize<AAAADnsRecord>(rawText, options),
            "ANAME" => JsonSerializer.Deserialize<AnameDnsRecord>(rawText, options),
            "CNAME" => JsonSerializer.Deserialize<CnameDnsRecord>(rawText, options),
            "NS"    => JsonSerializer.Deserialize<NsDnsRecord>(rawText, options),
            "MX"    => JsonSerializer.Deserialize<MxDnsRecord>(rawText, options),
            "SRV"   => JsonSerializer.Deserialize<SrvDnsRecord>(rawText, options),
            "TXT"   => JsonSerializer.Deserialize<TxtDnsRecord>(rawText, options),
            "CAA"   => JsonSerializer.Deserialize<CaaDnsRecord>(rawText, options),
            "TLSA"  => JsonSerializer.Deserialize<TlsaDnsRecord>(rawText, options),
            "DS"    => JsonSerializer.Deserialize<DsDnsRecord>(rawText, options),
            "SSHFP" => JsonSerializer.Deserialize<SshfpDnsRecord>(rawText, options),
            var unknown => throw new JsonException($"Unknown DNS record type: '{unknown}'.")
        };
    }

    public override void Write(Utf8JsonWriter writer, DnsRecord value, JsonSerializerOptions options)
    {
        // Serialize using the concrete runtime type. Because CanConvert returns false for all
        // derived types, STJ will use the default object serializer here — no recursion.
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

