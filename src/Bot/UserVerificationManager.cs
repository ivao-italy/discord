﻿using DSharpPlus;
using DSharpPlus.Entities;
using Ivao.It.DiscordBot.Data.Entities;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot;

internal static class UserVerificationManager
{
    public static async Task<bool> ForceUserVerificationAsync(DiscordClient client, DiscordMember member, DiscordGuild guild, User user)
    {
        //Il bot sta scrivendo
        var verificationChannel = guild.GetChannel(IvaoItBot.Config!.ActivationChannelId);
        await verificationChannel.TriggerTypingAsync();

        //Controllo che esista il ruolo configurato come "VerifiedUsersRoleId"
        if (!guild.Roles.ContainsKey(IvaoItBot.Config.VerifiedUsersRoleId))
        {
            client.Logger.LogError($"Configuration invalid: the Verified Users Role ID configured was not found on the server");
            await verificationChannel.SendMessageAsync(
                await DiscordEmbedHelper.GetErrorAsync(
                    guild,
                    "Hey! Ho un problema!",
                    "Non riesco a validare la tua registrazione. Puoi chiedere perchè a it-hq@ivao.aero."
            ));
            await verificationChannel.TriggerTypingAsync();
            return false;
        }

        //Nickname
        try
        {
            await member.ModifyAsync(edit =>
            {
                edit.Nickname = user.ToString();
            });
        }
        catch (Exception ex)
        {
            client.Logger.LogError(ex, "Discord API Error trying to update user's nickname. Nickname: {nickname}", user.ToString());
            await verificationChannel.SendMessageAsync(
                await DiscordEmbedHelper.GetErrorAsync(
                    guild,
                    "Hey! Ho un problema!",
                    "Non riesco ad impostare il tuo nickname! Puoi chiedere perchè a it-hq@ivao.aero"
            ));
            await verificationChannel.TriggerTypingAsync();
            return false;
        }


        //Ruolo
        try
        {
            DiscordRole verifiedUsersRole = guild.Roles[IvaoItBot.Config.VerifiedUsersRoleId];
            await member.GrantRoleAsync(verifiedUsersRole, "IVAO IT Bot - Nickname Correct!");
            client.Logger.LogInformation("User verified: {nickname}", member.Nickname);
        }
        catch (Exception ex)
        {
            client.Logger.LogError(ex, "Discord API Error trying to grant approved role to user. Nickname: {nickname}", user.ToString());
            await verificationChannel.SendMessageAsync(
                await DiscordEmbedHelper.GetErrorAsync(
                    guild,
                    "Hey! Ho un problema!",
                    "Non riesco ad assegnarti il ruolo 'Utente verificato'! Puoi chiedere perchè a it-hq@ivao.aero"
            ));
            await verificationChannel.TriggerTypingAsync();
            return false;
        }

        await member.SendMessageAsync(await DiscordEmbedHelper.GetErrorAsync(
                    guild,
                    $"Welcome aboard, Captain {user.FirstName}",
                    $"Mi raccomando, torna a leggere le [regole del server](https://discord.com/channels/{guild.Id}/{IvaoItBot.Config.WelcomeChannelId}). Sono importanti!"
            ));

        await verificationChannel.TriggerTypingAsync();
        return true;
    }
}
