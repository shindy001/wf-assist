<script lang="ts" module>
  import {
    RequestType,
    type RequestNode,
  } from "$lib/types";
  import { type Node } from "@xyflow/svelte";

  export type RequestNodeType = Node<RequestNode>;
</script>

<script lang="ts">
  import { type NodeProps, useSvelteFlow } from "@xyflow/svelte";
  import TurboNode from "./TurboNode.svelte";
  import InputHandle from "./InputHandle.svelte";
  import OutputHandle from "./OutputHandle.svelte";
  import { untrack } from "svelte";

  const { updateNodeData } = useSvelteFlow();
  const requestTypes = [...Object.values(RequestType)];
  const isRequestTypeWithBody = () =>
    props.data.requestType !== RequestType.Get &&
    props.data.requestType !== RequestType.Delete;

  let props: NodeProps<RequestNodeType> = $props();
  let initialData = $state(untrack(() => props.data));
  let selectedRequestType = $state(initialData.requestType ?? RequestType.Get);
  let urlInputText = $state(initialData.url ?? "");
  let requestBodyInputText = $state(initialData.requestBody ?? "");
</script>

<TurboNode
  label="Request"
  id={props.id}
  minResizableWidth={200}
  minResizableHeight={300}
>
  <InputHandle nodeId={props.id} />
  <OutputHandle nodeId={props.id} />

  <div class="flex flex-col space-y-2 h-full max-h-full">
    <fieldset>
      <legend>Url</legend>
      <input
        class="nodrag w-full"
        placeholder="Enter a url..."
        bind:value={urlInputText}
        onchange={() => updateNodeData(props.id, { url: urlInputText })}
      />
    </fieldset>

    <fieldset>
      <legend>Type</legend>
      <select
        class="nodrag w-full"
        bind:value={selectedRequestType}
        onchange={() => {
          updateNodeData(props.id, { requestType: selectedRequestType });
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
            updateNodeData(props.id, { requestBody: requestBodyInputText })}
        >
        </textarea>
      </fieldset>
    {/if}
  </div>
</TurboNode>
