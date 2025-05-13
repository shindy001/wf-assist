<script lang="ts">
    import {Background, Controls, type Edge, MiniMap, type Node, SvelteFlow, useSvelteFlow} from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import type {FlowData} from "../types";
    import {FlowNodeType, type WorkflowData} from "../types";
    import RequestNode from "./nodes/RequestNode.svelte";
    import {useDragAndDrop} from "../../lib/components/DragAndDropProvider.svelte";
    import {useFlowDataProcessor} from "../processors/flowDataProcessor";
    import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
    import PrintStringNode from "./nodes/PrintStringNode.svelte";
    import {useWorkflowDataService} from "../stores/workflowDataService";
    import {useWorkflowExecutor} from "../executors/workflowExecutor";
    import {throttle} from "lodash";
    import {useAppDataStore} from "../../lib/stores/appDataStore";

    const appDataStore = useAppDataStore();
    const workflowDataService = useWorkflowDataService();
    const flowDataProcessor = useFlowDataProcessor();
    const workflowExecutor = useWorkflowExecutor();
    const dragAndDropContext = useDragAndDrop();
    const additionalFlowNodes = {
        [FlowNodeType.Request]: RequestNode,
        [FlowNodeType.ExtractProperty]: ExtractPropertyNode,
        [FlowNodeType.PrintString]: PrintStringNode,
    };

    const {screenToFlowPosition} = $derived(useSvelteFlow());

    let currentWorkflow: WorkflowData | undefined;
    let nodes = $state.raw<Node[]>([]);
    let edges = $state.raw<Edge[]>([]);
    const saveRateLimitInMilliseconds = 500;

    const throttleSave = throttle(async (data: { nodes: Node[], edges: Edge[] }) => {
        const nodeExecutionList = flowDataProcessor.createExecutionList(data);
        if (currentWorkflow) {
            currentWorkflow.flowData.nodes = nodes;
            currentWorkflow.flowData.edges = edges;
            currentWorkflow.executionList = nodeExecutionList;
            await workflowDataService.updateWorkflow(currentWorkflow);
        }
    }, saveRateLimitInMilliseconds);

    $effect(() => {
        throttleSave({
            nodes: nodes,
            edges: edges,
        });
    });

    $effect(() => {
        const activeWorkflowName = $appDataStore.activeWorkflowName;
        if (currentWorkflow?.name !== activeWorkflowName) {
            initializeWorkflow().then(setCurrentWorkflow);
        }
    })

    const initializeWorkflow = async () => {
        if ($appDataStore?.activeWorkflowName) {
            return await workflowDataService.getWorkflow($appDataStore.activeWorkflowName);
        }
    };

    function setCurrentWorkflow(data: WorkflowData | undefined) {
        currentWorkflow = data;
        nodes = data?.flowData.nodes ?? [];
        edges = data?.flowData.edges ?? [];
    }

    function executeWorkflow() {
        const flowData: FlowData = {nodes: nodes, edges: edges};
        const nodeExecutionList = flowDataProcessor.createExecutionList(flowData);
        const workflowData: WorkflowData = {
            id: 0,
            name: "testWorkflow",
            flowData: flowData,
            executionList: nodeExecutionList
        }
        workflowExecutor.execute(workflowData);
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
    <button class="daisyui-btn absolute top-0 z-10 m-4" onclick={() => executeWorkflow()}>
        Execute Flow
    </button>
    <SvelteFlow
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
