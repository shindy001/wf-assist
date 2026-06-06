using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.Shared.Notifications;

public static class NotificationServiceCollectionExtensions
{
	public static void AddNotificationDispatcher(this IServiceCollection services, Type dispatcherType)
	{
		if (!typeof(INotificationDispatcher).IsAssignableFrom(dispatcherType))
		{
			throw new ArgumentException(
				$"{dispatcherType.Name} must implement {nameof(INotificationDispatcher)}");
		}

		RegisterNotificationTypes();
		services.AddSingleton(typeof(INotificationDispatcher), dispatcherType);
	}

	private static void RegisterNotificationTypes()
	{
		var notificationTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(t => !t.IsAbstract && typeof(Notification).IsAssignableFrom(t))
			.ToList();

		NotificationTypeRegistry.RegisterNotificationTypes(notificationTypes);
	}
}