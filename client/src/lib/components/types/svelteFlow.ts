import {
    type ExtractPropertyNodeData,
    type PrintTextNodeData,
    type RequestNodeData,
    WorkflowNodeType
} from "$lib/components/types/workflow";

export type SvelteFlowPrintTextNodeData = PrintTextNodeData & Record<string, unknown>;
export type SvelteFlowExtractPropertyNodeData = ExtractPropertyNodeData & Record<string, unknown>;
export type SvelteFlowRequestNodeData = RequestNodeData & Record<string, unknown>;

/* Node data factories */
export function createSvelteFlowPrintTextNodeData(
    data?: Partial<Omit<PrintTextNodeData, "type">>
): SvelteFlowPrintTextNodeData {
    return { type: WorkflowNodeType.PrintText, text: "Printing text...", useConsole: false, ...data }
}

export function createSvelteFlowExtractPropertyNodeData(
    data?: Partial<Omit<ExtractPropertyNodeData, "type">>
): SvelteFlowExtractPropertyNodeData {
    return { type: WorkflowNodeType.ExtractProperty, ...data }
}

export function createSvelteFlowRequestNodeData(
    data?: Partial<Omit<RequestNodeData, "type">>
): SvelteFlowRequestNodeData {
    return { type: WorkflowNodeType.Request, requestType: "GET", ...data }
}
