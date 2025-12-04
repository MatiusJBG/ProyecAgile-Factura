using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infrastructure.Services
{
    public interface ICertificadoService
    {
        Task<CertificadoDigital?> GetCertificadoActivoAsync();
        Task<List<CertificadoDigital>> GetAllCertificadosAsync();
        Task<CertificadoDigital> SubirCertificadoAsync(byte[] archivoBytes, string password, string nombre);
        Task<bool> ActivarCertificadoAsync(int idCertificado);
        Task<bool> ValidarCertificadoAsync(int idCertificado);
        Task<X509Certificate2> CargarCertificadoX509Async(int idCertificado);
        Task EliminarCertificadoAsync(int idCertificado);
    }

    public class CertificadoService : ICertificadoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _certificadosPath;

        public CertificadoService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            
            // Obtener ruta de certificados desde configuración o usar default
            _certificadosPath = configuration["FirmaElectronica:CertificadosPath"] ?? "Certificados";
            
            // Crear directorio si no existe
            if (!Directory.Exists(_certificadosPath))
            {
                Directory.CreateDirectory(_certificadosPath);
            }
        }

        public async Task<CertificadoDigital?> GetCertificadoActivoAsync()
        {
            return await _context.CertificadosDigitales
                .Where(c => c.Activo && c.Fecha_Expiracion > DateTime.UtcNow)
                .OrderByDescending(c => c.Fecha_Carga)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CertificadoDigital>> GetAllCertificadosAsync()
        {
            return await _context.CertificadosDigitales
                .OrderByDescending(c => c.Activo)
                .ThenByDescending(c => c.Fecha_Carga)
                .ToListAsync();
        }

        public async Task<CertificadoDigital> SubirCertificadoAsync(byte[] archivoBytes, string password, string nombre)
        {
            // Validar el certificado
            X509Certificate2 cert;
            try
            {
                cert = new X509Certificate2(archivoBytes, password, X509KeyStorageFlags.Exportable);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar el certificado: {ex.Message}");
            }

            // Verificar que tenga clave privada
            if (!cert.HasPrivateKey)
            {
                throw new Exception("El certificado no contiene una clave privada");
            }

            // Verificar que no esté expirado
            if (cert.NotAfter < DateTime.Now)
            {
                throw new Exception("El certificado está expirado");
            }

            // Generar nombre de archivo único
            string nombreArchivo = $"{Guid.NewGuid()}.pfx";
            string rutaCompleta = Path.Combine(_certificadosPath, nombreArchivo);

            // Guardar archivo
            await File.WriteAllBytesAsync(rutaCompleta, archivoBytes);

            // Guardar contraseña en archivo adjunto (solución temporal para persistencia)
            string rutaPassword = Path.ChangeExtension(rutaCompleta, ".pass");
            await File.WriteAllTextAsync(rutaPassword, password);

            // Hash de la contraseña (para validación futura)
            string passwordHash = HashPassword(password);

            // Crear registro en BD
            var certificado = new CertificadoDigital
            {
                Nombre = nombre,
                Ruta_Archivo = nombreArchivo,
                Password_Hash = passwordHash,
                Fecha_Emision = cert.NotBefore,
                Fecha_Expiracion = cert.NotAfter,
                Emisor = cert.Issuer,
                Serial_Number = cert.SerialNumber,
                Subject = cert.Subject,
                Activo = false, // No activar automáticamente
                Fecha_Carga = DateTime.UtcNow
            };

            _context.CertificadosDigitales.Add(certificado);
            await _context.SaveChangesAsync();

            return certificado;
        }

        public async Task<bool> ActivarCertificadoAsync(int idCertificado)
        {
            var certificado = await _context.CertificadosDigitales.FindAsync(idCertificado);
            if (certificado == null)
                return false;

            // Validar que no esté expirado
            if (certificado.Fecha_Expiracion < DateTime.UtcNow)
            {
                throw new Exception("No se puede activar un certificado expirado");
            }

            // Desactivar todos los demás certificados
            var certificadosActivos = await _context.CertificadosDigitales
                .Where(c => c.Activo)
                .ToListAsync();

            foreach (var cert in certificadosActivos)
            {
                cert.Activo = false;
            }

            // Activar el seleccionado
            certificado.Activo = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidarCertificadoAsync(int idCertificado)
        {
            var certificado = await _context.CertificadosDigitales.FindAsync(idCertificado);
            if (certificado == null)
                return false;

            // Verificar que el archivo existe
            string rutaCompleta = Path.Combine(_certificadosPath, certificado.Ruta_Archivo);
            if (!File.Exists(rutaCompleta))
                return false;

            // Verificar que no esté expirado
            if (certificado.Fecha_Expiracion < DateTime.UtcNow)
                return false;

            return true;
        }

        public async Task<X509Certificate2> CargarCertificadoX509Async(int idCertificado)
        {
            var certificado = await _context.CertificadosDigitales.FindAsync(idCertificado);
            if (certificado == null)
                throw new Exception("Certificado no encontrado");

            string rutaCompleta = Path.Combine(_certificadosPath, certificado.Ruta_Archivo);
            if (!File.Exists(rutaCompleta))
                throw new Exception("Archivo de certificado no encontrado");

            // Intentar leer la contraseña del archivo .pass
            string password = "";
            string rutaPassword = Path.ChangeExtension(rutaCompleta, ".pass");
            
            if (File.Exists(rutaPassword))
            {
                password = await File.ReadAllTextAsync(rutaPassword);
            }
            else
            {
                // Fallback a configuración si no existe archivo de contraseña
                password = _configuration["FirmaElectronica:CertPassword"] ?? "";
            }

            // Cargar el certificado con flags que permiten la exportación y uso de la clave privada
            return new X509Certificate2(rutaCompleta, password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
        }

        public async Task EliminarCertificadoAsync(int idCertificado)
        {
            var certificado = await _context.CertificadosDigitales.FindAsync(idCertificado);
            if (certificado == null)
                return;

            if (certificado.Activo)
                throw new Exception("No se puede eliminar el certificado activo");

            string rutaCompleta = Path.Combine(_certificadosPath, certificado.Ruta_Archivo);
            if (File.Exists(rutaCompleta))
                File.Delete(rutaCompleta);

            _context.CertificadosDigitales.Remove(certificado);
            await _context.SaveChangesAsync();
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
