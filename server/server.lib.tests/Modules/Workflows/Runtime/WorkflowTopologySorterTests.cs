using Shouldly;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;
using WfAssist.AspNetCore.Modules.Workflows.Runtime;

namespace server.lib.tests.Modules.Workflows.Runtime;

public class WorkflowTopologySorterTests
{
    [Fact]
    public void CalculateNodeExecution_should_succeed()
    {
        // ┌───────┐       ┌───────┐       ┌───────┐
        // │ Node1 │──────▶│ Node2 │──────▶│ Node3 │
        // └───────┘       └───────┘       └───────┘

        // Arrange
        var node1 = CreateExtractPropertyNode();
        var node2 = CreateRequestNode();
        var node3 = CreateExtractPropertyNode();

        var node1ToNode2Edge = CreateEdge(node1.Id, node2.Id);
        var node2ToNode3Edge = CreateEdge(node2.Id, node3.Id);

        var data = new WorkflowData
        {
            Nodes = [node3, node2, node1],
            Edges = [node2ToNode3Edge, node1ToNode2Edge]
        };

        // Act
        var result = WorkflowTopologySorter.CalculateNodeExecution(data);

        // Assert
        result.Count.ShouldBe(3);
        result[0].Id.ShouldBe(node1.Id);
        result[1].Id.ShouldBe(node2.Id);
        result[2].Id.ShouldBe(node3.Id);
    }


    [Fact]
    public void CalculateNodeExecution_should_correctly_order_nodes_when_node_has_multiple_edges()
    {
        // ┌───────┐       ┌───────┐       ┌───────┐
        // │ Node1 │──────▶│ Node2 │──────▶│ Node3 │
        // └───┬───┘       └───────┘       └───────┘
        //     │                               ▲
        //     └───────────────────────────────┘

        // Arrange
        var node1 = CreateExtractPropertyNode();
        var node2 = CreateRequestNode();
        var node3 = CreateExtractPropertyNode();

        var node1ToNode2Edge = CreateEdge(node1.Id, node2.Id);
        var node1ToNode3Edge = CreateEdge(node1.Id, node3.Id);
        var node2ToNode3Edge = CreateEdge(node2.Id, node3.Id);

        var data = new WorkflowData
        {
            Nodes = [node2, node3, node1],
            Edges = [node1ToNode3Edge, node2ToNode3Edge, node1ToNode2Edge]
        };

        // Act
        var result = WorkflowTopologySorter.CalculateNodeExecution(data);

        // Assert
        result.Count.ShouldBe(3);
        result[0].Id.ShouldBe(node1.Id);
        result[1].Id.ShouldBe(node2.Id);
        result[2].Id.ShouldBe(node3.Id);
    }

    [Fact]
    public void CalculateNodeExecution_should_correctly_order_nodes_when_graph_is_more_complex()
    {
        // Representation of example DAG at https://en.wikipedia.org/wiki/Directed_acyclic_graph
        //      ┌──────────────────────────────────┐
        //      │                                  ▼
        // ┌────┴──┐       ┌───────┐           ┌───────┐
        // │ Node1 │──────▶│ Node3 │──────────▶│ Node5 │
        // └─┬──┬──┘       └────┬──┘           └───────┘
        //   │  └────────────┐  │                  ▲
        //   │               │  │                  │
        //   ▼               ▼  ▼                  │
        // ┌───────┐       ┌───────┐               │
        // │ Node2 │──────▶│ Node4 │───────────────┘
        // └───────┘       └───────┘

        // Arrange
        var node1 = CreateExtractPropertyNode();
        var node2 = CreateRequestNode();
        var node3 = CreateExtractPropertyNode();
        var node4 = CreateRequestNode();
        var node5 = CreateExtractPropertyNode();

        var node1ToNode2Edge = CreateEdge(node1.Id, node2.Id);
        var node1ToNode3Edge = CreateEdge(node1.Id, node3.Id);
        var node1ToNode4Edge = CreateEdge(node1.Id, node4.Id);
        var node1ToNode5Edge = CreateEdge(node1.Id, node5.Id);
        var node2ToNode4Edge = CreateEdge(node2.Id, node4.Id);
        var node3ToNode4Edge = CreateEdge(node3.Id, node4.Id);
        var node3ToNode5Edge = CreateEdge(node3.Id, node5.Id);
        var node4ToNode5Edge = CreateEdge(node4.Id, node5.Id);

        var data = new WorkflowData
        {
            Nodes = [node5, node4, node3, node2, node1],
            Edges = [node1ToNode2Edge, node1ToNode3Edge, node1ToNode4Edge, node1ToNode5Edge, node2ToNode4Edge, node3ToNode4Edge, node3ToNode5Edge, node4ToNode5Edge]
        };

        // Act
        var result = WorkflowTopologySorter.CalculateNodeExecution(data);

        // Assert
        result.Count.ShouldBe(5);
        result[0].Id.ShouldBe(node1.Id);
        result[1].Id.ShouldBe(node2.Id);
        result[2].Id.ShouldBe(node3.Id);
        result[3].Id.ShouldBe(node4.Id);
        result[4].Id.ShouldBe(node5.Id);
    }

