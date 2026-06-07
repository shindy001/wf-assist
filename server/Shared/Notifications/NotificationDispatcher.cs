using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WfAssist.Shared.Notifications;

internal sealed class NotificationDispatcher : INotificationDispatcher
{
    private readonly ConcurrentDictionary<Guid, Channel<Notification>> _channels = new();

    public NotificationDispatcher(IHostApplicationLifetime applicationLifetime, ILogger<NotificationDispatcher> logger)
    {
        RegisterCloseChannelsOnAppShutdownCallback(applicationLifetime, logger);
    }

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

    private void RegisterCloseChannelsOnAppShutdownCallback(IHostApplicationLifetime applicationLifetime, ILogger<NotificationDispatcher> logger)
    {
        applicationLifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation($"{nameof(NotificationDispatcher)}: Application is stopping, closing channels...");
            foreach (var channel in _channels)
            {
                channel.Value.Writer.TryComplete();
            }
        });
    }
}

