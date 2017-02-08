using System.Collections.Generic;
using System.Xml;

public interface IHaveUnderlyingNode
{
	XmlNode UnderlyingNode { get; }
}

public interface ICanHaveProperties : IHaveUnderlyingNode
{
}

public interface ICanHaveItems : IHaveUnderlyingNode
{
}

public interface ICanBeConditional
{
	string Condition { get; set; }
}

