import {type Writable, writable} from "svelte/store";

export interface AppData {
    activeWorkflowName: string;
}

const appDataKey = "wfAssistAppData";
const storedValue = localStorage.getItem(appDataKey);
const initialValue: AppData = storedValue ? JSON.parse(storedValue) : {activeWorkflowName: undefined};
const appDataStore = writable<AppData>(initialValue);

appDataStore.subscribe((appData) => {
    localStorage.setItem(appDataKey, JSON.stringify(appData));
});

export function setActiveWorkflow(name: string) {
    appDataStore.set({activeWorkflowName: name});
}

export function useAppDataStore(): Writable<AppData> {
    return appDataStore;
}