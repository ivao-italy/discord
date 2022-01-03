using Ivao.It.DiscordBot.Data.Entities;
using Ivao.It.DiscordLogin.ApiDto;

namespace Ivao.It.DiscordLogin.Services;

/// <summary>
/// Implementazione di un login non gestibile con OAuth2/OIDC
/// </summary>
public interface ILoginService
{
    /// <summary>
    /// Login IVAO
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<LoginResult> ChallengeAsync(string token);

    /// <summary>
    /// Salva i dati dell'utente
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task StoreDataAsync(User user);
}
