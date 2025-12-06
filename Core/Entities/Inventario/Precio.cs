namespace Core.Entities.Inventario
{
    public class Precio
    {
        public int Id_Precio { get; set; }
        public int Id_Pro_Per { get; set; }
        public decimal Precio_Venta { get; set; }
        public DateTime Fecha_Actualizacion { get; set; }
        public string? Motivo { get; set; }

        // Navigation property
        public Producto Producto { get; set; } = null!;
    }
}

