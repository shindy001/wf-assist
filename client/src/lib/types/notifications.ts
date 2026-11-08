export type WorkflowExecutionEnded = {
  executionId: string;
  workflowId: string;
  workflowName: string;
  status: string;
};

export type WorkflowExecutionStarted = {
  executionId: string;
  workflowId: string;
  workflowName: string;
};
