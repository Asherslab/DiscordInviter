using System.ComponentModel;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Rest.Core;
using Remora.Results;

namespace AshersLab.Discord.Inviter.Commands;

public class InviteCommands : CommandGroup
{
    private readonly InteractionContext _context;
    private readonly Config _config;
    private readonly IDiscordRestInteractionAPI _interactionApi;
    private readonly IDiscordRestChannelAPI _channelApi;

    public InviteCommands(InteractionContext context, IDiscordRestChannelAPI channelApi,
        IDiscordRestInteractionAPI interactionApi, Config config)
    {
        _context = context;
        _channelApi = channelApi;
        _interactionApi = interactionApi;
        _config = config;
    }

    [Command("invite")]
    [Description("Creates a single use discord invite for you")]
    public async Task<Result> MyRewardsCommand()
    {
        if (_config.MaxAge == null && _config.MaxUses == null)
        {
            Result<IMessage> errorReplyResult = await Reply("MaxAge and/or MaxUses must be set in config!");

            return !errorReplyResult.IsSuccess
                ? Result.FromError(errorReplyResult)
                : Result.FromSuccess();
        }

        Result<IInvite> inviteResult = await _channelApi.CreateChannelInviteAsync(_context.ChannelID,
            _config.MaxAge != null ? TimeSpan.FromSeconds(_config.MaxAge.Value) : new Optional<TimeSpan>(),
            _config.MaxUses ?? new Optional<int>(),
            isUnique: true
        );

        if (!inviteResult.IsSuccess) return Result.FromError(inviteResult);
        
        if (_config.LogChannel != null)
        {
            Result<IMessage> logMessageResult = await _channelApi.CreateMessageAsync(
                new Snowflake(_config.LogChannel.Value),
                $"<@!{_context.User.ID.Value}> ({_context.User.ID.Value}) created invite <https://discord.gg/{inviteResult.Entity.Code}>",
                allowedMentions:new AllowedMentions(Users: Array.Empty<Snowflake>())
            );

            if (!logMessageResult.IsSuccess)
            {
                return Result.FromError(logMessageResult);
            }
        }
        
        Result<IMessage> messageResult = await Reply($"https://discord.gg/{inviteResult.Entity.Code}");

        return !messageResult.IsSuccess
            ? Result.FromError(messageResult)
            : Result.FromSuccess();
    }

    private async Task<Result<IMessage>> Reply(string content)
    {
        return await _interactionApi.CreateFollowupMessageAsync
        (
            _context.ApplicationID,
            _context.Token,
            content,
            ct: CancellationToken
        );
    }
}