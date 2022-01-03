using Ivao.It.DiscordLogin.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ivao.It.DiscordLogin.Pages;

public class AccountsModel : PageModel
{
    private readonly ILoginService _ivaoService;
    private readonly ILogger<AccountsModel> _logger;

    public AccountsModel(ILogger<AccountsModel> logger, ILoginService service)
    {
        this._ivaoService = service;
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
