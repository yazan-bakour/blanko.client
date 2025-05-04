using Banko.Client.Models;
using Banko.Client.Helper;
using Microsoft.AspNetCore.Components.Authorization;

namespace Banko.Client.Services.User;
public class UserStateService
{
  private readonly IUserService _userService;
  private readonly ICacheValidator<UserRead> _cacheValidator;
  private readonly AuthenticationStateProvider _authStateProvider;

  public UserStateService(
    IUserService userService,
    ICacheValidator<UserRead> cacheValidator,
    AuthenticationStateProvider authStateProvider)
  {
    _userService = userService;
    _cacheValidator = cacheValidator;
    _authStateProvider = authStateProvider;

    // Listen for auth state changes
    if (authStateProvider is Banko.Client.Services.Auth.AuthStateProvider authProvider)
    {
      // This will hook into auth state changes
      _cacheValidator.OnStateChanged += () => NotifyUserStateChanged();
    }
  }

  public event Action? OnUserStateChanged;

  public UserRead? CurrentUser => _cacheValidator.Data;

  public async Task LoadUserDataAsync()
  {
    // Don't reload data if we already have it
    if (_cacheValidator.IsInitialized && _cacheValidator.Data != null)
    {
      NotifyUserStateChanged();
      return;
    }

    try
    {
      var userData = await _userService.GetCurrentUserProfileAsync();
      if (userData != null)
      {
        _cacheValidator.UpdateCache(true, userData);
        NotifyUserStateChanged();
      }
      else
      {
        _cacheValidator.UpdateCache(true, null);
      }
    }
    catch
    {
      _cacheValidator.UpdateCache(false, null);
      throw;
    }
  }

  private void NotifyUserStateChanged()
  {
    OnUserStateChanged?.Invoke();
  }
}