<script lang="ts">
    import {Background, Controls, type Edge, MiniMap, type Node, SvelteFlow, useSvelteFlow} from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import { useSelectedNodeTypeContext } from '$lib/components/ui/state';
    import RequestNode from "./nodes/RequestNode.svelte";
    import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
    import PrintTextNode from "./nodes/PrintTextNode.svelte";
    import {createWorkflowNodeData, type WorkflowNode, WorkflowNodeType} from "$lib/components/types";

    const selectedNodeTypeContext = useSelectedNodeTypeContext();
    const additionalNodeTypes = {
        [WorkflowNodeType.ExtractProperty]: ExtractPropertyNode,
        [WorkflowNodeType.PrintText]: PrintTextNode,
        [WorkflowNodeType.Request]: RequestNode,
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

        const position = screenToFlowPosition({
            x: event.clientX,
            y: event.clientY,
        });

        const data = createWorkflowNodeData(selectedNodeTypeContext.nodeType);
        const newNode: WorkflowNode = {
            id: `${Date.now()}`,
            type: data.type,
            position,
            data: data,
        };

        nodes = [...nodes, { ...newNode, data: {...newNode.data} } ];
    };
</script>

<div class="size-full">
    <SvelteFlow
            colorMode="system"
            bind:nodes={nodes}
            bind:edges={edges}
            nodeTypes={additionalNodeTypes}
            fitView
            ondragover={onDragOver}
            ondrop={onDrop}
    >
        <Controls showLock={false} position="top-right"/>
        <Background/>
        <MiniMap/>
    </SvelteFlow>
</div>
