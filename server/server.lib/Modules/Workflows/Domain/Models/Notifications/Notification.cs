using System.Text.Json.Serialization;

namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Models.Notifications;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ExecutionStarted), nameof(ExecutionStarted))]
[JsonDerivedType(typeof(ExecutionEnded), nameof(ExecutionEnded))]
public abstract record Notification;