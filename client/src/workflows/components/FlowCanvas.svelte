<script lang="ts">
  import {
    Background,
    Controls,
    MiniMap,
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
  import { useWorkflowsAppState } from "../state/";
  import TurboEdge from "./nodes/TurboEdge.svelte";
  import { Button } from "$lib/components/ui/button";
  import { Icon } from "$lib/components/ui/icons";

  const workflowsAppState = useWorkflowsAppState();
  const additionalNodeTypes = {
    [WorkflowNodeDataType.Request]: RequestNode,
    [WorkflowNodeDataType.Headers]: HeadersNode,
  };

  const { screenToFlowPosition } = $derived(useSvelteFlow());

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
</script>

<div class="relative w-full">
  <SvelteFlow
    colorMode="system"
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
    <div class="absolute bottom-18 z-10 w-full content-center text-center">
      <Button variant="default" size="xl">
        <Icon name="material-symbols--electric-bolt-outline" />
        <span>Execute workflow</span>
        <!-- TODO - add onclick event to call execute workflow action -->
      </Button>
    </div>
  </SvelteFlow>
</div>
