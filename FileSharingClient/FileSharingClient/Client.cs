using System;
using System.IO;
using System.Net.Mime;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FileSharingClient
{
	public class Client
	{
		public static string SendingFilePath = "C:\\Users\\Tranq\\Desktop\\IMG_3863.JPG";

		private const int BufferSize = 1024;
		private const string BaseUrl = "e:\\client";

		public static void Main(string[] args)
		{
			var method = "POST_FILE";
			var ip = "192.168.51.177";
			var portNumber = 8080;

			PostFile(ip, portNumber, method);

			//GetFileNames(ip, portNumber, "GET_FILES");

			//GetImage(ip, portNumber, "GET_IMAGE");

			Console.WriteLine("press any key to exit ...");
			Console.ReadKey();
		}

		private static void GetImage(string ip, int portNumber, string method)
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
				reader = new StreamReader(networkStream);

				var fileName = "avatar.jpg";

				writer.WriteLine(method);
				writer.WriteLine(fileName);

				var fileSize = Convert.ToInt64(reader.ReadLine());

				Console.WriteLine(fileSize);

				var image = new byte[fileSize];

				var recData = new byte[BufferSize];
				var stream = new MemoryStream(image);
				int recBytes;
				while ((recBytes = reader.BaseStream.Read(recData, 0, recData.Length)) > 0)
				{
					stream.Write(recData, 0, recBytes);
				}

				File.WriteAllBytes("d:\\client\\avatar.jpg", image);

				Console.WriteLine(stream.Length);
				stream.Close();
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

				reader = new StreamReader(networkStream);
				var result = reader.ReadLine();
				Console.WriteLine(result);
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
				//var fs = new FileStream(SendingFilePath, FileMode.Open, FileAccess.Read);
				//var noOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fs.Length) / Convert.ToDouble(BufferSize)));
				//var totalLength = (int) fs.Length;
				//writer.WriteLine(fs.Length);
				//for (var i = 0; i < noOfPackets; i++)
				//{
				//	int currentPacketLength;
				//	if (totalLength > BufferSize)
				//	{
				//		currentPacketLength = BufferSize;
				//		totalLength = totalLength - currentPacketLength;
				//	}
				//	else
				//		currentPacketLength = totalLength;

				//	var sendingBuffer = new byte[currentPacketLength];
				//	fs.Read(sendingBuffer, 0, currentPacketLength);
				//	writer.BaseStream.Write(sendingBuffer, 0, sendingBuffer.Length);
				//}

				var fileData = File.ReadAllBytes(SendingFilePath);
				writer.WriteLine(fileData.Length);

				writer.BaseStream.Write(fileData, 0, fileData.Length);

				while (true)
				{
					var result = reader.ReadLine();
					
					Console.WriteLine(result);

					if ("done".Equals(result))
					{
						writer.Close();
						Console.WriteLine("Sent " + fileData.Length + " bytes to the server");
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
		}
	}
}
