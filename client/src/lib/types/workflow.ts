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
  type: WorkflowNodeType;
  position: Position;
  data: Record<string, unknown>;
};

// These enum values (except Default) needs to be exact match to the values on server as they are mapped in "workflowMapper.ts"
export enum WorkflowNodeType {
  RequestNode = "RequestNode",
  HeadersNode = "HeadersNode",
}

export type RequestNode = {
  url: string;
  requestType: RequestType;
  requestBody?: string;
} & WorkflowNode;

export type HeadersNode = {
  headers: Array<HttpHeader>;
} & WorkflowNode;

export type HttpHeader = {
  name: string;
  value: string;
};

export type Position = {
  x: number;
  y: number;
};

export enum RequestType {
  Get = "Get",
  Post = "Post",
  Put = "Put",
  Patch = "Patch",
  Delete = "Delete",
}

export type WorkflowIdentity = {
  id: string;
  name: string;
};

/* Node data factories */
function createRequestNode(
  data?: Partial<Omit<RequestNode, "type">>,
): RequestNode {
  return {
    id: "",
    type: WorkflowNodeType.RequestNode,
    position: {x: 0, y: 0},
    url: "",
    requestType: RequestType.Get,
    ...data,
  };
}

function createHeadersNode(
  data?: Partial<Omit<HeadersNode, "type">>,
): HeadersNode {
  return {
    id: "",
    type: WorkflowNodeType.HeadersNode,
    position: {x: 0, y: 0},
    headers: [],
    ...data,
  };
}

export function createWorkflowNode(
  nodeType: WorkflowNodeType,
  position?: Position
): RequestNode | HeadersNode {
  switch (nodeType) {
    case WorkflowNodeType.RequestNode:
      return createRequestNode({position: position});
    case WorkflowNodeType.HeadersNode:
      return createHeadersNode({position: position});
    default:
      throw new Error(`Unsupported node type '${nodeType}'`);
  }
}
