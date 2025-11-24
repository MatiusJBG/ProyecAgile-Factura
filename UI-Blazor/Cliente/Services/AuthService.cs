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
    Task<DateTime?> GetLockoutEndTimeAsync();
    event Action? OnAuthenticationStateChanged;
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private bool _isAuthenticated = false;
    private string? _username;
    private const string FAILED_ATTEMPTS_KEY = "failedLoginAttempts";
    private const string LOCKOUT_END_KEY = "lockoutEndTime";

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
            // Check lockout
            var lockoutEnd = await GetLockoutEndTimeAsync();
            if (lockoutEnd.HasValue && lockoutEnd.Value > DateTime.Now)
            {
                return false;
            }

            // If lock expired, clear it (optional, but keeps state clean)
            if (lockoutEnd.HasValue && lockoutEnd.Value <= DateTime.Now)
            {
                await _localStorage.RemoveItemAsync(LOCKOUT_END_KEY);
            }

            var loginRequest = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                
                if (loginResponse != null)
                {
                    await _localStorage.SetItemAsync("authToken", loginResponse.Token);
                    await _localStorage.SetItemAsync("username", loginResponse.Username);
                    
                    // Reset failed attempts on success
                    await _localStorage.RemoveItemAsync(FAILED_ATTEMPTS_KEY);
                    await _localStorage.RemoveItemAsync(LOCKOUT_END_KEY);

                    _isAuthenticated = true;
                    _username = loginResponse.Username;
                    
                    OnAuthenticationStateChanged?.Invoke();
                    return true;
                }
            }
            else
            {
                // Handle failed attempt
                await HandleFailedAttemptAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en login: {ex.Message}");
            // Treat exception as failure too? Maybe not network error, but let's be safe or just log.
            // For now, only count explicit failures or let the caller handle exceptions.
            // If we want to be strict, we could count this, but usually we don't count network errors as bad passwords.
        }
        
        return false;
    }

    private async Task HandleFailedAttemptAsync()
    {
        int attempts = await _localStorage.GetItemAsync<int>(FAILED_ATTEMPTS_KEY);
        attempts++;
        await _localStorage.SetItemAsync(FAILED_ATTEMPTS_KEY, attempts);

        if (attempts == 3)
        {
            // Lock for 30 seconds
            var lockoutTime = DateTime.Now.AddSeconds(30);
            await _localStorage.SetItemAsync(LOCKOUT_END_KEY, lockoutTime);
        }
        else if (attempts >= 6)
        {
            // Lock for 2 minutes
            // If they failed 3 times (locked 30s), then came back and failed 3 MORE times (total 6), lock 2m.
            // If they continue failing (7, 8...), keep locking or extend? 
            // Requirement: "si luego de esos se falla 3 veces mas el login se bloquee 2 minutos"
            // Let's lock for 2 minutes at 6, 9, 12...
            if (attempts % 3 == 0)
            {
                 var lockoutTime = DateTime.Now.AddMinutes(2);
                 await _localStorage.SetItemAsync(LOCKOUT_END_KEY, lockoutTime);
            }
        }
    }

    public async Task<DateTime?> GetLockoutEndTimeAsync()
    {
        return await _localStorage.GetItemAsync<DateTime?>(LOCKOUT_END_KEY);
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

