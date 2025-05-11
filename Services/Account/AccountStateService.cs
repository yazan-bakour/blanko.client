using Banko.Client.Models.Account;
using Banko.Client.Helper;

namespace Banko.Client.Services.Account
{
  public class AccountStateService(IAccountService accountService, ICacheValidator<AccountRead[]> cacheValidator)
  {
    private int? cachedUserId = null;
    public AccountRead[]? CurrentAccounts => cacheValidator.Data;
    public event Action? OnAccountStateChanged;

    public async Task InitializeAccountStateAsync(int userId, bool forceRefresh = false)
    {
      if (!forceRefresh && cacheValidator.IsInitialized && cachedUserId.HasValue && cachedUserId.Value == userId)
      {
        return;
      }
      try
      {

        var accountsData = await accountService.GetAccountByIdAsync(userId);
        cachedUserId = userId;
        cacheValidator.UpdateCache(true, accountsData);
        NotifyAccountStateChanged();
      }
      catch
      {
        NotifyAccountStateChanged();
        throw;
      }
    }

    public async Task CreateAccount(AccountCreate accountCreate)
    {
      var previousAccounts = cacheValidator.Data?.ToArray();
      try
      {
        var newAccount = await accountService.CreateAccountAsync(accountCreate);
        var currentList = previousAccounts?.ToList() ?? new List<AccountRead>();
        currentList.Add(newAccount);
        cachedUserId = accountCreate.UserId;
        cacheValidator.UpdateCache(true, currentList.ToArray());
        NotifyAccountStateChanged();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    private void NotifyAccountStateChanged()
    {
      OnAccountStateChanged?.Invoke();
    }
    public void ClearAccountState()
    {
      cachedUserId = null;
      cacheValidator.UpdateCache(false, null);
      NotifyAccountStateChanged();
    }
  }
}