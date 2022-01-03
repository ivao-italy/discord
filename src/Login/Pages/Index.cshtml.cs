using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ivao.It.DiscordLogin.Pages;

public class CallbackModel : PageModel
{
    [BindProperty]
    public bool IsPolicyIvaoAccepted { get; set; }
    [BindProperty]
    public bool IsPolicyDiscordAccepted { get; set; }

    public CallbackModel()
    {
    }

    public void OnGet()
    {
    }

    public void OnPost()
    {
        //redirect agli account solo se accetta tutte le policy
        if (IsPolicyDiscordAccepted && IsPolicyDiscordAccepted)
            this.Response.Redirect("/accounts");
    }
}
