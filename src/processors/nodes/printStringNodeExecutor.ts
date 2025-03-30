import type {NodeExecutor} from "../../models/nodes/NodeExecutor";
import type {PrintStringNode} from "../../models/nodes/PrintStringNode";

export function usePrintStringNodeExecutor(): NodeExecutor<PrintStringNode> {
    return {
        // TODO - simplify and clean processing logic
        execute: (node: PrintStringNode) => {
            if (node.targetId) {
                // TODO
                // 1. Get data from executionStore
                // 2. Get result from executed node node.targetId
                // 3. Print result if there is any
                const targetResult = "Nothing to print.";

                if (node.useLogger) {
                    console.log(targetResult);
                } else {
                    // TODO - print somewhere else???
                }
            }
        }
    }
}