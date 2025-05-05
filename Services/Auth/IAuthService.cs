using Banko.Client.Models.User;

namespace Banko.Client.Services.Auth;
public interface IAuthService
{
  Task LoginAsync(UserLogin loginModel);
  Task<UserRead> UserInfoAsync();
  Task RegisterAsync(UserRegister registerModel);
  Task LogoutAsync();
}