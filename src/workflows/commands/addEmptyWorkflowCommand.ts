import {setActiveWorkflow} from "../../lib/stores/appDataStore";
import {WorkflowDataService} from "../stores/workflowDataService";

export function createAddEmptyWorkflowCommand(
    workflowDataService: WorkflowDataService
) {
    return async ()=> {
        const newWorkflowName = await workflowDataService.addEmptyWorkflow();
        setActiveWorkflow(newWorkflowName);
    }
}