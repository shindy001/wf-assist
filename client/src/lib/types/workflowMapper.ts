import type {
  WorkflowDataDto,
  WorkflowDto,
  EdgeDto,
  NodeDtoHeadersNodeDto,
  NodeDtoRequestNodeDto,
  NodeDto,
  SizeDto,
  PositionDto,
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
  console.log(data);
  return {
    nodes: data.nodes?.map((x) => toWorkflowNodeDto(x)) ?? [],
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
  const node = {
    ...dto,
    width: dto.size.width,
    height: dto.size.height,
    data: { ...dto }
  };

  switch (dto.type) {
    case "RequestNode":
      return node as RequestNode;
    case "HeadersNode":
    return node as HeadersNode;
    default:
      throw new Error(`Unknown NodeDto type '${dto.type}'`);
  };
}

function toWorkflowNodeDto(node: WorkflowNode): NodeDto {
  const size: SizeDto = { width: node.width, height: node.height };
  const position: PositionDto = node.position;
  const nodeData = node.data;
  const dto = { ...nodeData, size, position };

  switch (node.type) {
    case "RequestNode":
      return dto as NodeDtoRequestNodeDto;
    case "HeadersNode":
      return dto as NodeDtoHeadersNodeDto;
    default:
      throw new Error(`Unknown node type '${node.type}'`);
  };
}
