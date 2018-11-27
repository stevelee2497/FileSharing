using System;
using System.IO;
using System.Net.Sockets;

namespace FileSharingClient
{
	public class Client
	{
		public static string SendingFilePath = "C:\\Users\\Tranq\\Desktop\\IMG_3863.JPG";
		public static int PortNumber = 8080;

		private const int BufferSize = 1024;

		public static void Main(string[] args)
		{
			TcpClient client = null;
			NetworkStream networkStream = null;
			StreamReader reader = null;
			StreamWriter writer = null;
			try
			{
				client = new TcpClient("127.0.0.1", PortNumber);
				Console.WriteLine("Connected to the Server...\n");
				networkStream = client.GetStream();

				writer = new StreamWriter(networkStream) { AutoFlush = true };
				reader = new StreamReader(networkStream);

				writer.WriteLine("avatar.jpg");
				var fs = new FileStream(SendingFilePath, FileMode.Open, FileAccess.Read);
				var noOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fs.Length) / Convert.ToDouble(BufferSize)));
				var totalLength = (int)fs.Length;
				writer.WriteLine(fs.Length);
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
				writer.WriteLine("client done");

				Console.WriteLine("Sent " + fs.Length + " bytes to the server");

				while (true)
				{
					var result = reader.ReadLine();
					if ("done".Equals(result))
					{
						Console.WriteLine(result);
						break;
					}
				}
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

			Console.WriteLine("press any key to exit ...");
			Console.ReadKey();
		}
	}
}
