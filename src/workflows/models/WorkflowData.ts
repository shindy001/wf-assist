import type {FlowData} from "./FlowData";

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