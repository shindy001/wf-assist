import {FlowNodeType} from "../models/flowNodeType";
import type {FlowData} from "../stores/flowDataStore";

export function useFlowDataProcessor() {

    // TODO - use types for flow order calculation
    const buildDependencyGraph = (data: FlowData) => {
        const inputNodes = data.nodes.filter(x => x.type === FlowNodeType.Input);

        if (inputNodes.length <= 0) {
            console.error("Cannot process graph, there is no input node");
            return {graph: [], inDegree: []};
        }

        if (inputNodes.length > 1) {
            console.error("Cannot process graph, there are multiple input nodes");
            return {graph: [], inDegree: []};
        }

        if (!data.edges.find(x => x.source === inputNodes[0]?.id)) {
            console.error("Cannot process graph, input node is not connected");
            return {graph: [], inDegree: []};
        }

        let graph: any = {};
        let inDegree: any = {};

        data.nodes.forEach(node => {
            graph[node.id] = [];
            inDegree[node.id] = 0;
        });

        data.edges.forEach(edge => {
            graph[edge.source].push(edge.target);
            inDegree[edge.target]++;
        });

        return {graph, inDegree};
    }

    return {
        // uses Kahn’s Algorithm to topologically order the graph nodes, https://en.wikipedia.org/wiki/Topological_sorting
        // TODO - use types
        calculateNodeExecutionOrder: (data: FlowData) => {
            const {graph, inDegree} = buildDependencyGraph(data);

            let queue: any = [];
            let order: any = [];

            Object.keys(inDegree).forEach(nodeId => {
                if (inDegree[nodeId] === 0) {
                    queue.push(nodeId);
                }
            });

            while (queue.length > 0) {
                let current = queue.shift();
                order.push(current);

                graph[current].forEach((nodeId: any) => {
                    inDegree[nodeId]--;
                    if (inDegree[nodeId] === 0) {
                        queue.push(nodeId);
                    }
                });
            }

            return order;
        }
    };
}