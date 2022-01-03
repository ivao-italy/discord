using Ivao.It.DiscordBot.Data;
using Ivao.It.DiscordBot.Data.Entities;
using Ivao.It.DiscordLogin.ApiDto;
using Microsoft.EntityFrameworkCore;

namespace Ivao.It.DiscordLogin.Services;

/// <summary>
/// Gestisce il "challenge" del login IVAO
/// </summary>
public class IvaoLoginService : ILoginService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly DiscordDbContext _db;
    private readonly ILogger<IvaoLoginService> _logger;

    public IvaoLoginService(IHttpClientFactory clientFactory, DiscordDbContext db, ILogger<IvaoLoginService> logger)
    {
        this._clientFactory = clientFactory;
        this._db = db;
        this._logger = logger;
    }

    /// <inheritdoc/>
    public async Task<LoginResult> ChallengeAsync(string token)
    {
        using var client = _clientFactory.CreateClient("ivaologin");
        LoginResult? loginResult = LoginResult.Failed;
        try
        {
            var response = await client.GetAsync($"api.php?type=json&token={token}");
            response.EnsureSuccessStatusCode();
            loginResult = await response.Content.ReadFromJsonAsync<LoginResult>() ?? LoginResult.Failed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling IVAO user API");
            throw;
        }
        if (loginResult == LoginResult.Failed)
        {
            _logger.LogDebug("IVAO Login failed");
            return loginResult;
        }

        _logger.LogInformation("IVAO Login success for user: {user} {firstname} {lastname}", loginResult.vid, loginResult.firstname, loginResult.lastname);
        return loginResult;
    }

    /// <inheritdoc/>
    public async Task StoreDataAsync(User user)
    {
        //Utente non trovato
        if (await this._db.Users.AsNoTracking().AnyAsync(u => u.Vid == user.Vid))
        {
            _logger.LogWarning("User not found on Database: {vid} {firstname} {lastname}", user.Vid, user.FirstName, user.LastName);
            return;
        }

        try
        {
            //Creazione utente db per generare il nickname Discord e salvare il consenso al trattamento dei dati
            await this._db.Users.AddAsync(user);
            await this._db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save new user on DB: {vid} {firstname} {lastname}", user.Vid, user.FirstName, user.LastName);
            throw;
        }
    }
}
