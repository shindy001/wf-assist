using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.Shared.CQRS;

public interface IQuery<out TResult>;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}

public interface IQueryDispatcher
{
    Task<TResult> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}

public sealed class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    public async Task<TResult> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
        var handler = serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(nameof(IQueryHandler<,>.Handle));
        if (method is null)
        {
            throw new InvalidOperationException($"'{nameof(IQueryHandler<,>)}.Handle' method not found on query '{queryType.Name}' handler.");
        }

        var result = method.Invoke(handler, [query, cancellationToken]);
        if (result is null)
        {
            throw new InvalidOperationException($"'{nameof(IQueryHandler<,>)}.Handle' for query '{queryType.Name}' did not return any value.");
        }

        return await (Task<TResult>)result;
    }
}
