using Banko.Client.Models.User;
using Banko.Client.Helper;
using Banko.Client.Extensions;
using Banko.Client.Services.Auth;

namespace Banko.Client.Services.User;

public class UserStateService(IUserService userService, ICacheValidator<UserRead> cacheValidator, AuthStateProvider authState)
{
  public event Action? OnUserStateChanged;
  public UserRead? CurrentUser => cacheValidator.Data;
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
    // var user = await userService.GetCurrentUserProfileAsync();
    // cacheValidator.UpdateCache(true, user);
    NotifyUserStateChanged();
    return true;
  }

  public async Task<bool> UpdatePreference(Preferences newPreferences)
  {
    var mapping = MapToUserUpdate(update =>
    {
      update.Preferences = newPreferences;
    });
    authState.UserPreferences = newPreferences;

    await userService.UpdateUserProfileAsync(mapping);
    await authState.GetUserInfo(true);
    return true;
  }

  public void NotifyUserStateChanged()
  {
    OnUserStateChanged?.Invoke();
  }
}