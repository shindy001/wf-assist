<script lang="ts">
    import type {ClassValue} from "svelte/elements";
    import {Button} from "$lib/components/ui/button";
    import {Icon} from "$lib/components/ui/icons";
    import {useAppState} from "$lib/stores";
    import {createGetWorkflowIdentitiesQuery} from "../actions/getWorkflowIdentitiesQuery.svelte";

    const props: { class?: ClassValue } = $props();
    const appState = await useAppState();
    const query = createGetWorkflowIdentitiesQuery();

    let contextMenuIsOpen = $state(false);
    let contextMenuPosition = $state<{x: number, y: number}>({x: 0, y: 0});

    function showContextMenu(event: MouseEvent) {
        event.preventDefault();

        contextMenuPosition = { x: event.clientX, y: event.clientY };
        contextMenuIsOpen = true;
    }

    function hideContextMenu() {
        contextMenuIsOpen = false;
    }

</script>

<svelte:window onclick={hideContextMenu} onblur={hideContextMenu}/>
{#if contextMenuIsOpen}
    <div class="p-2 z-50 absolute flex flex-col gap-1 bg-background shadow-xl rounded-md"
         style="left: {contextMenuPosition.x}px; top: {contextMenuPosition.y}px">
        <Button variant="ghost">
            <Icon name="material-symbols--edit-square-outline"/>
            <span>Rename</span>
        </Button>
        <Button variant="ghost">
            <Icon name="material-symbols--delete-outline"/>
            <span>Remove</span>
        </Button>
    </div>
{/if}

<div class={props.class}>
    <div class="flex justify-between items-center">
        <p class="text-lg">Workflows</p>
        <Button variant="ghost" size="icon">
            <Icon name="material-symbols--add" class="size-6" />
        </Button>
    </div>
    <div class="flex flex-col gap-1">
        {#await query()}
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
                            oncontextmenu={showContextMenu}
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