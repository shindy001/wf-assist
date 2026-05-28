using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace WfAssist.Shared.Notifications;

public sealed class NotificationTypeJsonResolver : DefaultJsonTypeInfoResolver
{
	public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		var info = base.GetTypeInfo(type, options);

		if (type != typeof(Notification))
		{
			return info;
		}

		info.PolymorphismOptions = new JsonPolymorphismOptions()
		{
			TypeDiscriminatorPropertyName = "type"
		};

		foreach (var derivedType in NotificationTypeRegistry.GetNotificationTypes())
		{
			info.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(derivedType, derivedType.Name));
		}

		return info;
	}
}