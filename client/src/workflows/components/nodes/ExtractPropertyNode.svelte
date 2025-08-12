<script lang="ts" module>
    import type {ExtractPropertyNode} from "../../types";
    import {NodeType} from "../../types";
    import {type Node} from "@xyflow/svelte";

    export type ExtractPropertyNodeType = Node<ExtractPropertyNode, NodeType.ExtractProperty>;
</script>

<script lang="ts">
    import NodeWrapper from "./NodeWrapper.svelte";
    import {Handle, type NodeProps, Position, useNodeConnections, useSvelteFlow} from "@xyflow/svelte";

    const {updateNodeData} = useSvelteFlow();
    const connections = useNodeConnections({handleType: 'target'});
    let {id, data}: NodeProps<ExtractPropertyNodeType> = $props();
    let pathInput = $state(data.path);
    let currentConnectionId = $derived(connections.current[0]?.target);
    let inputIsConnectable = $derived(connections.current.length === 0);

    $effect(() => {
        const data: ExtractPropertyNode = {id: id, path: pathInput, targetId: currentConnectionId};
        updateNodeData(id, data);
    });
</script>

<NodeWrapper label="Extract Property" class="w-[280px]">
    <div class="flex-col space-y-2 w-full">
        <Handle id="input" type="target" class="node-pin" position={Position.Left}
                isConnectable={inputIsConnectable}/>
        <fieldset class="">
            <legend class="">Path</legend>
            <input class="nodrag w-full" placeholder="Enter a property path (e.g. user.id)..."
                   bind:value={pathInput}/>
        </fieldset>
        <Handle id="input" type="source" class="node-pin" position={Position.Right}/>
    </div>


</NodeWrapper>