import type { WorkflowIdentity } from "$lib/components/types";

const selectedWorkflowStorageKey = "appState_selectedWorkflow";
let appStateInstance: AppState;
let initializeAppStatePromise: Promise<void> | null = null;

class AppState {
  selectedWorkflowIdentity: WorkflowIdentity = $state<WorkflowIdentity>({
    id: "",
    name: "",
  });

  constructor() {
    $effect.root(() => {
      $effect(() => {
        localStorage.setItem(
          selectedWorkflowStorageKey,
          JSON.stringify(this.selectedWorkflowIdentity),
        );
      });
    });
  }

  setSelectedWorkflow = (workflowIdentity: WorkflowIdentity) => {
    if (
      this.selectedWorkflowIdentity.id !== workflowIdentity.id ||
      this.selectedWorkflowIdentity.name !== workflowIdentity.name
    ) {
      this.selectedWorkflowIdentity = workflowIdentity;
    }
  };
}

async function initializeAppState(appState: AppState) {
  const item = localStorage.getItem(selectedWorkflowStorageKey);
  appState.selectedWorkflowIdentity =
    item === null ? { id: "", name: "" } : JSON.parse(item);
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
