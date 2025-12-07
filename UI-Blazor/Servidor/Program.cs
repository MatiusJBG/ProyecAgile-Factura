using Application.Services.Clientes;
using Application.Services.Facturacion;
using Application.Services.Inventario;
using Application.Services.Auth;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Infrastructure.Data;
using Infrastructure.Repositories.Clientes; using Infrastructure.Repositories.Facturacion; using Infrastructure.Repositories.Inventario; using Infrastructure.Repositories.Auth; using Infrastructure.Repositories.Certificados; using Infrastructure.Repositories.Common;
using Infrastructure.Services.Facturacion; using Infrastructure.Services.Certificados; using Infrastructure.Services; using Infrastructure.Services.Sri;
using Application.Interfaces;
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
builder.Services.AddScoped<IDescuentoRepository, DescuentoRepository>();
builder.Services.AddScoped<IFirmaElectronicaRepository, FirmaElectronicaRepository>();
builder.Services.AddScoped<ICertificadoRepository, CertificadoRepository>();

// Registrar servicios de aplicación
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<LoteService>();
builder.Services.AddScoped<FacturaService>();
builder.Services.AddScoped<DescuentoService>();
builder.Services.AddScoped<AuthService>();

// Registrar servicios de firma electrónica (de Infrastructure)
builder.Services.AddScoped<IFirmaElectronicaService, FirmaElectronicaService>();
builder.Services.AddScoped<IFirmaElectronicaService, FirmaElectronicaService>();
builder.Services.AddScoped<ICertificadoService, CertificadoService>();

// Registrar servicios SRI
builder.Services.AddScoped<FacturaXmlService>();
builder.Services.AddHttpClient<SriRecepcionClient>();
builder.Services.AddHttpClient<SriAutorizacionClient>();
builder.Services.AddScoped<IRideService, RideService>();

// Registrar sistema de caché en archivos
builder.Services.AddSingleton<IFileCacheService, FileCacheService>();
builder.Services.AddHostedService<FileCacheWorker>();

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

// Middleware de logging de errores global - AL INICIO
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "CRITICAL: Error no controlado en {Path}: {Message}", context.Request.Path, ex.Message);
        // Devolver 500 JSON amigable si es API
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error", details = ex.Message });
        }
        else 
        {
             throw; 
        }
    }
});

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

        // Parche: Agregar columna Porcentaje_Descuento si falta
        try
        {
            logger.LogInformation("Intentando agregar columna Porcentaje_Descuento...");
            context.Database.ExecuteSqlRaw("ALTER TABLE detallesfactura ADD COLUMN Porcentaje_Descuento decimal(5,2) NULL DEFAULT 0;");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error al agregar columna Porcentaje_Descuento (probablemente ya existe).");
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

        // Crear tabla CertificadosDigitales manualmente si no existe
        try
        {
            logger.LogInformation("Intentando crear tabla CertificadosDigitales...");
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS `CertificadosDigitales` (
                    `Id_Cert` int NOT NULL AUTO_INCREMENT,
                    `Nombre` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Ruta_Archivo` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Password_Hash` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Fecha_Emision` datetime(6) NOT NULL,
                    `Fecha_Expiracion` datetime(6) NOT NULL,
                    `Emisor` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Serial_Number` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Subject` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Activo` tinyint(1) NOT NULL,
                    `Fecha_Carga` datetime(6) NOT NULL,
                    `Observaciones` longtext CHARACTER SET utf8mb4 NULL,
                    CONSTRAINT `PK_CertificadosDigitales` PRIMARY KEY (`Id_Cert`)
                ) CHARACTER SET=utf8mb4;
            ");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear tabla CertificadosDigitales.");
        }

        // Crear tabla FirmasElectronicas manualmente si no existe
        try
        {
            logger.LogInformation("Intentando crear tabla FirmasElectronicas...");
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS `FirmasElectronicas` (
                    `Id_Firma` int NOT NULL AUTO_INCREMENT,
                    `Id_Fac_Per` int NOT NULL,
                    `Firma_Digital` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Algoritmo` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Certificado_Serial` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Fecha_Firma` datetime(6) NOT NULL,
                    `Hash_Documento` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Estado_Validacion` longtext CHARACTER SET utf8mb4 NOT NULL,
                    `Observaciones` longtext CHARACTER SET utf8mb4 NULL,
                    CONSTRAINT `PK_FirmasElectronicas` PRIMARY KEY (`Id_Firma`),
                    CONSTRAINT `FK_FirmasElectronicas_Facturas_Id_Fac_Per` FOREIGN KEY (`Id_Fac_Per`) REFERENCES `facturas` (`Id_Fac`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;
            ");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear tabla FirmasElectronicas.");
        }

        // Crear usuario admin si no existe
        try
        {
            if (!context.Usuarios.Any(u => u.Nom_Usu == "admin"))
            {
                logger.LogInformation("Creando usuario admin por defecto...");
                context.Usuarios.Add(new Core.Entities.Auth.Usuario { Nom_Usu = "admin", Contrasena_Usu = "admin" });
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear usuario admin.");
        }

        // Parche: Agregar columna Activo a Clientes si falta
        try
        {
            logger.LogInformation("Intentando agregar columna Activo a Clientes...");
            context.Database.ExecuteSqlRaw("ALTER TABLE clientes ADD COLUMN Activo tinyint(1) NOT NULL DEFAULT 1;");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error al agregar columna Activo a Clientes (probablemente ya existe).");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error fatal en la inicialización de la base de datos.");
    }
}

app.Run();
