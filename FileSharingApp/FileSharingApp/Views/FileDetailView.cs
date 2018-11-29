using Android.App;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Cross;
using FileSharingApp.Helpers;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FileSharingApp.Views
{
	[Activity(Label = "FileDetailView", Theme = "@style/AppTheme.Fullscreen")]
	public class FileDetailView : AppCompatActivity
	{
		private const int BufferSize = 1024;
		private string _ip;
		private int _portNumber;

		public static string FileName = "File Name";
		private MvxCachedImageView _image;
		private ImageView _closeBtn;
		private ImageView _downloadBtn;
		private string _fileName;
		byte[] _imageData;
		private MemoryStream _stream;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.file_detail);

			_ip = Intent.GetStringExtra(LoginView.HostIp);
			_portNumber = Intent.GetIntExtra(LoginView.PortNumber, 8080);
			_fileName = Intent.GetStringExtra(FileName);
			_image = FindViewById<MvxCachedImageView>(Resource.Id.imgMain);

			_image.SetImageDrawable(_fileName.HasImageExtension()
				? ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_image)
				: ContextCompat.GetDrawable(Application.Context, Resource.Drawable.default_file));

			_closeBtn = FindViewById<ImageView>(Resource.Id.closeBtn);
			_closeBtn.Click += CloseView;

			_downloadBtn = FindViewById<ImageView>(Resource.Id.downloadBtn);
			_downloadBtn.Click += Download;

			GetImage(_ip, _portNumber, "GET_IMAGE");
		}

		private void GetImage(string ip, int portNumber, string method)
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
				reader = new StreamReader(networkStream);

				writer.WriteLine(method);
				writer.WriteLine(_fileName);

				var fileSize = Convert.ToInt64(reader.ReadLine());

				Console.WriteLine(fileSize);

				_imageData = new byte[fileSize];

				var recData = new byte[BufferSize];
				_stream = new MemoryStream(_imageData);

				int recBytes = 1;

				while (_stream.Position < fileSize && recBytes > 0)
				{
					recBytes = reader.BaseStream.Read(recData, 0, recData.Length);
					_stream.Write(recData, 0, recBytes);
				}
				_stream.Close();

				ImageService.Instance.LoadStream(GetStream).DownSample(500).Into(_image);
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

		private Task<Stream> GetStream(CancellationToken arg)
		{
			TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();

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