import wrap from "svelte-spa-router/wrap";
import Executions from "./routes/executions/Executions.svelte";
import Workflows from "./workflows/Workflows.svelte";
import { replace } from "svelte-spa-router";

export default {
  '/': wrap({
    component: Workflows,
    conditions: [
      (detail) => {
        if (detail.location == '/') {
          replace('/workflows');
        }

        return true;
      }
    ]
  }),
  '/workflows': Workflows,
  '/executions': Executions
}
