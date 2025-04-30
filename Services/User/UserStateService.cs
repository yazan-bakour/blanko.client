using Banko.Client.Models;
using Banko.Client.Helper;

namespace Banko.Client.Services.User;
public class UserStateService(IUserService userService, ICacheValidator<UserRead> cacheValidator)
{
  private readonly IUserService _userService = userService;

  public event Action? OnUserStateChanged;

  public UserRead? CurrentUser => cacheValidator.Data;

  public async Task LoadUserDataAsync()
  {
    if (cacheValidator.IsInitialized) return;
    try
    {
      var userData = await _userService.GetCurrentUserProfileAsync();
      cacheValidator.UpdateCache(true, userData);
      NotifyUserStateChanged();
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