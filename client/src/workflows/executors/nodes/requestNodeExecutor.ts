import type {NodeExecutor, RequestNode} from "../../types";
import type {ResultsDataService} from "../../stores/resultsDataService";

export function useRequestNodeExecutor(resultDataService: ResultsDataService): NodeExecutor<RequestNode> {
    return {
        execute: async (executionId: string, nodeData: RequestNode) => {
            if (!nodeData.url) {
                return Promise.reject(`RequestNode ${nodeData.id}: Url was not provided.`);
            }

            const response = await fetch(new URL(nodeData.url), {
                method: nodeData.requestType,
                body: nodeData.requestBody ? nodeData.requestBody : null,
                headers: {
                    "Access-Control-Allow-Origin": "*",
                    "Access-Control-Allow-Methods": "GET, POST, PATCH, PUT, DELETE, OPTIONS",
                    "Access-Control-Allow-Headers": "Origin, Content-Type, X-Auth-Token",
                    "Content-type": "application/json; charset=UTF-8"
                }
            });

            const data = await response.json();
            await resultDataService.addOrUpdateResults(executionId, { nodeId: nodeData.id, value: data });
        }
    }
}