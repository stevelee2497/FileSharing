using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileSharingServer
{
	public class Server
	{
		private const string SaveFileName = "e:\\server";
		private const int PortNumber = 8080;
		private const int BufferSize = 1024;
		public string Status = string.Empty;

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
			try
			{
				Console.WriteLine($"{socket.RemoteEndPoint} has connected");
				using (var stream = new NetworkStream(socket))
				{
					try
					{
						var reader = new StreamReader(stream);
						var fileName = reader.ReadLine();
						Console.WriteLine(fileName);
						byte[] recData = new byte[BufferSize];
						int recBytes;
						var fileStream = new FileStream(Path.Combine(SaveFileName, fileName), FileMode.OpenOrCreate, FileAccess.Write);
						while ((recBytes = stream.Read(recData, 0, recData.Length)) > 0)
						{
							fileStream.Write(recData, 0, recBytes);
						}
						fileStream.Close();
						Console.WriteLine("done");

						var writer = new StreamWriter(stream);
						writer.WriteLine("done");
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				}

				
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				socket.Close();
				Console.WriteLine("=============================================");
			}
		}
	}
}
