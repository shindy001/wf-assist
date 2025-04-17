import {isBrowser} from "../utils/platformUtils";
import type {WorkflowData} from "../models/WorkflowData";
import {db, executeDbOperation} from "./db";
import {failure, type Result, success} from "../models/Types/Result";
import {ErrorDetail} from "../models/Types/ErrorDetail";
import {liveQuery} from "dexie";

const workflowDataError = (message: string) => new ErrorDetail("WorkflowDataError", message);
const alreadyExistsError = (item: string) => workflowDataError(`${item} already exists.`);
const notFoundError = (item: string) => workflowDataError(`${item} not found.`);
const fatalError = (message: string) => workflowDataError(`Fatal: ${message}`);

export function useWorkflowDataStore() {
    if (!isBrowser()) {
        throw new Error("WorkflowDataStore cannot be used outside of a browser");
    }

    /**
     * Collection with all workflow names
     * Note: Returns observable => collection is updated on any addition/deletion.
     */
    let workflowNames = liveQuery(
        async () => await executeDbOperation(db.workflows.toCollection().primaryKeys())
    );

    const workflowExists = async (name: string): Promise<Result<boolean>> => {
        try {
            const existingItem = await executeDbOperation(db.workflows.get(name));
            return success(existingItem !== undefined);
        } catch (error) {
            return failure(errorDetail(error));
        }
    }

    const getWorkflow = async (name: string): Promise<Result<WorkflowData | undefined>> => {
        try {
            return success(await executeDbOperation(db.workflows.get(name)));
        } catch (error) {
            return failure(errorDetail(error));
        }
    }

    const addWorkflow = async (data: WorkflowData): Promise<Result> => {
        try {
            if ((await workflowExists(data.name)).data === true) {
                return failure(alreadyExistsError(data.name));
            }
            await executeDbOperation(db.workflows.add(data));
            return success(undefined);
        } catch (error) {
            return failure(errorDetail(error));
        }
    }

    const updateWorkflow = async (data: WorkflowData): Promise<Result> => {
        try {
            const itemExists = (await workflowExists(data.name)).data;
            if (!itemExists) {
                return failure(notFoundError(data.name));
            }

            await executeDbOperation(db.workflows.put(data));
            return success(undefined);
        } catch (error) {
            return failure(errorDetail(error));
        }
    }

    const addOrUpdateWorkflow = async (data: WorkflowData): Promise<Result> => {
        try {
            await executeDbOperation(db.workflows.put(data));
            return success(undefined);
        } catch (error) {
            return failure(errorDetail(error));
        }
    }

    const deleteWorkflow = async (name: string): Promise<Result> => {
        try {
            await db.workflows.delete(name);
            return success(undefined);
        } catch (error) {
            return failure(errorDetail(error));
        }
    }

    const errorDetail = (error: unknown) => {
        const message = error instanceof Error ? error.message : 'Unknown error';
        return fatalError(message);
    };

    return {
        workflowNames,
        workflowExists,
        getWorkflow,
        addWorkflow,
        updateWorkflow,
        addOrUpdateWorkflow,
        deleteWorkflow,
    }
}