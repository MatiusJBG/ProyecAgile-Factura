using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Cliente;
using Cliente.Services.Clientes;
using Cliente.Services.Facturacion;
using Cliente.Services.Inventario;
using Cliente.Services.Auth;
using Cliente.Services.Certificados;
using Cliente.Services.Common;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient con headers anti-caché
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    
    // Headers anti-caché para evitar que el navegador cachee las respuestas GET
    httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
    {
        NoCache = true,
        NoStore = true,
        MustRevalidate = true
    };
    httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
    
    return httpClient;
});

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IImagenService, ImagenService>();
builder.Services.AddScoped<FacturaService>();
builder.Services.AddScoped<IDescuentoService, DescuentoService>();
builder.Services.AddScoped<FirmaElectronicaHttpClient>();
builder.Services.AddScoped<CertificadoHttpClient>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
