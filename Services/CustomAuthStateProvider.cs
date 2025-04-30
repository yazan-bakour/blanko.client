using System.Security.Claims;
using Banko.Client.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace Banko.Client.Services;
public class CustomAuthStateProvider : AuthenticationStateProvider
{
  private readonly IAuthService _authService;
  private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

  public CustomAuthStateProvider(IAuthService authService)
  {
    _authService = authService;
    _authService.OnAuthStateChanged += AuthStateChanged;
  }

  public override async Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    if (_cachedUser.Identity is { IsAuthenticated: true })
    {
      return new AuthenticationState(_cachedUser);
    }

    var isAuthenticated = await _authService.IsAuthenticatedAsync();

    if (!isAuthenticated)
    {
      _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());
      return new AuthenticationState(_cachedUser);
    }

    var currentUser = await _authService.GetCurrentUserAsync();

    if (currentUser == null)
    {
      _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());
      return new AuthenticationState(_cachedUser);
    }

    var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, currentUser.User.FullName ?? string.Empty),
                new Claim(ClaimTypes.Email, currentUser.User.Email ?? string.Empty),
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