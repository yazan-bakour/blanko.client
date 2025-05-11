using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Banko.Client.Models.Transaction
{
  public class Transactions
  {
    public Guid Id { get; set; }
    public required string TransactionNumber { get; set; }
    public required string ReferenceNumber { get; set; }

    [StringLength(20)]
    public required string SourceAccountNumber { get; set; }

    [StringLength(10)]
    public required string SourceAccountId { get; set; }

    [StringLength(10)]
    public required string DestinationAccountId { get; set; }

    [StringLength(20)]
    public required string SourceName { get; set; }

    [StringLength(20)]
    public required string RecipientName { get; set; }

    [StringLength(20)]
    public required string DestinationAccountNumber { get; set; }

    [Range(0.01, double.MaxValue)]
    [DefaultValue(0.00)]
    public decimal Amount { get; set; }

    [Column(TypeName = "varchar(3)")]
    [DefaultValue(Currency.EUR)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Currency Currency { get; set; } = Currency.EUR;

    [DefaultValue(PaymentMethod.CreditCard)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Column(TypeName = "varchar(20)")]
    public PaymentMethod PaymentMethod { get; set; }

    [DefaultValue(TransactionType.Deposit)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Column(TypeName = "varchar(10)")]
    public TransactionType Type { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Column(TypeName = "varchar(10)")]
    public TransactionStatus Status { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public string? Description { get; set; }
    public ICollection<Metadata> Metadata { get; set; } = [];
  }

  public class Metadata
  {
    [Key]
    public int Id { get; set; }

    public string IpAddress { get; set; } = "Unknown";
    public string Device { get; set; } = "Unknown";
    public string Location { get; set; } = "Unknown";

    public Guid TransactionId { get; set; }

    [ForeignKey("TransactionId")]
    public Transactions? Transaction { get; set; }
  }

  public enum TransactionType
  {
    [Description("Deposit")]
    Deposit,

    [Description("Withdrawal")]
    Withdrawal,

    [Description("Transfer")]
    Transfer,

    [Description("Payment")]
    Payment,

    [Description("Refund")]
    Refund,

    [Description("Fee")]
    Fee,

    [Description("Interest")]
    Interest
  }

  public enum PaymentMethod
  {
    [Description("Credit Card Payment")]
    CreditCard,
    [Description("Debit Card Payment")]
    DebitCard,
    [Description("Bank Transfer")]
    BankTransfer,
    [Description("SEBA Transfer")]
    SEBA,
    [Description("Direct Debit")]
    DirectDebit,
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

  public enum TransactionStatus
  {
    Created,
    Pending,
    Completed,
    Failed,
    Cancelled,
    Refunded,
    Disputed
  }
}