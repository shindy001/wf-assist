<script lang="ts">
    import {NodeResizeControl} from "@xyflow/svelte";
    import type {Snippet} from "svelte";

    let {label, minHeight = 250, minWidth = 250, children}: {
        label: string,
        minHeight?: number,
        minWidth?: number,
        children?: Snippet
    } = $props();

</script>

<div
        class="flex flex-col border border-solid border-gray-200 h-full rounded-2xl bg-white/70 shadow-[0_7px_9px_0_rgba(0,0,0,0.02)]"
>
    <div
            class="text-xs px-3 py-2 border-b border-solid border-gray-200 font-mono font-semibold rounded-t-2xl family-mono text-[#0F172A]"
    >
        {label}
    </div>
    <div
            class="relative h-full bg-white p-3 flex rounded-b-2xl family-sans font-light text-xs text-[#757575]"
    >
        {@render children?.()}
    </div>
    <NodeResizeControl
            minHeight={minHeight}
            minWidth={minWidth}
            style="background: transparent; border: none;"
    >
        <svg
                xmlns="http://www.w3.org/2000/svg"
                width="16"
                height="16"
                viewBox="0 0 24 24"
                stroke-width="2"
                stroke="gray"
                fill="none"
                stroke-linecap="round"
                stroke-linejoin="round"
                style="position: absolute; right: 5px; bottom: 5px;"
        >
            <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
            <polyline points="16 20 20 20 20 16"/>
            <line x1="14" y1="14" x2="20" y2="20"/>
            <polyline points="8 4 4 4 4 8"/>
            <line x1="4" y1="4" x2="10" y2="10"/>
        </svg>
    </NodeResizeControl>
</div>