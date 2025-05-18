import type {NodeExecutor, PrintStringNode} from "../../types";
import type {ResultsDataService} from "../../stores/resultsDataService";
import {isObject} from "lodash";

export function usePrintStringNodeExecutor(resultDataService: ResultsDataService): NodeExecutor<PrintStringNode> {
    return {
        execute: async (executionId: string, node: PrintStringNode) => {
            if (node.targetId) {
                const resultData = await resultDataService.getResults(executionId);
                const targetNodeResults = resultData?.data[node.targetId];

                console.log(isObject(targetNodeResults)
                    ? JSON.stringify(targetNodeResults)
                    : targetNodeResults ?? "Nothing to print");
            } else {
                console.warn(`PrintStringNode ${node.id} does not have targetId set, try to set edge to some existing node.`);
            }
        }
    }
}