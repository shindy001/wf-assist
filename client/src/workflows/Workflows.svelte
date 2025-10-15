<script>
    import {SvelteFlowProvider} from "@xyflow/svelte";
    import FlowSidebar from "./components/FlowSidebar.svelte";
    import FlowCanvas from "./components/FlowCanvas.svelte";
    import {FlowCanvasContextProvider} from "./state";
</script>

<div class="flex w-full h-full">
    <SvelteFlowProvider>
        <FlowCanvasContextProvider>
            <FlowSidebar/>
            <FlowCanvas/>
        </FlowCanvasContextProvider>
    </SvelteFlowProvider>
</div>