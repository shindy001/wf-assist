import { deleteApiWorkflowsById } from "$api";
import { failed, successful } from "$lib/types";

export function createRemoveWorkflowCommand() {
  return async (workflowId: string) => {
    const result = await deleteApiWorkflowsById({
      path: { id: workflowId },
    });

    return result.error ? failed(result.error.toString()) : successful();
  };
}
