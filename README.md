# IVAO IT Discord Automation
Project made with ❤️ by IVAO Italy division.


![GitHub milestone](https://img.shields.io/github/milestones/progress-percent/ivao-italy/discord/1) ![Website](https://img.shields.io/website?down_color=red&down_message=down&up_color=brightgreen&up_message=up&url=https%3A%2F%2Fdiscord.ivao.it) ![Discord](https://img.shields.io/discord/426318927220441089)


Automations introduced by the Bot:
1.  Automation of the policy acceptance flow linking IVAO Account to the Discord Account
2. Storing the policy acceptance date/time
3. Automatic posting of Exam/Training events on the Discord server with events crossposting
4. The bot allows to post rules and regulation in a structured way
5. Automatically starts events scheduled on the Guild
6. Automatically deletes exams and trainigs if canceled after publication on Discord
7. Automatically deletes old training/exams posts to avoid confusion in the announcement channel

#### Discord Permission Requirements
1. `CHANGE_NICKNAME` `MANAGE_NICKNAMES` `VIEW_CHANNEL` `READ_MESSAGE_HISTORY`
3. [Events management permissions](https://discord.com/developers/docs/resources/guild-scheduled-event#permissions-to-create-an-event-with-entitytype-stageinstance) + [Read and Create Messages](https://discord.com/developers/docs/resources/guild-scheduled-event#permissions-to-create-an-event-with-entitytype-stageinstance) + [Message Crossposting](https://discord.com/developers/docs/resources/channel#crosspost-message)
4. `VIEW_CHANNEL` `READ_MESSAGE_HISTORY` `EMBED_LINKS` `ATTACH_FILES` `SEND_MESSAGES`

### Events/Training auto post specifications
Remember that the Bot is operating "by convention" when posting an Exam/Training on the server.
Bot, basically, is looking to the facility of the event and determines which channel to select as the event location.
If the bot is unable to find a suitable voice channel, sets the event as "External Event", in fact not highlighting any of the voice channel set on the server.

How the vocal channel is selected? Bot applies the following logic:
* Event facility is a **standard airport ICAO?** (eg. LIPZ) If yes, it looks for the first channel with a name beginning with that facility ICAO code.
* Event facility is an **ACC ICAO code?** (eg. LIRR) If yes, it looks to the last 4 characters of the facility name as follows:
    * Facility name **ends with _CTR?** If yes Bot searches the first channel available named "[ICAO] ACC Room" (eg. LIRR ACC Room)
    * Facility name **ends with _APP?** If yes Bot searches the first channel available named "[ICAO] ACC Room" (eg. LIRR ARR/DEP)
    
### Scheduled Jobs
The bot includes a scheduled jobs engine based on [Quartz](https://www.quartz-scheduler.net/).
Jobs scheduled in v1.2.0:
* **Delete past exam/training posts**: scheduled every day at 00:00 UTC
* **Auto start Guild scheduled events**: scheduled every 30mins
* **Check exam/training cancellations**: scheduled every day, every 15mins, from 16:00UTC to 21:00UTC

### Components
2 projects are designed to be the 2 application entrypoints:
* **Ivao.It.DiscordLogin**: web UI for policy acceptance. .NET 6 AspNetCore. Database connected to store the policies consents and a adopts a OAuth2 authentication towards Discord.
* **Ivao.It.DiscordBot.Service**: .NET 6 worker service to run the Bot and enrich it with all the needed dependencies like logging, DI, runtime configurations.

2 libraries:
* **Ivao.It.DiscordBot.Bot**: Bot implementation. Wraps all the Bot logic, run by the service described above.
* **Ivao.It.DiscordBot.Data**: Data access layer for Web UI and Bot. Where the users storage happens and the consent is stored.

### Requirements
Built on the .NET 6 stack. No framework install needed to run.
Actually deployed on Linux x64 self contained single file executable.

For the Web UI a reverse proxy is needed. The proxy have to forward requests including 3 mandatory headers:
* `X-Forwarded-Proto`
* `X-Forwarded-Host`
* `X-Forwarded-For`

