<script lang="ts">
    import {Background, Controls, type Edge, MiniMap, type Node, SvelteFlow, useSvelteFlow} from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import {useDragAndDrop} from "../../lib/components/DragAndDropProvider.svelte";
    import RequestNode from "./nodes/RequestNode.svelte";
    import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
    import PrintTextNode from "./nodes/PrintTextNode.svelte";
    import {Button} from "$lib/components/ui/button/index.js";
    import {NodeType} from "../types";

    const dragAndDropContext = useDragAndDrop();
    const additionalFlowNodes = {
        [NodeType.ExtractProperty]: ExtractPropertyNode,
        [NodeType.PrintText]: PrintTextNode,
        [NodeType.Request]: RequestNode,
    };

    const {screenToFlowPosition} = $derived(useSvelteFlow());
    let nodes = $state.raw<Node[]>([]);
    let edges = $state.raw<Edge[]>([]);

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
            id: `${Date.now()}`,
            type: dragAndDropContext.nodeType,
            position,
            data: {label: `${dragAndDropContext.nodeType} node`},
            origin: [0.5, 0.5],
        } satisfies Node;

        nodes = [...nodes, newNode];
    };
</script>

<div class="w-full h-full">
    <Button onclick={() => console.log(nodes)}>Print</Button>
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
