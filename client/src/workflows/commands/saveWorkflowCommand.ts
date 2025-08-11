import {throttle} from "lodash";

export function createSaveWorkflowCommand(
    saveRateLimitInMilliseconds: number
) {
    return throttle(async (workflowData: unknown) => {
                // TODO - send save request to server
        }, saveRateLimitInMilliseconds);
}