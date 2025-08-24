<script>
    import {SvelteFlowProvider} from "@xyflow/svelte";
    import FlowSidebar from "./components/FlowSidebar.svelte";
    import FlowCanvas from "./components/FlowCanvas.svelte";
    import {DragAndDropProvider} from "$lib/components/ui/dragAndDrop";
</script>

<div class="flex w-full h-full">
    <SvelteFlowProvider>
        <DragAndDropProvider>
            <FlowSidebar/>
            <FlowCanvas/>
        </DragAndDropProvider>
    </SvelteFlowProvider>
</div>