using System.Diagnostics;
using System;
using System.IO;
using System.Threading;

namespace MetaSpecTools.Filter
{
	using System.Collections.Generic;
	using Merge;

	internal class FilteredSolutionWatcher : ISolutionContext
    {
        private readonly AcceptDifferencesHandler r_acceptDifferencesHandler;
        private readonly FilterFile r_filterFile;
        private SolutionFile m_filteredSolution;
        private readonly FileSystemWatcher r_watcher;

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

		public FilteredSolutionWatcher(
                    AcceptDifferencesHandler handler,
                    FilterFile filterFile,
                    SolutionFile filteredSolution)
        {
            r_acceptDifferencesHandler = handler;
            r_filterFile = filterFile;
            m_filteredSolution = filteredSolution;

            r_watcher = new FileSystemWatcher
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    Path = Path.GetDirectoryName(m_filteredSolution.SolutionFullPath),
                    Filter = Path.GetFileName(m_filteredSolution.SolutionFullPath)
                };
            r_watcher.Changed += OnChanged;
        }

        public void Start()
        {
            lock (r_watcher)
            {
                r_watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            lock (r_watcher)
            {
                r_watcher.EnableRaisingEvents = false;
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            lock (r_watcher)
            {
                try
                {
                    WaitForFileToBeReleased(e.FullPath);

                    var newFilteredSolution = SolutionFile.FromFile(this, m_filteredSolution.SolutionFullPath);
                    var difference = newFilteredSolution.CompareTo(m_filteredSolution);
                    if (difference != null)
                    {
                        difference.Remove(diff => diff.Identifier.Name.Contains("SccProjectTopLevelParentUniqueName"));
                        if (difference.Subdifferences.Count > 0)
                        {
                            if (r_acceptDifferencesHandler(difference))
                            {
                                var newOriginalSolution = SolutionFile.FromElement((NodeElement)r_filterFile.SourceSolution.ToElement().Apply(difference));
                                newOriginalSolution.Save();
                                m_filteredSolution = newFilteredSolution;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
					Trace.WriteLine("OnChanged handler: " + ex.ToString());
                }
            }
        }

        private static void WaitForFileToBeReleased(string path)
        {
            if (!File.Exists(path))
                return;

            var start = DateTime.Now;
            do
            {
                try
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        return;
                    }
                }
                catch (IOException)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            } while (DateTime.Now - start < TimeSpan.FromSeconds(20));
        }
    }
}
