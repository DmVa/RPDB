using RPDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RPDB.ViewModel
{
    public class ServerSettingsViewModel : BasePropertyChanged
    {
        private BaseCommand _closeCommand = null;
        private BaseCommand _saveCommand = null;
        private ServerSetting _serverSetting = null;
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

        public ServerSetting ServerSetting
        {
            get { return _serverSetting; }
            set { _serverSetting = value; RaisePropertyChanged(nameof(ServerSetting)); }
        }

        public void LoadData()
        {
            using(var dbcontext= new DataContext())
            {
                var settings = dbcontext.ServerSettings.FirstOrDefault();
                if (settings == null)
                    settings = new ServerSetting();

                ServerSetting = settings;
            }
        }

        public ServerSettingsViewModel()
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
                    var settings = dbcontext.ServerSettings.FirstOrDefault(x=>x.Id == _serverSetting.Id);
                    if (settings == null)
                    {
                        settings = _serverSetting;
                        dbcontext.ServerSettings.Add(settings);
                    }
                    else
                    {
                        dbcontext.Entry(settings).CurrentValues.SetValues(_serverSetting);
                        dbcontext.Entry(settings).State = System.Data.Entity.EntityState.Modified;
                    }

                    dbcontext.SaveChanges();
                }
            }

            this.Window.DialogResult = save;
            this.Window.Close();
        }
    }
}
