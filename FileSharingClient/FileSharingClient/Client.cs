using System;
using System.IO;
using System.Net.Sockets;

namespace FileSharingClient
{
	public class Client
	{
		public static string SendingFilePath = "e:\\ava.jpg";
		public static int PortNumber = 8080;

		private const int BufferSize = 1024;

		public static void Main(string[] args)
		{
			TcpClient client = null;
			NetworkStream networkStream = null;
			try
			{
				client = new TcpClient("127.0.0.1", PortNumber);
				Console.WriteLine("Connected to the Server...\n");
				networkStream = client.GetStream();

				var writer = new StreamWriter(networkStream) { AutoFlush = true };
				writer.WriteLine("avatar.jpg");

				var fs = new FileStream(SendingFilePath, FileMode.Open, FileAccess.Read);
				var noOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fs.Length) / Convert.ToDouble(BufferSize)));
				var totalLength = (int)fs.Length;
				for (var i = 0; i < noOfPackets; i++)
				{
					int currentPacketLength;
					if (totalLength > BufferSize)
					{
						currentPacketLength = BufferSize;
						totalLength = totalLength - currentPacketLength;
					}
					else
						currentPacketLength = totalLength;
					var sendingBuffer = new byte[currentPacketLength];
					fs.Read(sendingBuffer, 0, currentPacketLength);
					writer.BaseStream.Write(sendingBuffer, 0, sendingBuffer.Length);
				}

				Console.WriteLine("Sent " + fs.Length + " bytes to the server");
				fs.Close();

				var reader = new StreamReader(networkStream);
				while (true)
				{
					Console.WriteLine(reader.ReadLine());
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				networkStream?.Close();
				client?.Close();

			}
		}
	}
}
