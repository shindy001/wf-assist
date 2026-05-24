using Microsoft.Extensions.DependencyInjection;

namespace WfAssist.Shared.CQRS;

public interface ICommand<out TResult>;

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandDispatcher
{
    Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}

public sealed class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
        var handler = serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(nameof(ICommandHandler<,>.Handle));
        if (method is null)
        {
            throw new InvalidOperationException($"'{nameof(ICommandHandler<,>)}.Handle' method not found on command '{commandType.Name}' handler.");
        }

        var result = method.Invoke(handler, [command, cancellationToken]);
        if (result is null)
        {
            throw new InvalidOperationException($"'{nameof(ICommandHandler<,>)}.Handle' for command '{commandType.Name}' did not return any value.");
        }

        return await (Task<TResult>)result;
    }
}
