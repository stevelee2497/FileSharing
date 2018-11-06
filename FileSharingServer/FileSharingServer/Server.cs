using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileSharingServer
{
	public class Server
	{
		private const int PortNumber = 9999;

		public static void Main(string[] args)
		{
			var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), PortNumber);
			listener.Start();

			Console.WriteLine("Server started on " + listener.LocalEndpoint);
			Console.WriteLine("Waiting for a connection...");

			var socket = listener.AcceptSocket();
			Console.WriteLine("Connection received from " + socket.RemoteEndPoint);

			var stream = new NetworkStream(socket);
			var reader = new StreamReader(stream);
			var writer = new StreamWriter(stream) { AutoFlush = true };

			var str = reader.ReadToEnd();

			writer.WriteLine("Hello " + str);

			stream.Close();
			socket.Close();
			Console.Read();
		}
	}
}
