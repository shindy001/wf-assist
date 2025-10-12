export interface Position {
    x: number;
    y: number;
}

export enum WorkflowNodeType {
    Default = "default",
    PrintText = "printText",
    ExtractProperty = "extractProperty",
    Request = "request",
}

export interface WorkflowNodeDataBase {
    type: WorkflowNodeType;
}

export interface PrintTextNodeData extends WorkflowNodeDataBase {
    targetId?: string;
    text: string;
    useConsole: boolean;
}

export interface ExtractPropertyNodeData extends WorkflowNodeDataBase {
    path?: string;
    targetId?: string;
}

export interface RequestNodeData extends WorkflowNodeDataBase {
    url?: string;
    requestType: string;
    requestBody?: string
}

export type WorkflowNodeData = PrintTextNodeData | ExtractPropertyNodeData | RequestNodeData;

export interface WorkflowNode {
    id: string;
    type: WorkflowNodeType;
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

export interface WorkflowIdentity {
    id: string;
    name: string;
}
