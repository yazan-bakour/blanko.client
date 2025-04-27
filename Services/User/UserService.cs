using Banko.Client.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Banko.Client.Services.User
{
  public class UserService : IUserService
  {
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
      PropertyNameCaseInsensitive = true
    };

    public UserService(HttpClient httpClient, IConfiguration configuration)
    {
      _httpClient = httpClient;
      _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/users";

      _httpClient.DefaultRequestHeaders.Accept.Clear();
      _httpClient.DefaultRequestHeaders.Accept.Add(
          new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<UserRead> LoginAsync(UserLogin loginModel)
    {
      try
      {
        var content = JsonSerializer.Serialize(loginModel);
        var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/login", requestContent);

        if (!response.IsSuccessStatusCode)
        {
          throw new HttpRequestException(
            $"Login failed with status code: {response.StatusCode}, " +
            $"Message: {await response.Content.ReadAsStringAsync()}");
        }

        var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);
        return result ?? throw new InvalidOperationException("Failed to deserialize login response");
      }
      catch (Exception ex) when (ex is not HttpRequestException)
      {
        throw new HttpRequestException($"Login request failed: {ex.Message}", ex);
      }
    }
    public async Task<UserRead> RegisterAsync(UserRegister registerModel)
    {
      try
      {
        var content = JsonSerializer.Serialize(registerModel);
        var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/register", requestContent);

        if (!response.IsSuccessStatusCode)
        {
          throw new HttpRequestException(
            $"Register failed with status code: {response.StatusCode}, " +
            $"Message: {await response.Content.ReadAsStringAsync()}");
        }

        var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);
        return result ?? throw new InvalidOperationException("Failed to deserialize register response");
      }
      catch (Exception ex) when (ex is not HttpRequestException)
      {
        throw new HttpRequestException($"Register request failed: {ex.Message}", ex);
      }
    }
    public async Task<UserRead> GetCurrentUserProfileAsync(string token)
    {
      _httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

      var response = await _httpClient.GetAsync($"{_baseUrl}/current");

      if (!response.IsSuccessStatusCode)
      {
        throw new HttpRequestException(
          $"Failed to get current user profile: {response.StatusCode}, " +
          $"Message: {await response.Content.ReadAsStringAsync()}");
      }

      var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);
      return result ?? throw new InvalidOperationException("Failed to deserialize login response");
    }
  }
}