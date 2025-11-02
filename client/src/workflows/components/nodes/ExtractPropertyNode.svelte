<script lang="ts" module>
  import {
    type ExtractPropertyNodeData,
    WorkflowNodeDataType,
  } from "$lib/types";
  import { type Node } from "@xyflow/svelte";

  export type ExtractPropertyNodeType = Node<
    ExtractPropertyNodeData,
    WorkflowNodeDataType.ExtractProperty
  >;
</script>

<script lang="ts">
  import NodeWrapper from "./NodeWrapper.svelte";
  import {
    type NodeProps,
    useNodeConnections,
    useSvelteFlow,
  } from "@xyflow/svelte";
  import InputHandle from "./InputHandle.svelte";
  import OutputHandle from "./OutputHandle.svelte";

  const { updateNodeData } = useSvelteFlow();
  const connections = useNodeConnections({ handleType: "target" });
  let { id, data }: NodeProps<ExtractPropertyNodeType> = $props();
  let pathInput = $state(data.path);
  let currentConnectionId = $derived<string | undefined>(
    connections.current[0]?.target,
  );
  let inputIsConnectable = $derived(connections.current.length === 0);

  $effect(() => {
    updateNodeData(id, { targetId: currentConnectionId ?? "" });
  });
</script>

<NodeWrapper label="Extract Property" class="w-[280px]">
  <InputHandle nodeId={id} isConnectable={inputIsConnectable} />
  <OutputHandle nodeId={id} />

  <fieldset>
    <legend>Path</legend>
    <input
      class="nodrag w-full"
      placeholder="Enter a property path (e.g. user.id)..."
      bind:value={pathInput}
      onchange={() => updateNodeData(id, { path: pathInput })}
    />
  </fieldset>
</NodeWrapper>
