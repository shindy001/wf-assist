<script module lang="ts">
    import {getContext} from "svelte";

    export const useDragAndDrop = () => {
        return getContext('DragAndDrop') as { nodeType: string | null };
    }
</script>

<script lang="ts">
    import {onDestroy, setContext, type Snippet} from "svelte";

    let {children}: { children: Snippet } = $props();
    let dragAndDropType = $state(null);

    setContext('DragAndDrop', {
        set nodeType(value) {
            dragAndDropType = value;
        },
        get nodeType() {
            return dragAndDropType;
        }
    });

    onDestroy(() => {
        if (dragAndDropType) {
            dragAndDropType = null;
        }
    })
</script>

{@render children()}