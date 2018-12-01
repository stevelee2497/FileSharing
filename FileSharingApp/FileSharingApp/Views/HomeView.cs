using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FileSharingApp.Helpers;
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
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
	public class HomeView : AppCompatActivity
	{
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

			_ip = Intent.GetStringExtra(LoginView.HostIp);
			_portNumber = Intent.GetIntExtra(LoginView.PortNumber, 8080);
			_client = new FileSharingClient(_ip, _portNumber);

			CrossCurrentActivity.Current.Init(this, savedInstanceState);
			CrossMedia.Current.Initialize();

			_files = _client.GetFileNames("quoc");
			_rvFiles = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			_fileAdapter = new FileAdapter(_files, _ip, _portNumber);
			_rvFiles.SetLayoutManager(new GridLayoutManager(this, 4));
			_rvFiles.SetAdapter(_fileAdapter);

			_uploadFileBtn = FindViewById<ImageView>(Resource.Id.btnUpload);
			_uploadFileBtn.Click += UploadFileBtn;

			_takePhotoBtn = FindViewById<ImageView>(Resource.Id.btnTakePhoto);
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

			_client.PostFile("quoc", new FileSharingData
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

			_client.PostFile("quoc", new FileSharingData
			{
				FileName = file.FileName,
				FileData = file.DataArray
			});

			UpdateListView();
		}

		private void UpdateListView()
		{
			_files = _client.GetFileNames("quoc");
			_fileAdapter = new FileAdapter(_files, _ip, _portNumber);
			_rvFiles.SetAdapter(_fileAdapter);
		}
	}

	public class FileAdapter : RecyclerView.Adapter
	{
		private readonly List<string> _images;
		private readonly string _ip;
		private readonly int _portNumber;

		public FileAdapter(List<string> images, string _ip, int _portNumber)
		{
			_images = images;
			this._ip = _ip;
			this._portNumber = _portNumber;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.image_template, parent, false);
			return new FileViewHolder(itemView);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			var holder = ((FileViewHolder)viewHolder);
			holder.HostIp = _ip;
			holder.PortNumber = _portNumber;
			holder.ImageName = _images[position];
		}

		public override int ItemCount => _images.Count;
	}

	public class FileViewHolder : RecyclerView.ViewHolder
	{
		public string HostIp { get; set; }
		public int PortNumber { get; set; }
		public string ImageName
		{
			set
			{
				TvName.Text = value;
				Image.SetImageDrawable(value.HasImageExtension()
					? ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_image)
					: ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_file));
			}
		}

		public ImageView Image { get; private set; }
		public TextView TvName { get; private set; }

		public FileViewHolder(View itemView) : base(itemView)
		{
			Image = itemView.FindViewById<ImageView>(Resource.Id.image);
			TvName = itemView.FindViewById<TextView>(Resource.Id.tvName);

			Image.Click += ImageOnClicked;
		}

		private void ImageOnClicked(object sender, EventArgs e)
		{
			var intent = new Intent(ItemView.Context, typeof(FileDetailView));
			intent.PutExtra("File Name", TvName.Text);
			intent.PutExtra(LoginView.HostIp, HostIp);
			intent.PutExtra(LoginView.PortNumber, PortNumber);
			ItemView.Context.StartActivity(intent);
		}
	}
}