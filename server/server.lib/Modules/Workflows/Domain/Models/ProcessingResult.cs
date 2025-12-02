namespace WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

public record ProcessingResult
{
    public bool Successful { get; init; }
    public ProcessResultValueType ValueType { get; init; }
    public object? Data { get; init; }
    public string? ErrorMessage { get; init; }

    private ProcessingResult(
        bool Successful,
        ProcessResultValueType ValueType,
        object? Data = null,
        string? ErrorMessage = null)
    {
        this.Successful = Successful;
        this.ValueType = ValueType;
        this.Data = Data;
        this.ErrorMessage = ErrorMessage;
    }

    public static ProcessingResult Success(ProcessResultValueType valueType, object? data = null)
    {
        VerifyValueType(valueType, data);
        return new ProcessingResult(Successful: true, valueType, data);
    }

    public static ProcessingResult Error(string message, object? data = null)
    {
        return new ProcessingResult(Successful: false, ProcessResultValueType.Error, data, message);
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