export enum ExecutionStatus {
    Waiting,
    Started,
    Succeeded,
    Failed
}

export type ExecutionItem = {
    id: string;
    type?: string;
    data?: any;
    status: ExecutionStatus;
}

export interface WorkflowData {
    executionList: Array<ExecutionItem>;
    executingItem: string | undefined;
    status: ExecutionStatus;
    results: Record<string, {} | undefined>
}