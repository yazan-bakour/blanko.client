using Banko.Client.Models.Transaction;
using Banko.Client.Services;
using Banko.Client.Services.Transaction;
using MudBlazor;

namespace Banko.Client.State
{
  public class TransactionsComposable(ISnackbar Snackbar, LoadingService LoadingService, TransactionStateService _transactionStateService)
  {
    public TransactionRead[]? CurrentTransactions => _transactionStateService.CurrentTransactions;

    public async Task OnInitializedAsync(bool forceRefresh = false)
    {
      if (LoadingService.IsLoading && !forceRefresh) { return; }
      LoadingService.IsLoading = true;
      try
      {
        await _transactionStateService.InitializeTransactionStateAsync(forceRefresh);
      }
      catch (Exception ex)
      {
        Snackbar.Add(ex.Message, Severity.Error);
      }
      finally
      {
        LoadingService.IsLoading = false;
        _transactionStateService.NotifyTransactionStateChanged();
      }
    }
    public void RegisterComponent(Action handler)
    {
      _transactionStateService.OnTransactionStateChanged += handler;
    }
    public void UnregisterComponent(Action handler)
    {
      _transactionStateService.OnTransactionStateChanged -= handler;
    }
  }
}