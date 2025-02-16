<script lang="ts">
    import { Handle, Position, type NodeProps} from "@xyflow/svelte";

</script>

<div>
    <div>
        This is and UrlAction Node
    </div>
    <Handle type="target" position={Position.Left} />
    <Handle type="source" position={Position.Right} />
</div>

