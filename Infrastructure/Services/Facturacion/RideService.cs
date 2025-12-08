#nullable disable
using Core.Entities.Facturacion;
using Core.Interfaces.Facturacion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services.Facturacion
{
    public class RideService : IRideService
    {
        public RideService()
        {
            // IMPORTANT: Configure license (Community for this use case)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateRidePdf(Factura factura, string claveAcceso, DateTime? fechaAutorizacion)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial));

                    page.Header().Element(header => ComposeHeader(header, factura));
                    page.Content().Element(content => ComposeContent(content, factura, claveAcceso, fechaAutorizacion));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container, Factura factura)
        {
            container.Row(row =>
            {
                // Left Column: Logo & Emisor Info
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("R.U.C.: 1804368858001");
                    column.Item().Text("FACTURA");
                    
                    column.Item().Text("No. " + GetFormattedSecuencial(factura));
                    
                    column.Item().PaddingTop(10).Text("PRUEBAS SERVICIO DE RENTAS INTERNAS");
                    column.Item().Text("Dir Matriz: AMBATO");
                    column.Item().Text("Contribuyente Especial Nro: ");
                    column.Item().Text("Obligado a Llevar Contabilidad: NO");
                });

                // Right Column: Authorization Block
                row.RelativeItem().Border(1).Padding(5).Column(column =>
                {
                   column.Item().Text("AUTORIZACION");
                   
                   column.Item().PaddingTop(5).Text("FECHA Y HORA DE AUTORIZACIÓN");
                   column.Item().Text(factura.Fec_Fac.ToString("dd/MM/yyyy HH:mm")); 

                   column.Item().PaddingTop(5).Text("AMBIENTE: PRUEBAS");
                   column.Item().Text("EMISIÓN: NORMAL");

                   column.Item().PaddingTop(5).Text("CLAVE DE ACCESO");
                });
            });
        }

        private void ComposeContent(IContainer container, Factura factura, string claveAcceso, DateTime? fechaAutorizacion)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // Access Key
                column.Item().PaddingBottom(10).Border(1).Padding(5).Column(c =>
                {
                    c.Item().Text("CLAVE DE ACCESO").Bold().AlignCenter();
                    c.Item().Text(claveAcceso).FontSize(9).AlignCenter();
                });

                // Client Info
                column.Item().Border(1).Padding(5).Column(c =>
                {
                    c.Item().Text($"Razón Social / Nombres y Apellidos: {factura.Cliente?.Nombre} {factura.Cliente?.Apellido}".ToUpper());
                    c.Item().Text($"Identificación: {factura.Cliente?.Num_Documento}");
                    c.Item().Text($"Fecha Emisión: {factura.Fec_Fac:dd/MM/yyyy}");
                    c.Item().Text($"Dirección: {factura.Cliente?.Direccion ?? ""}");
                });

                column.Item().PaddingVertical(10);

                // Details Table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50); 
                        columns.ConstantColumn(40); 
                        columns.RelativeColumn();   
                        columns.ConstantColumn(60); 
                        columns.ConstantColumn(40); 
                        columns.ConstantColumn(60); 
                    });

                    table.Header(header =>
                    {
                        header.Cell().Border(1).Padding(2).Text("Cod. Principal").FontSize(8).Bold();
                        header.Cell().Border(1).Padding(2).Text("Cant").FontSize(8).Bold();
                        header.Cell().Border(1).Padding(2).Text("Descripción").FontSize(8).Bold();
                        header.Cell().Border(1).Padding(2).Text("Precio Unitario").FontSize(8).Bold();
                        header.Cell().Border(1).Padding(2).Text("Descuento").FontSize(8).Bold();
                        header.Cell().Border(1).Padding(2).Text("Precio Total").FontSize(8).Bold();
                    });

                    foreach (var item in factura.Detalles)
                    {
                        table.Cell().Border(1).Padding(2).Text(item.Id_Pro_Per.ToString()).FontSize(8);
                        table.Cell().Border(1).Padding(2).Text(item.Cantidad_Comprada.ToString()).FontSize(8);
                        table.Cell().Border(1).Padding(2).Text(item.Producto?.Nom_Pro ?? "Producto " + item.Id_Pro_Per).FontSize(8);
                        table.Cell().Border(1).Padding(2).Text(((decimal)item.Precio_Venta_Unit).ToString("F2")).FontSize(8).AlignRight();
                        table.Cell().Border(1).Padding(2).Text(((decimal)item.Porcentaje_Descuento).ToString("F2")).FontSize(8).AlignRight();
                        table.Cell().Border(1).Padding(2).Text(((decimal)item.Precio_Venta_Total).ToString("F2")).FontSize(8).AlignRight();
                    }
                });

                // Totals Section
                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem(6).Column(c =>
                    {
                        c.Item().Text("Información Adicional").Bold().Underline();
                        c.Item().Text($"Email: {factura.Cliente?.Correo}");
                        c.Item().Text("Forma de Pago: Otros");
                    });

                    row.RelativeItem(4).Border(1).Column(c =>
                    {
                       decimal subtotal15 = (factura.IVA_Fac ?? 0) > 0 ? (factura.Tot_Fac_Sin_IVA ?? 0) : 0;
                       decimal subtotal0 = (factura.IVA_Fac ?? 0) == 0 ? (factura.Tot_Fac_Sin_IVA ?? 0) : 0;
                       
                       c.Item().BorderBottom(1).Padding(2).Row(r => { r.RelativeItem().Text("SUBTOTAL 15%").FontSize(8); r.ConstantItem(60).Text(subtotal15.ToString("F2")).FontSize(8).AlignRight(); });
                       c.Item().BorderBottom(1).Padding(2).Row(r => { r.RelativeItem().Text("SUBTOTAL 0%").FontSize(8); r.ConstantItem(60).Text(subtotal0.ToString("F2")).FontSize(8).AlignRight(); });
                       c.Item().BorderBottom(1).Padding(2).Row(r => { r.RelativeItem().Text("SUBTOTAL NO IVA").FontSize(8); r.ConstantItem(60).Text("0.00").FontSize(8).AlignRight(); });
                       c.Item().BorderBottom(1).Padding(2).Row(r => { r.RelativeItem().Text("SUBTOTAL SIN IMPUESTOS").FontSize(8); r.ConstantItem(60).Text((factura.Tot_Fac_Sin_IVA ?? 0).ToString("F2")).FontSize(8).AlignRight(); });
                       c.Item().BorderBottom(1).Padding(2).Row(r => { r.RelativeItem().Text("IVA 15%").FontSize(8); r.ConstantItem(60).Text((factura.IVA_Fac ?? 0).ToString("F2")).FontSize(8).AlignRight(); });
                       c.Item().BorderBottom(1).Padding(2).Row(r => { r.RelativeItem().Text("VALOR TOTAL").FontSize(8).Bold(); r.ConstantItem(60).Text((factura.Tot_Fac_Con_IVA ?? 0).ToString("F2")).FontSize(8).AlignRight().Bold(); });
                    });
                });
            });
        }

        private string GetFormattedSecuencial(Factura fac)
        {
            return $"001-001-{fac.Id_Fac.ToString().PadLeft(9, '0')}";
        }
    }
}
