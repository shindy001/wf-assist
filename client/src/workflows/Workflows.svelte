<script lang="ts">
  import { SvelteFlowProvider } from "@xyflow/svelte";
  import FlowSidebar from "./components/FlowSidebar.svelte";
  import FlowCanvas from "./components/FlowCanvas.svelte";
  import { useWorkflowsAppState } from "./state";

  const workflowsAppState = useWorkflowsAppState();
  await workflowsAppState.fetchWorkflowIdentities();
  await workflowsAppState.setSelectedWorkflow(
    workflowsAppState.workflowIdentities?.[0]?.id,
  );
</script>

<SvelteFlowProvider>
  <div class="flex size-full">
    <FlowSidebar />
    <FlowCanvas />
  </div>
</SvelteFlowProvider>
