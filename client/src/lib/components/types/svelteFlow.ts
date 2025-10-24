import {
  type ExtractPropertyNodeData,
  type PrintTextNodeData,
  type RequestNodeData,
  type WorkflowNodeData,
  WorkflowNodeType,
} from "$lib/components/types/workflow";

export type SvelteFlowPrintTextNodeData = PrintTextNodeData &
  Record<string, unknown>;
export type SvelteFlowExtractPropertyNodeData = ExtractPropertyNodeData &
  Record<string, unknown>;
export type SvelteFlowRequestNodeData = RequestNodeData &
  Record<string, unknown>;

/* Node data factories */
export function createSvelteFlowPrintTextNodeData(
  data?: Partial<Omit<PrintTextNodeData, "type">>,
): SvelteFlowPrintTextNodeData {
  return {
    type: WorkflowNodeType.PrintText,
    text: "Printing text...",
    useConsole: false,
    ...data,
  };
}

export function createSvelteFlowExtractPropertyNodeData(
  data?: Partial<Omit<ExtractPropertyNodeData, "type">>,
): SvelteFlowExtractPropertyNodeData {
  return { type: WorkflowNodeType.ExtractProperty, ...data };
}

export function createSvelteFlowRequestNodeData(
  data?: Partial<Omit<RequestNodeData, "type">>,
): SvelteFlowRequestNodeData {
  return { type: WorkflowNodeType.Request, requestType: "GET", ...data };
}

export function createWorkflowNodeData(
  nodeType: WorkflowNodeType,
): WorkflowNodeData {
  switch (nodeType) {
    case WorkflowNodeType.PrintText:
      return createSvelteFlowPrintTextNodeData();
    case WorkflowNodeType.ExtractProperty:
      return createSvelteFlowExtractPropertyNodeData();
    case WorkflowNodeType.Request:
      return createSvelteFlowRequestNodeData();
    default:
      throw new Error(`Unsupported WorkflowNode type '${nodeType}'`);
  }
}
