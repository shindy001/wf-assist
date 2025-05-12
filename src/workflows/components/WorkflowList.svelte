<script lang="ts">
    import {useWorkflowDataStore} from "../stores/workflowDataStore";
    import type {ClassValue} from "svelte/elements";
    import {useAppDataStore} from "../../lib/stores/appDataStore";
    import {onMount} from "svelte";
    import Icon from "../../lib/components/Icon.svelte";

    const props: { class?: ClassValue } = $props();
    const appDataStore = useAppDataStore();
    const workflowDataStore = useWorkflowDataStore();
    let workflowIdentitiesObservable = workflowDataStore.workflowIdentities;
    let contextMenuIsOpen = $state(false);
    let contextMenuPosition = $state<{x: number, y: number}>({x: 0, y: 0});
    let contextMenuWorkflowName = $state<string | null>();
    let contextMenuWorkflowRenameValue = $state<string | null>();
    let showRenameInput = $state(false);
    let showRenameNameAlreadyExistError = $state(false);

    onMount(async () => {
        await initializeActiveWorkflow();
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

    async function initializeActiveWorkflow() {
        const activeWorkflowName = $appDataStore.activeWorkflowName;
        if (activeWorkflowName && (await workflowDataStore.workflowExists(activeWorkflowName)).data === true) {
            return;
        }
        else if ((await workflowDataStore.isEmpty()).data === true) {
            await addWorkflow();
            return;
        }
        else {
            const workflow = (await workflowDataStore.getWorkflowById(1)).data;
            setActiveWorkflow(workflow?.name ?? "");
        }
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

        const activeWorkflowIdentity = $workflowIdentitiesObservable.find(x => x.name === contextMenuWorkflowName);
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
        const nextWorkflow = $workflowIdentitiesObservable.find(x => x.name !== contextMenuWorkflowName);
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
</script>

<svelte:window onclick={hideContextMenu} onblur={hideContextMenu}/>
{#if contextMenuIsOpen}
    <div class="p-2 z-50 daisyui-menu absolute flex flex-col gap-1 bg-white shadow-xl rounded-md"
         style="left: {contextMenuPosition.x}px; top: {contextMenuPosition.y}px">
        <button class="p-2 flex gap-1 items-center hover:cursor-pointer hover:bg-gray-100 rounded-md" onclick={showRenameAction}>
            <Icon name="material-symbols--edit-square-outline"/>
            <span>Rename</span>
        </button>
        <button class="p-2 flex gap-1 items-center hover:cursor-pointer hover:bg-gray-100 rounded-md" onclick={removeWorkflow}>
            <Icon name="material-symbols--delete-outline"/>
            <span>Remove</span>
        </button>
    </div>
{/if}

<div class={props.class}>
    <div class="flex justify-between items-center">
        <p class="text-lg">Workflows</p>
        <button aria-label="add workflow" class="p-2 rounded-md cursor-pointer hover:bg-gray-100"
                onclick={addWorkflow}>
            <Icon name="material-symbols--add"/>
        </button>
    </div>
    <div class="flex flex-col gap-1">
        {#each $workflowIdentitiesObservable as workflow}
            <button
                    class={[
                        "w-full px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100",
                        workflow.name === $appDataStore.activeWorkflowName ? 'bg-gray-100' : '',
                        workflow.name === contextMenuWorkflowName && showRenameInput ? 'hidden' : ''

                    ]}
                    onclick={() => setActiveWorkflow(workflow.name)}
                    oncontextmenu={showContextMenu}
            >
                <Icon name="material-symbols--folder-data-outline-sharp"/>
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