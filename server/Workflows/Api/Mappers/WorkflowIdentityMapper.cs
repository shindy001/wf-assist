using WfAssist.Workflows.Api.Dtos;
using WfAssist.Workflows.Core.Models;

namespace WfAssist.Workflows.Api.Mappers;

internal static class WorkflowIdentityMapper
{
    internal static WorkFlowIdentityDto ToDto(this WorkflowIdentity entity)
        => new(entity.Id, entity.Name);
}