using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MetaSpecTools
{
	public class ProjectProxyList
		: KeyedCollection<string, Project>
	{
		protected readonly IProjectContext r_container;

		public ProjectProxyList(IProjectContext projectContext)
			: base(StringComparer.InvariantCultureIgnoreCase)
		{
			if (projectContext == null)
				throw new ArgumentNullException(nameof(projectContext));

			r_container = projectContext;
		}

		public ProjectProxyList(IProjectContext projectContext, IEnumerable<Project> items)
			: this(projectContext)
		{
			AddRange(items);
		}

		protected override string GetKeyForItem(Project item)
		{
			return item.ProjectGuid;
		}

		public IProjectContext Container
		{
			get { return r_container; }
		}

		public void AddRange(IEnumerable<Project> items)
		{
			if (items != null)
			{
				foreach (var item in items)
				{
					Add(item);
				}
			}
		}

		public Project FindByGuid(string guid)
		{
			return (Contains(guid)) ? this[guid] : null;
		}

		public Project FindByFullName(string projectFullName)
		{
			foreach (var item in this)
			{
				if (string.Compare(item.ProjectFullName, projectFullName, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return item;
				}
			}
			return null;
		}

		public void Sort()
		{
			Sort((p1, p2) => StringComparer.InvariantCultureIgnoreCase.Compare(p1.ProjectFullName, p2.ProjectFullName));
		}

		public void Sort(System.Comparison<Project> comparer)
		{
			var tempList = new List<Project>(this);
			tempList.Sort(comparer);

			Clear();
			AddRange(tempList);
		}
	}
}
