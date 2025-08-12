<script lang="ts">
    import {Background, Controls, type Edge, MiniMap, type Node, SvelteFlow, useSvelteFlow} from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import {useDragAndDrop} from "../../lib/components/DragAndDropProvider.svelte";
    import RequestNode from "./nodes/RequestNode.svelte";
    import ExtractPropertyNode from "./nodes/ExtractPropertyNode.svelte";
    import PrintTextNode from "./nodes/PrintTextNode.svelte";
    import {
        PrintTextNodeData,
        ExtractPropertyNodeData,
        RequestNodeData,
        type WorkflowNode, type WorkflowNodeData,
        WorkflowNodeType
    } from "../types";

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

        const data = CreateWorkflowNodeData(dragAndDropContext.nodeType);
        const newNode: WorkflowNode = {
            id: `${Date.now()}`,
            type: data.type,
            position,
            data: data,
        };

        nodes = [...nodes, { ...newNode, data: {...newNode.data} } ];
    };

    function CreateWorkflowNodeData(nodeType: WorkflowNodeType): WorkflowNodeData {
        switch (nodeType) {
            case WorkflowNodeType.PrintText:
                return new PrintTextNodeData();
            case WorkflowNodeType.ExtractProperty:
                return new ExtractPropertyNodeData();
            case WorkflowNodeType.Request:
                return new RequestNodeData();
            default:
                throw new Error(`Unsupported WorkflowNode type '${nodeType}'`);
        }
    }
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
