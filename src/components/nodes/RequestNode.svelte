<script lang="ts" module>
    import {FlowNodeType} from "../../models/nodes/FlowNodeType";
    import {type Node} from "@xyflow/svelte";

    export type UrlActionNodeType = Node<{
        url: string,
        requestType: string,
        requestBody: string,
    }, FlowNodeType.Request>;
</script>

<script lang="ts">
    import {
        Handle,
        type NodeProps,
        Position,
        useNodeConnections,
        useSvelteFlow
    } from "@xyflow/svelte";
    import NodeWrapper from "./NodeWrapper.svelte";

    const {updateNodeData, updateNode} = useSvelteFlow();
    const connections = useNodeConnections({handleType: 'target'});
    const requestTypes = ["GET", "POST", "PUT"];
    const isGetRequestType = () => data.requestType === "GET";
    const getNodeWidth = () => isGetRequestType() ? 200 : 320;
    const getNodeHeight = () => isGetRequestType() ? 300 : 450;

    let {id, data}: NodeProps<UrlActionNodeType> = $props();
    let selectedRequestType = $state(data.requestType ?? "GET");
    let urlInputText = $state(data.url ?? "");
    let requestBodyInputText = $state(data.requestBody ?? "");
    let inputIsConnectable = $derived(connections.current.length === 0);
    let initialWidth = $derived(getNodeWidth());
    let initialHeight = $derived(getNodeHeight());

    $effect(() => {
        updateNode(id, {width: getNodeWidth(), height: getNodeHeight()});
    })
</script>

<NodeWrapper label="Request"
             resizable
             minResizableWidth={initialWidth}
             minResizableHeight={initialHeight}>
    <div class="flex-col space-y-2 w-full">
        <div class="p-1">
            <div class="relative">
                <Handle id="input" class="input-flow-pin" type="target" position={Position.Left}
                        isConnectable={inputIsConnectable}/>
                <Handle id="output" class="output-flow-pin" type="source" position={Position.Right}/>
            </div>
        </div>

        <fieldset class="daisyui-fieldset">
            <legend class="daisyui-fieldset-legend">Url</legend>
            <input class="nodrag daisyui-input w-full" placeholder="Enter a url..." bind:value={urlInputText}
                   onchange={() => updateNodeData(id, { url: urlInputText })}/>
        </fieldset>

        <fieldset class="daisyui-fieldset">
            <legend class="daisyui-fieldset-legend">Type</legend>
            <select class="nodrag daisyui-select w-full" bind:value={selectedRequestType}
                    onchange={() =>{
                        updateNodeData(id, { requestType: selectedRequestType });
                    } }>
                {#each requestTypes as requestType}
                    <option value={requestType}>{requestType}</option>
                {/each}
            </select>
        </fieldset>

        {#if !isGetRequestType() }
            <fieldset class=" daisyui-fieldset">
                <legend class="daisyui-fieldset-legend w-full flex justify-between">Body</legend>
                <textarea
                        class="nodrag daisyui-textarea min-w-72 w-full text-nowrap resize-none"
                        rows="5"
                        placeholder="Contents (JSON, XML, etc.)..."
                        bind:value={requestBodyInputText}
                        onchange={() => updateNodeData(id, { requestBody: requestBodyInputText }) }>
                </textarea>
            </fieldset>
        {/if}
        <hr class="text-gray-100">
        <div class="">
            <div class="relative flex justify-end">
                <p class="mr-4">Result value</p>
                <Handle id="result" class="node-pin !bg-blue-300 -translate-x-1/2" type="source"
                        position={Position.Right}/>
            </div>
        </div>
    </div>
</NodeWrapper>


