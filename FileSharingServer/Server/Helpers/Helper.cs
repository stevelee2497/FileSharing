using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Server.Helpers
{
	public static class Helper
	{
		public static bool IsSocketDisconnected(this Socket s)
		{
			return s.Poll(1000, SelectMode.SelectRead) && s.Available == 0;
		}

		public static bool SignUp(string userName, string password)
		{
			var result = true;
			var xlApp = new Microsoft.Office.Interop.Excel.Application();
			var xlWorkbook = xlApp.Workbooks.Open(@"E:\server\data.xlsx");
			var xlWorksheet = xlWorkbook.Sheets[1];

			var xlRange = xlWorksheet.UsedRange;
			var rowCount = xlRange.Rows.Count;
			for (var i = 1; i <= rowCount; i++)
			{
				if (userName.Equals(xlRange.Cells[i, 1].Value2.ToString()))
				{
					result = false;
				}
			}

			xlWorksheet.Cells[rowCount + 1, 1] = userName;
			xlWorksheet.Cells[rowCount + 1, 2] = password;

			xlWorkbook.Save();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			Marshal.ReleaseComObject(xlRange);
			Marshal.ReleaseComObject(xlWorksheet);
			xlWorkbook.Close();
			Marshal.ReleaseComObject(xlWorkbook);
			xlApp.Quit();
			Marshal.ReleaseComObject(xlApp);

			return result;
		}

		public static bool Login(string userName, string password)
		{
			var result = false;

			var xlApp = new Microsoft.Office.Interop.Excel.Application();
			var xlWorkbook = xlApp.Workbooks.Open(@"E:\server\data.xlsx");
			Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
			var xlRange = xlWorksheet.UsedRange;

			var rowCount = xlRange.Rows.Count;

			for (var i = 1; i <= rowCount; i++)
			{
				if (userName.Equals(xlRange.Cells[i, 1].Value2.ToString()) && password.Equals(xlRange.Cells[i, 2].Value2.ToString()))
				{
					result = true;
				}
			}

			//cleanup
			GC.Collect();
			GC.WaitForPendingFinalizers();

			//release com objects to fully kill excel process from running in the background
			Marshal.ReleaseComObject(xlRange);
			Marshal.ReleaseComObject(xlWorksheet);

			//close and release
			xlWorkbook.Close();
			Marshal.ReleaseComObject(xlWorkbook);

			//quit and release
			xlApp.Quit();
			Marshal.ReleaseComObject(xlApp);

			return result;
		}
	}
}
