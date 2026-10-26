<script lang="ts">
  import { Handle, Position, type Connection } from "@xyflow/svelte";
  import type { EdgeBase } from "@xyflow/system";

  let { nodeId, isConnectable }: { nodeId: string; isConnectable?: boolean } =
    $props();

  function isValidInputConnection(connection: Connection | EdgeBase) {
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
