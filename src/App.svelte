<script>
    import FlowSidebar from "./components/FlowSidebar.svelte";
    import FlowCanvas from "./components/FlowCanvas.svelte";
    import {SvelteFlowProvider} from "@xyflow/svelte";
    import DragAndDropProvider from "./components/DragAndDropProvider.svelte";
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
