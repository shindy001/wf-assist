export interface Position {
  x: number;
  y: number;
}

// These enum values needs to be exact match to the values on server, see "workflowMapper.ts => toWorkflowData" to more info
export enum WorkflowNodeType {
  Default = "Default",
  PrintText = "PrintText",
  ExtractProperty = "ExtractProperty",
  Request = "Request",
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
  requestBody?: string;
}

export type WorkflowNodeData =
  | (PrintTextNodeData & Record<string, unknown>)
  | (ExtractPropertyNodeData & Record<string, unknown>)
  | (RequestNodeData & Record<string, unknown>);

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

/* Node data factories */
export function createPrintTextNodeData(
  data?: Partial<Omit<PrintTextNodeData, "type">>,
): WorkflowNodeData {
  return {
    type: WorkflowNodeType.PrintText,
    text: "Printing text...",
    useConsole: false,
    ...data,
  };
}

export function createExtractPropertyNodeData(
  data?: Partial<Omit<ExtractPropertyNodeData, "type">>,
): WorkflowNodeData {
  return { type: WorkflowNodeType.ExtractProperty, ...data };
}

export function createRequestNodeData(
  data?: Partial<Omit<RequestNodeData, "type">>,
): WorkflowNodeData {
  return { type: WorkflowNodeType.Request, requestType: "GET", ...data };
}

export function createDefaultWorkflowNodeData(
  nodeType: WorkflowNodeType,
): WorkflowNodeData {
  switch (nodeType) {
    case WorkflowNodeType.PrintText:
      return createPrintTextNodeData();
    case WorkflowNodeType.ExtractProperty:
      return createExtractPropertyNodeData();
    case WorkflowNodeType.Request:
      return createRequestNodeData();
    default:
      throw new Error(`Unsupported WorkflowNode type '${nodeType}'`);
  }
}
