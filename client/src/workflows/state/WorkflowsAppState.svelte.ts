import {
  type WorkflowIdentity,
  WorkflowNodeType,
  type WorkflowData,
  failed,
  type WorkflowNode,
  type WorkflowEdge,
} from "$lib/components/types";
import type { Node, Edge } from "@xyflow/svelte";
import { createGetWorkflowIdentitiesQuery } from "../actions/getWorkflowIdentitiesQuery";
import { createGetWorkflowQuery } from "../actions/getWorkflowQuery";
import { createSaveWorkflowCommand } from "../actions/saveWorkflowCommand";
import { merge } from "lodash";

class WorkflowsAppState {
  #workflowIdentities = $state<WorkflowIdentity[] | undefined>();
  #selectedWorkflowIdentity = $state<WorkflowIdentity | undefined>();
  #selectedNodeType = $state<WorkflowNodeType>(WorkflowNodeType.Default);
  flowCanvasNodes = $state.raw<Node[]>([]);
  flowCanvasEdges = $state.raw<Edge[]>([]);

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

  addFlowCanvasNode(node: Node) {
    this.flowCanvasNodes = [...this.flowCanvasNodes, node];
  }

  addFlowCanvasEdge(edge: Edge) {
    this.flowCanvasEdges = [...this.flowCanvasEdges, edge];
  }

  async fetchWorkflowIdentities() {
    const query = createGetWorkflowIdentitiesQuery();
    const result = await query();
    this.#workflowIdentities = result.data ?? [];
  }

  async setSelectedWorkflow(id: string) {
    const query = createGetWorkflowQuery();
    const workflow = (await query(id)).data;

    if (workflow) {
      this.#selectedWorkflowIdentity = {
        id: workflow?.id,
        name: workflow?.name,
      };
      this.flowCanvasNodes = [...(workflow?.data.nodes ?? [])];
      this.flowCanvasEdges = [...(workflow?.data.edges ?? [])];
    }
  }

  async saveWorkflowData() {
    if (!this.selectedWorkflowIdentity) {
      return failed("Cannot save workflow, no workflow is selected.");
    }

    const newData = {
      nodes: this.flowCanvasNodes.map(
        (x) => merge(x, x.data) as unknown as WorkflowNode,
      ),
      edges: this.flowCanvasEdges.map(
        (x) => merge(x, x.data) as unknown as WorkflowEdge,
      ),
    };
    return await this.saveWorkflowCommand(
      this.selectedWorkflowIdentity.id,
      newData,
    );
  }
}

const instance = new WorkflowsAppState();

export const useWorkflowsAppState = () => instance;
