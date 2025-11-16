using System.ComponentModel.DataAnnotations;
namespace Core.Entities;

public class Cliente
{
    [Key]
    public int Id_Cli { get; set; }

    // Tipo: Persona, Empresa, Extranjero
    [Required]
    public string TipoCliente { get; set; }

    // Tipo de documento: Cedula, RUC, Pasaporte
    [Required]
    public string TipoDocumento { get; set; }

    [Required]
    [MaxLength(20)]
    public string Documento { get; set; }

    // Si es persona
    public string Nombre { get; set; }
    public string Apellido { get; set; }

    // Si es empresa
    public string NombreEmpresa { get; set; }

    public string Direccion { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }

    public ICollection<Factura> Facturas { get; set; }
}
