using System.Text.Json.Serialization;
using Banko.Client.Models.Account;

namespace Banko.Client.Models.User
{
  public class Privacy
  {
    public bool HideEmail { get; set; } = false;
    public bool HideBalance { get; set; } = false;
    public bool EnableTwoFactorAuth { get; set; } = false;
    public bool ReceiveMarketingEmails { get; set; } = true;
  }

  public class Preferences
  {
    public bool DarkMode { get; set; } = false;
    public string Language { get; set; } = "en-US";
    public string TimeZone { get; set; } = "UTC";
    public string DateFormat { get; set; } = "MM/dd/yyyy";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Currency CurrencyDisplay { get; set; } = Currency.EUR;
    public bool PushNotifications { get; set; } = true;
    public bool TransactionAlerts { get; set; } = true;
    public bool LowBalanceAlerts { get; set; } = true;
    public decimal LowBalanceThreshold { get; set; } = 100.00m;
    public Privacy Privacy { get; set; } = new Privacy();
  }
  public enum UserRole
  {
    Admin,
    Customer,
    Support
  }
  public enum UserGender
  {
    Male,
    Female
  }

  public record UserRead
  {
    public string Token { get; set; } = string.Empty;
    public UserData User { get; set; } = new UserData();
  }
  public record UserData
  {
    public Preferences? Preferences { get; set; }
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole? Role { get; set; }
    public string? FirstName { get; set; } = null;
    public string? LastName { get; set; } = null;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? Nationality { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserGender? Gender { get; set; }
    public bool IsVerified { get; set; }
    public string? UniqueId { get; set; }
    public string? ProfilePictureDisplay { get; set; }
    public string? ProfilePictureUrl { get; set; }
  }
}
