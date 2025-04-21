using System.Security.Claims;
using Banko.Client.Services.User;
using Microsoft.AspNetCore.Components.Authorization;

namespace Banko.Client.Services
{
  public class CustomAuthStateProvider : AuthenticationStateProvider
  {
    private readonly UserStateService _userStateService;

    public CustomAuthStateProvider(UserStateService userStateService)
    {
      _userStateService = userStateService;
      _userStateService.OnAuthStateChanged += AuthStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      await _userStateService.InitializeAuthenticationStateAsync();

      if (!_userStateService.IsAuthenticated)
      {
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
      }

      var identity = new ClaimsIdentity(
        [

          new Claim(ClaimTypes.Name, _userStateService.CurrentUser?.User.FullName ?? string.Empty),
          new Claim(ClaimTypes.Email, _userStateService.CurrentUser?.User.Email ?? string.Empty),

        ], "apiauth_type"
      );

      return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private void AuthStateChanged()
    {
      NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
  }
}