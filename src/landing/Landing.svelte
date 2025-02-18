<script>
    import {SvelteFlowProvider} from "@xyflow/svelte";
    import DragAndDropProvider from "../components/DragAndDropProvider.svelte";
    import FlowSidebar from "../components/FlowSidebar.svelte";
    import FlowCanvas from "../components/FlowCanvas.svelte";
</script>

<SvelteFlowProvider>
    <DragAndDropProvider>
        <FlowSidebar class="absolute"></FlowSidebar>
        <FlowCanvas/>
    </DragAndDropProvider>
</SvelteFlowProvider>
