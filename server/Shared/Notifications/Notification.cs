namespace WfAssist.Shared.Notifications;

/// <summary>
/// Abstract notification used as base for WfAssist app notification, derived notification types are automatically
/// registered as polymorphic types for serialization by <see cref="NotificationServiceCollectionExtensions.AddNotifications"/>.
/// </summary>
public abstract record Notification;