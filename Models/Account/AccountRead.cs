using System.Text.Json.Serialization;

namespace Banko.Client.Models.Account
{
  public class AccountRead
  {
    public int Id { get; set; }
    public int UserId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType Type { get; set; }
    public string AccountNumber { get; set; } = string.Empty;

    // public string Bic { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0.0m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public decimal InterestRate { get; set; } = 0.0m;
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountStatus Status { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Currency Currency { get; set; } = Currency.EUR;
    public DateTime? LastTransactionDate { get; set; }
    public decimal MinimumBalance { get; set; } = 0.0m;
    public decimal OverdraftLimit { get; set; } = 0.0m;
  }
  public enum AccountType
  {
    Checking,
    Savings,
    Investment,
    Business,
    MoneyMarket,
    CertificateOfDeposit,
    RetirementAccount,
    StudentAccount,
    JointAccount,
    Trust
  }
  public enum AccountStatus
  {
    Active,
    Inactive,
    Frozen,
    Closed,
    PendingApproval
  }
  public enum Currency
  {
    USD,
    EUR,
    GBP,
    JPY,
    AUD,
    CAD,
    CHF,
    CNY,
    SEK,
    NZD
  }
}