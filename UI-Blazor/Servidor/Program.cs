using Application.Services;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configurar DbContext con MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registrar repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ILoteRepository, LoteRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IPrecioRepository, PrecioRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Registrar servicios de aplicación
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<LoteService>();
builder.Services.AddScoped<FacturaService>();

// Configurar JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ClaveSecretaSuperSeguraParaDesarrollo12345";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Solo para desarrollo
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Simplificado para desarrollo
        ValidateAudience = false // Simplificado para desarrollo
    };
});

// Configurar CORS para permitir el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7188", "http://localhost:5188")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseAuthentication(); // Agregar middleware de autenticación
app.UseAuthorization();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapFallbackToFile("index.html");

// Aplicar migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        try 
        {
            context.Database.Migrate();
            logger.LogInformation("Migraciones aplicadas correctamente.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al ejecutar migraciones automáticas. Intentando parche manual...");
        }

        // Parche temporal: Intentar agregar la columna Imagen si falta
        try
        {
            logger.LogInformation("Intentando aplicar parche manual para columna Imagen...");
            context.Database.ExecuteSqlRaw("ALTER TABLE productos ADD COLUMN Imagen longtext CHARACTER SET utf8mb4 NULL;");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error al aplicar parche manual (probablemente ya existe la columna).");
        }

        // Crear tabla descuentos_producto manualmente si no existe
        try
        {
            logger.LogInformation("Intentando crear tabla descuentos_producto...");
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS `descuentos_producto` (
                    `Id_Desc` int NOT NULL AUTO_INCREMENT,
                    `Id_Pro_Per` int NOT NULL,
                    `Porcentaje` decimal(5,2) NOT NULL,
                    `Motivo` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
                    `FechaInicio` datetime(6) NOT NULL,
                    `FechaFin` datetime(6) NULL,
                    `Activo` tinyint(1) NOT NULL,
                    CONSTRAINT `PK_descuentos_producto` PRIMARY KEY (`Id_Desc`),
                    CONSTRAINT `FK_descuentos_producto_productos_Id_Pro_Per` FOREIGN KEY (`Id_Pro_Per`) REFERENCES `productos` (`Id_Pro`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;
            ");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear tabla descuentos_producto.");
        }

        // Crear usuario admin si no existe
        try
        {
            if (!context.Usuarios.Any(u => u.Nom_Usu == "admin"))
            {
                logger.LogInformation("Creando usuario admin por defecto...");
                context.Usuarios.Add(new Core.Entities.Usuario { Nom_Usu = "admin", Contrasena_Usu = "admin" });
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear usuario admin.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error fatal en la inicialización de la base de datos.");
    }
}

app.Run();