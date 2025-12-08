using Microsoft.Extensions.Configuration;
using System.Security;
using System.Text;
using System.Xml;

namespace Infrastructure.Services.Sri
{
    public class SriAutorizacionClient
    {
        private readonly HttpClient _http;
        private readonly string _endpointUrl;

        public SriAutorizacionClient(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            _endpointUrl = configuration["Sri:AutorizacionUrl"] 
                           ?? "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl";
        }

        public async Task<SriAutorizacionResult> AutorizarAsync(string claveAcceso)
        {
            var soap = $@"
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.autorizacion"">
   <soapenv:Header/>
   <soapenv:Body>
      <ec:autorizacionComprobante>
         <claveAccesoComprobante>{SecurityElement.Escape(claveAcceso)}</claveAccesoComprobante>
      </ec:autorizacionComprobante>
   </soapenv:Body>
</soapenv:Envelope>";

            var req = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
            {
                Content = new StringContent(soap, Encoding.UTF8, "text/xml")
            };
            req.Headers.Add("SOAPAction", "");

            var resp = await _http.SendAsync(req);
            var responseString = await resp.Content.ReadAsStringAsync();

            // Log raw response ideally
            // Console.WriteLine(responseString);

            return ParseAutorizacionResponse(responseString);
        }

        private SriAutorizacionResult ParseAutorizacionResponse(string responseXml)
        {
            var result = new SriAutorizacionResult { RawXml = responseXml };
            var doc = new XmlDocument();

            try
            {
                doc.LoadXml(responseXml);
            }
            catch (XmlException ex)
            {
                result.Estado = "ERROR_COMUNICACION";
                result.Mensajes = $"La respuesta del SRI no es un XML válido. Posiblemente el servicio está caído. Error: {ex.Message}";
                return result;
            }

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("sri", "http://ec.gob.sri.ws.autorizacion");

            var autorizacionNode = doc.SelectSingleNode("//sri:autorizacion", nsmgr) ?? doc.SelectSingleNode("//autorizacion", nsmgr);

            if (autorizacionNode != null)
            {
                result.Estado = autorizacionNode["estado"]?.InnerText ?? string.Empty;
                result.NumeroAutorizacion = autorizacionNode["numeroAutorizacion"]?.InnerText ?? string.Empty;
                result.FechaAutorizacion = autorizacionNode["fechaAutorizacion"]?.InnerText ?? string.Empty;
                result.XmlAutorizado = autorizacionNode["comprobante"]?.InnerText ?? string.Empty;

                var mensajesNode = autorizacionNode.SelectSingleNode("mensajes");
                if (mensajesNode != null)
                {
                    var sb = new StringBuilder();
                    var msgNodes = mensajesNode.SelectNodes("mensaje");
                    if (msgNodes != null)
                    {
                        foreach (XmlNode msgNode in msgNodes)
                    {
                        string id = msgNode["identificador"]?.InnerText ?? "";
                        string ms = msgNode["mensaje"]?.InnerText ?? "";
                        string info = msgNode["informacionAdicional"]?.InnerText ?? "";
                        sb.AppendLine($"[{id}] {ms} ({info})".Trim());
                    }
                    }
                    result.Mensajes = sb.ToString();
                }
            }
            else
            {
                result.Estado = doc.SelectSingleNode("//estado", nsmgr)?.InnerText ?? "NO_PROCESADA";
                if (result.Estado == "") result.Estado = "NO_ENCONTRADA_EN_RESPUESTA";
            }

            return result;
        }
    }

    public class SriAutorizacionResult
    {
        public string Estado { get; set; } = string.Empty;
        public string NumeroAutorizacion { get; set; } = string.Empty;
        public string FechaAutorizacion { get; set; } = string.Empty;
        public string XmlAutorizado { get; set; } = string.Empty;
        public string Mensajes { get; set; } = string.Empty;
        public string RawXml { get; set; } = string.Empty;
    }
}
