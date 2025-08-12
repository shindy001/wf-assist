export interface Position {
    x: number;
    y: number;
}

export interface WorkflowNodeDataBase extends Record<string, unknown> {}

export interface PrintTextNodeData extends WorkflowNodeDataBase {
    targetId?: string;
    text?: string;
}

export interface ExtractPropertyNodeData extends WorkflowNodeDataBase {
    path?: string;
    targetId?: string;
}

export interface RequestNodeData extends WorkflowNodeDataBase {
    url?: string,
    requestType?: string,
    requestBody?: string,
}

export type WorkflowNodeData = PrintTextNodeData | ExtractPropertyNodeData | RequestNodeData;

export enum WorkflowNodeType {
    Request = "request",
    ExtractProperty = "extractProperty",
    PrintText = "printText",
}

export interface WorkflowNode {
    id: string;
    type: string;
    position: Position;
    data: WorkflowNodeData;
}

export interface WorkflowEdge {
    id: string;
    type: string;
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
