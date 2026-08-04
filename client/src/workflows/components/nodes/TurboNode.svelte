<script lang="ts">
  import { NodeResizeControl, type NodeProps } from "@xyflow/svelte";
  import { type Snippet } from "svelte";
  import type { ClassValue } from "svelte/elements";
  import { Icon } from "$lib/components/ui/icons";
  import { CopyButton } from "$lib/components/ui/copy-button";
  import * as Tooltip from "$lib/components/ui/tooltip";

  const props: {
    label: string;
    id: string;
    isActive?: boolean;
    minResizableWidth?: number;
    minResizableHeight?: number;
    children?: Snippet;
    class?: ClassValue;
    style?: string;
  } = $props();
</script>

<div
  class={[
    props.class,
    "turbo-node",
    props.isActive ? "turbo-node_gradient-animation" : "turbo-node_gradient",
  ]}

  style={props.minResizableWidth && props.minResizableHeight
    ? `min-width: ${props.minResizableWidth}px; min-height: ${props.minResizableHeight}px; ${props.style}`
    : props.style}
>
  <div
    class="relative flex flex-col bg-white dark:bg-black w-full text-xs p-2 border-b border-solid font-mono font-semibold rounded-md family-mono"
  >
    <div class="flex justify-between items-center">
      <div>{props.label}</div>
    </div>
    <hr class="my-2" />
    {@render props.children?.()}

    {#if props.id}
      <hr class="my-2" />
      <div class="flex gap-1 text-gray-500">
        <Tooltip.Provider>
          <Tooltip.Root>
            <Tooltip.Trigger>
              <CopyButton
                text={`#{node:${props.id}}`}
                class="nodrag p-1 size-fit"
                variant="outline">{`$ref: ${props.id}`}</CopyButton
              >
            </Tooltip.Trigger>
            <Tooltip.Content>
              <p>copy node reference</p>
            </Tooltip.Content>
          </Tooltip.Root>
        </Tooltip.Provider>
      </div>
    {/if}
  </div>

  {#if props.minResizableWidth && props.minResizableHeight}
    <NodeResizeControl
      minHeight={props.minResizableHeight}
      minWidth={props.minResizableWidth}
      style="background: transparent; border: none;"
    >
      <Icon
        class="rotate-45 absolute right-6 bottom-6"
        name="material-symbols--arrows-outward"
      />
    </NodeResizeControl>
  {/if}
</div>
