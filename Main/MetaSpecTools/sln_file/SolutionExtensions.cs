using System;
using System.Collections.Generic;

namespace MetaSpecTools
{
	public static class SolutionExtensions
	{
		static IEnumerable<Project> GetProjectsByTypeGuid(this SolutionFile solutionFile, string typeGuid)
		{
			List<Project> res = new List<Project>();
			return res;
		}
	}
}

