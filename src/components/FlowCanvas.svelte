<script lang="ts">
    import {Background, Controls, type Edge, MiniMap, type Node, SvelteFlow, useSvelteFlow} from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import {FlowNodeType} from "../models/nodes/FlowNodeType";
    import RequestNode from "./nodes/RequestNode.svelte";
    import {useDragAndDrop} from "./DragAndDropProvider.svelte";
    import {useFlowDataProcessor} from "../processors/flowDataProcessor";
    import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
    import PrintStringNode from "./nodes/PrintStringNode.svelte";
    import {type WorkflowData} from "../models/WorkflowData";
    import {useWorkflowDataStore, type WorkflowDataError} from "../stores/workflowDataStore";
    import {useWorkflowExecutor} from "../executors/workflowExecutor";
    import type {FlowData} from "../models/FlowData";
    import {throttle} from "lodash";
    import {useLocalStore} from "../stores/localStore";

    const localStore = useLocalStore();
    const workflowDataStore = useWorkflowDataStore();
    const flowDataProcessor = useFlowDataProcessor();
    const workflowExecutor = useWorkflowExecutor();
    const dragAndDropContext = useDragAndDrop();
    const additionalFlowNodes = {
        [FlowNodeType.Request]: RequestNode,
        [FlowNodeType.ExtractProperty]: ExtractPropertyNode,
        [FlowNodeType.PrintString]: PrintStringNode,
    };

    const {screenToFlowPosition} = $derived(useSvelteFlow());
    const activeWorkflow: string | undefined = localStore.getItem("activeWorkflow");

    let currentWorkflow: WorkflowData | undefined;
    let nodes = $state.raw<Node[]>([]);
    let edges = $state.raw<Edge[]>([]);
    const saveRateLimitInMilliseconds = 500;

    const throttleSave = throttle((data: { nodes: Node[], edges: Edge[] }) => {
        localStore.setItem("activeWorkflow", "workflow1");
        const nodeExecutionList = flowDataProcessor.createExecutionList(data);
        console.log(nodeExecutionList);
    }, saveRateLimitInMilliseconds);

    $effect(() => {
        throttleSave({
            nodes: nodes,
            edges: edges,
        });
    });

    const initializeWorkflow = async () => {
        let result: WorkflowData | WorkflowDataError | undefined = undefined;
        if (activeWorkflow) {
            result = await workflowDataStore.getWorkflow(activeWorkflow);
        }

        if ((result as WorkflowDataError).error) {
            console.error(result);
            return undefined;
        }

        return result as WorkflowData;
    };

    initializeWorkflow()
        .then((data) => {
            currentWorkflow = data;
            if (data) {
                nodes = data.flowData.nodes;
                edges = data.flowData.edges;
            }
        });

    function executeWorkflow() {
        const flowData: FlowData = {nodes: nodes, edges: edges};
        const nodeExecutionList = flowDataProcessor.createExecutionList(flowData);
        const workflowData: WorkflowData = {
            name: "workflow1",
            flowData: flowData,
            executionList: nodeExecutionList
        }
        workflowDataStore.addOrUpdateWorkflow(workflowData);
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
