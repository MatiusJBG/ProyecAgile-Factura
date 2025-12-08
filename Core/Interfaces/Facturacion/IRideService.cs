using Core.Entities.Facturacion;

namespace Core.Interfaces.Facturacion
{
    public interface IRideService
    {
        byte[] GenerateRidePdf(Factura factura, string claveAcceso, DateTime? fechaAutorizacion);
    }
}
