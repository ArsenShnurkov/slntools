using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MetaSpecTools
{
	public class FsPath
	{
		public static string Combine(string a, string b)
		{
			if (a.StartsWith(b)) return a;
			if (b.StartsWith(a)) return b;
			StringBuilder res = new StringBuilder(Path.Combine(a, b));
			if (res.ToString().IndexOf("..") >= 0)
			{
				//Debugger.Break();
			}
			res.Replace("\\", new string(Path.DirectorySeparatorChar, 1));
			int index;
			for (;;)
			{
				string up = Path.DirectorySeparatorChar + "..";
				string str = res.ToString();
				index = str.IndexOf(up);
				if (index < 0) break;
				int index2 = index - 1;
				while (index2 >= 0 && res[index2] != Path.DirectorySeparatorChar)
				{
					index2--;
				}
				res.Remove(index2, index + up.Length - index2);
			}
			for (;;)
			{
				string inplace = Path.DirectorySeparatorChar + ".";
				string str = res.ToString();
				index = str.IndexOf(inplace);
				if (index < 0) break;
				int index2 = index - 1;
				while (index2 >= 0 && res[index2] != Path.DirectorySeparatorChar)
				{
					index2--;
				}
				res.Remove(index2+1, index + inplace.Length - index2);
			}
			if (res.ToString().IndexOf(Path.DirectorySeparatorChar + ".") >= 0)
			{
				throw new ApplicationException("something gone wrong");
			}
			return res.ToString();
		}
	}
}

