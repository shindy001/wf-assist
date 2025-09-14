namespace WfAssist.AspNetCore.Features.Workflows.GetIdentities;

public record WorkFlowIdentityDto(string Id, string Name);

public record GetIdentitiesResponse
{
    public IEnumerable<WorkFlowIdentityDto> Identities { get; init; } = [];
}