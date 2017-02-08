using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MetaSpecTools.Filter
{
	using System;
	using Merge;

	public delegate bool AcceptDifferencesHandler(NodeDifference difference);

	public class FilterFile : ISolutionContext
    {
        public static FilterFile FromFile(string filterFullPath)
        {
            using (var stream = new FileStream(filterFullPath, FileMode.Open, FileAccess.Read))
            {
                return FromStream(filterFullPath, stream);
            }
        }

        public static FilterFile FromStream(string filterFullPath, Stream stream)
        {
            var filterFile = new FilterFile
                {
                    FilterFullPath = filterFullPath
                };

            var xmldoc = new XmlDocument();
            xmldoc.Load(stream);

            var configNode = xmldoc.SelectSingleNode("Config");

            var sourceSlnNode = configNode.SelectSingleNode("SourceSLN");
            filterFile.SourceSolutionFullPath = Path.Combine(
                        Path.GetDirectoryName(filterFullPath),
                        Path.GetFileName(sourceSlnNode.InnerText));

            XmlNode watchForChangesNode = configNode.SelectSingleNode("WatchForChangesOnFilteredSolution");
            if (watchForChangesNode != null)
            {
                filterFile.WatchForChangesOnFilteredSolution = bool.Parse(watchForChangesNode.InnerText);
            }
            XmlNode copyReSharperFilesNode = configNode.SelectSingleNode("CopyReSharperFiles");
            if (copyReSharperFilesNode != null)
            {
                filterFile.CopyReSharperFiles = bool.Parse(copyReSharperFilesNode.InnerText);
            }

            foreach (XmlNode node in configNode.SelectNodes("ProjectToKeep"))
            {
                filterFile.ProjectsToKeep.Add(node.InnerText);
            }

            return filterFile;
        }


        private FilteredSolutionWatcher m_watcher;

        public FilterFile()
        {
            this.SourceSolutionFullPath = null;
            this.FilterFullPath = null;
            this.ProjectsToKeep = new List<string>();
            this.WatchForChangesOnFilteredSolution = false;
            m_watcher = null;
        }

        public string SourceSolutionFullPath { get; set; }

        public SolutionFile SourceSolution
        {
            get { return SolutionFile.FromFile(this, this.SourceSolutionFullPath); }
        }

        public string FilterFullPath { get; set; }

        public string DestinationSolutionFullPath
        {
            get { return Path.ChangeExtension(this.FilterFullPath, ".sln"); }
        }

        public List<string> ProjectsToKeep { get; private set; }

        public bool WatchForChangesOnFilteredSolution { get; set; }

        public bool CopyReSharperFiles { get; set; }

		string ISolutionContext.FullPath
		{
			get
			{
				return String.Empty;
			}
		}

		IEnumerable<SolutionFile> ISolutionContext.Solutions
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public SolutionFile Apply()
        {
            return ApplyOn(this.SourceSolution);
        }

        public SolutionFile ApplyOn(SolutionFile original)
        {
            var includedProjects = new List<Project>();
            foreach (var projectFullName in this.ProjectsToKeep)
            {
                var projectToKeep = original.Projects.FindByFullName(projectFullName);
                if (projectToKeep != null)
                {
                    AddRecursiveDependenciesToList(includedProjects, projectToKeep);
                    foreach (var descendant in projectToKeep.AllDescendants)
                    {
                        AddRecursiveDependenciesToList(includedProjects, descendant);
                    }
                }
                else
                {
                    // TODO MessageBox Found project X in filter but doesn't exist in original solution
                }
            }

            return new SolutionFile(
                        this.DestinationSolutionFullPath,
                        original.Headers,
                        includedProjects,
                        original.GlobalSections);
        }

        private static void AddRecursiveDependenciesToList(List<Project> includedProjects, Project project)
        {
            if (includedProjects.Contains(project))
                return;

            includedProjects.Add(project);
            foreach (var dependency in project.Dependencies)
            {
                AddRecursiveDependenciesToList(includedProjects, dependency);
            }
        }

        public void StartFilteredSolutionWatcher(SolutionFile filteredSolution, AcceptDifferencesHandler handler)
        {
            if (this.WatchForChangesOnFilteredSolution && (m_watcher == null))
            {
                m_watcher = new FilteredSolutionWatcher(
                            handler,
                            this,
                            filteredSolution);
                m_watcher.Start();
            }
        }

        public void StopFilteredSolutionWatcher()
        {
            if (m_watcher != null)
            {
                m_watcher.Stop();
                m_watcher = null;
            }
        }

        public void Save()
        {
            SaveAs(this.FilterFullPath);
        }

        public void SaveAs(string filterFullPath)
        {
            var docFilter = new XmlDocument();

            XmlNode configNode = docFilter.CreateElement("Config");
            docFilter.AppendChild(configNode);

            XmlNode sourceSlnNode = docFilter.CreateElement("SourceSLN");
            sourceSlnNode.InnerText = Path.GetFileName(this.SourceSolutionFullPath);
            configNode.AppendChild(sourceSlnNode);

            XmlNode watchForChangesNode = docFilter.CreateElement("WatchForChangesOnFilteredSolution");
            watchForChangesNode.InnerText = this.WatchForChangesOnFilteredSolution.ToString();
            configNode.AppendChild(watchForChangesNode);

            XmlNode copyReSharperFilesNode = docFilter.CreateElement("CopyReSharperFiles");
            copyReSharperFilesNode.InnerText = this.CopyReSharperFiles.ToString();
            configNode.AppendChild(copyReSharperFilesNode);

            foreach (var projectFullName in this.ProjectsToKeep)
            {
                XmlNode node = docFilter.CreateElement("ProjectToKeep");
                node.InnerText = projectFullName;
                configNode.AppendChild(node);
            }

            docFilter.Save(filterFullPath);
        }
    }
}
