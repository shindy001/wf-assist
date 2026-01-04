using System.Text.Json.Serialization;

namespace WfAssist.AspNetCore.Core.Models.Notifications;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ExecutionStarted), nameof(ExecutionStarted))]
[JsonDerivedType(typeof(ExecutionEnded), nameof(ExecutionEnded))]
public abstract record Notification;