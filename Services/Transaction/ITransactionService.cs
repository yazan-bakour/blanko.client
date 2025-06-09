using Banko.Client.Models.Transaction;

namespace Banko.Client.Services.Transaction
{
  public interface ITransactionService
  {
    Task<TransactionRead[]> GetAllTransactionsAsync();
    Task<TransactionRead> CreateTransactionAsync(TransactionCreate TransactionCreate);
    Task<byte[]> DownloadStatementPdfAsync(
      DateTime? fromDate = null,
      DateTime? toDate = null,
      string? period = null
    );
  }
}
