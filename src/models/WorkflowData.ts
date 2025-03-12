import type {FlowData} from "./FlowData";

export enum ExecutionStatus {
    Waiting,
    Started,
    Succeeded,
    Failed
}

export type ExecutionItem = {
    id: string;
    type: string;
    data?: any;
}

export interface WorkflowData {
    flowData: FlowData;
    executionList: Array<ExecutionItem>;
}