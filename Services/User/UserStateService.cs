using Banko.Client.Models.User;
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

  /// <summary>
  /// Calls the API to update user settings and then refreshes the local user state.
  /// </summary>
  /// <param name="userUpdate">The user update DTO containing the changes.</param>
  /// <returns>True if the update was successful and state was refreshed, false otherwise.</returns>
  public async Task<bool> UpdateProfileSettingsAsync(UserUpdate userUpdate)
  {
    cacheValidator.UpdateCache(false, null);
    try
    {
      var updatedUserFromApi = await userService.UpdateUserProfileAsync(userUpdate);
      if (updatedUserFromApi != null)
      {
        await LoadUserDataAsync();
        NotifyUserStateChanged();
        return true;
      }
      return false;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return false;
    }
  }

  private void NotifyUserStateChanged()
  {
    OnUserStateChanged?.Invoke();
  }
}