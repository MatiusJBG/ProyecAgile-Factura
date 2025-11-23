namespace Core.Entities
{
    public class Lote
    {
        public int Id_Lote { get; set; }
        public int Id_Pro_Per { get; set; }
        public DateTime Fec_Ent { get; set; }
        public DateTime Fec_Exp { get; set; }
        public int Cantidad_Recibida { get; set; }
        public int Cantidad_Disponible { get; set; }
        public decimal Precio_Unitario { get; set; }
        
        // Campo generado en la base de datos (computed column)
        public decimal Precio_Lote { get; set; }

        // Navigation properties
        public Producto Producto { get; set; } = null!;
        public ICollection<DetalleFactura> DetallesFactura { get; set; } = new List<DetalleFactura>();
        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
    }
}
