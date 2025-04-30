namespace Banko.Client.Models.Account
{
  public class AccountRead
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Type { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0.0m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}