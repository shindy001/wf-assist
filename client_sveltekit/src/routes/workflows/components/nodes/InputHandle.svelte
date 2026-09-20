<script lang="ts">
  import { Handle, Position, type Connection } from "@xyflow/svelte";
  import type { Edge } from "@xyflow/svelte";

  let { nodeId, isConnectable }: { nodeId: string; isConnectable?: boolean } =
    $props();

  function isValidInputConnection(connection: Connection | Edge) {
    return connection.source !== nodeId && connection.targetHandle !== "output";
  }
</script>

<Handle
  id="input"
  type="target"
  class="node-pin"
  position={Position.Left}
  {isConnectable}
  isValidConnection={isValidInputConnection}
/>
