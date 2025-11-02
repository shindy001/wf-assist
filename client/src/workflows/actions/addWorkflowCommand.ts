import { postWfAssistWorkflows, type CreateWorkflowRequest } from "$api";
import { failed, successful, type WorkflowData } from "$lib/types";
import { toWorkflowDataDto } from "$lib/types/workflowMapper";

export function createAddWorkflowCommand() {
  return async (workflowName: string, workflowData?: WorkflowData) => {
    {
      const requestData: CreateWorkflowRequest = {
        name: workflowName,
        data: workflowData ? toWorkflowDataDto(workflowData) : {},
      };

      const result = await postWfAssistWorkflows({ body: requestData });

      return result.error ? failed(result.error.toString()) : successful();
    }
  };
}
