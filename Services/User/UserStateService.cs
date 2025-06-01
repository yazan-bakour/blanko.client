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
      update.Preferences ??= new Dictionary<string, string>();

      update.Preferences["DarkMode"] = preference.Theme.ToString();
      //enhance this
    });
    Preference.Theme = preference.Theme;
    await userService.UpdateUserProfileAsync(mapping);
    var user = await userService.GetCurrentUserProfileAsync();
    cacheValidator.UpdateCache(true, user);
    NotifyUserStateChanged();
    return true;
  }

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