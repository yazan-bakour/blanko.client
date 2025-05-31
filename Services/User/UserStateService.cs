using Banko.Client.Models.User;
using Banko.Client.Helper;

namespace Banko.Client.Services.User;

public class UserStateService(IUserService userService, ICacheValidator<UserRead> cacheValidator)
{
  public event Action? OnUserStateChanged;
  public event Action<bool>? OnDarkModeChanged;
  public UserRead? CurrentUser => cacheValidator.Data;
  private bool _isDarkMode;
  public bool IsDarkMode
  {
    get => _isDarkMode;
    private set
    {
      if (_isDarkMode != value)
      {
        _isDarkMode = value;
        OnDarkModeChanged?.Invoke(_isDarkMode);
      }
    }
  }

  public async Task LoadUserDataAsync()
  {

    if (cacheValidator.IsInitialized && cacheValidator.Data != null)
    {
      if (CurrentUser?.User.Preferences?.ContainsKey("DarkMode") == true)
      {
        IsDarkMode = Convert.ToBoolean(CurrentUser.User.Preferences["DarkMode"]);
      }
      NotifyUserStateChanged();
      return;
    }

    try
    {
      var userData = await userService.GetCurrentUserProfileAsync();
      if (userData != null)
      {
        cacheValidator.UpdateCache(true, userData);
        if (userData.User.Preferences?.ContainsKey("DarkMode") == true)
        {
          IsDarkMode = Convert.ToBoolean(userData.User.Preferences["DarkMode"]);
        }
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
    catch
    {
      return false;
    }
  }
  /// <summary>
  /// Updates the dark mode preference and persists it to the user profile
  /// </summary>
  public async Task<bool> UpdateDarkModePreferenceAsync(bool isDarkMode)
  {
    IsDarkMode = isDarkMode;

    // If user is logged in, save preference to their profile
    if (CurrentUser != null)
    {
      var userUpdate = new UserUpdate
      {
        // Include other necessary fields from CurrentUser
        Preferences = CurrentUser.User.Preferences ?? new Dictionary<string, string>()
      };

      userUpdate.Preferences["DarkMode"] = isDarkMode.ToString();
      return await UpdateProfileSettingsAsync(userUpdate);
    }

    return true;
  }

  private void NotifyUserStateChanged()
  {
    OnUserStateChanged?.Invoke();
  }
}