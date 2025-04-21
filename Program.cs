// Banko.Client/Program.cs
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Banko.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Banko.Client.Services;
using Blazored.LocalStorage;
using Banko.Client.Services.User;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserStateService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddMudServices();

// using (var scope = host.Services.CreateScope())
// {
//     var userState = scope.ServiceProvider.GetRequiredService<UserStateService>();
//     await userState.InitializeAuthenticationStateAsync();
// }

var apiBaseAddress = builder.Configuration["API_BASE_URL"] ??
  throw new InvalidOperationException("API Base Address is not configured.");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

await builder.Build().RunAsync();