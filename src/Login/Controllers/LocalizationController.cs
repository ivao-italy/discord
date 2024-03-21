using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Ivao.It.DiscordLogin.Controllers;

public class LocalizationController : ControllerBase
{
    [HttpGet]
    public IActionResult SwitchTo(string id)
    {
        var ci = SupportedCultures.Get().SingleOrDefault(c => id.StartsWith(c.TwoLetterISOLanguageName));

        if (ci != null)
        {
            this.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(ci)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }

        //return Redirect(Request.Headers["Referer"].ToString());
        return Redirect("/");
    }
}
