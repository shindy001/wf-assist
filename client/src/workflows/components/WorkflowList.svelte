<script lang="ts">
    import * as Dialog from "$lib/components/ui/dialog/index.js";
    import { Input } from "$lib/components/ui/input/index.js";
    import { Label } from "$lib/components/ui/label/index.js";
    import {Button} from "$lib/components/ui/button";
    import {Icon} from "$lib/components/ui/icons";
    import type {ClassValue} from "svelte/elements";
    import {useAppState} from "$lib/stores";
    import {createGetWorkflowIdentitiesQuery} from "../actions/getWorkflowIdentitiesQuery.svelte";
    import {createRenameWorkflowCommand} from "../actions/renameWorkflowCommand";
    import type {WorkflowIdentity} from "$lib/components/types";

    const props: { class?: ClassValue } = $props();
    const appState = await useAppState();
    const getWorkflowIdentitiesQuery = createGetWorkflowIdentitiesQuery();
    const renameWorkflowCommand = createRenameWorkflowCommand();

    let getWorkflowIdentities = $state(getWorkflowIdentitiesQuery());
    let contextMenuIsOpen = $state(false);
    let contextMenuPosition = $state<{x: number, y: number}>({x: 0, y: 0});
    let contextMenuWorkflowIdentity: WorkflowIdentity | null = null;

    let showRenameDialog = $state(false);
    let newWorkflowName = $state("");

    function showContextMenu(event: MouseEvent, workflowIdentity: WorkflowIdentity) {
        event.preventDefault();

        contextMenuWorkflowIdentity = workflowIdentity;
        newWorkflowName = contextMenuWorkflowIdentity.name;
        contextMenuPosition = { x: event.clientX, y: event.clientY };
        contextMenuIsOpen = true;
    }

    async function renameWorkflow() {
        if (contextMenuWorkflowIdentity === null
            || contextMenuWorkflowIdentity.name === newWorkflowName) {
            return;
        }

        await renameWorkflowCommand(contextMenuWorkflowIdentity.id, newWorkflowName);
        refreshWorkflowIdentities();
        newWorkflowName = "";
    }

    function refreshWorkflowIdentities() {
        getWorkflowIdentities = getWorkflowIdentitiesQuery();
    }

    function hideContextMenu() {
        contextMenuIsOpen = false;
    }

</script>

<svelte:window onclick={hideContextMenu} onblur={hideContextMenu}/>
{#if contextMenuIsOpen}
    <div class="p-2 z-50 absolute flex flex-col gap-1 bg-background shadow-xl rounded-md"
         style="left: {contextMenuPosition.x}px; top: {contextMenuPosition.y}px">
        <Button variant="ghost" onclick={() => showRenameDialog = true}>
            <Icon name="material-symbols--edit-square-outline"/>
            <span>Rename</span>
        </Button>
        <Button variant="ghost">
            <Icon name="material-symbols--delete-outline"/>
            <span>Remove</span>
        </Button>
    </div>
{/if}

<Dialog.Root bind:open={showRenameDialog}>
    <Dialog.Content class="sm:max-w-[425px]">
        <Dialog.Header>
            <Dialog.Title>Edit Workflow</Dialog.Title>
        </Dialog.Header>
        <div class="grid gap-4 py-4">
            <div class="grid grid-cols-4 items-center gap-4">
                <Label for="name" class="text-right">Name</Label>
                <Input bind:value={newWorkflowName} id="name" class="col-span-3" required />
            </div>
        </div>
        <Dialog.Footer>
            <Dialog.Close>
                <Button onclick={() => renameWorkflow()}>Save</Button>
            </Dialog.Close>
        </Dialog.Footer>
    </Dialog.Content>
</Dialog.Root>

<div class={props.class}>
    <div class="flex justify-between items-center">
        <p class="text-lg">Workflows</p>
        <Button variant="ghost" size="icon">
            <Icon name="material-symbols--add" class="size-6" />
        </Button>
    </div>
    <div class="flex flex-col gap-1">
        {#await getWorkflowIdentities}
            <p>Loading...</p>
        {:then data }
            {#if data.identities.length <= 0}
                <div class="px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100">
                    <p>No workflows yet, try adding one.</p>
                </div>
            {:else}
                {#each data.identities as workflowIdentity}
                    <Button
                            variant="ghost"
                            class={[ workflowIdentity.name === appState.selectedWorkflowIdentity.name ? 'bg-accent/50' : ''
                    ]}
                            onclick={() => appState.setSelectedWorkflow(workflowIdentity)}
                            oncontextmenu={(event) => showContextMenu(event, workflowIdentity)}
                    >
                        <Icon name="material-symbols--folder-data-outline-sharp"/>
                        <span class="w-full text-left">{workflowIdentity.name}</span>
                    </Button>
                {/each}
            {/if}
        {:catch error}
            <p>Error loading data: {error.message}</p>
        {/await}
    </div>
</div>
