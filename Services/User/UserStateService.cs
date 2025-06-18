using Banko.Client.Models.User;
using Banko.Client.Helper;
using Banko.Client.Extensions;

namespace Banko.Client.Services.User;

public class UserStateService(IUserService userService, ICacheValidator<UserRead> cacheValidator)
{
  public event Action? OnUserStateChanged;
  public UserRead? CurrentUser => cacheValidator.Data;
  public Preference Preference { get; } = new Preference();

  private UserUpdate MapToUserUpdate(Action<UserUpdate> updateAction)
  {
    if (CurrentUser?.User == null) throw new InvalidOperationException("No current user available to update.");

    var toUpdate = CurrentUser.ToUserUpdate();

    updateAction(toUpdate);

    return toUpdate;
  }

  public async Task<bool> UpdateProfileSettingsAsync(UserUpdate partialUpdate)
  {
    var mapping = MapToUserUpdate(update =>
    {
      update.FirstName = partialUpdate.FirstName;
      update.LastName = partialUpdate.LastName;
      update.PhoneNumber = partialUpdate.PhoneNumber;
      update.UpdatedAt = DateTime.UtcNow;
      update.Address = partialUpdate.Address;
      update.City = partialUpdate.City;
      update.State = partialUpdate.State;
      update.ZipCode = partialUpdate.ZipCode;
      update.Country = partialUpdate.Country;
      update.DateOfBirth = partialUpdate.DateOfBirth;
      update.LastLogin = partialUpdate.LastLogin;
      update.Nationality = partialUpdate.Nationality;
      update.Gender = partialUpdate.Gender;
      update.ProfilePictureDisplay = partialUpdate.ProfilePictureDisplay;
      update.ProfilePictureUrl = partialUpdate.ProfilePictureUrl;
    });
    await userService.UpdateUserProfileAsync(mapping);
    var user = await userService.GetCurrentUserProfileAsync();
    cacheValidator.UpdateCache(true, user);
    NotifyUserStateChanged();
    return true;
  }

  public async Task<bool> UpdatePreference(Preference preference)
  {
    var mapping = MapToUserUpdate(update =>
    {
      update.Preferences = CurrentUser?.User?.Preferences ?? new Dictionary<string, string>();
      update.Preferences["DarkMode"] = preference.Theme.ToString();
      update.Preferences["HideEmail"] = preference.Privacy.ToString();
    });
    Preference.Theme = preference.Theme;
    Preference.Privacy = preference.Privacy;

    await userService.UpdateUserProfileAsync(mapping);
    var user = await userService.GetCurrentUserProfileAsync();
    cacheValidator.UpdateCache(true, user);
    NotifyUserStateChanged();
    return true;
  }

  public async Task LoadUserDataAsync()
  {
    if (CurrentUser != null) return;
    try
    {
      var userData = await userService.GetCurrentUserProfileAsync();
      if (userData != null)
      {
        cacheValidator.UpdateCache(true, userData);

        if (userData.User?.Preferences != null)
        {
          if (userData.User.Preferences.TryGetValue("DarkMode", out var darkModeValue) &&
              bool.TryParse(darkModeValue, out var isDark))
          {
            Preference.Theme = isDark;
          }

          if (userData.User.Preferences.TryGetValue("HideEmail", out var hideEmailValue) &&
              bool.TryParse(hideEmailValue, out var shouldHide))
          {
            Preference.Privacy = shouldHide;
          }
        }
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
    finally
    {
      NotifyUserStateChanged();
    }
  }

  public void NotifyUserStateChanged()
  {
    OnUserStateChanged?.Invoke();
  }
}