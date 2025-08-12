export interface Position {
    x: number;
    y: number;
}

export enum WorkflowNodeType {
    PrintText = "printText",
    ExtractProperty = "extractProperty",
    Request = "request",
}

export interface PrintTextNodeData {
    type: WorkflowNodeType.PrintText;
    targetId?: string;
    text?: string;
}

export interface ExtractPropertyNodeData {
    type: WorkflowNodeType.ExtractProperty;
    path?: string;
    targetId?: string;
}

export interface RequestNodeData {
    type: WorkflowNodeType.Request;
    url?: string,
    requestType?: string,
    requestBody?: string,
}

export type WorkflowNodeData = PrintTextNodeData | ExtractPropertyNodeData | RequestNodeData;

export interface WorkflowNode {
    id: string;
    position: Position;
    data: WorkflowNodeData;
}

export interface WorkflowEdge {
    id: string;
    position: Position;
    source: string;
    target: string;
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
