<script lang="ts">
    import {NodeResizeControl} from "@xyflow/svelte";
    import type {Snippet} from "svelte";
    import type {ClassValue} from "svelte/elements";
    import Icon from "../../../lib/components/Icon.svelte";

    const props: {
        label: string,
        resizable?: boolean,
        minResizableHeight?: number,
        minResizableWidth?: number,
        children?: Snippet,
        class?: ClassValue,
    } = $props();

</script>

<div
        class={[props.class, "flex flex-col border border-solid h-full rounded-2xl"]}
>
    <div
            class="text-xs px-3 py-2 border-b border-solid font-mono font-semibold rounded-t-2xl family-mono"
    >
        {props.label}
    </div>
    <div
            class="relative h-full p-3 flex rounded-b-2xl family-sans font-light text-xs"
    >
        {@render props.children?.()}
    </div>
    {#if props.resizable}
        <NodeResizeControl
                minHeight={props.minResizableHeight}
                minWidth={props.minResizableWidth}
                style="background: transparent; border: none;"
        >
            <Icon class="rotate-45 absolute right-6 bottom-6" name="material-symbols--arrows-outward"/>
        </NodeResizeControl>
    {/if}
</div>