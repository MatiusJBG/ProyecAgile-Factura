using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class DescuentoProducto
    {
        [Key]
        public int Id_Desc { get; set; }

        public int Id_Pro_Per { get; set; }
        
        [ForeignKey("Id_Pro_Per")]
        public Producto? Producto { get; set; }

        [Required]
        public decimal Porcentaje { get; set; }

        [Required]
        [StringLength(255)]
        public string Motivo { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; } = DateTime.Today;
        
        public DateTime? FechaFin { get; set; }

        public bool Activo { get; set; } = true;
    }
}
