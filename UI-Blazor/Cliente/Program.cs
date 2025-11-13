// --- Using statements for the Client (Blazor WebAssembly) ---
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

// Make sure this namespace matches your Client project's name
using Cliente; 

// 1. --- Create the application builder ---
// Use WebAssemblyHostBuilder for browser-based apps
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// 2. --- Register root components ---
// Finds <div id="app"> in wwwroot/index.html
builder.RootComponents.Add<App>("#app"); 
// Adds support for <PageTitle> and <HeadContent> components
builder.RootComponents.Add<HeadOutlet>("head::after");

// 3. --- Register services with the container (for the client) ---

// Configure the HttpClient to point to the server's base address
// This allows you to make API calls (e.g., http.GetAsync("/api/clientes"))
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// (You can add other client-side services here, like MudBlazor, etc.)


// 4. --- Build and run the client application ---
await builder.Build().RunAsync();