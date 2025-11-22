using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Auditoria
    {
        [Key]
        public int Id_Aud { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [MaxLength(100)]
        public string Tipo_Accion { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public int? Id_Pro_Per { get; set; }
        public Producto? Producto { get; set; }

        public int? Id_Lote_Per { get; set; }
        public Lote? Lote { get; set; }

        [MaxLength(100)]
        public string? Usuario { get; set; }
    }
}
