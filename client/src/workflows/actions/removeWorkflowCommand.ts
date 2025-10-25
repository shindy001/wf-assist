import { deleteWfAssistWorkflowsById } from "$api";
import { failed, successful } from "$lib/components/types";

export function createRemoveWorkflowCommand() {
  return async (workflowId: string) => {
    const result = await deleteWfAssistWorkflowsById({
      path: { id: workflowId },
    });

    return result.error ? failed(result.error.toString()) : successful();
  };
}
