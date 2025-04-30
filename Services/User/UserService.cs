using Banko.Client.Models;
using Banko.Client.Services.Auth;
using System.Net.Http.Json;
using System.Text.Json;

namespace Banko.Client.Services.User
{
  public class UserService(
    HttpClient httpClient,
    IConfiguration configuration,
    IAuthService authService) : IUserService
  {
    private readonly string _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/users";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
      PropertyNameCaseInsensitive = true
    };

    public async Task<UserRead> GetCurrentUserProfileAsync()
    {
      await AuthorizationHeaderAsync();

      var response = await httpClient.GetAsync($"{_baseUrl}/current");

      if (!response.IsSuccessStatusCode)
      {
        throw new HttpRequestException(
          $"Failed to get current user profile: {response.StatusCode}, " +
          $"Message: {await response.Content.ReadAsStringAsync()}");
      }

      var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);
      return result ?? throw new InvalidOperationException("Failed to deserialize user response");
    }

    private async Task AuthorizationHeaderAsync()
    {
      var token = await authService.GetTokenAsync();

      if (string.IsNullOrEmpty(token))
      {
        throw new UnauthorizedAccessException("User is not authenticated");
      }

      httpClient.DefaultRequestHeaders.Authorization =
          new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
  }
}