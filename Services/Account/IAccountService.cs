using Banko.Client.Models.Account;

namespace Banko.Client.Services.Account
{
  public interface IAccountService
  {
    Task<AccountRead[]> GetAccountByIdAsync(int userId);
  }
}
