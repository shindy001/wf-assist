<script>
    import {SvelteFlowProvider} from '@xyflow/svelte';
    import Landing from "./landing/Landing.svelte";
    import DragAndDropProvider from "./components/DragAndDropProvider.svelte";
    import FlowSidebar from "./components/FlowSidebar.svelte";
</script>

<div id="content__root" style:height="100vh" style:width="100vw" class="relative">
    <!-- Header -->
    <!-- Sidebar -->
    <SvelteFlowProvider>
        <DragAndDropProvider>
            <FlowSidebar class="absolute z-10 w-full"></FlowSidebar>
            <Landing/> <!-- Content -->
        </DragAndDropProvider>
    </SvelteFlowProvider>
    <!-- Footer -->
</div>
