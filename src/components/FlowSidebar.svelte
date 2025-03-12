<script lang="ts">
    import {useDragAndDrop} from "./DragAndDropProvider.svelte";
    import {FlowNodeType} from "../models/nodes/FlowNodeType";
    import type {ClassValue} from "svelte/elements";
    import {Tween} from "svelte/motion";
    import {quintOut} from "svelte/easing";
    import {fade} from "svelte/transition";

    const props: { class?: ClassValue } = $props();
    const dragAndDropContext = useDragAndDrop();
    const collapsedWidth = 60;
    const expandedWidth = 400;
    let isSidebarCollapsed = $state(false);

    const sidebarWidth = new Tween(expandedWidth, {
        duration: 200,
        easing: quintOut
    });

    const toggleSidebar = () => {
        isSidebarCollapsed = !isSidebarCollapsed;
        sidebarWidth.set(isSidebarCollapsed ? collapsedWidth : expandedWidth);
    };

    const onDragStart = (event: DragEvent, nodeType: string) => {
        if (!event.dataTransfer) {
            return null;
        }

        dragAndDropContext.nodeType = nodeType;
        event.dataTransfer.effectAllowed = "move";
    };

    const nodeTypes = [...Object.values(FlowNodeType)];
</script>

<div class={[props.class, "h-full bg-white z-1 border border-gray-200"]}
     style:width={`${sidebarWidth.current}px`}>
    {#if isSidebarCollapsed}
        <div class="p-2 flex justify-end">
            <button aria-label="expand sidebar" class="p-2 rounded-md cursor-pointer hover:bg-gray-100"
                    onclick={toggleSidebar}>
                <svg class="w-6 h-6" aria-hidden="true" xmlns="http://www.w3.org/2000/svg"
                     width="24" height="24" fill="none" viewBox="0 0 24 24">
                    <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="m6 10 1.99994 1.9999-1.99994 2M11 5v14m-7 0h16c.5523 0 1-.4477 1-1V6c0-.55228-.4477-1-1-1H4c-.55228 0-1 .44772-1 1v12c0 .5523.44772 1 1 1Z"/>
                </svg>
            </button>
        </div>
    {:else }
        <div class="p-2 flex justify-end">
            <button aria-label="collapse sidebar" class="p-2 rounded-md cursor-pointer hover:bg-gray-100"
                    onclick={toggleSidebar}>
                <svg class="w-6 h-6" aria-hidden="true" xmlns="http://www.w3.org/2000/svg"
                     width="24" height="24" fill="none" viewBox="0 0 24 24">
                    <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M7.99994 10 6 11.9999l1.99994 2M11 5v14m-7 0h16c.5523 0 1-.4477 1-1V6c0-.55228-.4477-1-1-1H4c-.55228 0-1 .44772-1 1v12c0 .5523.44772 1 1 1Z"/>
                </svg>
            </button>
        </div>
        <aside
                class={["p-4 flex flex-col content-center items-center"]} in:fade>
            <h1 class="text-lg mb-3">You can drag these nodes to the canvas.</h1>
            <hr class="h-[1px] w-full text-gray-200">
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
            <hr class="h-[1px] w-full text-gray-200">
        </aside>
    {/if}
</div>
