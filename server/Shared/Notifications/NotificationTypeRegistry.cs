namespace WfAssist.Shared.Notifications;

public static class NotificationTypeRegistry
{
	private static readonly HashSet<Type> NotificationTypes = [];

	public static void RegisterNotificationTypes(List<Type> types)
	{
		foreach (var type in types)
		{
			if (type.IsAbstract || !typeof(Notification).IsAssignableFrom(type))
			{
				throw new ArgumentException($"The type {type.FullName} must inherit from Notification and cannot be abstract");
			}

			NotificationTypes.Add(type);
		}
	}

	public static HashSet<Type> GetNotificationTypes() => NotificationTypes;
}