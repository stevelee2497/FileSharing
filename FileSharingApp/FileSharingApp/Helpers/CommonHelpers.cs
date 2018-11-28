namespace FileSharingApp.Helpers
{
	public static class CommonHelpers
	{
		public static bool HasImageExtension(this string source)
		{
			return source.EndsWith(".png") || source.EndsWith(".jpg");
		}
	}
}