<script lang="ts">
    import {useWorkflowDataService} from "../stores/workflowDataService";
    import type {ClassValue} from "svelte/elements";
    import {useAppDataStore, setActiveWorkflow} from "../../lib/stores/appDataStore";
    import {onMount} from "svelte";
    import Icon from "../../lib/components/Icon.svelte";
    import {createInitializeActiveWorkflowCommand} from "../commands/initializeActiveWorkflowCommand";
    import {createAddEmptyWorkflowCommand} from "../commands/addEmptyWorkflowCommand";
    import {createRemoveWorkflowCommand} from "../commands/removeWorkflowCommand";
    import {createRenameWorkflowCommand} from "../commands/renameWorkflowCommand";
    import {AlreadyExistsError, NotFoundError} from "../../lib/types";

    const props: { class?: ClassValue } = $props();
    const appDataStore = useAppDataStore();
    const workflowDataService = useWorkflowDataService();
    const initializeActiveWorkflowCommand = createInitializeActiveWorkflowCommand(appDataStore, workflowDataService);
    const addEmptyWorkflowCommand = createAddEmptyWorkflowCommand(workflowDataService);
    const removeActiveWorkflowCommand = createRemoveWorkflowCommand(appDataStore, workflowDataService);
    const renameWorkflowCommand = createRenameWorkflowCommand(workflowDataService);
    let workflowIdentitiesObservable = workflowDataService.workflowIdentities;
    let contextMenuIsOpen = $state(false);
    let contextMenuPosition = $state<{x: number, y: number}>({x: 0, y: 0});
    let contextMenuWorkflowName = $state<string | null>();
    let contextMenuWorkflowRenameValue = $state<string | null>();
    let showRenameInput = $state(false);
    let showRenameNameAlreadyExistError = $state(false);

    onMount(async () => {
        await initializeActiveWorkflowCommand();
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

    async function renameWorkflow() {
        if(!contextMenuWorkflowName || !contextMenuWorkflowRenameValue) {
            return;
        }

        const result = await renameWorkflowCommand(contextMenuWorkflowName, contextMenuWorkflowRenameValue);
        switch (result?.constructor) {
            case AlreadyExistsError:
                showRenameNameAlreadyExistError = true;
                break;
            case NotFoundError:
                console.error(`Error while renaming workflow, workflow does not exist. currentWorkflow:
                    '${contextMenuWorkflowName}', newName: '${contextMenuWorkflowRenameValue}'`);
                hideRenameAction();
                break;
            default:
                hideRenameAction();
                return;
        }
    }

    async function removeWorkflow() {
        if (!contextMenuWorkflowName || !confirm(`Delete workflow ${contextMenuWorkflowName}?`)) {
            return;
        }

        await removeActiveWorkflowCommand(contextMenuWorkflowName);
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
                onclick={addEmptyWorkflowCommand}>
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