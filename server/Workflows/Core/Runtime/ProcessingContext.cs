using WfAssist.Shared.Contracts;

namespace WfAssist.Workflows.Core.Runtime;

internal sealed class ProcessingContext
{
    public Dictionary<string, ProcessingResult> ProcessingResults { get; } = [];

    public ProcessingResult? GetResult(string nodeId)
    {
        return ProcessingResults.GetValueOrDefault(nodeId);
    }

    public void AddResult(string nodeRefId, ProcessingResult result)
    {
        ProcessingResults.Add(nodeRefId, result);
    }

    public bool IsProcessingSuccessful() => ProcessingResults.Values.All(x => x.IsSuccessful);
}