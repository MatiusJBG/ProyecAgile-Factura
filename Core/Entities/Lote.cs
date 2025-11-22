using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Lote
    {
        [Key]
        public int Id_Lote { get; set; }

        [Required]
        public int Id_Pro_Per { get; set; }

        public Producto Producto { get; set; }

        [Required]
        public DateTime Fec_Ent { get; set; }

        [Required]
        public DateTime Fec_Exp { get; set; }

        [Required]
        public int Cantidad_Recibida { get; set; }

        [Required]
        public int Cantidad_Disponible { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio_Unitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio_Lote { get; set; }

        public ICollection<DetalleFactura>? Detalles { get; set; }
    }
}
