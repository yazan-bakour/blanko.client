using System.Net.Http.Json;
using System.Text.Json;
using Banko.Client.Models.Account;
using Banko.Client.Helper;

namespace Banko.Client.Services.Account
{
  public class AccountService(HttpClient httpClient, IConfiguration configuration, AuthHelper authHelper) : IAccountService
  {
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/accounts";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
      PropertyNameCaseInsensitive = true
    };
    private readonly AuthHelper _authHelper = authHelper;

    public async Task<AccountRead[]> GetAccountByIdAsync(int userId)
    {
      await _authHelper.AuthorizationHeaderAsync();

      var response = await _httpClient.GetAsync($"{_baseUrl}/user/{userId}/accounts");
      if (!response.IsSuccessStatusCode)
      {
        throw new HttpRequestException(
          $"Failed to get current user profile: {response.StatusCode}, " +
          $"Message: {await response.Content.ReadAsStringAsync()}");
      }

      var result = await response.Content.ReadFromJsonAsync<AccountRead[]>(_jsonOptions);
      return result ?? [];
    }
  }
}