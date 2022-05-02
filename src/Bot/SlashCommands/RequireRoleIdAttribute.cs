using DSharpPlus.SlashCommands;

namespace Ivao.It.DiscordBot.SlashCommands;

public class RequireRoleIdAttribute : SlashCheckBaseAttribute
{
    public ulong RoleId { get; private set; }

    public RequireRoleIdAttribute(ulong roleId)
    {
        this.RoleId = roleId;
    }

    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        var role = ctx.Member.Guild.GetRole(this.RoleId);
        if (role == null) return false;

        if (ctx.Member.Roles.Contains(role))
            return await Task.FromResult(true);
        else
            return await Task.FromResult(false);
    }
}