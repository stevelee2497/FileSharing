using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace FileSharingServer
{
	public static class Helper
	{
		public static bool IsSocketDisconnected(this Socket s)
		{
			return s.Poll(1000, SelectMode.SelectRead) && s.Available == 0;
		}
	}
}
