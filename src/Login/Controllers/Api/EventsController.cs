using EmiLab.Toolkit.Mail;
using Ivao.It.DiscordBot.Data;
using Ivao.It.DiscordLogin.ApiDto;
using Ivao.It.DiscordLogin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ivao.It.DiscordLogin.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITrainingAndExamsService _service;
    private readonly ILogger<EventsController> _logger;
    private readonly IConfiguration _config;
    private readonly DiscordDbContext _db;
    private readonly IEmailSender _emailSender;

    private string ApiKey => _config.GetValue<string>("ApiKey");

    public EventsController(
        IHttpClientFactory clientFactory,
        ITrainingAndExamsService service,
        ILogger<EventsController> logger,
        IConfiguration config,
        DiscordDbContext db,
        IEmailSender emailSender)
    {
        _clientFactory = clientFactory;
        _service = service;
        _logger = logger;
        _config = config;
        _db = db;
        _emailSender = emailSender;
    }


    [HttpGet()]
    public async Task<IActionResult> Run([FromHeader(Name = "X-API-Key")] string key)
    {
        if (key != ApiKey)
        {
            _logger.LogError("Events API. Api Key not valid.");
            return Unauthorized();
        }

        var examTrainings = (await this._service.GetPlannedAsync(_db, DateOnly.FromDateTime(DateTime.Now))).ToList();
        _logger.LogInformation("Events API called. Events found to publish: {count}", examTrainings.Count);

        var items = DiscordWebHookContent.BuildTemplated(examTrainings);

        try
        {
            using var client = _clientFactory.CreateClient("EventsWebHook");
            var post = await client.PostAsJsonAsync(string.Empty, items);
            post.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Events API. Error publishing via Discord WebHook");
            await _emailSender.SendEmailAsync(new Email
            {
                Recipients = new List<string> { _config.GetValue<string>("AdminEmail") },
                Subject = "Discord Exams/Training post failed",
                Body = $"Found {examTrainings.Count} events to post. But failed to post them to the Discord WebHook.\n\n{ex}",
                IsBodyHtml = false,
            });

            throw;
        }

        return Ok();
    }
}
