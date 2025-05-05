using Banko.Client.Models;
using Banko.Client.Helper;

namespace Banko.Client.Services.User;
public class UserStateService(IUserService userService, ICacheValidator<UserRead> cacheValidator)
{
  public event Action? OnUserStateChanged;

  public UserRead? CurrentUser => cacheValidator.Data;

  public async Task LoadUserDataAsync()
  {

    if (cacheValidator.IsInitialized && cacheValidator.Data != null)
    {
      NotifyUserStateChanged();
      return;
    }

    try
    {
      var userData = await userService.GetCurrentUserProfileAsync();
      if (userData != null)
      {
        cacheValidator.UpdateCache(true, userData);
        NotifyUserStateChanged();
      }
      else
      {
        cacheValidator.UpdateCache(true, null);
      }
    }
    catch
    {
      cacheValidator.UpdateCache(false, null);
      throw;
    }
  }

  private void NotifyUserStateChanged()
  {
    OnUserStateChanged?.Invoke();
  }
}