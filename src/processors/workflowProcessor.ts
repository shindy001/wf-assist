import {type WorkflowData} from "../models/WorkflowData";
import {FlowNodeType} from "../models/nodes/FlowNodeType";
import {usePrintStringNodeProcessor} from "./nodes/printStringNodeProcessor";
import type {NodeProcessor} from "../models/nodes/NodeProcessor";

const processorMap: Record<string, NodeProcessor<any>> = {
    [FlowNodeType.PrintString]: usePrintStringNodeProcessor(),
}

export function useWorkflowProcessor() {
    return {
        process: (data: WorkflowData) => {
            console.info(`Starting workflow '${data.name}'."`);

            for (const executionItem of data.executionList) {
                if (executionItem.type) {
                    const processor: NodeProcessor<any> | undefined = processorMap[executionItem.type];
                    if (!processor) {
                        console.error("Aborting workflow run, cannot find workflow processor for node type: " + executionItem.type);
                        break;
                    }
                    processor.process(executionItem.data);
                } else {
                    console.error("Aborting workflow run, empty node type: " + executionItem.type);
                }
            }

            console.info(`Workflow '${data.name}' ended."`);
        }
    }
}