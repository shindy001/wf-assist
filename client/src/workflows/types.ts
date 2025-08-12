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

interface WorkflowNodeDataBase {
    type: WorkflowNodeType;
}

export class PrintTextNodeData implements WorkflowNodeDataBase {
    readonly type: WorkflowNodeType = WorkflowNodeType.PrintText;

    constructor(public targetId?: string, public text?: string) {}
}

export class ExtractPropertyNodeData implements WorkflowNodeDataBase {
    readonly type: WorkflowNodeType = WorkflowNodeType.ExtractProperty;

    constructor(public path?: string, public targetId?: string) {}
}

export class RequestNodeData implements WorkflowNodeDataBase {
    readonly type: WorkflowNodeType = WorkflowNodeType.Request;

    constructor(public url?: string, public requestType?: string, public requestBody?: string) {}
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
