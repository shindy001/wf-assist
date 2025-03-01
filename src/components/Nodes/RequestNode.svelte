<script lang="ts" module>
    import {FlowNodeType} from "../../models/flowNodeType";
    import {type Node} from "@xyflow/svelte";

    export type UrlActionNodeType = Node<{
        url: string,
        requestType: string,
        requestBody: string
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
    <Handle class="!w-2 !h-2" type="target" position={Position.Left} {...data}/>

    <div class="flex-col space-y-2">
        <div class="flex gap-2">
            <p>Url:</p>
            <input bind:value={urlInputText} onchange={() => updateNodeData(id, { url: urlInputText })}
                   class="rounded-sm w-full focus:outline-0 focus:bg-gray-100" placeholder="Enter a url..."/>
        </div>

        <div class="flex gap-2">
            <p>Type:</p>
            <select bind:value={selectedRequestType}
                    onchange={() => updateNodeData(id, { requestType: selectedRequestType })}>
                {#each requestTypes as requestType}
                    <option value={requestType}>{requestType}</option>
                {/each}
            </select>
        </div>
        {#if selectedRequestType !== "GET" }
            <div class="flex gap-2">
                <p>Body: </p>
                <textarea
                        class="nodrag min-w-72 w-full text-nowrap rounded-sm focus:outline-0 bg-gray-50 focus:bg-gray-100"
                        rows="5"
                        bind:value={requestBodyInputText}
                        onchange={() => updateNodeData(id, { requestBody: requestBodyInputText })}></textarea>
            </div>
        {/if}
    </div>


    <Handle class="!w-2 !h-2" type="source" position={Position.Right} {...data}/>
</NodeWrapper>


