<script lang="ts" module>
    import {FlowNodeType} from "../../models/flowNodeType";
    import {type Node} from "@xyflow/svelte";

    export type UrlActionNodeType = Node<{
        url: string,
        requestType: string,
        requestBody: string,
    }, FlowNodeType.Request>;
</script>

<script lang="ts">
    import {Handle, type NodeProps, Position, useSvelteFlow} from "@xyflow/svelte";
    import NodeWrapper from "./NodeWrapper.svelte";

    const {updateNodeData} = useSvelteFlow();
    const requestTypes = ["GET", "POST", "PUT"];

    let {id, data}: NodeProps<UrlActionNodeType> = $props();
    let selectedRequestType = $state(data.requestType ?? "GET");
    let urlInputText = $state(data.url ?? "");
    let requestBodyInputText = $state(data.requestBody ?? "");
</script>

<NodeWrapper label="Request">
    <div class="flex-col space-y-2">
        <div class="w-full p-1">
            <div class="relative">
                <Handle id="input" class="input-flow-pin" type="target" position={Position.Left}/>
                <Handle id="output" class="output-flow-pin" type="source" position={Position.Right}/>
            </div>
        </div>

        <fieldset class="daisyui-fieldset">
            <legend class="daisyui-fieldset-legend">Url</legend>
            <input class="daisyui-input" placeholder="Enter a url..." bind:value={urlInputText}
                   onchange={() => updateNodeData(id, { url: urlInputText })}/>
        </fieldset>

        <fieldset class="daisyui-fieldset">
            <legend class="daisyui-fieldset-legend">Type</legend>
            <select class="daisyui-select" bind:value={selectedRequestType}
                    onchange={() => updateNodeData(id, { requestType: selectedRequestType })}>
                {#each requestTypes as requestType}
                    <option value={requestType}>{requestType}</option>
                {/each}
            </select>
        </fieldset>

        {#if selectedRequestType !== "GET" }
            <fieldset class="daisyui-fieldset">
                <legend class="daisyui-fieldset-legend">Body</legend>
                <textarea class="nodrag daisyui-textarea min-w-72 w-full text-nowrap"
                          placeholder="Contents (JSON, XML, etc.)..."
                          bind:value={requestBodyInputText}
                          onchange={() => updateNodeData(id, { requestBody: requestBodyInputText })}></textarea>
            </fieldset>
        {/if}
        <hr class="text-gray-100">
        <div class="">
            <div class="relative flex justify-end">
                <p class="mr-4">Result value</p>
                <Handle id="result" class="node-pin" type="source" position={Position.Right}/>
            </div>
        </div>
    </div>
</NodeWrapper>


