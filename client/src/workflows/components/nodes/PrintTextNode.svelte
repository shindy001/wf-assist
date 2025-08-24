<script lang="ts" module>
    import {WorkflowNodeType, type SvelteFlowPrintTextNodeData} from "$lib/components/types";
    import {type Node} from "@xyflow/svelte";

    export type PrintStringNodeType = Node<SvelteFlowPrintTextNodeData, WorkflowNodeType.PrintText>;
</script>

<script lang="ts">
    import NodeWrapper from "./NodeWrapper.svelte";
    import {Handle, type NodeProps, Position, useNodeConnections, useSvelteFlow} from "@xyflow/svelte";
    import {createSvelteFlowPrintTextNodeData} from "$lib/components/types";

    const {updateNodeData} = useSvelteFlow();
    let {id}: NodeProps<PrintStringNodeType> = $props();

    const connections = useNodeConnections({handleType: "target"});
    let currentConnectionId = $derived(connections.current[0]?.source);
    let inputIsConnectable = $derived(connections.current.length === 0);

    $effect(() => {
        const data: SvelteFlowPrintTextNodeData = createSvelteFlowPrintTextNodeData({ targetId: currentConnectionId });
        updateNodeData(id, data);
    });
</script>

<NodeWrapper label="Print String" class="w-38">
    <div class="flex-col space-y-2 w-full">
        <Handle id="input" type="target" class="node-pin" position={Position.Left}
                isConnectable={inputIsConnectable}/>
    </div>
</NodeWrapper>