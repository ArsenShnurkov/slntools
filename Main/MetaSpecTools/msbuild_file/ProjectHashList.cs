using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MetaSpecTools
{
	public class ProjectHashList : ProjectProxyList
	{
		public ProjectHashList(IProjectContext projectContext)
			: base(projectContext)
		{
		}

		public ProjectHashList(IProjectContext projectContext, IEnumerable<Project> items)
			: base(projectContext, items)
		{
		}

		protected override void InsertItem(int index, Project item)
		{
			// Add a clone of the item instead of the item itself
			base.InsertItem(index, new Project(r_container, item));
		}

		protected override void SetItem(int index, Project item)
		{
			// Add a clone of the item instead of the item itself
			base.SetItem(index, new Project(r_container, item));
		}
	}
}
