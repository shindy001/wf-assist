<script lang="ts" module>
  import { type HeadersNode } from "$lib/types";
  import { type Node } from "@xyflow/svelte";

  export type HeadersNodeType = Node<HeadersNode>;
</script>

<script lang="ts">
  import { type NodeProps, useSvelteFlow } from "@xyflow/svelte";
  import TurboNode from "./TurboNode.svelte";
  import InputHandle from "./InputHandle.svelte";
  import OutputHandle from "./OutputHandle.svelte";
  import { Button } from "$lib/components/ui/button";
  import { Icon } from "$lib/components/ui/icons";
  import { untrack } from "svelte";

  const { updateNodeData } = useSvelteFlow();

  let props: NodeProps<HeadersNodeType> = $props();
  let initialData = $state(untrack(() => props.data));
  let headers = $state(
    initialData.headers?.length <= 0
      ? [{ name: "", value: "" }]
      : initialData.headers,
  );

  $effect(() => {
    updateNodeData(props.id, { headers: headers });
  });
</script>

<TurboNode
  label="Set Http Headers"
  id={props.id}
  minResizableWidth={320}
  minResizableHeight={200}
>
  <InputHandle nodeId={props.id} />
  <OutputHandle nodeId={props.id} />

  <div
    class="flex flex-col space-y-2 h-full max-h-full max-w-full overflow-auto"
  >
    <fieldset>
      <div class="flex items-center gap-2">
        <legend>Headers</legend>
        <Button
          class="size-4"
          variant="link"
          onclick={() => headers.push({ name: "", value: "" })}
        >
          <Icon name="material-symbols--add" />
        </Button>
      </div>
      {#each headers as header, i}
        <div class="flex gap-1 px-1">
          <input
            name="key"
            class="nodrag w-20 grow-4"
            placeholder="Name"
            bind:value={header.name}
          />
          <input
            name="value"
            class="nodrag w-20 grow-8"
            placeholder="Value"
            bind:value={header.value}
          />
          <Button variant="link" size="sm" onclick={() => headers.splice(i, 1)}>
            <Icon name="material-symbols--delete-outline" />
          </Button>
        </div>
      {/each}
    </fieldset>
  </div>
</TurboNode>
