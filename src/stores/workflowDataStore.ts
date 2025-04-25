import type {WorkflowData, WorkflowDataInput} from "../models/WorkflowData";
import {db, executeDbOperation} from "./db";
import {failure, type Result, success} from "../models/Types/Result";
import {ErrorDetail} from "../models/Types/ErrorDetail";
import {liveQuery} from "dexie";

const workflowDataError = (message: string) => new ErrorDetail("WorkflowDataError", message);
const alreadyExistsError = (item: string) => workflowDataError(`${item} already exists.`);
const notFoundError = (item: string) => workflowDataError(`${item} not found.`);
const fatalError = (message: string) => workflowDataError(`Fatal: ${message}`);

export function useWorkflowDataStore() {
    /**
     * Collection with all workflow names
     * Note: Returns observable => collection is updated on any addition/deletion.
     */
    let workflowIdentities = liveQuery(
        async () => await executeDbOperation(db.workflows.toArray()
            .then((result: WorkflowData[]) => result.map(x => {
                return {id: x.id, name: x.name}
            })))
    );

    const workflowExists = async (name: string): Promise<Result<boolean>> => {
        try {
            const existingItem = await executeDbOperation(db.workflows.where('name').equalsIgnoreCase(name).first());
            return success(existingItem !== undefined);
        } catch (error) {
            return failure(errorDetail(error));
        }
    }

    const getWorkflow = async (name: string): Promise<Result<WorkflowData | undefined>> => {
        try {
            return success(await executeDbOperation(db.workflows.where("name").equalsIgnoreCase(name).first()));
        } catch (error) {
            return failure(errorDetail(error));
        }
    }

    const addWorkflow = async (data: WorkflowDataInput): Promise<Result> => {
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

    const deleteWorkflow = async (name: string): Promise<Result> => {
        try {
            await executeDbOperation(db.workflows.where("name").equals(name).delete());
            return success(undefined);
        } catch (error) {
            return failure(errorDetail(error));
        }
    }

    const renameWorkflow = async (id: number, newName: string): Promise<Result> => {
        try {
            const result = await executeDbOperation(db.workflows.where("id").equals(id).modify(data => {
                data.name = newName;
            }));
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
        workflowIdentities,
        workflowExists,
        getWorkflow,
        addWorkflow,
        updateWorkflow,
        deleteWorkflow,
        renameWorkflow,
    }
}