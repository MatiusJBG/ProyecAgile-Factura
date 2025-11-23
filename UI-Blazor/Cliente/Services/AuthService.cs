using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace UI_Blazor.Client.Services;

public interface IAuthService
{
    bool IsAuthenticated { get; }
    string? Username { get; }
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
    event Action? OnAuthenticationStateChanged;
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private bool _isAuthenticated = false;
    private string? _username;

    public bool IsAuthenticated => _isAuthenticated;
    public string? Username => _username;

    public event Action? OnAuthenticationStateChanged;

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var loginRequest = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                
                if (loginResponse != null)
                {
                    await _localStorage.SetItemAsync("authToken", loginResponse.Token);
                    await _localStorage.SetItemAsync("username", loginResponse.Username);
                    
                    _isAuthenticated = true;
                    _username = loginResponse.Username;
                    
                    OnAuthenticationStateChanged?.Invoke();
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en login: {ex.Message}");
        }
        
        return false;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("username");
        
        _isAuthenticated = false;
        _username = null;
        
        OnAuthenticationStateChanged?.Invoke();
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

