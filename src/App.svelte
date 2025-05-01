<script>
    import FlowSidebar from "./workflows/components/FlowSidebar.svelte";
    import FlowCanvas from "./workflows/components/FlowCanvas.svelte";
    import {SvelteFlowProvider} from "@xyflow/svelte";
    import DragAndDropProvider from "./workflows/components/DragAndDropProvider.svelte";
</script>

<div id="content__root" style:height="100vh" style:width="100vw" class="relative">
    <div class="flex w-full h-full">
        <SvelteFlowProvider>
            <DragAndDropProvider>
                <FlowSidebar></FlowSidebar>
                <FlowCanvas/>
            </DragAndDropProvider>
        </SvelteFlowProvider>
    </div>
</div>
