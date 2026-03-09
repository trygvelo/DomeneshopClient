using DomeneshopClient.Models;

namespace DomeneshopClient.Tests;

public class ForwardTests : DomeneshopClientTestBase
{
    private const int DomainId = 1;
    private const string Host = "www";

    private static readonly object FakeForward = new { host = Host, frame = false, url = "https://example.com" };

    // STJ serialises HttpForward properties in declaration order: host, frame, url.
    private const string ForwardJson = """{"host":"www","frame":false,"url":"https://example.com"}""";

    [Fact]
    public async Task GetForwardsAsync_SendsGetToForwardsEndpoint()
    {
        HttpTest.RespondWithJson(new[] { FakeForward });

        await Client.GetForwardsAsync(DomainId);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/forwards")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task CreateForwardAsync_SendsPostWithPayloadToForwardsEndpoint()
    {
        var forward = new HttpForward { Host = Host, Frame = false, Url = "https://example.com" };
        await Client.CreateForwardAsync(DomainId, forward);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/forwards")
            .WithVerb(HttpMethod.Post)
            .WithRequestBody(ForwardJson)
            .Times(1);
    }

    [Fact]
    public async Task GetForwardAsync_SendsGetToForwardByHostEndpoint()
    {
        HttpTest.RespondWithJson(FakeForward);

        await Client.GetForwardAsync(DomainId, Host);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/forwards/{Host}")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task UpdateForwardAsync_SendsPutWithPayloadToForwardByHostEndpoint()
    {
        var forward = new HttpForward { Host = Host, Frame = false, Url = "https://example.com" };
        await Client.UpdateForwardAsync(DomainId, Host, forward);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/forwards/{Host}")
            .WithVerb(HttpMethod.Put)
            .WithRequestBody(ForwardJson)
            .Times(1);
    }

    [Fact]
    public async Task DeleteForwardAsync_SendsDeleteToForwardByHostEndpoint()
    {
        await Client.DeleteForwardAsync(DomainId, Host);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/{DomainId}/forwards/{Host}")
            .WithVerb(HttpMethod.Delete)
            .Times(1);
    }
}

