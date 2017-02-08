using System;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Xml;

public class MSBuildFile : IDisposable
{
	public const string NamespaceName = "http://schemas.microsoft.com/developer/msbuild/2003";
	XmlDocument doc;
	bool bSaveRequired = false;
	string filename = null;
	List<MSBuildImport> importNodes = new List<MSBuildImport>();
	List<MSBuildTarget> targetNodes = new List<MSBuildTarget>();
	public XmlDocument UnderlyingObject
	{
		get
		{
			return doc;
		}
	}
	public string FileName
	{
		get
		{
			return filename;
		}
		set
		{
			filename = value;
		}
	}

	public MSBuildFile(XmlDocument d)
	{
		this.doc = d;
	}
	public MSBuildFile(string filename)
	{
		XmlDocument d = new XmlDocument();
		d.Load(filename);
		this.doc = d;
		this.filename = filename;
		FindAllImports();
		FindAllTargets();
	}

	public void Dispose()
	{
		if (bSaveRequired && String.IsNullOrWhiteSpace(filename) == false)
		{
			doc.Save(filename);
		}
	}

	public void AddTarget(MSBuildTarget target)
	{
		//  it does not automatically add the new object to the document tree.
		// To add the new object, one must explicitly call one of the node insert methods.
		XmlNode n = target.UnderlyingObject;
		XPathNavigator locator = doc.CreateNavigator();
		locator.MoveToRoot();
		XmlNode root = locator.UnderlyingObject as XmlNode;
		if (locator.MoveToFirstChild() == false)
		{
			root.AppendChild(n);
		}
		else
		{
			while (locator.MoveToNext()) { };
			XmlNode sibling = locator.UnderlyingObject as XmlNode;
			root.InsertAfter(n, sibling);
		}
		bSaveRequired = true;
	}

	static bool IsAlreadyExists(string import_name, XmlNamespaceManager xmlNamespaceManager, XPathNavigator navigator)
	{
		var xpath1 = "/ns:Project/ns:Import[@Project='" + import_name + "']";
		XPathExpression expr1 = navigator.Compile(xpath1);
		expr1.SetContext(xmlNamespaceManager);
		var nodeIterator1 = navigator.Select(expr1);
		if (nodeIterator1.Count > 0)
		{
			return true;
		}
		return false;
	}

	public MSBuildImport CreateImport()
	{
		var result = new MSBuildImport(this);
		return result;
	}

	public void FindAllImports()
	{
		var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
		xmlNamespaceManager.AddNamespace("ns", MSBuildFile.NamespaceName);

		XPathNavigator navigator = doc.CreateNavigator();
		navigator.MoveToRoot();

		var xpath = "/ns:Project/ns:Import[@Project]";
		XPathExpression expr = navigator.Compile(xpath);
		expr.SetContext(xmlNamespaceManager);

		var nodeIterator = navigator.Select(expr);
		if (nodeIterator.Count == 0)
		{
			return;
		}
		while (nodeIterator.MoveNext())
		{
			if (nodeIterator.Current is IHasXmlNode)
			{
				XmlNode node = ((IHasXmlNode)nodeIterator.Current).GetNode();
				XmlElement element = (XmlElement)node;
				MSBuildImport wrapperObject = new MSBuildImport(this, element);
				importNodes.Add(wrapperObject);
			}
		}
		// see also https://weblogs.asp.net/cazzu/86609
	}

	// locate if there is import of Microsoft.CSharp.targets
	public MSBuildImport FindImport(string v)
	{
		foreach (MSBuildImport item in this.importNodes)
		{
			if (string.Compare(item.Project, v) == 0)
			{
				return item;
			}
		}
		return null;
	}

	public void InsertImport(MSBuildImport newImport)
	{
		if (FindImport(newImport.Project) != null)
		{
			return;
		}

		// запомнить у себя
		this.importNodes.Add(newImport);

		// у тебя в руках узел, но оне вставленный в XML-документ
		XmlElement newXmlElement = newImport.UnderlyingObject;

		XPathNavigator navigator = doc.CreateNavigator();
		navigator.MoveToRoot();

		XmlNode root = (XmlNode)navigator.UnderlyingObject;
		XmlNode lc = root.LastChild;
		lc.AppendChild(newXmlElement);

		bSaveRequired = true;
	}

