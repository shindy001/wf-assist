using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.Shared.Notifications;

public static class NotificationServiceCollectionExtensions
{
	public static IServiceCollection RegisterNotificationTypes(
		this IServiceCollection services,
		params Assembly[] notificationAssemblies)
	{
		var notificationTypes = notificationAssemblies
			.SelectMany(assembly => assembly.GetTypes())
			.Where(t => !t.IsAbstract && typeof(Notification).IsAssignableFrom(t))
			.ToList();

		NotificationTypeRegistry.RegisterNotificationTypes(notificationTypes);

		return services;
	}
}