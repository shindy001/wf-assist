<script module lang="ts">
  import { getContext, setContext } from "svelte";
  import { WorkflowNodeType } from "$lib/components/types";

  const contextKey = Symbol("FlowCanvasContext");

  export const useFlowCanvasContext = () => {
    return getContext(contextKey) as { selectedNodeType: WorkflowNodeType };
  };
</script>

<script lang="ts">
  let { children } = $props();

  setContext(contextKey, { selectedNodeType: WorkflowNodeType.Default });
</script>

{@render children()}
