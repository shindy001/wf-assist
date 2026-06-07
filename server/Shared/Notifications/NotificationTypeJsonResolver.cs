using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace WfAssist.Shared.Notifications;

internal sealed class NotificationTypeJsonResolver(Type[] notificationTypes) : DefaultJsonTypeInfoResolver
{
	public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		var info = base.GetTypeInfo(type, options);

		if (type != typeof(Notification) || notificationTypes.Length <= 0)
		{
			return info;
		}

		info.PolymorphismOptions = new JsonPolymorphismOptions
		{
			TypeDiscriminatorPropertyName = "type",
		};

		foreach (var derivedType in notificationTypes)
		{
			info.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(derivedType, derivedType.Name));
		}

		return info;
	}
}