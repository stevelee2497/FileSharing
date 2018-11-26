using System;
using System.IO;
using System.Net.Sockets;

namespace FileSharingClient
{
	public class Client
	{
		private const int PortNumber = 9999;

		public static void Main(string[] args)
		{
			Console.ReadKey();
			using (var client = new TcpClient("127.0.0.1", PortNumber))
			using (var stream = client.GetStream())
			using (var reader = new StreamReader(stream))
			using (var writer = new StreamWriter(stream) { AutoFlush = true })
			{
				var content = File.ReadAllBytes("e:\\ava.jpg");
				stream.Write(content, 0, content.Length);

				var result = reader.ReadLine();

				Console.WriteLine(result);
			}
			Console.ReadKey();
		}
	}
}
