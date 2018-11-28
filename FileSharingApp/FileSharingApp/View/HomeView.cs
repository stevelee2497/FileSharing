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
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Plugin.FilePicker.Abstractions;

namespace FileSharingApp.View
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class HomeView : AppCompatActivity
	{
		private string _ip = "192.168.51.177";
		private int _portNumber = 8080;
		private List<string> _files;

		private ImageView _uploadFileBtn;
		private ImageView _takePhotoBtn;
		private RecyclerView _rvFiles;
		private FileAdapter _fileAdapter;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			CrossCurrentActivity.Current.Init(this, savedInstanceState);
			CrossMedia.Current.Initialize();

			_uploadFileBtn = FindViewById<ImageView>(Resource.Id.btnUpload);
			_uploadFileBtn.Click += UploadFileBtn;

			_takePhotoBtn = FindViewById<ImageView>(Resource.Id.btnTakePhoto);
			_takePhotoBtn.Click += TakePhoto;

			GetFileNames(_ip, _portNumber, "GET_FILES");

			_rvFiles = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			_fileAdapter = new FileAdapter(_files);
			_rvFiles.SetLayoutManager(new GridLayoutManager(this, 4));
			_rvFiles.SetAdapter(_fileAdapter);
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

		private void GetFileNames(string ip, int portNumber, string method)
		{
			TcpClient client = null;
			NetworkStream networkStream = null;
			StreamReader reader = null;
			StreamWriter writer = null;
			try
			{
				client = new TcpClient(ip, portNumber);
				Console.WriteLine("Connected to the Server...\n");
				networkStream = client.GetStream();

				writer = new StreamWriter(networkStream) { AutoFlush = true };

				writer.WriteLine(method);

				reader = new StreamReader(networkStream);
				var result = reader.ReadLine();
				_files = result?.Split(",").ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				reader?.Close();
				writer?.Close();
				networkStream?.Close();
				client?.Close();
			}
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

		private async void UploadFileBtn(object sender, EventArgs e)
		{
			var file = await CrossFilePicker.Current.PickFile();
			PostFile(_ip, _portNumber, "POST_FILE", file);
		}

		private void PostFile(string ip, int portNumber, string method, FileData fileData)
		{
			TcpClient client = null;
			NetworkStream networkStream = null;
			StreamReader reader = null;
			StreamWriter writer = null;
			try
			{
				client = new TcpClient(ip, portNumber);
				Console.WriteLine("Connected to the Server...\n");
				networkStream = client.GetStream();

				writer = new StreamWriter(networkStream) { AutoFlush = true };
				reader = new StreamReader(client.GetStream());

				writer.WriteLine(method);
				writer.WriteLine(fileData.FileName);
				writer.WriteLine(fileData.DataArray.Length);

				writer.BaseStream.Write(fileData.DataArray, 0, fileData.DataArray.Length);
				while (true)
				{
					var result = reader.ReadLine();

					Console.WriteLine(result);

					if ("done".Equals(result))
					{
						writer.Close();
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				reader?.Close();
				writer?.Close();
				networkStream?.Close();
				client?.Close();
			}
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