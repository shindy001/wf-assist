<script lang="ts">
    import {WorkflowNodeType} from "../types";
    import {useDragAndDrop} from "../../lib/components/DragAndDropProvider.svelte";
    import type {ClassValue} from "svelte/elements";
    import {Button} from "$lib/components/ui/button";

    const props: { class?: ClassValue } = $props();
    const dragAndDropContext = useDragAndDrop();

    const onDragStart = (event: DragEvent, nodeType: WorkflowNodeType) => {
        if (!event.dataTransfer) {
            return null;
        }

        dragAndDropContext.nodeType = nodeType;
        event.dataTransfer.effectAllowed = "move";
    };

    const nodeTypes = [...Object.values(WorkflowNodeType)];
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
