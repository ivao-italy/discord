using DSharpPlus.Entities;

namespace Ivao.It.DiscordBot;

internal class DiscordEmbedHelper
{
    public static readonly DiscordColor Blue = DiscordColor.DarkBlue;
    public static readonly DiscordColor Red = DiscordColor.DarkRed;
    public static readonly DiscordColor Orange = DiscordColor.Orange;
    public static readonly DiscordColor Green = DiscordColor.Green;

    internal static async Task<DiscordEmbed> GetEmbed(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, Blue);
    internal static async Task<DiscordEmbed> GetEmbedError(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, Red);
    internal static async Task<DiscordEmbed> GetEmbedWarning(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, Orange);
    internal static async Task<DiscordEmbed> GetEmbedSuccess(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, Green);

    private static async Task<DiscordEmbed> GetEmbed(DiscordGuild guild, string title, string body, DiscordColor color) => new DiscordEmbedBuilder()
        .WithColor(color)
        .WithTitle(title)
        .WithTimestamp(DateTime.Now)
        .WithDescription(body)
        .WithAuthor("IVAO Italia", @"https://www.ivao.it", guild.IconUrl ?? (await guild.GetMemberAsync(IvaoItBot.Config!.BotUserId)).AvatarUrl);

    private static async Task<DiscordEmbed> GetEmbed(DiscordGuild guild, string title, string body, string hexColorCode) => new DiscordEmbedBuilder()
          .WithColor(new DiscordColor(hexColorCode))
          .WithTitle(title)
          .WithTimestamp(DateTime.Now)
          .WithDescription(body)
          .WithAuthor("IVAO Italia", @"https://www.ivao.it", guild.IconUrl ?? (await guild.GetMemberAsync(IvaoItBot.Config!.BotUserId)).AvatarUrl);
}
