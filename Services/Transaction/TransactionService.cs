using System.Net.Http.Json;
using Banko.Client.Models.Transaction;
using Banko.Client.Helper;
using System.Text.Json;

namespace Banko.Client.Services.Transaction;
public class TransactionService(HttpClient httpClient, IConfiguration configuration, AuthHelper authHelper, JsonSerializerOptions jsonSerializerOptions, ErrorService errorService) : ITransactionService
{
  private readonly string _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/transactions";

  public async Task<TransactionRead[]> GetAllTransactionsAsync()
  {
    await authHelper.AuthorizationHeaderAsync();

    var response = await httpClient.GetFromJsonAsync<TransactionRead[]>($"{_baseUrl}/", jsonSerializerOptions);
    return response ?? [];
  }
  public async Task<TransactionRead> CreateTransactionAsync(TransactionCreate TransactionCreate)
  {
    await authHelper.AuthorizationHeaderAsync();

    var response = await httpClient.PostAsJsonAsync($"{_baseUrl}/", TransactionCreate, jsonSerializerOptions);

    if (!response.IsSuccessStatusCode)
    {
      await errorService.HandleHttpResponseErrorAsync(response);
    }

    var createdTransaction = await response.Content.ReadFromJsonAsync<TransactionRead>(jsonSerializerOptions);

    return createdTransaction ?? throw new InvalidOperationException("API did not return the created transaction details.");
  }
}