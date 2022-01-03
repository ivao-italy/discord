namespace Ivao.It.DiscordBot.Extensions;

internal static class StringExtensions
{
    /// <summary>
    /// E' una facility attestata in ACC?
    /// </summary>
    /// <param name="facility"></param>
    /// <returns></returns>
    internal static bool IsAccIcao(this string icaoCode)
    {
        var italianAccs = new[] { "LIRR", "LIBB", "LIMM", "LIPP" };
        return italianAccs.Contains(icaoCode);
    }

    /// <summary>
    /// E' un settore ACC?
    /// </summary>
    /// <param name="facility"></param>
    /// <returns></returns>
    internal static bool IsAccFacility(this string facility) => facility.EndsWith("_CTR");

    /// <summary>
    /// E' un settore APP/DEP=
    /// </summary>
    /// <param name="facility"></param>
    /// <returns></returns>
    internal static bool IsAppFacility(this string facility) => facility.EndsWith("_APP");
}
