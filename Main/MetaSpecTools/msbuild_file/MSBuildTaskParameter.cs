using System;
using System.Xml;

public class MSBuildTaskParameter
{
	// https://github.com/mono/mono/blob/master/mcs/class/referencesource/System.Xml/System/Xml/Dom/XmlAttribute.cs
	XmlAttribute uo;
	MSBuildTask parent;

	public XmlAttribute UnderlyingObject
	{
		get
		{
			return uo;
		}
	}

	public string Name { get { return uo.LocalName; } set { SetName(value); } }
	public string Value { get { return uo.Value; } set { uo.Value = value; } }

	public MSBuildTaskParameter(MSBuildTask p)
	{
		string name = "NoAttributeNameGiven";
		this.parent = p;
		XmlDocument doc = parent.UnderlyingObject.OwnerDocument;
		uo = (XmlAttribute)doc.CreateNode(XmlNodeType.Attribute, name, null/*MSBuildFile.NamespaceName*/);
	}

	void SetName(string name)
	{
		// replace underlaying object to change it's name
		XmlAttribute oldAttr = uo;
		XmlDocument doc = oldAttr.OwnerDocument;
		uo = (XmlAttribute)doc.CreateNode(XmlNodeType.Attribute, name, null/*MSBuildFile.NamespaceName*/);
		uo.Value = oldAttr.Value;
		if (oldAttr.ParentNode != null)
		{
			oldAttr.ParentNode.ReplaceChild(uo, oldAttr);
		}
	}
}
