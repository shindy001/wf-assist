import {type ExecutionItem, FlowNodeType, type NodeExecutor} from "../types";
import {usePrintStringNodeExecutor} from "./nodes/printStringNodeExecutor";

const executorMap: Record<string, NodeExecutor<any>> = {
    [FlowNodeType.PrintString]: usePrintStringNodeExecutor(),
}

export function useWorkflowExecutor() {
    return {
        execute: (workflowName: string, executionList: Array<ExecutionItem>) => {
            const executionId = `${workflowName}_${new Date().toISOString()}`
            console.info(`Starting workflow '${executionId}'."`);

            for (const executionItem of executionList) {
                if (executionItem.nodeType) {
                    const executor: NodeExecutor<any> | undefined = executorMap[executionItem.nodeType];
                    if (!executor) {
                        console.error("Aborting workflow run, cannot find workflow executor for node type: " + executionItem.nodeType);
                        break;
                    }
                    executor.execute(executionId, executionItem.nodeData);
                } else {
                    console.error("Aborting workflow run, empty node type: " + executionItem.nodeType);
                }
            }

            console.info(`Workflow '${executionId}' ended."`);
        }
    }
}