namespace Core.Entities
{
    public class DetalleFactura
    {
        public int Id_Det_Fac { get; set; }
        public int Id_Fac_Per { get; set; }
        public int Id_Lote_Per { get; set; }
        public int Id_Pro_Per { get; set; }
        public int Cantidad_Comprada { get; set; }
        public decimal Precio_Venta_Unit { get; set; }
        
        // Campo generado en la base de datos (computed column)
        public decimal Precio_Venta_Total { get; set; }

        // Navigation properties
        public Factura Factura { get; set; } = null!;
        public Lote Lote { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
    }
}
