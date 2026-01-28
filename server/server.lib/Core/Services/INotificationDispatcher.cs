using System.Threading.Channels;
using WfAssist.AspNetCore.Core.Models.Notifications;

namespace WfAssist.AspNetCore.Core.Services;

internal interface INotificationDispatcher
{
    ChannelReader<Notification> Register(Guid clientId);

    void Unregister(Guid clientId);

    Task Dispatch(Notification notification);
}