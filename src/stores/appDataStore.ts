import {type Writable, writable} from "svelte/store";

export interface AppData {
    activeWorkflowId: string;
}

const appDataKey = "wfAssistAppData";
const storedValue = localStorage.getItem(appDataKey);
const initialValue: AppData = storedValue ? JSON.parse(storedValue) : { activeWorkflowId: undefined };
const appDataStore = writable<AppData>(initialValue);

appDataStore.subscribe((appData) => {
    localStorage.setItem(appDataKey, JSON.stringify(appData));
});

export function useAppDataStore(): Writable<AppData> {
    return appDataStore;
}