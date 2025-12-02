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

export type WorkflowNodeData = RequestNodeData | HeadersNodeData;

export type WorkflowNodeDataBase = {
  type: WorkflowNodeDataType;
};

export enum RequestType {
  Get = "Get",
  Post = "Post",
  Put = "Put",
  Patch = "Patch",
  Delete = "Delete",
}

export type RequestNodeData = {
  url: string;
  requestType: RequestType;
  requestBody?: string;
} & WorkflowNodeDataBase;

export type HeadersNodeData = {
  headers: Array<HttpHeader>;
} & WorkflowNodeDataBase;

export type HttpHeader = {
  name: string;
  value: string;
};

export type Position = {
  x: number;
  y: number;
};

// These enum values (except Default) needs to be exact match to the values on server as they are mapped in "workflowMapper.ts"
export enum WorkflowNodeDataType {
  Default = "Default",
  Request = "Request",
  Headers = "Headers",
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
    requestType: RequestType.Get,
    ...data,
  };
}

function createHeadersNodeData(
  data?: Partial<Omit<HeadersNodeData, "type">>,
): WorkflowNodeData {
  return {
    type: WorkflowNodeDataType.Headers,
    headers: [],
    ...data,
  };
}

export function createDefaultWorkflowNodeData(
  nodeType: WorkflowNodeDataType,
): WorkflowNodeData {
  switch (nodeType) {
    case WorkflowNodeDataType.Request:
      return createRequestNodeData();
    case WorkflowNodeDataType.Headers:
      return createHeadersNodeData();
    default:
      throw new Error(`Unsupported WorkflowNode type '${nodeType}'`);
  }
}
