using AspNet.Security.OAuth.Discord;
using Ivao.It.Discord.Shared.Services;
using Ivao.It.DiscordBot.Data;
using Ivao.It.DiscordLogin;
using Ivao.It.DiscordLogin.Pages;
using Ivao.It.DiscordLogin.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages().AddMvcLocalization(LanguageViewLocationExpanderFormat.Suffix);
builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment()) builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<CallbackModel>("ivaologin", opt =>
{
    opt.BaseAddress = new Uri(builder.Configuration.GetConnectionString("IvaoLoginBaseAddress"));
});
builder.Services.AddHttpClient("EventsWebHook", opt =>
{
    opt.BaseAddress = new Uri(builder.Configuration.GetConnectionString("DiscordEventsWebHook"));
});

builder.Services.AddDbContext<DiscordDbContext>(opt =>
    opt.UseMySql(
        builder.Configuration.GetConnectionString("DiscordBot"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DiscordBot"))
    ));

builder.Services.AddLocalization(opt => opt.ResourcesPath = "Localization");

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(opt =>
{
    opt.LoginPath = "/discordauth/signedin";
})
.AddDiscord(opt =>
{
    opt.ClientId = "771681863952891944";
    opt.ClientSecret = "vBvBa3OLkydqkMalLQLHxjGmluqiqflC";
});

builder.Services.AddEmail(opt =>
{
    opt.SmtpServerAddress = "localhost";
    opt.SmtpServerPort = 25;
    opt.SenderAccount = "no-reply@ivao.it";
    opt.EnableSsl = false;
    opt.SenderDisplayName = "IVAO IT Discord UI";
});

builder.Services.AddTransient<ILoginService, IvaoLoginService>();
builder.Services.AddTransient<ITrainingAndExamsService, TrainingAndExamsService>();

var app = builder.Build();

/*************************************************************/

Log.Logger = new LoggerConfiguration()
          .ReadFrom.Configuration(app.Configuration, sectionName: "Serilog")
          .CreateLogger();

app.UseRequestLocalization(opt =>
{
    opt.DefaultRequestCulture = new RequestCulture(SupportedCultures.Get()[0].ToString());
    opt.SupportedCultures = SupportedCultures.Get();
    opt.SupportedUICultures = SupportedCultures.Get();
});

// HSTS e HTTPS disattivati: c'è apache da reverse proxy che cripta la sessione con l'utente
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
};
app.UseForwardedHeaders(forwardedHeadersOptions);
app.UseAuthorization();
app.UseAuthentication();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapRazorPages();
app.MapDefaultControllerRoute();

//troubleshoot headers https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-6.0#troubleshoot
//app.Use(async (context, next) =>
//{
//    // Request method, scheme, and path
//    Log.Logger.Debug("Request Method: {Method}", context.Request.Method);
//    Log.Logger.Debug("Request Scheme: {Scheme}", context.Request.Scheme);
//    Log.Logger.Debug("Request Path: {Path}", context.Request.Path);
//    Log.Logger.Debug("Request Host: {Host}", context.Request.Host);

//    // Headers
//    foreach (var header in context.Request.Headers)
//    {
//        Log.Logger.Warning("Header: {Key}: {Value}", header.Key, header.Value);
//    }
//    // Connection: RemoteIp
//    Log.Logger.Debug("Request RemoteIp: {RemoteIpAddress}", context.Connection.RemoteIpAddress);

//    await next();
//});

try
{
    Log.Information("IVAO IT Discord Policies WebApp Started");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "IVAO IT Discord Policies WebApp terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}