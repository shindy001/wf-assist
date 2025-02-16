<script>
  import {SvelteFlowProvider} from '@xyflow/svelte';
  import Landing from "./landing/Landing.svelte";
</script>

<div style:height="100vh" style:width="100vw">
    <!-- Header -->
    <!-- Sidebar -->
    <SvelteFlowProvider>
        <Landing/> <!-- Content -->
    </SvelteFlowProvider>
    <!-- Footer -->
</div>
