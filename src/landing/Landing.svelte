<script lang="ts">
    import {
        SvelteFlow,
        Controls,
        Background,
        MiniMap,
    } from '@xyflow/svelte';
    import '@xyflow/svelte/dist/style.css';
    import {FlowNodeType} from "../components/FlowNodeType";
    import UrlActionNode from "../components/UrlActionNode.svelte";

    const additionalFlowNodes = {
        [FlowNodeType.UrlAction]: UrlActionNode,
    };

    let nodes = $state.raw([
        {
            id: '1',
            type: FlowNodeType.UrlAction,
            data: { label: 'Hello' },
            position: { x: 0, y: 0 },
        },
        {
            id: '2',
            data: { label: 'World' },
            position: { x: 150, y: 150 },
        },
    ]);

    let edges = $state.raw([
        {
            id: '1-2',
            source: '1',
            target: '2',
        },
    ]);
</script>

<div class="w-full h-full">
    <SvelteFlow bind:nodes nodeTypes={additionalFlowNodes} bind:edges fitView>
        <Controls />
        <Background />
        <MiniMap />
    </SvelteFlow>
</div>
