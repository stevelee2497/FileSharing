using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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