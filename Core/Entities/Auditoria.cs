namespace Core.Entities
{
    public class Auditoria
    {
        public int Id_Aud { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo_Accion { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int? Id_Pro_Per { get; set; }
        public int? Id_Lote_Per { get; set; }
        public string? Usuario { get; set; }

        // Navigation properties
        public Producto? Producto { get; set; }
        public Lote? Lote { get; set; }
    }
}
