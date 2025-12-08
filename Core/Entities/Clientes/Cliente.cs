using Core.Enums.Clientes;
using Core.Entities.Facturacion;

namespace Core.Entities.Clientes
{    public class Cliente
    {
        public int Id_Cli { get; set; }
        public TipoCliente Tipo_Cliente { get; set; }
        public TipoDocumento Tipo_Documento { get; set; }
        public string Num_Documento { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Apellido { get; set; }
        public string? Direccion { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El correo es obligatorio.")]
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public bool Activo { get; set; } = true;

        // Navigation properties
        public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
    }
}

