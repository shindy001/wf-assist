<script lang="ts">
  import {
    Background,
    Controls,
    type Edge,
    MiniMap,
    type Node,
    SvelteFlow,
    useSvelteFlow,
  } from "@xyflow/svelte";
  import "@xyflow/svelte/dist/style.css";
  import RequestNode from "./nodes/RequestNode.svelte";
  import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
  import PrintTextNode from "./nodes/PrintTextNode.svelte";
  import {
    createDefaultWorkflowNodeData,
    type WorkflowNode,
    WorkflowNodeType,
  } from "$lib/components/types";
  import { useFlowCanvasContext } from "../state";
  import { createGetWorkflowQuery } from "../actions/getWorkflowQuery";
  import { useAppState } from "$lib/stores";

  const getWorkflowQuery = createGetWorkflowQuery();
  const appState = await useAppState();
  const flowCanvasContext = useFlowCanvasContext();
  const additionalNodeTypes = {
    [WorkflowNodeType.ExtractProperty]: ExtractPropertyNode,
    [WorkflowNodeType.PrintText]: PrintTextNode,
    [WorkflowNodeType.Request]: RequestNode,
  };

  const { screenToFlowPosition } = $derived(useSvelteFlow());
  let nodes = $state.raw<Node[]>([]);
  let edges = $state.raw<Edge[]>([]);

  $effect(() => {
    const selectedWorkflowIdentity = appState.selectedWorkflowIdentity;
    if (selectedWorkflowIdentity) {
      fetchSelectedWorkflow(selectedWorkflowIdentity.id);
    }
  });

  const fetchSelectedWorkflow = async (id: string) => {
    const result = await getWorkflowQuery(id);
    if (result.isSuccessful && result.data) {
      const workflow = result.data;
      nodes = [...workflow.data.nodes];
    }
  };

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
      flowCanvasContext.selectedNodeType,
    );
    const newNode: WorkflowNode = {
      id: `${Date.now()}`,
      type: data.type,
      position,
      data: data,
    };

    nodes = [...nodes, { ...newNode, data: { ...newNode.data } }];
  };
</script>

<div class="w-full">
  <SvelteFlow
    colorMode="system"
    bind:nodes
    bind:edges
    nodeTypes={additionalNodeTypes}
    fitView
    ondragover={onDragOver}
    ondrop={onDrop}
  >
    <Controls showLock={false} position="top-right" />
    <Background />
    <MiniMap />
  </SvelteFlow>
</div>
