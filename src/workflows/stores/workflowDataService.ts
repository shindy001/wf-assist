import type {WorkflowData, WorkflowDataInput} from "../types";
import {AlreadyExistsError, type Either, ErrorDetail, NotFoundError} from "../../lib/types";
import {db, executeDbOperation} from "../../lib/stores/db";
import {liveQuery} from "dexie";

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

    workflowExists = async (name: string): Promise<boolean> => {
        const existingItem = await executeDbOperation(db.workflows.where('name').equalsIgnoreCase(name).first());
        return existingItem !== undefined;
    }

    isEmpty = async (): Promise<boolean> => {
        const firstItem = await executeDbOperation(db.workflows.toCollection().first());
        return firstItem === undefined;
    }

    getWorkflow = async (name: string): Promise<WorkflowData | undefined> => {
        return await executeDbOperation(db.workflows.where("name").equalsIgnoreCase(name).first());
    }

    getWorkflowById = async (id: number): Promise<WorkflowData | undefined> => {
        return await executeDbOperation(db.workflows.get(id));
    }

    getLastWorkflow = async (): Promise<WorkflowData | undefined> => {
        return await executeDbOperation(db.workflows.toCollection().last());
    }

    addWorkflow = async (data: WorkflowDataInput): Promise<Either<AlreadyExistsError, void>> => {
        if (await this.workflowExists(data.name)) {
            return new AlreadyExistsError();
        }

        await executeDbOperation(db.workflows.add(data));
    }

    addEmptyWorkflow = async (): Promise<string> => {
        const workflowData = {
            name: `Undefined${Date.now()}`, // Needs unique name
            flowData: {nodes: [], edges: []},
            executionList: []
        };
        const result = await this.addWorkflow(workflowData);
        if (result instanceof AlreadyExistsError) {
            // This should not happen when adding empty workflow with unique name
            throw new ErrorDetail("WorkflowDataError", `Error while adding Empty workflow: item already exists.`);
        }
        return workflowData.name;
    }

    updateWorkflow = async (data: WorkflowData): Promise<Either<NotFoundError, void>> => {
        if (!(await this.workflowExists(data.name))) {
            return new NotFoundError();
        }

        await executeDbOperation(db.workflows.put(data));
    }

    deleteWorkflow = async (name: string): Promise<void> => {
        await executeDbOperation(db.workflows.where("name").equals(name).delete());
    }

    renameWorkflow = async (id: number, newName: string): Promise<Either<NotFoundError, void>> => {
        const workflowData = (await this.getWorkflowById(id));
        if (!workflowData) {
            return new NotFoundError();
        }

        await executeDbOperation(db.workflows.where("id").equals(id).modify(data => {
            data.name = newName;
        }));
    }
}

export function useWorkflowDataService() {
    return new WorkflowDataService();
}
