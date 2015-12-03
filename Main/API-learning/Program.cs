using System;
using CWDev.SLNTools.Core;

class MainClass
{
	public static void Main(string[] args)
	{
		try
		{
			if (args.Length < 2)
			{
				Console.WriteLine ("{0}", "usage: API-learning {filename} {guid}");
				Environment.Exit(ReturnCodes.NO_ENOUGH_ARGUMENTS);
			}
			var filename = args [1];
			var project_id = args [2];
			var sol = SolutionFile.FromFile (filename);
			sol.Projects.Remove (project_id);
			sol.SaveAs (filename + ".new");
			Environment.Exit(ReturnCodes.SUCCESS);
		}
		catch (Exception ex)
		{
			Console.WriteLine (ex.ToString ());
			Environment.Exit(ReturnCodes.EXCEPTION);
		}
	}
}
