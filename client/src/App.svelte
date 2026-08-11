<script>
  import "./app.css";
  import Workflows from "./workflows/Workflows.svelte";
  import { Toaster } from "$lib/components/ui/toaster";
  import HiddenResources from "$lib/HiddenResources.svelte";
</script>

<HiddenResources />

<Workflows />
<Toaster />
