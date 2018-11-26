using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileSharingServer
{
	public class Server
	{
		private const string SaveFileName = "e:\\server\\avatar.jpg";
		private const int PortNumber = 9999;
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
				using (var reader = new StreamReader(stream))
				using (var writer = new StreamWriter(stream) {AutoFlush = true})
				{
					try
					{
						byte[] recData = new byte[BufferSize];
						int recBytes;
						var fileStream = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);
						while ((recBytes = reader.BaseStream.Read(recData, 0, recData.Length)) > 0)
						{
							Console.WriteLine(recBytes);
							if (recBytes < BufferSize)
							{
								
							}
							fileStream.Write(recData, 0, recBytes);
						}
						fileStream.Close();
						Console.WriteLine("saved");

						writer.WriteLine("done");
						Console.WriteLine("done");
					}
					catch (Exception e)
					{
						writer.WriteLine("Error" + e.StackTrace);
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
