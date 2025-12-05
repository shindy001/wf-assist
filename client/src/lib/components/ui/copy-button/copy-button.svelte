<script lang="ts" module>
  import type { Snippet } from "svelte";
  import { cn } from "$lib/utils";
  import { Button, type ButtonProps } from "$lib/components/ui/button";
  import CheckIcon from "@lucide/svelte/icons/check";
  import CopyIcon from "@lucide/svelte/icons/copy";
  import XIcon from "@lucide/svelte/icons/x";
  import { scale } from "svelte/transition";

  export type CopyButtonProps = ButtonProps & {
    text: string;
    tooltip?: string;
    icon?: Snippet<[]>;
    animationDuration?: number;
  };
</script>

<script lang="ts">
  let status = $state<"success" | "failure">();

  let {
    ref = $bindable(null),
    text,
    icon,
    animationDuration = 500,
    tooltip = "",
    variant = "ghost",
    size = "icon",
    class: className,
    tabindex = -1,
    children,
    ...rest
  }: CopyButtonProps = $props();

  async function copy(text: string) {
    try {
      await navigator.clipboard.writeText(text);
      status = "success";
      setTimeout(() => {
        status = undefined;
      }, animationDuration);
    } catch {
      status = "failure";

      setTimeout(() => {
        status = undefined;
      }, animationDuration);
    }
  }
</script>

<Button
  {...rest}
  bind:ref
  {variant}
  {size}
  {tabindex}
  class={cn("flex items-center gap-2", className)}
  type="button"
  name="copy"
  title={tooltip}
  onclick={async () => await copy(text)}
>
  {#if status === "success"}
    <div in:scale={{ duration: animationDuration, start: 0.85 }}>
      <CheckIcon tabindex={-1} class="text-green-700" />
      <span class="sr-only">Copied</span>
    </div>
  {:else if status === "failure"}
    <div in:scale={{ duration: animationDuration, start: 0.85 }}>
      <XIcon tabindex={-1} class="text-red-700" />
      <span class="sr-only">Failed to copy</span>
    </div>
  {:else}
    <div in:scale={{ duration: animationDuration, start: 0.85 }}>
      {#if icon}
        {@render icon()}
      {:else}
        <CopyIcon tabindex={-1} />
      {/if}
      <span class="sr-only">Copy</span>
    </div>
  {/if}
  {@render children?.()}
</Button>
