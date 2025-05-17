import {WorkflowDataService} from "../stores/workflowDataService";
import type {AppState} from "../../lib/stores/appState.svelte";

export function createInitializeActiveWorkflowCommand(
    appState: AppState,
    workflowDataService: WorkflowDataService
) {
    return async ()=> {
        const activeWorkflowName = appState.lastActiveWorkflowName;
        if (activeWorkflowName && (await workflowDataService.workflowExists(activeWorkflowName))) {
            return;
        } else if ((await workflowDataService.isEmpty())) {
            await addEmptyWorkflow();
            return;
        }
        else {
            await setDefaultActiveWorkflow();
        }
    }

    async function setDefaultActiveWorkflow() {
        const workflow = await workflowDataService.getLastWorkflow();
        appState.lastActiveWorkflowName = workflow?.name ?? "";
    }

    async function addEmptyWorkflow() {
        appState.lastActiveWorkflowName = await workflowDataService.addEmptyWorkflow();
    }
}