using System.Collections.Concurrent;
using System.Threading.Channels;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models.Notifications;

namespace WfAssist.AspNetCore.Modules.Workflows.Infrastructure;

internal sealed class NotificationDispatcher
{
    private readonly ConcurrentDictionary<Guid, Channel<Notification>> _channels = new();

    public ChannelReader<Notification> Register(Guid clientId)
    {
        var channel = Channel.CreateUnbounded<Notification>();
        _channels[clientId] = channel;
        return channel.Reader;
    }

    public void Unregister(Guid clientId)
    {
        if (_channels.TryRemove(clientId, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }

    public async Task Dispatch(Notification notification)
    {
        foreach (var channel in _channels)
        {
            await channel.Value.Writer.WriteAsync(notification);
        }
    }
}

