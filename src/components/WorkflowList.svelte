<script lang="ts">
    import {useWorkflowDataStore} from "../stores/workflowDataStore";
    import type {ClassValue} from "svelte/elements";
    import {useAppDataStore} from "../stores/appDataStore";
    import {onMount} from "svelte";

    const props: { class?: ClassValue } = $props();
    const appDataStore = useAppDataStore();
    const workflowDataStore = useWorkflowDataStore();
    const workflowIdentities = workflowDataStore.workflowIdentities;
    let contextMenuIsOpen = $state(false);
    let contextMenuPosition = $state<{x: number, y: number}>({x: 0, y: 0});
    let contextMenuWorkflowName = $state<string | null>();
    let contextMenuWorkflowRenameValue = $state<string | null>();
    let showRenameInput = $state(false);
    let showRenameNameAlreadyExistError = $state(false);

    onMount(() => {
        initializeActiveWorkflowId();
    });

    function showContextMenu(event: MouseEvent) {
        event.preventDefault();
        hideRenameAction();

        const targetText = (event.target as HTMLElement)?.textContent;
        contextMenuWorkflowName = targetText;
        contextMenuWorkflowRenameValue = targetText;
        contextMenuPosition = { x: event.clientX, y: event.clientY };
        contextMenuIsOpen = true;
    }

    function hideContextMenu() {
        contextMenuIsOpen = false;
    }

    function showRenameAction() {
        showRenameInput = true;
    }

    function hideRenameAction() {
        showRenameInput = false;
        showRenameNameAlreadyExistError = false;
    }

    function initializeActiveWorkflowId() {
        workflowIdentities.subscribe(value => {
            if (value && !value.map(x => x.name).includes($appDataStore.activeWorkflowName)) {
                appDataStore.set({activeWorkflowName: value[0].name})
            }
        }).unsubscribe();
    }

    function setActiveWorkflow(name: string) {
        appDataStore.set({activeWorkflowName: name})
    }

    async function addWorkflow() {
        const workflowData = {
            name: `Undefined${Date.now()}`, // Needs unique name
            flowData: {nodes: [], edges: []},
            executionList: []
        };
        await workflowDataStore.addWorkflow(workflowData);
        setActiveWorkflow(workflowData.name);
    }

    async function renameWorkflow() {
        if(!contextMenuWorkflowName || !contextMenuWorkflowRenameValue) {
            return;
        }

        const activeWorkflowIdentity = $workflowIdentities.find(x => x.name === contextMenuWorkflowName);
        if (!activeWorkflowIdentity
            || contextMenuWorkflowName === contextMenuWorkflowRenameValue) {
            hideRenameAction();
            return;
        }

        const result = await workflowDataStore.workflowExists(contextMenuWorkflowRenameValue);
        if (result.isSuccessful) {
            if (result.data === true) {
                showRenameNameAlreadyExistError = true;
            }
            else {
                await workflowDataStore.renameWorkflow(activeWorkflowIdentity.id, contextMenuWorkflowRenameValue);
                hideRenameAction();
            }
        }
    }

    async function removeWorkflow() {
        if (!contextMenuWorkflowName || !confirm(`Delete workflow ${contextMenuWorkflowName}?`)) {
            return;
        }

        const removingActiveWorkflow = contextMenuWorkflowName === $appDataStore.activeWorkflowName;
        const nextWorkflow = $workflowIdentities.find(x => x.name !== contextMenuWorkflowName);
        const result = await workflowDataStore.deleteWorkflow(contextMenuWorkflowName);

        if (result.isSuccessful) {
            if (removingActiveWorkflow && nextWorkflow) {
                setActiveWorkflow(nextWorkflow.name);
            }

            if (!nextWorkflow) {
                await addWorkflow();
            }
        }
    }
</script>

<svelte:window onclick={hideContextMenu} onblur={hideContextMenu}/>
{#if contextMenuIsOpen}
    <div class="p-2 z-50 daisyui-menu absolute flex flex-col gap-1 bg-white shadow-xl rounded-md"
         style="left: {contextMenuPosition.x}px; top: {contextMenuPosition.y}px">
        <button class="p-2 flex gap-1 items-center hover:cursor-pointer hover:bg-gray-100 rounded-md" onclick={showRenameAction}>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-4">
                <path stroke-linecap="round" stroke-linejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10" />
            </svg>
            <span>Rename</span>
        </button>
        <button class="p-2 flex gap-1 items-center hover:cursor-pointer hover:bg-gray-100 rounded-md" onclick={removeWorkflow}>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-4">
                <path stroke-linecap="round" stroke-linejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
            </svg>
            <span>Remove</span>
        </button>
    </div>
{/if}

<div class={props.class}>
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
        {#each $workflowIdentities as workflow}
            <button
                    class={[
                        "w-full px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100",
                        workflow.name === $appDataStore.activeWorkflowName ? 'bg-gray-100' : '',
                        workflow.name === contextMenuWorkflowName && showRenameInput ? 'hidden' : ''

                    ]}
                    onclick={() => setActiveWorkflow(workflow.name)}
                    oncontextmenu={showContextMenu}
            >
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                     stroke="currentColor" class="size-4">
                    <path stroke-linecap="round" stroke-linejoin="round"
                          d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m2.25 0H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z"/>
                </svg>
                <span class="w-full text-left">{workflow.name}</span>
            </button>

            {#if workflow.name === contextMenuWorkflowName && showRenameInput}
                <div class="relative">
                    <input class="w-full px-2 pr-12 border border-black rounded-md" type="text" bind:value={contextMenuWorkflowRenameValue} required minlength="1">
                    <button aria-label="rename" class="absolute z-10 right-[1px] top-[1px] px-1 rounded-md cursor-pointer bg-gray-300 hover:bg-gray-100 content-center items-center" onclick={renameWorkflow}>
                        Save
                    </button>
                </div>

                {#if showRenameNameAlreadyExistError}
                    <p class="text-red-600">Name already exist.</p>
                {/if}
            {/if}
        {:else}
            <div class="px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100">
                <p>No workflows yet, try adding one.</p>
            </div>
        {/each}
    </div>
</div>