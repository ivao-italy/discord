using System.Text.RegularExpressions;

namespace Ivao.It.DiscordBot;

internal class Consts
{
    public static readonly Regex NicknamePattern = new Regex(@"^\d{6} [\w\s']*$");
}
