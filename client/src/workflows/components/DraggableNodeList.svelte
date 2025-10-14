<script lang="ts">
    import {WorkflowNodeType} from "$lib/components/types";
    import type {ClassValue} from "svelte/elements";
    import { useSelectedNodeTypeContext } from "$lib/components/ui/state";
    import {Button} from "$lib/components/ui/button";

    const props: { class?: ClassValue } = $props();
    const selectedNodeTypeContext = useSelectedNodeTypeContext();

    const onDragStart = (event: DragEvent, nodeType: WorkflowNodeType) => {
        if (!event.dataTransfer) {
            return null;
        }

        selectedNodeTypeContext.nodeType = nodeType;
        event.dataTransfer.effectAllowed = "move";
    };

    const nodeTypes = [...Object.values(WorkflowNodeType).filter(x => x !== WorkflowNodeType.Default)];
</script>

<div class={props.class}>
    <p class="text-lg">Nodes</p>
    <div class="w-full flex flex-wrap gap-3 px-2 py-4 rounded-md ">
        {#each nodeTypes as nodeType}
            <Button
                    variant="outline"
                    class="p-4 cursor-grab translate-px"
                    ondragstart={(event) => onDragStart(event, nodeType)}
                    draggable={true}
            >{nodeType}</Button>
        {/each}
    </div>
</div>
