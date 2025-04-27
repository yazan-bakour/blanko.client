using System.Security.Claims;
using Banko.Client.Services.User;
using Microsoft.AspNetCore.Components.Authorization;

namespace Banko.Client.Services
{
  public class CustomAuthStateProvider : AuthenticationStateProvider
  {
    private readonly UserStateService _userStateService;
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

    public CustomAuthStateProvider(UserStateService userStateService)
    {
      _userStateService = userStateService;
      _userStateService.OnAuthStateChanged += AuthStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      if (_cachedUser.Identity is { IsAuthenticated: true })
      {
        return new AuthenticationState(_cachedUser);
      }

      await _userStateService.InitializeAuthenticationStateAsync();

      if (!_userStateService.IsAuthenticated)
      {
        _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
      }

      var identity = new ClaimsIdentity(
        [
          new Claim(ClaimTypes.Name, _userStateService.CurrentUser?.User.FullName ?? string.Empty),
          new Claim(ClaimTypes.Email, _userStateService.CurrentUser?.User.Email ?? string.Empty),
        ], "apiauth_type"
      );

      _cachedUser = new ClaimsPrincipal(identity);

      return new AuthenticationState(_cachedUser);
    }

    private void AuthStateChanged()
    {
      NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
  }
}