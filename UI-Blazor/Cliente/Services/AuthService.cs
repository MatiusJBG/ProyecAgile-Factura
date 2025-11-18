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
    private bool _isAuthenticated = false;
    private string? _username;

    public bool IsAuthenticated => _isAuthenticated;
    public string? Username => _username;

    public event Action? OnAuthenticationStateChanged;

    public Task<bool> LoginAsync(string username, string password)
    {
        // Autenticación simple para desarrollo
        // En producción, esto debería validar contra un backend
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            _isAuthenticated = true;
            _username = username;
            
            // Guardar en localStorage para persistencia
            if (OperatingSystem.IsBrowser())
            {
                // En Blazor WebAssembly, usar JSInterop para localStorage
                // Por ahora, solo guardamos en memoria
            }
            
            OnAuthenticationStateChanged?.Invoke();
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }

    public Task LogoutAsync()
    {
        _isAuthenticated = false;
        _username = null;
        OnAuthenticationStateChanged?.Invoke();
        return Task.CompletedTask;
    }
}

