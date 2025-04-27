using Banko.Client.Models;
using Blazored.LocalStorage;

namespace Banko.Client.Services.User
{
  public class UserStateService(IUserService userService, ILocalStorageService localStorage)
  {
    private readonly IUserService _userService = userService;
    private UserRead? _currentUser;
    private bool _isAuthenticated;
    private bool _isInitialized;
    private readonly ILocalStorageService _localStorage = localStorage;
    public event Action? OnAuthStateChanged;

    public UserRead? CurrentUser => _currentUser;
    public bool IsAuthenticated => _isAuthenticated;

    public async Task LoginAsync(UserLogin loginModel)
    {
      try
      {
        var user = await _userService.LoginAsync(loginModel);

        _currentUser = user;
        _isAuthenticated = true;

        await StoreTokenAsync(user.Token);

        NotifyAuthStateChanged();
      }
      catch (Exception ex)
      {
        _currentUser = null;
        _isAuthenticated = false;
        throw new HttpRequestException($"Login request failed: {ex.Message}", ex);
      }
    }

    public async Task RegisterAsync(UserRegister registerModel)
    {
      try
      {
        var user = await _userService.RegisterAsync(registerModel);

        _currentUser = user;
        _isAuthenticated = true;

        await StoreTokenAsync(user.Token);

        NotifyAuthStateChanged();
      }
      catch (Exception ex)
      {
        _currentUser = null;
        _isAuthenticated = false;
        throw new HttpRequestException($"Register request failed: {ex.Message}", ex);
      }
    }

    public async Task LogoutAsync()
    {
      _currentUser = null;
      _isAuthenticated = false;
      _isInitialized = false;

      await ClearTokenAsync();

      NotifyAuthStateChanged();
    }

    // If we want to update the user information use the InvalidateUserCache method.
    // await UserService.UpdateProfileAsync(...);
    // UserState.InvalidateUserCache();
    // NavigationManager.NavigateTo("/profile", forceLoad: true);
    // }
    public void InvalidateUserCache()
    {
      _isInitialized = false;
    }

    public async Task<bool> InitializeAuthenticationStateAsync()
    {
      if (_isInitialized)
      {
        return _isAuthenticated;
      }
      var token = await GetStoredTokenAsync();

      if (string.IsNullOrEmpty(token))
      {
        _isAuthenticated = false;
        _currentUser = null;
        _isInitialized = true;
        return false;
      }

      try
      {
        var userProfile = await _userService.GetCurrentUserProfileAsync(token);

        if (userProfile != null)
        {
          _currentUser = userProfile;
          _isAuthenticated = true;
          _isInitialized = true;

          return true;
        }

        _isInitialized = true;
        return false;
      }
      catch
      {
        await LogoutAsync();
        _isInitialized = true;
        return false;
      }
    }

    // Helper methods for token storage
    private async Task StoreTokenAsync(string token)
    {
      // This example uses browser local storage, but you can use more secure approaches
      await _localStorage.SetItemAsync("authToken", token);
    }

    private async Task<string> GetStoredTokenAsync()
    {
      return await _localStorage.GetItemAsync<string>("authToken") ?? string.Empty;
    }

    private async Task ClearTokenAsync()
    {
      await _localStorage.RemoveItemAsync("authToken");
    }

    private void NotifyAuthStateChanged()
    {
      OnAuthStateChanged?.Invoke();
    }
  }
}