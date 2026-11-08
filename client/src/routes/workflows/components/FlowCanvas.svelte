<script lang="ts">
  import {
    Background,
    Controls,
    SvelteFlow,
    useSvelteFlow,
  } from "@xyflow/svelte";
  import "@xyflow/svelte/dist/style.css";
  import RequestNode from "./nodes/RequestNode.svelte";
  import HeadersNode from "./nodes/HeadersNode.svelte";
  import {
    createWorkflowNode,
    WorkflowNodeType,
    type WorkflowNode,
  } from "$lib/types";
  import { useWorkflowsAppState } from "../state/";
  import TurboEdge from "./nodes/TurboEdge.svelte";
  import { Button } from "$lib/components/ui/button";
  import { Icon } from "$lib/components/ui/icons";
  import { createExecuteWorkflowCommand } from "../actions/executeWorkflowCommand";

  const workflowsAppState = useWorkflowsAppState();
  const additionalNodeTypes = {
    [WorkflowNodeType.RequestNode]: RequestNode,
    [WorkflowNodeType.HeadersNode]: HeadersNode,
  };

  const { screenToFlowPosition } = $derived(useSvelteFlow());
  let executeWorkflowBtnDisabled = $state(false);

  $effect(() => {
    workflowsAppState.saveWorkflowData();
  });

  const onDragOver = (event: DragEvent) => {
    event.preventDefault();

    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = "move";
    }
  };

  const onDrop = (event: DragEvent) => {
    event.preventDefault();

    const position = screenToFlowPosition({
      x: event.clientX,
      y: event.clientY,
    });

    const newNode = createWorkflowNode(
      workflowsAppState.selectedNodeType,
      position,
    );

    workflowsAppState.addFlowCanvasNode(newNode);
  };

  async function executeCurrentWorkflow() {
    const workflowId = workflowsAppState.selectedWorkflowIdentity?.id;
    if (!workflowId) {
      return;
    }

    executeWorkflowBtnDisabled = true;
    setTimeout(() => (executeWorkflowBtnDisabled = false), 2000);

    const executeWorkflowCommand = createExecuteWorkflowCommand();
    await executeWorkflowCommand(workflowId);
  }
</script>

<div class="relative w-full">
  <SvelteFlow
    colorMode="dark"
    bind:nodes={workflowsAppState.flowCanvasNodes}
    bind:edges={workflowsAppState.flowCanvasEdges}
    nodeTypes={additionalNodeTypes}
    edgeTypes={{ turbo: TurboEdge }}
    defaultEdgeOptions={{ type: "turbo" }}
    ondragover={onDragOver}
    ondrop={onDrop}
  >
    <Controls showLock={false} />
    <Background />
    {#if workflowsAppState.selectedWorkflowIdentity}
      <div class="absolute bottom-36 z-4 h-0 w-full content-center text-center">
        <Button
          variant="default"
          size="xl"
          onclick={executeCurrentWorkflow}
          disabled={executeWorkflowBtnDisabled}
        >
          <Icon name="material-symbols--electric-bolt-outline" />
          <span>Execute workflow</span>
        </Button>
      </div>
    {/if}
  </SvelteFlow>
</div>
