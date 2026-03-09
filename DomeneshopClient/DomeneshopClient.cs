using System.Text.Json;
using DomeneshopClient.Models;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace DomeneshopClient;

/// <summary>
/// Client for the Domeneshop REST API v0.
/// Obtain a token and secret from https://www.domeneshop.no/admin?view=api.
/// </summary>
public sealed class DomeneshopClient : IDisposable
{
    private const string BaseUrl = "https://api.domeneshop.no/v0";

    private readonly string _token;
    private readonly string _secret;
    private readonly FlurlClient _client;

    public DomeneshopClient(string token, string secret)
    {
        _token = token;
        _secret = secret;

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new DnsRecordConverter());

        _client = new FlurlClient(BaseUrl);
        _client.Settings.JsonSerializer = new DefaultJsonSerializer(jsonOptions);
    }

    private IFlurlRequest Request(params object[] segments) =>
        _client.Request(segments).WithBasicAuth(_token, _secret);

    // -------------------------------------------------------------------------
    // Domains — GET /domains, GET /domains/{domainId}
    // -------------------------------------------------------------------------

    /// <summary>Lists all domains on the account.</summary>
    public Task<List<Domain>> GetDomainsAsync(CancellationToken ct = default) =>
        Request("domains").GetJsonAsync<List<Domain>>(cancellationToken: ct);

    /// <summary>Gets a domain by its numeric ID.</summary>
    public Task<Domain> GetDomainAsync(int domainId, CancellationToken ct = default) =>
        Request("domains", domainId).GetJsonAsync<Domain>(cancellationToken: ct);

    // -------------------------------------------------------------------------
    // Invoices — GET /invoices, GET /invoices/{invoiceNumber}
    // -------------------------------------------------------------------------

    /// <summary>
    /// Lists invoices on the account.
    /// Optionally filter by <paramref name="status"/>: "unpaid", "paid", or "settled".
    /// </summary>
    public Task<List<Invoice>> GetInvoicesAsync(string? status = null, CancellationToken ct = default)
    {
        var req = Request("invoices");
        if (status is not null)
            req = req.SetQueryParam("status", status);
        return req.GetJsonAsync<List<Invoice>>(cancellationToken: ct);
    }

    /// <summary>Gets a specific invoice by its number.</summary>
    public Task<Invoice> GetInvoiceAsync(int invoiceNumber, CancellationToken ct = default) =>
        Request("invoices", invoiceNumber).GetJsonAsync<Invoice>(cancellationToken: ct);

    // -------------------------------------------------------------------------
    // DNS  — /domains/{domainId}/dns[/{recordId}]
    // -------------------------------------------------------------------------

    /// <summary>Lists all DNS records for a domain.</summary>
    public Task<List<DnsRecord>> GetDnsRecordsAsync(int domainId, CancellationToken ct = default) =>
        Request("domains", domainId, "dns").GetJsonAsync<List<DnsRecord>>(cancellationToken: ct);

    /// <summary>
    /// Creates a new DNS record. Returns the record as stored by the API, including its assigned ID.
    /// Pass a concrete subtype such as <see cref="ADnsRecord"/>, <see cref="MxDnsRecord"/>, etc.
    /// </summary>
    public async Task<DnsRecord> CreateDnsRecordAsync(int domainId, DnsRecord record, CancellationToken ct = default)
    {
        var response = await Request("domains", domainId, "dns").PostJsonAsync(record, cancellationToken: ct);
        return await response.GetJsonAsync<DnsRecord>();
    }

    /// <summary>Gets a specific DNS record by its ID.</summary>
    public Task<DnsRecord> GetDnsRecordAsync(int domainId, int recordId, CancellationToken ct = default) =>
        Request("domains", domainId, "dns", recordId).GetJsonAsync<DnsRecord>(cancellationToken: ct);

    /// <summary>Replaces a DNS record. The record type may not be changed.</summary>
    public Task UpdateDnsRecordAsync(int domainId, int recordId, DnsRecord record, CancellationToken ct = default) =>
        Request("domains", domainId, "dns", recordId).PutJsonAsync(record, cancellationToken: ct);

    /// <summary>Deletes a DNS record.</summary>
    public Task DeleteDnsRecordAsync(int domainId, int recordId, CancellationToken ct = default) =>
        Request("domains", domainId, "dns", recordId).DeleteAsync(cancellationToken: ct);

    // -------------------------------------------------------------------------
    // DDNS  — GET /dyndns/update
    // -------------------------------------------------------------------------

    /// <summary>
    /// Updates the A/AAAA record for <paramref name="hostname"/> via the DDNS endpoint.
    /// When <paramref name="myip"/> is omitted the API uses the caller's public IP address.
    /// </summary>
    public Task UpdateDdnsAsync(string hostname, string? myip = null, CancellationToken ct = default)
    {
        var req = Request("dyndns", "update").SetQueryParam("hostname", hostname);
        if (myip is not null)
            req = req.SetQueryParam("myip", myip);
        return req.GetAsync(cancellationToken: ct);
    }

    // -------------------------------------------------------------------------
    // HTTP Forwards  — /domains/{domainId}/forwards[/{host}]
    // -------------------------------------------------------------------------

    /// <summary>Lists all HTTP forwards for a domain.</summary>
    public Task<List<HttpForward>> GetForwardsAsync(int domainId, CancellationToken ct = default) =>
        Request("domains", domainId, "forwards").GetJsonAsync<List<HttpForward>>(cancellationToken: ct);

    /// <summary>Creates an HTTP forward under a domain.</summary>
    public Task CreateForwardAsync(int domainId, HttpForward forward, CancellationToken ct = default) =>
        Request("domains", domainId, "forwards").PostJsonAsync(forward, cancellationToken: ct);

    /// <summary>Gets a specific HTTP forward by its host.</summary>
    public Task<HttpForward> GetForwardAsync(int domainId, string host, CancellationToken ct = default) =>
        Request("domains", domainId, "forwards", host).GetJsonAsync<HttpForward>(cancellationToken: ct);

    /// <summary>Replaces an HTTP forward.</summary>
    public Task UpdateForwardAsync(int domainId, string host, HttpForward forward, CancellationToken ct = default) =>
        Request("domains", domainId, "forwards", host).PutJsonAsync(forward, cancellationToken: ct);

    /// <summary>Deletes an HTTP forward.</summary>
    public Task DeleteForwardAsync(int domainId, string host, CancellationToken ct = default) =>
        Request("domains", domainId, "forwards", host).DeleteAsync(cancellationToken: ct);

    public void Dispose() => _client.Dispose();
}

