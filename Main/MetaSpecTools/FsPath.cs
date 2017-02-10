using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MetaSpecTools
{
	public class FsPath
	{
		/// <summary>
		/// Combine the specified paths a and b.
		/// </summary>
		/// <param name="a">The first part of path component.</param>
		/// <param name="b">The second part of path component.</param>
		/// <remarks>
		/// Alternative for
		/// <br />
		/// new FileInfo(sln.SolutionFullName).FullName
		/// <br />
		/// which doesn't require file presence
		/// <remark>
		public static string Combine(string a, string b)
		{
			if (string.IsNullOrEmpty(a)) a = String.Empty;
			if (string.IsNullOrEmpty(b)) b = String.Empty;
			if (a == String.Empty) return b;
			if (b == String.Empty) return a;
			if (a.StartsWith(b, StringComparison.InvariantCulture)) return a;
			if (b.StartsWith(a, StringComparison.InvariantCulture)) return b;
			StringBuilder res = new StringBuilder(Path.Combine(a, b));
			if (res.ToString().IndexOf("..", StringComparison.InvariantCulture) >= 0)
			{
				//Debugger.Break();
			}
			res.Replace("\\", new string(Path.DirectorySeparatorChar, 1));
			int index;
			for (;;)
			{
				string up = Path.DirectorySeparatorChar + "..";
				string str = res.ToString();
				index = str.IndexOf(up, StringComparison.InvariantCulture);
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
				index = str.IndexOf(inplace, StringComparison.InvariantCulture);
				if (index < 0) break;
				int index2 = index - 1;
				while (index2 >= 0 && res[index2] != Path.DirectorySeparatorChar)
				{
					index2--;
				}
				res.Remove(index2+1, index + inplace.Length - index2);
			}
			if (res.ToString().IndexOf(Path.DirectorySeparatorChar + ".", StringComparison.InvariantCulture) >= 0)
			{
				throw new ApplicationException("something gone wrong");
			}
			return res.ToString();
		}
	}
}

