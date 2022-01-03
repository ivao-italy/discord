using System.Globalization;

namespace Ivao.It.DiscordLogin;

public static class SupportedCultures
{
    public static CultureInfo[] Get() => new[]
    {
            new CultureInfo("en-US"),
            new CultureInfo("it-IT")
        };
}
