using Android.App;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Cross;
using FileSharingApp.Helpers;
using FileSharingApp.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSharingApp.Views
{
	[Activity(Label = "FileDetailView", Theme = "@style/AppTheme.Fullscreen")]
	public class FileDetailView : AppCompatActivity
	{
		private byte[] _imageData;
		private string _userName;
		private string _ip;
		private int _portNumber;
		private string _fileName;
		private ImageView _closeBtn;
		private ImageView _downloadBtn;
		private FileSharingClient _client;
		private MvxCachedImageView _image;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.file_detail);

			_userName = Intent.GetStringExtra(AppConstants.UserName);
			_ip = Intent.GetStringExtra(AppConstants.HostIp);
			_portNumber = Intent.GetIntExtra(AppConstants.PortNumber, 8080);
			_client = new FileSharingClient(_ip, _portNumber);

			_fileName = Intent.GetStringExtra(AppConstants.FileName);
			_image = FindViewById<MvxCachedImageView>(Resource.Id.imgMain);

			_image.SetImageDrawable(_fileName.HasImageExtension()
				? ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_image)
				: ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_file));

			_closeBtn = FindViewById<ImageView>(Resource.Id.closeBtn);
			_closeBtn.Click += CloseView;

			_downloadBtn = FindViewById<ImageView>(Resource.Id.downloadBtn);
			_downloadBtn.Click += Download;

			_imageData = _client.GetImage(_userName, _fileName).FileData;
			ImageService.Instance.LoadStream(GetStream).DownSample(500).Into(_image);
		}

		

		private Task<Stream> GetStream(CancellationToken arg)
		{
			var tcs = new TaskCompletionSource<Stream>();

			tcs.TrySetResult(new MemoryStream(_imageData));

			return tcs.Task;
		}

		private void CloseView(object sender, EventArgs e)
		{
			Finish();
		}

		private void Download(object sender, EventArgs e)
		{
			//CrossFilePicker.Current.SaveFile(new FileData());
		}
	}
}