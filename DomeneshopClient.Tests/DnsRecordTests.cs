using DomeneshopClient.Models;

namespace DomeneshopClient.Tests;

public class DnsRecordTests : DomeneshopClientTestBase
{
    private const int DomainId = 1;
    private const int RecordId = 5;

    // Minimal JSON the fake API sends back; the 'type' field is required by DnsRecordConverter.
    private static readonly object FakeARecord = new { id = RecordId, host = "www", ttl = 3600, type = "A", data = "1.2.3.4" };
    private static readonly object FakeMxRecord = new { id = RecordId, host = "@", ttl = 3600, type = "MX", data = "mail.example.com", priority = 10 };

    // STJ serialises base-class properties first (DnsRecord → DataDnsRecord → concrete type),
    // so property order in these strings must match that declaration order.
    private const string ARecordJson  = """{"type":"A","data":"1.2.3.4","host":"www","ttl":3600}""";
    private const string MxRecordJson = """{"type":"MX","priority":10,"data":"mail.example.com","host":"@","ttl":3600}""";

    [Fact]
    public async Task GetDnsRecordsAsync_SendsGetToDnsEndpoint()
    {
        HttpTest.RespondWithJson(new[] { FakeARecord });

        await Client.GetDnsRecordsAsync(DomainId);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/dns")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetDnsRecordAsync_SendsGetToDnsRecordByIdEndpoint()
    {
        HttpTest.RespondWithJson(FakeARecord);

        await Client.GetDnsRecordAsync(DomainId, RecordId);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/dns/{RecordId}")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task CreateDnsRecordAsync_SendsPostWithARecordPayload()
    {
        HttpTest.RespondWithJson(FakeARecord, 201);

        var record = new ADnsRecord { Host = "www", Data = "1.2.3.4" };
        await Client.CreateDnsRecordAsync(DomainId, record);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/dns")
            .WithVerb(HttpMethod.Post)
            .WithRequestBody(ARecordJson)
            .Times(1);
    }

    [Fact]
    public async Task CreateDnsRecordAsync_SendsPostWithMxRecordPayload()
    {
        HttpTest.RespondWithJson(FakeMxRecord, 201);

        var record = new MxDnsRecord { Host = "@", Data = "mail.example.com", Priority = 10 };
        await Client.CreateDnsRecordAsync(DomainId, record);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/dns")
            .WithVerb(HttpMethod.Post)
            .WithRequestBody(MxRecordJson)
            .Times(1);
    }

    [Fact]
    public async Task UpdateDnsRecordAsync_SendsPutWithPayloadToRecordEndpoint()
    {
        var record = new ADnsRecord { Host = "www", Data = "1.2.3.4" };
        await Client.UpdateDnsRecordAsync(DomainId, RecordId, record);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/dns/{RecordId}")
            .WithVerb(HttpMethod.Put)
            .WithRequestBody(ARecordJson)
            .Times(1);
    }

    [Fact]
    public async Task DeleteDnsRecordAsync_SendsDeleteToRecordEndpoint()
    {
        await Client.DeleteDnsRecordAsync(DomainId, RecordId);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/dns/{RecordId}")
            .WithVerb(HttpMethod.Delete)
            .Times(1);
    }
}

