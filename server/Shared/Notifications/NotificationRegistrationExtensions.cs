using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.Shared.Notifications;

public static class NotificationServiceCollectionExtensions
{
	/// <summary>
	/// Registers notification dispatcher,
	/// probes app domain assemblies for <see cref="Notification"/> derived types and adds
	/// <see cref="NotificationTypeJsonResolver"/> with the derived types to <see cref="JsonSerializerOptions.TypeInfoResolverChain"/>.
	/// </summary>
	public static void AddNotifications(this IServiceCollection services)
	{
		services.AddSingleton<INotificationDispatcher, NotificationDispatcher>();

		var notificationTypes = GetNotificationTypes();
		services.Configure<JsonOptions>(options =>
			options.SerializerOptions.TypeInfoResolverChain.Insert(0,
				new NotificationTypeJsonResolver(notificationTypes)));
	}

	private static Type[] GetNotificationTypes()
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(t => !t.IsAbstract && typeof(Notification).IsAssignableFrom(t))
			.ToArray();
	}
}