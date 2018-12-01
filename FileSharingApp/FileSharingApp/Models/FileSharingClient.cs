using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Java.Lang;
using Exception = System.Exception;

namespace FileSharingApp.Models
{
	public class FileSharingClient
	{
		private const int BufferSize = 1024;
		private readonly string _hostIp;
		private readonly int _portNumber;
		private TcpClient _client;
		private NetworkStream _networkStream;
		private StreamReader _reader;
		private StreamWriter _writer;

		public FileSharingClient(string hostIp, int portNumber)
		{
			_hostIp = hostIp;
			_portNumber = portNumber;
		}

		public void Connect()
		{
			_client = new TcpClient(_hostIp, _portNumber);
			_networkStream = _client.GetStream();
			_reader = new StreamReader(_networkStream);
			_writer = new StreamWriter(_networkStream) { AutoFlush = true };
			Console.WriteLine("Connected to the Server...\n");
		}

		public void Disconnect()
		{
			_writer?.Close();
			_reader?.Close();
			_networkStream?.Close();
			_client?.Close();
		}

		public string PostFile(string userName, FileSharingData file)
		{
			string result = null;
			try
			{
				Connect();

				var header = string.Join(",", Method.PostFile, userName, file.FileName, file.FileData.Length);
				_writer.WriteLine(header);

				Thread.Sleep(50);

				_writer.BaseStream.Write(file.FileData, 0, file.FileData.Length);
				_writer.BaseStream.Flush();

				result = _reader.ReadLine();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Disconnect();
			}
			return result;
		}

		public List<string> GetFileNames(string userName)
		{
			List<string> result = null;
			try
			{
				Connect();

				var header = string.Join(",", Method.GetFileNames, userName);
				_writer.WriteLine(header);

				result = _reader.ReadLine()?.Split(',').ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Disconnect();
			}
			return result;
		}

		public string Login(string userName, string password)
		{
			string result = null;
			try
			{
				Connect();

				var header = string.Join(",", Method.Login, userName, password);
				_writer.WriteLine(header);

				result = _reader.ReadLine();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Disconnect();
			}
			return result;
		}

		public FileSharingData GetImage(string userName, string fileName)
		{
			FileSharingData file = null;
			try
			{
				Connect();
				var header = string.Join(",", Method.GetFile, userName, fileName);
				_writer.WriteLine(header);

				var fileSize = Convert.ToInt64(_reader.ReadLine());
				var data = ReadFileFromStream(fileSize);
				file = new FileSharingData
				{
					FileName = fileName,
					FileData = data
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Disconnect();
			}
			return file;
		}

		private byte[] ReadFileFromStream(long fileSize)
		{
			var data = new byte[fileSize];
			var recData = new byte[BufferSize];
			var stream = new MemoryStream(data);
			var recBytes = 1;
			while (stream.Position < fileSize && recBytes > 0)
			{
				recBytes = _reader.BaseStream.Read(recData, 0, recData.Length);
				stream.Write(recData, 0, recBytes);
			}
			stream.Close();
			return data;
		}
	}
}