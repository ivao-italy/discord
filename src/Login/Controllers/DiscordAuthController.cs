using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Ivao.It.DiscordLogin.Controllers;

/// <summary>
/// Gestisce il challenge a discord. Controller senza View perchè fa solo redirects
/// </summary>
public class DiscordAuthController : Controller
{
    const string PROVIDER = "Discord";
    private readonly ILogger<DiscordAuthController> _logger;

    public DiscordAuthController(ILogger<DiscordAuthController> logger)
    {
        this._logger = logger;
    }

    [HttpPost()]
    public async Task<IActionResult> SignInAsync()
    {
        if (!await HttpContext.IsProviderSupportedAsync(PROVIDER))
        {
            _logger.LogError("OAuth2 OIDC Login provider not supported.");
            return BadRequest();
        }

        _logger.LogDebug("Challenging Discord OAuth2 OIDC Login provider...");
        return Challenge(new AuthenticationProperties { IsPersistent = true, RedirectUri = Url.ActionLink("SignedIn") }, PROVIDER);
    }

    [HttpGet]
    public IActionResult SignedInAsync()
    {
        _logger.LogInformation("User logged in with Discord: {user}", this.User.Identity!.Name);
        return RedirectToPage("/accounts");
    }
}

public static class HttpContextExt
{
    public static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(this HttpContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (from scheme in await schemes.GetAllSchemesAsync()
                where !string.IsNullOrEmpty(scheme.DisplayName)
                select scheme).ToArray();
    }

    public static async Task<bool> IsProviderSupportedAsync(this HttpContext context, string provider)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return (from scheme in await context.GetExternalProvidersAsync()
                where string.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase)
                select scheme).Any();
    }
}
