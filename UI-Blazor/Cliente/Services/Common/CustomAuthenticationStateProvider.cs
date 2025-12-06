using Cliente.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Cliente.Services.Common;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly ILocalStorageService _localStorage;

    public CustomAuthenticationStateProvider(IAuthService authService, ILocalStorageService localStorage)
    {
        _authService = authService;
        _localStorage = localStorage;
        _authService.OnAuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var username = await _localStorage.GetItemAsync<string>("username");

        ClaimsIdentity identity;

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var claims = Application.Utils.JwtUtil.ParseClaimsFromJwt(token);
                identity = new ClaimsIdentity(claims, "jwt");
            }
            catch
            {
                // Si el token es inválido, ignorarlo
                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("username");
                identity = new ClaimsIdentity();
            }
        }
        else
        {
            identity = new ClaimsIdentity();
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    private void OnAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }


}

