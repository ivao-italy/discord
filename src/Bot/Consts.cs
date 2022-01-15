using System.Text.RegularExpressions;

namespace Ivao.It.DiscordBot;

internal class Consts
{
    internal static Regex FacilityRegex =
        new(@"([A-Z]{4})_([A-Z0-9]{0,3})(_){0,1}([A-Z]{3})", RegexOptions.Compiled);

    internal const string ExamTitle = @"Esame!";
    internal const string TrainingTitle = @"Training!";
}
