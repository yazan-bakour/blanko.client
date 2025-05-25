using Banko.Client.Models.User;
using MudBlazor;

namespace Banko.Client.Services.User;

public class UserDataLoaderService
{
  private readonly UserStateService _userState;
  private readonly ISnackbar _snackbar;
  private readonly LoadingService _loadingService;
  private readonly ErrorService _errorService;

  public UserDataLoaderService(
      UserStateService userState,
      ISnackbar snackbar,
      LoadingService loadingService,
      ErrorService errorService)
  {
    _userState = userState;
    _snackbar = snackbar;
    _loadingService = loadingService;
    _errorService = errorService;
  }

  /// <summary>
  /// Loads or refreshes user data from the API
  /// </summary>
  /// <param name="forceRefresh">Whether to force a refresh from the API even if data is already cached</param>
  /// <returns>True if the data was loaded successfully, false otherwise</returns>
  public async Task<bool> RefreshUserDataAsync(bool forceRefresh = true)
  {
    _loadingService.IsLoading = true;

    try
    {
      await _userState.LoadUserDataAsync();

      if (_userState.CurrentUser == null)
      {
        _snackbar.Add("Could not load user profile data.", Severity.Error);
        return false;
      }

      return true;
    }
    catch (HttpRequestException ex)
    {
      _snackbar.Add($"Error loading profile: {ex.Message}", Severity.Error);
      return false;
    }
    catch (Exception ex)
    {
      _snackbar.Add($"Unexpected error: {ex.Message}", Severity.Error);
      return false;
    }
    finally
    {
      _loadingService.IsLoading = false;
    }
  }

  /// <summary>
  /// Updates the user profile settings
  /// </summary>
  /// <param name="userUpdate">The user update data</param>
  /// <returns>True if update was successful, false otherwise</returns>
  public async Task<bool> UpdateUserProfileAsync(UserUpdate userUpdate)
  {
    _loadingService.IsLoading = true;

    try
    {
      bool success = await _userState.UpdateProfileSettingsAsync(userUpdate);

      if (success)
      {
        _snackbar.Add("Profile updated successfully!", Severity.Success);
        return true;
      }
      else
      {
        _snackbar.Add("Failed to update profile.", Severity.Error);
        return false;
      }
    }
    catch (HttpRequestException ex)
    {
      _snackbar.Add($"Error updating profile: {ex.Message}", Severity.Error);
      return false;
    }
    catch (Exception ex)
    {
      _snackbar.Add($"Unexpected error: {ex.Message}", Severity.Error);
      return false;
    }
    finally
    {
      _loadingService.IsLoading = false;
    }
  }

  /// <summary>
  /// Gets the current user data from the state service
  /// </summary>
  public UserRead? GetCurrentUser() => _userState.CurrentUser;

  /// <summary>
  /// Subscribes to user state changes
  /// </summary>
  public void SubscribeToUserStateChanges(Action handler) =>
      _userState.OnUserStateChanged += handler;

  /// <summary>
  /// Unsubscribes from user state changes
  /// </summary>
  public void UnsubscribeFromUserStateChanges(Action handler) =>
      _userState.OnUserStateChanged -= handler;
}