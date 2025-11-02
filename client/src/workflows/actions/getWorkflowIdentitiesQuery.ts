import { getWfAssistWorkflowsIdentities } from "$api";
import { failed, successful, type WorkflowIdentity } from "$lib/types";

export function createGetWorkflowIdentitiesQuery() {
  return async () => {
    {
      const result = await getWfAssistWorkflowsIdentities();

      return result.error
        ? failed(result.error.toString())
        : successful(
            result.data?.identities.map((x) => x as WorkflowIdentity) ?? [],
          );
    }
  };
}
