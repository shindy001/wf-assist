using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.Path;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;

namespace WfAssist.AspNetCore.Modules.Workflows.Runtime;

internal partial class WorkflowNodeReferenceResolver
{
    private readonly ProcessingContext _processingContext;

    private readonly Regex _pattern = NodeReferencePattern();

    public WorkflowNodeReferenceResolver(ProcessingContext processingContext)
    {
        _processingContext = processingContext;
    }

    public string Resolve(string input)
    {
        return _pattern.Replace(input, match =>
        {
            var expressionType = match.Groups[1].Value.Trim();
            var nodeId =  match.Groups[2].Value.Trim();
            var dataPath = match.Groups[3].Value.Replace(" ", string.Empty);

            if (!IsNodeReferenceExpression(expressionType))
            {
                throw new ArgumentException($"Invalid Node reference expression type '{expressionType}'. Used expression: {match.Value}");
            }

            var nodeResult = _processingContext.GetResult(nodeId);
            if (nodeResult is null)
            {
                throw new ArgumentException($"There is no result for node {nodeId}. Used expression: {match.Value}");
            }

            return nodeResult.ValueType switch
            {
                ProcessResultValueType.None => throw new ArgumentException(
                    $"Result for node expression '{match.Value}' does not have value."),
                ProcessResultValueType.Error => throw new ArgumentException(
                    $"Result for node expression '{match.Value}' has error value only."),
                ProcessResultValueType.JsonDocument => ParseJsonExpression(dataPath, nodeResult.Data as JsonDocument),
                _ => throw new InvalidOperationException(
                    $"Unexpected node result value type '{nodeResult.ValueType}' while parsing expression: '{match.Value}'.")
            };
        });
    }

    private string ParseJsonExpression(string jsonPath, JsonDocument? jsonDocument)
    {
        if (jsonDocument is null)
        {
            throw new ArgumentException($"Cannot resolve json path {jsonPath}, json document is empty.");
        }

        var jsonNode = jsonDocument.Deserialize<JsonNode>();
        var path = JsonPath.Parse($"$.{jsonPath}");
        var result = path.Evaluate(jsonNode);
        return result.Matches.Count switch
        {
            1 => result.Matches.First().Value?.ToString() ?? string.Empty,
            0 => throw new ArgumentException($"Result data for json path {jsonPath} not found."),
            _ => throw new ArgumentException($"Result data for json path {jsonPath} has multiple matches.")
        };
    }

    private static bool IsNodeReferenceExpression(string expressionType) =>
        expressionType.Equals("node", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Node reference patterns #{[expressionType]:[nodeId].[dataPath]}
    /// </summary>
    /// <example>
    /// #{node:1.id}
    /// #{node:1234.items[0].name}
    /// #{node:id5.basket.items[0].price}
    /// </example>
    /// <returns></returns>
    [GeneratedRegex(@"#\{([^:}]+):([^\.}]+)\.([^}]+)\}", RegexOptions.Compiled)]
    private static partial Regex NodeReferencePattern();
}