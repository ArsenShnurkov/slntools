using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace MetaSpecTools
{
    using Merge;

	public interface ISolutionContext
	{
		string FullPath { get; }

		IEnumerable<SolutionFile> Solutions { get; }
		SolutionFile LoadSolution(string path);

		IProjectContext ProjectContext { get; }
	}

	public class SolutionFile : IProjectContext
	{
		#region public static: Methods FromFile / FromStream

		public static SolutionFile FromFile(string path, ISolutionContext solutionContext)
		{
			string solutionFullPath = FsPath.Combine(solutionContext.FullPath, path);
			using (var reader = new SolutionFileReader(solutionFullPath, solutionContext))
			{
				var f = reader.LoadSolutionFile(solutionFullPath);
				return f;
			}
		}

		public static SolutionFile FromStream(ISolutionContext context, string path, Stream stream)
		{
			using (var reader = new SolutionFileReader(stream))
			{
				var f = reader.CreateSolutionFile();
				f.SolutionFullName = FsPath.Combine(context.FullPath, path);
				return f;
			}
		}

		#endregion

		public const string DefaultExtension = ".sln";
		public const string UndefinedName = "undefined" + DefaultExtension;
		private string m_solutionFullName;
		IProjectContext m_projectContext = null;
		private ProjectList m_projects;
		private readonly List<String> r_warnings;

		public SolutionFile(ISolutionContext ctx) : this()
		{
			m_solutionFullName = Path.Combine(ctx.FullPath, SolutionFile.UndefinedName);
			m_projectContext = ctx.ProjectContext;
			m_projects = new ProjectList();
		}

		public SolutionFile()
        {
            m_solutionFullName = null;
			m_projectContext = null;
			m_projects = new ProjectList();

			this.Headers = new List<string>();
			this.GlobalSections = new SectionHashList();
			r_warnings = new List<string>();
		}

        public SolutionFile(SolutionFile original)
			: this(original.FullPath, original.Headers, original.m_projects, original.GlobalSections)
        {
        }

		public SolutionFile(string fullpath, IEnumerable<string> headers, ProjectList projects, IEnumerable<Section> globalSections)
        {
            m_solutionFullName = fullpath;
            this.Headers = new List<string>(headers);
			var projectContext = (this as ISolutionContext).ProjectContext;
			this.m_projects = new ProjectList(projects);
            this.GlobalSections = new SectionHashList(globalSections);
        }

        public string SolutionFullName
        {
            get { return m_solutionFullName; }
            set { m_solutionFullName = value; }
        }

        public List<string> Headers { get; private set; }

		public ProjectList Projects { get { return m_projects; } private set { m_projects = value;} }

        public SectionHashList GlobalSections { get; private set; }

        public ReadOnlyCollection<string> Warnings { get { return new ReadOnlyCollection<string>(r_warnings);} }

        public IEnumerable<Project> Childs
        {
            get
            {
                foreach (Project project in this.Projects)
                {
                    if (project.ParentFolder == null)
                    {
                        yield return project;
                    }
                }
            }
        }

		public void AddWarning(string format, params object[] args)
        {
            r_warnings.Add(string.Format(format, args));
        }

        public void Save()
        {
            SaveAs(m_solutionFullName);
        }

        public void SaveAs(string solutionPath)
        {
            using (var writer = new SolutionFileWriter(solutionPath))
            {
                writer.WriteSolutionFile(this);
            }
        }

        public NodeDifference CompareTo(SolutionFile oldSolution)
        {
            return (NodeDifference) this.ToElement().CompareTo(oldSolution.ToElement());
        }

        public NodeConflict Validate(out List<string> messages)
        {
            // TODO Finish this.
            messages = new List<string>();

            var projectsByFullName = new Dictionary<string, Project>(StringComparer.InvariantCultureIgnoreCase);
            var acceptedDifferences = new List<Difference>();
            var conflicts = new List<Conflict>();
            foreach (Project project in this.Projects)
            {
                Project otherProject;
                if (projectsByFullName.TryGetValue(project.ProjectFullName, out otherProject))
                {
                    acceptedDifferences.Add(
                                new NodeDifference(
                                    new ElementIdentifier(
                                        TagProject + project.ProjectGuid,
                                        string.Format("Project \"{0}\" [{1}]", project.ProjectFullName, project.ProjectGuid)),
                                    OperationOnParent.Removed,
                                    null));

                    var otherProjectIdentifier = new ElementIdentifier(
                                        TagProject + otherProject.ProjectGuid,
                                        string.Format("Project \"{0}\" [{1}]", otherProject.ProjectFullName, otherProject.ProjectGuid));

                    conflicts.Add(
                                Conflict.Merge(
                                    new NodeElement(otherProjectIdentifier, null),
                                    otherProject.ToElement(otherProjectIdentifier),
                                    project.ToElement(otherProjectIdentifier)));
                }
                else
                {
                    projectsByFullName.Add(project.ProjectFullName, project);
                }
            }

            return new NodeConflict(new ElementIdentifier("SolutionFile"), OperationOnParent.Modified, acceptedDifferences, conflicts);
        }

		#region Methods ToElement / FromElement

		private const string TagHeader = "Header";
        private const string TagSolutionFolderGuids = "SolutionFolderGuids";
        private const string TagProject = "P_";
        private const string TagSolutionFolder = "SF_";
        private const string TagGlobalSection = "GS_";

        public NodeElement ToElement()
        {
            var childs = new List<Element>
                        {
                            new ValueElement(
                                        new ElementIdentifier(TagHeader),
                                        String.Join("|", this.Headers.ToArray()))
                        };

            var solutionFoldersElements = new List<Element>();
            foreach (Project project in this.Projects)
            {
                if (project.ProjectTypeGuid == KnownProjectTypeGuid.SolutionFolder)
                {
                    childs.Add(
                                project.ToElement(
                                    new ElementIdentifier(
                                        TagSolutionFolder + project.ProjectFullName,
                                        string.Format("SolutionFolder \"{0}\"", project.ProjectFullName))));
                    solutionFoldersElements.Add(
                                new ValueElement(
                                    new ElementIdentifier(project.ProjectFullName),
                                    project.ProjectGuid));
                }
                else
                {
                    childs.Add(
                                project.ToElement(
                                    new ElementIdentifier(
                                        TagProject + project.ProjectGuid,
                                        string.Format("Project \"{0}\"", project.ProjectFullName))));
                }
            }
            childs.Add(new NodeElement(
                            new ElementIdentifier(TagSolutionFolderGuids),
                            solutionFoldersElements));

            foreach (var globalSection in this.GlobalSections)
            {
                childs.Add(
                            globalSection.ToElement(
                                new ElementIdentifier(
                                    TagGlobalSection + globalSection.Name,
                                    string.Format("GlobalSection \"{0}\"", globalSection.Name))));
            }
            return new NodeElement(
                        new ElementIdentifier("SolutionFile"),
                        childs);
        }

        public static SolutionFile FromElement(NodeElement element)
        {
            var headers = new string[0];
            var projects = new ProjectList();
            var globalSections = new List<Section>();

            var solutionFolderGuids = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (ValueElement solutionGuid in ((NodeElement)element.Childs[new ElementIdentifier(TagSolutionFolderGuids)]).Childs)
            {
                solutionFolderGuids.Add(solutionGuid.Identifier.Name, solutionGuid.Value);
            }

            foreach (var child in element.Childs)
            {
                var identifier = child.Identifier;
                if (identifier.Name == TagHeader)
                {
                    headers = ((ValueElement)child).Value.Split('|');
                }
                else if (identifier.Name == TagSolutionFolderGuids)
                {
                    // Ignore it because we already handled it above
                }
                else if (identifier.Name.StartsWith(TagProject))
                {
                    var projectGuid = identifier.Name.Substring(TagProject.Length);
                    projects.AddWithGuid(projectGuid, Project.FromElement(projectGuid, (NodeElement)child, solutionFolderGuids));
                }
                else if (identifier.Name.StartsWith(TagSolutionFolder))
                {
                    var projectFullPath = identifier.Name.Substring(TagSolutionFolder.Length);
                    if (!solutionFolderGuids.ContainsKey(projectFullPath))
                    {
                        throw new Exception("TODO");
                    }
					string projectGuid = solutionFolderGuids[projectFullPath];
					projects.AddWithGuid(projectGuid, Project.FromElement(projectGuid, (NodeElement)child, solutionFolderGuids));
                }
                else if (identifier.Name.StartsWith(TagGlobalSection))
                {
                    var sectionName = identifier.Name.Substring(TagGlobalSection.Length);
                    globalSections.Add(Section.FromElement(sectionName, (NodeElement)child));
                }
                else
                {
                    throw new SolutionFileException(string.Format("Invalid identifier '{0}'.", identifier.Name));
                }
            }
            return new SolutionFile(null, headers, projects, globalSections);
        }

		#endregion

		public string FullPath
		{
			get
			{
				if (this.m_solutionFullName == null)
				{
					return string.Empty;
				}
				FileInfo f = new FileInfo(this.m_solutionFullName);
				return f.DirectoryName;
			}
		}

		public IProjectContext ProjectContext
		{
			get
			{
				return this;
			}
		}

		IEnumerable<Project> IProjectContext.Projects
		{
			get
			{
				if (m_projectContext == null)
				{
					return this.Projects;
				}
				return m_projectContext.Projects;
			}
		}

		public Project LoadProject(string path)
		{
			Project p;
			if (m_projectContext == null)
			{
				p = new Project(this, path);
			}
			else
			{
				string projectFullPath = FsPath.Combine(this.FullPath, path);
				p = m_projectContext.LoadProject(projectFullPath);
			}
			return p;
		}

		public string GetGuidForProject(Project project)
		{
			throw new NotImplementedException();
		}
	}
}
