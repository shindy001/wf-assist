import {
  postWfAssistWorkflows,
  type CreateWorkflowRequest,
  type WorkflowNodeDto,
} from "$api";
import { failed, successful, type WorkflowData } from "$lib/components/types";

export function createAddWorkflowCommand() {
  return async (workflowName: string, workflowData?: WorkflowData) => {
    {
      const requestData: CreateWorkflowRequest = {
        name: workflowName,
        data: {
          nodes: workflowData?.nodes?.map(
            (x) => x as unknown as WorkflowNodeDto,
          ),
          edges: workflowData?.edges,
        },
      };

      const result = await postWfAssistWorkflows({ body: requestData });

      return result.error ? failed(result.error.toString()) : successful();
    }
  };
}
