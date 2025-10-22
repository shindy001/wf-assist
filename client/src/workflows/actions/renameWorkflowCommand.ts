import {postWfAssistWorkflowsByIdRename} from "$api";

export function createRenameWorkflowCommand() {
    return async (workflowId: string, newName: string) => {
        {
            const result = await postWfAssistWorkflowsByIdRename({ path: { id: workflowId }, body: { newName: newName } });

            if (result.error) {
                // TODO - consume with some error service and show message box or something like that???
                console.error(result.error);
            }
        }
    }
}