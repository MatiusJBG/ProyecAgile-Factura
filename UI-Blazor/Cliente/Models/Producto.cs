namespace UI_Blazor.Client.Models
{
    public class Producto
    {
        public int Id_Pro { get; set; }
        public string Tip_Pro { get; set; } = string.Empty;
        public string Nom_Pro { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        
        // Para display en UI (poblado por backend desde suma de lotes)
        public int StockTotal { get; set; } = 0;
        public int NumLotes { get; set; } = 0;
    }
}