using RPDB.Data;
using RPDB.Services;
using RPDB.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RPDB.ViewModel
{
    public class MainWindowViewModel: BasePropertyChanged
    { 
        private BaseCommand _serverSettingsCommand;
        private BaseCommand _appSettingsCommand;
        private BaseCommand _databasesSettingsCommand;
        private BaseCommand _searchFoldersCommand;
        private BaseCommand _scanChangesCommand;
        private BaseCommand _loadSelectedScriptCommand;
        private BaseCommand _clearLogCommand;
        private BaseCommand _registerScriptCommand;
        private BaseCommand _registerAllScriptsCommand;
        private BaseCommand _unregisterAllScriptsCommand;
        private BaseCommand _runScriptCommand;
        private BaseCommand _compareCommand;

        private bool _isLoading;
        private string _updateLog;
        private ObservableCollection<Data.Database> _databases;
        private Data.Database _selectedDatabase;
        private bool _isShowUnchanged;
        private ObservableCollection<ScriptData> _allScripts;
        private ObservableCollection<ScriptData> _scripts;
        private ScriptData _selectedScript;
        private string _scriptText;
        private ScriptRunner _scriptRunner;

        public MainWindowViewModel()
        {
            _scriptRunner = new ScriptRunner();
            CreateCommands();
        }
        public Window Window { get; set; }

        #region "Commands"
        public ICommand ServerSettingsCommand
        {
            get { return _serverSettingsCommand; }
        }
        public ICommand AppSettingsCommand
        {
            get { return _appSettingsCommand; }
        }
        public ICommand DatabasesSettingsCommand
        {
            get { return _databasesSettingsCommand; }
        }
        public ICommand SearchFoldersCommand
        {
            get { return _searchFoldersCommand; }
        }
        public ICommand ScanChangesCommand
        {
            get { return _scanChangesCommand; }
        }

        public ICommand LoadSelectedScriptCommand
        {
            get { return _loadSelectedScriptCommand; }
        }
        public ICommand ClearLog
        {
            get { return _clearLogCommand; }
        }
        public ICommand RegisterScriptCommand
        {
            get { return _registerScriptCommand; }
        }
        public ICommand UnRegisterAllScriptsCommand
        {
            get { return _unregisterAllScriptsCommand; }
        }
        public ICommand RegisterAllScriptsCommand
        {
            get { return _registerAllScriptsCommand; }
        }
        public ICommand RunScriptCommand
        {
            get { return _runScriptCommand; }
        }
        public ICommand CompareCommand
        {
            get { return _compareCommand; }
        }


        #endregion
        #region "Binded Properties"
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(nameof(IsLoading));
            }
        }

        public bool IsShowUnchanged
        {
            get { return _isShowUnchanged; }
            set
            {
                _isShowUnchanged = value;
                RaisePropertyChanged(nameof(IsShowUnchanged));
                FilterShownScripts();
            }
        }
        public ObservableCollection<Data.Database> DatabasesList
        {
            get { return _databases; }
            set { _databases = value; RaisePropertyChanged(nameof(DatabasesList)); }
        }

        public Data.Database SelectedDatabase
        {
            get { return _selectedDatabase; }
            set { _selectedDatabase = value; RaisePropertyChanged(nameof(SelectedDatabase)); }
        }

        public ObservableCollection<ScriptData> Scripts
        {
            get { return _scripts; }
            set { 
                _scripts = value;
                RaisePropertyChanged(nameof(Scripts));
            }
        }

        public string UpdateLog
        {
            get { return _updateLog; }
            set { _updateLog = value; RaisePropertyChanged(nameof(UpdateLog)); }
        }

        public ScriptData SelectedScript
        {
            get { return _selectedScript; }
            set
            {
                _selectedScript = value;
                if (value != null)
                {
                    LoadSelectedScript();
                }
                RaisePropertyChanged(nameof(SelectedScript));
            }
        }

        public string ScriptText
        {
            get { return _scriptText; }
            set { _scriptText = value; RaisePropertyChanged(nameof(ScriptText)); }
        }
        #endregion

        #region "Methods"
        public void LoadInitdata()
        {
            try
            {
                LoadDatabases();
            }
            catch (Exception ex)
            {
                AddLog("Databases initialization fail" + ex.Message);
            }
            
            try
            {
                DoScanChanges();
            }
            catch(Exception ex)
            {
                AddLog("Initial scan fail" + ex.Message);
            }
        }

        private void LoadDatabases()
        {
            var databases = new ObservableCollection<Data.Database>();
            using (var dbcontext = new DataContext())
            {
                foreach (var dataBase in dbcontext.Databases.ToList())
                {
                    databases.Add(dataBase);
                }
            }

            DatabasesList = databases;
        }

        private void CreateCommands()
        {
            _clearLogCommand = new BaseCommand(() => UpdateLog = "");
            _serverSettingsCommand = new BaseCommand(ShowServerSettings, CanExecuteAction);
            _appSettingsCommand = new BaseCommand(ShowAppSettings, CanExecuteAction);
            _databasesSettingsCommand = new BaseCommand(ShowDatabasesSettings, CanExecuteAction);
            _searchFoldersCommand = new BaseCommand(ShowSearchFoldersSettings, CanExecuteAction);
            _scanChangesCommand = new BaseCommand(DoScanChanges, CanExecuteAction);
            _loadSelectedScriptCommand = new BaseCommand(LoadSelectedScript, CanExecuteAction);

            _registerScriptCommand = new BaseCommand(DoRegisterScript, CanExecuteAction);
            _registerAllScriptsCommand = new BaseCommand(DoRegisterAllScripts, CanExecuteAction);
            _unregisterAllScriptsCommand = new BaseCommand(DoUnregisterAllScripts, CanExecuteAction);
            _runScriptCommand = new BaseCommand(DoRunScript, CanExecuteAction);
            _compareCommand = new BaseCommand(DoCompare);
        }

        private void DoUnregisterAllScripts()
        {
             _scriptRunner.UnRegisterAllScripts();
            AddLog("registration cleared"); 
            DoScanChanges();
        }

        private void DoCompare()
        {
            if (SelectedScript == null)
            {
                AddLog($"Script not selected");
                return;
            }

            if (SelectedScript.Registered == null)
            {
                AddLog($"Script not registered");
                return;
            }

            if (string.IsNullOrEmpty(SelectedScript.Registered.Text))
            {
                AddLog($"Script text was not saved");
                return;
            }
            string compareProcess = "";
            using (var dbContext = new DataContext())
            {
                compareProcess = dbContext.AppSettings.Where(x => x.Id == AppSettingEnum.CompareCommand).Select(x => x.Value).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(compareProcess))
            {
                AddLog($"compare command not defined");
                return;
            }

            if (!compareProcess.Contains("%1") || !compareProcess.Contains("%2"))
            {
                AddLog($"compare command bad format, expected <appname> \"%1\" \"%2\"");
                AddLog($"<appname> \"%1\" \"%2\"");
                AddLog($"\"%1\" - temp registered file \"%2\" - actual file on disk");
                return;
            }



            string temp_file = "tmpRegistered_" + SelectedScript.Registered.Name;
            if (File.Exists(temp_file))
                File.Delete(temp_file);

            File.WriteAllText(temp_file, SelectedScript.Registered.Text);
            string[] compareCommand = compareProcess.Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries);
            if (compareCommand.Length == 0)
            {
                AddLog($"cannot understand compare command");
            }

            string command = compareProcess.Substring(0, compareCommand[0].Length).Trim();
            var argumentsIndex = command.Length;
            string arguments = compareProcess.Substring(argumentsIndex).Trim();

            arguments = arguments.Replace("%1", temp_file);
            arguments = arguments.Replace("%2", SelectedScript.FileData.FullFileName);

            AddLog($"Executing: {command} {arguments} ");

            var process = new Process();
           
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.EnableRaisingEvents = true;
            process.Exited += (s, e) =>
            {
                if (File.Exists(temp_file))
                    File.Delete(temp_file);
            };

            process.Start();

        }

        private void CompareProcess_Exited(object sender, EventArgs e)
        {
          
        }

        private void DoRunScript()
        {
            if (SelectedScript == null)
            {
                AddLog($"Script not selected");
                return;
            }
            var sp = new Stopwatch();
            sp.Start();
            try
            {
                _scriptRunner.RunScript(SelectedScript, SelectedDatabase?.Id ?? 0);
            }
            catch(Exception ex)
            {
                AddLog("Execution error: ");
                AddLog(ex.Message);
                throw;
            }

            sp.Stop();
            AddLog($"Script Executed, time {sp.Elapsed.TotalSeconds} sec");
            SelectedScript.RaisePropertyChanged();
        }

        private void DoRegisterScript()
        {
            if (SelectedScript == null)
            {
                AddLog($"Script not selected");
                return;
            }

            var sp = new Stopwatch();
            sp.Start();
            _scriptRunner.RegisterWithoutRun(SelectedScript, SelectedDatabase?.Id ?? 0);
            sp.Stop();
            AddLog($"Script registered time {sp.Elapsed.TotalSeconds} sec");
            SelectedScript.RaisePropertyChanged();
        }

        private void DoRegisterAllScripts()
        {
            if (Scripts == null)
            {
                AddLog($"nothing to register");
                return;
            }

            AddLog($"Registration started");
            var sp = new Stopwatch();
            sp.Start();
            IsLoading = true;
            var worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                _scriptRunner.RegisterAllWithoutRun(Scripts);
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                IsLoading = false;
                sp.Stop();
                AddLog($"Script registered time {sp.Elapsed.TotalSeconds} sec");
                DoScanChanges();

            };
            worker.RunWorkerAsync();

        }

        private void LoadSelectedScript()
        {
            if (SelectedScript == null)
                throw new ApplicationException("Script not selected");
            var sp = new Stopwatch();
            sp.Start();
            string text = File.ReadAllText(SelectedScript.FileData.FullFileName);
            sp.Stop();
            AddLog($"script loaded {sp.Elapsed.TotalSeconds} sec");
            ScriptText = text;
            var database = DatabasesList.FirstOrDefault(x => x.Id == SelectedScript.ExpectedDatabaseId);
            SelectedDatabase = database;
        }

        private void AddLog(string log)
        {
            if (!string.IsNullOrEmpty(_updateLog))
                _updateLog += Environment.NewLine;
            UpdateLog += log;
        }

        private void DoScanChanges()
        {
            IsLoading = true;
            var sp = new Stopwatch();
            sp.Start();
            var sc = new ScriptsScanner();
            var worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                _allScripts = sc.Collect();
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                IsLoading = false;
                FilterShownScripts();
                sp.Stop();
                AddLog($"Scan duration {sp.Elapsed.TotalSeconds} sec, files: {_allScripts.Count}");
            };
            worker.RunWorkerAsync();
        }

        private void FilterShownScripts()
        {
            if (_allScripts == null)
                return;

            if (_isShowUnchanged)
                Scripts = _allScripts;
            else
                Scripts = new ObservableCollection<ScriptData>(_allScripts.Where(x => x.Status != ScriptStatus.Unchanged).ToList());
        }

        private bool CanExecuteAction()
        {
            return !_isLoading;
        }

        private void ShowServerSettings()
        {
            var settingsWindow = new View.ServerSetting();
            settingsWindow.Owner = Window;
            var dlgResult = settingsWindow.ShowDialog();
        }

        private void ShowAppSettings()
        {
            var settingsWindow = new View.AppSetting();
            settingsWindow.Owner = Window;
            var dlgResult = settingsWindow.ShowDialog();
        }

        private void ShowDatabasesSettings()
        {
            var settingsWindow = new DatabasesWindow();
            settingsWindow.Owner = Window;
            var dlgResult = settingsWindow.ShowDialog();
            if (dlgResult == true)
            {
                SelectedDatabase = null;
                LoadDatabases();
            }
        }

        private void ShowSearchFoldersSettings()
        {
            var settingsWindow = new SearchFoldersWindow();
            settingsWindow.Owner = Window;
            var dlgResult = settingsWindow.ShowDialog();
        }
        #endregion
    }
}
