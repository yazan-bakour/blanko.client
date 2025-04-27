namespace Banko.Client.Models
{
  public enum UserRole
  {
    Admin,
    Customer,
    Support
  }
  public class UserRead
  {
    public string Token { get; set; } = string.Empty;
    public UserData User { get; set; } = new UserData();
  }
  public class UserData
  {
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public UserRole? Role { get; set; }
  }
}
