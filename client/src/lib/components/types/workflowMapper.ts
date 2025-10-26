import type { WorkflowDto, WorkflowEdgeDto, WorkflowNodeDto } from "$api";
import {
  WorkflowNodeType,
  type ExtractPropertyNodeData,
  type PrintTextNodeData,
  type RequestNodeData,
  type Workflow,
  type WorkflowEdge,
  type WorkflowNode,
  type WorkflowNodeData,
} from "./workflow";

export function toWorkflow(dto: WorkflowDto): Workflow {
  return {
    id: dto.id,
    name: dto.name,
    data: {
      edges: dto.data.edges?.map((x) => toWorkflowEdge(x)) ?? [],
      nodes: dto.data.nodes?.map((x) => toWorkflowNode(x)) ?? [],
    },
  };
}

function toWorkflowEdge(dto: WorkflowEdgeDto): WorkflowEdge {
  return {
    id: dto.id,
    position: { x: dto.position.x, y: dto.position.y },
    source: dto.source,
    target: dto.target,
  };
}

function toWorkflowNode(dto: WorkflowNodeDto): WorkflowNode {
  return {
    id: dto.id,
    type: dto.type as WorkflowNodeType,
    position: { x: dto.position.x, y: dto.position.y },
    data: toWorkflowData(dto),
  };
}

function toWorkflowData(dto: WorkflowNodeDto): WorkflowNodeData {
  switch (dto.type) {
    case "PrintText":
      return dto as unknown as PrintTextNodeData & Record<string, unknown>;
    case "ExtractProperty":
      return dto as unknown as ExtractPropertyNodeData &
        Record<string, unknown>;
    case "Request":
      return dto as unknown as RequestNodeData & Record<string, unknown>;
    default:
      throw new Error(`Unknown WorkflowNodeDto type '${dto.type}'`);
  }
}
