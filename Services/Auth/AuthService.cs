using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Banko.Client.Models;
using Banko.Client.Helper;
using Blazored.LocalStorage;

namespace Banko.Client.Services.Auth;
public class AuthService(
  HttpClient httpClient,
  IConfiguration configuration,
  ILocalStorageService localStorage,
  ICacheValidator<UserRead> cacheValidator) : IAuthService
{
  private readonly string _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/users";
  private readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public event Action? OnAuthStateChanged;

  public async Task<bool> LoginAsync(UserLogin loginModel)
  {
    try
    {
      var content = JsonSerializer.Serialize(loginModel);
      var requestContent = new StringContent(content, Encoding.UTF8, "application/json");
      var response = await httpClient.PostAsync($"{_baseUrl}/login", requestContent);

      if (!response.IsSuccessStatusCode)
      {
        throw new HttpRequestException(
            $"Login failed with status code: {response.StatusCode}, " +
            $"Message: {await response.Content.ReadAsStringAsync()}");
      }

      var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);

      if (result == null)
      {
        throw new InvalidOperationException("Failed to deserialize login response");
      }

      await localStorage.SetItemAsync("authToken", result.Token);
      cacheValidator.UpdateCache(true, result);
      NotifyAuthStateChanged();
      return true;
    }
    catch (Exception ex)
    {
      cacheValidator.UpdateCache(false, null);
      throw new HttpRequestException($"Login request failed: {ex.Message}", ex);
    }
  }

  public async Task<bool> RegisterAsync(UserRegister registerModel)
  {
    try
    {
      var content = JsonSerializer.Serialize(registerModel);
      var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

      var response = await httpClient.PostAsync($"{_baseUrl}/register", requestContent);

      if (!response.IsSuccessStatusCode)
      {
        throw new HttpRequestException(
          $"Register failed with status code: {response.StatusCode}, " +
          $"Message: {await response.Content.ReadAsStringAsync()}");
      }

      var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);
      if (result == null)
      {
        throw new InvalidOperationException("Failed to deserialize register response");
      }
      NotifyAuthStateChanged();
      return true;
    }
    catch (Exception ex) when (ex is not HttpRequestException)
    {
      throw new HttpRequestException($"Register request failed: {ex.Message}", ex);
    }
  }

  public async Task LogoutAsync()
  {
    await localStorage.RemoveItemAsync("authToken");
    cacheValidator.UpdateCache(false, null);
    httpClient.DefaultRequestHeaders.Authorization = null;
    NotifyAuthStateChanged();
  }
  public async Task<bool> IsAuthenticatedAsync()
  {
    if (!cacheValidator.IsInitialized)
    {
      await InitializeAuthenticationStateAsync();
    }
    return cacheValidator.Data != null;
  }
  public async Task<string> GetTokenAsync()
  {
    return await localStorage.GetItemAsync<string>("authToken") ?? string.Empty;
  }
  public async Task<UserRead?> GetCurrentUserAsync()
  {
    if (!cacheValidator.IsInitialized)
    {
      await InitializeAuthenticationStateAsync();
    }
    return cacheValidator.Data;
  }
  public async Task InitializeAuthenticationStateAsync()
  {
    var token = await GetTokenAsync();

    if (string.IsNullOrEmpty(token))
    {
      cacheValidator.UpdateCache(true, null);
      return;
    }

    try
    {
      httpClient.DefaultRequestHeaders.Authorization =
          new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

      var response = await httpClient.GetAsync($"{_baseUrl}/current");

      if (!response.IsSuccessStatusCode)
      {
        await LogoutAsync();
        return;
      }

      var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);
      cacheValidator.UpdateCache(true, result);
    }
    catch
    {
      await LogoutAsync();
    }
  }
  private void NotifyAuthStateChanged()
  {
    OnAuthStateChanged?.Invoke();
  }
}