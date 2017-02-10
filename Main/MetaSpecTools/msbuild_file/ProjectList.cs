using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MetaSpecTools
{
	interface IProjectList : IEnumerable<Project>
	{
		Project FindByFullName(string projectFullName);
		Project FindByGuid(string guid);
		bool Contains(Project p);
		bool Remove(Project p);
		void Sort();
	}

	public class ProjectList : IProjectList
	{
		Dictionary<string, Project> m_Projects = new Dictionary<string, Project>();
		List<Project> m_orderedList = new List<Project>();

		public ProjectList()
		{
		}

		public ProjectList(ProjectList original)
		{
			foreach (var guid in original.m_Projects.Keys)
			{
				Project p = original.m_Projects[guid];
				AddWithGuid(guid, p);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return m_orderedList.GetEnumerator();
		}

		IEnumerator<Project> IEnumerable<Project>.GetEnumerator()
		{
			return m_orderedList.GetEnumerator();
		}

		public void AddWithGuid(string guid, Project item)
		{
			m_Projects.Add(guid, item);
		}

		public Project FindByFullName(string projectFullName)
		{
			foreach (Project p in m_orderedList)
			{
				if (string.Compare(p.FileName, projectFullName) == 0)
				{
					return p;
				}
			}
			return null;
		}

		public Project FindByGuid(string guid)
		{
			foreach (var pair in m_Projects)
			{
				if (string.Compare(pair.Key, guid) == 0)
				{
					return pair.Value;
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

			m_orderedList.Clear();
			m_orderedList.AddRange(tempList);
		}

		public bool Contains(Project p)
		{
			return m_orderedList.Contains(p);
		}

		public bool Remove(Project p)
		{
			bool bRemoved1 = m_orderedList.Contains(p);
			int nRemoved2 = 0;
			string lastKey = string.Empty;
			foreach (var pair in this.m_Projects)
			{
				if (pair.Value == p)
				{
					lastKey = pair.Key;
					nRemoved2++;
				}
			}
			if (bRemoved1 == false && nRemoved2 == 0)
			{
				return false;
			}
			if (bRemoved1 == true && nRemoved2 == 1)
			{
				m_orderedList.Remove(p);
				m_Projects.Remove(lastKey);
				return true;
			}
			Debugger.Break();
			throw new ApplicationException();
		}
	}
}
