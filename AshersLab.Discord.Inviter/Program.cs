// See https://aka.ms/new-console-template for more information

using AshersLab.Discord.Inviter;
using AshersLab.Discord.Inviter.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Hosting.Extensions;
using Remora.Rest.Core;
using Remora.Results;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration(config =>
{
    config
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json");
});

builder.AddDiscordService(
    services =>
    {
        IConfiguration configuration = services.GetRequiredService<IConfiguration>();

        return configuration.GetValue<string?>("BotToken") ??
               throw new InvalidOperationException
               (
                   "No bot token has been provided. Set the REMORA_BOT_TOKEN environment variable to a " +
                   "valid token."
               );
    }
);

builder.ConfigureServices((ctx, services) =>
{
    Config? config = ctx.Configuration.GetSection("invite").Get<Config>();
    services.AddSingleton(config ?? new Config());
    
    services.AddDiscordCommands(true);

    services.AddCommandGroup<InviteCommands>();
});

builder.ConfigureLogging(x => { x.AddConsole(); });

IHost host = builder.Build();

ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
SlashService slashService = host.Services.GetRequiredService<SlashService>();
IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();

Result supportsSlashCommands = slashService.SupportsSlashCommands();

if (!supportsSlashCommands.IsSuccess)
{
    logger.LogWarning("The registered commands of the bot don't support slash commands: {Reason}",
        supportsSlashCommands.Error.Message);
    return 1;
}
else
{
    ulong? guildId = configuration.GetValue<ulong?>("guildId");
    Result updateSlashCommands = await slashService.UpdateSlashCommandsAsync(guildId != null ? new Snowflake(guildId.Value) : null);
    if (!updateSlashCommands.IsSuccess)
    {
        logger.LogWarning("Failed to update slash commands: {Reason}",
            updateSlashCommands.Error.Message);
        return 1;
    }
}

await host.RunAsync();
return 0;