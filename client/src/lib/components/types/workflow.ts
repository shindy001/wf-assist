export interface Workflow {
  id: string;
  name: string;
  data: WorkflowData;
}

export interface WorkflowData {
  nodes: Array<WorkflowNode>;
  edges: Array<WorkflowEdge>;
}

export interface WorkflowEdge {
  id: string;
  source: string;
  target: string;
}

export interface WorkflowNode {
  id: string;
  position: Position;
  data: WorkflowNodeData;
}

// SvelteFlow node contract (needs type prop at the top level)
export interface SvelteFlowWorkflowNode
  extends WorkflowNode,
    WorkflowNodeDataBase {}

// SvelteFlow edge contract
export interface SvelteFlowWorkflowEdge extends WorkflowEdge {}

export type WorkflowNodeData =
  | PrintTextNodeData
  | ExtractPropertyNodeData
  | RequestNodeData;

export type WorkflowNodeDataBase = {
  type: WorkflowNodeDataType;
};

export type PrintTextNodeData = {
  targetId?: string;
  text: string;
  useConsole: boolean;
} & WorkflowNodeDataBase;

export type ExtractPropertyNodeData = {
  path: string;
  targetId: string;
} & WorkflowNodeDataBase;

export type RequestNodeData = {
  url: string;
  requestType: string;
  requestBody?: string;
} & WorkflowNodeDataBase;

export interface Position {
  x: number;
  y: number;
}

// These enum values (except Default) needs to be exact match to the values on server as they are mapped in "workflowMapper.ts"
export enum WorkflowNodeDataType {
  Default = "Default",
  PrintText = "PrintText",
  ExtractProperty = "ExtractProperty",
  Request = "Request",
}

export enum WorkflowDataState {
  Uninitialized = "Uninitialized",
  Initialized = "Initialized",
  ReadyToChange = "ReadyToChange",
}

export interface WorkflowIdentity {
  id: string;
  name: string;
}

/* Node data factories */
function createPrintTextNodeData(
  data?: Partial<Omit<PrintTextNodeData, "type">>,
): WorkflowNodeData {
  return {
    type: WorkflowNodeDataType.PrintText,
    text: "Printing text...",
    useConsole: false,
    ...data,
  };
}

function createExtractPropertyNodeData(
  data?: Omit<ExtractPropertyNodeData, "type">,
): WorkflowNodeData {
  return {
    type: WorkflowNodeDataType.ExtractProperty,
    path: "",
    targetId: "",
    ...data,
  };
}

function createRequestNodeData(
  data?: Partial<Omit<RequestNodeData, "type">>,
): WorkflowNodeData {
  return {
    type: WorkflowNodeDataType.Request,
    url: "",
    requestType: "GET",
    ...data,
  };
}

export function createDefaultWorkflowNodeData(
  nodeType: WorkflowNodeDataType,
): WorkflowNodeData {
  switch (nodeType) {
    case WorkflowNodeDataType.PrintText:
      return createPrintTextNodeData();
    case WorkflowNodeDataType.ExtractProperty:
      return createExtractPropertyNodeData();
    case WorkflowNodeDataType.Request:
      return createRequestNodeData();
    default:
      throw new Error(`Unsupported WorkflowNode type '${nodeType}'`);
  }
}
