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
        class={[props.class, "flex flex-col border border-solid border-gray-200 h-full rounded-2xl bg-white/70 shadow-[0_7px_9px_0_rgba(0,0,0,0.02)]"]}
>
    <div
            class="text-xs px-3 py-2 border-b border-solid border-gray-200 font-mono font-semibold rounded-t-2xl family-mono text-[#0F172A]"
    >
        {props.label}
    </div>
    <div
            class="relative h-full bg-white p-3 flex rounded-b-2xl family-sans font-light text-xs text-[#757575]"
    >
        {@render props.children?.()}
    </div>
    {#if props.resizable}
        <NodeResizeControl
                minHeight={props.minResizableHeight}
                minWidth={props.minResizableWidth}
                style="background: transparent; border: none;"
        >
            <Icon class="text-gray-400 rotate-45 absolute right-6 bottom-6" name="material-symbols--arrows-outward"/>
        </NodeResizeControl>
    {/if}
</div>