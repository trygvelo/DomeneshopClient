using System.Net;
using System.Text;
using DomeneshopClient.Models;

namespace DomeneshopClient.Tests;

public class DomainTests : DomeneshopClientTestBase
{
    private static readonly object FakeDomain = new
    {
        id = 1,
        domain = "example.com",
        renew = false,
        registrant = "Test User",
        status = "active",
        nameservers = new[] { "ns1.domeneshop.no", "ns2.domeneshop.no" },
        services = new { registrar = true, dns = true, email = false }
    };

    [Fact]
    public async Task GetDomainsAsync_SendsGetToDomainsEndpoint()
    {
        HttpTest.RespondWithJson(new[] { FakeDomain });

        await Client.GetDomainsAsync();

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetDomainAsync_SendsGetToDomainByIdEndpoint()
    {
        HttpTest.RespondWithJson(FakeDomain);

        await Client.GetDomainAsync(42);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/domains/42")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task AllRequests_SendBasicAuthHeader()
    {
        HttpTest.RespondWithJson(new[] { FakeDomain });

        await Client.GetDomainsAsync();

        var expected = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Token}:{Secret}"));
        HttpTest.ShouldHaveCalled("*")
            .WithHeader("Authorization", expected)
            .Times(1);
    }
}

