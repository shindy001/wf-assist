using Microsoft.AspNetCore.SignalR;

namespace WfAssist.Shared.Messaging;

/// <summary>
/// Shared hub for messages sent by client to server. Use partial class to implement methods that can be invoked by SignalR client.
/// </summary>
/// <example>
/// <code>
/// public sealed partial class ClientMessageHub
/// {
///     public Task&lt;string&gt; GetUserName(Guid id)
///     {
///         return Task.FromResult("TestUserName");
///     }
/// }
/// </code>
/// </example>
public sealed partial class MessageHub : Hub;
