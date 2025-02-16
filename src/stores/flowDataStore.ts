import type {Edge, Node} from "@xyflow/svelte";
import {get, writable} from "svelte/store";

export interface FlowData {
    nodes: Node[],
    edges: Edge[],
}

export function useFlowDataStore() {
    const store = writable<FlowData>();
    const isBrowser = typeof window !== "undefined";
    const flowDataKey = "flowData";

    return {
        setData: (data: FlowData) => {
            if (!isBrowser) {
                throw new Error("FlowDataStore cannot be used outside of browser")
            }
            localStorage.setItem(flowDataKey, JSON.stringify(data));
            store.set(data);
        },
        getData: () => {
            if (!isBrowser) {
                throw new Error("FlowDataStore cannot be used outside of browser")
            }

            let data = get(store);
            if (data) {
                return data;
            }

            const json = localStorage.getItem(flowDataKey);
            if (json) {
                data = JSON.parse(json);
            }
            store.set(data);
            return get(store);
        }
    }
}