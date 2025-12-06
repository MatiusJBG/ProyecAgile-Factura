using Core.Entities.Facturacion;
using Core.Entities.Common;

namespace Core.Entities.Inventario
{
    public class Producto
    {
        public int Id_Pro { get; set; }
        public string Tip_Pro { get; set; } = string.Empty;
        public string Nom_Pro { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string? Imagen { get; set; }

        // Navigation properties
        public ICollection<Lote> Lotes { get; set; } = new List<Lote>();
        public ICollection<Precio> Precios { get; set; } = new List<Precio>();
        public ICollection<DetalleFactura> DetallesFactura { get; set; } = new List<DetalleFactura>();
        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
        public ICollection<DescuentoProducto> Descuentos { get; set; } = new List<DescuentoProducto>();
    }
}

