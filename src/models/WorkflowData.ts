import type {FlowData} from "./FlowData";

export type ExecutionItem = {
    id: string;
    type: string;
    data?: Record<string, unknown>;
}

export interface WorkflowData {
    name: string;
    flowData: FlowData;
    executionList: Array<ExecutionItem>;
}