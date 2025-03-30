import {isBrowser} from "../utils/platformUtils";
import type {WorkflowData} from "../models/WorkflowData";
import {db, executeDbOperation} from "./db";

export enum WorkflowDataStoreError {
    AlreadyExists,
    DoesNotExist,
    Fatal
}

export type WorkflowDataError = {
    error: WorkflowDataStoreError;
    message: string;
}

export function useWorkflowDataStore() {
    if (!isBrowser()) {
        throw new Error("WorkflowDataStore cannot be used outside of a browser");
    }

    const workflowExists = async (name: string): Promise<boolean | WorkflowDataError> => {
        try {
            const existingItem = await executeDbOperation(db.workflows.get(name));
            return existingItem !== undefined;
        } catch (error) {
            return handleError(error);
        }
    }

    const getWorkflow = async (name: string): Promise<WorkflowData | undefined | WorkflowDataError> => {
        try {
            return await executeDbOperation(db.workflows.get(name));
        } catch (error) {
            return handleError(error);
        }
    }

    const addWorkflow = async (data: WorkflowData): Promise<void | WorkflowDataError> => {
        try {
            if (await workflowExists(data.name)) {
                return {
                    error: WorkflowDataStoreError.AlreadyExists,
                    message: `Workflow '${data.name}' already exists.`
                };
            }
            await executeDbOperation(db.workflows.add(data));
        } catch (error) {
            return handleError(error);
        }
    }

    const updateWorkflow = async (data: WorkflowData): Promise<void | WorkflowDataError> => {
        try {
            const itemExists = await workflowExists(data.name);
            if (!itemExists) {
                return {
                    error: WorkflowDataStoreError.DoesNotExist,
                    message: `Workflow '${data.name}' does not exist.`
                };
            }
            await executeDbOperation(db.workflows.put(data));
        } catch (error) {
            return handleError(error);
        }
    }

    const addOrUpdateWorkflow = async (data: WorkflowData): Promise<void | WorkflowDataError> => {
        try {
            await executeDbOperation(db.workflows.put(data));
        } catch (error) {
            return handleError(error);
        }
    }

    const deleteWorkflow = async (name: string): Promise<void | WorkflowDataError> => {
        try {
            const itemExists = await workflowExists(name);
            if (itemExists) {
                await db.workflows.delete(name);
            }
        } catch (error) {
            return handleError(error);
        }
    }

    const handleError = (error: unknown): WorkflowDataError => {
        const message = error instanceof Error ? error.message : 'Unknown error';
        return {error: WorkflowDataStoreError.Fatal, message: message};
    };

    return {
        workflowExists,
        getWorkflow,
        addWorkflow,
        updateWorkflow,
        addOrUpdateWorkflow,
        deleteWorkflow,
    }
}