import type {Edge, Node} from "@xyflow/svelte";

export interface FlowData {
    nodes: Node[],
    edges: Edge[],
}

export type ExecutionItem = {
    id: string;
    type: string;
    data?: Record<string, unknown>;
}

export interface WorkflowData {
    id: number;
    name: string;
    flowData: FlowData;
    executionList: Array<ExecutionItem>;
}

export interface WorkflowDataInput {
    name: string;
    flowData: FlowData;
    executionList: Array<ExecutionItem>;
}

export interface NodeExecutor<T> {
    execute: (node: T) => void;
}

/**
 * Custom node types
 * @link {https://reactflow.dev/api-reference/types/node#default-node-types}
 */
export enum FlowNodeType {
    Request = "Request",
    ExtractProperty = "Extract Property",
    PrintString = "Print String",
}

export interface ExtractPropertyNode extends Record<string, unknown> {
    path?: string;
    targetId?: string;
}

export interface PrintStringNode extends Record<string, unknown> {
    useLogger: boolean;
    targetId?: string;
}

export interface RequestNode extends Record<string, unknown> {
    url?: string,
    requestType?: string,
    requestBody?: string,
}
