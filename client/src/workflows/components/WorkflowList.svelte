<script lang="ts">
  import * as Dialog from "$lib/components/ui/dialog/index.js";
  import * as AlertDialog from "$lib/components/ui/alert-dialog/index.js";
  import { Input } from "$lib/components/ui/input/index.js";
  import { Label } from "$lib/components/ui/label/index.js";
  import { Button } from "$lib/components/ui/button";
  import { Icon } from "$lib/components/ui/icons";
  import type { ClassValue } from "svelte/elements";
  import { type Result, type WorkflowIdentity } from "$lib/components/types";
  import { useAppState } from "$lib/stores";
  import { createGetWorkflowIdentitiesQuery } from "../actions/getWorkflowIdentitiesQuery.svelte";
  import { createRenameWorkflowCommand } from "../actions/renameWorkflowCommand";
  import { createRemoveWorkflowCommand } from "../actions/removeWorkflowCommand";
  import { createAddWorkflowCommand } from "../actions/addWorkflowCommand";

  const props: { class?: ClassValue } = $props();
  const appState = await useAppState();
  const getWorkflowIdentitiesQuery = createGetWorkflowIdentitiesQuery();
  const renameWorkflowCommand = createRenameWorkflowCommand();
  const removeWorkflowCommand = createRemoveWorkflowCommand();
  const addWorkflowCommand = createAddWorkflowCommand();

  let workflowIdentitiesQueryResult = $state<Result<WorkflowIdentity[]>>();
  let contextMenuIsOpen = $state(false);
  let contextMenuPosition = $state<{ x: number; y: number }>({ x: 0, y: 0 });
  let contextMenuWorkflowIdentity: WorkflowIdentity | null = $state(null);

  let showRenameDialog = $state(false);
  let showRemoveWorkflowDialog = $state(false);
  let newWorkflowName = $state("");
  let actionError;

  refreshWorkflowIdentities();

  async function refreshWorkflowIdentities() {
    workflowIdentitiesQueryResult = await getWorkflowIdentitiesQuery();
  }

  function showContextMenu(
    event: MouseEvent,
    workflowIdentity: WorkflowIdentity,
  ) {
    event.preventDefault();

    contextMenuWorkflowIdentity = workflowIdentity;
    newWorkflowName = contextMenuWorkflowIdentity.name;
    contextMenuPosition = { x: event.clientX, y: event.clientY };
    contextMenuIsOpen = true;
  }

  async function addNewWorkflow() {
    await addWorkflowCommand("New Workflow", undefined);
    refreshWorkflowIdentities();
  }

  async function renameWorkflow() {
    if (
      contextMenuWorkflowIdentity === null ||
      contextMenuWorkflowIdentity.name === newWorkflowName
    ) {
      return;
    }

    await renameWorkflowCommand(
      contextMenuWorkflowIdentity.id,
      newWorkflowName,
    );
    refreshWorkflowIdentities();
  }

  async function removeWorkflow() {
    if (
      contextMenuWorkflowIdentity === null ||
      !contextMenuWorkflowIdentity.id
    ) {
      return;
    }

    await removeWorkflowCommand(contextMenuWorkflowIdentity.id);
    refreshWorkflowIdentities();
  }

  function hideContextMenu() {
    contextMenuIsOpen = false;
  }
</script>

<svelte:window onclick={hideContextMenu} onblur={hideContextMenu} />
{#if contextMenuIsOpen}
  <div
    class="p-2 z-50 absolute flex flex-col gap-1 bg-background shadow-xl rounded-md"
    style="left: {contextMenuPosition.x}px; top: {contextMenuPosition.y}px"
  >
    <Button variant="ghost" onclick={() => (showRenameDialog = true)}>
      <Icon name="material-symbols--edit-square-outline" />
      <span>Rename</span>
    </Button>
    <Button variant="ghost" onclick={() => (showRemoveWorkflowDialog = true)}>
      <Icon name="material-symbols--delete-outline" />
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
        <Input
          bind:value={newWorkflowName}
          id="name"
          class="col-span-3"
          required
        />
      </div>
    </div>
    <Dialog.Footer>
      <Button
        onclick={() => renameWorkflow().then(() => (showRenameDialog = false))}
        >Save</Button
      >
    </Dialog.Footer>
  </Dialog.Content>
</Dialog.Root>

<AlertDialog.Root bind:open={showRemoveWorkflowDialog}>
  <AlertDialog.Content interactOutsideBehavior="close">
    <AlertDialog.Header>
      <AlertDialog.Title>Remove Workflow</AlertDialog.Title>
      <AlertDialog.Description>
        Are you sure you want to remove Workflow <b
          >{contextMenuWorkflowIdentity?.name}</b
        >?
      </AlertDialog.Description>
    </AlertDialog.Header>
    <AlertDialog.Footer>
      <AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
      <AlertDialog.Action
        onclick={() =>
          removeWorkflow().then(() => (showRemoveWorkflowDialog = false))}
        >Remove</AlertDialog.Action
      >
    </AlertDialog.Footer>
  </AlertDialog.Content>
</AlertDialog.Root>

<div class={props.class}>
  <div class="flex justify-between items-center">
    <p class="text-lg">Workflows</p>
    <Button variant="ghost" size="icon" onclick={() => addNewWorkflow()}>
      <Icon name="material-symbols--add" class="size-6" />
    </Button>
  </div>
  <div class="flex flex-col gap-1">
    {#if workflowIdentitiesQueryResult === undefined}
      <p>Loading...</p>
    {:else if workflowIdentitiesQueryResult.error}
      <p>Error loading data: {workflowIdentitiesQueryResult.error}</p>
    {:else if (workflowIdentitiesQueryResult.data ?? []).length <= 0}
      <div
        class="px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100"
      >
        <p>No workflows yet, try adding one.</p>
      </div>
    {:else}
      {#each workflowIdentitiesQueryResult.data as workflowIdentity}
        <Button
          variant="ghost"
          class={[
            workflowIdentity.id === appState.selectedWorkflowIdentity.id
              ? "bg-accent/50"
              : "",
          ]}
          onclick={() => appState.setSelectedWorkflow(workflowIdentity)}
          oncontextmenu={(event) => showContextMenu(event, workflowIdentity)}
        >
          <Icon name="material-symbols--folder-data-outline-sharp" />
          <span class="w-full text-left">{workflowIdentity.name}</span>
        </Button>
      {/each}
    {/if}
  </div>
</div>
