using Newtonsoft.Json;
using RPDB.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Init
{
    public class Initializer
    {
        private readonly string _settingsFile = "settings.json";
        public void SaveSettings()
        {
            var data = (new InitialData()).CreateDefault();
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_settingsFile, json);
        }

        public void LoadInitialSettings()
        {
            try
            {
                if (!File.Exists(_settingsFile))
                    return;

                var json = File.ReadAllText(_settingsFile);
                InitialData data = JsonConvert.DeserializeObject<InitialData>(json);
                if (data == null)
                    return;

                ImportToDb(data);
            }
            catch(Exception e)
            {
                ApplicationSettings.Current.Logger.LogError("unhandled in load settings", e);
            }
        }

        internal void SyncFolderDefinitionsIfRequired()
        {
            try
            {
                string fileToSync = null;
                using (var context = new DataContext())
                {
                    var setting = context.AppSettings.Where(x => x.Id == AppSettingEnum.AutosyncFoldersFrom).FirstOrDefault();
                    fileToSync = setting?.Value;
                }
                if (string.IsNullOrEmpty(fileToSync))
                    return;
                if (!File.Exists(fileToSync))
                    return;

                ImportSearchFolders(fileToSync);
            }
            catch (Exception e)
            {
                ApplicationSettings.Current.Logger.LogError("unhandled in sync folder definitions", e);
            }
        }

        private void ImportToDb(InitialData data)
        {
            using(var context = new DataContext())
            {
                if (context.ServerSettings.Any())
                {
                    return;
                }

                context.ServerSettings.Add(data.ServerSetting);
                context.SaveChanges();

                if (!context.AppSettings.Any())
                {
                    context.AppSettings.AddRange(data.AppSetting);
                    context.SaveChanges();
                }

                if (!context.Databases.Any())
                {
                    foreach (var db in data.Databases)
                    {
                        context.Databases.Add(db);
                    }
                    context.SaveChanges();
                }

                if (!context.SearchFolders.Any())
                {
                    foreach (var sfd in data.SerachFolders)
                    {
                        var db = context.Databases.FirstOrDefault(x => x.Name == sfd.DatabaseName);
                        if (db == null)
                            continue;
                        var sf = new SearchFolder();
                        
                        sf.DatabaseId = db.Id;
                        sf.IncludeSubfolders = sfd.IncludeSubfolders;
                        sf.Path = sfd.Path;
                        sf.SortOrder = sfd.SortOrder;
                        
                        context.SearchFolders.Add(sf);
                    }
                    context.SaveChanges();
                }
            }
        }

        public void ExportSearchFolders(string fileName)
        {
            using (var context = new DataContext())
            {
                var model = new SearchFolderExportModel();
                model.Folders = context.SearchFolders.Include("Database").ToList();

                var json = JsonConvert.SerializeObject(model, Formatting.Indented);
                File.WriteAllText(fileName, json);
            }
        }

        public void ImportSearchFolders(string fileName)
        {
            var json = File.ReadAllText(fileName);
            SearchFolderExportModel data = JsonConvert.DeserializeObject<SearchFolderExportModel>(json);
            if (data.Folders == null)
            {
                throw new ApplicationException("Data not defined");
            }

            EnusureDatabasesDefined(data);
            using (var context = new DataContext())
            {
                foreach (var item in data.Folders)
                {

                    ImportSearchFolder(item, context);
                }
                context.SaveChanges();
            }
        }

        private void EnusureDatabasesDefined(SearchFolderExportModel data)
        {
            using (var context = new DataContext())
            {
                foreach (var item in data.Folders)
                {
                    var database = context.Databases.Where(x => x.Name == item.Database.Name).First();
                    if (database == null)
                    {
                        database = new Database();
                        database.Name = item.Database.Name;
                        context.Databases.Add(database);
                    }
                    database.Alias = item.Database.Alias;
                }

                context.SaveChanges();
            }
        }

        private void ImportSearchFolder(SearchFolder item, DataContext context)
        {
            var sf = context.SearchFolders.Where(x => x.Path == item.Path).FirstOrDefault();
            if (sf == null)
            {
                sf = new SearchFolder();
                context.SearchFolders.Add(sf);
            }

            sf.DatabaseId = context.Databases.Where(x => x.Name == item.Database.Name).Select(x => x.Id).First();
            sf.Path = item.Path;
            sf.IncludeSubfolders = item.IncludeSubfolders;
            sf.SortOrder = item.SortOrder;
            context.SaveChanges();
        }
    }

    public class SearchFolderExportModel
    {
        public List<SearchFolder> Folders { get; set; }
    }
}
