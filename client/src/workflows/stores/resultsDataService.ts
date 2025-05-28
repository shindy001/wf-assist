import type {WorkflowResults} from "../types";
import {db, executeDbOperation} from "../../lib/stores/db";

export class ResultsDataService {
    getResults = async (id: string): Promise<WorkflowResults | undefined> => {
        return await executeDbOperation(db.results.get(id));
    }

    addOrUpdateResults = async (resultsId: string, nodeResult: { nodeId: string; value: unknown }): Promise<void> => {
        if (await this.getResults(resultsId) === undefined) {
            await executeDbOperation(db.results.add({ id: resultsId, data: {} }));
        }

        await executeDbOperation(db.results.where("id").equals(resultsId).modify(x => {
            x.data[nodeResult.nodeId] = nodeResult.value;
        }));
    }

    deleteResults = async (id: string): Promise<void> => {
        await executeDbOperation(db.results.delete(id));
    }
}

export function useResultsDataService() {
    return new ResultsDataService();
}
