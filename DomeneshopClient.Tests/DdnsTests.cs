namespace DomeneshopClient.Tests;

public class DdnsTests : DomeneshopClientTestBase
{
    [Fact]
    public async Task UpdateDdnsAsync_SendsGetWithHostnameParam()
    {
        await Client.UpdateDdnsAsync("home.example.com");

        HttpTest.ShouldHaveCalled($"{BaseUrl}/dyndns/update*")
            .WithVerb(HttpMethod.Get)
            .WithQueryParam("hostname", "home.example.com")
            .Times(1);
    }

    [Fact]
    public async Task UpdateDdnsAsync_WithMyIp_AppendsBothQueryParams()
    {
        await Client.UpdateDdnsAsync("home.example.com", "1.2.3.4");

        HttpTest.ShouldHaveCalled($"{BaseUrl}/dyndns/update*")
            .WithVerb(HttpMethod.Get)
            .WithQueryParam("hostname", "home.example.com")
            .WithQueryParam("myip", "1.2.3.4")
            .Times(1);
    }

    [Fact]
    public async Task UpdateDdnsAsync_WithoutMyIp_OmitsMyIpParam()
    {
        await Client.UpdateDdnsAsync("home.example.com");

        HttpTest.ShouldHaveCalled($"{BaseUrl}/dyndns/update*")
            .WithoutQueryParam("myip")
            .Times(1);
    }
}

