namespace DomeneshopClient.Tests;

public class InvoiceTests : DomeneshopClientTestBase
{
    private static readonly object FakeInvoice = new
    {
        id = 1,
        type = "invoice",
        amount = 299,
        currency = "NOK",
        issued_date = "2024-01-15",
        status = "paid",
        url = "https://domeneshop.no/invoice/1"
    };

    [Fact]
    public async Task GetInvoicesAsync_SendsGetToInvoicesEndpoint()
    {
        HttpTest.RespondWithJson(new[] { FakeInvoice });

        await Client.GetInvoicesAsync();

        HttpTest.ShouldHaveCalled($"{BaseUrl}/invoices")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetInvoicesAsync_WithStatus_AppendsStatusQueryParam()
    {
        HttpTest.RespondWithJson(new[] { FakeInvoice });

        await Client.GetInvoicesAsync("unpaid");

        HttpTest.ShouldHaveCalled($"{BaseUrl}/invoices*")
            .WithVerb(HttpMethod.Get)
            .WithQueryParam("status", "unpaid")
            .Times(1);
    }

    [Fact]
    public async Task GetInvoicesAsync_WithoutStatus_OmitsStatusQueryParam()
    {
        HttpTest.RespondWithJson(new[] { FakeInvoice });

        await Client.GetInvoicesAsync();

        HttpTest.ShouldHaveCalled($"{BaseUrl}/invoices")
            .WithVerb(HttpMethod.Get)
            .WithoutQueryParam("status")
            .Times(1);
    }

    [Fact]
    public async Task GetInvoiceAsync_SendsGetToInvoiceByNumberEndpoint()
    {
        HttpTest.RespondWithJson(FakeInvoice);

        await Client.GetInvoiceAsync(1001);

        HttpTest.ShouldHaveCalled($"{BaseUrl}/invoices/1001")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }
}

