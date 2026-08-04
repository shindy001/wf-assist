namespace WfAssist.Workflows.Core.Models;

internal sealed record Size(int Width, int Height);

internal static class SizeExtensions
{
	extension (Size)
	{
		public static Size Default => new(0, 0);
	}
}