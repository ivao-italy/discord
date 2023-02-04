using FluentValidation;
using Ivao.It.DiscordBot.Data.Entities;

namespace Ivao.It.DiscordBot.Validators;

public class EventValidator : AbstractValidator<Event>
{
    public EventValidator(IEnumerable<EventTaskType> types)
    {
        RuleFor(e => e.Tasks.Count)
            .GreaterThanOrEqualTo(3)
            .WithMessage("Events tasks should be 3 at least: Announcement, Social Networks and ATCs are always needed.");


    }
}
