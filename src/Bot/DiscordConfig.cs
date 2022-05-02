namespace Ivao.It.DiscordBot;

public class DiscordConfig
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string DiscordToken { get; set; }
    public ulong WelcomeChannelId { get; set; }
    public ulong AnnouncementsChannelId { get; set; }
    public ulong ActivationChannelId { get; set; }
    public ulong BotControlChannelId { get; set; }
    public ulong BotUserId { get; set; }
    public ulong VerifiedUsersRoleId { get; set; }
    public ulong EventsStaffChannelId { get; set; }
    public string DiscordAccessWizardHomepage { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}
