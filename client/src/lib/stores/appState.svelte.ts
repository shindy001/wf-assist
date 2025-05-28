const lastActiveWorkflowLocalStorageKey = "appState_lastActiveWorkflowName";
let appStateInstance: AppState;
let initializeAppStatePromise: Promise<void> | null = null;

class AppState {
    lastActiveWorkflowName = $state<string>("");

    constructor() {
        $effect.root(() => {
            $effect(() => {
                localStorage.setItem(lastActiveWorkflowLocalStorageKey, this.lastActiveWorkflowName);
            });
        });
    }

    setActiveWorkflowName = (name: string) => {
        this.lastActiveWorkflowName = name;
    }
}

async function initializeAppState(appState: AppState) {
    const item = localStorage.getItem(lastActiveWorkflowLocalStorageKey);
    appState.lastActiveWorkflowName = item ?? "";
}

export type { AppState };
export async function useAppState() {
    if (!appStateInstance) {
        appStateInstance = new AppState();
        initializeAppStatePromise = initializeAppState(appStateInstance);
    }

    if (initializeAppStatePromise) {
        await initializeAppStatePromise;
    }

    return appStateInstance;
}