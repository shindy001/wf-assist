<script module lang="ts">
    import {getContext} from "svelte";
    import {WorkflowNodeType} from "../../workflows/types";

    export const useDragAndDrop = () => {
        return getContext('DragAndDrop') as { nodeType: WorkflowNodeType };
    }
</script>

<script lang="ts">
    import {setContext, type Snippet} from "svelte";

    // https://svelte.dev/docs/svelte/snippet#Passing-snippets-to-components-Optional-snippet-props
    let { children } = $props();
    let dragAndDropType: WorkflowNodeType = $state(WorkflowNodeType.Default);

    setContext('DragAndDrop', {
        set nodeType(value) {
            dragAndDropType = value;
        },
        get nodeType() {
            return dragAndDropType;
        }
    });
</script>

{@render children?.()}