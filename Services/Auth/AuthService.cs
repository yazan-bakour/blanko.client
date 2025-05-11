using System.Net.Http.Json;
using System.Text.Json;
using Banko.Client.Models.User;
using Banko.Client.Helper;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Banko.Client.Services.Auth;
public class AuthService(
  HttpClient httpClient,
  IConfiguration configuration,
  ILocalStorageService localStorage,
  ICacheValidator<UserRead> cacheValidator,
  AuthHelper authHelper,
  ErrorService errorService) : IAuthService
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
      if (!response.IsSuccessStatusCode)
      {
        await errorService.HandleHttpResponseErrorAsync(response);
      }

      var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);

      if (result == null || string.IsNullOrEmpty(result.Token))
      {
        throw new InvalidOperationException("Login successful but token was not received.");
      }

      await localStorage.SetItemAsync("authToken", result.Token);
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
      cacheValidator.UpdateCache(true, result);
    }
    catch (HttpRequestException)
    {
      await authHelper.ClearTokenAsync();
      cacheValidator.UpdateCache(false, null);
      throw;
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

      if (!response.IsSuccessStatusCode)
      {
        await errorService.HandleHttpResponseErrorAsync(response);
      }
    }
    catch (HttpRequestException)
    {
      throw;
    }
    catch (Exception ex)
    {
      throw new HttpRequestException($"Registration failed: {ex.Message}", ex);
    }
  }

  public async Task LogoutAsync()
  {
    await authHelper.ClearTokenAsync();
    cacheValidator.UpdateCache(false, null);
  }
}