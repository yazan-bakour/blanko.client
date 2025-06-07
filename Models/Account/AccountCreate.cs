using System.ComponentModel.DataAnnotations;

namespace Banko.Client.Models.Account;

public class AccountCreate

{
  public int UserId { get; set; }

  [Range(0, double.MaxValue, ErrorMessage = "Balance must be a positive number")]
  public decimal Balance { get; set; }

  public AccountType Type { get; set; }
  // personal or contact
}