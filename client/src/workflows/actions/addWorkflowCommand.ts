import { postWfAssistWorkflows, type CreateWorkflowRequest } from "$api";
import type { WorkflowData } from "$lib/components/types";

export function createAddWorkflowCommand() {
  return async (workflowName: string, workflowData: WorkflowData) => {
    {
      workflowData;
      const requestData: CreateWorkflowRequest = {
        name: workflowName,
        data: {},
      };

      // const result = await postWfAssistWorkflows({ body: { name: workflowName, data: workflowData ?? {} } });
      //
      // if (result.error) {
      //     // TODO - consume with some error service and show message box or something like that???
      //     console.error(result.error);
      // }
    }
  };
}
