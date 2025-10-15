<script lang="ts">
    import type {ClassValue} from "svelte/elements";
    import {Button} from "$lib/components/ui/button";
    import {Icon} from "$lib/components/ui/icons";
    import { createQuery } from "@tanstack/svelte-query";
    import { getWfAssistWorkflowsIdentitiesQuery } from "$api/@tanstack/svelte-query.gen";
    import {useAppState} from "$lib/stores";

    const appState = await useAppState();
    const props: { class?: ClassValue } = $props();
    const query = createQuery(() => getWfAssistWorkflowsIdentitiesQuery());

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
        {#if query.isLoading}
            <p>Loading...</p>
        {:else if query.isError}
            <p>Error: {query.error.message}</p>
        {:else if query.isSuccess}
            {#each query.data.identities as workflowIdentity}
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
            {:else}
                <div class="px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100">
                    <p>No workflows yet, try adding one.</p>
                </div>
            {/each}
        {/if}
    </div>
</div>