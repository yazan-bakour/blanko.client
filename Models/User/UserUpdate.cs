using System.Text.Json.Serialization;

namespace Banko.Client.Models.User;

public class UserUpdate
{
  public UserUpdate() { }

  public UserUpdate(UserUpdate other)
  {
    if (other == null) return;
    Id = other.Id;
    NewPassword = other.NewPassword;
    FullName = other.FullName;
    Email = other.Email;
    CreatedAt = other.CreatedAt;
    Role = other.Role;
    FirstName = other.FirstName;
    LastName = other.LastName;
    UpdatedAt = other.UpdatedAt;
    PhoneNumber = other.PhoneNumber;
    Address = other.Address;
    City = other.City;
    State = other.State;
    ZipCode = other.ZipCode;
    Country = other.Country;
    DateOfBirth = other.DateOfBirth;
    LastLogin = other.LastLogin;
    Nationality = other.Nationality;
    Gender = other.Gender;
    IsVerified = other.IsVerified;
    UniqueId = other.UniqueId;
    ProfilePictureDisplay = other.ProfilePictureDisplay;
    ProfilePictureFile = other.ProfilePictureFile;
    Preferences = other.Preferences;
  }

  public int Id { get; set; }
  public string? NewPassword { get; set; } = null;
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
  public string? ProfilePictureUrl { get; set; }
  public string? ProfilePictureFile { get; set; }
  public string? ProfilePictureDisplay { get; set; }
  public Preferences Preferences { get; set; }
}
