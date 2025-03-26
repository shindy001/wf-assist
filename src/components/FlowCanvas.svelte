<script lang="ts">
    import {Background, Controls, type Edge, MiniMap, type Node, SvelteFlow, useSvelteFlow} from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import {FlowNodeType} from "../models/nodes/FlowNodeType";
    import RequestNode from "./nodes/RequestNode.svelte";
    import {useFlowDataStore} from "../stores/flowDataStore";
    import {useDragAndDrop} from "./DragAndDropProvider.svelte";
    import {useFlowDataProcessor} from "../processors/flowDataProcessor";
    import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
    import PrintStringNode from "./nodes/PrintStringNode.svelte";
    import {type WorkflowData} from "../models/WorkflowData";
    import {useWorkflowDataStore} from "../stores/workflowDataStore";
    import {useWorkflowProcessor} from "../processors/workflowProcessor";
    import type {FlowData} from "../models/FlowData";

    const {screenToFlowPosition} = $derived(useSvelteFlow());
    const dragAndDropContext = useDragAndDrop();
    const flowDataStore = useFlowDataStore();
    const flowDataProcessor = useFlowDataProcessor();
    const initialData = flowDataStore.getData();
    const additionalFlowNodes = {
        [FlowNodeType.Request]: RequestNode,
        [FlowNodeType.ExtractProperty]: ExtractPropertyNode,
        [FlowNodeType.PrintString]: PrintStringNode,
    };

    let nodes = $state.raw<Node[]>(initialData?.nodes ?? []);
    let edges = $state.raw<Edge[]>(initialData?.edges ?? []);
    const saveRateLimitInMilliseconds = 1000;
    let canSaveFlow = true;

    $effect(() => {
        // Leave data outside if statement to force svelte to evaluate effect on nodes/edges change
        const data = {
            nodes: nodes,
            edges: edges,
        }

        if (canSaveFlow) {
            canSaveFlow = false;
            flowDataStore.setData(data);
            const nodeExecutionList = flowDataProcessor.createExecutionList(data);
            console.log(nodeExecutionList);

            // Rate limit the saves
            setTimeout(() => {
                canSaveFlow = true;
            }, saveRateLimitInMilliseconds);
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
        const workflowDataStore = useWorkflowDataStore();
        workflowDataStore.addOrUpdateWorkflow(workflowData);
        const workflowProcessor = useWorkflowProcessor();
        workflowProcessor.process(workflowData);
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
    <button class="daisyui-btn absolute z-10" onclick={() => executeWorkflow()}>
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
