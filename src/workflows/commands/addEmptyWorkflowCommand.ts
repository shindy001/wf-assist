import {WorkflowDataService} from "../stores/workflowDataService";
import type {AppState} from "../../lib/stores/appState.svelte";

export function createAddEmptyWorkflowCommand(
    appState: AppState,
    workflowDataService: WorkflowDataService
) {
    return async ()=> {
        appState.lastActiveWorkflowName = await workflowDataService.addEmptyWorkflow();
    }
}