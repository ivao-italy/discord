using DSharpPlus.Entities;

namespace Ivao.It.DiscordBot;

internal class DiscordEmbedHelper
{
    public static readonly DiscordColor Blue = DiscordColor.DarkBlue;
    public static readonly DiscordColor Red = DiscordColor.DarkRed;
    public static readonly DiscordColor Orange = DiscordColor.Orange;
    public static readonly DiscordColor Green = DiscordColor.Green;

    internal static async Task<DiscordEmbed> GetAsync(DiscordGuild guild, string title, string body)
        => (await GetEmbed(guild, title, Blue)).WithDescription(body);
    internal static async Task<DiscordEmbedBuilder> GetAsync(DiscordGuild guild, string title)
        => await GetEmbed(guild, title, Blue);

    internal static async Task<DiscordEmbed> GetErrorAsync(DiscordGuild guild, string title, string body)
        => (await GetEmbed(guild, title, Red)).WithDescription(body);
    internal static async Task<DiscordEmbedBuilder> GetErrorAsync(DiscordGuild guild, string title)
        => await GetEmbed(guild, title, Red);

    internal static async Task<DiscordEmbed> GetWarningAsync(DiscordGuild guild, string title, string body)
        => (await GetEmbed(guild, title, Orange)).WithDescription(body);
    internal static async Task<DiscordEmbedBuilder> GetWarningAsync(DiscordGuild guild, string title)
        => await GetEmbed(guild, title, Orange);

    internal static async Task<DiscordEmbed> GetSuccessAsync(DiscordGuild guild, string title, string body)
        => (await GetEmbed(guild, title,  Green)).WithDescription(body);
    internal static async Task<DiscordEmbedBuilder> GetSuccessAsync(DiscordGuild guild, string title)
        => await GetEmbed(guild, title, Green);



    private static async Task<DiscordEmbedBuilder> GetEmbed(DiscordGuild guild, string title, DiscordColor color) => new DiscordEmbedBuilder()
        .WithColor(color)
        .WithTitle(title)
        .WithTimestamp(DateTime.Now)
        .WithAuthor("IVAO Italia", @"https://www.ivao.it", guild.IconUrl ?? (await guild.GetMemberAsync(IvaoItBot.Config!.BotUserId)).AvatarUrl);

    private static async Task<DiscordEmbedBuilder> GetEmbed(DiscordGuild guild, string title, string hexColorCode) => new DiscordEmbedBuilder()
          .WithColor(new DiscordColor(hexColorCode))
          .WithTitle(title)
          .WithTimestamp(DateTime.Now)
          .WithAuthor("IVAO Italia", @"https://www.ivao.it", guild.IconUrl ?? (await guild.GetMemberAsync(IvaoItBot.Config!.BotUserId)).AvatarUrl);
}
