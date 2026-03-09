# DomeneshopClient

A .NET 8 client library for the [Domeneshop REST API v0](https://api.domeneshop.no/docs/), built with [Flurl.Http](https://flurl.dev/).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A Domeneshop API token and secret — generate them at  
  **My account → API credentials** at https://www.domeneshop.no/admin?view=api

## Getting started

### 1. Add the project reference

```xml
<ProjectReference Include="..\DomeneshopClient\DomeneshopClient.csproj" />
```

### 2. Create a client

```csharp
using DomeneshopClient;

using var client = new DomeneshopClient("your-token", "your-secret");
```

The client is `IDisposable`. Wrap it in a `using` statement or register it as a singleton in your DI container and dispose it on shutdown.

---

## API reference

Every method accepts an optional `CancellationToken` as the last parameter.

### Domains

```csharp
// List all domains on the account
List<Domain> domains = await client.GetDomainsAsync();

// Get a single domain by its numeric ID
Domain domain = await client.GetDomainAsync(domainId: 12345);
```

### Invoices

```csharp
// List all invoices
List<Invoice> all = await client.GetInvoicesAsync();

// Filter by status: "unpaid", "paid", or "settled"
List<Invoice> unpaid = await client.GetInvoicesAsync(status: "unpaid");

// Get a specific invoice by its number
Invoice invoice = await client.GetInvoiceAsync(invoiceNumber: 1001);
```

### DNS records

All DNS methods target a specific domain identified by its numeric `domainId`.

```csharp
// List all DNS records for a domain
List<DnsRecord> records = await client.GetDnsRecordsAsync(domainId);

// Get a specific record by its ID
DnsRecord record = await client.GetDnsRecordAsync(domainId, recordId);

// Create a new record — pass a concrete subtype (see DNS record types below)
DnsRecord created = await client.CreateDnsRecordAsync(domainId, new ADnsRecord
{
    Host = "www",
    Data = "203.0.113.10",
    Ttl  = 3600
});

// Replace an existing record (type may not change)
await client.UpdateDnsRecordAsync(domainId, created.Id!.Value, new ADnsRecord
{
    Host = "www",
    Data = "203.0.113.99",
    Ttl  = 3600
});

// Delete a record
await client.DeleteDnsRecordAsync(domainId, created.Id!.Value);
```

#### DNS record types

| Class | `type` | Extra properties |
|---|---|---|
| `ADnsRecord` | `A` | `Data` (IPv4) |
| `AAAADnsRecord` | `AAAA` | `Data` (IPv6) |
| `CnameDnsRecord` | `CNAME` | `Data` (hostname) |
| `AnameDnsRecord` | `ANAME` | `Data` (hostname) |
| `NsDnsRecord` | `NS` | `Data` (hostname) |
| `TxtDnsRecord` | `TXT` | `Data` (text) |
| `MxDnsRecord` | `MX` | `Data` (hostname), `Priority` |
| `SrvDnsRecord` | `SRV` | `Data` (hostname), `Priority`, `Weight`, `Port` |
| `CaaDnsRecord` | `CAA` | `Flags`, `Tag`, `Data` |
| `TlsaDnsRecord` | `TLSA` | `Usage`, `Selector`, `Dtype`, `Data` |
| `DsDnsRecord` | `DS` | `KeyTag`, `Alg`, `DigestType`, `Digest` |
| `SshfpDnsRecord` | `SSHFP` | `Algorithm`, `HashType`, `Data` |

```csharp
// MX record
await client.CreateDnsRecordAsync(domainId, new MxDnsRecord
{
    Host     = "@",
    Data     = "mail.example.com.",
    Priority = 10
});

// SRV record
await client.CreateDnsRecordAsync(domainId, new SrvDnsRecord
{
    Host     = "_sip._tcp",
    Data     = "sip.example.com.",
    Priority = 10,
    Weight   = 20,
    Port     = 5060
});
```

### Dynamic DNS (DDNS)

```csharp
// Update the A/AAAA record for a hostname.
// When myip is omitted, the API uses the caller's public IP.
await client.UpdateDdnsAsync("home.example.com");
await client.UpdateDdnsAsync("home.example.com", myip: "203.0.113.10");
```

### HTTP forwards

Forwards are identified by their `Host` subdomain, not by a numeric ID.

```csharp
// List all HTTP forwards for a domain
List<HttpForward> forwards = await client.GetForwardsAsync(domainId);

// Get a specific forward
HttpForward forward = await client.GetForwardAsync(domainId, host: "www");

// Create a forward
await client.CreateForwardAsync(domainId, new HttpForward
{
    Host  = "www",
    Url   = "https://my-site.example.com",
    Frame = false   // true = iframe, false = redirect
});

// Replace a forward
await client.UpdateForwardAsync(domainId, host: "www", new HttpForward
{
    Host  = "www",
    Url   = "https://new-site.example.com",
    Frame = false
});

// Delete a forward
await client.DeleteForwardAsync(domainId, host: "www");
```

---

## Running the tests

```bash
dotnet test
```

The test suite uses [Flurl's built-in `HttpTest`](https://flurl.dev/docs/testable-http/) to intercept all HTTP calls in-process — no network access or test server is required.

```
Total tests: 21
     Passed: 21
```

### Test project structure

| File | What it covers |
|---|---|
| `DomainTests.cs` | `GET /domains`, `GET /domains/{id}`, Basic Auth header |
| `InvoiceTests.cs` | Invoice listing with and without status filter, single invoice |
| `DnsRecordTests.cs` | All DNS CRUD operations, A and MX payload assertions |
| `DdnsTests.cs` | Required `hostname` param, optional `myip` param |
| `ForwardTests.cs` | All HTTP forward CRUD operations and payloads |

---

## Project structure

```
DomeneshopClient/
├── Models/
│   ├── Domain.cs          # Domain, DomainServices
│   ├── Invoice.cs         # Invoice
│   ├── DnsRecord.cs       # Abstract DnsRecord hierarchy + DnsRecordConverter
│   └── HttpForward.cs     # HttpForward
└── DomeneshopClient.cs    # Main client

DomeneshopClient.Tests/
├── DomeneshopClientTestBase.cs   # Shared HttpTest + client setup
├── DomainTests.cs
├── InvoiceTests.cs
├── DnsRecordTests.cs
├── DdnsTests.cs
└── ForwardTests.cs
```

## Dependencies

| Package | Purpose |
|---|---|
| [Flurl.Http](https://www.nuget.org/packages/Flurl.Http) 4.0.2 | HTTP client and URL building |
| `System.Text.Json` | JSON serialisation (included in .NET 8) |

