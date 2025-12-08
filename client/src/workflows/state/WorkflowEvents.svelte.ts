import { getWfAssistWorkflowsEvents, type Notification } from "$api";
import { client } from "$api/client.gen";

class WorkflowEvents {
  #lastEvent = $state<Notification | undefined>();

  constructor() {
    this.initialize();
  }

  get lastEvent() {
    return this.#lastEvent;
  }

  async initialize() {
    const { stream } = await getWfAssistWorkflowsEvents({
      // Generated sse method does not have correct base url, grab it from the client
      baseUrl: client.getConfig().baseUrl,
    });
    for await (const event of stream) {
      this.#lastEvent = event as Notification;
    }
  }
}

const instance = new WorkflowEvents();

export const useWorkflowEvents = () => instance;
