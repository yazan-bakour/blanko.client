using Banko.Client.Models.Account;
using Banko.Client.Services;
using Banko.Client.Services.Account;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Banko.Client.State;

public class AccountComposable(ISnackbar Snackbar, LoadingService LoadingService, AccountStateService AccountStateService)
{

  public AccountRead[]? Accounts { get; set; }
  public bool IsDataLoaded { get; private set; }
  public int? CachedUserId { get; set; }

  public async Task OnInitializeAsync(int userId, bool forceRefresh = false)
  {
    if (LoadingService.IsLoading && !forceRefresh)
    {
      return;
    }
    LoadingService.IsLoading = true;
    try
    {
      await AccountStateService.InitializeAccountStateAsync(userId, forceRefresh);
      Accounts = AccountStateService.CurrentAccounts;
      IsDataLoaded = true;
    }
    catch (Exception ex)
    {
      Snackbar.Add($"Error loading accounts: {ex.Message}", Severity.Error);
    }
    finally
    {
      LoadingService.IsLoading = false;
      IsDataLoaded = true;
      AccountStateService.NotifyAccountStateChanged();
    }
  }
  public void RegisterComponent(Action handler)
  {
    AccountStateService.OnAccountStateChanged += handler;
  }
  public void UnregisterComponent(Action handler)
  {
    AccountStateService.OnAccountStateChanged -= handler;
  }
}
