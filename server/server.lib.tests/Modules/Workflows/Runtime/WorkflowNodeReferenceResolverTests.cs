using System.Text.Json;
using Json.More;
using Shouldly;
using WfAssist.AspNetCore.Modules.Workflows.Domain.Models;
using WfAssist.AspNetCore.Modules.Workflows.Runtime;

namespace server.lib.tests.Modules.Workflows.Runtime;

public class WorkflowNodeReferenceResolverTests
{
    [Fact]
    public void Resolve_should_succeed()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse("""{ "id": "1", "name": "test1" }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        var result = resolver.Resolve("Test resolve #{node:1.name}");

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldBe("Test resolve test1");
    }

    [Fact]
    public void Resolve_should_succeed_with_whitespaces_in_expression()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse("""{ "id": "1", "name": "test1" }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        var result = resolver.Resolve("Test resolve #{   node   :   1  .   name   }");

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldBe("Test resolve test1");
    }

    [Fact]
    public void Resolve_should_return_original_string_when_expression_type_node_is_not_specified()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse("""{ "id": "1", "name": "test1" }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        var result = resolver.Resolve("Test resolve #{1.name}"); // Expression type 'node:' missing before the JSON prop path

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldBe("Test resolve #{1.name}");
    }

    [Fact]
    public void Resolve_should_return_original_string_for_expression_that_does_not_specify_expression_type_node()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse("""{ "id": "1", "name": "test1" }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        var result = resolver.Resolve("Test #{node:1.id} resolve #{1.name}"); // Expression type 'node:' missing before the JSON prop path

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldBe("Test 1 resolve #{1.name}");
    }

    [Fact]
    public void Resolve_should_succeed_for_deep_selectors()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse("""{ "data": { "id": "5", "prop1": { "prop2": 5 } } }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        var result = resolver.Resolve("Test resolve #{node:1.data.prop1.prop2}");

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldBe("Test resolve 5");
    }

    [Fact]
    public void Resolve_should_succeed_for_multiple_selectors()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse(
                    """{ "data": { "id": "5", "prop1": { "prop2": 5, "data": { "prop3" : "very deep" } } } }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        var result = resolver.Resolve("Test resolve #{node:1.data.id}, second is #{node:1.data.prop1.data.prop3}");

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldBe("Test resolve 5, second is very deep");
    }

    [Fact]
    public void Resolve_should_succeed_for_object_selectors()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse(
                    """{ "data": { "id": "5", "prop1": "test1" } }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        var result = resolver.Resolve("""{"id": "1", "data": #{node:1.data} }""");

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        JsonShouldMatch(expected: """{ "id": "1", "data": { "id": "5", "prop1": "test1" } }""", actual: result);
    }

    [Fact]
    public void Resolve_should_succeed_for_combination_of_value_and_object_selector()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse(
                    """{ "data": { "id": "5", "prop1": { "name": "value" } } }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        var result = resolver.Resolve("Test resolve #{node:1.data.id} #{node:1.data.prop1}");

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldBe("""
                        Test resolve 5 {
                          "name": "value"
                        }
                        """);
    }

    [Fact]
    public void Resolve_should_succeed_for_combination_of_value_and_object_selector_from_different_node_results()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse(
                    """{ "data": { "id": "5", "prop1": { "name": "node1_Test" } } }""")));
        processingContext.AddResult("2",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse(
                    """{ "items": { "count": 42 } }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        var result = resolver.Resolve("Test resolve #{node:2.items.count} #{node:1.data.prop1.name}");

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldBe("Test resolve 42 node1_Test");
    }

    [Fact]
    public void Resolve_should_throw_ArgumentException_when_node_result_from_expression_does_not_exist()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        Action act = () =>  resolver.Resolve("Test resolve #{node:1.name}");

        // Assert
        act.ShouldThrow<ArgumentException>().Message.ShouldBe("There is no result for node 1. Used expression: #{node:1.name}");
    }

    [Fact]
    public void Resolve_should_throw_ArgumentException_when_different_expression_type_is_specified()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument,
                JsonDocument.Parse("""{ "id": "1", "name": "test1" }""")));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        Action act =  () => resolver.Resolve("Test resolve #{ctx:1.name}"); // Expression type 'ctx:'

        // Assert
        act.ShouldThrow<ArgumentException>().Message.ShouldBe("Invalid Node reference expression type 'ctx'. Used expression: #{ctx:1.name}");
    }

    [Fact]
    public void Resolve_should_throw_ArgumentException_when_node_result_from_expression_does_not_have_data()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.None));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        Action act =  () => resolver.Resolve("Test resolve #{node:1.name}");

        // Assert
        act.ShouldThrow<ArgumentException>().Message.ShouldBe("Result for node expression '#{node:1.name}' does not have value.");
    }

    [Fact]
    public void Resolve_should_throw_ArgumentException_when_node_result_from_expression_have_error()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Error("Uh oh, something happened", "1"));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        Action act =  () => resolver.Resolve("Test resolve #{node:1.name}");

        // Assert
        act.ShouldThrow<ArgumentException>().Message.ShouldBe("Result for node expression '#{node:1.name}' has error value only.");
    }

    [Fact]
    public void Resolve_should_throw_ArgumentException_when_node_result_specify_JsonDocument_data_but_data_has_different_type()
    {
        // Arrange
        var processingContext = new ProcessingContext();
        processingContext.AddResult("1",
            ProcessingResult.Success(ProcessResultValueType.JsonDocument, "test"));

        var resolver = new WorkflowNodeReferenceResolver(processingContext);

        // Act
        Action act =  () => resolver.Resolve("Test resolve #{node:1.name}");

        // Assert
        act.ShouldThrow<ArgumentException>().Message.ShouldBe("Cannot resolve json path name, json document is empty.");
    }

    private static void JsonShouldMatch(string expected, string actual)
    {
        var expectedNormalized = JsonDocument.Parse(expected).RootElement.ToJsonString();
        var actualNormalized = JsonDocument.Parse(actual).RootElement.ToJsonString();
        actualNormalized.ShouldBe(expectedNormalized);
    }
}