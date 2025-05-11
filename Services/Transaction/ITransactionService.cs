using Banko.Client.Models.Transaction;

namespace Banko.Client.Services.Transaction
{
  public interface ITransactionService
  {
    Task<TransactionRead[]> GetAllTransactionsAsync();
    Task<TransactionRead> CreateTransactionAsync(TransactionCreate TransactionCreate);
  }
}
