import { postWfAssistWorkflowsByIdUpdateData } from "$api";
import { failed, successful, type WorkflowData } from "$lib/types";
import { toWorkflowDataDto } from "$lib/types/workflowMapper";
import { throttle } from "lodash";

export function createSaveWorkflowCommand(saveRateLimitInMilliseconds: number) {
  return throttle(async (id: string, workflowData: WorkflowData) => {
    const result = await postWfAssistWorkflowsByIdUpdateData({
      path: {
        id: id,
      },
      body: { data: toWorkflowDataDto(workflowData) },
    });

    return result.error ? failed(result.error.toString()) : successful();
  }, saveRateLimitInMilliseconds);
}
