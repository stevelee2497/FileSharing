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

			try
			{
				stream = new NetworkStream(socket);
				reader = new StreamReader(stream);
				writer = new StreamWriter(stream);

				var fileName = reader.ReadLine();
				var totalLength = Convert.ToInt64(reader.ReadLine());
				Console.WriteLine(fileName + " " + totalLength);
				byte[] recData = new byte[BufferSize];
				var fileStream = new FileStream(Path.Combine(BaseUrl, fileName), FileMode.OpenOrCreate, FileAccess.Write);
				try
				{
					var recBytes = BufferSize;
					while (totalLength > 0)
					{
						recBytes = BufferSize < totalLength ? BufferSize : (int)totalLength;
						Console.Write(recBytes + " ");
						recBytes = stream.Read(recData, 0, recBytes);
						Console.WriteLine(recBytes);
						fileStream.Write(recData, 0, recBytes);
						totalLength -= recBytes;
					}
					Console.WriteLine(totalLength + " " + recBytes);

					Console.WriteLine(reader.ReadLine());
					Console.WriteLine("server done");

					while (true)
					{
						writer.WriteLine("done");
					}

				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
				finally
				{
					fileStream.Close();
					
					Console.WriteLine("=============================================");
				}
			}
			finally
			{
				reader?.Close();
				writer?.Close();
				stream?.Close();
				socket.Close();
			}
		}
	}
}
