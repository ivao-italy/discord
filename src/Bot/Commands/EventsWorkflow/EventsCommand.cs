using DSharpPlus.CommandsNext.Attributes;
using Ivao.It.DiscordBot.Services;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow;

[Group("events")]
[Description("Commands used to handle IVAO Divisional Events")]
internal partial class EventsCommands : IvaoBaseCommandModule
{
    private readonly EventsService _service;

    public EventsCommands(EventsService service)
    {
        _service = service;
    }
}
