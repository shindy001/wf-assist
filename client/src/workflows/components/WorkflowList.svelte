<script lang="ts" module>
    import {useAppState} from "../../lib/stores/appState.svelte";

    const appState = await useAppState();
</script>

<script lang="ts">
    import type {ClassValue} from "svelte/elements";
    import Icon from "../../lib/components/Icon.svelte";

    const props: { class?: ClassValue } = $props();

    let contextMenuIsOpen = $state(false);
    let contextMenuPosition = $state<{x: number, y: number}>({x: 0, y: 0});
    let contextMenuWorkflowName = $state<string | null>();
    let contextMenuWorkflowRenameValue = $state<string | null>();
    let showRenameInput = $state(false);
    let showRenameNameAlreadyExistError = $state(false);

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
</script>

<svelte:window onclick={hideContextMenu} onblur={hideContextMenu}/>
{#if contextMenuIsOpen}
    <div class="p-2 z-50 absolute flex flex-col gap-1 bg-white shadow-xl rounded-md"
         style="left: {contextMenuPosition.x}px; top: {contextMenuPosition.y}px">
        <button class="p-2 flex gap-1 items-center hover:cursor-pointer hover:bg-gray-100 rounded-md" onclick={showRenameAction}>
            <Icon name="material-symbols--edit-square-outline"/>
            <span>Rename</span>
        </button>
        <button class="p-2 flex gap-1 items-center hover:cursor-pointer hover:bg-gray-100 rounded-md">
            <Icon name="material-symbols--delete-outline"/>
            <span>Remove</span>
        </button>
    </div>
{/if}

<div class={props.class}>
    <div class="flex justify-between items-center">
        <p class="text-lg">Workflows</p>
        <button aria-label="add workflow" class="p-2 rounded-md cursor-pointer hover:bg-gray-100">
            <Icon name="material-symbols--add"/>
        </button>
    </div>
    <div class="flex flex-col gap-1">
        {#each ["workflow1", "workflow2"] as workflow}
            <button
                    class={[
                        "w-full px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100",
                        workflow === appState.lastActiveWorkflowName ? 'bg-gray-100' : '',
                        workflow === contextMenuWorkflowName && showRenameInput ? 'hidden' : ''
                    ]}
                    onclick={() => appState.setActiveWorkflowName(workflow)}
                    oncontextmenu={showContextMenu}
            >
                <Icon name="material-symbols--folder-data-outline-sharp"/>
                <span class="w-full text-left">{workflow}</span>
            </button>

            {#if workflow === contextMenuWorkflowName && showRenameInput}
                <div class="relative">
                    <form onsubmit={null}>
                        <label for="rename">Name: (at least one a-Z or numeric char)</label>
                        <input id="rename" class="w-full px-2 pr-12 border border-black rounded-md" bind:value={contextMenuWorkflowRenameValue} required pattern="\s*(\S\s*)&lbrace;1,&rbrace;">
                        <button type="submit" class="absolute z-10 right-[1px] bottom-[1px] px-1 rounded-md cursor-pointer bg-gray-300 hover:bg-gray-100 content-center items-center">
                            Save
                        </button>
                    </form>
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