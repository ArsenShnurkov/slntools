using System;
using System.Collections.Generic;
using System.Xml;

public class MSBuildPropertyGroup : ICanBeConditional
{
	XmlElement uo;
	List<MSBuildProperty> properties = new List<MSBuildProperty>();

	public XmlElement UnderlyingObject { get { return uo; } }
	public IEnumerable<MSBuildProperty> Properties { get { return properties; } }

	public string Condition { get { return GetCondition(); } set { SetCondition(value); } }

	public MSBuildPropertyGroup(ICanHaveProperties parent)
	{
		//this.parent = parent;
		XmlDocument doc = parent.UnderlyingNode.OwnerDocument;
		uo = (XmlElement)doc.CreateNode(XmlNodeType.Element, "PropertyGroup", MSBuildFile.NamespaceName);
	}

	string GetCondition()
	{
		if (uo.HasAttribute("Condition") == false) return null;
		return uo.Attributes["Condition"].Value;
	}

	void SetCondition(string value)
	{
		uo.SetAttribute("Condition", value);
	}

	public MSBuildProperty CreateProperty()
	{
		MSBuildProperty res = new MSBuildProperty(this);
		return res;
	}

	public void AppendProperty(MSBuildProperty item)
	{
		// insert on this level
		this.properties.Add(item);
		// insert on underlaying level
		XmlNode tn = item.UnderlyingObject;
		uo.AppendChild(tn);
	}

	public void AddProperty(string name, string val)
	{
		MSBuildProperty prop = this.CreateProperty();
		prop.Name = name;
		prop.Value = val;
		this.AppendProperty(prop);
	}
}
