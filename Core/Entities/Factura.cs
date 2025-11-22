using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Factura
    {
        [Key]
        public int Id_Fac { get; set; }

        [Required]
        public DateTime Fec_Fac { get; set; }

        [Required]
        public int Id_Cli_Per { get; set; }

        public Cliente Cliente { get; set; }

        public decimal? Tot_Fac_Sin_IVA { get; set; }

        public decimal? IVA_Fac { get; set; }

        public decimal? Tot_Fac_Con_IVA { get; set; }

        public ICollection<DetalleFactura>? DetallesFactura { get; set; }
    }
}
