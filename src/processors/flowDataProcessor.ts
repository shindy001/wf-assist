import type {FlowData} from "../models/FlowData";

export function useFlowDataProcessor() {

    /**
     * Converts FlowData to graph of node dependencies and inDegree (incoming edges)
     * @param {FlowData} data - collection of nodes and edges of flow diagram
     * @returns { graph: { [key: string]: string[] }, inDegree: { [key: string]: number }} - execution order of nodeIds
     */
    const buildDependencyGraph = (data: FlowData):
        {
            graph: { [key: string]: string[] },
            inDegree: { [key: string]: number }
        } => {
        let graph: { [key: string]: string[] } = {};
        let inDegree: { [key: string]: number } = {};

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
        /**
         * Calculates dependency graph and then uses Kahn’s Algorithm to topologically order the graph nodes - https://en.wikipedia.org/wiki/Topological_sorting
         * @param {FlowData} data - collection of nodes and edges of flow diagram
         * @returns {string[]} - execution order of nodeIds
         */
        calculateNodeExecutionOrder: (data: FlowData): string[] => {
            const {graph, inDegree} = buildDependencyGraph(data);

            let queue: string[] = [];
            let order: string[] = [];

            Object.keys(inDegree).forEach(nodeId => {
                if (inDegree[nodeId] === 0) {
                    queue.push(nodeId);
                }
            });

            while (queue.length > 0) {
                let current = queue.shift()!;
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