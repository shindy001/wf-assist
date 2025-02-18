<script lang="ts">
    import {useDragAndDrop} from "./DragAndDropProvider.svelte";
    import {FlowNodeType} from "../models/flowNodeType";

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

<div class={[$$props.class, "w-[400px] h-full bg-white z-1"]}>
    <aside class="p-4 flex flex-col content-center items-center">
        <h1 class="text-lg mb-3">You can drag these nodes to the canvas.</h1>
        <hr class="h-[1px] w-full text-gray-200">
        <div class="w-full flex flex-wrap gap-3 px-2 py-4 rounded-md ">
            {#each nodeTypes as nodeType}
                <div
                        role="listitem"
                        class="p-4 bg-gray-200 hover:bg-gray-300 rounded-md font-[#222428] cursor-grab translate-px"
                        on:dragstart={(event) => onDragStart(event, nodeType)}
                        draggable={true}
                >{nodeType}</div>
            {/each}
        </div>
        <hr class="h-[1px] w-full text-gray-200">
    </aside>
</div>
