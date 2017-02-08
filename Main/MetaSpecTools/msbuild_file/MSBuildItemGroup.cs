using System;
using System.Collections.Generic;
using System.Xml;

public class MSBuildItemGroup
{
	XmlElement uo;

	public XmlElement UnderlyingObject { get { return uo; } }

	IEnumerable<MSBuildItem> Items { get; }

	//ICanHaveItems parent;

	public MSBuildItemGroup(ICanHaveItems parent)
	{
		//this.parent = parent;
		XmlDocument doc = parent.UnderlyingNode.OwnerDocument;
		uo = (XmlElement)doc.CreateNode(XmlNodeType.Element, "UndefilnedItemName", MSBuildFile.NamespaceName);
	}

	public MSBuildItem CreateItem()
	{
		MSBuildItem res = new MSBuildItem(this);
		return res;
	}
	public void AppendItem(MSBuildItem item)
	{
		throw new NotImplementedException();
	}
}

