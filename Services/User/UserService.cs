using Banko.Client.Models.User;
using System.Net.Http.Json;
using System.Text.Json;
using Banko.Client.Helper;

namespace Banko.Client.Services.User
{
  public class UserService(
    HttpClient httpClient,
    IConfiguration configuration,
    AuthHelper authHelper,
    ErrorService errorService,
    JsonSerializerOptions jsonSerializerOptions) : IUserService
  {
    private readonly string _baseUrl = $"{configuration["API_HTTP_BASE_URL"]}/api/users";

    public async Task<UserRead> GetCurrentUserProfileAsync()
    {
      try
      {
        var response = await httpClient.GetAsync($"{_baseUrl}/current");

        if (!response.IsSuccessStatusCode)
        {
          if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
          {
            await authHelper.ClearTokenAsync();
          }
          throw new UnauthorizedAccessException("Authentication failed.");
        }

        var result = await response.Content.ReadFromJsonAsync<UserRead>(jsonSerializerOptions);
        return result!;
      }
      catch (Exception ex)
      {
        await Console.Error.WriteLineAsync($"Error fetching user profile: {ex.Message}");

        return null!;
      }
    }

    public async Task<UserRead?> UpdateUserProfileAsync(UserUpdate userUpdate)
    {
      try
      {
        var response = await httpClient.PutAsJsonAsync($"{_baseUrl}/settings", userUpdate, jsonSerializerOptions);
        if (response.IsSuccessStatusCode)
        {

          var updatedUser = await response.Content.ReadFromJsonAsync<UserRead>(jsonSerializerOptions);
          return updatedUser;
        }
        else
        {
          await errorService.HandleHttpResponseErrorAsync(response);
          return null;
        }
      }
      catch (HttpRequestException httpEx)
      {
        throw new HttpRequestException($"User details update failed. Status: {httpEx.StatusCode}", httpEx);
      }
    }
  }
}