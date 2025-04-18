<script lang="ts">
    import type {ClassValue} from "svelte/elements";
    import {Tween} from "svelte/motion";
    import {quintOut} from "svelte/easing";
    import {fade} from "svelte/transition";
    import WorkflowList from "./WorkflowList.svelte";
    import DraggableNodeList from "./DraggableNodeList.svelte";

    const props: { class?: ClassValue } = $props();
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

</script>

<div class={[props.class, "h-full bg-white z-1 border border-gray-200 relative"]}
     style:width={`${sidebarWidth.current}px`}>
    {#if isSidebarCollapsed}
        <div class="p-4 flex justify-center items-center">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                 stroke="currentColor" class="w-6 h-6" width="24" height="24">
                <path stroke-linecap="round" stroke-linejoin="round"
                      d="M4.098 19.902a3.75 3.75 0 0 0 5.304 0l6.401-6.402M6.75 21A3.75 3.75 0 0 1 3 17.25V4.125C3 3.504 3.504 3 4.125 3h5.25c.621 0 1.125.504 1.125 1.125v4.072M6.75 21a3.75 3.75 0 0 0 3.75-3.75V8.197M6.75 21h13.125c.621 0 1.125-.504 1.125-1.125v-5.25c0-.621-.504-1.125-1.125-1.125h-4.072M10.5 8.197l2.88-2.88c.438-.439 1.15-.439 1.59 0l3.712 3.713c.44.44.44 1.152 0 1.59l-2.879 2.88M6.75 17.25h.008v.008H6.75v-.008Z"/>
            </svg>
        </div>
        <div class="p-2 flex justify-end">
            <button aria-label="expand sidebar"
                    class="p-2 rounded-md cursor-pointer hover:bg-gray-100  absolute bottom-0 right-0"
                    onclick={toggleSidebar}>
                <svg class="w-6 h-6" aria-hidden="true" xmlns="http://www.w3.org/2000/svg"
                     width="24" height="24" fill="none" viewBox="0 0 24 24">
                    <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="m6 10 1.99994 1.9999-1.99994 2M11 5v14m-7 0h16c.5523 0 1-.4477 1-1V6c0-.55228-.4477-1-1-1H4c-.55228 0-1 .44772-1 1v12c0 .5523.44772 1 1 1Z"/>
                </svg>
            </button>
        </div>
    {:else }
        <div class="p-4 flex justify-center items-center">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                 stroke="currentColor" class="w-6 h-6" width="24" height="24">
                <path stroke-linecap="round" stroke-linejoin="round"
                      d="M4.098 19.902a3.75 3.75 0 0 0 5.304 0l6.401-6.402M6.75 21A3.75 3.75 0 0 1 3 17.25V4.125C3 3.504 3.504 3 4.125 3h5.25c.621 0 1.125.504 1.125 1.125v4.072M6.75 21a3.75 3.75 0 0 0 3.75-3.75V8.197M6.75 21h13.125c.621 0 1.125-.504 1.125-1.125v-5.25c0-.621-.504-1.125-1.125-1.125h-4.072M10.5 8.197l2.88-2.88c.438-.439 1.15-.439 1.59 0l3.712 3.713c.44.44.44 1.152 0 1.59l-2.879 2.88M6.75 17.25h.008v.008H6.75v-.008Z"/>
            </svg>
            <div class="p-2">WF Assist</div>
        </div>
        <aside class={["flex flex-col"]} in:fade>
            <div class="p-4">
                <WorkflowList/>
            </div>

            <hr class="h-[1px] w-full text-gray-200">

            <DraggableNodeList class="p-4"/>

            <hr class="h-[1px] w-full text-gray-200">
        </aside>
        <div class="p-2 flex justify-end absolute bottom-0 right-0">
            <button aria-label="collapse sidebar" class="rounded-md cursor-pointer hover:bg-gray-100"
                    onclick={toggleSidebar}>
                <svg class="w-6 h-6" aria-hidden="true" xmlns="http://www.w3.org/2000/svg"
                     width="24" height="24" fill="none" viewBox="0 0 24 24">
                    <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M7.99994 10 6 11.9999l1.99994 2M11 5v14m-7 0h16c.5523 0 1-.4477 1-1V6c0-.55228-.4477-1-1-1H4c-.55228 0-1 .44772-1 1v12c0 .5523.44772 1 1 1Z"/>
                </svg>
            </button>
        </div>
    {/if}
</div>
