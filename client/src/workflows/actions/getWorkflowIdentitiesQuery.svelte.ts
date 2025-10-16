import {getWfAssistWorkflowsIdentities} from "$api";
import type {WorkflowIdentity} from "$lib/components/types";

export function createGetWorkflowIdentitiesQuery() {
    return async () => {
        {
            const result = await getWfAssistWorkflowsIdentities();

            if (result.error) {
                // TODO - consume with some error service and show message box or something like that???
                console.error(result.error);
            }

            return {
                identities: result.data?.identities.map(x => x as WorkflowIdentity) ?? []
            };
        }
    }
}