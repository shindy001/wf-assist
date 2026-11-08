import * as signalR from "@microsoft/signalr";

export type WorkflowExecutionEnded = {
  type: "WorkflowExecutionEnded";
  executionId: string;
  workflowId: string;
  workflowName: string;
  status: string;
};

export type WorkflowExecutionStarted = {
  type: "WorkflowExecutionStarted";
  executionId: string;
  workflowId: string;
  workflowName: string;
};

class WorkflowEvents {
  #lastEvent = $state<WorkflowExecutionEnded | WorkflowExecutionStarted>();

  constructor() {
    this.initialize().then(() => console.info("WorkflowEvents initialized."));
  }

  get lastEvent() {
    return this.#lastEvent;
  }

  private async initialize() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`${import.meta.env.VITE_API_WEBSOCKET}`, { withCredentials: false })
        .configureLogging(signalR.LogLevel.Information)
        .build();

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    };

    connection.on("WorkflowExecutionStarted", (message) => {
      this.#lastEvent = { ...message, type: "WorkflowExecutionStarted"};
    });

    connection.on("WorkflowExecutionEnded", (message) => {
      this.#lastEvent = { ...message, type: "WorkflowExecutionEnded"};
    });

    connection.onclose(async () => {
        await start();
    });

    // Start the connection.
    start();
  }
}

const instance = new WorkflowEvents();

export const useWorkflowEvents = () => instance;
