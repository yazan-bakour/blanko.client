
using Banko.Client.Services.Auth;

namespace Banko.Client.Helper;

public class AuthHelper(IAuthService AuthService)
{
  private static readonly HttpClient _httpClient = new();
  private readonly IAuthService _AuthService = AuthService;

  public async Task AuthorizationHeaderAsync()
  {
    var token = await _AuthService.GetTokenAsync();

    if (string.IsNullOrEmpty(token))
    {
      throw new UnauthorizedAccessException("User is not authenticated");
    }

    _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
  }
}