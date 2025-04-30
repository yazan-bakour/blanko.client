using Banko.Client.Models;

namespace Banko.Client.Services.Auth;
public interface IAuthService
{
  Task<bool> LoginAsync(UserLogin loginModel);
  Task<bool> RegisterAsync(UserRegister registerModel);
  Task LogoutAsync();
  Task<bool> IsAuthenticatedAsync();
  Task<string> GetTokenAsync();
  Task<UserRead?> GetCurrentUserAsync();
  event Action OnAuthStateChanged;
}