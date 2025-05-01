import {type WorkflowData} from "../models/WorkflowData";
import {FlowNodeType} from "../models/nodes/FlowNodeType";
import {usePrintStringNodeExecutor} from "./nodes/printStringNodeExecutor";
import type {NodeExecutor} from "../models/nodes/NodeExecutor";

const executorMap: Record<string, NodeExecutor<any>> = {
    [FlowNodeType.PrintString]: usePrintStringNodeExecutor(),
}

export function useWorkflowExecutor() {
    return {
        execute: (data: WorkflowData) => {
            console.info(`Starting workflow '${data.name}'."`);

            for (const executionItem of data.executionList) {
                if (executionItem.type) {
                    const executor: NodeExecutor<any> | undefined = executorMap[executionItem.type];
                    if (!executor) {
                        console.error("Aborting workflow run, cannot find workflow executor for node type: " + executionItem.type);
                        break;
                    }
                    executor.execute(executionItem.data);
                } else {
                    console.error("Aborting workflow run, empty node type: " + executionItem.type);
                }
            }

            console.info(`Workflow '${data.name}' ended."`);
        }
    }
}