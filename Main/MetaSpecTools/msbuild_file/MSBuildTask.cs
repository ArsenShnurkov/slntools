using System;
using System.Collections.Generic;
using System.Xml;

public class MSBuildTask : ICanBeConditional
{
	MSBuildTarget parent;
	XmlElement uo;
	List<MSBuildTaskParameter> parameters = new List<MSBuildTaskParameter>();
	List<MSBuildTaskResultProperty> resultProperties = new List<MSBuildTaskResultProperty>();
	List<MSBuildTaskResultItem> resultItems = new List<MSBuildTaskResultItem>();
	public XmlNode UnderlyingObject
	{
		get
		{
			return uo;
		}
	}

	public string Condition { get { return GetCondition(); } set { SetCondition(value); } }

	public string Name { get { return uo.LocalName; } set { SetName(value); } }

	public MSBuildTask(MSBuildTarget p)
	{
		string name = "NoXmlElementName";
		this.parent = p;
		XmlDocument doc = parent.UnderlyingObject.OwnerDocument;
		uo = (XmlElement)doc.CreateNode(XmlNodeType.Element, name, MSBuildFile.NamespaceName);
	}

	void SetName(string name)
	{
		// replace underlaying object to change it's name
		XmlElement oldItem = uo;
		XmlDocument doc = oldItem.OwnerDocument;
		// replace name
		uo = (XmlElement)doc.CreateNode(XmlNodeType.Element, name, MSBuildFile.NamespaceName);
		//  what about node's text content ?
		uo.InnerText = oldItem.InnerText;
		// copy attributes
		foreach (XmlAttribute a in oldItem.Attributes)
		{
			uo.Attributes.Append((XmlAttribute)a.CloneNode(true));
		}
		// copy childs
		for (XmlNode child = oldItem.FirstChild; child != null; child = child.NextSibling)
		{
			uo.AppendChild(child.CloneNode(true));
		}
		if (oldItem.ParentNode != null)
		{
			oldItem.ParentNode.ReplaceChild(uo, oldItem);
		}
	}

	public MSBuildTaskParameter CreateParameter()
	{
		MSBuildTaskParameter res = new MSBuildTaskParameter(this);
		return res;
	}

	public MSBuildTaskResultProperty CreateResultProperty()
	{
		MSBuildTaskResultProperty res = new MSBuildTaskResultProperty(this);
		return res;
	}

	public MSBuildTaskResultItem CreateResultItem()
	{
		MSBuildTaskResultItem res = new MSBuildTaskResultItem(this);
		return res;
	}

	public void AppendParameter(MSBuildTaskParameter attribute)
	{
		// insert on this level
		this.parameters.Add(attribute);
		// insert on underlaying level
		XmlAttribute tn = attribute.UnderlyingObject;
		uo.SetAttributeNode(tn);
	}

	public void AppendResultProperty(MSBuildTaskResultProperty subNode)
	{
		// insert on this level
		this.resultProperties.Add(subNode);
		// insert on underlaying level
		XmlNode tn = subNode.UnderlyingObject;
		uo.AppendChild(tn);
	}

	public void AppendResultItem(MSBuildTaskResultItem subNode)
	{
		// insert on this level
		this.resultItems.Add(subNode);
		// insert on underlaying level
		XmlNode tn = subNode.UnderlyingObject;
		uo.AppendChild(tn);
	}

	public void AddParameter(string name, string val)
	{
		MSBuildTaskParameter parameter = this.CreateParameter();
		parameter.Name = name;
		parameter.Value = val;
		this.AppendParameter(parameter);
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
}
