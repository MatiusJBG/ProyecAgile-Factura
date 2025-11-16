using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Entities;

public class ActualizacionPrecio
{
    [Key]
    public int Id_Act_Pro { get; set; }

    [Required]
    public int Id_Pro { get; set; }

    [ForeignKey("Id_Pro")]
    public Producto Producto { get; set; }

    [Column(TypeName ="decimal(18,2)")]
    public decimal PrecioNuevo { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public string Motivo { get; set; }
}
