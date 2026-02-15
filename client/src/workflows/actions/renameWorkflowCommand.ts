import { postApiWorkflowsByIdRename } from "$api";
import { failed, successful } from "$lib/types";

export function createRenameWorkflowCommand() {
  return async (workflowId: string, newName: string) => {
    {
      const result = await postApiWorkflowsByIdRename({
        path: { id: workflowId },
        body: { newName: newName },
      });

      return result.error ? failed(result.error.toString()) : successful();
    }
  };
}
