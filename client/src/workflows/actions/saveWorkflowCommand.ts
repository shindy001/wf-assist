import {
  postWfAssistWorkflowsByIdUpdateData,
  type WorkflowNodeDto,
} from "$api";
import { failed, successful, type WorkflowData } from "$lib/components/types";
import { throttle } from "lodash";

export function createSaveWorkflowCommand(saveRateLimitInMilliseconds: number) {
  return throttle(async (id: string, workflowData: WorkflowData) => {
    const result = await postWfAssistWorkflowsByIdUpdateData({
      path: {
        id: id,
      },
      body: {
        data: {
          nodes: workflowData?.nodes?.map(
            (x) => x as unknown as WorkflowNodeDto,
          ),
          edges: workflowData.edges,
        },
      },
    });
    return result.error ? failed(result.error.toString()) : successful();
  }, saveRateLimitInMilliseconds);
}
