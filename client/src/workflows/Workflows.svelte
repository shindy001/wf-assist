<script>
  import { SvelteFlowProvider } from "@xyflow/svelte";
  import FlowSidebar from "./components/FlowSidebar.svelte";
  import FlowCanvas from "./components/FlowCanvas.svelte";
  import { useWorkflowsAppState } from "./state";

  const workflowsAppState = useWorkflowsAppState();
  await workflowsAppState.fetchWorkflowIdentities();
</script>

<SvelteFlowProvider>
  <div class="flex size-full">
    <FlowSidebar />
    <FlowCanvas />
  </div>
</SvelteFlowProvider>
