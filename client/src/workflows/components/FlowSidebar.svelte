<script lang="ts">
  import type { ClassValue } from "svelte/elements";
  import { Tween } from "svelte/motion";
  import { quintOut } from "svelte/easing";
  import { fade } from "svelte/transition";
  import WorkflowList from "./WorkflowList.svelte";
  import DraggableNodeList from "./DraggableNodeList.svelte";
  import { Button } from "$lib/components/ui/button";
  import { Icon } from "$lib/components/ui/icons";

  const props: { class?: ClassValue } = $props();
  const collapsedWidth = 60;
  const expandedWidth = 400;
  let isSidebarCollapsed = $state(false);
  const sidebarWidth = new Tween(expandedWidth, {
    duration: 200,
    easing: quintOut,
  });

  const toggleSidebar = () => {
    isSidebarCollapsed = !isSidebarCollapsed;
    sidebarWidth.set(isSidebarCollapsed ? collapsedWidth : expandedWidth);
  };
</script>

<div
  class={[props.class, "h-full z-1 border relative"]}
  style:width={`${sidebarWidth.current}px`}
>
  {#if isSidebarCollapsed}
    <div class="p-4 flex justify-center items-center">
      <Icon name="material-symbols--flowchart-outline-sharp" />
    </div>
    <div class="p-2 flex justify-end">
      <Button
        variant="ghost"
        size="icon"
        class="absolute bottom-0 right-1"
        onclick={toggleSidebar}
      >
        <Icon
          name="material-symbols--left-panel-open-outline-sharp"
          class="size-6"
        />
      </Button>
    </div>
  {:else}
    <div class="p-4 flex justify-center items-center">
      <Icon name="material-symbols--flowchart-outline-sharp" />
      <div class="p-2">WF Assist</div>
    </div>
    <aside class={["flex flex-col"]} in:fade>
      <WorkflowList class="p-4" />
      <hr class="h-[1px] w-full" />
      <DraggableNodeList class="p-4" />
      <hr class="h-[1px] w-full" />
    </aside>
    <div class="p-2 flex justify-end">
      <Button
        variant="ghost"
        size="icon"
        class="absolute bottom-0 right-1"
        onclick={toggleSidebar}
      >
        <Icon
          name="material-symbols--right-panel-open-outline-sharp"
          class="size-6"
        />
      </Button>
    </div>
  {/if}
</div>
