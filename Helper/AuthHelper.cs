using Blazored.LocalStorage;

namespace Banko.Client.Helper;

public class AuthHelper(ILocalStorageService localStorage, HttpClient httpClient)
{
  public event Action? OnAuthStateChanged;

  public void NotifyAuthStateChanged()
  {
    OnAuthStateChanged?.Invoke();
  }

  public void SetAuthorizationHeader(string token)
  {
    if (!string.IsNullOrEmpty(token))
    {
      httpClient.DefaultRequestHeaders.Authorization =
          new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
  }

  public async Task<bool> AuthorizationHeaderAsync()
  {
    try
    {
      var token = await localStorage.GetItemAsync<string>("authToken");
      if (string.IsNullOrEmpty(token))
        return false;

      SetAuthorizationHeader(token);
      return true;
    }
    catch
    {
      return false;
    }
  }

  public async Task ClearTokenAsync()
  {
    await localStorage.RemoveItemAsync("authToken");
    httpClient.DefaultRequestHeaders.Authorization = null;
    NotifyAuthStateChanged();
  }
}