import { postWfAssistWorkflowsByIdRename } from "$api";
import { failed, successful } from "$lib/components/types";

export function createRenameWorkflowCommand() {
  return async (workflowId: string, newName: string) => {
    {
      const result = await postWfAssistWorkflowsByIdRename({
        path: { id: workflowId },
        body: { newName: newName },
      });

      return result.error ? failed(result.error.toString()) : successful();
    }
  };
}
