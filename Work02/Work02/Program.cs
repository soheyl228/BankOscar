using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Work02.Services;
using Work02.Interface;
using Work02.Domain;

namespace Work02
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Register root components for Blazor WebAssembly
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Register a simple user helper service (persist username in localStorage)
            // Concrete registration (no interface) — good for small apps.
            builder.Services.AddScoped<Work02.Services.UserService>();

            // Register storage abstraction used across the app to persist state (localStorage wrapper).
            // Allows swapping/mocking storage in tests.
            builder.Services.AddScoped<IStorageService, StorageService>();

            // Register account business service (application logic).
            // Components/pages inject IAccountService to create/manage accounts and transactions.
            builder.Services.AddScoped<IAccountService, AccountService>();

            // add registrations — replace the services registration area with this block (or add lines)
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBudgetService, BudgetService>();

            // HttpClient for potential HTTP calls (base address provided by host).
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Build and run the application
            await builder.Build().RunAsync();
        }
    }
}
