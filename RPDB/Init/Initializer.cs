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

        public void LoadSettings()
        {
            if (!File.Exists(_settingsFile))
                return;

            var json = File.ReadAllText(_settingsFile);
            InitialData data = JsonConvert.DeserializeObject<InitialData>(json);
            if (data == null)
                return;

            ImportToDb(data);
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
                    context.AppSettings.Add(data.AppSetting);
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
    }
}
