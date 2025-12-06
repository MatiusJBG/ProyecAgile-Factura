using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Cliente.Services.Inventario
{
    public interface IImagenService
    {
        Task<string?> UploadImageAsync(Stream imageStream, string fileName);
        Task DeleteImageAsync(string imageUrl);
    }

    public class ImagenService : IImagenService
    {
        private readonly HttpClient _httpClient;

        public ImagenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> UploadImageAsync(Stream imageStream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(imageStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(streamContent, "file", fileName);

                var response = await _httpClient.PostAsync("api/imagenes/upload", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ImageUploadResponse>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return result?.Url;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return null;
            }
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            try
            {
                await _httpClient.DeleteAsync($"api/imagenes/delete?imageUrl={Uri.EscapeDataString(imageUrl)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }

        private class ImageUploadResponse
        {
            public string? Url { get; set; }
        }
    }
}
