using RPDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Init
{
    public class InitialData
    {
        public List<AppSetting> AppSetting { get; set; }
        public ServerSetting ServerSetting { get; set; }
        public List<Database> Databases { get; set; }
        public List<SearchFolderInit> SerachFolders { get; set; }
        public InitialData CreateDefault()
        {
            var result = new InitialData();
            result.AppSetting = new List<AppSetting>();
            result.ServerSetting = new ServerSetting() { IsWindowsAuth = true, Name = "." };
            result.AppSetting.Add(new AppSetting() { Id = AppSettingEnum.CompareCommand, Value = "WinMergeU.exe /u /dl registered \"%1\" /dr \"%2\" \"%2\"" });
            result.AppSetting.Add(new AppSetting() { Id = AppSettingEnum.AutosyncFoldersFrom, Value = @"\\azwesv0025\Tools\folders.rpdb" });
            result.Databases = new List<Database>();
            result.Databases.Add(new Database() { Name = "C1App", Alias = "$(C1App)" });
            result.Databases.Add(new Database() { Name = "Warehouse", Alias = "$(Warehouse)" });
            result.Databases.Add(new Database() { Name = "Workflow", Alias = "$(Workflow)" });
            result.Databases.Add(new Database() { Name = "Bpm", Alias = "$(Bpm)" });
            result.Databases.Add(new Database() { Name = "CPP", Alias = "$(CPP)" });
            result.Databases.Add(new Database() { Name = "Data", Alias = "$(Data)" });
            result.Databases.Add(new Database() { Name = "Hexacom", Alias = "$(Hexacom)" });
            result.Databases.Add(new Database() { Name = "Str", Alias = "$(Str)" });
            result.SerachFolders = new List<SearchFolderInit>();
            result.SerachFolders.Add(new SearchFolderInit() { Path = @"c:\Source\c1\DbProjects\DB\Workflow", IncludeSubfolders = true, DatabaseName= "Workflow", SortOrder = 1 });
            result.SerachFolders.Add(new SearchFolderInit() { Path = @"c:\Source\c1\DbProjects\DB\Bpm", IncludeSubfolders = true, DatabaseName = "Bpm", SortOrder = 2 });
            result.SerachFolders.Add(new SearchFolderInit() { Path = @"c:\Source\c1\DbProjects\DB\C1App", IncludeSubfolders = true, DatabaseName = "C1App", SortOrder = 3 });
            result.SerachFolders.Add(new SearchFolderInit() { Path = @"c:\Source\c1\DbProjects\DB\STR", IncludeSubfolders = true, DatabaseName = "STR",  SortOrder = 4 });
            result.SerachFolders.Add(new SearchFolderInit() { Path = @"c:\Source\c1\DbProjects\DB\Warehouse", IncludeSubfolders = true, DatabaseName = "Warehouse",  SortOrder = 5 });
            result.SerachFolders.Add(new SearchFolderInit() { Path = @"c:\Source\c1\install\resources\After_Creation\C1App", IncludeSubfolders = true, DatabaseName = "C1App", SortOrder = 6 });
            result.SerachFolders.Add(new SearchFolderInit() { Path = @"c:\Source\c1\install\resources\After_Creation\STR", IncludeSubfolders = true, DatabaseName = "Str", SortOrder = 7 });
            result.SerachFolders.Add(new SearchFolderInit() { Path = @"c:\Source\c1\install\resources\After_Creation\Workflow", IncludeSubfolders = true, DatabaseName = "Workflow",  SortOrder = 8 });


            return result;
        }
    }
}
