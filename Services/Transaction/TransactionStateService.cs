using Banko.Client.Models.Transaction;
using Banko.Client.Helper;

namespace Banko.Client.Services.Transaction
{
  public class TransactionStateService(ITransactionService TransactionService, ICacheValidator<TransactionRead[]> cacheValidator)
  {
    public TransactionRead[]? CurrentTransactions => cacheValidator.Data;
    public event Action? OnTransactionStateChanged;

    public async Task InitializeTransactionStateAsync()
    {
      if (cacheValidator.IsInitialized)
      {
        return;
      }
      try
      {

        var TransactionsData = await TransactionService.GetAllTransactionsAsync();
        cacheValidator.UpdateCache(true, TransactionsData);
        NotifyUserStateChanged();
      }
      catch
      {
        cacheValidator.UpdateCache(false, null);
        throw;
      }
    }

    // public async Task CreateTransaction(TransactionCreate TransactionCreate)
    // {
    //   var previousTransactions = cacheValidator.Data?.ToArray();
    //   try
    //   {
    //     var newTransaction = await TransactionService.CreateTransactionAsync(TransactionCreate);
    //     var currentList = previousTransactions?.ToList() ?? new List<TransactionRead>();
    //     currentList.Add(newTransaction);
    //     cachedUserId = TransactionCreate.UserId;
    //     cacheValidator.UpdateCache(true, currentList.ToArray());
    //     NotifyUserStateChanged();
    //   }
    //   catch (Exception ex)
    //   {
    //     Console.WriteLine($"TransactionStateService: Error creating new Transactions for UserId : {ex.Message}");
    //     throw;
    //   }
    // }

    private void NotifyUserStateChanged()
    {
      OnTransactionStateChanged?.Invoke();
    }
  }
}