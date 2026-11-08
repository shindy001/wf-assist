import { toast } from "$lib/components/ui/toaster";
import type {
  WorkflowExecutionEnded,
  WorkflowExecutionStarted,
} from "$lib/types";
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${import.meta.env.VITE_API_WEBSOCKET}`, { withCredentials: false })
  .configureLogging(signalR.LogLevel.Information)
  .build();

export async function connectToMessageHub() {
  try {
    await connection.start();
    console.log("SignalR Connected.");
  } catch (err) {
    console.log(err);
    setTimeout(connectToMessageHub, 5000);
  }
}

connection.on(
  "WorkflowExecutionStarted",
  (message: WorkflowExecutionStarted) => {
    toast.info("WorkflowExecutionStarted", {
      description: `Name: ${message.workflowName}`,
    });
  },
);

connection.on("WorkflowExecutionEnded", (message: WorkflowExecutionEnded) => {
  toast.info("WorkflowExecutionEnded", {
    description: `Name: ${message.workflowName}`,
  });
});

connection.onclose(async () => {
  await connectToMessageHub();
});
