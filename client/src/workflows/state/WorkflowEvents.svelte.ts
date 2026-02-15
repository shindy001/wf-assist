import { getApiWorkflowsEvents, type Notification } from "$api";

class WorkflowEvents {
  #lastEvent = $state<Notification | undefined>();

  constructor() {
    this.initialize().then(() => console.info("WorkflowEvents initialized."));
  }

  get lastEvent() {
    return this.#lastEvent;
  }

  private async initialize() {
    const { stream } = await getApiWorkflowsEvents();
    for await (const event of stream) {
      this.#lastEvent = event as Notification;
    }
  }
}

const instance = new WorkflowEvents();

export const useWorkflowEvents = () => instance;
