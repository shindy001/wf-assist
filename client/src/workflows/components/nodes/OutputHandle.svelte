<script lang="ts">
  import { Handle, Position, type Connection } from "@xyflow/svelte";
  import type { EdgeBase } from "@xyflow/system";

  let { nodeId, isConnectable }: { nodeId: string; isConnectable?: boolean } =
    $props();

  function isValidOutputConnection(connection: Connection | EdgeBase) {
    return connection.target !== nodeId && connection.targetHandle === "input";
  }
</script>

<Handle
  id="output"
  type="source"
  class="node-pin"
  position={Position.Right}
  {isConnectable}
  isValidConnection={isValidOutputConnection}
/>
