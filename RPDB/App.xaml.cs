using log4net;
using RPDB.Data;
using RPDB.Init;
using RPDB.Logs;
using RPDB.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RPDB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {

            ApplicationSettings.Current.Logger = new Log4NetWrapper();
            ApplicationSettings.Current.AutoClose = false;

            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, RPDB.Migrations.Configuration>());
            var initializer = new Initializer();
           // initializer.SaveSettings();
            initializer.LoadInitialSettings();
            initializer.SyncFolderDefinitionsIfRequired();
            var cmd = new CommandLine(e.Args);
            var errors = new List<string>();
            cmd.AddCommand("run", ()=> { var actions = new CommandLineActions(); actions.Run(); errors.AddRange(actions.Errors); });
            if (cmd.IsValid)
            {
               cmd.Execute();
                if (errors.Count == 0)
                {
                    Environment.Exit(0);
                }
                else
                {
                    foreach (var err in errors)
                    {
                        Console.WriteLine(err);
                    }
                    Environment.Exit(errors.Count);
                }
            }
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string message = e.Exception.Message;
            if (e.Exception.InnerException != null)
                 message = e.Exception.InnerException.InnerException != null ? e.Exception.InnerException.InnerException.Message : e.Exception.InnerException.Message;

            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ApplicationSettings.Current.Logger.LogError("unhandled", e.Exception);
            e.Handled = true;
          
        }
    }
}
