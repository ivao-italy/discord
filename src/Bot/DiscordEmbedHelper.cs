using DSharpPlus.Entities;

namespace Ivao.It.DiscordBot;

internal class DiscordEmbedHelper
{
    internal static async Task<DiscordEmbed> GetEmbed(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, "#2a4982");
    internal static async Task<DiscordEmbed> GetEmbedError(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, "#cc0000");
    internal static async Task<DiscordEmbed> GetEmbedWarning(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, "#ebb134");
    internal static async Task<DiscordEmbed> GetEmbedSuccess(DiscordGuild guild, string title, string body)
        => await GetEmbed(guild, title, body, "#217d15");

    private static async Task<DiscordEmbed> GetEmbed(DiscordGuild guild, string title, string body, string hexColorCode) => new DiscordEmbedBuilder()
          .WithColor(new DiscordColor(hexColorCode))
          .WithTitle(title)
          .WithTimestamp(DateTime.Now)
          .WithDescription(body)
          .WithAuthor("IVAO Italia", @"https://www.ivao.it", guild.IconUrl ?? (await guild.GetMemberAsync(IvaoItBot.Config!.BotUserId)).AvatarUrl);
}
