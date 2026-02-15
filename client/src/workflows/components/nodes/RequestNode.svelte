<script lang="ts" module>
  import {
    WorkflowNodeDataType,
    RequestType,
    type RequestNodeData,
  } from "$lib/types";
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
  import { untrack } from "svelte";

  const { updateNodeData, updateNode } = useSvelteFlow();
  const requestTypes = [...Object.values(RequestType)];
  const isRequestTypeWithBody = () =>
    props.data.requestType !== RequestType.Get &&
    props.data.requestType !== RequestType.Delete;

  let props: NodeProps<RequestNodeType> = $props();
  let nodeId = $state(untrack(() => props.id));
  let initialData = $state(untrack(() => props.data));
  let selectedRequestType = $state(initialData.requestType ?? RequestType.Get);
  let urlInputText = $state(initialData.url ?? "");
  let requestBodyInputText = $state(initialData.requestBody ?? "");

  // Set default node width after data init
  updateNode(nodeId, { width: 200, height: 300 });
</script>

<TurboNode
  label="Request"
  id={nodeId}
  resizable
  minResizableWidth={200}
  minResizableHeight={300}
>
  <InputHandle {nodeId} />
  <OutputHandle {nodeId} />

  <div class="flex flex-col space-y-2 h-full max-h-full">
    <fieldset>
      <legend>Url</legend>
      <input
        class="nodrag w-full"
        placeholder="Enter a url..."
        bind:value={urlInputText}
        onchange={() => updateNodeData(nodeId, { url: urlInputText })}
      />
    </fieldset>

    <fieldset>
      <legend>Type</legend>
      <select
        class="nodrag w-full"
        bind:value={selectedRequestType}
        onchange={() => {
          updateNodeData(nodeId, { requestType: selectedRequestType });
        }}
      >
        {#each requestTypes as requestType}
          <option class="bg-black" value={requestType}>{requestType}</option>
        {/each}
      </select>
    </fieldset>

    {#if isRequestTypeWithBody()}
      <fieldset class="grow">
        <legend class="justify-between">Body</legend>
        <textarea
          name="request-payload"
          class="nodrag resize-none size-full"
          placeholder="Contents (JSON, XML, etc.)..."
          bind:value={requestBodyInputText}
          onchange={() =>
            updateNodeData(nodeId, { requestBody: requestBodyInputText })}
        >
        </textarea>
      </fieldset>
    {/if}
  </div>
</TurboNode>
