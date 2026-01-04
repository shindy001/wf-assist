using WfAssist.AspNetCore.Core.Models;

namespace WfAssist.AspNetCore.Core.Runtime;

public static class WorkflowTopologySorter
{
    /// <summary>
    /// Calculates dependency graph, topologically orders the nodes and returns nodes according to execution order
    /// </summary>
    /// <param name="data"><see cref="WorkflowData"/> - collection of nodes and edges</param>
    /// <returns><see cref="List{WorkflowNode}"/> in execution order</returns>
    public static List<WorkflowNode> CalculateNodeExecution(WorkflowData data)
    {
        if (!data.Nodes.Any())
        {
            return [];
        }

        ValidateEdges(data);

        var nodeExecutionOrder = CalculateExecutionOrder(data);
        return nodeExecutionOrder.Select(nodeId => data.Nodes.First(x => x.Id == nodeId)).ToList();
    }

    private static void ValidateEdges(WorkflowData data)
    {
        var nodeIds = data.Nodes.Select(x => x.Id).ToList();
        foreach (var edge in data.Edges)
        {
            if (!nodeIds.Contains(edge.Source))
            {
                throw new ArgumentException(
                    $"Invalid edge '{edge.Id}', source node with Id '{edge.Source}' does not exist.");
            }

            if (!nodeIds.Contains(edge.Target))
            {
                throw new ArgumentException(
                    $"Invalid edge '{edge.Id}', target node with Id '{edge.Target}' does not exist.");
            }
        }
    }

    /// <summary>
    /// Converts <see cref="WorkflowData"/> to graph of node dependencies and inDegree (incoming edges)
    /// </summary>
    /// <param name="data"><see cref="WorkflowData"/> - collection of nodes and edges</param>
    /// <returns>
    /// Graph - map of node Ids to connected node Ids<br/>
    /// InDegrees - map of node Ids to number of incoming edges
    /// </returns>
    private static (Dictionary<string, List<string>> graph, Dictionary<string, int> inDegree) BuildDependencyGraph(
        WorkflowData data)

    {
        var graph = new Dictionary<string, List<string>>();
        var inDegree = new Dictionary<string, int>();

        foreach (var node in data.Nodes)
        {
            graph[node.Id] = [];
            inDegree[node.Id] = 0;
        }

        foreach (var edge in data.Edges)
        {
            graph[edge.Source].Add(edge.Target);
            inDegree[edge.Target]++;
        }

        return (graph, inDegree);
    }

    /// <summary>
    /// Calculates dependency graph and then uses Kahn’s Algorithm to topologically order the graph nodes - <see href="https://en.wikipedia.org/wiki/Topological_sorting"/>
    /// </summary>
    /// <param name="data"><see cref="WorkflowData"/> - collection of nodes and edges</param>
    /// <returns><see cref="List{string}"/> of Ids in execution order</returns>
    private static List<string> CalculateExecutionOrder(WorkflowData data)
    {
        var (graph, inDegree) = BuildDependencyGraph(data);
        var nodeIdQueue = new Queue<string>();
        var executionOrder = new List<string>();

        // Enqueue items that has no inDegrees (incoming edges)
        foreach (var item in inDegree)
        {
            if (item.Value == 0)
            {
                nodeIdQueue.Enqueue(item.Key);
            }
        }

        while (nodeIdQueue.Count > 0)
        {
            // Push first item from queue to final node order
            var current = nodeIdQueue.Dequeue();
            executionOrder.Add(current);

            // Decrease current nodeId inDegrees (incoming edges) as its already in final order list
            foreach (var id in graph[current])
            {
                inDegree[id]--;
                if (inDegree[id] <= 0)
                {
                    nodeIdQueue.Enqueue(id);
                }
            }
        }

        return executionOrder.Count != data.Nodes.Count()
            ? throw new InvalidOperationException("Cycle detected in workflow graph, cannot calculate execution order.")
            : executionOrder;
    }
}