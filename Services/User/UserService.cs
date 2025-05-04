using Banko.Client.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Banko.Client.Helper;

namespace Banko.Client.Services.User
{
  public class UserService : IUserService
  {
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly AuthHelper _authHelper;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
      PropertyNameCaseInsensitive = true
    };

    public UserService(HttpClient httpClient, IConfiguration configuration, AuthHelper authHelper)
    {
      _httpClient = httpClient;
      _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/users";
      _authHelper = authHelper;
    }

    public async Task<UserRead?> GetCurrentUserProfileAsync()
    {
      // Check if we have a valid authorization header
      if (!await _authHelper.AuthorizationHeaderAsync())
      {
        return null; // Return null to indicate no authentication
      }

      try
      {
        var response = await _httpClient.GetAsync($"{_baseUrl}/current");

        if (!response.IsSuccessStatusCode)
        {
          if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
          {
            await _authHelper.ClearTokenAsync();
          }
          return null; // Return null to indicate authentication failed
        }

        var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);
        return result; // Return the user data or null if deserialization failed
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error fetching user profile: {ex.Message}");
        // If any exception occurs during API call, return null
        return null;
      }
    }
  }
}