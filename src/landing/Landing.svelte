<script lang="ts">
    import {Background, Controls, type Edge, MiniMap, type Node, Position, SvelteFlow,} from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import {FlowNodeType} from "../components/FlowNodeType";
    import UrlActionNode from "../components/UrlActionNode.svelte";

    const additionalFlowNodes = {
        [FlowNodeType.UrlAction]: UrlActionNode,
    };

    const initialNodes: Node[] = [
        {
            id: '1',
            type: 'input',
            data: { label: 'An input node' },
            position: { x: 0, y: 50 },
            sourcePosition: Position.Right,
        },
        {
            id: '2',
            type: FlowNodeType.UrlAction,
            data: {},
            position: { x: 300, y: 50 },
        },
        {
            id: '3',
            type: FlowNodeType.Output,
            data: { label: 'Output A' },
            position: { x: 650, y: 25 },
            targetPosition: Position.Left,
        },
        {
            id: '4',
            type: FlowNodeType.Output,
            data: { label: 'Output B' },
            position: { x: 650, y: 100 },
            targetPosition: Position.Left,
        },
    ];

    const initialEdges: Edge[] = [
        {
            id: 'e1-2',
            source: '1',
            target: '2',
            animated: true,
        },
        {
            id: 'e2a-3',
            source: '2',
            target: '3',
            animated: true,
        },
        {
            id: 'e2b-4',
            source: '2',
            target: '4',
            animated: true,
        },
    ];

    let nodes = $state.raw<Node[]>(initialNodes);
    let edges = $state.raw<Edge[]>(initialEdges);
</script>

<div class="w-full h-full">
    <SvelteFlow
            bind:nodes
            bind:edges
            nodeTypes={additionalFlowNodes}
            fitView

    >
        <Controls showLock={false} />
        <Background />
        <MiniMap />
    </SvelteFlow>
</div>
