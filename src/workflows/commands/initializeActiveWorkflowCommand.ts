import {get, type Writable} from "svelte/store";
import {type AppData, setActiveWorkflow} from "../../lib/stores/appDataStore";
import {WorkflowDataService} from "../stores/workflowDataService";

export function createInitializeActiveWorkflowCommand(
    appDataStore: Writable<AppData>,
    workflowDataService: WorkflowDataService
) {
    return async ()=> {
        const activeWorkflowName = get(appDataStore).activeWorkflowName;
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
        const workflow = await workflowDataService.getWorkflowById(1);
        setActiveWorkflow(workflow?.name ?? "")
    }

    async function addEmptyWorkflow() {
        const newEmptyWorkflowName = await workflowDataService.addEmptyWorkflow();
        setActiveWorkflow(newEmptyWorkflowName);
    }
}