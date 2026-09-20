<script lang="ts">
  import { type EdgeProps, getBezierPath } from "@xyflow/svelte";

  let {
    sourceX,
    sourceY,
    sourcePosition,
    targetX,
    targetY,
    targetPosition,
    id,
  }: EdgeProps = $props();

  let [edgePath] = $derived.by(() => {
    const xEqual = sourceX === targetX;
    const yEqual = sourceY === targetY;
    return getBezierPath({
      // we need this little hack in order to display the gradient for a straight line
      sourceX: xEqual ? sourceX + 0.0001 : sourceX - 2, // Offset sourceX to avoid gaps between the edge and node pin
      sourceY: yEqual ? sourceY + 0.0001 : sourceY,
      sourcePosition,
      targetX: targetX + 2, // Offset targetX to avoid gaps between the edge and node pin
      targetY,
      targetPosition,
    });
  });
</script>

<path {id} class="svelte-flow__edge-path" d={edgePath} />
