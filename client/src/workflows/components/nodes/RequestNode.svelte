<script lang="ts" module>
  import { WorkflowNodeDataType, type RequestNodeData } from "$lib/types";
  import { type Node } from "@xyflow/svelte";

  export type RequestNodeType = Node<
    RequestNodeData,
    WorkflowNodeDataType.Request
  >;
</script>

<script lang="ts">
  import { type NodeProps, useSvelteFlow } from "@xyflow/svelte";
  import TurboNode from "./TurboNode.svelte";
  import InputHandle from "./InputHandle.svelte";
  import OutputHandle from "./OutputHandle.svelte";

  const { updateNodeData, updateNode } = useSvelteFlow();
  const requestTypes = ["GET", "POST", "PUT"];
  const isGetRequestType = () => data.requestType === "GET";
  const getNodeWidth = () => (isGetRequestType() ? 200 : 320);
  const getNodeHeight = () => (isGetRequestType() ? 300 : 450);

  let { id, data }: NodeProps<RequestNodeType> = $props();
  let selectedRequestType = $state(data.requestType ?? "GET");
  let urlInputText = $state(data.url ?? "");
  let requestBodyInputText = $state(data.requestBody ?? "");
  let initialWidth = $derived(getNodeWidth());
  let initialHeight = $derived(getNodeHeight());

  $effect(() => {
    updateNode(id, { width: getNodeWidth(), height: getNodeHeight() });
  });
</script>

<TurboNode
  label="Request"
  resizable
  minResizableWidth={initialWidth}
  minResizableHeight={initialHeight}
>
  <InputHandle nodeId={id} />
  <OutputHandle nodeId={id} />

  <div class="flex-col space-y-2 w-full">
    <fieldset>
      <legend>Url</legend>
      <input
        class="nodrag w-full"
        placeholder="Enter a url..."
        bind:value={urlInputText}
        onchange={() => updateNodeData(id, { url: urlInputText })}
      />
    </fieldset>

    <fieldset>
      <legend>Type</legend>
      <select
        class="nodrag w-full"
        bind:value={selectedRequestType}
        onchange={() => {
          updateNodeData(id, { requestType: selectedRequestType });
        }}
      >
        {#each requestTypes as requestType}
          <option value={requestType}>{requestType}</option>
        {/each}
      </select>
    </fieldset>

    {#if !isGetRequestType()}
      <fieldset>
        <legend class="w-full flex justify-between">Body</legend>
        <textarea
          class="nodrag min-w-72 w-full text-nowrap resize-none"
          rows="5"
          placeholder="Contents (JSON, XML, etc.)..."
          bind:value={requestBodyInputText}
          onchange={() =>
            updateNodeData(id, { requestBody: requestBodyInputText })}
        >
        </textarea>
      </fieldset>
    {/if}
  </div>
</TurboNode>
