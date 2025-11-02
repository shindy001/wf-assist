<script lang="ts" module>
  import {
    type ExtractPropertyNodeData,
    WorkflowNodeDataType,
  } from "$lib/types";
  import { type Connection, type Node } from "@xyflow/svelte";
  import type { EdgeBase } from "@xyflow/system";

  export type ExtractPropertyNodeType = Node<
    ExtractPropertyNodeData,
    WorkflowNodeDataType.ExtractProperty
  >;
</script>

<script lang="ts">
  import NodeWrapper from "./NodeWrapper.svelte";
  import {
    Handle,
    type NodeProps,
    Position,
    useNodeConnections,
    useSvelteFlow,
  } from "@xyflow/svelte";

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

  function isValidInputConnection(connection: Connection | EdgeBase) {
    return connection.source !== id && connection.targetHandle !== "output";
  }

  function isValidOutputConnection(connection: Connection | EdgeBase) {
    return connection.target !== id && connection.targetHandle === "input";
  }
</script>

<NodeWrapper label="Extract Property" class="w-[280px]">
  <div class="flex-col space-y-2 w-full">
    <Handle
      id="input"
      type="target"
      class="node-pin"
      position={Position.Left}
      isConnectable={inputIsConnectable}
      isValidConnection={isValidInputConnection}
    />
    <fieldset>
      <legend>Path</legend>
      <input
        class="nodrag w-full"
        placeholder="Enter a property path (e.g. user.id)..."
        bind:value={pathInput}
        onchange={() => updateNodeData(id, { path: pathInput })}
      />
    </fieldset>
    <Handle
      id="output"
      type="source"
      class="node-pin"
      position={Position.Right}
      isValidConnection={isValidOutputConnection}
    />
  </div>
</NodeWrapper>
