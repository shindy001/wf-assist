<script lang="ts" module>
    import {FlowNodeType} from "../../models/flowNodeType";
    import {type Node} from "@xyflow/svelte";

    export type ExtractPropertyNodeType = Node<{
        path: string,
    }, FlowNodeType.Request>;
</script>

<script lang="ts">
    import NodeWrapper from "./NodeWrapper.svelte";
    import {Handle, type NodeProps, Position, useNodeConnections, useSvelteFlow} from "@xyflow/svelte";

    const {updateNodeData} = useSvelteFlow();
    const connections = useNodeConnections({handleType: 'target'});
    let {id, data}: NodeProps<ExtractPropertyNodeType> = $props();
    let pathInput: string = $state("");
    let inputIsConnectable = $derived(connections.current.length === 0);

</script>

<NodeWrapper label="Extract Property" minWidth={300} minHeight={150}>
    <div class="flex-col space-y-2 w-full">
        <Handle id="input" type="target" position={Position.Left}
                isConnectable={inputIsConnectable}/>
        <fieldset class="daisyui-fieldset">
            <legend class="daisyui-fieldset-legend">Path</legend>
            <input class="daisyui-input w-full" placeholder="Enter a property path (e.g. user.id)..."
                   bind:value={pathInput}
                   onchange={() => updateNodeData(id, { path: pathInput })}/>
        </fieldset>
        <Handle id="input" type="source" position={Position.Right}/>
    </div>


</NodeWrapper>