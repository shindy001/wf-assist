import {
  type WorkflowIdentity,
  failed,
  type WorkflowNode,
  type WorkflowEdge,
  WorkflowNodeType,
} from "$lib/types";
import { createGetWorkflowIdentitiesQuery } from "../actions/getWorkflowIdentitiesQuery";
import { createGetWorkflowQuery } from "../actions/getWorkflowQuery";
import { createSaveWorkflowCommand } from "../actions/saveWorkflowCommand";
import { getNextId } from "./idGenerator";

class WorkflowsAppState {
  #workflowIdentities = $state<WorkflowIdentity[] | undefined>();
  #selectedWorkflowIdentity = $state<WorkflowIdentity | undefined>();
  #selectedNodeType = $state<WorkflowNodeType>(
    WorkflowNodeType.RequestNode,
  );
  flowCanvasNodes = $state.raw<WorkflowNode[]>([]);
  flowCanvasEdges = $state.raw<WorkflowEdge[]>([]);

  private saveRateLimitInMiliseconds = 1000;
  private saveWorkflowCommand = createSaveWorkflowCommand(
    this.saveRateLimitInMiliseconds,
  );

  get workflowIdentities() {
    return this.#workflowIdentities;
  }

  get selectedWorkflowIdentity() {
    return this.#selectedWorkflowIdentity;
  }

  get selectedNodeType() {
    return this.#selectedNodeType;
  }

  set selectedNodeType(nodeType: WorkflowNodeType) {
    this.#selectedNodeType = nodeType;
  }

  addFlowCanvasNode(node: WorkflowNode) {
    const nextId = getNextId(this.flowCanvasNodes.at(-1)?.id ?? "000");
    const newNode = { ...node, id: nextId };
    this.flowCanvasNodes = [...this.flowCanvasNodes, { ...newNode, data: { ...newNode } }]; // Copy props to data so Custom nodes can use them
  }

  addFlowCanvasEdge(edge: WorkflowEdge) {
    this.flowCanvasEdges = [...this.flowCanvasEdges, edge];
  }

  async fetchWorkflowIdentities() {
    const query = createGetWorkflowIdentitiesQuery();
    const result = await query();
    this.#workflowIdentities = result.data ?? [];
  }

  async setSelectedWorkflow(id?: string) {
    if (!id) {
      this.#selectedWorkflowIdentity = undefined;
      return;
    }

    const query = createGetWorkflowQuery();
    const workflow = (await query(id)).data;

    if (workflow) {
      this.#selectedWorkflowIdentity = {
        id: workflow.id,
        name: workflow.name,
      };

      this.flowCanvasNodes = workflow.data.nodes;
      this.flowCanvasEdges = workflow.data.edges;
    }
  }

  async saveWorkflowData() {
    if (!this.selectedWorkflowIdentity) {
      return failed("Cannot save workflow, no workflow is selected.");
    }

    const newData = {
      nodes: this.flowCanvasNodes,
      edges: this.flowCanvasEdges,
    };
    return await this.saveWorkflowCommand(
      this.selectedWorkflowIdentity.id,
      newData,
    );
  }
}

const instance = new WorkflowsAppState();

export const useWorkflowsAppState = () => instance;
