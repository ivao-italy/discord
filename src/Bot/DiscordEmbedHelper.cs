using DSharpPlus.Entities;

namespace Ivao.It.DiscordBot;

internal class DiscordEmbedHelper
{
    internal static async Task<DiscordEmbed> GetEmbed(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, DiscordColor.DarkBlue);
    internal static async Task<DiscordEmbed> GetEmbedError(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, DiscordColor.DarkRed);
    internal static async Task<DiscordEmbed> GetEmbedWarning(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, DiscordColor.Orange);
    internal static async Task<DiscordEmbed> GetEmbedSuccess(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, DiscordColor.DarkGreen);

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
