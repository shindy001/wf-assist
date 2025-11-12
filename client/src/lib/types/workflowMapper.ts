import type {
  WorkflowDataDto,
  WorkflowDto,
  WorkflowEdgeDto,
  WorkflowNodeDataDto,
  WorkflowNodeDataDtoExtractPropertyNodeDataDto,
  WorkflowNodeDataDtoRequestNodeDataDto,
  WorkflowNodeDto,
} from "$api";
import {
  type ExtractPropertyNodeData,
  type RequestNodeData,
  type Workflow,
  type WorkflowData,
  type WorkflowEdge,
  type WorkflowNode,
  type WorkflowNodeData,
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
    nodes: data.nodes?.map((x) => toWorkflowNodeDto(x)) ?? [],
    edges: data.edges?.map((x) => toWorkflowEdgeDto(x)) ?? [],
  };
}

function toWorkflowEdge(dto: WorkflowEdgeDto): WorkflowEdge {
  return {
    id: dto.id,
    source: dto.source,
    target: dto.target,
  };
}

function toWorkflowEdgeDto(data: WorkflowEdge): WorkflowEdgeDto {
  return {
    id: data.id,
    source: data.source,
    target: data.target,
  };
}

function toWorkflowNode(dto: WorkflowNodeDto): WorkflowNode {
  return {
    id: dto.id,
    position: { x: dto.position.x, y: dto.position.y },
    data: toWorkflowNodeData(dto),
  };
}

function toWorkflowNodeDto(data: WorkflowNode): WorkflowNodeDto {
  return {
    id: data.id,
    position: { x: data.position.x, y: data.position.y },
    data: toWorkflowNodeDataDto(data),
  };
}

function toWorkflowNodeData(dto: WorkflowNodeDto): WorkflowNodeData {
  switch (dto.data.type) {
    case "ExtractProperty":
      return dto.data as ExtractPropertyNodeData;
    case "Request":
      return dto.data as RequestNodeData;
    default:
      throw new Error(`Unknown WorkflowNodeDataDto type '${dto.data.type}'`);
  }
}

function toWorkflowNodeDataDto(node: WorkflowNode): WorkflowNodeDataDto {
  switch (node.data.type) {
    case "ExtractProperty":
      return node.data as WorkflowNodeDataDtoExtractPropertyNodeDataDto;
    case "Request":
      return node.data as WorkflowNodeDataDtoRequestNodeDataDto;
    default:
      throw new Error(`Unknown WorkflowNodeData type '${node.data.type}'`);
  }
}
