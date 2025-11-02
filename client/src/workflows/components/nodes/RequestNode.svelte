<script lang="ts" module>
  import { WorkflowNodeDataType, type RequestNodeData } from "$lib/types";
  import { type Connection, type Node } from "@xyflow/svelte";
  import type { EdgeBase } from "@xyflow/system";

  export type RequestNodeType = Node<
    RequestNodeData,
    WorkflowNodeDataType.Request
  >;
</script>

<script lang="ts">
  import {
    Handle,
    type NodeProps,
    Position,
    useSvelteFlow,
  } from "@xyflow/svelte";
  import NodeWrapper from "./NodeWrapper.svelte";

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

  function isValidInputConnection(connection: Connection | EdgeBase) {
    return connection.source !== id && connection.targetHandle !== "output";
  }

  function isValidOutputConnection(connection: Connection | EdgeBase) {
    return connection.target !== id && connection.targetHandle === "input";
  }

  $effect(() => {
    updateNode(id, { width: getNodeWidth(), height: getNodeHeight() });
  });
</script>

<NodeWrapper
  label="Request"
  resizable
  minResizableWidth={initialWidth}
  minResizableHeight={initialHeight}
>
  <Handle
    id="input"
    type="target"
    class="node-pin"
    position={Position.Left}
    isValidConnection={isValidInputConnection}
  />
  <Handle
    id="output"
    type="source"
    class="node-pin"
    position={Position.Right}
    isValidConnection={isValidOutputConnection}
  />

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
</NodeWrapper>
