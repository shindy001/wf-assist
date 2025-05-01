<script lang="ts">
    import {FlowNodeType} from "../types";
    import {useDragAndDrop} from "./DragAndDropProvider.svelte";
    import type {ClassValue} from "svelte/elements";

    const props: { class?: ClassValue } = $props();
    const dragAndDropContext = useDragAndDrop();

    const onDragStart = (event: DragEvent, nodeType: string) => {
        if (!event.dataTransfer) {
            return null;
        }

        dragAndDropContext.nodeType = nodeType;
        event.dataTransfer.effectAllowed = "move";
    };

    const nodeTypes = [...Object.values(FlowNodeType)];
</script>

<div class={props.class}>
    <p class="text-lg">Nodes</p>
    <div class="w-full flex flex-wrap gap-3 px-2 py-4 rounded-md ">
        {#each nodeTypes as nodeType}
            <div
                    role="listitem"
                    class="p-4 bg-gray-200 hover:bg-gray-300 rounded-md font-[#222428] cursor-grab translate-px"
                    ondragstart={(event) => onDragStart(event, nodeType)}
                    draggable={true}
            >{nodeType}</div>
        {/each}
    </div>
</div>
