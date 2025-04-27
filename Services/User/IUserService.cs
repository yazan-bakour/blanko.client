using Banko.Client.Models;

namespace Banko.Client.Services.User
{
  public interface IUserService
  {
    Task<UserRead> LoginAsync(UserLogin loginModel);
    Task<UserRead> GetCurrentUserProfileAsync(string token);
    Task<UserRead> RegisterAsync(UserRegister registerModel);
  }
}