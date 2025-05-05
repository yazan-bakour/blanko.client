using Banko.Client.Models.Account;
using Banko.Client.Helper;

namespace Banko.Client.Services.Account
{
  public class AccountStateService(IAccountService accountService, ICacheValidator<AccountRead[]> cacheValidator)
  {
    private int? cachedUserId = null;
    public AccountRead[]? CurrentAccounts => cacheValidator.Data;
    public event Action? OnAccountStateChanged;

    public async Task InitializeAccountStateAsync(int userId)
    {
      if (cacheValidator.IsInitialized && cachedUserId.HasValue && cachedUserId.Value == userId)
      {
        Console.WriteLine($"AccountStateService: Cache hit for UserId {userId}.");
        return;
      }
      Console.WriteLine($"AccountStateService: Cache miss or different UserId. Fetching for UserId {userId}.");
      try
      {

        var accountsData = await accountService.GetAccountByIdAsync(userId);
        cachedUserId = userId;
        cacheValidator.UpdateCache(true, accountsData);
        NotifyUserStateChanged();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"AccountStateService: Error fetching accounts for UserId {userId}: {ex.Message}");
        cachedUserId = null;
        cacheValidator.UpdateCache(false, null);
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
        NotifyUserStateChanged();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"AccountStateService: Error creating new accounts for UserId : {ex.Message}");
        throw;
      }
    }

    private void NotifyUserStateChanged()
    {
      OnAccountStateChanged?.Invoke();
    }
  }
}