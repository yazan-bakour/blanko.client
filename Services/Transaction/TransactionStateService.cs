using Banko.Client.Models.Transaction;
using Banko.Client.Helper;

namespace Banko.Client.Services.Transaction
{
  public class TransactionStateService(ITransactionService TransactionService, ICacheValidator<TransactionRead[]> cacheValidator)
  {
    public TransactionRead[]? CurrentTransactions => cacheValidator.Data;
    public event Action? OnTransactionStateChanged;

    public async Task InitializeTransactionStateAsync(bool forceRefresh = false)
    {
      if (!forceRefresh && cacheValidator.IsInitialized)
      {
        return;
      }
      try
      {

        var TransactionsData = await TransactionService.GetAllTransactionsAsync();
        cacheValidator.UpdateCache(true, TransactionsData);
        NotifyTransactionStateChanged();
      }
      catch
      {
        cacheValidator.UpdateCache(false, null);
        throw;
      }
    }

    public async Task CreateTransaction(TransactionCreate TransactionCreate)
    {
      var previousTransactions = cacheValidator.Data?.ToArray();
      try
      {
        var newTransaction = await TransactionService.CreateTransactionAsync(TransactionCreate);
        var currentList = previousTransactions?.ToList() ?? new List<TransactionRead>();
        currentList.Add(newTransaction);
        cacheValidator.UpdateCache(true, currentList.ToArray());
        NotifyTransactionStateChanged();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public Task<byte[]> GetStatementAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    string? period = null)
    => TransactionService.DownloadStatementPdfAsync(fromDate, toDate, period);

    public void NotifyTransactionStateChanged()
    {
      OnTransactionStateChanged?.Invoke();
    }
  }
}