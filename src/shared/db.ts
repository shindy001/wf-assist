import Dexie, {type EntityTable} from "dexie";
import type {WorkflowData} from "../models/WorkflowData";

/**
 * Represents error that happened during {@link db} call
 */
export class DBError extends Error {
    constructor(message: string) {
        super(message);
        this.name = "DBError";
        Object.setPrototypeOf(this, DBError.prototype);
    }
}

/**
 * Wraps calls to db and throws {@link DBError}
 * Note: wrap only direct {@link db} calls.
 */
export const executeDbOperation = async <T>(fn: Promise<T>): Promise<T> => {
    try {
        return await fn;
    } catch (error) {
        const message = error instanceof Error ? error.message : 'Unknown error';
        console.error(message);
        throw new DBError(`Error during method: '${fn.constructor.name}': ${message}`);
    }
}

/**
 * Database provider
 */
export const db = new Dexie("wf-assist") as Dexie & {
    workflows: EntityTable<WorkflowData, "id">;
};

db.version(1).stores({
    workflows: "id++, name"
});
