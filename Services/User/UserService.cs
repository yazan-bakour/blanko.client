using Banko.Client.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Banko.Client.Services.User
{
	public interface IUserService
	{
		Task<UserRead> LoginAsync(UserLogin loginModel);
	}

	public class UserService : IUserService
	{
		private readonly HttpClient _httpClient;
		private const string BaseUrl = "http://localhost:5077/api/users";
		private static readonly JsonSerializerOptions _jsonOptions = new()
		{
			PropertyNameCaseInsensitive = true
		};

		public UserService(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(
					new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<UserRead> LoginAsync(UserLogin loginModel)
		{
			try
			{
				var content = JsonSerializer.Serialize(loginModel);
				var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

				var response = await _httpClient.PostAsync($"{BaseUrl}/login", requestContent);

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(
							$"Login failed with status code: {response.StatusCode}, " +
							$"Message: {await response.Content.ReadAsStringAsync()}");
				}

				var result = await response.Content.ReadFromJsonAsync<UserRead>(_jsonOptions);
				return result ?? throw new InvalidOperationException("Failed to deserialize login response");
			}
			catch (Exception ex) when (ex is not HttpRequestException)
			{
				throw new HttpRequestException($"Login request failed: {ex.Message}", ex);
			}
		}
	}
}