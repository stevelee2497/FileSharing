using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileSharingServer
{
	public class Server
	{
		private const string SaveFileName = "e:\\server\\avatar.jpg";
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
						byte[] recData = new byte[BufferSize];
						int recBytes;
						var fileStream = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);
						while ((recBytes = stream.Read(recData, 0, recData.Length)) > 0)
						{
							fileStream.Write(recData, 0, recBytes);
						}
						fileStream.Close();
						Console.WriteLine("done");
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

		static string DecodeFileName(Stream stream)
		{
			byte[] fileNameLengthBuffer = new byte[4];

			FillBufferFromStream(stream, fileNameLengthBuffer);
			int fileNameLength = BitConverter.ToInt32(fileNameLengthBuffer, 0);

			byte[] fileNameBuffer = new byte[fileNameLength];

			FillBufferFromStream(stream, fileNameBuffer);
			return Encoding.UTF8.GetString(fileNameBuffer);
		}

		static void FillBufferFromStream(Stream stream, byte[] buffer)
		{
			int cbTotal = 0;
			while (cbTotal < buffer.Length)
			{
				int cbRead = stream.Read(buffer, cbTotal, buffer.Length - cbTotal);

				if (cbRead == 0)
				{
					throw new InvalidDataException("premature end-of-stream");
				}

				cbTotal += cbRead;
			}
		}
	}
}
