using Banko.Client.Models.User;

namespace Banko.Client.Services.User
{
  public interface IUserService
  {
    Task<UserRead> GetCurrentUserProfileAsync();
    Task<UserRead?> UpdateUserProfileAsync(UserUpdate userUpdate);
  }
}