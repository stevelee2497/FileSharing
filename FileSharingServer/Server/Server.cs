using Server.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
namespace Server
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

	public enum HeaderParam
	{
		Method,
		UserName,
		Password,
		FileName,
		FileSize
	}

	public class ClientHandler
	{
		private const string BaseUrl = "e:\\server";
		private const int BufferSize = 1024;

		private readonly Socket _socket;
		private NetworkStream _stream;
		private StreamReader _reader;
		private StreamWriter _writer;

		public ClientHandler(Socket socket)
		{
			_socket = socket;
			_stream = new NetworkStream(_socket);
			_reader = new StreamReader(_stream);
			_writer = new StreamWriter(_stream) { AutoFlush = true };
		}

		public void Listen()
		{
			Console.WriteLine("=============================================");
			Console.WriteLine($"{_socket.RemoteEndPoint} has connected");

			try
			{
				_stream = new NetworkStream(_socket);
				_reader = new StreamReader(_stream);
				_writer = new StreamWriter(_stream) { AutoFlush = true };

				while (!_socket.IsSocketDisconnected())
				{
					var header = _reader.ReadLine();
					if (header == null) continue;
					var headers = header.Split(',');
					switch (headers[0])
					{
						case "LOGIN":
							Login(headers[1], headers[2]);
							break;
						case "POST_FILE":
							SaveFile(headers[1], headers[2], Convert.ToInt64(headers[3]));
							break;
						case "GET_FILES":
							SendFileNames(headers[1]);
							break;
						case "GET_FILE":
							SendFile(headers[1], headers[2]);
							break;
						case "DELETE_FILE":
							DeleteFile(headers[1], headers[2]);
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
				_writer?.Close();
				_reader?.Close();
				_stream?.Close();
				_socket.Close();
				Console.WriteLine("=============================================");
			}
		}

		private void Login(string userName, string password)
		{
			_writer.WriteLine(Helper.Login(userName, password) ? "success" : "error");
		}
		
		private void DeleteFile(string userName, string fileName)
		{
			File.Delete(Path.Combine(BaseUrl, userName, fileName));
			_writer.WriteLine("done");
		}

		private void SendFile(string userName, string fileName)
		{
			var data = File.ReadAllBytes(Path.Combine(BaseUrl,userName, fileName));
			_writer.WriteLine(data.Length);
			Thread.Sleep(50);
			_writer.BaseStream.Write(data, 0, data.Length);
			_writer.BaseStream.Flush(); 
		}

		private void SendFileNames(string userName)
		{
			var fileNames = Directory.GetFileSystemEntries(Path.Combine(BaseUrl, userName));
			var result = string.Join(",", fileNames.Select(Path.GetFileName));
			_writer.WriteLine(result);
		}

		private void SaveFile(string userName, string fileName, long totalLength)
		{
			Console.Write(totalLength + " ");
			var filePath = Path.Combine(BaseUrl, userName, fileName);
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
			int recBytes = 1;
			var recData = new byte[BufferSize];
			while (fileStream.Length < totalLength && recBytes > 0)
			{
				recBytes = _reader.BaseStream.Read(recData, 0, recData.Length);
				fileStream.Write(recData, 0, recBytes);
			}
			Console.WriteLine(fileStream.Length);
			fileStream.Close();

			_writer.WriteLine("done");
		}
	}
}
