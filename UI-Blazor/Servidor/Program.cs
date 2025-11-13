// --- Using statements for the Server (ASP.NET Core) ---
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

// 1. --- Create the application builder ---
// Use WebApplicationBuilder for server-side apps
var builder = WebApplication.CreateBuilder(args);

// 2. --- Register services with the container (for the server) ---

// This is where you would add services for your API controllers,
// database context, authentication, etc.
// For example: builder.Services.AddDbContext<MyDbContext>();

// Add services to support controllers and API endpoints
builder.Services.AddControllersWithViews();
// Add services to support Razor Pages (optional, but good practice)
builder.Services.AddRazorPages();

// Add Swagger services for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 3. --- Build the application ---
var app = builder.Build();

// 4. --- Configure the HTTP request pipeline ---

// Configure exception handling and HSTS for production
if (app.Environment.IsDevelopment())
{
    // Enables detailed error pages and debugging tools for the client-side app
    app.UseWebAssemblyDebugging();
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();
    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI();
}
else
{
    // Use a generic error handler for production
    app.UseExceptionHandler("/Error");
    // Enforce HTTPS
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Serve the Blazor WebAssembly application files
// This middleware finds the client app (Cliente.csproj) and serves its content
app.UseBlazorFrameworkFiles();

// Serve static files from wwwroot (like images, css)
app.UseStaticFiles();

// Enable routing to match incoming requests to endpoints
app.UseRouting();

// 5. --- Map endpoints ---

// Map Razor Pages (if you have any in the Pages folder)
app.MapRazorPages();
// Map API controllers (e.g., /api/clientes)
app.MapControllers();
// Set the client-side app as the fallback for any requests that don't match an API or file
app.MapFallbackToFile("index.html");


// 6. --- Run the server application ---
app.Run();