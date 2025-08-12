<script lang="ts" module>
    import {WorkflowNodeType, type RequestNodeData} from "../../types";
    import {type Connection, type Node} from "@xyflow/svelte";
    import type {EdgeBase} from "@xyflow/system"

    export type RequestNodeType = Node<RequestNodeData, WorkflowNodeType.Request>;
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

    let {id, data}: NodeProps<RequestNodeType> = $props();
    let selectedRequestType = $state(data.requestType ?? "GET");
    let urlInputText = $state(data.url ?? "");
    let requestBodyInputText = $state(data.requestBody ?? "");
    let inputIsConnectable = $derived(connections.current.length === 0);
    let initialWidth = $derived(getNodeWidth());
    let initialHeight = $derived(getNodeHeight());

    function isValidInputConnection(connection: Connection | EdgeBase) {
        return connection.source !== id;
    }

    function isValidOutputConnection(connection: Connection | EdgeBase) {
        return connection.targetHandle === "input-flow-pin";
    }

    $effect(() => {
        updateNode(id, {width: getNodeWidth(), height: getNodeHeight()});
    });

    $effect(() => {
        const data: RequestNodeData = {
            id: id,
            url: urlInputText,
            requestType: selectedRequestType,
            requestBody: requestBodyInputText
        };
        updateNodeData(id, data);
    });
</script>

<NodeWrapper label="Request"
             resizable
             minResizableWidth={initialWidth}
             minResizableHeight={initialHeight}>
    <div class="space-y-2 w-full">
        <div class="p-1">
            <div class="relative">
                <Handle id="input-flow-pin" type="target" position={Position.Left}
                        isValidConnection={isValidInputConnection}
                        isConnectable={inputIsConnectable}/>
                <Handle id="output-flow-pin" type="source" position={Position.Right}
                        isValidConnection={isValidOutputConnection}/>
            </div>
        </div>

        <fieldset class="">
            <legend class="">Url</legend>
            <input class="nodrag w-full" placeholder="Enter a url..." bind:value={urlInputText}
                   onchange={() => updateNodeData(id, { url: urlInputText })}/>
        </fieldset>

        <fieldset class="">
            <legend class="">Type</legend>
            <select class="nodrag w-full" bind:value={selectedRequestType}
                    onchange={() =>{
                        updateNodeData(id, { requestType: selectedRequestType });
                    } }>
                {#each requestTypes as requestType}
                    <option value={requestType}>{requestType}</option>
                {/each}
            </select>
        </fieldset>

        {#if !isGetRequestType() }
            <fieldset>
                <legend class="w-full flex justify-between">Body</legend>
                <textarea
                        class="nodrag min-w-72 w-full text-nowrap resize-none"
                        rows="5"
                        placeholder="Contents (JSON, XML, etc.)..."
                        bind:value={requestBodyInputText}
                        onchange={() => updateNodeData(id, { requestBody: requestBodyInputText }) }>
                </textarea>
            </fieldset>
        {/if}
        <hr class="text-gray-100">
        <div class="p-1">
            <div class="relative">
                <p class="">Result value</p>
                <Handle id="result" class="!bg-blue-300" type="source"
                        position={Position.Right}/>
            </div>
        </div>
    </div>
</NodeWrapper>


