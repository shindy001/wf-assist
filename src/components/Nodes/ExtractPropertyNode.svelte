<script lang="ts" module>
    import {FlowNodeType} from "../../models/FlowNodeType";
    import {type Node} from "@xyflow/svelte";

    export type ExtractPropertyNodeType = Node<{
        path: string,
    }, FlowNodeType.ExtractProperty>;
</script>

<script lang="ts">
    import NodeWrapper from "./NodeWrapper.svelte";
    import {Handle, type NodeProps, Position, useNodeConnections, useSvelteFlow} from "@xyflow/svelte";

    const {updateNodeData} = useSvelteFlow();
    const connections = useNodeConnections({handleType: 'target'});
    let {id, data}: NodeProps<ExtractPropertyNodeType> = $props();
    let pathInput: string = $state(data.path);
    let inputIsConnectable = $derived(connections.current.length === 0);
</script>

<NodeWrapper label="Extract Property" class="w-[280px]">
    <div class="flex-col space-y-2 w-full">
        <Handle id="input" type="target" class="node-pin" position={Position.Left}
                isConnectable={inputIsConnectable}/>
        <fieldset class="daisyui-fieldset">
            <legend class="daisyui-fieldset-legend">Path</legend>
            <input class="nodrag daisyui-input w-full" placeholder="Enter a property path (e.g. user.id)..."
                   bind:value={pathInput}
                   onchange={() => updateNodeData(id, { path: pathInput })}/>
        </fieldset>
        <Handle id="input" type="source" class="node-pin" position={Position.Right}/>
    </div>


</NodeWrapper>