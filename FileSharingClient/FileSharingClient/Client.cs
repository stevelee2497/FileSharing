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
			{
				try
				{
					TcpClient client = new TcpClient();

					// 1. connect
					client.Connect("127.0.0.1", PortNumber);
					Stream stream = client.GetStream();

					Console.WriteLine("Connected to Y2Server.");
					while (true)
					{
						Console.Write("Enter your name: ");

						string str = Console.ReadLine();
						var reader = new StreamReader(stream);
						var writer = new StreamWriter(stream) {AutoFlush = true};

						// 2. send
						writer.WriteLine(str);

						// 3. receive
						str = reader.ReadLine();
						Console.WriteLine(str);
						if (str?.ToUpper() == "BYE")
							break;
					}
					// 4. close
					stream.Close();
					client.Close();
				}

				catch (Exception ex)
				{
					Console.WriteLine("Error: " + ex);
				}

				Console.Read();
			}
			
		}
	}
}
