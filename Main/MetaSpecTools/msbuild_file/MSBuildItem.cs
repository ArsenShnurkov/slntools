using System;
using System.Xml;

public class MSBuildItem
{
	XmlElement uo;

	public XmlElement UnderlyingObject { get { return uo; } }

	public MSBuildItem(MSBuildItemGroup parent)
	{
		XmlDocument doc = parent.UnderlyingObject.OwnerDocument;
		uo = (XmlElement)doc.CreateNode(XmlNodeType.Element, "UndefilnedItemName", MSBuildFile.NamespaceName);
	}
}

