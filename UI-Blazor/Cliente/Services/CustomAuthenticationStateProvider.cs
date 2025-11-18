using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace UI_Blazor.Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;

    public CustomAuthenticationStateProvider(IAuthService authService)
    {
        _authService = authService;
        _authService.OnAuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsIdentity identity;

        if (_authService.IsAuthenticated && !string.IsNullOrEmpty(_authService.Username))
        {
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, _authService.Username)
            }, "custom");
        }
        else
        {
            identity = new ClaimsIdentity();
        }

        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }

    private void OnAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

