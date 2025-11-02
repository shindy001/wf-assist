<script lang="ts" module>
  import { WorkflowNodeDataType, type PrintTextNodeData } from "$lib/types";
  import { type Node } from "@xyflow/svelte";

  export type PrintStringNodeType = Node<
    PrintTextNodeData,
    WorkflowNodeDataType.PrintText
  >;
</script>

<script lang="ts">
  import TurboNode from "./TurboNode.svelte";
  import {
    type NodeProps,
    useNodeConnections,
    useSvelteFlow,
  } from "@xyflow/svelte";
  import InputHandle from "./InputHandle.svelte";

  const { updateNodeData } = useSvelteFlow();
  let { id }: NodeProps<PrintStringNodeType> = $props();

  const connections = useNodeConnections({ handleType: "target" });
  let currentConnectionId = $derived(connections.current[0]?.source);
  let inputIsConnectable = $derived(connections.current.length === 0);

  $effect(() => {
    updateNodeData(id, { targetId: currentConnectionId });
  });
</script>

<TurboNode label="Print String" class="w-38">
  <InputHandle nodeId={id} isConnectable={inputIsConnectable} />

  <div class="flex-col space-y-2 w-full"></div>
</TurboNode>
