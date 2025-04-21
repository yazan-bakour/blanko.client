// Banko.Client/Program.cs
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Banko.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Banko.Client.Services;
using Blazored.LocalStorage;
using Banko.Client.Services.User;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
// builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserStateService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

// Configure the HttpClient to use your API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7296") });


await builder.Build().RunAsync();