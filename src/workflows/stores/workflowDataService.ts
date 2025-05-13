import type {WorkflowData, WorkflowDataInput} from "../types";
import {ErrorDetail, failure, type Result, success} from "../../lib/types";
import {db, executeDbOperation} from "../../lib/stores/db";
import {liveQuery} from "dexie";

const workflowDataError = (message: string) => new ErrorDetail("WorkflowDataError", message);
const alreadyExistsError = (item: string) => workflowDataError(`${item} already exists.`);
const notFoundError = (item: string) => workflowDataError(`${item} not found.`);
const fatalError = (message: string) => workflowDataError(`Fatal: ${message}`);

export class WorkflowDataService {
    /**
     * Collection with all workflow names in reverse order, last added => first in collection
     * Note: Returns observable => collection is updated on any addition/deletion.
     */
    workflowIdentities = liveQuery(
        async () => await executeDbOperation(db.workflows.reverse().toArray()
            .then((result: WorkflowData[]) => result.map(x => {
                return {id: x.id, name: x.name}
            })))
    );

    workflowExists = async (name: string): Promise<Result<boolean>> => {
        try {
            const existingItem = await executeDbOperation(db.workflows.where('name').equalsIgnoreCase(name).first());
            return success(existingItem !== undefined);
        } catch (error) {
            return failure(this.getErrorDetail(error));
        }
    }

    isEmpty = async (): Promise<Result<boolean>> => {
        try {
            const firstItem = await executeDbOperation(db.workflows.toCollection().first());
            return success(firstItem === undefined);
        } catch (error) {
            return failure(this.getErrorDetail(error));
        }
    }

    getWorkflow = async (name: string): Promise<Result<WorkflowData | undefined>> => {
        try {
            return success(await executeDbOperation(db.workflows.where("name").equalsIgnoreCase(name).first()));
        } catch (error) {
            return failure(this.getErrorDetail(error));
        }
    }

    getWorkflowById = async (id: number): Promise<Result<WorkflowData | undefined>> => {
        try {
            return success(await executeDbOperation(db.workflows.get(id)));
        } catch (error) {
            return failure(this.getErrorDetail(error));
        }
    }

    addWorkflow = async (data: WorkflowDataInput): Promise<Result> => {
        try {
            if ((await this.workflowExists(data.name)).data === true) {
                return failure(alreadyExistsError(data.name));
            }
            await executeDbOperation(db.workflows.add(data));
            return success(undefined);
        } catch (error) {
            return failure(this.getErrorDetail(error));
        }
    }

    addEmptyWorkflow = async () => {
        const workflowData = {
            name: `Undefined${Date.now()}`, // Needs unique name
            flowData: {nodes: [], edges: []},
            executionList: []
        };
        const result = await this.addWorkflow(workflowData);
        return result.isSuccessful
            ? success(workflowData)
            : failure(result.error);
    }

    updateWorkflow = async (data: WorkflowData): Promise<Result> => {
        try {
            const itemExists = (await this.workflowExists(data.name)).data;
            if (!itemExists) {
                return failure(notFoundError(data.name));
            }

            await executeDbOperation(db.workflows.put(data));
            return success(undefined);
        } catch (error) {
            return failure(this.getErrorDetail(error));
        }
    }

    deleteWorkflow = async (name: string): Promise<Result> => {
        try {
            await executeDbOperation(db.workflows.where("name").equals(name).delete());
            return success(undefined);
        } catch (error) {
            return failure(this.getErrorDetail(error));
        }
    }

    renameWorkflow = async (id: number, newName: string): Promise<Result> => {
        try {
            await executeDbOperation(db.workflows.where("id").equals(id).modify(data => {
                data.name = newName;
            }));
            return success(undefined);
        } catch (error) {
            return failure(this.getErrorDetail(error));
        }
    }

    private getErrorDetail(error: unknown) {
        const message = error instanceof Error ? error.message : 'Unknown error';
        return fatalError(message);
    };
}

export function useWorkflowDataService() {
    return new WorkflowDataService();
}
