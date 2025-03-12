<script lang="ts" module>
    import {FlowNodeType} from "../../models/FlowNodeType";
    import {type Node} from "@xyflow/svelte";

    export type PrintStringNodeType = Node<{
        useLogger: boolean,
        targetId: string,
    }, FlowNodeType.PrintString>;
</script>
<script lang="ts">
    import NodeWrapper from "./NodeWrapper.svelte";
    import {Handle, type NodeProps, Position, useNodeConnections, useSvelteFlow} from "@xyflow/svelte";

    const {updateNodeData} = useSvelteFlow();
    let {id, data}: NodeProps<PrintStringNodeType> = $props();

    const connections = useNodeConnections({handleType: 'target'});
    let currentConnectionId = $derived(connections.current[0]?.target);
    let inputIsConnectable = $derived(connections.current.length === 0);
    let useLogger: boolean = $state(data.useLogger ?? false);

    $effect(() => {
        updateNodeData(id, {useLogger: useLogger, targetId: currentConnectionId});
    });
</script>

<NodeWrapper label="Print String" class="w-38">
    <div class="flex-col space-y-2 w-full">
        <Handle id="input" type="target" class="node-pin" position={Position.Left}
                isConnectable={inputIsConnectable}/>
        <div class="flex gap-2 items-center">
            <p class="font-bold">Use logger</p>
            <input type="checkbox" bind:checked={useLogger} class="nodrag daisyui-checkbox"/>
        </div>
    </div>
</NodeWrapper>