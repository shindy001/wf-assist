<script lang="ts">
    import { Handle, Position, type NodeProps} from "@xyflow/svelte";

    let { data }: NodeProps = $props();
</script>

<div>
    <div>
        This is and UrlAction Node
    </div>
    <Handle type="target" position={Position.Left} {...data} />
    <Handle type="source" position={Position.Right} {...data} />
</div>

