import type {
  WorkflowDataDto,
  WorkflowDto,
  EdgeDto,
  NodeDtoHeadersNodeDto,
  NodeDtoRequestNodeDto,
  NodeDto,
} from "$api";
import {
  type RequestNode,
  type HeadersNode,
  type Workflow,
  type WorkflowData,
  type WorkflowEdge,
  type WorkflowNode,
} from "./workflow";

export function toWorkflow(dto: WorkflowDto): Workflow {
  return {
    id: dto.id,
    name: dto.name,
    data: toWorkflowData(dto.data),
  };
}

export function toWorkflowData(dto: WorkflowDataDto): WorkflowData {
  return {
    nodes: dto.nodes?.map((x) => toWorkflowNode(x)) ?? [],
    edges: dto.edges?.map((x) => toWorkflowEdge(x)) ?? [],
  };
}

export function toWorkflowDataDto(data: WorkflowData): WorkflowDataDto {
  return {
    nodes: data.nodes?.map((x) => ({ ...x.data, ...x } as NodeDto)) ?? [],
    edges: data.edges?.map((x) => toWorkflowEdgeDto(x)) ?? [],
  };
}

function toWorkflowEdge(dto: EdgeDto): WorkflowEdge {
  return {
    id: dto.id,
    source: dto.source,
    target: dto.target,
  };
}

function toWorkflowEdgeDto(data: WorkflowEdge): EdgeDto {
  return {
    id: data.id,
    source: data.source,
    target: data.target,
  };
}

function toWorkflowNode(dto: NodeDto): WorkflowNode {
  switch (dto.type) {
    case "RequestNode":
      return { ...dto, data: { ...dto } } as RequestNode;
    case "HeadersNode":
      return { ...dto, data: { ...dto } } as HeadersNode;
    default:
      throw new Error(`Unknown NodeDto type '${dto.type}'`);
  };
}
