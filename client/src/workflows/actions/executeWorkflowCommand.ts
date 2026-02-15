import { postApiWorkflowsByIdQueueRun } from "$api";
import { failed, successful } from "$lib/types";

export function createExecuteWorkflowCommand() {
  return async (workflowId: string) => {
    {
      const result = await postApiWorkflowsByIdQueueRun({
        path: { id: workflowId },
      });

      return result.error
        ? failed(result.error.toString())
        : successful({ executionId: result.data!.runId });
    }
  };
}
