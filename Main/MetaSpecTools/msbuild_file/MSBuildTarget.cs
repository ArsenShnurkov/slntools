using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

public class MSBuildTarget : ICanHaveProperties, ICanHaveItems
{
	MSBuildFile file;
	XmlElement uo;
	List<MSBuildPropertyGroup> propertyGroups = new List<MSBuildPropertyGroup>();
	List<MSBuildTask> tasks = new List<MSBuildTask>();
	List<MSBuildItem> items = new List<MSBuildItem>();

	public string Name { get { return uo.Attributes["Name"].Value; } set { uo.SetAttribute("Name", value); } }
	public IEnumerable<MSBuildTask> Tasks { get { return tasks; } }
	public XmlElement UnderlyingObject { get { return uo; } }
	public XmlNode UnderlyingNode { get { return UnderlyingObject; } }
	public IEnumerable<MSBuildPropertyGroup> PropertyGroups {get { return propertyGroups; } }
	public IEnumerable<MSBuildItem> Items { get { return items; } }

	public MSBuildTarget(MSBuildFile f)
	{
		this.file = f;
		uo = (XmlElement)file.UnderlyingObject.CreateNode(XmlNodeType.Element, "Target", MSBuildFile.NamespaceName);
	}

	public MSBuildTarget(MSBuildFile f, XmlElement el)
	{
		this.file = f;
		uo = el;
	}

	public MSBuildTask CreateTask()
	{
		MSBuildTask res = new MSBuildTask(this);
		return res;
	}

	public MSBuildPropertyGroup CreatePropertyGroup()
	{
		MSBuildPropertyGroup res = new MSBuildPropertyGroup(this);
		return res;
	}

	public void AppendTask(MSBuildTask task)
	{
		// insert on this level
		this.tasks.Add(task);
		// insert on underlaying level
		XmlNode tn = task.UnderlyingObject;
		uo.AppendChild(tn);
	}

	public void AppendPropertyGroup(MSBuildPropertyGroup item)
	{
		// insert on this level
		this.propertyGroups.Add(item);
		// insert on underlaying level
		XmlNode tn = item.UnderlyingObject;
		uo.AppendChild(tn);
	}

	public void AppendItem(MSBuildItem item)
	{
		// insert on this level
		this.items.Add(item);
		// insert on underlaying level
		XmlNode tn = item.UnderlyingObject;
		uo.AppendChild(tn);
	}

	public readonly char[] TargetSeparators = new char[] { ';' };

	void AppendListOfTargets(string attributeName, string nameToAdd)
	{
		string attr = uo.GetAttribute(attributeName);
		string[] targets = attr.Split(TargetSeparators);
		for (int i = 0; i < targets.Length; i++)
		{
			if (string.Compare(targets[i], nameToAdd) == 0)
			{
				return;
			}
		}
		StringBuilder res = new StringBuilder();
		for (int i = 0; i < targets.Length; i++)
		{
			if (i > 0)
			{
				res.Append(TargetSeparators);
			}
			res.Append(targets[i]);
		}
		if (res.Length > 0)
		{
			res.Append(TargetSeparators);
		}
		res.Append(nameToAdd);
		uo.SetAttribute(attributeName, res.ToString());
	}

	public void AddAfterTarget(string name)
	{
		AppendListOfTargets("AfterTargets", name);
	}

	public void AddDependOnTarget(string name)
	{
		AppendListOfTargets("DependsOnTargets", name);
	}
}
