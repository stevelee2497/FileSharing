using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileSharingServer
{
	public class Server
	{
		private const string BaseUrl = "d:\\server";
		private const int PortNumber = 8080;
		private const int BufferSize = 1024;

		public static void Main(string[] args)
		{
			var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), PortNumber);
			listener.Start();

			Console.WriteLine("Server started on " + listener.LocalEndpoint);
			Console.WriteLine("Waiting for a connection...");

			while (true)
			{
				SaveFile(listener.AcceptSocket());
			}
		}

		private static void SaveFile(Socket socket)
		{
			Console.WriteLine("=============================================");
			Console.WriteLine($"{socket.RemoteEndPoint} has connected");

			NetworkStream stream = null;
			StreamReader reader = null;
			StreamWriter writer = null;
			FileStream fileStream = null;
			try
			{
				stream = new NetworkStream(socket);
				reader = new StreamReader(stream);
				writer = new StreamWriter(stream);

				var fileName = reader.ReadLine();
				var totalLength = Convert.ToInt64(reader.ReadLine());
				var recData = new byte[BufferSize];
				fileStream = new FileStream(Path.Combine(BaseUrl, fileName), FileMode.OpenOrCreate, FileAccess.Write);
				int recBytes;
				while ((recBytes = stream.Read(recData, 0, recData.Length)) > 0)
				{
					recBytes = totalLength > recBytes ? recBytes : (int)totalLength;
					fileStream.Write(recData, 0, recBytes);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
			finally
			{
				reader?.Close();
				writer?.Close();
				stream?.Close();
				socket.Close();
				fileStream?.Close();

			}
		}
	}
}
