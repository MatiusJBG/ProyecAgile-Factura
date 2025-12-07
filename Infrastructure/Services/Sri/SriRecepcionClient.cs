using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;

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

            // Aseguramos TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<string> EnviarComprobanteAsync(byte[] xmlFirmado)
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

            // request.Headers.Add("SOAPAction", ""); // Opcional según WSDL

            HttpResponseMessage response = null;
            string responseBody = null;

            try
            {
                response = await _http.SendAsync(request).ConfigureAwait(false);
                responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                
                response.EnsureSuccessStatusCode();

                return responseBody;
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
    }
}
