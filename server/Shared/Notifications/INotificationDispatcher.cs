using System.Threading.Channels;

namespace WfAssist.Shared.Notifications;

/// <summary>
/// Notification dispatcher used for notification dispatching(<see cref="Dispatch"/>) and for consuming them(<see cref="Register"/>).
/// <remarks>Notification channels should be unbounded by default and should never complete hence call <see cref="Unregister"/> if there is no longer need to listen to it.</remarks>
/// </summary>
public interface INotificationDispatcher
{
    ChannelReader<Notification> Register(Guid clientId);

    void Unregister(Guid clientId);

    Task Dispatch(Notification notification);
}