<script lang="ts">
  import './layout.css';
  import { Toaster } from "$lib/components/ui/toaster";
  import HiddenResources from "$lib/HiddenResources.svelte";

  let { children } = $props();
</script>

<HiddenResources />
<Toaster />

{@render children()}
