import type { Node, Edge } from "@xyflow/svelte";

interface FlowData {
  nodes: Node[];
  edges: Edge[];
}

type ExecutionItem = {
  nodeId: string;
  nodeType: string;
  nodeData?: Record<string, unknown>;
};

export function useFlowDataProcessor() {
  /**
   * Converts FlowData to graph of node dependencies and inDegree (incoming edges)
   * @param {FlowData} data - collection of nodes and edges of flow diagram
   * @returns { graph: { [key: string]: string[] }, inDegree: { [key: string]: number }} - graph of nodes with edges
   * @remarks inDegree = the number of incoming edges to a node (vertex)
   */
  const buildDependencyGraph = (
    data: FlowData,
  ): {
    graph: { [key: string]: string[] };
    inDegree: { [key: string]: number };
  } => {
    let graph: { [key: string]: string[] } = {};
    let inDegree: { [key: string]: number } = {};

    data.nodes.forEach((node) => {
      graph[node.id] = [];
      inDegree[node.id] = 0;
    });

    data.edges.forEach((edge) => {
      graph[edge.source].push(edge.target);
      inDegree[edge.target]++;
    });

    return { graph, inDegree };
  };

  /**
   * Calculates dependency graph and then uses Kahn’s Algorithm to topologically order the graph nodes - https://en.wikipedia.org/wiki/Topological_sorting
   * @param {FlowData} data - collection of nodes and edges of flow diagram
   * @returns {string[]} - calculated execution order of nodes specified by node Ids
   */
  const calculateNodeExecutionOrder = (data: FlowData): string[] => {
    const { graph, inDegree } = buildDependencyGraph(data);
    let queue: string[] = [];
    let order: string[] = [];

    Object.keys(inDegree).forEach((nodeId) => {
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
  };

  /**
   * Calculates dependency graph, topologically orders the nodes and returns nodes according to execution order
   * @param {FlowData} data - collection of nodes and edges of flow diagram
   * @returns Array<ExecutionItem> - execution order of nodeIds
   */
  const createExecutionList = (data: FlowData): Array<ExecutionItem> => {
    const nodeExecutionOrder = calculateNodeExecutionOrder(data);

    return nodeExecutionOrder.map((nodeId) => {
      const node = data.nodes.find((node) => node.id === nodeId);
      if (!node?.type) {
        throw Error(
          `Node with id:'${nodeId}' has no type specified, cannot create execution list without node type.`,
        );
      }

      return { nodeId: nodeId, nodeType: node.type, nodeData: node?.data };
    });
  };

  return {
    createExecutionList: createExecutionList,
  };
}
