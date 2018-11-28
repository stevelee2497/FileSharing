using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FileSharingApp.Helpers;
using Plugin.CurrentActivity;
using Plugin.FilePicker;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;

namespace FileSharingApp.View
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class HomeView : AppCompatActivity
	{
		private readonly List<string> _files = new List<string>
		{
			"image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip",
			"image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip",
			"image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip",
			"image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip", "image.zip",
			"image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg",
			"image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg",
			"image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg",
			"image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg", "image.jpg",
		};

		private ImageView _uploadFileBtn;
		private ImageView _takePhotoBtn;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			CrossCurrentActivity.Current.Init(this, savedInstanceState);
			CrossMedia.Current.Initialize();

			var rvFiles = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			var fileAdapter = new FileAdapter(_files);
			rvFiles.SetLayoutManager(new GridLayoutManager(this, 4));
			rvFiles.SetAdapter(fileAdapter);

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
		}

		private void UploadFileBtn(object sender, EventArgs e)
		{
			CrossFilePicker.Current.PickFile();
		}
	}

	public class FileAdapter : RecyclerView.Adapter
	{
		private readonly List<string> _images;

		public FileAdapter(List<string> images)
		{
			_images = images;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.image_template, parent, false);
			return new FileViewHolder(itemView);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			((FileViewHolder)viewHolder).ImageName = _images[position];
		}

		public override int ItemCount => _images.Count;
	}

	public class FileViewHolder : RecyclerView.ViewHolder
	{
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

		public FileViewHolder(Android.Views.View itemView) : base(itemView)
		{
			Image = itemView.FindViewById<ImageView>(Resource.Id.image);
			TvName = itemView.FindViewById<TextView>(Resource.Id.tvName);

			Image.Click += ImageOnClicked;
		}

		private void ImageOnClicked(object sender, EventArgs e)
		{
			var intent = new Intent(ItemView.Context, typeof(FileDetailView));
			intent.PutExtra("File Name", TvName.Text);
			ItemView.Context.StartActivity(intent);
		}
	}
}