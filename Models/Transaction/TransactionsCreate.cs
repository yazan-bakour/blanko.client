using System.ComponentModel.DataAnnotations;

namespace Banko.Client.Models.Transaction
{
  public class TransactionCreate
  {
    public string? SourceAccountNumber { get; set; }
    public string? DestinationAccountNumber { get; set; }
    public TransactionType Type { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be at least $0.01.")]
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Description { get; set; }
  }
}