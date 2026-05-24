namespace WfAssist.Shared;

public abstract record Error(string Message);

public sealed record NotFoundError(string Message) : Error(Message);