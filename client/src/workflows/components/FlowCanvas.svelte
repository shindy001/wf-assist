<script lang="ts">
    import {Background, Controls, MiniMap, SvelteFlow, useSvelteFlow, type Node, type Edge} from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import {useDragAndDrop} from "../../lib/components/DragAndDropProvider.svelte";
    import RequestNode from "./nodes/RequestNode.svelte";
    import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
    import PrintTextNode from "./nodes/PrintTextNode.svelte";
    import {type WorkflowNode, WorkflowNodeType} from "../types";

    const dragAndDropContext = useDragAndDrop();
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

        if (!dragAndDropContext.nodeType) {
            return;
        }

        const position = screenToFlowPosition({
            x: event.clientX,
            y: event.clientY,
        });

        const newNode: WorkflowNode = {
            id: `${Date.now()}`,
            type: dragAndDropContext.nodeType,
            position,
            data: { type: WorkflowNodeType.Request },
        };

        nodes = [...nodes, { ...newNode, data: {...newNode.data} } ];
    };
</script>

<div class="size-full">
    <SvelteFlow
            colorMode="system"
            nodes={nodes}
            edges={edges}
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
