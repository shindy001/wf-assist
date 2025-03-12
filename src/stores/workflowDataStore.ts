import {get, writable} from "svelte/store";
import {isBrowser} from "../utils/platformUtils";
import type {WorkflowData} from "../models/WorkflowData";

const workflowDataStore = writable<WorkflowData>();

export function useWorkflowDataStore() {
    const workflowData = "workflowData";

    return {
        setData: (data: WorkflowData) => {
            if (!isBrowser()) {
                throw new Error("WorkflowDataStore cannot be used outside of a browser")
            }
            localStorage.setItem(workflowData, JSON.stringify(data));
            workflowDataStore.set(data);
        },
        getData: () => {
            if (!isBrowser()) {
                throw new Error("FlowDataStore cannot be used outside of a browser")
            }

            let data = get(workflowDataStore);
            if (data) {
                return data;
            }

            const json = localStorage.getItem(workflowData);
            if (json) {
                data = JSON.parse(json);
            }
            workflowDataStore.set(data);
            return get(workflowDataStore);
        }
    }
}