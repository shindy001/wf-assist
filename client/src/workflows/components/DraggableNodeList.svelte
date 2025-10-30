<script lang="ts">
  import { WorkflowNodeDataType } from "$lib/components/types";
  import type { ClassValue } from "svelte/elements";
  import { Button } from "$lib/components/ui/button";
  import { useWorkflowsAppState } from "../state";

  const props: { class?: ClassValue } = $props();
  const workflowsAppState = useWorkflowsAppState();

  const onDragStart = (
    event: DragEvent,
    selectedNodeType: WorkflowNodeDataType,
  ) => {
    if (!event.dataTransfer) {
      return null;
    }

    workflowsAppState.selectedNodeType = selectedNodeType;
    event.dataTransfer.effectAllowed = "move";
  };

  const nodeTypes = [
    ...Object.values(WorkflowNodeDataType).filter(
      (x) => x !== WorkflowNodeDataType.Default,
    ),
  ];
</script>

<div class={props.class}>
  <p class="text-lg">Nodes</p>
  {#if !workflowsAppState.selectedWorkflowIdentity}
    <p>Select or create a workflow to see nodes.</p>
  {:else}
    <div class="w-full flex flex-wrap gap-3 px-2 py-4 rounded-md">
      {#each nodeTypes as nodeType}
        <Button
          variant="outline"
          class="p-4 cursor-grab translate-px"
          ondragstart={(event) => onDragStart(event, nodeType)}
          draggable={true}>{nodeType}</Button
        >
      {/each}
    </div>
  {/if}
</div>
