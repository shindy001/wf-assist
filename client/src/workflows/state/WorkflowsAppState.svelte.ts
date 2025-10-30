import {
  type WorkflowIdentity,
  WorkflowNodeDataType,
  failed,
  type SvelteFlowWorkflowNode,
  type SvelteFlowWorkflowEdge,
} from "$lib/components/types";
import { createGetWorkflowIdentitiesQuery } from "../actions/getWorkflowIdentitiesQuery";
import { createGetWorkflowQuery } from "../actions/getWorkflowQuery";
import { createSaveWorkflowCommand } from "../actions/saveWorkflowCommand";

class WorkflowsAppState {
  #workflowIdentities = $state<WorkflowIdentity[] | undefined>();
  #selectedWorkflowIdentity = $state<WorkflowIdentity | undefined>();
  #selectedNodeType = $state<WorkflowNodeDataType>(
    WorkflowNodeDataType.Default,
  );
  flowCanvasNodes = $state.raw<SvelteFlowWorkflowNode[]>([]);
  flowCanvasEdges = $state.raw<SvelteFlowWorkflowEdge[]>([]);

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

  set selectedNodeType(nodeType: WorkflowNodeDataType) {
    this.#selectedNodeType = nodeType;
  }

  addFlowCanvasNode(node: SvelteFlowWorkflowNode) {
    this.flowCanvasNodes = [...this.flowCanvasNodes, node];
  }

  addFlowCanvasEdge(edge: SvelteFlowWorkflowEdge) {
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

      this.flowCanvasNodes = workflow.data.nodes.map((x) => ({
        type: x.data.type,
        ...x,
      }));
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
