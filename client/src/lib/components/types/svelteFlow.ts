import {
  type ExtractPropertyNodeData,
  type PrintTextNodeData,
  type RequestNodeData,
  type WorkflowNodeData,
  WorkflowNodeType,
} from "$lib/components/types/workflow";

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