	public void InsertImportAfter(MSBuildImport existingImport, MSBuildImport newImport)
	{
		// запомнить у себя
		this.importNodes.Add(newImport);
		// вставить в нижележащий слой
		XmlElement existingElement = existingImport.UnderlyingObject;
		XmlElement newElement = newImport.UnderlyingObject;
		existingElement.ParentNode.InsertAfter(newElement, existingElement);

		bSaveRequired = true;
	}

	public MSBuildTarget CreateTarget()
	{
		var result = new MSBuildTarget(this);
		return result;
	}

	public void FindAllTargets()
	{
		var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
		xmlNamespaceManager.AddNamespace("ns", MSBuildFile.NamespaceName);

		XPathNavigator navigator = doc.CreateNavigator();
		navigator.MoveToRoot();

		var xpath = "/ns:Project/ns:Target[@Name]";
		XPathExpression expr = navigator.Compile(xpath);
		expr.SetContext(xmlNamespaceManager);

		var nodeIterator = navigator.Select(expr);
		if (nodeIterator.Count == 0)
		{
			return;
		}
		while (nodeIterator.MoveNext())
		{
			if (nodeIterator.Current is IHasXmlNode)
			{
				XmlNode node = ((IHasXmlNode)nodeIterator.Current).GetNode();
				XmlElement element = (XmlElement)node;
				MSBuildTarget wrapperObject = new MSBuildTarget(this, element);
				targetNodes.Add(wrapperObject);
			}
		}
		// see also https://weblogs.asp.net/cazzu/86609
	}

	public MSBuildTarget FindTarget(string v)
	{
		foreach (MSBuildTarget item in this.targetNodes)
		{
			if (string.Compare(item.Name, v) == 0)
			{
				return item;
			}
		}
		return null;
	}

	public void InsertTarget(MSBuildTarget newTarget)
	{
		if (FindTarget(newTarget.Name) != null)
		{
			return;
		}

		// запомнить у себя
		this.targetNodes.Add(newTarget);

		// у тебя в руках узел, но оне вставленный в XML-документ
		XmlElement newXmlElement = newTarget.UnderlyingObject;

		XPathNavigator navigator = doc.CreateNavigator();
		navigator.MoveToRoot();

		XmlNode root = (XmlNode)navigator.UnderlyingObject;
		XmlNode project = root.LastChild;
		project.AppendChild(newXmlElement);

		bSaveRequired = true;
	}

	public void EnsureTargetExists(string targetName)
	{
		if (FindTarget(targetName) != null)
		{
			return;
		}
		MSBuildTarget targ = this.CreateTarget();
		targ.Name = targetName;
		InsertTarget(targ);

		bSaveRequired = true;
	}

	public void AddAfterTarget(string precedingTarget, string followingTarget)
	{
		MSBuildTarget preceding = FindTarget(precedingTarget);
		if (preceding == null)
		{
			throw new ApplicationException($"Target {precedingTarget} not found");
		}
		MSBuildTarget following = FindTarget(followingTarget);
		if (following == null)
		{
			throw new ApplicationException($"Target {followingTarget} not found");
		}
		preceding.AddAfterTarget(following.Name);

		bSaveRequired = true;
	}

	public void AddDependOnTarget(string precedingTarget, string followingTarget)
	{
		MSBuildTarget preceding = FindTarget(precedingTarget);
		if (preceding == null)
		{
			throw new ApplicationException($"Target {precedingTarget} not found");
		}
		MSBuildTarget following = FindTarget(followingTarget);
		if (following == null)
		{
			throw new ApplicationException($"Target {followingTarget} not found");
		}
		preceding.AddDependOnTarget(following.Name);

		bSaveRequired = true;
	}
}
