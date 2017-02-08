using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MetaSpecTools
{
    public class SolutionFileWriter : IDisposable
    {
        private StreamWriter m_writer;

        public SolutionFileWriter(string solutionFullPath)
            : this(new FileStream(solutionFullPath, FileMode.Create, FileAccess.Write))
        {
        }

        public SolutionFileWriter(Stream writer)
        {
            m_writer = new StreamWriter(writer, Encoding.UTF8);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_writer != null)
            {
                m_writer.Dispose();
                m_writer = null;
            }
        }

        #endregion

        public void WriteSolutionFile(SolutionFile solutionFile)
        {
            lock (m_writer)
            {
                WriteHeader(solutionFile);
                WriteProjects(solutionFile);
                WriteGlobal(solutionFile);
            }
        }

        private void WriteHeader(SolutionFile solutionFile)
        {
            // If the header doesn't start with an empty line, add one
            // (The first line of sln files saved as UTF-8 with BOM must be blank, otherwise Visual Studio Version Selector will not detect VS version correctly.)
            if (solutionFile.Headers.Count == 0 || solutionFile.Headers[0].Trim().Length > 0)
            {
                m_writer.WriteLine();
            }

            foreach (var line in solutionFile.Headers)
            {
                m_writer.WriteLine(line);
            }
        }

        private void WriteProjects(SolutionFile solutionFile)
        {
            foreach (var project in solutionFile.Projects)
            {
                m_writer.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
                            project.ProjectTypeGuid,
                            project.ProjectName,
                            project.RelativePath,
                            project.ProjectGuid);
                foreach (var projectSection in project.ProjectSections)
                {
                    WriteSection(projectSection, projectSection.PropertyLines);
                }
                m_writer.WriteLine("EndProject");
            }
        }

        private void WriteGlobal(SolutionFile solutionFile)
        {
            m_writer.WriteLine("Global");
            WriteGlobalSections(solutionFile);
            m_writer.WriteLine("EndGlobal");
        }

        private void WriteGlobalSections(SolutionFile solutionFile)
        {
            foreach (var globalSection in solutionFile.GlobalSections)
            {
                var propertyLines = new List<PropertyLine>(globalSection.PropertyLines);
                switch (globalSection.Name)
                {
                    case "NestedProjects":
                        foreach (var project in solutionFile.Projects)
                        {
                            if (project.ParentFolderGuid != null)
                            {
                                propertyLines.Add(new PropertyLine(project.ProjectGuid, project.ParentFolderGuid));
                            }
                        }
                        break;

                    case "ProjectConfigurationPlatforms":
                        foreach (var project in solutionFile.Projects)
                        {
                            foreach (var propertyLine in project.ProjectConfigurationPlatformsLines)
                            {
                                propertyLines.Add(
                                            new PropertyLine(
                                                string.Format("{0}.{1}", project.ProjectGuid, propertyLine.Name),
                                                propertyLine.Value));
                            }
                        }
                        break;

                    default:
                        if (globalSection.Name.EndsWith("Control", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var index = 1;
                            foreach (var project in solutionFile.Projects)
                            {
                                if (project.VersionControlLines.Count > 0)
                                {
                                    foreach (var propertyLine in project.VersionControlLines)
                                    {
                                        propertyLines.Add(
                                                    new PropertyLine(
                                                        string.Format("{0}{1}", propertyLine.Name, index),
                                                        propertyLine.Value));
                                    }
                                    index++;
                                }
                            }

                            propertyLines.Insert(0, new PropertyLine("SccNumberOfProjects", index.ToString()));
                        }
                        break;
                }

                WriteSection(globalSection, propertyLines);
            }
        }

        private void WriteSection(Section section, IEnumerable<PropertyLine> propertyLines)
        {
            m_writer.WriteLine("\t{0}({1}) = {2}", section.SectionType, section.Name, section.Step);
            foreach (var propertyLine in propertyLines)
            {
                m_writer.WriteLine("\t\t{0} = {1}", propertyLine.Name, propertyLine.Value);
            }
            m_writer.WriteLine("\tEnd{0}", section.SectionType);
        }
    }
}
