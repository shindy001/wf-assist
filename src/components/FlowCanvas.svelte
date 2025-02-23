<script lang="ts">
    import {
        Background,
        Controls,
        type Edge,
        MiniMap,
        type Node,
        SvelteFlow,
        useEdges,
        useNodes,
        useSvelteFlow
    } from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import {FlowNodeType} from "../models/flowNodeType";
    import UrlActionNode from "./UrlActionNode.svelte";
    import {useFlowDataStore} from "../stores/flowDataStore";
    import {useDragAndDrop} from "./DragAndDropProvider.svelte";
    import {useFlowDataProcessor} from "../processors/flowDataProcessor";

    const currentNodes = $derived(useNodes());
    const currentEdges = $derived(useEdges());
    const {screenToFlowPosition} = $derived(useSvelteFlow());
    const dragAndDropContext = useDragAndDrop();

    const flowDataStore = useFlowDataStore();
    const flowDataProcessor = useFlowDataProcessor();
    const initialData = flowDataStore.getData();
    const additionalFlowNodes = {
        [FlowNodeType.UrlAction]: UrlActionNode,
    };

    let nodes = $state.raw<Node[]>(initialData?.nodes ?? []);
    let edges = $state.raw<Edge[]>(initialData?.edges ?? []);

    $effect(() => {
        const data = {
            nodes: currentNodes.current,
            edges: currentEdges.current,
        }
        flowDataStore.setData(data);
        console.log(data);
        const nodeExecutionOrder = flowDataProcessor.calculateNodeExecutionOrder(data);
        console.log(nodeExecutionOrder);
    });

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
            origin: [0.5, 0.0],
        } satisfies Node;

        nodes = [...nodes, newNode];
    };
</script>

<div class="w-full h-full">
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
