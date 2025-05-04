using Banko.Client.Models;

namespace Banko.Client.Services.Auth;
public interface IAuthService
{
  Task LoginAsync(UserLogin loginModel);
  Task<UserRead> UserInfoAsync();
  Task RegisterAsync(UserRegister registerModel);
  Task LogoutAsync();
}