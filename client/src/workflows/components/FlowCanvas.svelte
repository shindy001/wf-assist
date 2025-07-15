<script module>
    import {useAppState} from "../../lib/stores/appState.svelte.js";

    const appState = await useAppState();
</script>

<script lang="ts">
    import {Background, Controls, type Edge, MiniMap, type Node, SvelteFlow, useSvelteFlow} from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import {FlowNodeType, type WorkflowData} from "../types";
    import RequestNode from "./nodes/RequestNode.svelte";
    import {useDragAndDrop} from "../../lib/components/DragAndDropProvider.svelte";
    import {useFlowDataProcessor} from "../processors/flowDataProcessor";
    import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
    import PrintStringNode from "./nodes/PrintStringNode.svelte";
    import {useWorkflowDataService} from "../stores/workflowDataService";
    import {useWorkflowExecutor} from "../executors/workflowExecutor";
    import {createSaveWorkflowCommand} from "../commands/saveWorkflowCommand";
    import {useResultsDataService} from "../stores/resultsDataService";

    const workflowDataService = useWorkflowDataService();
    const flowDataProcessor = useFlowDataProcessor();
    const resultsDataService = useResultsDataService();
    const workflowExecutor = useWorkflowExecutor(resultsDataService);
    const dragAndDropContext = useDragAndDrop();
    const additionalFlowNodes = {
        [FlowNodeType.Request]: RequestNode,
        [FlowNodeType.ExtractProperty]: ExtractPropertyNode,
        [FlowNodeType.PrintString]: PrintStringNode,
    };

    const {screenToFlowPosition} = $derived(useSvelteFlow());
    const saveRateLimitInMilliseconds = 500;
    const saveWorkflowCommand = createSaveWorkflowCommand(workflowDataService, saveRateLimitInMilliseconds);
    let currentWorkflow: WorkflowData = {id: 0, name: "", flowData: {nodes: [], edges: []}};
    let nodes = $state.raw<Node[]>([]);
    let edges = $state.raw<Edge[]>([]);
    let activeWorkflowName = $derived(appState.lastActiveWorkflowName);

    $effect(() => {
        if (currentWorkflow) {
            currentWorkflow.flowData.nodes = nodes;
            currentWorkflow.flowData.edges = edges;
            saveWorkflowCommand(currentWorkflow);
        }
    });

    $effect(() => {
        if (currentWorkflow?.name !== activeWorkflowName) {
            initializeWorkflow().then(setCurrentWorkflow);
        }
    })

    const initializeWorkflow = async () => {
        if (activeWorkflowName) {
            return await workflowDataService.getWorkflow(activeWorkflowName);
        }
    };

    function setCurrentWorkflow(data: WorkflowData | undefined) {
        currentWorkflow = data ?? {id: 0, name: "", flowData: {nodes: [], edges: []}};
        nodes = data?.flowData.nodes ?? [];
        edges = data?.flowData.edges ?? [];
    }

    function executeWorkflow() {
        const executionList = flowDataProcessor.createExecutionList(currentWorkflow.flowData);
        workflowExecutor.execute(currentWorkflow.name, executionList);
    }

    const onDragOver = (event: DragEvent) => {
        event.preventDefault();

        if (event.dataTransfer) {
            event.dataTransfer.dropEffect = 'move';
        }
    };

    const onDrop = (event: DragEvent) => {
        event.preventDefault();

        if (!dragAndDropContext.nodeType) {
            return;
        }

        const position = screenToFlowPosition({
            x: event.clientX,
            y: event.clientY,
        });

        const newNode = {
            id: `${Math.random()}`,
            type: dragAndDropContext.nodeType,
            position,
            data: {label: `${dragAndDropContext.nodeType} node`},
            origin: [0.5, 0.5],
        } satisfies Node;

        nodes = [...nodes, newNode];
    };
</script>

<div class="w-full h-full">
    <button class="absolute top-0 z-10 m-4" onclick={() => executeWorkflow()}>
        Execute Flow
    </button>
    <SvelteFlow
            colorMode="system"
            bind:nodes
            bind:edges
            nodeTypes={additionalFlowNodes}
            fitView
            ondragover={onDragOver}
            ondrop={onDrop}
    >
        <Controls showLock={false} position="top-right"/>
        <Background/>
        <MiniMap/>
    </SvelteFlow>
</div>
