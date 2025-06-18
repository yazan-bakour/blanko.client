// filepath: BlazorStateManager/BlazorStateManager/Client/State/UserState.cs
using System;
using System.Threading.Tasks;
using Banko.Client.Models.User;

namespace Banko.Client.State
{
  public class UserState
  {
    public UserRead? CurrentUser { get; private set; }

    public event Action OnUserStateChanged;

    public async Task LoadUserDataAsync()
    {
      await Task.Delay(1000);
      NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnUserStateChanged?.Invoke();
  }
}