using System;
using Android.App;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Widget;
using FileSharingApp.Helpers;

namespace FileSharingApp.View
{
	[Activity(Label = "FileDetailView", Theme = "@style/AppTheme.Fullscreen")]
	public class FileDetailView : AppCompatActivity
	{
		public static string FileName = "File Name";
		private ImageView _image;
		private ImageView _closeBtn;
		private ImageView _downloadBtn;
		private string _fileName;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.file_detail);

			_fileName = Intent.GetStringExtra(FileName);
			_image = FindViewById<ImageView>(Resource.Id.imgMain);

			_image.SetImageDrawable(_fileName.HasImageExtension()
				? ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_image)
				: ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_file));

			_closeBtn = FindViewById<ImageView>(Resource.Id.closeBtn);
			_closeBtn.Click += CloseView;

			_downloadBtn = FindViewById<ImageView>(Resource.Id.downloadBtn);
			_downloadBtn.Click += Download;
		}

		private void CloseView(object sender, EventArgs e)
		{
			Finish();
		}

		private void Download(object sender, EventArgs e)
		{
		}
	}
}