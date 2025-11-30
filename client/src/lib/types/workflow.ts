export type Workflow = {
  id: string;
  name: string;
  data: WorkflowData;
};

export type WorkflowData = {
  nodes: Array<WorkflowNode>;
  edges: Array<WorkflowEdge>;
};

export type WorkflowEdge = {
  id: string;
  source: string;
  target: string;
};

export type WorkflowNode = {
  id: string;
  position: Position;
  data: WorkflowNodeData;
};

// SvelteFlow node (needs type prop at the top level)
export type SvelteFlowWorkflowNode = WorkflowNode &
  Pick<WorkflowNodeData, "type">;

// SvelteFlow edge
export type SvelteFlowWorkflowEdge = WorkflowEdge;

export type WorkflowNodeData = RequestNodeData;

export type WorkflowNodeDataBase = {
  type: WorkflowNodeDataType;
};

export type RequestType = "Get" | "Post" | "Put" | "Patch" | "Delete";

export type RequestNodeData = {
  url: string;
  requestType: RequestType;
  requestBody?: string;
} & WorkflowNodeDataBase;

export type Position = {
  x: number;
  y: number;
};

// These enum values (except Default) needs to be exact match to the values on server as they are mapped in "workflowMapper.ts"
export enum WorkflowNodeDataType {
  Default = "Default",
  Request = "Request",
}

export enum WorkflowDataState {
  Uninitialized = "Uninitialized",
  Initialized = "Initialized",
  ReadyToChange = "ReadyToChange",
}

export type WorkflowIdentity = {
  id: string;
  name: string;
};

/* Node data factories */
function createRequestNodeData(
  data?: Partial<Omit<RequestNodeData, "type">>,
): WorkflowNodeData {
  return {
    type: WorkflowNodeDataType.Request,
    url: "",
    requestType: "Get",
    ...data,
  };
}

export function createDefaultWorkflowNodeData(
  nodeType: WorkflowNodeDataType,
): WorkflowNodeData {
  switch (nodeType) {
    case WorkflowNodeDataType.Request:
      return createRequestNodeData();
    default:
      throw new Error(`Unsupported WorkflowNode type '${nodeType}'`);
  }
}
