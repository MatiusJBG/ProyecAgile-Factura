using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Entities;

public class DetalleFactura
{
    [Key]
    public int Id_Det_Fac { get; set; }

    [Required]
    public int Id_Fac { get; set; }

    [ForeignKey("Id_Fac")]
    public Factura Factura { get; set; }

    [Required]
    public int Id_Pro { get; set; }

    [ForeignKey("Id_Pro")]
    public Producto Producto { get; set; }

    [Required]
    public int Cantidad { get; set; }

    // Lote que se usó en FIFO
    public int? Id_Lote { get; set; }

    [ForeignKey("Id_Lote")]
    public Lote Lote { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioUnitario { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }
}
