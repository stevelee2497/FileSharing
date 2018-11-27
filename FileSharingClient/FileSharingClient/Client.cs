using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FileSharingClient
{
	public class Client
	{
		public static string SendingFilePath = "C:\\Users\\Tranq\\Desktop\\IMG_3863.JPG";

		private const int BufferSize = 1024;

		public static void Main(string[] args)
		{
			var method = "POST_FILE";
			var ip = "127.0.0.1";
			var portNumber = 8080;

			//PostFile(ip, portNumber, method);

			GetFileNames(ip, portNumber, "GET_FILES");

			Console.WriteLine("press any key to exit ...");
			Console.ReadKey();
		}

		private static void GetFileNames(string ip, int portNumber, string method)
		{
			TcpClient client = null;
			NetworkStream networkStream = null;
			StreamReader reader = null;
			StreamWriter writer = null;
			try
			{
				client = new TcpClient(ip, portNumber);
				Console.WriteLine("Connected to the Server...\n");
				networkStream = client.GetStream();

				writer = new StreamWriter(networkStream) { AutoFlush = true };
				

				writer.WriteLine(method);

				while (true)
				{
					reader = new StreamReader(networkStream);
					var result = reader.ReadLine();
					Console.WriteLine(result);
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
		}

		private static void PostFile(string ip, int portNumber, string method)
		{
			TcpClient client = null;
			NetworkStream networkStream = null;
			StreamReader reader = null;
			StreamWriter writer = null;
			try
			{
				client = new TcpClient(ip, portNumber);
				Console.WriteLine("Connected to the Server...\n");
				networkStream = client.GetStream();

				writer = new StreamWriter(networkStream) {AutoFlush = true};
				reader = new StreamReader(client.GetStream());

				writer.WriteLine(method);
				writer.WriteLine("avatar.jpg");
				var fs = new FileStream(SendingFilePath, FileMode.Open, FileAccess.Read);
				var noOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fs.Length) / Convert.ToDouble(BufferSize)));
				var totalLength = (int) fs.Length;
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

				Console.WriteLine("Sent " + fs.Length + " bytes to the server");
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
		}
	}
}
