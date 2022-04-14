using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Ivao.It.DiscordBot.Extensions;
using Ivao.It.DiscordBot.Models.Events;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow;
internal partial class EventsCommands
{
    [Command("complete")]
    [Description("Declares as completed a Task related to the event.")]
    [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    [CheckCallChannel]
    public async Task Completed(
        CommandContext ctx,
        [Description("Event code (the number before # in the event post)")] int eventId,
        [Description("Task completed. Values allowed: rfe, forum, routes, banner, announcement, social, atcs")] string task,
        [Description("Task content (link to forum, rfe tool, or the image to post the event on Discord)")] string? content = null
        )
    {
        //Can I find the event?
        var evt = await _service.GetAsync(eventId);
        if (evt is null)
        {
            await ctx.RespondAsync(await DiscordEmbedHelper.GetErrorAsync(
                ctx.Guild,
                "Ops!",
                $"Sorry, I don't know anything 'bout an event with code '{eventId}'. Maybe you can tell me more about planning a new event?"));
            ctx.Client.Logger.LogWarning("Event task completed command: Event not found - {event} - {user}",
                eventId,
                ctx.Member?.Nickname ?? ctx.User.Username);
            return;
        }

        var taskValue = task.ToEventsTasks();

        //Can I find the task? Is the task already done?
        var x = evt.Tasks.SingleOrDefault(t => t.TaskTypeId == (short)taskValue);
        if (x is null)
        {
            await ctx.RespondAsync(await DiscordEmbedHelper.GetWarningAsync(
                ctx.Guild,
                "Ops!",
                $"Sorry, I think that task is not supposed to be done for this event."));
            ctx.Client.Logger.LogWarning("Event task completed command: Task not found - {event} - {taskType} - {user}",
                eventId,
                taskValue.ToString(),
                ctx.Member?.Nickname ?? ctx.User.Username);
            return;
        }
        if (x.CompletedBy.HasValue)
        {
            await ctx.RespondAsync(await DiscordEmbedHelper.GetSuccessAsync(
                ctx.Guild,
                "You're already in business, mate!",
                $"Good news! That task is already completed for this event!"));
            ctx.Client.Logger.LogWarning("Event task completed command: Task not found - {event} - {taskType} - {user}",
                eventId,
                taskValue.ToString(),
                ctx.Member?.Nickname ?? ctx.User.Username);
            return;
        }

        //Ok, I'm now ready to complete the task
        try
        {
            if (taskValue == EventsTasks.ForumTopic || taskValue == EventsTasks.Graphics || taskValue == EventsTasks.WebBookingTool)
            {
                if (!(content?.IsValidUri()).GetValueOrDefault())
                    throw new IvaoItBotBusinessException(
                        "For Forum Topic, Graphics and Web Booking Tool is mandatory to add the URL in the task content");
            }

            await this._service.CompleteTaskAsync(taskValue, eventId, ctx.User.Id, content);
            await ctx.RespondAsync(await DiscordEmbedHelper.GetSuccessAsync(
                ctx.Guild,
                "Well done, mate!",
                "Task marked as :white_check_mark:"));
        }
        catch (Exception e)
        {
            await ctx.RespondAsync(await DiscordEmbedHelper.GetErrorAsync(
                ctx.Guild,
                "Ops!",
                $"Something went wrong completing the task...{Environment.NewLine}{e.Message}"));
            ctx.Client.Logger.LogError(e, "Event task completed command: Error updating the task - {event} - {taskType} - {user}",
                eventId,
                taskValue.ToString(),
                ctx.Member?.Nickname ?? ctx.User.Username);
        }
    }
}
