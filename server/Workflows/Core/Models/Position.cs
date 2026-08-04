namespace WfAssist.Workflows.Core.Models;

internal sealed record Position(float X, float Y);

internal static class PositionExtensions
{
	extension (Position)
	{
		public static Position Default => new(0, 0);
	}
}