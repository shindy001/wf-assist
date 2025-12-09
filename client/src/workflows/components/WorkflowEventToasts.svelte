<script lang="ts">
  import { toast } from "$lib/components/ui/toaster";
  import { useWorkflowEvents } from "../state";

  const { lastEvent } = $derived(useWorkflowEvents());

  $effect(() => {
    toast.info(`${lastEvent?.type} - Workflow ID: ${lastEvent?.workflowId}`);
  });
</script>
