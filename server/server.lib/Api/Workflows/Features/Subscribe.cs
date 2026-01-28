using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WfAssist.AspNetCore.Core.Models.Notifications;
using WfAssist.AspNetCore.Core.Services;

namespace WfAssist.AspNetCore.Api.Workflows.Features;

public static class Subscribe
{
    [ProducesResponseType(typeof(IAsyncEnumerable<WorkflowExecutionStarted>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IAsyncEnumerable<WorkflowExecutionEnded>), StatusCodes.Status200OK)]
    public static void MapSubscribeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/events",
            (INotificationDispatcher notificationDispatcher, CancellationToken cancellationToken) =>
                TypedResults.ServerSentEvents(GetNotifications(notificationDispatcher, cancellationToken)));
    }

    private static async IAsyncEnumerable<Notification> GetNotifications(INotificationDispatcher notificationDispatcher,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var clientId = Guid.NewGuid();
        var reader = notificationDispatcher.Register(clientId);

        try
        {
            while (await reader.WaitToReadAsync(cancellationToken))
            {
                while (reader.TryRead(out var notification))
                {
                    yield return notification;
                }
            }
        }
        finally
        {
            notificationDispatcher.Unregister(clientId);
        }
    }
}