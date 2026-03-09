using System.Text.Json.Serialization;

namespace DomeneshopClient.Models;

public sealed class Invoice
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>"invoice" or "credit_note"</summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;

    [JsonPropertyName("due_date")]
    public string? DueDate { get; init; }

    [JsonPropertyName("issued_date")]
    public string IssuedDate { get; init; } = string.Empty;

    [JsonPropertyName("paid_date")]
    public string? PaidDate { get; init; }

    /// <summary>"unpaid", "paid", or "settled"</summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}

