<script module lang="ts">
    import {getContext, setContext} from "svelte";
    import {WorkflowNodeType} from "$lib/components/types";

    const contextKey = Symbol("SelectedNodeTypeContext");

    export const useSelectedNodeTypeContext = () => {
        return getContext(contextKey) as { nodeType: WorkflowNodeType };
    }
</script>

<script lang="ts">
    let { children } = $props();

    setContext(contextKey, { nodeType: WorkflowNodeType.Default });
</script>

{@render children()}