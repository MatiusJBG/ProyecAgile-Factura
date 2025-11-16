using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Entities;

public class Factura
{
    [Key]
    public int Id_Fac { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public int Id_Cli { get; set; }

    [ForeignKey("Id_Cli")]
    public Cliente Cliente { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalSinIVA { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal IVA { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalConIVA { get; set; }

    public ICollection<DetalleFactura> Detalles { get; set; }
}
