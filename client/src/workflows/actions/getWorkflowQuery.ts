import { getWfAssistWorkflowsById } from "$api";
import { failed, successful } from "$lib/components/types";
import { toWorkflow } from "$lib/components/types/workflowMapper";

export function createGetWorkflowQuery() {
  return async (id: string) => {
    {
      const result = await getWfAssistWorkflowsById({ path: { id: id } });

      return result.error
        ? failed(result.error.toString())
        : successful(
            result.data?.item ? toWorkflow(result.data.item) : undefined,
          );
    }
  };
}
