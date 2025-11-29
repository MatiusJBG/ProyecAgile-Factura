using System;

namespace UI_Blazor.Client.Models
{
    public class DescuentoProducto
    {
        public int Id_Desc { get; set; }
        public int Id_Pro_Per { get; set; }
        public decimal Porcentaje { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        public DateTime? FechaFin { get; set; }
        public bool Activo { get; set; } = true;
    }
}
