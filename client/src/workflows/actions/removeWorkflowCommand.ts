import { deleteWfAssistWorkflowsById } from "$api";

export function createRemoveWorkflowCommand() {
  return async (workflowId: string) => {
    const result = await deleteWfAssistWorkflowsById({
      path: { id: workflowId },
    });

    if (result.error) {
      // TODO - consume with some error service and show message box or something like that???
      console.error(result.error);
    }
  };
}
