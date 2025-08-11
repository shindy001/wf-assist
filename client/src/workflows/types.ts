export interface Position {
    x: number;
    y: number;
}

export interface PrintTextNodeData {
    text: string;
}

export interface ExtractPropertyNodeData {
    propertyPath: string;
}

export type NodeData = undefined | PrintTextNodeData | ExtractPropertyNodeData;

export interface WorkflowNode {
    id: string;
    type: string;
    position: Position;
    data: NodeData;
}

export interface WorkflowEdge {
    id: string;
    type: string;
    position: Position;
}

export interface WorkflowData {
    nodes: Array<WorkflowNode>;
    edges: Array<WorkflowEdge>;
}

export interface Workflow {
    id: string;
    name: string;
    data: WorkflowData;
}

/**
 * Custom node types
 * @link {https://reactflow.dev/api-reference/types/node#default-node-types}
 */
export enum FlowNodeType {
    Request = "request",
    ExtractProperty = "extractProperty",
    PrintString = "printString",
}

export interface NodeBase extends Record<string, unknown> {
    id: string;
}

export interface ExtractPropertyNode extends NodeBase {
    path?: string;
    targetId?: string;
}

export interface PrintStringNode extends NodeBase {
    targetId?: string;
}


export interface RequestNode extends NodeBase {
    url?: string,
    requestType?: string,
    requestBody?: string,
}

export enum NodeType {
    Request = "request",
    ExtractProperty = "extractProperty",
    PrintString = "printString",
}
