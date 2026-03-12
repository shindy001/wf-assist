using System.Threading.Channels;
using WfAssist.Workflows.Core.Models.Notifications;

namespace WfAssist.Workflows.Core.Services;

internal interface INotificationDispatcher
{
    ChannelReader<Notification> Register(Guid clientId);

    void Unregister(Guid clientId);

    Task Dispatch(Notification notification);
}