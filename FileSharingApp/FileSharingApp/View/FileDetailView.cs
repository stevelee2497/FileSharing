using System;
using System.IO;
using System.Net.Mime;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Widget;
using FFImageLoading;
using FileSharingApp.Helpers;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Stream = System.IO.Stream;

namespace FileSharingApp.View
{
	[Activity(Label = "FileDetailView", Theme = "@style/AppTheme.Fullscreen")]
	public class FileDetailView : AppCompatActivity
	{
		private const int BufferSize = 1024;
		private string _ip = "192.168.51.177";
		private int _portNumber = 8080;

		public static string FileName = "File Name";
		private ImageView _image;
		private ImageView _closeBtn;
		private ImageView _downloadBtn;
		private string _fileName;
		byte[] image;
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

				var fileName = "avatar.jpg";

				writer.WriteLine(method);
				writer.WriteLine(fileName);

				var fileSize = Convert.ToInt64(reader.ReadLine());

				Console.WriteLine(fileSize);

				
				image = new byte[fileSize];

				var recData = new byte[BufferSize];
				var stream = new MemoryStream(image);
				int recBytes;
				while ((recBytes = reader.BaseStream.Read(recData, 0, recData.Length)) > 0)
				{
					recBytes = fileSize > recBytes ? recBytes : (int)fileSize;
					stream.Write(recData, 0, recBytes);
				}

				var bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length);
				_image.SetImageBitmap(bitmap);
				stream.Close();
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