using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Precio
    {
        [Key]
        public int Id_Precio { get; set; }

        [Required]
        public int Id_Pro_Per { get; set; }

        public Producto Producto { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio_Venta { get; set; }

        [Required]
        public DateTime Fecha_Actualizacion { get; set; }

        [MaxLength(60)]
        public string? Motivo { get; set; }
    }
}
