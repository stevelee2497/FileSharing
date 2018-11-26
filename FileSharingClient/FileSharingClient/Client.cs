using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

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
					networkStream.Write(sendingBuffer, 0, sendingBuffer.Length);
				}

				Console.WriteLine("Sent " + fs.Length + " bytes to the server");
				fs.Close();
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
		void TransmitFileName(Stream stream, string fileName)
		{
			byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName),
				fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);

			stream.Write(fileNameLengthBytes, 0, 4);
			stream.Write(fileNameBytes, 0, fileNameBytes.Length);
		}
	}
}
