using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Entities;

public class Producto
{
    [Key]
    public int Id_Pro { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nom_Pro { get; set; }

    [Required]
    [MaxLength(150)]
    public string Marca { get; set; }

    [Required]
    [MaxLength(100)]
    public string Tip_Pro { get; set; }

    // Precio sugerido de venta actual
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioVenta { get; set; }

    // Relación con lotes
    public ICollection<Lote> Lotes { get; set; }
}
