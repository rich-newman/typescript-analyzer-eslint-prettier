using Microsoft.VisualStudio.Shell.TableManager;
using System.Linq;
using System;
using System.Collections.Generic;

namespace TypeScriptAnalyzerEslintVsix
{
    class SinkManager : IDisposable
    {
        private readonly ITableDataSink sink;
        private readonly ErrorListDataSource errorList;
        private readonly List<TableEntriesSnapshot> snapshots = new List<TableEntriesSnapshot>();

        internal SinkManager(ErrorListDataSource errorList, ITableDataSink sink)
        {
            this.sink = sink;
            this.errorList = errorList;

            errorList.AddSinkManager(this);
        }

        internal void Clear()
        {
            sink.RemoveAllSnapshots();
        }

        internal void UpdateSink(IEnumerable<TableEntriesSnapshot> snapshots)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                foreach (var snapshot in snapshots)
                {
                    var existing = this.snapshots.FirstOrDefault(s => snapshot.FilePath.Equals(s.FilePath, StringComparison.OrdinalIgnoreCase));

                    if (existing != null)
                    {
                        this.snapshots.Remove(existing);
                        sink.ReplaceSnapshot(existing, snapshot);
                    }
                    else
                    {
                        sink.AddSnapshot(snapshot);
                    }

                    this.snapshots.Add(snapshot);
                }

            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        internal void RemoveSnapshots(IEnumerable<string> files)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                foreach (string file in files)
                {
                    var existing = snapshots.FirstOrDefault(s => file.Equals(s.FilePath, StringComparison.OrdinalIgnoreCase));

                    if (existing != null)
                    {
                        snapshots.Remove(existing);
                        sink.RemoveSnapshot(existing);
                    }
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        public void Dispose()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Called when the person who subscribed to the data source disposes of the cookie (== this object) they were given.
            errorList.RemoveSinkManager(this);
        }
    }
}
