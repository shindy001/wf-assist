namespace WfAssist.AspNetCore.Features.Workflows.GetIdentities;

internal sealed record WorkFlowIdentityDto(string Id, string Name);

internal sealed record GetIdentitiesResponse
{
    public IEnumerable<WorkFlowIdentityDto> Identities { get; init; } = [];
}