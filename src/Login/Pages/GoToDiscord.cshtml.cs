using AspNet.Security.OAuth.Discord;
using Ivao.It.DiscordBot.Data.Entities;
using Ivao.It.DiscordLogin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Ivao.It.DiscordLogin.Pages;

[Authorize(AuthenticationSchemes = DiscordAuthenticationDefaults.AuthenticationScheme)] //Obbligatoria l'auth discord
public class GoToDiscordModel : PageModel
{
    private readonly ILoginService _ivaoService;
    private readonly ILogger<GoToDiscordModel> _logger;

    public User? IvaoUser { get; set; }

    public GoToDiscordModel(ILoginService service, ILogger<GoToDiscordModel> logger)
    {
        this._ivaoService = service;
        this._logger = logger;
    }

    public async Task OnGetAsync()
    {
        if (!this.HttpContext.Request.Query.ContainsKey("IVAOTOKEN"))
        {
            _logger.LogWarning("Calling page 'GoToDiscord' without expected querystring 'IVAOTOKEN' from IP {ip}", this.HttpContext.Connection.RemoteIpAddress);
            this.RedirectToPage("LoginError");
        }

        var token = this.HttpContext.Request.Query["IVAOTOKEN"];
        if (token == "error")
        {
            this.RedirectToPage("LoginError");
        }

        this.IvaoUser = (User)await _ivaoService.ChallengeAsync(token);
        var userIdClaim = this.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
        this.IvaoUser.DiscordUserId = ulong.Parse(userIdClaim);
        await _ivaoService.StoreDataAsync(this.IvaoUser);
    }
}