    [Fact]
    public void CalculateNodeExecution_should_not_change_order_when_there_are_no_edges()
    {
        // ┌───────┐       ┌───────┐       ┌───────┐
        // │ Node1 │       │ Node2 │       │ Node3 │
        // └───────┘       └───────┘       └───────┘

        // Arrange
        var node1 = CreateExtractPropertyNode();
        var node2 = CreateRequestNode();
        var node3 = CreateExtractPropertyNode();

        var data = new WorkflowData
        {
            Nodes = [node2, node3, node1],
            Edges = []
        };

        // Act
        var result = WorkflowTopologySorter.CalculateNodeExecution(data);

        // Assert
        result.Count.ShouldBe(3);
        result[0].Id.ShouldBe(node2.Id);
        result[1].Id.ShouldBe(node3.Id);
        result[2].Id.ShouldBe(node1.Id);
    }

    [Fact]
    public void CalculateNodeExecution_should_return_empty_nodes_when_given_data_are_empty()
    {
        // Arrange
        var edge1 = CreateEdge(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var edge2 = CreateEdge(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        var data = new WorkflowData
        {
            Nodes = [],
            Edges = [edge1, edge1]
        };

        // Act
        var result = WorkflowTopologySorter.CalculateNodeExecution(data);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void CalculateNodeExecution_should_throw_ArgumentException_when_edge_has_invalid_source()
    {
        // Arrange
        var node1 = CreateRequestNode();
        var node2 = CreateExtractPropertyNode();

        var edge1 = CreateEdge(sourceId: Guid.NewGuid().ToString(), targetId: node2.Id);

        var data = new WorkflowData
        {
            Nodes = [node1, node2],
            Edges = [edge1]
        };

        // Act
        Action act = () => WorkflowTopologySorter.CalculateNodeExecution(data);

        // Assert
        act.ShouldThrow<ArgumentException>().Message.ShouldBe(
            $"Invalid edge '{edge1.Id}', source node with Id '{edge1.Source}' does not exist.");
    }

    [Fact]
    public void CalculateNodeExecution_should_return_empty_nodes_when_given_nodes_are_empty()
    {
        // Arrange
        var data = new WorkflowData
        {
            Nodes = [],
            Edges = []
        };

        // Act
        var result = WorkflowTopologySorter.CalculateNodeExecution(data);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void CalculateNodeExecution_should_throw_InvalidOperationException_when_there_is_cyclic_dependency()
    {
        //     ┌───────────────────────────────┐
        //     ▼                               │
        // ┌───────┐       ┌───────┐       ┌───────┐
        // │ Node1 │──────▶│ Node2 │──────▶│ Node3 │
        // └───────┘       └───────┘       └───────┘

        // Arrange
        var node1 = CreateExtractPropertyNode();
        var node2 = CreateRequestNode();
        var node3 = CreateExtractPropertyNode();

        var node1ToNode2Edge = CreateEdge(node1.Id, node2.Id);
        var node2ToNode3Edge = CreateEdge(node2.Id, node3.Id);
        var node3ToNode1Edge = CreateEdge(node3.Id, node1.Id);

        var data = new WorkflowData
        {
            Nodes = [node2, node3, node1],
            Edges = [node1ToNode2Edge, node2ToNode3Edge, node3ToNode1Edge ]
        };

        // Act
        Action act = () => WorkflowTopologySorter.CalculateNodeExecution(data);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Cycle detected in workflow graph, cannot calculate execution order.");
    }

    private static WorkflowNode CreateExtractPropertyNode()
    {
        return new WorkflowNode
        {
            Id = Guid.NewGuid().ToString(),
            Position = new Position(0, 0),
            Data = new ExtractPropertyNodeData {Path = string.Empty, TargetId = string.Empty}
        };
    }

    private static WorkflowNode CreateRequestNode()
    {
        return new WorkflowNode
        {
            Id = Guid.NewGuid().ToString(),
            Position = new Position(0, 0),
            Data = new RequestNodeData { RequestType = RequestType.Get, Url = string.Empty }
        };
    }

    private static WorkflowEdge CreateEdge(string sourceId, string targetId)
    {
        return new WorkflowEdge
        {
            Id = Guid.NewGuid().ToString(),
            Source = sourceId,
            Target = targetId
        };
    }
}