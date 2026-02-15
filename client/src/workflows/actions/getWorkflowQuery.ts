import { getApiWorkflowsById } from "$api";
import { failed, successful } from "$lib/types";
import { toWorkflow } from "$lib/types/workflowMapper";

export function createGetWorkflowQuery() {
  return async (id: string) => {
    {
      const result = await getApiWorkflowsById({ path: { id: id } });

      return result.error
        ? failed(result.error.toString())
        : successful(
            result.data?.item ? toWorkflow(result.data.item) : undefined,
          );
    }
  };
}
