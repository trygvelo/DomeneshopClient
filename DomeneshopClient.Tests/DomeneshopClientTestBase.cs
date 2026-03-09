using DomeneshopClient;
using Flurl.Http.Testing;

namespace DomeneshopClient.Tests;

/// <summary>
/// Spins up a <see cref="HttpTest"/> before each test so all Flurl calls are intercepted,
/// then tears it down (and disposes the client) after the test completes.
/// </summary>
public abstract class DomeneshopClientTestBase : IDisposable
{
    protected const string Token = "test-token";
    protected const string Secret = "test-secret";
    protected const string BaseUrl = "https://api.domeneshop.no/v0";

    protected readonly HttpTest HttpTest;
    protected readonly DomeneshopClient Client;

    protected DomeneshopClientTestBase()
    {
        HttpTest = new HttpTest();
        Client = new DomeneshopClient(Token, Secret);
    }

    public void Dispose()
    {
        Client.Dispose();
        HttpTest.Dispose();
    }
}

