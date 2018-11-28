using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FileSharingServer
{
	public class Server
	{
		private const string BaseUrl = "d:\\server";
		private const int PortNumber = 8080;
		private const int BufferSize = 1024;

		public static void Main(string[] args)
		{
			var listener = new TcpListener(IPAddress.Parse("192.168.51.177"), PortNumber);
			listener.Start();

			Console.WriteLine("Server started on " + listener.LocalEndpoint);
			Console.WriteLine("Waiting for a connection...");

			while (true)
			{
				HandleClient(listener.AcceptSocket());
			}
		}

		private static void HandleClient(Socket socket)
		{
			Console.WriteLine("=============================================");
			Console.WriteLine($"{socket.RemoteEndPoint} has connected");

			NetworkStream stream = null;
			StreamReader reader = null;
			StreamWriter writer = null;
			try
			{
				stream = new NetworkStream(socket);
				reader = new StreamReader(stream);
				writer = new StreamWriter(stream) { AutoFlush = true };

				var method = reader.ReadLine();

				switch (method)
				{
					case "POST_FILE":
						SaveFile(reader);
						break;
					case "GET_FILES":
						SendFileNames(writer);
						break;
					case "GET_IMAGE":
						SendFile(writer, reader);
						break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
			finally
			{
				stream?.Close();
				socket.Close();
				Console.WriteLine("=============================================");
			}
		}

		private static void SendFile(StreamWriter writer, StreamReader reader)
		{
			var fileName = reader.ReadLine();

			var data = File.ReadAllBytes(Path.Combine(BaseUrl, fileName));
			writer.WriteLine(data.Length);
			writer.BaseStream.Write(data, 0, data.Length);
			writer.Close();
		}

		private static void SendFileNames(StreamWriter writer)
		{
			var fileNames = new[]
			{
				"avatar.jpg", "avatar.jpg", "avatar.jpg", "avatar.jpg", "avatar.jpg", "avatar.jpg", "avatar.jpg",
			};
			writer.WriteLine(string.Join(',', fileNames));
		}

		private static void SaveFile(StreamReader reader)
		{
			var fileName = reader.ReadLine();
			var totalLength = Convert.ToInt64(reader.ReadLine());
			Console.Write(totalLength + " ");
			var recData = new byte[BufferSize];
			var fileStream = new FileStream(Path.Combine(BaseUrl, fileName), FileMode.OpenOrCreate, FileAccess.Write);
			int recBytes;
			while ((recBytes = reader.BaseStream.Read(recData, 0, recData.Length)) > 0)
			{
				recBytes = totalLength > recBytes ? recBytes : (int)totalLength;
				fileStream.Write(recData, 0, recBytes);
			}

			Console.WriteLine(fileStream.Length);
			fileStream.Close();
		}
	}
}
