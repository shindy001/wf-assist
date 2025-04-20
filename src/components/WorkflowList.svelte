<script lang="ts">
    import {useWorkflowDataStore} from "../stores/workflowDataStore";
    import type {ClassValue} from "svelte/elements";
    import {useAppDataStore} from "../stores/appDataStore";
    import {onMount} from "svelte";

    const props: { class?: ClassValue } = $props();
    const appDataStore = useAppDataStore();
    const workflowDataStore = useWorkflowDataStore();
    const workflowNames = workflowDataStore.workflowNames;

    onMount(() => {
        const subscription = workflowNames.subscribe(value => {
            if (value && !value.includes($appDataStore.activeWorkflowId)) {
                appDataStore.set({activeWorkflowId: value[0]})
                subscription.unsubscribe();
            }
        });
    });

    const setActiveWorkflow = (id: string) => {
        appDataStore.set({activeWorkflowId: id})
    }

    const addWorkflow = async () => {
        const workflowData = {
            name: `Undefined${Date.now()}`, // Needs unique name
            flowData: {nodes: [], edges: []},
            executionList: []
        };
        const result = await workflowDataStore.addWorkflow(workflowData);
        if (!result.isSuccessful) {
            console.error(result.error);
        }
    }
</script>

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
        {#each $workflowNames as workflow}
            <button
                    class={[
                        "px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100",
                        workflow === $appDataStore.activeWorkflowId ? 'bg-gray-100' : ''
                    ]}
                    onclick={() => setActiveWorkflow(workflow)}
            >
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                     stroke="currentColor" class="size-4">
                    <path stroke-linecap="round" stroke-linejoin="round"
                          d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m2.25 0H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z"/>
                </svg>
                <p>{workflow}</p>
            </button>
        {:else}
            <div class="px-2 flex gap-2 items-center content-center rounded-md cursor-pointer hover:bg-gray-100">
                <p>No workflows yet, try adding one.</p>
            </div>
        {/each}
    </div>
</div>