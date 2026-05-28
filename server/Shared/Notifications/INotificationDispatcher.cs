using System.Threading.Channels;

namespace WfAssist.Shared.Notifications;

public interface INotificationDispatcher
{
    ChannelReader<Notification> Register(Guid clientId);

    void Unregister(Guid clientId);

    Task Dispatch(Notification notification);
}