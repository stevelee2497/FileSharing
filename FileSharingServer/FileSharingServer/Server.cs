using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FileSharingServer
{
	public class Server
	{
		private const int PortNumber = 8080;

		public static void Main(string[] args)
		{
			var listener = new TcpListener(IPAddress.Parse("10.0.143.67"), PortNumber);
			listener.Start();

			Console.WriteLine("Server started on " + listener.LocalEndpoint);
			Console.WriteLine("Waiting for a connection...");

			while (true)
			{
				var client = new ClientHandler(listener.AcceptSocket());
				Task.Run(() => client.Listen());
			}
		}
	}

	public class ClientHandler
	{
		private const string BaseUrl = "e:\\server";
		private const int BufferSize = 1024;

		private readonly Socket _socket;

		public ClientHandler(Socket socket)
		{
			_socket = socket;
		}

		public void Listen()
		{
			Console.WriteLine("=============================================");
			Console.WriteLine($"{_socket.RemoteEndPoint} has connected");

			NetworkStream stream = null;
			StreamReader reader = null;
			StreamWriter writer = null;
			try
			{
				stream = new NetworkStream(_socket);
				reader = new StreamReader(stream);
				writer = new StreamWriter(stream) { AutoFlush = true };

				while (!_socket.IsSocketDisconnected())
				{
					var method = reader.ReadLine();

					switch (method)
					{
						case "POST_FILE":
							SaveFile(reader, writer);
							break;
						case "GET_FILES":
							SendFileNames(writer);
							break;
						case "GET_IMAGE":
							SendFile(reader, writer);
							break;
						case "DELETE_FILE":
							DeleteFile(reader, writer);
							break;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
			finally
			{
				stream?.Close();
				_socket.Close();
				Console.WriteLine("=============================================");
			}
		}

		private void DeleteFile(StreamReader reader, StreamWriter writer)
		{
			var fileName = reader.ReadLine();
			File.Delete(Path.Combine(BaseUrl, fileName));
			writer.WriteLine("done");
		}

		private void SendFile(StreamReader reader, StreamWriter writer)
		{
			var fileName = reader.ReadLine();
			var data = File.ReadAllBytes(Path.Combine(BaseUrl, fileName));
			writer.WriteLine(data.Length);
			Thread.Sleep(50);
			writer.BaseStream.Write(data, 0, data.Length);
			writer.BaseStream.Flush();
		}

		private void SendFileNames(StreamWriter writer)
		{
			var fileNames = Directory.GetFileSystemEntries(BaseUrl);
			var result = string.Join(',', fileNames.Select(Path.GetFileName));
			writer.WriteLine(result);
		}

		private void SaveFile(StreamReader reader, StreamWriter writer)
		{
			var fileName = reader.ReadLine();
			var totalLength = Convert.ToInt64(reader.ReadLine());
			Console.Write(totalLength + " ");
			var recData = new byte[BufferSize];
			var filePath = Path.Combine(BaseUrl, fileName);
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
			int recBytes = 1;
			while (fileStream.Length < totalLength && recBytes > 0)
			{
				recBytes = reader.BaseStream.Read(recData, 0, recData.Length);
				fileStream.Write(recData, 0, recBytes);
			}
			Console.WriteLine(fileStream.Length);
			fileStream.Close();

			writer.WriteLine("done");
		}
	}
}
