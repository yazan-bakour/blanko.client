using Banko.Client.Models;
using Blazored.LocalStorage;

namespace Banko.Client.Services.User
{
  public class UserStateService(IUserService userService, ILocalStorageService localStorage)
  {
    private readonly IUserService _userService = userService;
    private UserRead? _currentUser;
    private bool _isAuthenticated;
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

    public async Task LogoutAsync()
    {
      _currentUser = null;
      _isAuthenticated = false;

      await ClearTokenAsync();

      NotifyAuthStateChanged();
    }

    public async Task<bool> InitializeAuthenticationStateAsync()
    {
      var token = await GetStoredTokenAsync();

      if (string.IsNullOrEmpty(token))
      {
        return false;
      }

      try
      {
        // TODO: Add a method to your UserService to validate token or get user info
        // _currentUser = await _userService.GetUserInfoAsync();
        _isAuthenticated = true;
        return true;
      }
      catch
      {
        await LogoutAsync();
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