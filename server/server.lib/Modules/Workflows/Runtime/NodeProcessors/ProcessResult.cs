namespace WfAssist.AspNetCore.Modules.Workflows.Runtime.NodeProcessors;

public record ProcessResult
{
    public bool IsSuccessful { get; init; }
    public ProcessResultValueType ValueType { get; init; }
    public object? Data { get; init; }
    public string? ErrorMessage { get; init; }

    private ProcessResult(bool IsSuccessful,
        ProcessResultValueType ValueType,
        object? Data = null,
        string? ErrorMessage = null)
    {
        this.IsSuccessful = IsSuccessful;
        this.ValueType = ValueType;
        this.Data = Data;
        this.ErrorMessage = ErrorMessage;
    }

    public static ProcessResult Success(ProcessResultValueType valueType, object? data = null)
    {
        VerifyValueType(valueType, data);
        return new ProcessResult(IsSuccessful: true, valueType, data);
    }

    public static ProcessResult Error(string message, ProcessResultValueType valueType, object? data = null)
    {
        VerifyValueType(valueType, data);
        return new ProcessResult(IsSuccessful: true, valueType, data);
    }

    private static void VerifyValueType(ProcessResultValueType valueType, object? data)
    {
        if (valueType is ProcessResultValueType.None && data is not null)
        {
            throw new InvalidOperationException(
                $"Result data cannot have value when {nameof(ProcessResultValueType)} is {ProcessResultValueType.None}");
        }

        if (valueType is not ProcessResultValueType.None && data is null)
        {
            throw new InvalidOperationException(
                $"Result data cannot be null when {nameof(ProcessResultValueType)} is {valueType}");
        }
    }
}