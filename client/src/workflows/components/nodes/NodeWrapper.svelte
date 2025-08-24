<script lang="ts">
    import {NodeResizeControl} from "@xyflow/svelte";
    import type {Snippet} from "svelte";
    import type {ClassValue} from "svelte/elements";
    import {Icon} from "$lib/components/ui/icons";

    const props: {
        label: string,
        resizable?: boolean,
        isActive?: boolean,
        minResizableHeight?: number,
        minResizableWidth?: number,
        children?: Snippet,
        class?: ClassValue,
    } = $props();

</script>

<div class={[
    props.class,
    "turbo-node",
    props.isActive ? "turbo-node_gradient-animation" : "turbo-node_gradient"
    ]}>
    <div class="relative bg-white dark:bg-black size-full text-xs p-2 border-b border-solid font-mono font-semibold rounded-md family-mono">
        {props.label}
        <hr class="my-2">
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