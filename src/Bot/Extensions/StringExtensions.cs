namespace Ivao.It.DiscordBot.Extensions;

internal static class StringExtensions
{
    /// <summary>
    /// Is an ACC ICAO code?
    /// </summary>
    /// <param name="facility"></param>
    /// <returns></returns>
    internal static bool IsAccIcao(this string icaoCode)
    {
        var italianAccs = new[] { "LIRR", "LIBB", "LIMM", "LIPP" };
        return italianAccs.Contains(icaoCode);
    }

    /// <summary>
    /// Is a valid ACC sector?
    /// </summary>
    /// <param name="facility"></param>
    /// <returns></returns>
    internal static bool IsAccFacility(this string facility) => facility.EndsWith("_CTR");

    /// <summary>
    /// Is a valid APP/DEP ATC sector?
    /// </summary>
    /// <param name="facility"></param>
    /// <returns></returns>
    internal static bool IsAppFacility(this string facility) => facility.EndsWith("_APP");

    /// <summary>
    /// Is the string in a URI format?
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    internal static bool IsValidUri(this string uri) => Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out _);
}
