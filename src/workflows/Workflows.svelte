<script>
    import {SvelteFlowProvider} from "@xyflow/svelte";
    import FlowSidebar from "./components/FlowSidebar.svelte";
    import DragAndDropProvider from "./components/DragAndDropProvider.svelte";
    import FlowCanvas from "./components/FlowCanvas.svelte";
</script>

<div class="flex w-full h-full">
    <SvelteFlowProvider>
        <DragAndDropProvider>
            <FlowSidebar/>
            <FlowCanvas/>
        </DragAndDropProvider>
    </SvelteFlowProvider>
</div>