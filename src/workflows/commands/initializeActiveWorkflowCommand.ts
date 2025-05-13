import {get, type Writable} from "svelte/store";
import {type AppData, setActiveWorkflow} from "../../lib/stores/appDataStore";
import {WorkflowDataService} from "../stores/workflowDataService";

export function createInitializeActiveWorkflowCommand(
    appDataStore: Writable<AppData>,
    workflowDataStore: WorkflowDataService
) {
    return async ()=> {
        const activeWorkflowName = get(appDataStore).activeWorkflowName;
        if (activeWorkflowName && (await workflowDataStore.workflowExists(activeWorkflowName))) {
            return;
        } else if ((await workflowDataStore.isEmpty())) {
            await addEmptyWorkflow();
            return;
        }
        else {
            await setDefaultActiveWorkflow();
        }
    }

    async function setDefaultActiveWorkflow() {
        const workflow = await workflowDataStore.getWorkflowById(1);
        setActiveWorkflow(workflow?.name ?? "")
    }

    async function addEmptyWorkflow() {
        const newEmptyWorkflowName = await workflowDataStore.addEmptyWorkflow();
        setActiveWorkflow(newEmptyWorkflowName);
    }
}