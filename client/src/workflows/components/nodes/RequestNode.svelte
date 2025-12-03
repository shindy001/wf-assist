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

  const { updateNodeData, updateNode } = useSvelteFlow();
  const requestTypes = [...Object.values(RequestType)];
  const isRequestTypeWithBody = () =>
    data.requestType !== RequestType.Get &&
    data.requestType !== RequestType.Delete;

  let { id, data }: NodeProps<RequestNodeType> = $props();
  let selectedRequestType = $state(data.requestType ?? RequestType.Get);
  let urlInputText = $state(data.url ?? "");
  let requestBodyInputText = $state(data.requestBody ?? "");

  // Set default node width after data init
  updateNode(id, { width: 200, height: 300 });
</script>

<TurboNode
  label={`Request ${id}`}
  resizable
  minResizableWidth={200}
  minResizableHeight={300}
>
  <InputHandle nodeId={id} />
  <OutputHandle nodeId={id} />

  <div class="flex flex-col space-y-2 h-full max-h-full">
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
          <option class="bg-black" value={requestType}>{requestType}</option>
        {/each}
      </select>
    </fieldset>

    {#if isRequestTypeWithBody()}
      <fieldset class="grow">
        <legend class="justify-between">Body</legend>
        <textarea
          name="request-payload"
          class="nodrag resize-none w-full h-[calc(100%-20px)]"
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
