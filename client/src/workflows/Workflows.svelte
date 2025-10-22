<script>
    import {SvelteFlowProvider} from "@xyflow/svelte";
    import FlowSidebar from "./components/FlowSidebar.svelte";
    import FlowCanvas from "./components/FlowCanvas.svelte";
    import {FlowCanvasContextProvider} from "./state";
</script>

<SvelteFlowProvider>
    <FlowCanvasContextProvider>
        <div class="flex size-full">
            <FlowSidebar/>
            <FlowCanvas/>
        </div>
    </FlowCanvasContextProvider>
</SvelteFlowProvider>