using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class DetalleFactura
    {
        [Key]
        public int Id_Det_Fac { get; set; }

        [Required]
        public int Id_Fac_Per { get; set; }
        public Factura Factura { get; set; }

        [Required]
        public int Id_Lote_Per { get; set; }
        public Lote Lote { get; set; }

        [Required]
        public int Id_Pro_Per { get; set; }
        public Producto Producto { get; set; }

        [Required]
        public int Cantidad_Comprada { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio_Venta_Unit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio_Venta_Total { get; set; }
    }
}
