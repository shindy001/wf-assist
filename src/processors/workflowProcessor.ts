import {ExecutionStatus, type WorkflowData} from "../models/WorkflowData";
import {FlowNodeType} from "../models/nodes/FlowNodeType";
import {usePrintStringNodeProcessor} from "./nodes/printStringNodeProcessor";
import type {NodeProcessor} from "../models/nodes/NodeProcessor";

const processorMap: Record<string, NodeProcessor<any>> = {
    [FlowNodeType.PrintString]: usePrintStringNodeProcessor(),
}

export function useWorkflowProcessor() {
    return {
        // TODO - simplify and clean processing logic
        process: (data: WorkflowData) => {
            if (data.status !== ExecutionStatus.Waiting) {
                // TODO - return or throw error???
                console.error("Cannot start workflow, that is not in waiting state, workflow stat: " + data.status);
                return;
            }

            data.status = ExecutionStatus.Started;

            for (const executionItem of data.executionList) {
                if (executionItem.type) {
                    const processor: NodeProcessor<any> | undefined = processorMap[executionItem.type];
                    if (!processor) {
                        console.error("Aborting workflow run, cannot find workflow processor for node type: " + executionItem.type);
                        executionItem.status = ExecutionStatus.Failed;
                        //data.status = ExecutionStatus.Failed;
                        //break;
                        continue;
                    }
                    processor.process(executionItem.data);
                    executionItem.status = ExecutionStatus.Succeeded;
                } else {
                    console.error("Aborting workflow run, empty node type: " + executionItem.type);
                    data.status = ExecutionStatus.Failed;
                }
            }

            data.status = ExecutionStatus.Succeeded;
            return data;
        }
    }
}