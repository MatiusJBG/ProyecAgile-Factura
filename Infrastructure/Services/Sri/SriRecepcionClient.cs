using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.Xml;

namespace Infrastructure.Services.Sri
{
    public class SriRecepcionClient
    {
        private readonly HttpClient _http;
        private readonly string _endpointUrl;

        public SriRecepcionClient(HttpClient http, IConfiguration configuration)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            // URL por defecto del ambiente de PRUEBAS
            _endpointUrl = configuration["Sri:RecepcionUrl"] 
                           ?? "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";


        }

        public async Task<SriRecepcionResult> EnviarComprobanteAsync(byte[] xmlFirmado)
        {
            if (xmlFirmado == null || xmlFirmado.Length == 0)
                throw new ArgumentException("xmlFirmado no puede ser null/empty.", nameof(xmlFirmado));

            var xmlBase64 = Convert.ToBase64String(xmlFirmado);

            var soapEnvelope = $@"
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.recepcion"">
   <soapenv:Header/>
   <soapenv:Body>
      <ec:validarComprobante>
         <xml>{xmlBase64}</xml>
      </ec:validarComprobante>
   </soapenv:Body>
</soapenv:Envelope>";

            using var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
            };

            HttpResponseMessage response = default!;
            string responseBody = string.Empty;

            try
            {
                response = await _http.SendAsync(request).ConfigureAwait(false);
                responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                
                response.EnsureSuccessStatusCode();

                return ParseRecepcionResponse(responseBody ?? string.Empty);
            }
            catch (HttpRequestException ex)
            {
                var status = response?.StatusCode.ToString() ?? "NoResponse";
                var msg = new StringBuilder();
                msg.AppendLine($"Error HTTP al llamar al SRI. Status: {status}.");
                msg.AppendLine($"Mensaje: {ex.Message}");
                if (!string.IsNullOrEmpty(responseBody))
                {
                    msg.AppendLine("Respuesta del servicio:");
                    msg.AppendLine(responseBody);
                }
                throw new HttpRequestException(msg.ToString(), ex);
            }
        }

        private SriRecepcionResult ParseRecepcionResponse(string responseXml)
        {
            var result = new SriRecepcionResult();
            var doc = new XmlDocument();

            try
            {
                doc.LoadXml(responseXml);
            }
            catch (XmlException ex)
            {
                result.Estado = "ERROR_COMUNICACION";
                result.Mensajes = $"RESPUESTA INVALIDA: {ex.Message}";
                return result;
            }

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");

            // Usamos local-name() para evitar problemas con namespaces variables del SRI
            
            // Buscar estado
            var estadoNode = doc.SelectSingleNode("//*[local-name()='estado']");
            result.Estado = estadoNode?.InnerText ?? "DESCONOCIDO";

            // Buscar clave de acceso
            var claveNode = doc.SelectSingleNode("//*[local-name()='claveAcceso']");
            result.ClaveAcceso = claveNode?.InnerText ?? string.Empty;

            // Mensajes
            var mensajesNodes = doc.SelectNodes("//*[local-name()='mensaje']");
            if (mensajesNodes != null && mensajesNodes.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (XmlNode msgNode in mensajesNodes)
                {
                    var id = msgNode["identificador"]?.InnerText // A veces es hijo directo
                             ?? msgNode.SelectSingleNode("*[local-name()='identificador']")?.InnerText; 
                             
                    var ms = msgNode["mensaje"]?.InnerText 
                             ?? msgNode.SelectSingleNode("*[local-name()='mensaje']")?.InnerText;
                             
                    var info = msgNode["informacionAdicional"]?.InnerText
                               ?? msgNode.SelectSingleNode("*[local-name()='informacionAdicional']")?.InnerText;
                               
                    var tipo = msgNode["tipo"]?.InnerText
                               ?? msgNode.SelectSingleNode("*[local-name()='tipo']")?.InnerText;

                    if (!string.IsNullOrEmpty(ms))
                    {
                        sb.AppendLine($"[{tipo}] {ms} {info}".Trim());
                    }
                }
                result.Mensajes = sb.ToString();
            }

            return result;
        }
    }

    public class SriRecepcionResult
    {
        public string Estado { get; set; } = string.Empty;
        public string ClaveAcceso { get; set; } = string.Empty;
        public string Mensajes { get; set; } = string.Empty;
    }
}
