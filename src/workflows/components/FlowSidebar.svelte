<script lang="ts">
    import type {ClassValue} from "svelte/elements";
    import {Tween} from "svelte/motion";
    import {quintOut} from "svelte/easing";
    import {fade} from "svelte/transition";
    import WorkflowList from "./WorkflowList.svelte";
    import DraggableNodeList from "./DraggableNodeList.svelte";
    import Icon from "../../lib/components/Icon.svelte";

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
            <Icon name="material-symbols--flowchart-outline-sharp"/>
        </div>
        <div class="p-2 flex justify-end">
            <button aria-label="expand sidebar"
                    class="p-2 rounded-md cursor-pointer hover:bg-gray-100  absolute bottom-0 right-0"
                    onclick={toggleSidebar}>
                <Icon name="material-symbols--left-panel-open-outline-sharp"/>
            </button>
        </div>
    {:else }
        <div class="p-4 flex justify-center items-center">
            <Icon name="material-symbols--flowchart-outline-sharp"/>
            <div class="p-2">WF Assist</div>
        </div>
        <aside class={["flex flex-col"]} in:fade>
            <WorkflowList class="p-4"/>
            <hr class="h-[1px] w-full text-gray-200">
            <DraggableNodeList class="p-4"/>
            <hr class="h-[1px] w-full text-gray-200">
        </aside>
        <div class="p-2 flex justify-end absolute bottom-0 right-0">
            <button aria-label="collapse sidebar" class="rounded-md cursor-pointer hover:bg-gray-100"
                    onclick={toggleSidebar}>
                <Icon name="material-symbols--right-panel-open-outline-sharp"/>
            </button>
        </div>
    {/if}
</div>
