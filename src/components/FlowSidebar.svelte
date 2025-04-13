<script lang="ts">
    import {useDragAndDrop} from "./DragAndDropProvider.svelte";
    import {FlowNodeType} from "../models/nodes/FlowNodeType";
    import type {ClassValue} from "svelte/elements";
    import {Tween} from "svelte/motion";
    import {quintOut} from "svelte/easing";
    import {fade} from "svelte/transition";
    import {useWorkflowDataStore} from "../stores/workflowDataStore";

    const props: { class?: ClassValue } = $props();
    const dragAndDropContext = useDragAndDrop();
    const workflowDataStore = useWorkflowDataStore();
    const collapsedWidth = 60;
    const expandedWidth = 400;
    let isSidebarCollapsed = $state(false);
    const sidebarWidth = new Tween(expandedWidth, {
        duration: 200,
        easing: quintOut
    });
    const workflows = $state(["Workflow1", "Workflow2", "Workflow3", "Workflow4", "Workflow5"]);

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

    const addWorkflow = async () => {
        const workflowData = {
            name: `Undefined${Date.now()}`, // Needs unique name
            flowData: {nodes: [], edges: []},
            executionList: []
        };
        const result = await workflowDataStore.addWorkflow(workflowData);
        if (result.isSuccessful) {
            workflows.unshift(workflowData.name);
        } else {
            console.error(result.error);
        }
    }

    const nodeTypes = [...Object.values(FlowNodeType)];
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
                <div class="flex justify-between items-center">
                    <p class="text-lg">Workflows</p>
                    <button aria-label="add workflow" class="p-2 rounded-md cursor-pointer hover:bg-gray-100"
                            onclick={addWorkflow}>
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                             stroke="currentColor" class="size-6">
                            <path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15"/>
                        </svg>
                    </button>
                </div>
                <div class="flex flex-col gap-1">
                    {#each workflows as workflow}
                        <div class="px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                                 stroke="currentColor" class="size-4">
                                <path stroke-linecap="round" stroke-linejoin="round"
                                      d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m2.25 0H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z"/>
                            </svg>
                            <p>{workflow}</p>
                        </div>
                    {/each}
                </div>
            </div>

            <hr class="h-[1px] w-full text-gray-200">

            <div class="p-4">
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
