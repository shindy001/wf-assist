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
  import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
  import PrintTextNode from "./nodes/PrintTextNode.svelte";
  import {
    createDefaultWorkflowNodeData,
    type SvelteFlowWorkflowNode,
    WorkflowNodeDataType,
  } from "$lib/types";
  import { useWorkflowsAppState } from "../state/";

  const workflowsAppState = useWorkflowsAppState();
  const additionalNodeTypes = {
    [WorkflowNodeDataType.ExtractProperty]: ExtractPropertyNode,
    [WorkflowNodeDataType.PrintText]: PrintTextNode,
    [WorkflowNodeDataType.Request]: RequestNode,
  };

  const { screenToFlowPosition } = $derived(useSvelteFlow());

  $effect(() => {
    const workflowData = {
      nodes: workflowsAppState.flowCanvasNodes,
      edges: workflowsAppState.flowCanvasEdges,
    };
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
      id: `${Date.now()}`,
      position,
      type: data.type,
      data: data,
    };

    workflowsAppState.addFlowCanvasNode(newNode);
  };
</script>

<div class="w-full">
  <SvelteFlow
    colorMode="system"
    bind:nodes={workflowsAppState.flowCanvasNodes}
    bind:edges={workflowsAppState.flowCanvasEdges}
    nodeTypes={additionalNodeTypes}
    fitView
    ondragover={onDragOver}
    ondrop={onDrop}
  >
    <Controls showLock={false} />
    <Background />
    <MiniMap />
  </SvelteFlow>
</div>
