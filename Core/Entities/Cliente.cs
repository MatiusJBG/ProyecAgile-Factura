using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Cliente
    {
        [Key]
        public int Id_Cli { get; set; }

        [Required]
        public TipoCliente Tipo_Cliente { get; set; }

        [Required]
        public TipoDocumento Tipo_Documento { get; set; }

        [Required]
        [MaxLength(20)]
        public string Num_Documento { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; }

        [MaxLength(255)]
        public string? Apellido { get; set; }

        [MaxLength(255)]
        public string? Direccion { get; set; }

        [MaxLength(255)]
        public string? Correo { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public ICollection<Factura>? Facturas { get; set; }
    }
}
