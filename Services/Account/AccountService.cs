using System.Net.Http.Json;
using Banko.Client.Models.Account;
using Banko.Client.Helper;
using System.Text.Json;

namespace Banko.Client.Services.Account;
public class AccountService(
  HttpClient httpClient,
  IConfiguration configuration,
  AuthHelper authHelper,
  JsonSerializerOptions jsonSerializerOptions,
  ErrorService errorService) : IAccountService
{
  private readonly string _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/accounts";

  public async Task<AccountRead[]> GetAccountByIdAsync(int userId)
  {
    await authHelper.AuthorizationHeaderAsync();

    var response = await httpClient.GetFromJsonAsync<AccountRead[]>($"{_baseUrl}/user/{userId}/accounts", jsonSerializerOptions);
    return response ?? [];
  }

  public async Task<AccountRead> CreateAccountAsync(AccountCreate accountCreate)
  {
    try
    {
      await authHelper.AuthorizationHeaderAsync();
      var response = await httpClient.PostAsJsonAsync($"{_baseUrl}/create", accountCreate, jsonSerializerOptions);
      if (!response.IsSuccessStatusCode)
      {
        await errorService.HandleHttpResponseErrorAsync(response);
      }
      var createdAccount = await response.Content.ReadFromJsonAsync<AccountRead>(jsonSerializerOptions) ?? throw new InvalidOperationException("API did not return the created account details.");
      return createdAccount;
    }
    catch (HttpRequestException httpEx)
    {
      throw new HttpRequestException($"Account creation failed. Status: {httpEx.StatusCode}", httpEx);
    }
  }
}