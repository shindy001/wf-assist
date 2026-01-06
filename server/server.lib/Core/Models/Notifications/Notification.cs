using System.Text.Json.Serialization;

namespace WfAssist.AspNetCore.Core.Models.Notifications;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(WorkflowExecutionStarted), nameof(WorkflowExecutionStarted))]
[JsonDerivedType(typeof(WorkflowExecutionEnded), nameof(WorkflowExecutionEnded))]
public abstract record Notification;