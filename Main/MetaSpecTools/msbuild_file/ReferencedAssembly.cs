using System;
namespace MetaSpecTools
{
	public class ReferencedAssembly
	{
		string m_Specification;
		string m_Package;
		public ReferencedAssembly()
		{
		}
		public ReferencedAssembly(string spec)
		{
			m_Specification = spec;
		}
		public ReferencedAssembly(string spec, string package)
		{
			m_Specification = spec;
			m_Package = package;
		}
		public string AssemblyName
		{
			get
			{
				return m_Specification;
			}
		}
		public string Package
		{
			get
			{
				return m_Package;
			}
		}
	}
}

