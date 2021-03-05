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
    public class DatabasesWindowViewModel : BasePropertyChanged
    {
        private BaseCommand _closeCommand;
        private BaseCommand _saveCommand;
        private BaseCommand _deleteCommand;
        private ObservableCollection<Database> _databases;
        private Database _selectedDatabase;
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
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand;
            }
        }

        public ObservableCollection<Database> DatabasesList
        {
            get { return _databases; }
            set { _databases = value; RaisePropertyChanged(nameof(DatabasesList)); } 
        }

        public Database SelectedDatabase
        {
            get { return _selectedDatabase; }
            set { _selectedDatabase= value; RaisePropertyChanged(nameof(SelectedDatabase)); }
        }
       
        public void LoadData()
        {
            using(var dbcontext= new DataContext())
            {
                var dataList = new ObservableCollection<Database>();

                var data = dbcontext.Databases.ToList();
                foreach(var dataBase in data)
                {
                    dataList.Add(dataBase);
                }

                DatabasesList = dataList;
            }
        }

        public DatabasesWindowViewModel()
        {
            _closeCommand = new BaseCommand(()=> { Close(false); });
            _saveCommand = new BaseCommand(()=> { Close(true); });
            _deleteCommand = new BaseCommand(DeleteRecord);
        }

        private void DeleteRecord()
        {
            if (SelectedDatabase == null || SelectedDatabase.Id <= 0)
                return;

            using (var dbcontext = new DataContext())
            {
                var dbData = dbcontext.Databases.FirstOrDefault(x => x.Id == SelectedDatabase.Id);
                if (dbData != null)
                {
                    dbcontext.Databases.Remove(dbData);
                }

                dbcontext.SaveChanges();
            }

            var dbDataModel = DatabasesList.FirstOrDefault(x => x.Id == SelectedDatabase.Id);
            if (dbDataModel != null)
                DatabasesList.Remove(dbDataModel);
        }

        private void Close(bool save)
        {
            if (save)
            {
                using (var dbcontext = new DataContext())
                {
                    var dataList = dbcontext.Databases.ToList();
                    foreach (var dataBaseModel in DatabasesList)
                    {
                        Database dbData;
                        if (dataBaseModel.Id > 0)
                        {
                            dbData = dataList.FirstOrDefault(x => x.Id == dataBaseModel.Id);
                        }
                        else
                        {
                            dbData = new Database();
                            dbcontext.Databases.Add(dbData);
                        }
                        if (dbData != null)
                        {
                            dbData.Name = dataBaseModel.Name;
                            dbData.Alias = dataBaseModel.Alias;
                        }
                    }
                    dbcontext.SaveChanges();
                }
            }

            this.Window.DialogResult = save;
            this.Window.Close();
        }
    }

   
}
