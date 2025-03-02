export enum FlowNodeType {
    // Default node types - https://reactflow.dev/api-reference/types/node#default-node-types

    // "default" - default node is a fallback when node is not specified, no need an enum value for it
    // "group" - Used for grouping, group is a parent node that contains other nodes and moves together,
    // grouping is not implemented right now, more info https://svelteflow.dev/learn/guides/sub-flows

    Input = "input",
    Output = "output",

    // Custom node types
    Request = "request",
    ExtractProperty = "extractProperty",
}