import {WorkflowDataService} from "../stores/workflowDataService";
import {AlreadyExistsError, NotFoundError} from "../../lib/types";

export function createRenameWorkflowCommand(
    workflowDataService: WorkflowDataService
) {
    return async (currentName: string, newName: string)=> {
        if (currentName === newName) {
            return;
        }

        if (await workflowDataService.workflowExists(newName)) {
            return new AlreadyExistsError();
        }

        const currentWorkflow = await workflowDataService.getWorkflow(currentName);
        if (!currentWorkflow) {
            return new NotFoundError();
        }

        return await workflowDataService.renameWorkflow(currentWorkflow.id, newName);
    }
}