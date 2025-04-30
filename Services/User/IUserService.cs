using Banko.Client.Models;

namespace Banko.Client.Services.User
{
  public interface IUserService
  {
    Task<UserRead> GetCurrentUserProfileAsync();
  }
}