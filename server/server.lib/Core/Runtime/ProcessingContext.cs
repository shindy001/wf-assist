using WfAssist.AspNetCore.Core.Models;

namespace WfAssist.AspNetCore.Core.Runtime;

internal sealed class ProcessingContext
{
    public Dictionary<string, ProcessingResult> ProcessingResults { get; } = [];

    public ProcessingResult? GetResult(string nodeId)
    {
        return ProcessingResults.GetValueOrDefault(nodeId);
    }

    public void AddResult(string nodeId, ProcessingResult result)
    {
        ProcessingResults.Add(nodeId, result);
    }

    public bool IsProcessingSuccessful() => ProcessingResults.Values.All(x => x.IsSuccessful);
}