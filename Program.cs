// Banko.Client/Program.cs
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Banko.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Banko.Client.Services;
using Blazored.LocalStorage;
using Banko.Client.Services.User;
using MudBlazor.Services;
using Banko.Client.Services.Account;
using Banko.Client.Services.Transaction;
using Banko.Client.Services.Auth;
using Banko.Client.Helper;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthStateProvider>());

builder.Services.AddScoped<LoadingService>();
builder.Services.AddScoped<AuthHelper>();
builder.Services.AddScoped(typeof(ICacheValidator<>), typeof(CacheValidator<>));

builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<UserStateService>();
builder.Services.AddScoped<AccountStateService>();
builder.Services.AddScoped<TransactionStateService>();


builder.Services.AddMudServices();

var apiBaseAddress = builder.Configuration["API_BASE_URL"] ?? throw new InvalidOperationException("API Base Address is not configured.");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

builder.Services.Configure<System.Text.Json.JsonSerializerOptions>(options =>
{
  options.Converters.Add(new JsonStringEnumConverter());
  options.PropertyNameCaseInsensitive = true;
});
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JsonSerializerOptions>>().Value);

await builder.Build().RunAsync();