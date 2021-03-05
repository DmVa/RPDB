﻿using RPDB.Data;
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
    public class SearchFoldersWindowViewModel : BasePropertyChanged
    {
        private BaseCommand _closeCommand;
        private BaseCommand _saveCommand;
        private BaseCommand _deleteCommand;
        private ObservableCollection<SearchFolder> _folders;
        private SearchFolder _selectedFolder;
        private ObservableCollection<Database> _databases;
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

        public ObservableCollection<SearchFolder> FoldersList
        {
            get { return _folders; }
            set { _folders = value; RaisePropertyChanged(nameof(FoldersList)); } 
        }

        public SearchFolder SelectedFolder
        {
            get { return _selectedFolder; }
            set { _selectedFolder = value; RaisePropertyChanged(nameof(SelectedFolder)); }
        }
       
        public void LoadData()
        {
            using(var dbcontext= new DataContext())
            {
                var databases = new ObservableCollection<Database>();
                foreach (var dataBase in dbcontext.Databases.ToList())
                {
                    databases.Add(dataBase);
                }

                DatabasesList = databases;

                var dataList = new ObservableCollection<SearchFolder>();

                var data = dbcontext.SearchFolders.OrderBy(x => x.SortOrder).ToList();
                foreach(var dataBase in data)
                {
                    dataList.Add(dataBase);
                }

                FoldersList = dataList;
            }
        }

        public SearchFoldersWindowViewModel()
        {
            _closeCommand = new BaseCommand(()=> { Close(false); });
            _saveCommand = new BaseCommand(()=> { Close(true); });
            _deleteCommand = new BaseCommand(DeleteRecord);
        }

        private void DeleteRecord()
        {
            if (SelectedFolder == null || SelectedFolder.Id <= 0)
                return;

            using (var dbcontext = new DataContext())
            {
                var dbData = dbcontext.SearchFolders.FirstOrDefault(x => x.Id == SelectedFolder.Id);
                if (dbData != null)
                {
                    dbcontext.SearchFolders.Remove(dbData);
                }

                dbcontext.SaveChanges();
            }

            var dbDataModel = FoldersList.FirstOrDefault(x => x.Id == SelectedFolder.Id);
            if (dbDataModel != null)
                FoldersList.Remove(dbDataModel);
        }

        private void Close(bool save)
        {
            if (save)
            {
                using (var dbcontext = new DataContext())
                {
                    var dataList = dbcontext.SearchFolders.ToList();
                    foreach (var folderModel in FoldersList)
                    {
                        SearchFolder dbData;
                        if (folderModel.Id > 0)
                        {
                            dbData = dataList.FirstOrDefault(x => x.Id == folderModel.Id);
                        }
                        else
                        {
                            dbData = new SearchFolder();
                            dbcontext.SearchFolders.Add(dbData);
                        }
                        if (dbData != null)
                        {
                            dbData.IncludeSubfolders = folderModel.IncludeSubfolders;
                            dbData.Path = folderModel.Path;
                            dbData.SortOrder = folderModel.SortOrder;
                            dbData.DatabaseId = folderModel.DatabaseId;
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
