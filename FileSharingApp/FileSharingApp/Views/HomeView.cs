using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using FileSharingApp.Models;
using Plugin.CurrentActivity;
using Plugin.FilePicker;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileSharingApp.Views
{
	[Activity(Theme = "@style/AppTheme")]
	public class HomeView : AppCompatActivity
	{
		private string _userName;
		private string _ip;
		private int _portNumber;
		private List<string> _files;
		private ImageView _uploadFileBtn;
		private ImageView _takePhotoBtn;
		private RecyclerView _rvFiles;
		private FileAdapter _fileAdapter;
		private FileSharingClient _client;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			_userName = Intent.GetStringExtra(AppConstants.UserName);
			_ip = Intent.GetStringExtra(AppConstants.HostIp);
			_portNumber = Intent.GetIntExtra(AppConstants.PortNumber, 8080);
			_client = new FileSharingClient(_ip, _portNumber);
			CrossCurrentActivity.Current.Init(this, savedInstanceState);
			CrossMedia.Current.Initialize();

			_rvFiles = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			_uploadFileBtn = FindViewById<ImageView>(Resource.Id.btnUpload);
			_takePhotoBtn = FindViewById<ImageView>(Resource.Id.btnTakePhoto);

			_files = _client.GetFileNames(_userName);
			if (_files != null)
			{
				_fileAdapter = new FileAdapter(_files, _ip, _portNumber, _userName);
				_rvFiles.SetLayoutManager(new GridLayoutManager(this, 4));
				_rvFiles.SetAdapter(_fileAdapter);
			}

			_uploadFileBtn.Click += UploadFileBtn;
			_takePhotoBtn.Click += TakePhoto;
		}

		protected override void OnDestroy()
		{
			_uploadFileBtn.Click -= UploadFileBtn;
			_takePhotoBtn.Click -= TakePhoto;
			base.OnDestroy();
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		private async void TakePhoto(object sender, EventArgs e)
		{
			var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
			{
				SaveToAlbum = true,
				Name = DateTime.Now.ToShortTimeString(),
				Directory = "FileSharing"
			});

			if (file == null)
			{
				return;
			}

			_client.PostFile(_userName, new FileSharingData
			{
				FileName = string.Join(".", DateTimeOffset.Now.ToString("dd-MM-yyyy-HH-mm-tt"), "jpg"),
				FilePath = file.Path,
				FileData = File.ReadAllBytes(file.Path)
			});

			UpdateListView();
		}

		private async void UploadFileBtn(object sender, EventArgs e)
		{
			var file = await CrossFilePicker.Current.PickFile();

			if (file == null)
			{
				return;
			}

			_client.PostFile(_userName, new FileSharingData
			{
				FileName = file.FileName,
				FileData = file.DataArray
			});

			UpdateListView();
		}

		private void UpdateListView()
		{
			_files = _client.GetFileNames(_userName);
			if (_files != null)
			{
				_fileAdapter = new FileAdapter(_files, _ip, _portNumber, _userName);
				_rvFiles.SetAdapter(_fileAdapter);
			}
		}
	}
}