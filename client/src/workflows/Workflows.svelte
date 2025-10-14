<script>
    import {SvelteFlowProvider} from "@xyflow/svelte";
    import FlowSidebar from "./components/FlowSidebar.svelte";
    import FlowCanvas from "./components/FlowCanvas.svelte";
    import {SelectedNodeTypeProvider} from "$lib/components/ui/state";
</script>

<div class="flex w-full h-full">
    <SvelteFlowProvider>
        <SelectedNodeTypeProvider>
            <FlowSidebar/>
            <FlowCanvas/>
        </SelectedNodeTypeProvider>
    </SvelteFlowProvider>
</div>