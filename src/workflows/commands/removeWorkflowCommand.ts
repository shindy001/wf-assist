import {WorkflowDataService} from "../stores/workflowDataService";
import type {AppState} from "../../lib/stores/appState.svelte";

export function createRemoveWorkflowCommand(
    appState: AppState,
    workflowDataService: WorkflowDataService
) {
    return async (workflowName: string)=> {
        const removingActiveWorkflow = workflowName === appState.lastActiveWorkflowName;
        await workflowDataService.deleteWorkflow(workflowName);

        if (removingActiveWorkflow) {
            await setNextActiveWorkflow();
        }
    }

    async function setNextActiveWorkflow() {
        let nextWorkflowName = (await workflowDataService.getLastWorkflow())?.name;
        if (!nextWorkflowName) {
            const nextWorkflowName = await workflowDataService.addEmptyWorkflow();
        }

        appState.lastActiveWorkflowName = nextWorkflowName ?? "";
    }
}