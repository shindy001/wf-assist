import {useWorkflowDataStore} from "../../stores/workflowDataStore";
import type {NodeProcessor} from "../../models/NodeProcessor";

export interface PrintStringNode {
    useLogger: boolean;
    targetId: string | undefined;
}

export function usePrintStringNodeProcessor(): NodeProcessor<PrintStringNode> {
    return {
        // TODO - simplify and clean processing logic
        process: (node: PrintStringNode) => {
            if (node.targetId) {
                const workflowDataStore = useWorkflowDataStore();
                const workflowData = workflowDataStore.getData();
                const targetResult = workflowData.results[node.targetId] ?? "Nothing to print.";

                if (node.useLogger) {
                    console.log(targetResult);
                } else {
                    // TODO - print somewhere else???
                }
            }
        }
    }
}