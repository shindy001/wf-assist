<script lang="ts">
  import { toast } from "svelte-sonner";
  import { useWorkflowEvents } from "../state";

  const { lastEvent } = $derived(useWorkflowEvents());

  $effect(() => {
    toast.info(`${lastEvent?.type} - Workflow ID: ${lastEvent?.workflowId}`);
  });
</script>
