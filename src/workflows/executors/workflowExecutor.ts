import {type ExecutionItem, FlowNodeType, type NodeExecutor} from "../types";
import {usePrintStringNodeExecutor} from "./nodes/printStringNodeExecutor";

const executorMap: Record<string, NodeExecutor<any>> = {
    [FlowNodeType.PrintString]: usePrintStringNodeExecutor(),
}

export function useWorkflowExecutor() {
    return {
        execute: (workflowName: string, executionList: Array<ExecutionItem>) => {
            console.info(`Starting workflow '${workflowName}'."`);

            for (const executionItem of executionList) {
                if (executionItem.nodeType) {
                    const executor: NodeExecutor<any> | undefined = executorMap[executionItem.nodeType];
                    if (!executor) {
                        console.error("Aborting workflow run, cannot find workflow executor for node type: " + executionItem.nodeType);
                        break;
                    }
                    executor.execute(executionItem.nodeData);
                } else {
                    console.error("Aborting workflow run, empty node type: " + executionItem.nodeType);
                }
            }

            console.info(`Workflow '${workflowName}' ended."`);
        }
    }
}