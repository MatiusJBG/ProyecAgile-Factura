namespace Application.DTOs
{
    public class LoteDto
    {
        public int Id_Lote { get; set; }
        public int Id_Pro_Per { get; set; }
        public DateTime Fec_Ent { get; set; }
        public DateTime Fec_Exp { get; set; }
        public int Cantidad_Recibida { get; set; }
        public int Cantidad_Disponible { get; set; }
        public decimal Precio_Unitario { get; set; }
        public decimal Precio_Lote { get; set; }
    }
}
