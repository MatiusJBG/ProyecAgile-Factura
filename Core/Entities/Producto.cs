using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Producto
    {
        [Key]
        public int Id_Pro { get; set; }

        [Required]
        [MaxLength(30)]
        public string Tip_Pro { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nom_Pro { get; set; }

        [Required]
        [MaxLength(30)]
        public string Marca { get; set; }

        public ICollection<Lote>? Lotes { get; set; }
        public ICollection<Precio>? Precios { get; set; }
        public ICollection<DetalleFactura>? Detalles { get; set; }
    }
}
