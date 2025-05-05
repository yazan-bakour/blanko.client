using System.Net.Http.Json;
using Banko.Client.Models.Transaction;
using Banko.Client.Helper;
using System.Text.Json;

namespace Banko.Client.Services.Transaction;
public class TransactionService(HttpClient httpClient, IConfiguration configuration, AuthHelper authHelper, JsonSerializerOptions jsonSerializerOptions) : ITransactionService
{
  private readonly string _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/transactions";

  public async Task<TransactionRead[]> GetAllTransactionsAsync()
  {
    await authHelper.AuthorizationHeaderAsync();

    var response = await httpClient.GetFromJsonAsync<TransactionRead[]>($"{_baseUrl}/", jsonSerializerOptions);
    return response ?? [];
  }
}