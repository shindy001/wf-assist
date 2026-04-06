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
    createDefaultWorkflowNodeData,
    type SvelteFlowWorkflowNode,
    WorkflowNodeDataType,
  } from "$lib/types";
  import { useWorkflowEvents, useWorkflowsAppState } from "../state/";
  import TurboEdge from "./nodes/TurboEdge.svelte";
  import { Button } from "$lib/components/ui/button";
  import { Icon } from "$lib/components/ui/icons";
  import { createExecuteWorkflowCommand } from "../actions/executeWorkflowCommand";

  const workflowsAppState = useWorkflowsAppState();
  const additionalNodeTypes = {
    [WorkflowNodeDataType.Request]: RequestNode,
    [WorkflowNodeDataType.Headers]: HeadersNode,
  };

  const { screenToFlowPosition } = $derived(useSvelteFlow());
  const { lastEvent } = $derived(useWorkflowEvents());
  let executingWorkflow = $state(false);
  let executingWorkflowId = $state<string | undefined>();
  let executingWorkflowExecutionId = $state<string | undefined>();

  $effect(() => {
    workflowsAppState.saveWorkflowData();
  });

  $effect(() => {
    if (
      lastEvent?.executionId === executingWorkflowExecutionId &&
      lastEvent?.type === "WorkflowExecutionEnded" &&
      lastEvent?.workflowId === executingWorkflowId
    ) {
      executingWorkflow = false;
      executingWorkflowId = undefined;
      executingWorkflowExecutionId = undefined;
    }
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

    const data = createDefaultWorkflowNodeData(
      workflowsAppState.selectedNodeType,
    );

    const newNode: SvelteFlowWorkflowNode = {
      id: "",
      position,
      type: data.type,
      data: data,
    };

    workflowsAppState.addFlowCanvasNode(newNode);
  };

  async function executeCurrentWorkflow() {
    const workflowId = workflowsAppState.selectedWorkflowIdentity?.id;
    if (!workflowId) {
      return;
    }

    const executeWorkflowCommand = createExecuteWorkflowCommand();
    const result = await executeWorkflowCommand(workflowId);
    executingWorkflow = true;
    executingWorkflowId = workflowId;
    executingWorkflowExecutionId = result.data?.executionId;
    // TODO - use execution ID to subscribe to events (when api is available) and propagate state of execution to flow canvas (set animation for node that is executed)
    console.log("executionId: " + result.data?.executionId);
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
    fitView
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
          disabled={executingWorkflow}
        >
          <Icon name="material-symbols--electric-bolt-outline" />
          <span>Execute workflow</span>
        </Button>
      </div>
    {/if}
  </SvelteFlow>
</div>
