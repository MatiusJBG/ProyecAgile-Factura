namespace UI_Blazor.Client.Models;

/// <summary>
/// DTO para tabla 'lotes' - Información de lotes de productos
/// Nota: Este modelo se usa solo para display/UI, la lógica de persistencia es manejada por el backend
/// </summary>
public class Lote
{
    public int Id_Lote { get; set; }
    
    // Foreign key a productos
    public int Id_Pro_Per { get; set; }
    
    // Fecha de entrada del lote
    public DateTime Fec_Ent { get; set; } = DateTime.Today;
    
    // Fecha de expiración/vencimiento del lote
    public DateTime Fec_Exp { get; set; } = DateTime.Today.AddMonths(6);
    
    // Cantidad recibida inicialmente
    public int Cantidad_Recibida { get; set; }
    
    // Cantidad disponible actualmente
    public int Cantidad_Disponible { get; set; }
    
    // Precio unitario por producto
    public decimal Precio_Unitario { get; set; }
    
    // Precio total del lote (calculado: Cantidad_Recibida * Precio_Unitario)
    // En BD es campo generado, aquí se calcula en UI
    public decimal Precio_Lote => Cantidad_Recibida * Precio_Unitario;
}
