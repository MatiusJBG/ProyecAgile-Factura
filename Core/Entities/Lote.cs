using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Entities;

public class Lote
{
    [Key]
    public int Id_Lote { get; set; }

    [Required]
    public int Id_Pro { get; set; }

    [ForeignKey("Id_Pro")]
    public Producto Producto { get; set; }

    [Required]
    public DateTime FechaEntrada { get; set; }

    [Required]
    public int CantidadEntrada { get; set; }

    [Required]
    public int CantidadDisponible { get; set; }

    // Precio al que te lo dejan (compra)
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioUnidadCompra { get; set; }

    // Precio total del lote (Cantidad * PrecioUnidadCompra)
    [Column(TypeName ="decimal(18,2)")]
    public decimal PrecioTotalLote { get; set; }

    // Fecha de expiración del lote
    [Required]
    public DateTime FechaExpiracion { get; set; }
}
