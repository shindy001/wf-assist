<script lang="ts">
    import {useDragAndDrop} from "./DragAndDropProvider.svelte";
    import {FlowNodeType} from "../models/flowNodeType";

    const type = useDragAndDrop();

    const onDragStart = (event: DragEvent, nodeType: string) => {
        if (!event.dataTransfer) {
            return null;
        }

        type.current = nodeType;
        event.dataTransfer.effectAllowed = "move";
    };
</script>

<div class={$$props.class}>
    <aside>
        <div class="label">You can drag these nodes to the pane below.</div>
        <div class="nodes-container">
            <div
                    role="listitem"
                    class="input-node node"
                    on:dragstart={(event) => onDragStart(event, FlowNodeType.UrlAction)}
                    draggable={true}
            >
                UrlAction Node
            </div>
            <div
                    role="listitem"
                    class="input-node node"
                    on:dragstart={(event) => onDragStart(event, FlowNodeType.Input)}
                    draggable={true}
            >
                Input Node
            </div>
            <div
                    role="listitem"
                    class="default-node node"
                    on:dragstart={(event) => onDragStart(event, FlowNodeType.Default)}
                    draggable={true}
            >
                Default Node
            </div>
            <div
                    role="listitem"
                    class="output-node node"
                    on:dragstart={(event) => onDragStart(event, FlowNodeType.Output)}
                    draggable={true}
            >
                Output Node
            </div>
        </div>
    </aside>
</div>

<style>
    aside {
        width: 100%;
        background: #fff;
        font-size: 12px;
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
    }

    .label {
        margin: 1rem 0;
        font-size: 0.9rem;
    }

    .nodes-container {
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .node {
        margin: 0.5rem;
        border: 1px solid #111;
        padding: 0.5rem 1rem;
        font-weight: 700;
        border-radius: 5px;
        cursor: grab;
    }
</style>