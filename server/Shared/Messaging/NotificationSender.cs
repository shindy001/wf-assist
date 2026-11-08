using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.Shared.Messaging;

public interface INotificationSender
{
	Task Send(Notification message);
}

internal sealed class NotificationSender(IHubContext<MessageHub> clientMessageHub) : INotificationSender
{
	public async Task Send(Notification message)
	{
		await clientMessageHub.Clients.All.SendAsync(message.GetType().Name, message);
	}
}

public static class MessagingExtensions
{
	public static void AddMessaging(this IServiceCollection services)
	{
		services.AddSignalR();
		services.AddTransient<INotificationSender, NotificationSender>();
	}

	public static void UseMessaging(this WebApplication app, string websocketRoute = Constants.ApiWebSocketRoute)
	{
		app.MapHub<MessageHub>(websocketRoute);
	}
}
