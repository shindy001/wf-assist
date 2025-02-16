<script module lang="ts">
    export const useDragAndDrop = () => {
        return getContext('DragAndDrop') as { current: string | null };
    }
</script>

<script lang="ts">
    import {getContext, onDestroy, setContext, type Snippet} from "svelte";

    let {children}: { children: Snippet } = $props();
    let dragAndDropType = $state(null);

    setContext('DragAndDrop', {
        set current(value) {
            dragAndDropType = value;
        },
        get current() {
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