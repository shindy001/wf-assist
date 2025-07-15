export function useWorkflowExecutor() {
    return {
        // TODO - maybe use workflowId ???
        execute: async (workflowName: string) => {
            console.info(`Starting workflow '${workflowName}'."`);

            // TODO call server to execute workflow and monitor progress

            console.info(`Workflow '${workflowName}' ended."`);
        }
    }
}