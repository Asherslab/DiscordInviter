# Discord Inviter Bot

Stupid simple invite restricting bot.

Adds a slash command to your discord (/invite), you set the config for restricting the settings for all created
invites (MaxAge and MaxUses)
Every created invite can be logged to a channel if configured to do so. that's all!

# Config

Appsettings.json or ENV variables

| Json Path           | ENV                  | Type              | Description                                                                     |
|---------------------|----------------------|-------------------|---------------------------------------------------------------------------------|
| `GuildId`           | `GuildId`            | `Nullable<ulong>` | Guild ID for adding commands, if not set: commands are global                   |
| `BotToken`          | `BotToken`           | `string`          | It's the bot token                                                              | 
| `Invite.MaxUses`    | `Invite__MaxUses`    | `Nullable<int>`   | Max number of times an invite can be used, if not set: infinite times           |
| `Invite.MaxAge`     | `Invite__MaxAge`     | `Nullable<long>`  | Number of seconds an invite lasts for, maxes out at 24hrs, if not set: infinite |
| `Invite.LogChannel` | `Invite__LogChannel` | `Nullable<ulong>` | Channel to log all invite creations to, if not set: doesn't log                 |

Built container available at https://hub.docker.com/r/asherslab/dockerinviter