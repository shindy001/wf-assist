import {get, writable} from "svelte/store";
import type {FlowData} from "../models/FlowData";
import {isBrowser} from "../utils/platformUtils";

const flowDataStore = writable<FlowData>();

export function useFlowDataStore() {
    const flowDataKey = "flowData";

    return {
        setData: (data: FlowData) => {
            if (!isBrowser()) {
                throw new Error("FlowDataStore cannot be used outside of browser")
            }
            localStorage.setItem(flowDataKey, JSON.stringify(data));
            flowDataStore.set(data);
        },
        getData: () => {
            if (!isBrowser()) {
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