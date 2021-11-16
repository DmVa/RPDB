using RPDB.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Services
{
    public class ScriptsScanner
    {
        public ObservableCollection<ScriptData> Collect()
        {
            var localScripts = new List<ScriptFile>();
            var registered = new Dictionary<string,RegisteredScript>();
            using (var dbcontext = new DataContext())
            {
                var databases = dbcontext.Databases.ToDictionary(x => x.Id);
                var folders = dbcontext.SearchFolders.OrderBy(x => x.SortOrder).ToList();
                foreach (var folder in folders)
                {
                    var di = new DirectoryInfo(folder.Path);
                    Collect(di, folder.SearchMask, folder.IncludeSubfolders, folder.Database, localScripts);
                }

                registered = dbcontext.RegisteredScripts.Include("Database").ToDictionary(x=>x.Path+"\\"+x.Name);
            }

            ObservableCollection<ScriptData> merged = MergeScripts(localScripts, registered);

            return merged;
        }

        private ObservableCollection<ScriptData> MergeScripts(List<ScriptFile> localScripts, Dictionary<string, RegisteredScript> registered)
        {
            var collected = new ObservableCollection<ScriptData>();
            foreach(var script in localScripts)
            {
                var data = new ScriptData();
                data.FileData = script;
                RegisteredScript registeredScript;
                registered.TryGetValue(script.FullFileName, out registeredScript);
                data.Registered = registeredScript;
                if (data.Registered == null)
                {
                    data.Status = ScriptStatus.New;
                }
                else
                {
                    double seconds = Math.Abs ((script.FileDate - data.Registered.FileTime.Value).TotalSeconds);
                    if (seconds<=1 && script.FileSize == data.Registered.FileSize)
                    {
                        data.Status = ScriptStatus.Unchanged;
                    }
                    else
                    {
                        data.Status = ScriptStatus.Modified;
                    }
                }
                collected.Add(data);
            }
            return collected;
        }

        private void Collect(DirectoryInfo di, string searchMask, bool includeSubfolders, Database database, IList<ScriptFile> collected)
        {
            if (string.IsNullOrEmpty(searchMask))
                searchMask = "*";
            var masks = searchMask.Split(';');
            IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo>();
            foreach (var mask in masks)
            {
                if (!string.IsNullOrEmpty(mask))
                   files = files.Concat(di.GetFiles(mask).OrderBy(x => x.Name).ToArray());
            }

            foreach (var fileEntry in files)
            {
                CollectFile(fileEntry, database, collected);
            }

            // Recurse into subdirectories of this directory.
            if (includeSubfolders)
            {
                var subdirectoryEntries = di.GetDirectories();
                foreach (var subdirectory in subdirectoryEntries)
                    Collect(subdirectory, searchMask, includeSubfolders, database, collected);
            }
        }


        private void CollectFile(FileInfo fileInfo, Database database, IList<ScriptFile> collected)
        {
            var fileData = new ScriptFile();
            fileData.FullFileName = fileInfo.FullName;
            fileData.Path = fileInfo.DirectoryName;
            fileData.FileName = fileInfo.Name;
            fileData.ExpectedDatabaseId = database?.Id ?? 0;
            fileData.ExpectedDatabaseName = database?.Name ?? "";
            fileData.FileDate = fileInfo.LastWriteTime;
            fileData.FileSize = fileInfo.Length;
            collected.Add(fileData);
        }
    }
}
