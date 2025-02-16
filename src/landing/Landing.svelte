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
    import {FlowNodeType} from "../components/FlowNodeType";
    import UrlActionNode from "../components/UrlActionNode.svelte";
    import {useFlowDataStore} from "../stores/flowDataStore";
    import {useDragAndDrop} from "../components/DragAndDropProvider.svelte";

    const currentNodes = $derived(useNodes());
    const currentEdges = $derived(useEdges());
    const {screenToFlowPosition} = $derived(useSvelteFlow());
    const type = useDragAndDrop();

    const flowDataStore = useFlowDataStore();
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
    });

    const onDragOver = (event: DragEvent) => {
        event.preventDefault();

        if (event.dataTransfer) {
            event.dataTransfer.dropEffect = 'move';
        }
    };

    const onDrop = (event: DragEvent) => {
        event.preventDefault();

        if (!type.current) {
            return;
        }

        const position = screenToFlowPosition({
            x: event.clientX,
            y: event.clientY,
        });

        const newNode = {
            id: `${Math.random()}`,
            type: type.current,
            position,
            data: {label: `${type.current} node`},
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
        <Controls showLock={false}/>
        <Background/>
        <MiniMap/>
    </SvelteFlow>
</div>
