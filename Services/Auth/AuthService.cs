using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Banko.Client.Models;
using Banko.Client.Helper;
using Blazored.LocalStorage;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Banko.Client.Services.Auth;
public class AuthService(
  HttpClient httpClient,
  IConfiguration configuration,
  ILocalStorageService localStorage,
  ICacheValidator<UserRead> cacheValidator,
  AuthHelper authHelper) : IAuthService
{
  private readonly string _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/users";
  private readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public async Task LoginAsync(UserLogin loginModel)
  {
    try
    {
      var response = await httpClient.PostAsJsonAsync($"{_baseUrl}/login", loginModel);
      response.EnsureSuccessStatusCode();

      var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);

      if (result == null || string.IsNullOrEmpty(result.Token))
      {
        throw new InvalidOperationException("Login successful but token was not received.");
      }

      await localStorage.SetItemAsync("authToken", result.Token);
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
      cacheValidator.UpdateCache(true, result);
    }
    catch (Exception ex)
    {
      await authHelper.ClearTokenAsync();
      cacheValidator.UpdateCache(false, null);
      Console.WriteLine($"Login failed: {ex.Message}");
      throw;
    }
  }
  public async Task<UserRead> UserInfoAsync()
  {
    var response = await httpClient.GetFromJsonAsync<UserRead>($"{_baseUrl}/current") ?? throw new InvalidOperationException("Received null response from user endpoint.");
    return response!;
  }

  public async Task RegisterAsync(UserRegister registerModel)
  {
    try
    {
      var response = await httpClient.PostAsJsonAsync($"{_baseUrl}/register", registerModel);
      response.EnsureSuccessStatusCode();
    }
    catch (HttpRequestException httpEx)
    {
      throw new HttpRequestException($"Registration failed. Status: {httpEx.StatusCode}", httpEx);
    }
    catch (Exception ex)
    {
      throw new ApplicationException("An unexpected error occurred during registration.", ex); // Re-throw as a different type if desired
    }
  }

  public async Task LogoutAsync()
  {
    await authHelper.ClearTokenAsync();
    cacheValidator.UpdateCache(false, null);
  }
}