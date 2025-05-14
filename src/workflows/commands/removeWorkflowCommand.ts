import {type AppData, setActiveWorkflow} from "../../lib/stores/appDataStore";
import {WorkflowDataService} from "../stores/workflowDataService";
import {get, type Writable} from "svelte/store";

export function createRemoveWorkflowCommand(
    appDataStore: Writable<AppData>,
    workflowDataService: WorkflowDataService
) {
    return async (workflowName: string)=> {
        const removingActiveWorkflow = workflowName === get(appDataStore).activeWorkflowName;
        await workflowDataService.deleteWorkflow(workflowName);

        if (removingActiveWorkflow) {
            await setNextActiveWorkflow();
        }
    }

    async function setNextActiveWorkflow() {
        let nextWorkflow = (await workflowDataService.getWorkflowById(1))?.name;
        if (!nextWorkflow) {
            nextWorkflow = await workflowDataService.addEmptyWorkflow();
        }
        setActiveWorkflow(nextWorkflow);
    }
}