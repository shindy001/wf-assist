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
    type WorkflowData,
    WorkflowDataState,
    type WorkflowEdge,
    type WorkflowNode,
    WorkflowNodeType,
  } from "$lib/components/types";
  import { useFlowCanvasContext } from "../state";
  import { createGetWorkflowQuery } from "../actions/getWorkflowQuery";
  import { useAppState } from "$lib/stores";
  import { createSaveWorkflowCommand } from "../actions/saveWorkflowCommand";
  import { onMount } from "svelte";
  import { merge } from "lodash";

  const saveWorkflowRateInMiliseconds = 500;
  const getWorkflowQuery = createGetWorkflowQuery();
  const saveWorkflowCommand = createSaveWorkflowCommand(
    saveWorkflowRateInMiliseconds,
  );
  const appState = await useAppState();
  const flowCanvasContext = useFlowCanvasContext();
  const additionalNodeTypes = {
    [WorkflowNodeType.ExtractProperty]: ExtractPropertyNode,
    [WorkflowNodeType.PrintText]: PrintTextNode,
    [WorkflowNodeType.Request]: RequestNode,
  };

  const { screenToFlowPosition, fitView } = $derived(useSvelteFlow());
  let nodes = $state.raw<Node[]>([]);
  let edges = $state.raw<Edge[]>([]);
  let workflowDataState = WorkflowDataState.Uninitialized;

  onMount(() => {
    const interval = setInterval(() => {
      if (workflowDataState === WorkflowDataState.ReadyToSave) {
        saveWorkflow(appState.selectedWorkflowIdentity.id, {
          nodes: nodes.map((x) => merge(x, x.data) as unknown as WorkflowNode),
          edges: edges.map((x) => merge(x, x.data) as unknown as WorkflowEdge),
        });
      }
    }, saveWorkflowRateInMiliseconds);

    return () => clearInterval(interval);
  });

  $effect(() => {
    const selectedWorkflowIdentity = appState.selectedWorkflowIdentity;
    if (selectedWorkflowIdentity) {
      fetchSelectedWorkflow(selectedWorkflowIdentity.id).then(() => fitView());
    }
  });

  $effect(() => {
    // TODO - effect is firstly called after data fetch(initialization), next effect after that means change in workflow data
    const workflowData = { nodes: nodes, edges: edges };
    if (workflowDataState === WorkflowDataState.Initialized) {
      workflowDataState = WorkflowDataState.ReadyToChange;
    } else if ((workflowDataState = WorkflowDataState.ReadyToChange)) {
      workflowDataState = WorkflowDataState.ReadyToSave;
    }
  });

  const saveWorkflow = async (id: string, workflowData: WorkflowData) => {
    workflowDataState = WorkflowDataState.Saving;
    const result = await saveWorkflowCommand(id, workflowData);
    if (!result.isSuccessful) {
      // Saving failed, change state to ReadyToSave to try again.
      workflowDataState = WorkflowDataState.ReadyToSave;

      // TODO - use toast to show error???
      console.error(`Error while saving workflow: ${result.error}.`);
    } else {
      workflowDataState = WorkflowDataState.ReadyToChange;
    }
  };

  const fetchSelectedWorkflow = async (id: string) => {
    workflowDataState = WorkflowDataState.Uninitialized;
    const result = await getWorkflowQuery(id);
    if (result.isSuccessful && result.data) {
      const workflow = result.data;
      nodes = [...workflow.data.nodes];
    }
    workflowDataState = WorkflowDataState.Initialized;
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
