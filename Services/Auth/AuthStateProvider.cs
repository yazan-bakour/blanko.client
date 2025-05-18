using System.Security.Claims;
using Banko.Client.Helper;
using Microsoft.AspNetCore.Components.Authorization;
using Banko.Client.Models.User;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Banko.Client.Services.Auth;

public class AuthStateProvider : AuthenticationStateProvider
{
  private readonly IAuthService _authService;
  private UserRead? _userCache;
  private readonly AuthHelper _authHelper;
  private readonly ILocalStorageService _localStorage;
  private readonly ICacheValidator<UserRead> _cacheValidator;
  private readonly HttpClient _httpClient;

  public AuthStateProvider(
    IAuthService authService,
    AuthHelper authHelper,
    ILocalStorageService localStorage,
    ICacheValidator<UserRead> cacheValidator,
    HttpClient httpClient)
  {
    _authService = authService;
    _authHelper = authHelper;
    _localStorage = localStorage;
    _cacheValidator = cacheValidator;
    _httpClient = httpClient;

    _authHelper.OnAuthStateChanged += () =>
    {
      NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    };
  }

  public async Task Login(UserLogin loginModel)
  {
    await _authService.LoginAsync(loginModel);
    _userCache = null;
    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
  }
  public async Task Register(UserRegister registerModel)
  {
    await _authService.RegisterAsync(registerModel);
    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
  }
  public async Task<UserRead> GetUserInfo()
  {
    if (_userCache != null)
    {
      return _userCache;
    }
    _userCache = await _authService.UserInfoAsync();
    return _userCache;
  }

  public override async Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    var identity = new ClaimsIdentity();
    var token = await _localStorage.GetItemAsync<string>("authToken");

    if (!string.IsNullOrEmpty(token))
    {
      try
      {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var userInfo = await GetUserInfo();

        if (userInfo?.User != null)
        {
          var user = userInfo.User;
          var claims = new List<Claim>
          {
              new (ClaimTypes.NameIdentifier, user.Id.ToString()),
              new (ClaimTypes.Email, user.Email ?? string.Empty),
              new (ClaimTypes.Name, user.FullName ?? string.Empty),
              new (ClaimTypes.GivenName, user.FirstName ?? string.Empty),
              new (ClaimTypes.Surname, user.LastName ?? string.Empty),
              new (ClaimTypes.Role, user.Role?.ToString() ?? string.Empty),
              new (ClaimTypes.DateOfBirth, user.DateOfBirth?.ToString("o") ?? string.Empty),
              new (ClaimTypes.Gender, user.Gender?.ToString() ?? string.Empty),
              new (ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
              new (ClaimTypes.StreetAddress, user.Address ?? string.Empty),
              new (ClaimTypes.Locality, user.City ?? string.Empty),
              new (ClaimTypes.StateOrProvince, user.State ?? string.Empty),
              new (ClaimTypes.PostalCode, user.ZipCode ?? string.Empty),
              new (ClaimTypes.Country, user.Country ?? string.Empty),
              new ("uniqueId", user.UniqueId ?? string.Empty),
              new ("nationality", user.Nationality ?? string.Empty),
              new ("profilePictureDisplay", user.ProfilePictureDisplay ?? string.Empty),
              new ("isVerified", user.IsVerified.ToString()),
              new ("createdAt", user.CreatedAt?.ToString("o") ?? string.Empty),
              new ("updatedAt", user.UpdatedAt.ToString("o") ?? string.Empty),
          };
          identity = new ClaimsIdentity(claims, "Server authentication");
          _cacheValidator.UpdateCache(true, userInfo);
        }
      }
      catch
      {
        identity = new ClaimsIdentity();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        await _authHelper.ClearTokenAsync();
      }
    }
    else
    {
      Console.WriteLine("GetAuthenticationStateAsync: No token found. Ensuring anonymous state.");
    }

    return new AuthenticationState(new ClaimsPrincipal(identity));
  }
}