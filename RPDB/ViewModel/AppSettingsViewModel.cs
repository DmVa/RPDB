using RPDB.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RPDB.ViewModel
{
    public class AppSettingsViewModel : BasePropertyChanged
    {
        private BaseCommand _closeCommand = null;
        private BaseCommand _saveCommand = null;
        private ObservableCollection<AppSettingModel> _settings = null;
        public Window Window { get; set; }
        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand;
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand;
            }
        }

        public ObservableCollection<AppSettingModel> SettingsList
        {
            get { return _settings; }
            set { _settings = value; RaisePropertyChanged(nameof(SettingsList)); } 
        }
        

        public void LoadData()
        {
            using(var dbcontext= new DataContext())
            {
                var settingsList = new ObservableCollection<AppSettingModel>();

                var settings = dbcontext.AppSettings.ToList();
                foreach(var setting in settings)
                {
                    settingsList.Add(new AppSettingModel() { Id = (int)setting.Id, Value = setting.Value, SettingName = setting.Id.ToString() });
                }

                SettingsList = settingsList;
            }
        }

        public AppSettingsViewModel()
        {
            _closeCommand = new BaseCommand(()=> { Close(false); });
            _saveCommand = new BaseCommand(()=> { Close(true); });
        }

        private void Close(bool save)
        {
            if (save)
            {
                using (var dbcontext = new DataContext())
                {
                    var settings = dbcontext.AppSettings.ToList();
                    foreach (var setting in settings)
                    {
                        var newValue = _settings.FirstOrDefault(x => (AppSettingEnum)x.Id == setting.Id);
                        if (newValue != null)
                        {
                            setting.Value = newValue.Value;
                        }
                    }

                    dbcontext.SaveChanges();
                }
            }

            this.Window.DialogResult = save;
            this.Window.Close();
        }
    }

    public class AppSettingModel
    {
        public int Id { get; set; }
        public string SettingName { get; set; }
        public string Value { get; set; }
    }
}
