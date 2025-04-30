using Banko.Client.Models.Account;
using Banko.Client.Helper;

namespace Banko.Client.Services.Account
{
  public class AccountStateService(IAccountService accountService, ICacheValidator<AccountRead[]> cacheValidator)
  {
    private readonly IAccountService _accountService = accountService;

    public AccountRead[]? CurrentAccounts => cacheValidator.Data;
    public event Action? OnUserStateChanged;

    public async Task InitializeAccountStateAsync(int userId)
    {
      if (cacheValidator.IsInitialized) return;
      try
      {

        var accountsData = await _accountService.GetAccountByIdAsync(userId);
        cacheValidator.UpdateCache(true, accountsData);
        NotifyUserStateChanged();
      }
      catch
      {
        cacheValidator.UpdateCache(false, null);
        throw;
      }
    }

    private void NotifyUserStateChanged()
    {
      OnUserStateChanged?.Invoke();
    }
  }
}