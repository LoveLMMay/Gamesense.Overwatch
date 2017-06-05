using System;
using System.IO;

namespace Gamesense.Overwatch
{
	public class Util
	{
		public static void WriteLog(string msg)
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "RazerInterceptor.log");
			using (var w = new StreamWriter(path, true))
			{
				w.WriteLine($"{DateTime.Now}: {msg}");
			}
		}
	}
}