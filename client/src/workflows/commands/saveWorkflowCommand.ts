import {WorkflowDataService} from "../stores/workflowDataService";
import {throttle} from "lodash";
import type {WorkflowData} from "../types";

export function createSaveWorkflowCommand(
    workflowDataService: WorkflowDataService,
    saveRateLimitInMilliseconds: number
) {
    return throttle(async (workflowData: WorkflowData) => {
                await workflowDataService.updateWorkflow(workflowData);
        }, saveRateLimitInMilliseconds);
}