import {throttle} from "lodash";
import type {WorkflowData} from "../types";

export function createSaveWorkflowCommand(
    saveRateLimitInMilliseconds: number
) {
    return throttle(async (workflowData: WorkflowData) => {
                // TODO - send save request to server
        }, saveRateLimitInMilliseconds);
}