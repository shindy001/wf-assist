import type {Edge, Node} from "@xyflow/svelte";
import {get, writable} from "svelte/store";

export interface FlowData {
    nodes: Node[],
    edges: Edge[],
}

const flowDataStore = writable<FlowData>();

export function useFlowDataStore() {
    const isBrowser = typeof window !== "undefined";
    const flowDataKey = "flowData";

    return {
        setData: (data: FlowData) => {
            if (!isBrowser) {
                throw new Error("FlowDataStore cannot be used outside of browser")
            }
            localStorage.setItem(flowDataKey, JSON.stringify(data));
            flowDataStore.set(data);
        },
        getData: () => {
            if (!isBrowser) {
                throw new Error("FlowDataStore cannot be used outside of browser")
            }

            let data = get(flowDataStore);
            if (data) {
                return data;
            }

            const json = localStorage.getItem(flowDataKey);
            if (json) {
                data = JSON.parse(json);
            }
            flowDataStore.set(data);
            return get(flowDataStore);
        }
    }
}