using System;
using System.Collections.Generic;
using System.Text;

namespace FileSharingClient.Models
{
	public class FileSharingData
	{
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public byte[] FileData { get; set; }
	}
}
